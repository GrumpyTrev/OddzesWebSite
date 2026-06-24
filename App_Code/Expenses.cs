using System.Collections.Generic;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Linq;

/// <summary>
/// The Expenses class encapsulates the expenses associated with a trip. 
/// It provides methods to determine the total expenses for a trip and the share of the expenses attributable to each user in the trip.
/// </summary>
public class Expenses
{
    public static Expenses GetExpensesForTrip(int tripId, int currencyId, int expensesRecordId = 0)
    {
        return new Expenses(tripId, currencyId, expensesRecordId);
    }

    public int TripId { get; set; }

    public int CurrencyId { get; set; }

    public double TotalExpenses { get; set; }

    public List<UserExpense> UserExpenses { get; set; }

    private Expenses(int tripId, int currencyId, int expensesRecordId )
    {
        TripId = tripId;
        CurrencyId = currencyId;
        UserExpenses = new List<UserExpense>();

        // Get all the transactions associated with the specified trip. Only consider those in the specified expense record
        List<Transaction> tripTransactions = TripModel.Transactions.
            Where(tran => ( tran.TripId == tripId ) && ( tran.ExpensesId == expensesRecordId ) ).ToList();

        // Add up all the expenses associated with the current trip and convert to the specified currency
        List<Transaction> expenses = tripTransactions.Where(tran => tran.IsBalance == false).ToList();
        TotalExpenses = expenses.Sum(trans => HelperMethods.ConvertAmount(trans.LocalValue, trans.CurrencyId, currencyId));

        // Get the list of users associated with the current trip
        IEnumerable<UserProfile> users = TripModel.UserProfiles.Join(TripModel.Trippers.Where(tripper => tripper.TripId == tripId),
            user => user.UserId, tripper => tripper.UserId, (user, tripper) => user);

        // Determine expenses for each user in the trip.
        foreach ( UserProfile user in users )
        {
            // Work out for each user how much they have paid
            double paid = expenses.Where(tran => ( tran.WhoId == user.UserId )).
                Sum(trans => HelperMethods.ConvertAmount(trans.LocalValue, trans.CurrencyId, currencyId));

            // Now work out the share of the expenses in each transaction attributable to this user.
            // For now assume an equal share
            List<Sharer> shares = TripModel.Sharers.Where(split => split.UserId == user.UserId).ToList();
            double userShare = expenses.Where(trans => shares.Any(share => share.TransactionId == trans.Id)).
                Sum(trans => HelperMethods.ConvertAmount(trans.LocalValue, trans.CurrencyId, currencyId) / ( int )trans.ShareCount);

            // Work out how much this user has overpaid or underpaid by taking any payments made or received into account
            double paymentsReceived = tripTransactions.Where(tran => ( tran.PaidToId == user.UserId ) && ( tran.IsBalance == true )).
                Sum(tran => HelperMethods.ConvertAmount(tran.LocalValue, tran.CurrencyId, currencyId));
            double paymentsMade = tripTransactions.Where(tran => ( tran.WhoId == user.UserId ) && ( tran.IsBalance == true )).
                Sum(tran => HelperMethods.ConvertAmount(tran.LocalValue, tran.CurrencyId, currencyId));

            double balance = paid - userShare + paymentsMade - paymentsReceived;

            UserExpenses.Add(new UserExpense { UserId = user.UserId, Paid = paid, Share = userShare, Balance = balance, UserName = user.Email,
                Payments = new List<BalancePayment>() });
        }

        // For each user that has underpaid determine how much is owed to those who have overpaid
        foreach ( UserExpense underPayer in UserExpenses.Where(user => user.Balance < -1.0) )
        {
            double amountOwed = -underPayer.Balance;
            foreach ( UserExpense overPayer in UserExpenses.Where(user => user.Balance > 1.0) )
            {
                if ( amountOwed > 0 )
                {
                    double amountToPay = System.Math.Min(amountOwed, overPayer.Balance);
                    overPayer.Balance -= amountToPay;
                    underPayer.Balance += amountToPay;
                    amountOwed -= amountToPay;

                    // Record this payment in the underPayer's record
                    underPayer.Payments.Add(new BalancePayment { ToUserId = overPayer.UserId, Amount = amountToPay, ToUserName = overPayer.UserName });
                }
            }
        }
    }
}

/// <summary>
/// The UserExpense class encapsulates the expenses attributable to a user in a trip. 
/// It contains the total amount paid by the user and the share of the expenses attributable to the user.
/// </summary>
public class UserExpense
{
    public int UserId { get; set; }
    public double Paid { get; set; }
    public double Share { get; set; }
    public string UserName { get; set; }

    /// <summary>
    /// This is the amount that the user has overpaid (positive) or underpaid (negative) after taking into account the share of the 
    /// expenses attributable to the user and any payments made or received by the user.
    /// </summary>
    public double Balance { get; set; }

    public List<BalancePayment> Payments { get; set; }
}

/// <summary>
/// The BalancePayment class encapsulates a possible payment to be made by a user to another user to balance the expenses for a trip.
/// </summary>
public class BalancePayment
{
    public int ToUserId { get; set; }

    public string ToUserName { get; set; }

    public double Amount { get; set; }
}