using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Collections.ObjectModel;


namespace Soduko_App.Game_Logic
{
    class Serilizer
    {

        public static void SaveDataToAddress( string address, object data )
        {
            var state = new ApplicationDataCompositeValue();
            state["def"] = data;

            ApplicationData.Current.LocalSettings.Values[address] = state;
        }

        public static object RestoreDataFromAddress( string address )
        {
            var state = ApplicationData.Current.LocalSettings.Values[address] as ApplicationDataCompositeValue;
            if (state == null)
                return null;

            return state["def"];
        }



        public static void SavePuzzleState(SodukoPiece[,] p,int[,] originalState, int[,] currentPositions)
        {
            var state = new ApplicationDataCompositeValue();
            StringBuilder sb = new StringBuilder();
            foreach (SodukoPiece piece in p)
            {
                sb.Append(piece.GetInfoAsString());
            }

            state["Pieces"] = sb.ToString();

            sb.Clear();
            foreach (int i in originalState)
            {
                sb.Append(i.ToString());
                sb.Append(";");
            }

            state["OriginalState"] = sb.ToString();

            sb.Clear();
            foreach (int i in currentPositions)
            {
                sb.Append(i.ToString());
                sb.Append(";");
            }

            state["CurrentPos"] = sb.ToString();


            ApplicationData.Current.LocalSettings.Values["PuzzleState"] = state;
        }

        public static void ClearSaveGameCache()
        {
            ApplicationData.Current.LocalSettings.Values["PuzzleState"] = null;
        }

        public static bool IsStateSaved()
        {
            var state = ApplicationData.Current.LocalSettings.Values["PuzzleState"] as ApplicationDataCompositeValue;
            if (state == null)
                return false;
            return true;
        }

        public static ApplicationDataCompositeValue GetPuzzleState()
        {
            return ApplicationData.Current.LocalSettings.Values["PuzzleState"] as ApplicationDataCompositeValue;
        }

        public static SodukoGameInfo GetPersistingSodukoGameInfo()
        {
            SodukoGameInfo sgi = new SodukoGameInfo();
            var state = ApplicationData.Current.LocalSettings.Values["PersistingInfo"] as ApplicationDataCompositeValue;
            if (state == null)
            {
                // Hasn't completed a game yet so just return the defaults.
                sgi.Entries.Add(new HighscoreKey(1, Difficulty.Easy), new HighscoreEntry(600));
                sgi.Entries.Add(new HighscoreKey(2, Difficulty.Easy), new HighscoreEntry(660));
                sgi.Entries.Add(new HighscoreKey(3, Difficulty.Easy), new HighscoreEntry(720));

                sgi.Entries.Add(new HighscoreKey(1, Difficulty.Normal), new HighscoreEntry(780));
                sgi.Entries.Add(new HighscoreKey(2, Difficulty.Normal), new HighscoreEntry(840));
                sgi.Entries.Add(new HighscoreKey(3, Difficulty.Normal), new HighscoreEntry(900));

                sgi.Entries.Add(new HighscoreKey(1, Difficulty.Hard), new HighscoreEntry(960));
                sgi.Entries.Add(new HighscoreKey(2, Difficulty.Hard), new HighscoreEntry(1020));
                sgi.Entries.Add(new HighscoreKey(3, Difficulty.Hard), new HighscoreEntry(1080));

                return sgi;
            }

            foreach (KeyValuePair<string, object> key in state)
            {
                sgi.Entries[new HighscoreKey(key.Key)] = new HighscoreEntry((int)key.Value);
            }

            return sgi;
        }

        public static void SavePersistingSodukoGameInfo(SodukoGameInfo sgi)
        {
            var state = new ApplicationDataCompositeValue();
            foreach (KeyValuePair<HighscoreKey,HighscoreEntry> key in sgi.Entries)
            {
                state[key.Key.ToString()] = key.Value.Seconds;
            }
            ApplicationData.Current.LocalSettings.Values["PersistingInfo"] = state;
        }
    }

}
