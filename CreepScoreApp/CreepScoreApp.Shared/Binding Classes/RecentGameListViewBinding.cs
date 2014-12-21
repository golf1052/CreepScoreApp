using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using CreepScoreAPI;
using CreepScoreAPI.Constants;
using CreepScoreApp.Constants;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace CreepScoreApp
{
    public class RecentGameListViewBinding
    {
        public Game game;
        public SolidColorBrush BorderColor { get; set; }
        public SolidColorBrush WinLossBar { get; set; }
        public ImageSource ChampionImage { get; set; }
        public SolidColorBrush TeamBar { get; set; }
        public int Kills { get; set; }
        public int Deaths { get; set; }
        public int Assists { get; set; }
        public ImageSource Item0Image { get; set; }
        public ImageSource Item1Image { get; set; }
        public ImageSource Item2Image { get; set; }
        public ImageSource Item3Image { get; set; }
        public ImageSource Item4Image { get; set; }
        public ImageSource Item5Image { get; set; }
        public ImageSource TrinketImage { get; set; }
        public ImageSource SummonerSpell1Image { get; set; }
        public ImageSource SummonerSpell2Image { get; set; }
        public string GameDateTime { get; set; }
        public string GameSubType { get; set; }
        public string GameModeType { get; set; }

        public RecentGameListViewBinding(Game game)
        {
            this.game = game;
            BorderColor = AppConstants.themeColor;
            SetWinLossBar();
            SetChampionImage();
            SetTeamBar();
            SetStats();
            SetItemImages();
            SetSummonerSpellImages();
            SetInfoText();
        }

        public void SetWinLossBar()
        {
            if ((bool)game.stats.win)
            {
                WinLossBar = AppConstants.metroGreen;
            }
            else
            {
                WinLossBar = AppConstants.metroRed;
            }
        }

        public void SetChampionImage()
        {
            string championName = AppConstants.championsData.keys[game.championId.ToString()];
            ChampionStatic champion = AppConstants.championsData.data[championName];
            ChampionImage = AppConstants.SetImageSource(new Uri(AppConstants.ChampionIconUrl() + champion.image.full));
        }

        public void SetTeamBar()
        {
            if (game.teamId == GameConstants.TeamID.Blue)
            {
                TeamBar = AppConstants.metroBlue2;
            }
            else if (game.teamId == GameConstants.TeamID.Purple)
            {
                TeamBar = AppConstants.metroPurple;
            }
        }

        public void SetStats()
        {
            Kills = SetNumber(game.stats.championsKilled);
            Deaths = SetNumber(game.stats.numDeaths);
            Assists = SetNumber(game.stats.assists);
        }

        public int SetNumber(int? number)
        {
            if (number == null)
            {
                return 0;
            }
            else
            {
                return (int)number;
            }
        }

        public void SetItemImages()
        {
            List<ItemStatic> items = new List<ItemStatic>();
            items = AddItem(items, game.stats.item0);
            items = AddItem(items, game.stats.item1);
            items = AddItem(items, game.stats.item2);
            items = AddItem(items, game.stats.item3);
            items = AddItem(items, game.stats.item4);
            items = AddItem(items, game.stats.item5);
            items = AddItem(items, game.stats.item6);

            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].tags.Contains("Trinket") || items[i].id == 2052)
                {
                    TrinketImage = AppConstants.SetImageSource(new Uri(AppConstants.ItemIconUrl() + items[i].image.full));
                    items.RemoveAt(i);
                    i--;
                    break;
                }
            }

            int itemsToAdd = 6 - items.Count;
            for (int i = 0; i < itemsToAdd; i++)
            {
                items.Add(null);
            }

            Item0Image = SetItemImageSource(items[0], Item0Image);
            Item1Image = SetItemImageSource(items[1], Item1Image);
            Item2Image = SetItemImageSource(items[2], Item2Image);
            Item3Image = SetItemImageSource(items[3], Item3Image);
            Item4Image = SetItemImageSource(items[4], Item4Image);
            Item5Image = SetItemImageSource(items[5], Item5Image);
        }

        public List<ItemStatic> AddItem(List<ItemStatic> items, int? itemId)
        {
            if (itemId != null)
            {
                items.Add(AppConstants.itemsData.data[itemId.ToString()]);
            }

            return items;
        }

        public ImageSource SetItemImageSource(ItemStatic item, ImageSource imageSource)
        {
            if (item != null)
            {
                return AppConstants.SetImageSource(new Uri(AppConstants.ItemIconUrl() + item.image.full));
            }
            else
            {
                return null;
            }
        }

        public void SetSummonerSpellImages()
        {
            SummonerSpellStatic spell1 = GetSpell(game.spell1Id);
            SummonerSpellStatic spell2 = GetSpell(game.spell2Id);
            SummonerSpell1Image = AppConstants.SetImageSource(new Uri(AppConstants.SpellIconUrl() + spell1.image.full));
            SummonerSpell2Image = AppConstants.SetImageSource(new Uri(AppConstants.SpellIconUrl() + spell2.image.full));
        }

        public SummonerSpellStatic GetSpell(int id)
        {
            foreach (KeyValuePair<string, SummonerSpellStatic> summonerSpell in AppConstants.summonerSpellsData.data)
            {
                if (summonerSpell.Value.id == id)
                {
                    return summonerSpell.Value;
                }
            }

            return null;
        }

        public void SetInfoText()
        {
            DateTime l = game.createDate.ToLocalTime();
            GameDateTime = l.ToString("m") + ", " + l.Year + " - " + l.ToString("t");
            GameSubType = ConvertSubType(game.subType);
            GameModeType = ConvertGameMode(game.gameMode) + " - " + ConvertGameType(game.gameType);
        }

        public string ConvertSubType(GameConstants.SubType subType)
        {
            if (subType == GameConstants.SubType.Normal)
            {
                return "Summoner's Rift";
            }
            else if (subType == GameConstants.SubType.Coop)
            {
                return "Coop v AI Summoner's Rift";
            }
            else if (subType == GameConstants.SubType.RankedSolo5v5)
            {
                return "Ranked Solo 5 v 5";
            }
            else if (subType == GameConstants.SubType.RankedPremade3v3)
            {
                return "Ranked Premade 3 v 3";
            }
            else if (subType == GameConstants.SubType.RankedPremade5v5)
            {
                return "Ranked Premade 5 v 5";
            }
            else if (subType == GameConstants.SubType.DominionUnranked)
            {
                return "The Crystal Scar";
            }
            else if (subType == GameConstants.SubType.RankedTeam3v3)
            {
                return "Ranked Team 3 v 3";
            }
            else if (subType == GameConstants.SubType.RankedTeam5v5)
            {
                return "Ranked Team 5 v 5";
            }
            else if (subType == GameConstants.SubType.Normal3v3)
            {
                return "Twisted Treeline";
            }
            else if (subType == GameConstants.SubType.Coop3v3)
            {
                return "Coop v AI Twisted Treeline";
            }
            else if (subType == GameConstants.SubType.Cap5v5)
            {
                return "Team Builder";
            }
            else if (subType == GameConstants.SubType.Aram)
            {
                return "Howling Abyss";
            }
            else if (subType == GameConstants.SubType.OneForAll)
            {
                return "One For All";
            }
            else if (subType == GameConstants.SubType.Showdown1v1)
            {
                return "Showdown 1 v 1";
            }
            else if (subType == GameConstants.SubType.Showdown2v2)
            {
                return "Showdown 2 v 2";
            }
            else if (subType == GameConstants.SubType.Hexakill)
            {
                return "Summoners Rift Hexakill";
            }
            else if (subType == GameConstants.SubType.Urf)
            {
                return "Ultra Rapid Fire";
            }
            else if (subType == GameConstants.SubType.UrfCoop)
            {
                return "Ultra Rapid Fire Coop";
            }
            else if (subType == GameConstants.SubType.DoomBots)
            {
                return "Doom Bots of Doom";
            }
            else if (subType == GameConstants.SubType.Ascension)
            {
                return "Ascension";
            }
            else if (subType == GameConstants.SubType.TwistedTreelineHexakill)
            {
                return "Twisted Treeline Hexakill";
            }
            else if (subType == GameConstants.SubType.KingPoro)
            {
                return "King Poro";
            }
            else
            {
                return "None";
            }
        }

        public string ConvertGameMode(GameConstants.GameMode gameMode)
        {
            if (gameMode == GameConstants.GameMode.Classic)
            {
                return "Classic";
            }
            else if (gameMode == GameConstants.GameMode.Dominion)
            {
                return "Dominion";
            }
            else if (gameMode == GameConstants.GameMode.Aram)
            {
                return "ARAM";
            }
            else if (gameMode == GameConstants.GameMode.Tutorial)
            {
                return "Tutorial";
            }
            else if (gameMode == GameConstants.GameMode.OneForAll)
            {
                return "One For All";
            }
            else if (gameMode == GameConstants.GameMode.FirstBlood)
            {
                return "Showdown";
            }
            else if (gameMode == GameConstants.GameMode.Ascension)
            {
                return "Ascension";
            }
            else if (gameMode == GameConstants.GameMode.KingPoro)
            {
                return "King Poro";
            }
            else
            {
                return "None";
            }
        }

        public string ConvertGameType(GameConstants.GameType gameType)
        {
            if (gameType == GameConstants.GameType.Custom)
            {
                return "Custom Game";
            }
            else if (gameType == GameConstants.GameType.Matched)
            {
                return "Matched Game";
            }
            else if (gameType == GameConstants.GameType.CoOp)
            {
                return "Coop v AI Game";
            }
            else if (gameType == GameConstants.GameType.Tutorial)
            {
                return "Tutorial Game";
            }
            else
            {
                return "None";
            }
        }
    }
}
