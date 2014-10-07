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
using Windows.UI.Xaml.Media.Imaging;
using System.Diagnostics;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace CreepScoreApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ChampionAbilitiesDetailPage : Page
    {
        KeyValuePair<string, ChampionStatic> champion;

        public ChampionAbilitiesDetailPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            List<object> parameters = (List<object>)e.Parameter;
            champion = (KeyValuePair<string, ChampionStatic>)parameters[0];
        }

        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }

        private void championFullNameTextBlock_Loaded(object sender, RoutedEventArgs e)
        {
            championFullNameTextBlock.Text = champion.Value.name;
        }

        private void abilitiesHub_Loaded(object sender, RoutedEventArgs e)
        {
            passiveHeaderTextBlock.Text = champion.Value.passive.name;
            BitmapImage passiveBitmapImage = new BitmapImage(new Uri(AppConstants.ChampionPassiveIconUrl() + champion.Value.passive.image.full));
            passiveHeaderImage.Source = passiveBitmapImage;
            ability1HeaderTextBlock.Text = champion.Value.spells[0].name;
            BitmapImage ability1BitmapImage = new BitmapImage(new Uri(AppConstants.SpellIconUrl() + champion.Value.spells[0].image.full));
            ability1HeaderImage.Source = ability1BitmapImage;
            ability2HeaderTextBlock.Text = champion.Value.spells[1].name;
            BitmapImage ability2BitmapImage = new BitmapImage(new Uri(AppConstants.SpellIconUrl() + champion.Value.spells[1].image.full));
            ability2HeaderImage.Source = ability2BitmapImage;
            ability3HeaderTextBlock.Text = champion.Value.spells[2].name;
            BitmapImage ability3BitmapImage = new BitmapImage(new Uri(AppConstants.SpellIconUrl() + champion.Value.spells[2].image.full));
            ability3HeaderImage.Source = ability3BitmapImage;
            ability4HeaderTextBlock.Text = champion.Value.spells[3].name;
            BitmapImage ability4BitmapImage = new BitmapImage(new Uri(AppConstants.SpellIconUrl() + champion.Value.spells[3].image.full));
            ability4HeaderImage.Source = ability4BitmapImage;

            passiveHubSection.Width = 500;
            ability1HubSection.Width = 500;
            ability2HubSection.Width = 500;
            ability3HubSection.Width = 500;
            ability4HubSection.Width = 500;
        }

        private void passiveDescriptionTextBlock_Loaded(object sender, RoutedEventArgs e)
        {
            TextBlock textBlock = sender as TextBlock;
            textBlock.Text = "Description: " + champion.Value.passive.sanitizedDescription;
        }

        private void ability1DescriptionTextBlock_Loaded(object sender, RoutedEventArgs e)
        {
            TextBlock textBlock = sender as TextBlock;
            textBlock.Text = "Description: " + AppConstants.PopulateBracketData(champion.Value.spells[0], champion.Value.spells[0].sanitizedTooltip);
        }

        private void ability2DescriptionTextBlock_Loaded(object sender, RoutedEventArgs e)
        {
            TextBlock textBlock = sender as TextBlock;
            textBlock.Text = "Description: " + AppConstants.PopulateBracketData(champion.Value.spells[1], champion.Value.spells[1].sanitizedTooltip);
        }

        private void ability3DescriptionTextBlock_Loaded(object sender, RoutedEventArgs e)
        {
            TextBlock textBlock = sender as TextBlock;
            textBlock.Text = "Description: " + AppConstants.PopulateBracketData(champion.Value.spells[2], champion.Value.spells[2].sanitizedTooltip);
        }

        private void ability4DescriptionTextBlock_Loaded(object sender, RoutedEventArgs e)
        {
            TextBlock textBlock = sender as TextBlock;
            textBlock.Text = "Description: " + AppConstants.PopulateBracketData(champion.Value.spells[3], champion.Value.spells[3].sanitizedTooltip);
        }
    }
}
