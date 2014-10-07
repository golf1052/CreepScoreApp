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
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Shapes;
using CreepScoreAPI;
using CreepScoreApp.Constants;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace CreepScoreApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ChampionDetailPage : Page
    {
        KeyValuePair<string, ChampionStatic> champion;

        BitmapImage championBitmap;
        ObservableCollection<SkinsGridViewBinding> skinsCollection;

        bool justLoadedPage = true;

        public ChampionDetailPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            List<object> parameters = (List<object>)e.Parameter;
            champion = (KeyValuePair<string, ChampionStatic>)parameters[0];

            skinsCollection = new ObservableCollection<SkinsGridViewBinding>();
            for (int i = 0; i < champion.Value.skins.Count; i++)
            {
                if (i == 0)
                {
                    skinsCollection.Add(new SkinsGridViewBinding(new Uri(AppConstants.championLoadingUrl + champion.Key + "_" + i.ToString() + ".jpg"), "Classic"));
                }
                else
                {
                    skinsCollection.Add(new SkinsGridViewBinding(new Uri(AppConstants.championLoadingUrl + champion.Key + "_" + i.ToString() + ".jpg"), champion.Value.skins[i].name));
                }
            }

            justLoadedPage = false;
        }

        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }

        private void championMainSection_Loaded(object sender, RoutedEventArgs e)
        {
            championBitmap = new BitmapImage(new Uri(AppConstants.championSplashUrl + champion.Key + "_0.jpg"));
            championMainSection.Height = mainGrid.ActualHeight;
            championMainSection.Width = championMainSection.Height * 1.694560669456067;
            championImage.ImageSource = championBitmap;
        }

        private void championFullNameTextBlock_Loaded(object sender, RoutedEventArgs e)
        {
            championFullNameTextBlock.Text = champion.Value.name + " " + champion.Value.title;
        }

        private void skinsGridView_Loaded(object sender, RoutedEventArgs e)
        {
            GridView gridView = sender as GridView;
            gridView.ItemsSource = skinsCollection;
            gridView.SelectedIndex = 0;
        }

        private void skinsGridView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!justLoadedPage)
            {
                GridView gridView = sender as GridView;
                championBitmap = new BitmapImage(new Uri(AppConstants.championSplashUrl + champion.Key + "_" + gridView.SelectedIndex.ToString() + ".jpg"));
                championImage.ImageSource = championBitmap;
            }
        }

        private void skinsGridViewGrid_Loaded(object sender, RoutedEventArgs e)
        {
            Grid grid = sender as Grid;
            grid.Margin = new Thickness(0, mainGrid.ActualHeight - 662, 0, 0);
        }

        private void attackRectangle_Loaded(object sender, RoutedEventArgs e)
        {
            Rectangle rect = sender as Rectangle;
            rect.Width = champion.Value.info.attack * 20;
            ToolTip toolTip = new ToolTip();
            toolTip.Content = "Attack power: " + champion.Value.info.attack;
            ToolTipService.SetToolTip(rect, toolTip);
        }

        private void defenseRectangle_Loaded(object sender, RoutedEventArgs e)
        {
            Rectangle rect = sender as Rectangle;
            rect.Width = champion.Value.info.defense * 20;
            ToolTip toolTip = new ToolTip();
            toolTip.Content = "Defense power: " + champion.Value.info.defense;
            ToolTipService.SetToolTip(rect, toolTip);
        }

        private void abilityRectangle_Loaded(object sender, RoutedEventArgs e)
        {
            Rectangle rect = sender as Rectangle;
            rect.Width = champion.Value.info.magic * 20;
            ToolTip toolTip = new ToolTip();
            toolTip.Content = "Ability power: " + champion.Value.info.magic;
            ToolTipService.SetToolTip(rect, toolTip);
        }

        private void difficultyRectangle_Loaded(object sender, RoutedEventArgs e)
        {
            Rectangle rect = sender as Rectangle;
            rect.Width = champion.Value.info.difficulty * 20;
            ToolTip toolTip = new ToolTip();
            toolTip.Content = "Difficulty: " + champion.Value.info.difficulty;
            ToolTipService.SetToolTip(rect, toolTip);
        }

        private void healthTextBlock_Loaded(object sender, RoutedEventArgs e)
        {
            TextBlock textBlock = sender as TextBlock;
            textBlock.Text = champion.Value.stats.hp + " (+" + champion.Value.stats.hpPerLevel + ")";
        }

        private void healthRegenTextBlock_Loaded(object sender, RoutedEventArgs e)
        {
            TextBlock textBlock = sender as TextBlock;
            textBlock.Text = champion.Value.stats.hpRegen + " (+" + champion.Value.stats.hpRegenPerLevel + ")";
        }

        private void manaTextBlock_Loaded(object sender, RoutedEventArgs e)
        {
            TextBlock textBlock = sender as TextBlock;
            textBlock.Text = champion.Value.stats.mp + " (+" + champion.Value.stats.mpPerLevel + ")";
        }

        private void manaRegenTextBlock_Loaded(object sender, RoutedEventArgs e)
        {
            TextBlock textBlock = sender as TextBlock;
            textBlock.Text = champion.Value.stats.mpRegen + " (+" + champion.Value.stats.mpRegenPerLevel + ")";
        }

        private void rangeTextBlock_Loaded(object sender, RoutedEventArgs e)
        {
            TextBlock textBlock = sender as TextBlock;
            textBlock.Text = champion.Value.stats.attackRange.ToString();
        }

        private void attackDamageTextBlock_Loaded(object sender, RoutedEventArgs e)
        {
            TextBlock textBlock = sender as TextBlock;
            textBlock.Text = champion.Value.stats.attackDamage + " (+" + champion.Value.stats.attackDamagePerLevel + ")";
        }

        private void attackSpeedTextBlock_Loaded(object sender, RoutedEventArgs e)
        {
            TextBlock textBlock = sender as TextBlock;
            double attackSpeedBase = 1 / (1.6 * (1 + champion.Value.stats.attackSpeedOffset));
            textBlock.Text = attackSpeedBase.ToString("0.000") + " (+" + champion.Value.stats.attackSpeedPerLevel + "%)";
        }

        private void armorTextBlock_Loaded(object sender, RoutedEventArgs e)
        {
            TextBlock textBlock = sender as TextBlock;
            textBlock.Text = champion.Value.stats.armor + " (+" + champion.Value.stats.armorPerLevel + ")";
        }

        private void magicResTextBlock_Loaded(object sender, RoutedEventArgs e)
        {
            TextBlock textBlock = sender as TextBlock;
            textBlock.Text = champion.Value.stats.spellBlock + " (+" + champion.Value.stats.spellBlockPerLevel + ")";
        }

        private void moveSpeedTextBlock_Loaded(object sender, RoutedEventArgs e)
        {
            TextBlock textBlock = sender as TextBlock;
            textBlock.Text = champion.Value.stats.moveSpeed.ToString();
        }

        private void powerTypeTextBlock_Loaded(object sender, RoutedEventArgs e)
        {
            TextBlock textBlock = sender as TextBlock;
            textBlock.Text = champion.Value.parType;
        }

        private void powerTypeRegenTextBlock_Loaded(object sender, RoutedEventArgs e)
        {
            TextBlock textBlock = sender as TextBlock;
            textBlock.Text = champion.Value.parType + " Regen.";
        }

        private void passiveImage_Loaded(object sender, RoutedEventArgs e)
        {
            Image image = sender as Image;
            BitmapImage bitmapImage = new BitmapImage(new Uri(AppConstants.ChampionPassiveIconUrl() + champion.Value.passive.image.full));
            image.Source = bitmapImage;
        }

        private void passiveNameTextBlock_Loaded(object sender, RoutedEventArgs e)
        {
            TextBlock textBlock = sender as TextBlock;
            textBlock.Text = champion.Value.passive.name;
        }

        private void passiveDescriptionTextBlock_Loaded(object sender, RoutedEventArgs e)
        {
            TextBlock textBlock = sender as TextBlock;
            textBlock.Text = champion.Value.passive.sanitizedDescription;
        }

        private void ability1TextBlock_Loaded(object sender, RoutedEventArgs e)
        {
            TextBlock textBlock = sender as TextBlock;
            textBlock.Text = champion.Value.spells[0].name;
        }

        private void ability1Image_Loaded(object sender, RoutedEventArgs e)
        {
            Image image = sender as Image;
            BitmapImage bitmapImage = new BitmapImage(new Uri(AppConstants.SpellIconUrl() + champion.Value.spells[0].image.full));
            image.Source = bitmapImage;
        }

        private void ability1DescriptionTextBlock_Loaded(object sender, RoutedEventArgs e)
        {
            TextBlock textBlock = sender as TextBlock;
            textBlock.Text = champion.Value.spells[0].sanitizedDescription;
        }

        private void ability2TextBlock_Loaded(object sender, RoutedEventArgs e)
        {
            TextBlock textBlock = sender as TextBlock;
            textBlock.Text = champion.Value.spells[1].name;
        }

        private void ability2Image_Loaded(object sender, RoutedEventArgs e)
        {
            Image image = sender as Image;
            BitmapImage bitmapImage = new BitmapImage(new Uri(AppConstants.SpellIconUrl() + champion.Value.spells[1].image.full));
            image.Source = bitmapImage;
        }

        private void ability2DescriptionTextBlock_Loaded(object sender, RoutedEventArgs e)
        {
            TextBlock textBlock = sender as TextBlock;
            textBlock.Text = champion.Value.spells[1].sanitizedDescription;
        }

        private void ability3TextBlock_Loaded(object sender, RoutedEventArgs e)
        {
            TextBlock textBlock = sender as TextBlock;
            textBlock.Text = champion.Value.spells[2].name;
        }

        private void ability3Image_Loaded(object sender, RoutedEventArgs e)
        {
            Image image = sender as Image;
            BitmapImage bitmapImage = new BitmapImage(new Uri(AppConstants.SpellIconUrl() + champion.Value.spells[2].image.full));
            image.Source = bitmapImage;
        }

        private void ability3DescriptionTextBlock_Loaded(object sender, RoutedEventArgs e)
        {
            TextBlock textBlock = sender as TextBlock;
            textBlock.Text = champion.Value.spells[2].sanitizedDescription;
        }

        private void ability4TextBlock_Loaded(object sender, RoutedEventArgs e)
        {
            TextBlock textBlock = sender as TextBlock;
            textBlock.Text = champion.Value.spells[3].name;
        }

        private void ability4Image_Loaded(object sender, RoutedEventArgs e)
        {
            Image image = sender as Image;
            BitmapImage bitmapImage = new BitmapImage(new Uri(AppConstants.SpellIconUrl() + champion.Value.spells[3].image.full));
            image.Source = bitmapImage;
        }

        private void ability4DescriptionTextBlock_Loaded(object sender, RoutedEventArgs e)
        {
            TextBlock textBlock = sender as TextBlock;
            textBlock.Text = champion.Value.spells[3].sanitizedDescription;
        }

        private void championHub_SectionHeaderClick(object sender, HubSectionHeaderClickEventArgs e)
        {
            if (e.Section.Name == "abilitiesHubSection")
            {
                List<object> parameters = new List<object>();
                parameters.Add(champion);
                Frame.Navigate(typeof(ChampionAbilitiesDetailPage), parameters);
            }
        }
    }
}
