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
using Windows.UI.Xaml.Media.Imaging;
using Windows.Graphics.Display;
using System.Diagnostics;
using CreepScoreAPI;
using CreepScoreApp.Constants;
using System.Threading.Tasks;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

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

        ComboBox regionComboBox;
        RegionComboBoxSettings regionSettings;

        TextBlock errorTextBlock;
        ProgressRing searchingForSummonerRing;

        public MainPage()
        {
            this.InitializeComponent();

            this.SizeChanged += MainPage_SizeChanged;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            List<string> keys = AppConstants.championsData.data.Keys.ToList();
            int randomChamp = random.Next(0, keys.Count);
            featuredChamp = AppConstants.championsData.data[keys[randomChamp]];
            featuredChampRealName = keys[randomChamp];
            previousHeight = heroSection.Height;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.SizeChanged -= MainPage_SizeChanged;
        }

        void MainPage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.NewSize.Width == 500)
            {
                mainHub.Orientation = Orientation.Vertical;
                if (loadedSkin != -1)
                {
                    championImage = new BitmapImage(new Uri(AppConstants.championLoadingUrl + featuredChampRealName + "_" + loadedSkin + ".jpg"));
                }
                else
                {
                    loadedSkin = random.Next(0, featuredChamp.skins.Count);
                    championImage = new BitmapImage(new Uri(AppConstants.championLoadingUrl + featuredChampRealName + "_" + loadedSkin + ".jpg"));
                }
                heroSection.Width = 500;
                heroSection.Height = 560.0 * (125.0 / 77.0);
                heroImage.ImageSource = championImage;
                if (heroName != null)
                {
                    heroName.Margin = new Thickness(25, mainGrid.ActualHeight - 350, 0, 0);
                    heroName.FontSize = 64;
                }
            }
            else
            {
                mainHub.Orientation = Orientation.Horizontal;
                if (loadedSkin != -1)
                {
                    championImage = new BitmapImage(new Uri(AppConstants.championSplashUrl + featuredChampRealName + "_" + loadedSkin + ".jpg"));
                }
                else
                {
                    loadedSkin = random.Next(0, featuredChamp.skins.Count);
                    championImage = new BitmapImage(new Uri(AppConstants.championSplashUrl + featuredChampRealName + "_" + loadedSkin + ".jpg"));
                }
                heroSection.Width = mainGrid.ActualHeight * 1.694560669456067;
                heroSection.Height = previousHeight;
                heroImage.ImageSource = championImage;
                if (heroName != null)
                {
                    heroName.Margin = new Thickness(50, mainGrid.ActualHeight - 424, 0, 0);
                    heroName.FontSize = 150;
                }
            }
        }

        private void heroSection_Loaded(object sender, RoutedEventArgs e)
        {
            loadedSkin = random.Next(0, featuredChamp.skins.Count);
            championImage = new BitmapImage(new Uri(AppConstants.championSplashUrl + featuredChampRealName + "_" + loadedSkin + ".jpg"));
            
            heroSection.Width = mainGrid.ActualHeight * 1.694560669456067;
            heroImage.ImageSource = championImage;
        }

        private void heroName_Loaded(object sender, RoutedEventArgs e)
        {
            heroName = sender as TextBlock;
            heroName.Text = featuredChamp.name;

            if (mainHub.Orientation == Orientation.Horizontal)
            {
                heroName.Margin = new Thickness(50, mainGrid.ActualHeight - 424, 0, 0);
                heroName.FontSize = 150;
            }
            else
            {
                heroName.Margin = new Thickness(25, mainGrid.ActualHeight - 350, 0, 0);
                heroName.FontSize = 64;
            }
        }

        private void heroName_Tapped(object sender, TappedRoutedEventArgs e)
        {
            List<object> parameters = new List<object>();
            parameters.Add(mainGrid.ActualHeight);
            Frame.Navigate(typeof(ChampionsPage), parameters);
        }

        private void settingsButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(SettingsPage));
        }

        private void regionComboBox_Loaded(object sender, RoutedEventArgs e)
        {
            regionComboBox = sender as ComboBox;
            regionSettings = new RegionComboBoxSettings(regionComboBox, false);
        }

        private async void summonerSearchBox_QuerySubmitted(SearchBox sender, SearchBoxQuerySubmittedEventArgs args)
        {
            if (args.QueryText == "")
            {
                return;
            }

            sender.IsEnabled = false;
            searchingForSummonerRing.IsActive = true;
            //heroName.Focus(FocusState.Programmatic);
            string summonerName = args.QueryText;
            CreepScore.Region region = (CreepScore.Region)regionComboBox.SelectedIndex;

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
                sender.IsEnabled = true;
            }
            else
            {
                searchingForSummonerRing.IsActive = false;
                List<object> parameters = new List<object>();
                parameters.Add(summoner);
                Frame.Navigate(typeof(SummonerPage), parameters);
            }
        }

        private void cannotFindSummonerTextBlock_Loaded(object sender, RoutedEventArgs e)
        {
            errorTextBlock = sender as TextBlock;
        }

        private void summonerSearchBox_GotFocus(object sender, RoutedEventArgs e)
        {
            errorTextBlock.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
        }

        private void searchingForSummonerRing_Loaded(object sender, RoutedEventArgs e)
        {
            searchingForSummonerRing = sender as ProgressRing;
            searchingForSummonerRing.Foreground = AppConstants.themeColor;
        }
    }
}
