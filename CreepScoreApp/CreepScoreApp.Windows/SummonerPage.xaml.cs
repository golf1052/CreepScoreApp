using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices.WindowsRuntime;
using CreepScoreAPI;
using CreepScoreAPI.Constants;
using CreepScoreApp.Constants;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Diagnostics;
using Windows.System;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

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

        Dictionary<string, RunePages> runePages;
        Dictionary<string, RunePageInfoListViewBinding> runeValues;
        List<Image> runeImages;
        #region Rune Images
        Image rune1Image;
        Image rune2Image;
        Image rune3Image;
        Image rune4Image;
        Image rune5Image;
        Image rune6Image;
        Image rune7Image;
        Image rune8Image;
        Image rune9Image;
        Image rune10Image;
        Image rune11Image;
        Image rune12Image;
        Image rune13Image;
        Image rune14Image;
        Image rune15Image;
        Image rune16Image;
        Image rune17Image;
        Image rune18Image;
        Image rune19Image;
        Image rune20Image;
        Image rune21Image;
        Image rune22Image;
        Image rune23Image;
        Image rune24Image;
        Image rune25Image;
        Image rune26Image;
        Image rune27Image;
        Image rune28Image;
        Image rune29Image;
        Image rune30Image;
        #endregion
        ListView runePageInfoListView;
        TextBlock runePageNameTextBlock;
        ComboBox runePageComboBox;
        ObservableCollection<RunePageInfoListViewBinding> runeInfoCollection;

        bool justLoaded = true;

        public SummonerPage()
        {
            this.InitializeComponent();

            leagueEntriesCollection = new ObservableCollection<LeagueListViewBinding>();
            recentGamesCollection = new ObservableCollection<RecentGameListViewBinding>();
            runeInfoCollection = new ObservableCollection<RunePageInfoListViewBinding>();
            runePages = new Dictionary<string, RunePages>();
            runeValues = new Dictionary<string, RunePageInfoListViewBinding>();
            runeImages = new List<Image>();
            leagueKey = new List<League>();
        }

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

            recentGames = await summoner.RetrieveRecentGames();
            foreach (Game game in recentGames.games)
            {
                recentGamesCollection.Add(new RecentGameListViewBinding(game));
            }

            runePages = await summoner.RetrieveRunePages();
            LoadRuneImages();
            foreach (RunePage page in runePages[summoner.id.ToString()].pages)
            {
                ComboBoxItem item = new ComboBoxItem();
                if (page.current)
                {
                    item.Content = page.name + " - Active";
                }
                else
                {
                    item.Content = page.name;
                }
                runePageComboBox.Items.Add(item);
            }
            runePageComboBox.SelectedIndex = 0;
            LoadRunePage(runePages[summoner.id.ToString()].pages[0]);
            justLoaded = false;
        }

        void LoadRunePage(RunePage page)
        {
            runeInfoCollection.Clear();
            runeValues.Clear();
            runePageNameTextBlock.Text = page.name;
            string numberPart = "[\\+|-][\\d\\W]+";
            for (int i = 1; i <= 30; i++)
            {
                int runeId = 0;
                foreach (RuneSlot runeSlot in page.slots)
                {
                    if (runeSlot.runeSlotId == i)
                    {
                        runeId = runeSlot.runeId;
                        break;
                    }
                }
                RuneStatic rune = AppConstants.runesData.data[runeId.ToString()];
                string runeDescriptionModified = "";

                if (rune.sanitizedDescription.Contains('('))
                {
                    int index = rune.sanitizedDescription.IndexOf('(');
                    runeDescriptionModified = rune.sanitizedDescription.Substring(0, index);
                }
                else
                {
                    runeDescriptionModified = rune.sanitizedDescription;
                }

                string runeData = Regex.Match(runeDescriptionModified, numberPart).ToString();
                string runeInfo = AppConstants.ToRegularCase(runeDescriptionModified.Substring(runeData.Length));
                List<double> nonNullRuneValues = new List<double>();
                //foreach (double? value in rune.stats.values)
                //{
                //    if (value != null)
                //    {
                //        nonNullRuneValues.Add((double)value);
                //    }
                //}
                
                if (nonNullRuneValues.Count == 1)
                {
                    if (!runeValues.ContainsKey(runeInfo))
                    {
                        RunePageInfoListViewBinding binding = new RunePageInfoListViewBinding(runeInfo,
                            nonNullRuneValues[0],
                            runeData.Contains('%'),
                            runeData.Contains('+'),
                            runeInfo.Contains("Per Level"));
                        runeValues.Add(runeInfo, binding);
                    }
                    else
                    {
                        runeValues[runeInfo].AddToValue(nonNullRuneValues[0]);
                    }
                }
                else if (nonNullRuneValues.Count == 2)
                {
                    if (!runeValues.ContainsKey("armor penetration"))
                    {
                        RunePageInfoListViewBinding binding = new RunePageInfoListViewBinding("armor penetration",
                            nonNullRuneValues[0],
                            runeData.Contains('%'),
                            runeData.Contains('+'),
                            runeInfo.Contains("Per Level"));
                        runeValues.Add("armor penetration", binding);
                    }
                    else
                    {
                        runeValues["armor penetration"].AddToValue(nonNullRuneValues[0]);
                    }

                    if (!runeValues.ContainsKey("magic penetration"))
                    {
                        RunePageInfoListViewBinding binding = new RunePageInfoListViewBinding("magic penetration",
                            nonNullRuneValues[1],
                            runeData.Contains('%'),
                            runeData.Contains('+'),
                            runeInfo.Contains("Per Level"));
                        runeValues.Add("magic penetration", binding);
                    }
                    else
                    {
                        runeValues["magic penetration"].AddToValue(nonNullRuneValues[1]);
                    }
                }

                runeImages[i - 1].Source = AppConstants.SetImageSource(new Uri(AppConstants.RuneIconUrl() + rune.image.full));
                ToolTip toolTip = new ToolTip();
                toolTip.Content = rune.name + "\n" + rune.sanitizedDescription;
                ToolTipService.SetToolTip(runeImages[i - 1], toolTip);
            }

            foreach (KeyValuePair<string, RunePageInfoListViewBinding> binding in runeValues)
            {
                if (binding.Value.plus)
                {
                    binding.Value.RuneValue = "+";
                }

                if (binding.Value.isPercent)
                {
                    binding.Value.value *= 100;
                    binding.Value.RuneValue += binding.Value.value.ToString() + "%";
                }
                else
                {
                    binding.Value.RuneValue += binding.Value.value.ToString("0.###");
                }

                if (binding.Value.perLevel)
                {
                    binding.Value.RuneValue += " (" + binding.Value.value * 18 + " @ lvl 18)";
                }

                runeInfoCollection.Add(binding.Value);
            }
        }

        void LoadRuneImages()
        {
            runeImages.Add(rune1Image);
            runeImages.Add(rune2Image);
            runeImages.Add(rune3Image);
            runeImages.Add(rune4Image);
            runeImages.Add(rune5Image);
            runeImages.Add(rune6Image);
            runeImages.Add(rune7Image);
            runeImages.Add(rune8Image);
            runeImages.Add(rune9Image);
            runeImages.Add(rune10Image);
            runeImages.Add(rune11Image);
            runeImages.Add(rune12Image);
            runeImages.Add(rune13Image);
            runeImages.Add(rune14Image);
            runeImages.Add(rune15Image);
            runeImages.Add(rune16Image);
            runeImages.Add(rune17Image);
            runeImages.Add(rune18Image);
            runeImages.Add(rune19Image);
            runeImages.Add(rune20Image);
            runeImages.Add(rune21Image);
            runeImages.Add(rune22Image);
            runeImages.Add(rune23Image);
            runeImages.Add(rune24Image);
            runeImages.Add(rune25Image);
            runeImages.Add(rune26Image);
            runeImages.Add(rune27Image);
            runeImages.Add(rune28Image);
            runeImages.Add(rune29Image);
            runeImages.Add(rune30Image);
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

        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }

        private void leagueListView_Loaded(object sender, RoutedEventArgs e)
        {
            leagueListView = sender as ListView;
            leagueListView.ItemsSource = leagueEntriesCollection;
        }

        private void leagueBorder_Loaded(object sender, RoutedEventArgs e)
        {
            Border border = sender as Border;
            border.BorderBrush = AppConstants.themeColor;
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

        private void downDivisonButton_Loaded(object sender, RoutedEventArgs e)
        {
            downDivisionButton = sender as Button;
        }

        private void tierNameTextBlock_Loaded(object sender, RoutedEventArgs e)
        {
            tierNameTextBlock = sender as TextBlock;
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

        private void recentGamesListView_Loaded(object sender, RoutedEventArgs e)
        {
            recentGamesListView = sender as ListView;
            recentGamesListView.ItemsSource = recentGamesCollection;
        }

        #region Rune Images
        private void rune1Image_Loaded(object sender, RoutedEventArgs e)
        {
            rune1Image = sender as Image;
        }

        private void rune2Image_Loaded(object sender, RoutedEventArgs e)
        {
            rune2Image = sender as Image;
        }

        private void rune3Image_Loaded(object sender, RoutedEventArgs e)
        {
            rune3Image = sender as Image;
        }

        private void rune4Image_Loaded(object sender, RoutedEventArgs e)
        {
            rune4Image = sender as Image;
        }

        private void rune5Image_Loaded(object sender, RoutedEventArgs e)
        {
            rune5Image = sender as Image;
        }

        private void rune6Image_Loaded(object sender, RoutedEventArgs e)
        {
            rune6Image = sender as Image;
        }

        private void rune7Image_Loaded(object sender, RoutedEventArgs e)
        {
            rune7Image = sender as Image;
        }

        private void rune8Image_Loaded(object sender, RoutedEventArgs e)
        {
            rune8Image = sender as Image;
        }

        private void rune9Image_Loaded(object sender, RoutedEventArgs e)
        {
            rune9Image = sender as Image;
        }

        private void rune10Image_Loaded(object sender, RoutedEventArgs e)
        {
            rune10Image = sender as Image;
        }

        private void rune11Image_Loaded(object sender, RoutedEventArgs e)
        {
            rune11Image = sender as Image;
        }

        private void rune12Image_Loaded(object sender, RoutedEventArgs e)
        {
            rune12Image = sender as Image;
        }

        private void rune13Image_Loaded(object sender, RoutedEventArgs e)
        {
            rune13Image = sender as Image;
        }

        private void rune14Image_Loaded(object sender, RoutedEventArgs e)
        {
            rune14Image = sender as Image;
        }

        private void rune15Image_Loaded(object sender, RoutedEventArgs e)
        {
            rune15Image = sender as Image;
        }

        private void rune16Image_Loaded(object sender, RoutedEventArgs e)
        {
            rune16Image = sender as Image;
        }

        private void rune17Image_Loaded(object sender, RoutedEventArgs e)
        {
            rune17Image = sender as Image;
        }

        private void rune18Image_Loaded(object sender, RoutedEventArgs e)
        {
            rune18Image = sender as Image;
        }

        private void rune19Image_Loaded(object sender, RoutedEventArgs e)
        {
            rune19Image = sender as Image;
        }

        private void rune20Image_Loaded(object sender, RoutedEventArgs e)
        {
            rune20Image = sender as Image;
        }

        private void rune21Image_Loaded(object sender, RoutedEventArgs e)
        {
            rune21Image = sender as Image;
        }

        private void rune22Image_Loaded(object sender, RoutedEventArgs e)
        {
            rune22Image = sender as Image;
        }

        private void rune23Image_Loaded(object sender, RoutedEventArgs e)
        {
            rune23Image = sender as Image;
        }

        private void rune24Image_Loaded(object sender, RoutedEventArgs e)
        {
            rune24Image = sender as Image;
        }

        private void rune25Image_Loaded(object sender, RoutedEventArgs e)
        {
            rune25Image = sender as Image;
        }

        private void rune26Image_Loaded(object sender, RoutedEventArgs e)
        {
            rune26Image = sender as Image;
        }

        private void rune27Image_Loaded(object sender, RoutedEventArgs e)
        {
            rune27Image = sender as Image;
        }

        private void rune28Image_Loaded(object sender, RoutedEventArgs e)
        {
            rune28Image = sender as Image;
        }

        private void rune29Image_Loaded(object sender, RoutedEventArgs e)
        {
            rune29Image = sender as Image;
        }

        private void rune30Image_Loaded(object sender, RoutedEventArgs e)
        {
            rune30Image = sender as Image;
        }
        #endregion

        private void runePageNameTextBlock_Loaded(object sender, RoutedEventArgs e)
        {
            runePageNameTextBlock = sender as TextBlock;
        }

        private void runePageComboBox_Loaded(object sender, RoutedEventArgs e)
        {
            runePageComboBox = sender as ComboBox;
        }

        private void runePageComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!justLoaded)
            {
                LoadRunePage(runePages[summoner.id.ToString()].pages[runePageComboBox.SelectedIndex]);
            }
        }

        private void runePageInfoListView_Loaded(object sender, RoutedEventArgs e)
        {
            runePageInfoListView = sender as ListView;
            runePageInfoListView.ItemsSource = runeInfoCollection;
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

        private async void onlineMatchHistoryButton_Click(object sender, RoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri("http://matchhistory.na.leagueoflegends.com/en/#match-history/NA/40669849"));
        }
    }
}
