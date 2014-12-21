using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CreepScoreAPI;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace CreepScoreApp.Constants
{
    public static class AppConstants
    {
        public static CreepScore creepScore;
        public static RealmStatic realmData;
        public static ChampionListStatic championsData;
        public static ItemListStatic itemsData;
        public static MasteryListStatic masteriesData;
        public static RuneListStatic runesData;
        public static SummonerSpellListStatic summonerSpellsData;
        public static List<string> versions;

        public static string championSplashUrl = "http://ddragon.leagueoflegends.com/cdn/img/champion/splash/";
        public static string championLoadingUrl = "http://ddragon.leagueoflegends.com/cdn/img/champion/loading/";

        public static SolidColorBrush themeColor = creepScoreYellow;
        // #6495ED
        public static SolidColorBrush creepScoreBlue = new SolidColorBrush(Colors.CornflowerBlue);
        // #F9C518
        public static SolidColorBrush creepScoreYellow = new SolidColorBrush(Color.FromArgb(255, 249, 197, 24));
        // #1D1D1D
        public static SolidColorBrush metroBlack = new SolidColorBrush(Color.FromArgb(255, 29, 29, 29));
        // #FBFBFB
        public static SolidColorBrush metroWhite = new SolidColorBrush(Color.FromArgb(255, 251, 251, 251));
        // #DF2020
        public static SolidColorBrush metroRed = new SolidColorBrush(Color.FromArgb(255, 223, 32, 32));
        // #DF8020
        public static SolidColorBrush metroOrange = new SolidColorBrush(Color.FromArgb(255, 223, 128, 32));
        // #DFDF20
        public static SolidColorBrush metroYellow = new SolidColorBrush(Color.FromArgb(255, 223, 223, 32));
        // #80DF20
        public static SolidColorBrush metroGreen = new SolidColorBrush(Color.FromArgb(255, 128, 223, 32));
        // #2020DF
        public static SolidColorBrush metroBlue = new SolidColorBrush(Color.FromArgb(255, 32, 32, 223));
        // #8020DF
        public static SolidColorBrush metroPurple = new SolidColorBrush(Color.FromArgb(255, 128, 32, 223));
        // #2080DF
        public static SolidColorBrush metroBlue2 = new SolidColorBrush(Color.FromArgb(255,32, 128, 223));

        public static CreepScore.Region preferredRegion = CreepScore.Region.NA;

        public static string ChampionIconUrl()
        {
            string url = realmData.cdn + "/" + realmData.v + "/img/champion/";
            return url;
        }

        public static string ChampionPassiveIconUrl()
        {
            string url = realmData.cdn + "/" + realmData.v + "/img/passive/";
            return url;
        }

        /// <summary>
        /// Can be used to retrieve champion abilities and summoner spell abilities
        /// </summary>
        /// <returns>The url base as a string</returns>
        public static string SpellIconUrl()
        {
            string url = realmData.cdn + "/" + realmData.v + "/img/spell/";
            return url;
        }

        public static string SummonerProfileIconUrl()
        {
            string url = realmData.cdn + "/" + realmData.v + "/img/profileicon/";
            return url;
        }

        public static string ItemIconUrl()
        {
            string url = realmData.cdn + "/" + realmData.v + "/img/item/";
            return url;
        }

        public static string RuneIconUrl()
        {
            string url = realmData.cdn + "/" + realmData.v + "/img/rune/";
            return url;
        }

        public static string PopulateBracketData(ChampionSpellStatic spell, string toPopulate)
        {
            string str = toPopulate;
            int currentStartLocation = str.IndexOf("{{ ");
            int currentEndLocation = str.IndexOf(" }}");

            while (currentStartLocation != -1)
            {
                bool replaced = false;
                string key = str.Substring(currentStartLocation + 3, currentEndLocation - (currentStartLocation + 3));
                string fullKey = str.Substring(currentStartLocation, (currentEndLocation + 3) - currentStartLocation);
                if (key[0] == 'e')
                {
                    int effectIndex = int.Parse(key[1].ToString()) - 1;
                    try
                    {
                        str = str.Replace(fullKey, spell.effectBurn[effectIndex]);
                        replaced = true;
                    }
                    catch (Exception ex)
                    {
                        str = str.Replace(fullKey, "EFFECTISFUCKINGMISSING");
                        replaced = true;
                    }
                }
                else
                {
                    for (int i = 0; i < spell.vars.Count; i++)
                    {
                        if (spell.vars[i].key == key)
                        {
                            if (spell.vars[i].coeff.Count == 1)
                            {
                                double coeffPercent = spell.vars[i].coeff[0] * 100.0;
                                str = str.Replace(fullKey, coeffPercent + "% * " + LinkToDamageType(spell.vars[i].link));
                                replaced = true;
                            }
                            else
                            {
                                string coeffStr = "";
                                for (int j = 0; j < spell.vars[i].coeff.Count; j++)
                                {
                                    if (j != spell.vars[i].coeff.Count - 1)
                                    {
                                        coeffStr += spell.vars[i].coeff[j] + "/";
                                    }
                                    else
                                    {
                                        coeffStr += spell.vars[i].coeff[j].ToString();
                                    }
                                }

                                str = str.Replace(fullKey, coeffStr + " " + LinkToDamageType(spell.vars[i].link));
                                replaced = true;
                            }
                            break;
                        }
                    }
                }

                if (!replaced)
                {
                    //str = str.Replace(fullKey, "---");
                    str = str.Replace(fullKey, "AVARISFUCKINGMISSING");
                }
                currentStartLocation = str.IndexOf("{{ ");
                currentEndLocation = str.IndexOf(" }}");
            }

            return str;
        }

        static string LinkToDamageType(string link)
        {
            if (link != null)
            {
                if (link == "spelldamage")
                {
                    return "AP";
                }
                else if (link == "attackdamage")
                {
                    return "AD";
                }
                else if (link == "bonusattackdamage")
                {
                    return "bonus AD";
                }
                else if (link == "bonusspellblock")
                {
                    return "bonus MR";
                }
                else if (link == "bonusarmor")
                {
                    return "bonus AR";
                }
                else if (link == "@text")
                {
                    return "SOMETHINGTEXT";
                }
                else
                {
                    return link;
                }
            }
            else
            {
                return "LINKWASNULLGG";
            }
        }

        /// <summary>
        /// Tries to create a file. If the file is there it just returns it.
        /// If it is not there it creates the file and returns it.
        /// </summary>
        /// <param name="fileName">Desired file name</param>
        /// <param name="folder">StorageFolder the file goes in</param>
        /// <returns>A file</returns>
        public static async Task<StorageFile> TryCreateFile(string fileName, StorageFolder folder)
        {
            StorageFile file;
#if WINDOWS_APP
            file = await folder.TryGetItemAsync(fileName) as StorageFile;
#endif
#if WINDOWS_PHONE_APP
            file = await AppConstants.TryGetItemAsync(folder, fileName);
#endif

            if (file != null)
            {
                return file;
            }
            else
            {
                file = await folder.CreateFileAsync(fileName);
                return file;
            }
        }

        public static async Task<StorageFile> TryGetFile(string fileName, StorageFolder folder)
        {
            StorageFile file;
#if WINDOWS_APP
            file = await folder.TryGetItemAsync(fileName) as StorageFile;
#endif
#if WINDOWS_PHONE_APP
            file = await AppConstants.TryGetItemAsync(folder, fileName);
#endif

            if (file != null)
            {
                return file;
            }
            else
            {
                return null;
            }
        }

        public static async Task OverwriteFile(StorageFile file, string data)
        {
            await FileIO.WriteTextAsync(file, data);
        }

        public static async Task<string> ReadFile(StorageFile file)
        {
            string data = await FileIO.ReadTextAsync(file);
            return data;
        }

        public static CreepScore.Region SetRegion(string region)
        {
            if (region == "na")
            {
                return CreepScore.Region.NA;
            }
            else if (region == "euw")
            {
                return CreepScore.Region.EUW;
            }
            else if (region == "eune")
            {
                return CreepScore.Region.EUNE;
            }
            else if (region == "oce")
            {
                return CreepScore.Region.OCE;
            }
            else if (region == "kr")
            {
                return CreepScore.Region.KR;
            }
            else if (region == "br")
            {
                return CreepScore.Region.BR;
            }
            else if (region == "lan")
            {
                return CreepScore.Region.LAN;
            }
            else if (region == "las")
            {
                return CreepScore.Region.LAS;
            }
            else if (region == "ru")
            {
                return CreepScore.Region.RU;
            }
            else if (region == "tr")
            {
                return CreepScore.Region.TR;
            }
            else
            {
                return CreepScore.Region.None;
            }
        }

        public static BitmapImage SetImageSource(Uri imageUrl)
        {
            BitmapImage image = new BitmapImage(imageUrl);
            return image;
        }

        public static string ToRegularCase(string str)
        {
            string strModified = str.ToLower();
            string[] brokenUp = strModified.Split(' ');
            string returnString = "";
            for (int i = 0; i < brokenUp.Length; i++)
            {
                if (brokenUp[i] != "")
                {
                    if (brokenUp[i].Length > 1)
                    {
                        brokenUp[i] = brokenUp[i][0].ToString().ToUpper() + brokenUp[i].Substring(1);
                    }
                    else
                    {
                        brokenUp[i] = brokenUp[i][0].ToString().ToUpper();
                    }

                    if (i != brokenUp.Length - 1)
                    {
                        returnString += brokenUp[i] + " ";
                    }
                    else
                    {
                        returnString += brokenUp[i];
                    }
                }
            }

            return returnString;
        }

        public static async Task<StorageFile> TryGetItemAsync(StorageFolder folder, string fileName)
        {
            StorageFile file;

            try
            {
                file = await folder.GetItemAsync(fileName) as StorageFile;
                return file;
            }
            catch
            {
                return null;
            }
        }

        public static void ClearThemeSetting()
        {
            if (ApplicationData.Current.RoamingSettings.Values.ContainsKey("IsLightTheme"))
            {
                ApplicationData.Current.RoamingSettings.Values.Remove("IsLightTheme");
            }
        }

        public static void ClearLaunchedSetting()
        {
            if (ApplicationData.Current.RoamingSettings.Values.ContainsKey("LaunchedApp"))
            {
                ApplicationData.Current.RoamingSettings.Values.Remove("LaunchedApp");
            }
        }

        public static int CreepScoreRegion(int comboBoxNumber)
        {
            if (comboBoxNumber == 0)
            {
                // NA
                return 1;
            }
            else if (comboBoxNumber == 1)
            {
                // EUW
                return 2;
            }
            else if (comboBoxNumber == 2)
            {
                // EUNE
                return 3;
            }
            else if (comboBoxNumber == 3)
            {
                // OCE
                return 7;
            }
            else if (comboBoxNumber == 4)
            {
                // KR
                return 8;
            }
            else if (comboBoxNumber == 5)
            {
                // BR
                return 4;
            }
            else if (comboBoxNumber == 6)
            {
                // LAN
                return 5;
            }
            else if (comboBoxNumber == 7)
            {
                // LAS
                return 6;
            }
            else if (comboBoxNumber == 8)
            {
                // RU
                return 10;
            }
            else if (comboBoxNumber == 9)
            {
                // TR
                return 9;
            }
            else
            {
                return 0;
            }
        }
    }
}
