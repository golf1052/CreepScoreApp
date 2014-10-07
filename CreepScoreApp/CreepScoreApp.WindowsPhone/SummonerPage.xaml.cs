using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using CreepScoreAPI;
using CreepScoreAPI.Constants;
using CreepScoreApp.Constants;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Phone.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace CreepScoreApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SummonerPage : Page
    {
        Summoner summoner;

        ListView leagueListView;
        Dictionary<string, List<League>> leagues;
        ObservableCollection<LeagueListViewBinding> leagueEntriesCollection;

        ListView recentGamesListView;
        RecentGames recentGames;
        ObservableCollection<RecentGameListViewBinding> recentGamesCollection;

        TextBlock tierTextBlock;
        TextBlock divisionTextBlock;
        Button upDivisionButton;
        Button downDivisionButton;
        TextBlock tierNameTextBlock;
        ComboBox queueTypeComboBox;
        string currentDivision;
        League currentLeague;
        List<League> leagueKey;

        bool justLoaded = true;

        public SummonerPage()
        {
            this.InitializeComponent();

            HardwareButtons.BackPressed += HardwareButtons_BackPressed;

            leagueEntriesCollection = new ObservableCollection<LeagueListViewBinding>();
            recentGamesCollection = new ObservableCollection<RecentGameListViewBinding>();
            leagueKey = new List<League>();
        }

        private void HardwareButtons_BackPressed(object sender, BackPressedEventArgs e)
        {
            Frame frame = Window.Current.Content as Frame;
            if (frame == null)
            {
                return;
            }

            if (frame.CanGoBack)
            {
                frame.GoBack();
                e.Handled = true;
            }
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            List<object> parameters = e.Parameter as List<object>;
            summoner = parameters[0] as Summoner;

            summonerNameTextBlock.Text = summoner.name;
            summonerProfileIconImage.Source = AppConstants.SetImageSource(new Uri(AppConstants.SummonerProfileIconUrl() + summoner.profileIconId.ToString() + ".png"));

            if (summoner.summonerLevel == 30)
            {
                leagues = await summoner.RetrieveLeague();

                if (leagues != null)
                {
                    League league = null;
                    LeagueEntry summonerLeagueEntry = null;

                    foreach (League l in leagues[summoner.id.ToString()])
                    {
                        if (l.queue == GameConstants.Queue.Solo5)
                        {
                            league = l;
                            ComboBoxItem temp = new ComboBoxItem();
                            temp.Content = "Solo";
                            queueTypeComboBox.Items.Add(temp);
                        }
                        else if (l.queue == GameConstants.Queue.Team5)
                        {
                            ComboBoxItem temp = new ComboBoxItem();
                            temp.Content = "5 v 5";
                            queueTypeComboBox.Items.Add(temp);
                        }
                        else if (l.queue == GameConstants.Queue.Team3)
                        {
                            ComboBoxItem temp = new ComboBoxItem();
                            temp.Content = "3 v 3";
                            queueTypeComboBox.Items.Add(temp);
                        }

                        leagueKey.Add(l);
                    }

                    if (queueTypeComboBox.Items.Count != 0)
                    {
                        queueTypeComboBox.SelectedIndex = 0;
                    }

                    if (league != null)
                    {
                        summonerLeagueEntry = FindLeagueEntry(league, summoner.id.ToString());
                        currentDivision = summonerLeagueEntry.division;
                        currentLeague = league;
                        DisplayDisvison(league, summonerLeagueEntry.division);
                    }
                }
                else
                {
                    rankedLeaguesHubSection.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                }
            }
            justLoaded = false;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            HardwareButtons.BackPressed -= HardwareButtons_BackPressed;
        }

        LeagueEntry FindLeagueEntry(League league, string id)
        {
            foreach (LeagueEntry entry in league.entries)
            {
                if (entry.playerOrTeamId == id)
                {
                    return entry;
                }
            }

            return null;
        }

        void DisplayDisvison(League league, string requestedDivision)
        {
            if (league != null)
            {
                if (league.tier == GameConstants.Tier.Challenger)
                {
                    upDivisionButton.IsEnabled = false;
                    downDivisionButton.IsEnabled = false;
                }
                tierTextBlock.Text = league.tierString[0] + league.tierString.ToLower().Substring(1);
                divisionTextBlock.Text = requestedDivision;
                if (requestedDivision == "I")
                {
                    upDivisionButton.IsEnabled = false;
                }
                else if (requestedDivision == "V")
                {
                    downDivisionButton.IsEnabled = false;
                }
                tierNameTextBlock.Text = league.name;
                leagueEntriesCollection.Clear();
                List<LeagueEntry> inSeriesEntries = new List<LeagueEntry>();
                List<LeagueEntry> notInSeriesEntries = new List<LeagueEntry>();
                List<LeagueEntry> sortedEntries = new List<LeagueEntry>();

                foreach (LeagueEntry entry in league.entries)
                {
                    if (entry.division == requestedDivision)
                    {
                        if (entry.miniSeries == null)
                        {
                            notInSeriesEntries.Add(entry);
                        }
                        else
                        {
                            inSeriesEntries.Add(entry);
                        }
                    }
                }
                inSeriesEntries.Sort(CompareWins);
                notInSeriesEntries.Sort(CompareLadderPoints);
                sortedEntries = inSeriesEntries;
                foreach (LeagueEntry entry in notInSeriesEntries)
                {
                    sortedEntries.Add(entry);
                }

                int rankCounter = 1;
                int summonerPosition = -1;
                LeagueListViewBinding summonerBinding = null;
                foreach (LeagueEntry entry in sortedEntries)
                {
                    bool isSummoner;
                    if (entry.playerOrTeamId == league.participantId)
                    {
                        isSummoner = true;
                    }
                    else
                    {
                        isSummoner = false;
                    }

                    if (entry.miniSeries == null)
                    {
                        leagueEntriesCollection.Add(new LeagueListViewBinding(rankCounter,
                            entry.playerOrTeamName,
                            entry.isVeteran,
                            entry.isHotStreak,
                            entry.isFreshBlood,
                            entry.isInactive,
                            entry.wins,
                            entry.leaguePoints,
                            isSummoner));
                    }
                    else
                    {
                        leagueEntriesCollection.Add(new LeagueListViewBinding(rankCounter,
                            entry.playerOrTeamName,
                            entry.isVeteran,
                            entry.isHotStreak,
                            entry.isFreshBlood,
                            entry.isInactive,
                            entry.wins,
                            entry.miniSeries.target,
                            entry.miniSeries.progress,
                            isSummoner));
                    }

                    if (isSummoner)
                    {
                        summonerBinding = leagueEntriesCollection[leagueEntriesCollection.Count - 1];
                    }
                    rankCounter++;
                }
                leagueListView.ScrollIntoView(summonerBinding);
            }
        }

        int CompareLadderPoints(LeagueEntry x, LeagueEntry y)
        {
            int returnValue = y.leaguePoints - x.leaguePoints;
            if (returnValue == 0)
            {
                return CompareWins(x, y);
            }
            else
            {
                return returnValue;
            }
        }

        int CompareWins(LeagueEntry x, LeagueEntry y)
        {
            return y.wins - x.wins;
        }

        private void tierTextBlock_Loaded(object sender, RoutedEventArgs e)
        {
            tierTextBlock = sender as TextBlock;
        }

        private void divisionTextBlock_Loaded(object sender, RoutedEventArgs e)
        {
            divisionTextBlock = sender as TextBlock;
        }

        private void upDivisionButton_Loaded(object sender, RoutedEventArgs e)
        {
            upDivisionButton = sender as Button;
        }

        private void upDivisionButton_Click(object sender, RoutedEventArgs e)
        {
            upDivisionButton.IsEnabled = true;
            downDivisionButton.IsEnabled = true;

            if (currentDivision == "I")
            {
            }
            else if (currentDivision == "II")
            {
                currentDivision = "I";
            }
            else if (currentDivision == "III")
            {
                currentDivision = "II";
            }
            else if (currentDivision == "IV")
            {
                currentDivision = "III";
            }
            else if (currentDivision == "V")
            {
                currentDivision = "IV";
            }

            DisplayDisvison(currentLeague, currentDivision);
        }

        private void downDivisonButton_Loaded(object sender, RoutedEventArgs e)
        {
            downDivisionButton = sender as Button;
        }

        private void downDivisonButton_Click(object sender, RoutedEventArgs e)
        {
            upDivisionButton.IsEnabled = true;
            downDivisionButton.IsEnabled = true;

            if (currentDivision == "I")
            {
                currentDivision = "II";
            }
            else if (currentDivision == "II")
            {
                currentDivision = "III";
            }
            else if (currentDivision == "III")
            {
                currentDivision = "IV";
            }
            else if (currentDivision == "IV")
            {
                currentDivision = "V";
            }
            else if (currentDivision == "V")
            {
            }

            DisplayDisvison(currentLeague, currentDivision);
        }

        private void tierNameTextBlock_Loaded(object sender, RoutedEventArgs e)
        {
            tierNameTextBlock = sender as TextBlock;
        }

        private void queueTypeComboBox_Loaded(object sender, RoutedEventArgs e)
        {
            queueTypeComboBox = sender as ComboBox;
        }

        private void queueTypeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!justLoaded)
            {
                upDivisionButton.IsEnabled = true;
                downDivisionButton.IsEnabled = true;
                League league = leagueKey[queueTypeComboBox.SelectedIndex];
                LeagueEntry participantLeagueId = FindLeagueEntry(league, league.participantId);

                if (participantLeagueId == null)
                {
                    currentDivision = "I";
                }
                else
                {
                    currentDivision = participantLeagueId.division;
                }
                currentLeague = league;
                DisplayDisvison(league, currentDivision);
            }
        }

        private void leagueBorder_Loaded(object sender, RoutedEventArgs e)
        {
            Border border = sender as Border;
            border.BorderBrush = AppConstants.themeColor;
        }

        private void leagueListView_Loaded(object sender, RoutedEventArgs e)
        {
            leagueListView = sender as ListView;
            leagueListView.ItemsSource = leagueEntriesCollection;
        }

        private async void leagueListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            LeagueListViewBinding binding = e.ClickedItem as LeagueListViewBinding;
            Summoner newSummoner = await AppConstants.creepScore.RetrieveSummoner(summoner.region, binding.SummonerName);

            if (summoner == null)
            {

            }
            else
            {
                List<object> parameters = new List<object>();
                parameters.Add(newSummoner);
                Frame.Navigate(typeof(SummonerPage), parameters);
            }
        }
    }
}
