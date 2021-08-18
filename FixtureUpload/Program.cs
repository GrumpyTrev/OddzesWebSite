using System;
using System.Collections.Generic;
using System.Data.SqlServerCe;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace FixtureUpload
{
    class Program
    {
        static void Main( string[] args )
        {
            XmlGames xmlGames = DeserialiseLeagueGames();
            List<Game> games = XmlGamesToGames( xmlGames );

            AddGamesToDatabase( games );

            Console.Read();
        }

        private static void AddGamesToDatabase( List<Game> games )
        {
            string dbFileName = "StarterSite.sdf";
            string dbConnectionString = string.Format( "Data Source=|DataDirectory|{0};Max Database Size=4091", dbFileName );

            using ( var connection = new SqlCeConnection( dbConnectionString ) )
            {
                using ( var context = new StarterSiteContext( connection ) )
                {
                    // Find the competitions to add the games to - 
                    // "2021 - 2022 League"
                    // "Cup 1 2021"
                    // "Cup 2 2021"
                    // "Cup 3 2021"

                    Competition league = context.Competitions.Single( comp => comp.Name == "2021 - 2022 League" );
                    Competition cup1 = context.Competitions.Single( comp => comp.Name == "Cup 1 2021" );
                    Competition cup2 = context.Competitions.Single( comp => comp.Name == "Cup 2 2021" );
                    Competition cup3 = context.Competitions.Single( comp => comp.Name == "Cup 3 2021" );

                    foreach ( Game game in games )
                    {
                        // Check the type of game, i.e. a league game or cup
                        if ( game.Opponents.Contains( "(C" ) == false )
                        {
                            game.Competition = league.Id;
                            context.Games.InsertOnSubmit( game );
                        }
                        else if ( game.Opponents.Contains( " (C1)" ) == true )
                        {
                            game.Opponents = game.Opponents.Replace( " (C1)", "" );
                            game.Competition = cup1.Id;
                            context.Games.InsertOnSubmit( game );
                        }
                        else if ( game.Opponents.Contains( " (C2)" ) == true )
                        {
                            game.Opponents = game.Opponents.Replace( " (C2)", "" );
                            game.Competition = cup2.Id;
                            context.Games.InsertOnSubmit( game );
                        }
                        else if ( game.Opponents.Contains( " (C3)" ) == true )
                        {
                            game.Opponents = game.Opponents.Replace( " (C3)", "" );
                            game.Competition = cup3.Id;
                            context.Games.InsertOnSubmit( game );
                        }
                    }

                    context.SubmitChanges();
                }
            }
        }

        private static XmlGames DeserialiseLeagueGames()
        {
            XmlGames games = null;

            using ( StreamReader str = new StreamReader( @"Fixtures 2021 - 2022.xml" ) )
            {
                games = ( XmlGames )new XmlSerializer( typeof( XmlGames ) ).Deserialize( str );
            }

            return games;
        }

        private static List<Game> XmlGamesToGames( XmlGames xmlGames )
        {
            List<Game> games = new List<Game>();

            foreach ( XmlGame xmlGame in xmlGames.Games )
            {
                Game game = new Game() { Opponents = xmlGame.Opponents, Late = ( xmlGame.Late == "Late" ), Lane = ( xmlGame.Lane == "Right" ),
                 Complete = false, InProgress = false, Them = -1, Us = -1 };

                // Remove any ordinal suffixes from the date
                string dateToParse = Regex.Replace( xmlGame.Date, @"(.+\d+)(th|rd|st|nd)(.+)", "$1$3" );

                // Extract the month and if it's month number is less than the current month then assume its next year
                string monthString = Regex.Match( dateToParse, @"(\w+)$" ).Value;
                int monthNumber = DateTime.ParseExact( monthString, "MMMM", CultureInfo.CurrentCulture ).Month;
                if ( monthNumber < DateTime.Now.Month )
                {
                    dateToParse += " " + ( DateTime.Now.Year + 1 ).ToString();
                }

                game.Date = DateTime.Parse( dateToParse );

                games.Add( game );
            }

            return games;
        }
    }

    [XmlType( "Root" )]
    public class XmlGames
    {
        [XmlElement( "Game" )]
        public List<XmlGame> Games { get; set; } = new List<XmlGame>();
    }

    [XmlType( "Game" )]
    public class XmlGame
    {
        public string Opponents { get; set; }

        public string Date { get; set; }

        public string Late { get; set; }

        public string Lane { get; set; }
    }
}
