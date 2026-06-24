
using System.Collections.Generic;
using System.Linq;

public class TripModel
{
    public static List<Currency> Currencies
    {
        get
        {
            if ( currencies == null )
            {
                currencies = underlyingModel.Currencies.ToList();
            }

            return currencies;
        }
    }
    private static List<Currency> currencies = null;

    public static List<Transaction> Transactions
    {
        get
        {
            if ( transactions == null )
            {
                transactions = underlyingModel.Transactions.ToList();
            }

            return transactions;
        }
    }
    private static List<Transaction> transactions = null;

    public static List<UserProfile> UserProfiles
    {
        get
        {
            if ( userProfiles == null )
            {
                userProfiles = underlyingModel.UserProfiles.ToList();
            }

            return userProfiles;
        }
    }
    private static List<UserProfile> userProfiles = null;

    public static List<Tripper> Trippers
    {
        get
        {
            if ( trippers == null )
            {
                trippers = underlyingModel.Trippers.ToList();
            }

            return trippers;
        }
    }
    private static List<Tripper> trippers = null;

    public static List<Sharer> Sharers
    {
        get
        {
            if ( sharers == null )
            {
                sharers = underlyingModel.Sharers.ToList();
            }

            return sharers;
        }
    }
    private static List<Sharer> sharers = null;

    public static void SaveChanges()
    {
        currencies = null;
        transactions = null;
        userProfiles = null;
        trippers = null;

        underlyingModel.SaveChanges();
    }

    private static StarterSiteEntities underlyingModel = new StarterSiteEntities();
}