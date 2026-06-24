using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Security;
using WebMatrix.WebData;

public class HelperMethods
{

    /// <summary>
    /// Determine all the participants associated with a trip 
    /// </summary>
    /// <param name="tripId"></param>
    /// <returns></returns>
    static public List<string> TripParticipants(int tripId)
    {
        IEnumerable<int> participants = TripModel.Trippers.Where(tripper => tripper.TripId == tripId).Select(tripper => tripper.UserId);

        return ( participants.Count() == 0 ) ? new List<string>() :
            TripModel.UserProfiles.Where(user => participants.Contains(user.UserId)).Select(user => user.Email).ToList();
    }

    /// <summary>
    /// Determine all the participants associated with a trip, with the current user first (if they are a participant)
    /// </summary>
    /// <param name="tripId"></param>
    /// <returns></returns>
    static public List<string> OrderedTripParticipants(int tripId)
    {
        List<string> participants = null;

        // Get the list of users associated with the specified trip
        IEnumerable<UserProfile> users = TripModel.UserProfiles.Join(TripModel.Trippers.Where(tripper => tripper.TripId == tripId),
            user => user.UserId, tripper => tripper.UserId, (user, tripper) => user);

        // Order the users so that the current user is first (if they are a participant)
        UserProfile currentUser = users.SingleOrDefault(user => user.Email == WebSecurity.CurrentUserName);
        if (currentUser != null)
        {
            participants = new List<string> { currentUser.Email }
                .Concat(users.Where(user => user.Email != WebSecurity.CurrentUserName).Select(user => user.Email)).ToList();
        }
        else
        {
            participants = users.Select(user => user.Email).ToList();
        }

        return participants;
    }

    /// <summary>
    /// Return the UserProfiles of all the participants associated with a trip
    /// </summary>
    /// <param name="tripId"></param>
    /// <returns></returns>
    static public List<UserProfile> TripParticipantsUserProfiles(int tripId)
    {
        IEnumerable<int> participants = TripModel.Trippers.Where(tripper => tripper.TripId == tripId).Select(tripper => tripper.UserId);
        return TripModel.UserProfiles.Where(user => participants.Contains(user.UserId)).ToList();
    }

    /// <summary>
    /// Return the UserProfiles of all the trippers in the database, with the current user's trippers first (if any)
    /// </summary>
    /// <returns></returns>
    static public List<UserProfile> OrderedTrippers()
    {
        // Get a local copy of the UserProfiles so that Entity LINQ isn't required when getting trippers only
        List<UserProfile> users = TripModel.UserProfiles.ToList();
        IEnumerable<UserProfile> unorderedTrippers = users.Where(user => Roles.IsUserInRole(user.Email, "Tripper"));

        // Reorder this list so that the logged in user is at the top of the list.
        List<UserProfile> trippers = new List<UserProfile>();

        UserProfile loggedOnUser = unorderedTrippers.SingleOrDefault(user => user.Email == WebSecurity.CurrentUserName);
        if ( loggedOnUser != null )
        {
            trippers.Add(loggedOnUser);
        }

        trippers.AddRange(unorderedTrippers.Where(user => user.Email != WebSecurity.CurrentUserName));

        return trippers;
    }

    /// <summary>
    /// Generate a string representation of an amount in the specified currency
    /// </summary>
    /// <param name="amount"></param>
    /// <param name="currencyId"></param>
    /// <returns></returns>
    static public string AmountString( double amount, int currencyId )
    {
        Currency currencyToUse = TripModel.Currencies.Single(cur => cur.Id == currencyId);

        return currencyToUse.Before ? string.Format("{0}{1}", currencyToUse.Symbol, amount.ToString("F2"))
            : string.Format("{1}{0}", currencyToUse.Symbol, amount.ToString("F2"));
    }

    /// <summary>
    /// Get the name of a user given its Id
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    static public string UserNameString( int userId )
    {
        return TripModel.UserProfiles.Single(user => user.UserId == userId).Email;
    }

    /// <summary>
    /// Return the Id of the Sterling currency record
    /// </summary>
    /// <param name="currencyId"></param>
    /// <returns></returns>
    static public int SterlingId()
    {
        return TripModel.Currencies.Single(cur => cur.Name == SterlingName).Id;
    }

    /// <summary>
    /// The constant name for Sterling currency
    /// </summary>
    public const string SterlingName = "Sterling";

    /// <summary>
    /// Convert the specified amount from one currency to another
    /// </summary>
    /// <param name="amount"></param>
    /// <param name="fromCurrency"></param>
    /// <param name="toCurrency"></param>
    /// <returns></returns>
    static public double ConvertAmount( double amount, int fromCurrency, int toCurrency )
    {
        double convertedAmount = amount;

        if ( fromCurrency != toCurrency )
        {
            Currency from = TripModel.Currencies.Single( cur => cur.Id == fromCurrency);
            Currency to = TripModel.Currencies.Single( cur => cur.Id == toCurrency);

            if ( from.Name == SterlingName )
            {
                // Just convert to GetCurrency
                convertedAmount = amount / to.Rate;
            }
            else if ( to.Name == SterlingName )
            {
                convertedAmount = amount * from.Rate;
            }
            else
            {
                convertedAmount = ( amount * from.Rate ) / to.Rate;
            }
        }

        return convertedAmount;
    }

    /// <summary>
    /// Return a string representation of the day of the month with an ordinal suffix, followed by the month name 
    /// (e.g. "1st January", "2nd February", "3rd March", "4th April" etc.)
    /// </summary>
    /// <param name="date"></param>
    /// <returns></returns>
    static public string DayOrdinalMonth( DateTime date )
    {
        string daySuffix = "th";
        if ( date.Day % 10 == 1 && date.Day != 11 )
        {
            daySuffix = "st";
        }
        else if ( date.Day % 10 == 2 && date.Day != 12 )
        {
            daySuffix = "nd";
        }
        else if ( date.Day % 10 == 3 && date.Day != 13 )
        {
            daySuffix = "rd";
        }

        return string.Format("{0}{1} {2}", date.Day, daySuffix, date.ToString("MMMM"));
    }
}