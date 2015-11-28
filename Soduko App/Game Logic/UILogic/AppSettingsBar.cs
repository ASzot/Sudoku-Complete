using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.ApplicationSettings;
using Windows.UI.Popups;

namespace Soduko_App.Game_Logic.UILogic
{
    class AppSettingsBar
    {
        private List<SettingsCommand> _settingCommands;
        public AppSettingsBar(List<string> desiredSettingCommands, UICommandInvokedHandler uicih)
        {
            _settingCommands = new List<SettingsCommand>();
            foreach (string name in desiredSettingCommands)
            {
                _settingCommands.Add(new SettingsCommand(name, name, uicih));
            }
        }

        public void AppSettingsBar_CommandsRequested(SettingsPane sender, SettingsPaneCommandsRequestedEventArgs args)
        {
            foreach (SettingsCommand sc in _settingCommands)
            {
                args.Request.ApplicationCommands.Add(sc);
            }
            _settingCommands.Clear();
        }

        // Just a list of all the settings commands in our game.
        public const string SETTINGS_COMMAND_SOLVE_PUZZLE = "Solve the puzzle.";
    }
}
