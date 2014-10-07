using System;
using System.Collections.Generic;
using System.Text;
using CreepScoreApp.Constants;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace CreepScoreApp
{
    public class LeagueListViewBinding
    {
        // Icons
        // Veteran = Clock
        // Hotstreak = Go
        // Newbie = Favorite
        // Inactive = Important
        // Series Game Win = Accept
        // Series Game Loss = Cancel
        // Series Game Not Played = Remove

        public int Rank { get; set; }
        public string SummonerName { get; set; }
        public SolidColorBrush VeteranIconColor { get; set; }
        bool isVeteran;
        public SolidColorBrush HotStreakIconColor { get; set; }
        bool onHotStreak;
        public SolidColorBrush NewbieIconColor { get; set; }
        bool isNewbie;
        public SolidColorBrush InactiveIconColor { get; set; }
        bool isInactive;
        public int Wins { get; set; }
        public int LadderPoints { get; set; }
        public Visibility LadderPointsVisibility { get; set; }
        public Visibility SeriesVisibility { get; set; }
        public Symbol Game1Icon { get; set; }
        public Symbol Game2Icon { get; set; }
        public Symbol Game3Icon { get; set; }
        public Symbol Game4Icon { get; set; }
        public Symbol Game5Icon { get; set; }
        public SolidColorBrush Game1IconColor { get; set; }
        public SolidColorBrush Game2IconColor { get; set; }
        public SolidColorBrush Game3IconColor { get; set; }
        public SolidColorBrush Game4IconColor { get; set; }
        public SolidColorBrush Game5IconColor { get; set; }
        public Visibility Bo5 { get; set; }
        public SolidColorBrush BackgroundColor { get; set; }
        public Visibility BackgroundVisibility { get; set; }

        public LeagueListViewBinding(int rank,
            string name,
            bool isVeteran,
            bool onHotStreak,
            bool isNewbie,
            bool isInactive,
            int wins,
            int ladderPoints,
            bool background)
        {
            BackgroundColor = AppConstants.themeColor;
            if (background)
            {
                BackgroundVisibility = Visibility.Visible;
            }
            else
            {
                BackgroundVisibility = Visibility.Collapsed;
            }
            this.Rank = rank;
            this.SummonerName = name;
            this.isVeteran = isVeteran;
            if (isVeteran)
            {
                VeteranIconColor = AppConstants.metroYellow;
            }
            else
            {
                VeteranIconColor = DetermineEmptyColor();
            }
            this.onHotStreak = onHotStreak;
            if (onHotStreak)
            {
                HotStreakIconColor = AppConstants.metroOrange;
            }
            else
            {
                HotStreakIconColor = DetermineEmptyColor();
            }
            this.isNewbie = isNewbie;
            if (isNewbie)
            {
                NewbieIconColor = AppConstants.metroGreen;
            }
            else
            {
                NewbieIconColor = DetermineEmptyColor();
            }
            this.isInactive = isInactive;
            if (isInactive)
            {
                InactiveIconColor = AppConstants.metroRed;
            }
            else
            {
                InactiveIconColor = DetermineEmptyColor();
            }
            this.Wins = wins;
            this.LadderPoints = ladderPoints;
            LadderPointsVisibility = Visibility.Visible;
            SeriesVisibility = Visibility.Collapsed;

            Game1Icon = Symbol.Remove;
            Game2Icon = Symbol.Remove;
            Game3Icon = Symbol.Remove;
            Game4Icon = Symbol.Remove;
            Game5Icon = Symbol.Remove;
        }

        public LeagueListViewBinding(int rank,
            string name,
            bool isVeteran,
            bool onHotStreak,
            bool isNewbie,
            bool isInactive,
            int wins,
            int seriesType,
            string seriesProgress,
            bool background)
        {
            BackgroundColor = AppConstants.themeColor;
            if (background)
            {
                BackgroundVisibility = Visibility.Visible;
            }
            else
            {
                BackgroundVisibility = Visibility.Collapsed;
            }
            this.Rank = rank;
            this.SummonerName = name;
            this.isVeteran = isVeteran;
            if (isVeteran)
            {
                VeteranIconColor = AppConstants.metroYellow;
            }
            else
            {
                VeteranIconColor = DetermineEmptyColor();
            }
            this.onHotStreak = onHotStreak;
            if (onHotStreak)
            {
                HotStreakIconColor = AppConstants.metroOrange;
            }
            else
            {
                HotStreakIconColor = DetermineEmptyColor();
            }
            this.isNewbie = isNewbie;
            if (isNewbie)
            {
                NewbieIconColor = AppConstants.metroGreen;
            }
            else
            {
                NewbieIconColor = DetermineEmptyColor();
            }
            this.isInactive = isInactive;
            if (isInactive)
            {
                InactiveIconColor = AppConstants.metroRed;
            }
            else
            {
                InactiveIconColor = DetermineEmptyColor();
            }
            this.Wins = wins;
            LadderPointsVisibility = Visibility.Collapsed;
            SeriesVisibility = Visibility.Visible;
            Game1Icon = Symbol.Remove;
            Game2Icon = Symbol.Remove;
            Game3Icon = Symbol.Remove;
            Game4Icon = Symbol.Remove;
            Game5Icon = Symbol.Remove;
            if (seriesType == 2)
            {
                Bo5 = Visibility.Collapsed;
                Game1Icon = DetermineGameIcon(seriesProgress[0]);
                Game1IconColor = DetermineGameIconColor(seriesProgress[0]);
                Game2Icon = DetermineGameIcon(seriesProgress[1]);
                Game2IconColor = DetermineGameIconColor(seriesProgress[1]);
                Game3Icon = DetermineGameIcon(seriesProgress[2]);
                Game3IconColor = DetermineGameIconColor(seriesProgress[2]);
            }
            else
            {
                Bo5 = Visibility.Visible;
                Game1Icon = DetermineGameIcon(seriesProgress[0]);
                Game1IconColor = DetermineGameIconColor(seriesProgress[0]);
                Game2Icon = DetermineGameIcon(seriesProgress[1]);
                Game2IconColor = DetermineGameIconColor(seriesProgress[1]);
                Game3Icon = DetermineGameIcon(seriesProgress[2]);
                Game3IconColor = DetermineGameIconColor(seriesProgress[2]);
                Game4Icon = DetermineGameIcon(seriesProgress[3]);
                Game4IconColor = DetermineGameIconColor(seriesProgress[3]);
                Game5Icon = DetermineGameIcon(seriesProgress[4]);
                Game5IconColor = DetermineGameIconColor(seriesProgress[4]);
            }
        }

        SolidColorBrush DetermineEmptyColor()
        {
            if (AppConstants.themeColor == AppConstants.creepScoreYellow)
            {
                return AppConstants.metroWhite;
            }
            else
            {
                return AppConstants.metroBlack;
            }
        }

        Symbol DetermineGameIcon(char game)
        {
            if (game == 'W')
            {
                return Symbol.Accept;
            }
            else if (game == 'L')
            {
                return Symbol.Cancel;
            }
            else
            {
                return Symbol.Remove;
            }
        }

        SolidColorBrush DetermineGameIconColor(char game)
        {
            if (game == 'W')
            {
                return AppConstants.metroGreen;
            }
            else if (game == 'L')
            {
                return AppConstants.metroRed;
            }
            else
            {
                return DetermineEmptyColor();
            }
        }
    }
}
