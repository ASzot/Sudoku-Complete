using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using Windows.Storage;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.ApplicationSettings;

namespace Soduko_App.Game_Logic
{
    enum HintMode { Off,Adjacent,Everything };

    class SodukoInitInfo
    {
        public bool RestoreState;
        public Difficulty DesiredDifficulty;
    }

    class SodukoGameInfo
    {

        public HighscoreEntry GetHighscoreEntry(HighscoreKey key)
        {
            // There should just be one element in this list.
            IEnumerable<HighscoreEntry> highscoreEntry = from k in Entries
                                                         where k.Key.Diff == key.Diff
                                                         where k.Key.Place == key.Place
                                                         select k.Value;
            return highscoreEntry.ElementAt(0);
        }

        public bool AddIfHighScore(Difficulty d, int seconds)
        {
            HighscoreEntry he = new HighscoreEntry(seconds);
            HighscoreKey hk = new HighscoreKey(4, d);
            Entries[hk] = he;

            var myList = Entries.ToList();
            myList.Sort((firstPair, secondPair) =>
                {
                    int diffAsIntFirst = (int)firstPair.Key.Diff;
                    int diffAsIntSecond = (int)secondPair.Key.Diff;
                    if (diffAsIntFirst < diffAsIntSecond)
                        return -1;
                    else if (diffAsIntFirst > diffAsIntSecond)
                        return 1;
                    return firstPair.Value.Seconds.CompareTo(secondPair.Value.Seconds);
                }
            );

            // Trim the list to just 3 elements per diff list.
            int count = 0;
            Difficulty prevDiff = Difficulty.Easy;
            for (int i = 0; i < myList.Count; ++i)
            {
                var keyValuePair = myList.ElementAt(i);
                HighscoreEntry entryValue = keyValuePair.Value;
                HighscoreKey entryKey = keyValuePair.Key;
                if (entryKey.Diff == prevDiff)
                    ++count;
                else
                    count = 1;
                if (count > 3)
                {
                    myList.RemoveAt(i);
                    --i;
                    continue;
                }
                prevDiff = entryKey.Diff;
                myList.ElementAt(i).Key.Place = count;
            }

            Entries = myList.ToDictionary(t => t.Key, t => t.Value);

            var listOfSimilar = (from key in Entries
                                 where key.Key.Diff == d
                                 where key.Value.Seconds == seconds
                                 select key).ToList();

            if (listOfSimilar.Count > 0)
                return true;
            return false;
        }
        public Dictionary<HighscoreKey, HighscoreEntry> Entries = new Dictionary<HighscoreKey, HighscoreEntry>();
    }

    class HighscoreKey
    {
        public HighscoreKey()
        {

        }
        public HighscoreKey(int place, Difficulty d)
        {
            Place = place;
            Diff = d;
        }
        public HighscoreKey(string s)
        {
            FromString(s);
        }
        public int Place;
        public Difficulty Diff;

        public override string ToString()
        {
            string place = Place.ToString();
            string diff = Diff.ToString();
            return place + " " + diff;
        }

        public void FromString(string s)
        {
            string[] infoStrs = s.Split();
            Place = Int32.Parse(infoStrs[0]);
            Diff = (Difficulty)Enum.Parse(typeof(Difficulty), infoStrs[1]);
        }

        public override bool Equals(object obj)
        {
            HighscoreKey hk = (HighscoreKey)obj;
            if ((Place == hk.Place) && (Diff == hk.Diff))
            {
                return true;
            }
            return false;
        }
    }

    struct HighscoreEntry
    {
        public HighscoreEntry(int seconds)
        {
            this.Seconds = seconds;
        }

        public override string ToString()
        {
            return Seconds.FromSecondsToTimeFormat();
        }

        public Int32 Seconds;
    }

    static class IntegerAddOn
    {
        public static string FromSecondsToTimeFormat(this Int32 t)
        {
            int minutesTicking = t / 60;
            int secondsTicking = t % 60;
            string min, sec;
            if (minutesTicking < 10)
                min = "0" + minutesTicking.ToString();
            else
                min = minutesTicking.ToString();
            if (secondsTicking < 10)
                sec = "0" + secondsTicking.ToString();
            else
                sec = secondsTicking.ToString();
            string r = "00:" + min + ":" + sec;
            return r;
        }
    }

    struct TimesPlayedData
    {
        const string SAVE_KEY = "TimesPlayedInfo";

        public static void OnGameBeaten(Difficulty diff)
        {
            switch (diff)
            {
                case Difficulty.Easy:
                    ++TimesPlayedOnEasy;
                    break;
                case Difficulty.Normal:
                    ++TimesPlayedOnNormal;
                    break;
                case Difficulty.Hard:
                    ++TimesPlayedOnHard;
                    break;
            }
            ++TimesPlayed;

            SaveData();
        }

        public static void LoadData()
        {
            TimesPlayed = 0;
            TimesPlayedOnHard = 0;
            TimesPlayedOnNormal = 0;
            TimesPlayedOnEasy = 0;
            object obj = Serilizer.RestoreDataFromAddress(SAVE_KEY);
            if (obj != null)
            {
                TimesPlayed = (int)obj;
                TimesPlayedOnHard = (int)Serilizer.RestoreDataFromAddress(SAVE_KEY + "Hard");
                TimesPlayedOnNormal = (int)Serilizer.RestoreDataFromAddress(SAVE_KEY + "Normal");
                TimesPlayedOnEasy = (int)Serilizer.RestoreDataFromAddress(SAVE_KEY + "Easy");
            }
        }

        public static void SaveData()
        {
            Serilizer.SaveDataToAddress(SAVE_KEY, TimesPlayed);
            Serilizer.SaveDataToAddress(SAVE_KEY + "Hard", TimesPlayedOnHard);
            Serilizer.SaveDataToAddress(SAVE_KEY + "Normal", TimesPlayedOnNormal);
            Serilizer.SaveDataToAddress(SAVE_KEY + "Easy", TimesPlayedOnEasy);
        }

        public static void UpdateUI(TextBlock textBlock, TextBlock textBlockEasy = null, TextBlock textBlockNormal = null, TextBlock textBlockHard = null)
        {
            textBlock.Text = "Total Puzzles Completed: " + TimesPlayed.ToString();
            if (textBlockEasy != null)
            {
                textBlockEasy.Text = "Puzzles On Easy: " + TimesPlayedOnEasy.ToString();
            }
            if (textBlockNormal != null)
            {
                textBlockNormal.Text = "Puzzles On Normal: " + TimesPlayedOnNormal.ToString();
            }
            if (textBlockHard != null)
            {
                textBlockHard.Text = "Puzzles On Hard: " + TimesPlayedOnHard.ToString();
            }
        }

        public static int TimesPlayed;
        public static int TimesPlayedOnHard;
        public static int TimesPlayedOnNormal;
        public static int TimesPlayedOnEasy;
    }
}
