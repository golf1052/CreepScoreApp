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
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace CreepScoreApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        TextBlock heroName;

        ChampionStatic featuredChamp;
        string featuredChampRealName;
        Random random = new Random();

        BitmapImage championImage;
        int loadedSkin = -1;

        double previousHeight;

        TextBox summonerSearchBox;
        ComboBox regionComboBox;
        RegionComboBoxSettings regionSettings;

        TextBlock errorTextBlock;
        ProgressRing searchingForSummonerRing;

        public MainPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            List<string> keys = AppConstants.championsData.data.Keys.ToList();
            int randomChamp = random.Next(0, keys.Count);
            featuredChamp = AppConstants.championsData.data[keys[randomChamp]];
            featuredChampRealName = keys[randomChamp];
            previousHeight = heroSection.Height;
        }

        private void heroName_Loaded(object sender, RoutedEventArgs e)
        {
            heroName = sender as TextBlock;
            heroName.Text = featuredChamp.name;
            heroName.Margin = new Thickness(20, mainGrid.ActualHeight - 180, 0, 0);
        }

        private void heroSection_Loaded(object sender, RoutedEventArgs e)
        {
            loadedSkin = random.Next(0, featuredChamp.skins.Count);
            championImage = new BitmapImage(new Uri(AppConstants.championLoadingUrl + featuredChampRealName + "_" + loadedSkin + ".jpg"));

            heroSection.Width = mainGrid.ActualHeight * 0.55;
            heroImage.ImageSource = championImage;
        }

        private void regionComboBox_Loaded(object sender, RoutedEventArgs e)
        {
            regionComboBox = sender as ComboBox;
            regionSettings = new RegionComboBoxSettings(regionComboBox, false);
        }

        private void cannotFindSummonerTextBlock_Loaded(object sender, RoutedEventArgs e)
        {
            errorTextBlock = sender as TextBlock;
        }

        private void searchingForSummonerRing_Loaded(object sender, RoutedEventArgs e)
        {
            searchingForSummonerRing = sender as ProgressRing;
            searchingForSummonerRing.Foreground = AppConstants.themeColor;
        }

        private void summonerSearchBox_GotFocus(object sender, RoutedEventArgs e)
        {
            errorTextBlock.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        }

        private async void summonerSearchBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                if (summonerSearchBox.Text == "")
                {
                    return;
                }

                summonerSearchBox.IsEnabled = false;
                searchingForSummonerRing.IsActive = true;
                //heroName.Focus(FocusState.Programmatic);
                string summonerName = summonerSearchBox.Text;
                CreepScore.Region region = (CreepScore.Region)AppConstants.GetCreepScoreRegion(regionComboBox.SelectedIndex);

                Summoner summoner = await AppConstants.creepScore.RetrieveSummoner(region, summonerName);

                if (summoner == null)
                {
                    searchingForSummonerRing.IsActive = false;
                    if (AppConstants.creepScore.ErrorString == "404")
                    {
                        errorTextBlock.Text = "Cannot find summoner";
                        errorTextBlock.Visibility = Windows.UI.Xaml.Visibility.Visible;
                    }
                    else if (AppConstants.creepScore.ErrorString == "401")
                    {
                        errorTextBlock.Text = "Unauthorized";
                        errorTextBlock.Visibility = Windows.UI.Xaml.Visibility.Visible;
                    }
                    else if (AppConstants.creepScore.ErrorString == "429")
                    {
                        errorTextBlock.Text = "Rate limit exceeded";
                        errorTextBlock.Visibility = Windows.UI.Xaml.Visibility.Visible;
                    }
                    else if (AppConstants.creepScore.ErrorString == "500")
                    {
                        errorTextBlock.Text = "Internal Riot server error";
                        errorTextBlock.Visibility = Windows.UI.Xaml.Visibility.Visible;
                    }
                    else if (AppConstants.creepScore.ErrorString == "503")
                    {
                        errorTextBlock.Text = "Riot service unavalible";
                        errorTextBlock.Visibility = Windows.UI.Xaml.Visibility.Visible;
                    }
                    summonerSearchBox.IsEnabled = true;
                }
                else
                {
                    searchingForSummonerRing.IsActive = false;
                    List<object> parameters = new List<object>();
                    parameters.Add(summoner);
                    Frame.Navigate(typeof(SummonerPage), parameters);
                }
            }
        }

        private void summonerSearchBox_Loaded(object sender, RoutedEventArgs e)
        {
            summonerSearchBox = sender as TextBox;
        }

        private void settingsButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(SettingsPage));
        }
    }
}
