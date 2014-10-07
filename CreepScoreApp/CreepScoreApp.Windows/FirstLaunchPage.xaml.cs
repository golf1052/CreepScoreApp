using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using CreepScoreAPI;
using CreepScoreApp.Constants;
using Windows.Storage;
using Windows.UI.Popups;
using CreepScoreAPI.Constants;
using System.Threading.Tasks;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace CreepScoreApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class FirstLaunchPage : Page
    {
        bool justLaunched = true;
        bool changedTheme = false;
        StorageFolder localFolder;

        public FirstLaunchPage()
        {
            this.InitializeComponent();

            if (AppConstants.themeColor == AppConstants.creepScoreYellow)
            {
                themeComboBox.SelectedIndex = 0;
            }
            else
            {
                themeComboBox.SelectedIndex = 1;
            }
            launchButton.Background = AppConstants.themeColor;
            launchButton.BorderBrush = AppConstants.themeColor;
            localFolder = ApplicationData.Current.LocalFolder;

            justLaunched = false;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        private async void themeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!justLaunched)
            {
                if (themeComboBox.SelectedIndex == 0)
                {
                    AppConstants.themeColor = AppConstants.creepScoreYellow;
                    this.RequestedTheme = ElementTheme.Dark;
                }
                else
                {
                    AppConstants.themeColor = AppConstants.creepScoreBlue;
                    this.RequestedTheme = ElementTheme.Light;
                }
                changedTheme = true;

                launchButton.Background = AppConstants.themeColor;
                launchButton.BorderBrush = AppConstants.themeColor;

                if (AppConstants.themeColor == AppConstants.creepScoreBlue)
                {
                    ApplicationData.Current.RoamingSettings.Values["IsLightTheme"] = true;
                }
                else
                {
                    ApplicationData.Current.RoamingSettings.Values["IsLightTheme"] = false;
                }

                MessageDialog message = new MessageDialog("Restart the app now to view your theme color. You can also change the theme color in the settings.");
                await message.ShowAsync();
            }
        }

        private async void launchButton_Click(object sender, RoutedEventArgs e)
        {
            if (changedTheme)
            {
                MessageDialog message = new MessageDialog("Note: You will not see your new theme until you restart the app.");
                message.Commands.Add(new UICommand("Don't Launch App Yet"));
                message.Commands.Add(new UICommand("Launch App Anyway", LaunchApp));
                await message.ShowAsync();
            }
            else
            {
                ApplicationData.Current.RoamingSettings.Values["LaunchedApp"] = true;
                await SaveRegionSetting();
                Frame.Navigate(typeof(LoadingPage));
            }
        }

        async void LaunchApp(IUICommand command)
        {
            ApplicationData.Current.RoamingSettings.Values["LaunchedApp"] = true;
            await SaveRegionSetting();
            Frame.Navigate(typeof(LoadingPage));
        }

        async Task SaveRegionSetting()
        {
            AppConstants.preferredRegion = (CreepScore.Region)regionComboBox.SelectedIndex;
            StorageFile regionFile = await AppConstants.TryCreateFile("Region.json", localFolder);
            await AppConstants.OverwriteFile(regionFile, UrlConstants.GetRegion(AppConstants.preferredRegion));
        }

        private void regionComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!justLaunched)
            {
                AppConstants.preferredRegion = (CreepScore.Region)regionComboBox.SelectedIndex;
            }
        }
    }
}
