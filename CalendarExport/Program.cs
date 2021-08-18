using System;
using System.Collections.Generic;
using System.Data.SqlServerCe;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalendarExport
{
    class Program
    {
        static void Main( string[] args )
        {
            List<Game> games = GetGames();
            ExportGames( games );
            Console.Read();
        }

        private static List<Game> GetGames()
        {
            List<Game> games = new List<Game>();

            string dbFileName = "StarterSite.sdf";
            string dbConnectionString = string.Format( "Data Source=|DataDirectory|{0};Max Database Size=4091", dbFileName );

            using ( var connection = new SqlCeConnection( dbConnectionString ) )
            {
                using ( var context = new StarterSiteContext( connection ) )
                {
                    Competition league = context.Competitions.Single( comp => comp.Name == "2021 - 2022 League" );
                    Competition cup1 = context.Competitions.Single( comp => comp.Name == "Cup 1 2021" );
                    Competition cup2 = context.Competitions.Single( comp => comp.Name == "Cup 2 2021" );
                    Competition cup3 = context.Competitions.Single( comp => comp.Name == "Cup 3 2021" );

                    // Get all the games for each of these competitions and add to list
                    games.AddRange( context.Games.Where( game => game.Competition == league.Id ) );

                    // Must adjust name of opponents to include the cup name
                    List<Game> cup1Games = context.Games.Where( game => game.Competition == cup1.Id ).ToList();
                    cup1Games.ForEach( game => game.Opponents += " (C1)" );
                    games.AddRange( cup1Games );

                    List<Game> cup2Games = context.Games.Where( game => game.Competition == cup2.Id ).ToList();
                    cup2Games.ForEach( game => game.Opponents += " (C2)" );
                    games.AddRange( cup2Games );

                    List<Game> cup3Games = context.Games.Where( game => game.Competition == cup3.Id ).ToList();
                    cup3Games.ForEach( game => game.Opponents += " (C3)" );
                    games.AddRange( cup3Games );
                }
            }

            return games;
        }

        private static void ExportGames( List<Game> games)
        {
            StringBuilder sb = new StringBuilder();
            const string DateFormat = "yyyyMMddTHHmmssZ";
            string now = DateTime.Now.ToUniversalTime().ToString( DateFormat );

            sb.AppendLine( "BEGIN:VCALENDAR" );
            sb.AppendLine( "PRODID:-//Oddzes//Skittles//EN" );
            sb.AppendLine( "VERSION:2.0" );
            sb.AppendLine( "METHOD:PUBLISH" );
            foreach ( Game game in games )
            {
                DateTime dtStart = game.Date.Add( game.Late.GetValueOrDefault() == false ? new TimeSpan( 19, 30, 00 ) : new TimeSpan( 20, 40, 00 ) );
                DateTime dtEnd = dtStart.AddHours( 1 );
                sb.AppendLine( "BEGIN:VEVENT" );
                sb.AppendLine( "DTSTART:" + dtStart.ToUniversalTime().ToString( DateFormat ) );
                sb.AppendLine( "DTEND:" + dtEnd.ToUniversalTime().ToString( DateFormat ) );
                sb.AppendLine( "DTSTAMP:" + now );
                sb.AppendLine( "UID:" + Guid.NewGuid() );
                sb.AppendLine( "CREATED:" + now );
                sb.AppendLine( "LOCATION:" );
                sb.AppendLine( "SEQUENCE:0" );
                sb.AppendLine( "STATUS:CONFIRMED" );
                sb.AppendLine( "SUMMARY:" + string.Format( "Skittles {0} {1}", game.Late.GetValueOrDefault() ? "Late" : "Early", game.Opponents ) );
                sb.AppendLine( "TRANSP:OPAQUE" );
                sb.AppendLine( "END:VEVENT" );
            }

            sb.AppendLine( "END:VCALENDAR" );

            File.WriteAllText( "Skittles.ics", sb.ToString() );

        }
    }
}
