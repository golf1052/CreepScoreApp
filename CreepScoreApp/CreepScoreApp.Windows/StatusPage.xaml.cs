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
using CreepScoreAPI;
using CreepScoreApp.Constants;
using Windows.UI.Popups;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace CreepScoreApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class StatusPage : Page
    {
        ShardStatus shardStatus;
        ObservableCollection<StatusListViewBinding> gameIncidents;
        ObservableCollection<StatusListViewBinding> storeIncidents;
        ObservableCollection<StatusListViewBinding> boardIncidents;
        ObservableCollection<StatusListViewBinding> websiteIncidents;
        bool justLoaded;

        ListView gameListView;
        ListView storeListView;
        ListView boardsListView;
        ListView websiteListView;

        RegionComboBoxSettings regionComboBoxSettings;

        public StatusPage()
        {
            justLoaded = true;
            this.InitializeComponent();
            gameIncidents = new ObservableCollection<StatusListViewBinding>();
            storeIncidents = new ObservableCollection<StatusListViewBinding>();
            boardIncidents = new ObservableCollection<StatusListViewBinding>();
            websiteIncidents = new ObservableCollection<StatusListViewBinding>();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            List<object> parameters = ((List<object>)e.Parameter);
            shardStatus = (ShardStatus)parameters[0];
            LoadStatus();
            justLoaded = false;
        }

        async void RefreshStatus(CreepScore.Region region)
        {
            shardStatus = await AppConstants.creepScore.RetrieveShardStatus(region);
            LoadStatus();
        }

        async void LoadStatus()
        {
            gameIncidents.Clear();
            storeIncidents.Clear();
            boardIncidents.Clear();
            websiteIncidents.Clear();
            if (shardStatus != null)
            {
                foreach (Service service in shardStatus.services)
                {
                    if (service.name == "Game")
                    {
                        if (service.status == "online")
                        {
                            gameEllipse.Fill = AppConstants.metroGreen;
                        }
                        else if (service.status == "offline")
                        {
                            gameEllipse.Fill = AppConstants.metroRed;
                        }
                        else
                        {
                            gameEllipse.Fill = AppConstants.metroYellow;
                        }

                        foreach (Incident incident in service.incidents)
                        {
                            foreach (Message message in incident.updates)
                            {
                                DateTime dateTime = DateTime.Parse(message.updatedAt).ToLocalTime();
                                string date = dateTime.ToString("f");
                                gameIncidents.Add(new StatusListViewBinding(message.author, date, message.content));
                            }
                        }

                        if (gameIncidents.Count == 0)
                        {
                            if (gameListView != null)
                            {
                                gameListView.Width = 0;
                            }
                        }
                        else
                        {
                            if (gameListView != null)
                            {
                                gameListView.Width = 500;
                            }
                        }
                    }
                    else if (service.name == "Store")
                    {
                        if (service.status == "online")
                        {
                            storeEllipse.Fill = AppConstants.metroGreen;
                        }
                        else if (service.status == "offline")
                        {
                            storeEllipse.Fill = AppConstants.metroRed;
                        }
                        else
                        {
                            storeEllipse.Fill = AppConstants.metroYellow;
                        }

                        foreach (Incident incident in service.incidents)
                        {
                            foreach (Message message in incident.updates)
                            {
                                DateTime dateTime = DateTime.Parse(message.updatedAt).ToLocalTime();
                                string date = dateTime.ToString("f");
                                storeIncidents.Add(new StatusListViewBinding(message.author, date, message.content));
                            }
                        }

                        if (storeIncidents.Count == 0)
                        {
                            if (storeListView != null)
                            {
                                storeListView.Width = 0;
                            }
                        }
                        else
                        {
                            if (storeListView != null)
                            {
                                storeListView.Width = 500;
                            }
                        }
                    }
                    else if (service.name == "Boards")
                    {
                        if (service.status == "online")
                        {
                            boardsEllipse.Fill = AppConstants.metroGreen;
                        }
                        else if (service.status == "offline")
                        {
                            boardsEllipse.Fill = AppConstants.metroRed;
                        }
                        else
                        {
                            boardsEllipse.Fill = AppConstants.metroYellow;
                        }

                        foreach (Incident incident in service.incidents)
                        {
                            foreach (Message message in incident.updates)
                            {
                                DateTime dateTime = DateTime.Parse(message.updatedAt).ToLocalTime();
                                string date = dateTime.ToString("f");
                                boardIncidents.Add(new StatusListViewBinding(message.author, date, message.content));
                            }
                        }

                        if (boardIncidents.Count == 0)
                        {
                            if (boardsListView != null)
                            {
                                boardsListView.Width = 0;
                            }
                        }
                        else
                        {
                            if (boardsListView != null)
                            {
                                boardsListView.Width = 500;
                            }
                        }
                    }
                    else if (service.name == "Website")
                    {
                        if (service.status == "online")
                        {
                            websiteEllipse.Fill = AppConstants.metroGreen;
                        }
                        else if (service.status == "offline")
                        {
                            websiteEllipse.Fill = AppConstants.metroRed;
                        }
                        else
                        {
                            websiteEllipse.Fill = AppConstants.metroYellow;
                        }

                        foreach (Incident incident in service.incidents)
                        {
                            foreach (Message message in incident.updates)
                            {
                                DateTime dateTime = DateTime.Parse(message.updatedAt).ToLocalTime();
                                string date = dateTime.ToString("f");
                                websiteIncidents.Add(new StatusListViewBinding(message.author, date, message.content));
                            }
                        }

                        if (websiteIncidents.Count == 0)
                        {
                            if (websiteListView != null)
                            {
                                websiteListView.Width = 0;
                            }
                        }
                        else
                        {
                            if (websiteListView != null)
                            {
                                websiteListView.Width = 500;
                            }
                        }
                    }
                }
            }
            else
            {
                MessageDialog message = new MessageDialog("Could not get service status. Try refreshing.");
                await message.ShowAsync();
            }
        }

        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }

        private void refreshStatusButton_Click(object sender, RoutedEventArgs e)
        {
            RefreshStatus((CreepScore.Region)AppConstants.GetCreepScoreRegion(regionComboBox.SelectedIndex));
        }

        private void regionComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!justLoaded)
            {
                RefreshStatus((CreepScore.Region)AppConstants.GetCreepScoreRegion(regionComboBox.SelectedIndex));
            }
        }

        private void gameListView_Loaded(object sender, RoutedEventArgs e)
        {
            gameListView = sender as ListView;
            gameListView.ItemsSource = gameIncidents;
        }

        private void storeListView_Loaded(object sender, RoutedEventArgs e)
        {
            storeListView = sender as ListView;
            storeListView.ItemsSource = storeIncidents;
        }

        private void boardsListView_Loaded(object sender, RoutedEventArgs e)
        {
            boardsListView = sender as ListView;
            boardsListView.ItemsSource = boardIncidents;
        }

        private void websiteListView_Loaded(object sender, RoutedEventArgs e)
        {
            websiteListView = sender as ListView;
            websiteListView.ItemsSource = websiteIncidents;
        }

        private void regionComboBox_Loaded(object sender, RoutedEventArgs e)
        {
            regionComboBoxSettings = new RegionComboBoxSettings(regionComboBox, false);
        }
    }
}
