using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.ApplicationSettings;
using Soduko_App.Game_Logic;
using Windows.UI.Popups;
using System.Threading;
using Windows.Storage;
using System.Threading.Tasks;
using Windows.UI;

namespace Soduko_App.Game_Logic
{
    struct ExperienceSaveData
    {
        public ExperienceSaveData(int level, int expTowardsNextLevel)
        {
            Level = level;
            ExpTowardsNextLevel = expTowardsNextLevel;
        }

        public ExperienceSaveData(string s)
        {
            Level = 0;
            ExpTowardsNextLevel = 0;
            Parse(s);
        }

        public override string ToString()
        {
            string final = Level.ToString() + ";" + ExpTowardsNextLevel.ToString();
            return final;
        }

        public void Parse(string s)
        {
            if (s == null)
            {
                Level = 0;
                ExpTowardsNextLevel = 0;
                return;
            }
            char[] seperator = new char[1];
            seperator[0] = ';';
            string[] data = s.Split(seperator);
            Level = Int32.Parse(data[0]);
            ExpTowardsNextLevel = Int32.Parse(data[1]);
        }

        public int Level;
        public int ExpTowardsNextLevel;
    }
    class ExperienceMgr
    {
        private static int[] ExperienceTable;

        private const int MAX_LEVEL = 9;
        private const float EXPERIENCE_FACTOR = 150.0f;
        private const string DATA_ADDRESS = "ExpData";
        public ExperienceMgr(ExperienceSaveData esd)
        {
            CreateExperienceTable();
            _esd = esd;
        }

        public static void CreateExperienceTable()
        {
            ExperienceTable = new int[MAX_LEVEL + 1];
            ExperienceTable[0] = 50;
            ExperienceTable[1] = 100;
            ExperienceTable[2] = 200;
            ExperienceTable[3] = 350;
            ExperienceTable[4] = 500;
            ExperienceTable[5] = 700;
            ExperienceTable[6] = 910;
            ExperienceTable[7] = 1140;
            ExperienceTable[8] = 1400;
            ExperienceTable[9] = 1630;
        }

        public ExperienceMgr()
        {
            CreateExperienceTable();
        }

        public void LoadSaveData(ExperienceSaveData esd)
        {
            _esd = esd;
        }

        public int Experience
        {
            get
            {
                return _esd.ExpTowardsNextLevel;
            }
            set
            {
                if ((value) > ExperienceTable[Level])
                {
                    int difference = (_esd.ExpTowardsNextLevel + value) - ExperienceTable[Level];
                    Experience = difference;
                    if (OnLevelUp != null)
                        OnLevelUp(this, new EventArgs());
                    ++Level;
                    
                }
                else
                    _esd.ExpTowardsNextLevel = value;
            }
        }

        public int Level
        {
            get
            {
                return _esd.Level;
            }
            set
            {
                if (value < MAX_LEVEL)
                    _esd.Level = value;
            }
        }

        public void PuzzleCompleted(Difficulty difficutlyCompletedOn, int secondsCompletedIn)
        {
            float difficultyFactor = 0.0f;
            switch (difficutlyCompletedOn)
            {
                case Difficulty.Easy:
                    difficultyFactor = 2.0f;
                    break;
                case Difficulty.Normal:
                    difficultyFactor = 4.0f;
                    break;
                case Difficulty.Hard:
                    difficultyFactor = 8.0f;
                    break;
            }
            float scaledSeconds = ((float)secondsCompletedIn) * 0.045f;

            float experienceEarned = (difficultyFactor * EXPERIENCE_FACTOR) / scaledSeconds;

            Experience += (int)Math.Round(experienceEarned);
        }

        public void SerilizeData()
        {
            string dataAsStr = _esd.ToString();
            Serilizer.SaveDataToAddress( DATA_ADDRESS, dataAsStr );
        }

        public void LoadData()
        {
            _esd.Parse((string)Serilizer.RestoreDataFromAddress(DATA_ADDRESS));

        }

        public void UpdateControls(ProgressBar levelBar, TextBlock levelText, TextBlock expText)
        {
            float experienceTowardsNextLevel = (float)ExperienceTable[Level];
            float possessedExp = (float)Experience;
            float percentage = possessedExp / experienceTowardsNextLevel;

            levelBar.Maximum = 100.0f;
            levelBar.Minimum = 0.0f;
            levelBar.Value = percentage * 100.0f;

            string finalLevelText = "Level: " + Level.ToString();

            string finalExpText = "( " + possessedExp.ToString() + " / " + experienceTowardsNextLevel.ToString() + " )";

            levelText.Text = finalLevelText;
            expText.Text = finalExpText;

        }

        private ExperienceSaveData _esd;

        public event EventHandler OnLevelUp = null;
    }
}
