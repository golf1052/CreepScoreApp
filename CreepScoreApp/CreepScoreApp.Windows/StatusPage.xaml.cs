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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace CreepScoreApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class StatusPage : Page
    {
        ShardStatus shardStatus;

        public StatusPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            List<object> parameters = ((List<object>)e.Parameter);
            shardStatus = (ShardStatus)parameters[0];
            foreach (Service service in shardStatus.services)
            {
                if (service.name == "Game")
                {
                    if (service.status == "online")
                    {
                        gameEllipse.Fill = AppConstants.metroGreen;
                    }
                    else
                    {
                        gameEllipse.Fill = AppConstants.metroYellow;
                    }
                }
                else if (service.name == "Store")
                {
                    if (service.status == "online")
                    {
                        storeEllipse.Fill = AppConstants.metroGreen;
                    }
                    else
                    {
                        storeEllipse.Fill = AppConstants.metroYellow;
                    }
                }
                else if (service.name == "Boards")
                {
                    if (service.status == "online")
                    {
                        boardsEllipse.Fill = AppConstants.metroGreen;
                    }
                    else
                    {
                        boardsEllipse.Fill = AppConstants.metroYellow;
                    }
                }
                else if (service.name == "Website")
                {
                    if (service.status == "online")
                    {
                        websiteEllipse.Fill = AppConstants.metroGreen;
                    }
                    else
                    {
                        websiteEllipse.Fill = AppConstants.metroYellow;
                    }
                }
            }
        }

        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }
    }
}
