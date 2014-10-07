using CreepScoreApp.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Windows.ApplicationModel.Search;
using CreepScoreAPI;
using CreepScoreApp.Constants;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace CreepScoreApp
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class ChampionsPage : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        double screenHeight;
        ObservableCollection<ChampionsGridViewBinding> championsCollection;
        SortedDictionary<string, ChampionStatic> sortedChampions;

        bool justLaunchedPage = true;
        string selectedRole = "All";

        /// <summary>
        /// This can be changed to a strongly typed view model.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        /// <summary>
        /// NavigationHelper is used on each page to aid in navigation and 
        /// process lifetime management
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }


        public ChampionsPage()
        {
            this.InitializeComponent();
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += navigationHelper_LoadState;
            this.navigationHelper.SaveState += navigationHelper_SaveState;
            this.SizeChanged += ChampionsPage_SizeChanged;
        }

        /// <summary>
        /// Populates the page with content passed during navigation. Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        /// The source of the event; typically <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Event data that provides both the navigation parameter passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested and
        /// a dictionary of state preserved by this page during an earlier
        /// session. The state will be null the first time a page is visited.</param>
        private void navigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            List<object> parameters = (List<object>)e.NavigationParameter;
            screenHeight = (double)parameters[0];

            sortedChampions = new SortedDictionary<string, ChampionStatic>();

            foreach (KeyValuePair<string, ChampionStatic> champion in AppConstants.championsData.data)
            {
                sortedChampions.Add(champion.Key, champion.Value);
            }
            championsCollection = new ObservableCollection<ChampionsGridViewBinding>();
            RefreshChampionsCollection();

            justLaunchedPage = false;
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="sender">The source of the event; typically <see cref="NavigationHelper"/></param>
        /// <param name="e">Event data that provides an empty dictionary to be populated with
        /// serializable state.</param>
        private void navigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
            this.SizeChanged -= ChampionsPage_SizeChanged;
        }

        void ChampionsPage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            screenHeight = e.NewSize.Height;
            RefreshChampionsCollection();
        }

        #region NavigationHelper registration

        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// 
        /// Page specific logic should be placed in event handlers for the  
        /// <see cref="GridCS.Common.NavigationHelper.LoadState"/>
        /// and <see cref="GridCS.Common.NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private void championsGridView_Loaded(object sender, RoutedEventArgs e)
        {
            championsGridView.ItemsSource = championsCollection;
        }

        private void roleComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!justLaunchedPage)
            {
                selectedRole = (string)((ComboBoxItem)roleComboBox.SelectedItem).Content;
                RefreshChampionsCollection();
            }
        }

        void RefreshChampionsCollection()
        {
            championsCollection.Clear();
            if (selectedRole == "All")
            {
                foreach (KeyValuePair<string, ChampionStatic> champion in sortedChampions)
                {
                    if (screenHeight >= 1440)
                    {
                        championsCollection.Add(new ChampionsGridViewBinding(new Uri(AppConstants.championLoadingUrl + champion.Key + "_0.jpg"), 308, 560, champion.Key, champion.Value));
                    }
                    else
                    {
                        championsCollection.Add(new ChampionsGridViewBinding(new Uri(AppConstants.ChampionIconUrl() + champion.Key + ".png"), 120, 120, champion.Key, champion.Value));
                    }
                }
            }
            else
            {
                foreach (KeyValuePair<string, ChampionStatic> champion in sortedChampions)
                {
                    if (champion.Value.tags[0] == selectedRole)
                    {
                        if (screenHeight >= 1440)
                        {
                            championsCollection.Add(new ChampionsGridViewBinding(new Uri(AppConstants.championLoadingUrl + champion.Key + "_0.jpg"), 308, 560, champion.Key, champion.Value));
                        }
                        else
                        {
                            championsCollection.Add(new ChampionsGridViewBinding(new Uri(AppConstants.ChampionIconUrl() + champion.Key + ".png"), 120, 120, champion.Key, champion.Value));
                        }
                    }
                }
            }
        }

        private void championSearchBox_SuggestionsRequested(SearchBox sender, SearchBoxSuggestionsRequestedEventArgs args)
        {
            string search = args.QueryText;

            if (search != "")
            {
                SearchSuggestionCollection suggestionCollegion = args.Request.SearchSuggestionCollection;
                championsCollection.Clear();
                foreach (KeyValuePair<string, ChampionStatic> champion in sortedChampions)
                {
                    if (champion.Value.name.ToLower().Contains(search.ToLower()))
                    {
                        if (selectedRole == "All")
                        {
                            suggestionCollegion.AppendQuerySuggestion(champion.Value.name);
                            if (screenHeight >= 1440)
                            {
                                championsCollection.Add(new ChampionsGridViewBinding(new Uri(AppConstants.championLoadingUrl + champion.Key + "_0.jpg"), 308, 560, champion.Key, champion.Value));
                            }
                            else
                            {
                                championsCollection.Add(new ChampionsGridViewBinding(new Uri(AppConstants.ChampionIconUrl() + champion.Key + ".png"), 120, 120, champion.Key, champion.Value));
                            }
                        }
                        else
                        {
                            if (champion.Value.tags[0] == selectedRole)
                            {
                                suggestionCollegion.AppendQuerySuggestion(champion.Value.name);
                                if (screenHeight >= 1440)
                                {
                                    championsCollection.Add(new ChampionsGridViewBinding(new Uri(AppConstants.championLoadingUrl + champion.Key + "_0.jpg"), 308, 560, champion.Key, champion.Value));
                                }
                                else
                                {
                                    championsCollection.Add(new ChampionsGridViewBinding(new Uri(AppConstants.ChampionIconUrl() + champion.Key + ".png"), 120, 120, champion.Key, champion.Value));
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                RefreshChampionsCollection();
            }
        }

        private void championsGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            ChampionsGridViewBinding championItem = (ChampionsGridViewBinding)e.ClickedItem;

            List<object> parameters = new List<object>();
            KeyValuePair<string, ChampionStatic> champion = new KeyValuePair<string, ChampionStatic>(championItem.dataName, championItem.champion);
            parameters.Add(champion);
            Frame.Navigate(typeof(ChampionDetailPage), parameters);
        }
    }
}
