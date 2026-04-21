using System.Collections.Generic;
using System.Linq;

public class HelperMethods
{
    /// <summary>
    /// String representation of all the participants in a trip
    /// </summary>
    /// <param name="tripId"></param>
    /// <returns></returns>
    static public string TripParticipantsString(int tripId)
    {
        string text = "No trippers yet!!";

        List<string> userNames = TripParticipants( tripId );
        if ( userNames.Count > 0 )
        {
            text = string.Join(", ", userNames);
        }

        return text;
    }

    /// <summary>
    /// Determine all the participants associated with a trip 
    /// </summary>
    /// <param name="tripId"></param>
    /// <returns></returns>
    static public List<string> TripParticipants(int tripId)
    {
        StarterSiteEntities model = new StarterSiteEntities();

        IEnumerable<int> participants = model.Trippers.Where(tripper => tripper.TripId == tripId).Select(tripper => tripper.UserId);

        return ( participants.Count() == 0 ) ? new List<string>() :
            model.UserProfiles.Where(user => participants.Contains(user.UserId)).Select(user => user.Email).ToList();
    }

    /// <summary>
    /// Return the UserProfiles of all the participants associated with a trip
    /// </summary>
    /// <param name="tripId"></param>
    /// <returns></returns>
    static public List<UserProfile> TripParticipantsUserProfiles(int tripId)
    {
        StarterSiteEntities model = new StarterSiteEntities();
        IEnumerable<int> participants = model.Trippers.Where(tripper => tripper.TripId == tripId).Select(tripper => tripper.UserId);
        return model.UserProfiles.Where(user => participants.Contains(user.UserId)).ToList();
    }

    /// <summary>
    /// Generate a string representation of an amount in the specified currency
    /// </summary>
    /// <param name="amount"></param>
    /// <param name="currencyId"></param>
    /// <returns></returns>
    static public string AmountString( double amount, int currencyId )
    {
        StarterSiteEntities model = new StarterSiteEntities();

        Currency currencyToUse = model.Currencies.Single(cur => cur.Id == currencyId);

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
        return new StarterSiteEntities().UserProfiles.Single(user => user.UserId == userId).Email;
    }

    /// <summary>
    /// Return the Id of the Sterling currency record
    /// </summary>
    /// <param name="currencyId"></param>
    /// <returns></returns>
    static public int SterlingId()
    {
        return new StarterSiteEntities().Currencies.Single(cur => cur.Name == SterlingName).Id;
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

        StarterSiteEntities model = new StarterSiteEntities();

        if ( fromCurrency != toCurrency )
        {
            Currency from = model.Currencies.Single(cur => cur.Id == fromCurrency);
            Currency to = model.Currencies.Single(cur => cur.Id == toCurrency);

            if ( from.Name == SterlingName )
            {
                // Just convert to toCurrency
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
}