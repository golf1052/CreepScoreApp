using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CreepScoreAPI;
using CreepScoreAPI.Constants;
using CreepScoreApp.Constants;
using Newtonsoft.Json.Linq;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace CreepScoreApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LoadingPage : Page
    {
        public LoadingPage()
        {
            this.InitializeComponent();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            // Set loading ring color
            loadingRing.Foreground = AppConstants.themeColor;

            // get the latest realm data
            loadingText.Text = "fetching current League of Legends patch version";
            Task task1 = Task.Run(async () => { AppConstants.realmData = await AppConstants.creepScore.RetrieveRealmData(CreepScore.Region.NA); });
            task1.Wait();

            //List<string> versions = new List<string>();
            //Task task2 = Task.Run(async () => { versions = await AppConstants.creepScore.RetrieveVersions(CreepScore.Region.NA); });
            //task2.Wait();
            //AppConstants.realmData.v = versions[0];

            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            StorageFile realmFile;

            // check to see if we already have a realm data file
#if WINDOWS_APP
            realmFile = await localFolder.TryGetItemAsync("RealmData.json") as StorageFile;
#endif
#if WINDOWS_PHONE_APP
            realmFile = await AppConstants.TryGetItemAsync(localFolder, "RealmData.json");
#endif
            if (realmFile != null)
            {
                // if so read it and compare the versions
                string realmString = await FileIO.ReadTextAsync(realmFile);
                loadingText.Text = "comparing saved patch version to current patch version";
                if (realmString == AppConstants.realmData.v)
                {
                    loadingText.Text = "versions match. loading saved patch data";

                    loadingText.Text = "loading saved champion data";
                    AppConstants.championsData = await ReadAndLoadData<ChampionListStatic, StaticDataConstants.ChampData>(CreepScore.Region.NA, AppConstants.championsData, StaticDataConstants.ChampData.All, localFolder, "ChampionsData.json", AppConstants.creepScore.LoadChampionListStatic, AppConstants.creepScore.RetrieveChampionsData);

                    loadingText.Text = "loading saved item data";
                    AppConstants.itemsData = await ReadAndLoadData<ItemListStatic, StaticDataConstants.ItemListData>(CreepScore.Region.NA, AppConstants.itemsData, StaticDataConstants.ItemListData.All, localFolder, "ItemsData.json", HelperMethods.LoadItemsStatic, AppConstants.creepScore.RetrieveItemsData);

                    loadingText.Text = "loading saved mastery data";
                    AppConstants.masteriesData = await ReadAndLoadData<MasteryListStatic, StaticDataConstants.MasteryListData>(CreepScore.Region.NA, AppConstants.masteriesData, StaticDataConstants.MasteryListData.All, localFolder, "MasteriesData.json", HelperMethods.LoadMasteryListStatic, AppConstants.creepScore.RetrieveMasteriesData);

                    loadingText.Text = "loading saved rune data";
                    AppConstants.runesData = await ReadAndLoadData<RuneListStatic, StaticDataConstants.RuneListData>(CreepScore.Region.NA, AppConstants.runesData, StaticDataConstants.RuneListData.All, localFolder, "RunesData.json", HelperMethods.LoadRuneListStatic, AppConstants.creepScore.RetrieveRunesData);

                    loadingText.Text = "loading saved summoner spells data";
                    AppConstants.summonerSpellsData = await ReadAndLoadData<SummonerSpellListStatic, StaticDataConstants.SpellData>(CreepScore.Region.NA, AppConstants.summonerSpellsData, StaticDataConstants.SpellData.All, localFolder, "SummonerSpellsData.json", HelperMethods.LoadSummonerSpellListStatic, AppConstants.creepScore.RetrieveSummonerSpellsData);

                    loadingText.Text = "fetching version data";
                    LoadVersions(CreepScore.Region.NA);
                    LaunchIntoApplication();
                }
                else
                {
                    // if the versions do not match write the current version to the file then write the new champions data
                    loadingText.Text = "versions do not match. fetching current patch data";
                    await FileIO.WriteTextAsync(realmFile, AppConstants.realmData.v);

                    loadingText.Text = "fetching current champion data";
                    AppConstants.championsData = await ReadAndOverwriteData<ChampionListStatic, StaticDataConstants.ChampData>(CreepScore.Region.NA, AppConstants.championsData, StaticDataConstants.ChampData.All, localFolder, "ChampionsData.json", AppConstants.creepScore.RetrieveChampionsData);

                    loadingText.Text = "fetching current item data";
                    AppConstants.itemsData = await ReadAndOverwriteData<ItemListStatic, StaticDataConstants.ItemListData>(CreepScore.Region.NA, AppConstants.itemsData, StaticDataConstants.ItemListData.All, localFolder, "ItemsData.json", AppConstants.creepScore.RetrieveItemsData);

                    loadingText.Text = "fetching current mastery data";
                    AppConstants.masteriesData = await ReadAndOverwriteData<MasteryListStatic, StaticDataConstants.MasteryListData>(CreepScore.Region.NA, AppConstants.masteriesData, StaticDataConstants.MasteryListData.All, localFolder, "MasteriesData.json", AppConstants.creepScore.RetrieveMasteriesData);

                    loadingText.Text = "fetching current rune data";
                    AppConstants.runesData = await ReadAndOverwriteData<RuneListStatic, StaticDataConstants.RuneListData>(CreepScore.Region.NA, AppConstants.runesData, StaticDataConstants.RuneListData.All, localFolder, "RunesData.json", AppConstants.creepScore.RetrieveRunesData);

                    loadingText.Text = "fetching current summoner spell data";
                    AppConstants.summonerSpellsData = await ReadAndOverwriteData<SummonerSpellListStatic, StaticDataConstants.SpellData>(CreepScore.Region.NA, AppConstants.summonerSpellsData, StaticDataConstants.SpellData.All, localFolder, "SummonerSpellsData.json", AppConstants.creepScore.RetrieveSummonerSpellsData);

                    loadingText.Text = "fetching version data";
                    LoadVersions(CreepScore.Region.NA);
                    LaunchIntoApplication();
                }
            }
            else
            {
                // we dont have a realm file so create it and write the current version to it and load the current champ data
                loadingText.Text = "saved patch version file not found. checking for champion data file";
                realmFile = await localFolder.CreateFileAsync("RealmData.json");
                await FileIO.WriteTextAsync(realmFile, AppConstants.realmData.v);

                loadingText.Text = "fetching current champion data";
                AppConstants.championsData = await ReadAndOverwriteData<ChampionListStatic, StaticDataConstants.ChampData>(CreepScore.Region.NA, AppConstants.championsData, StaticDataConstants.ChampData.All, localFolder, "ChampionsData.json", AppConstants.creepScore.RetrieveChampionsData);

                loadingText.Text = "fetching current item data";
                AppConstants.itemsData = await ReadAndOverwriteData<ItemListStatic, StaticDataConstants.ItemListData>(CreepScore.Region.NA, AppConstants.itemsData, StaticDataConstants.ItemListData.All, localFolder, "ItemsData.json", AppConstants.creepScore.RetrieveItemsData);

                loadingText.Text = "fetching current mastery data";
                AppConstants.masteriesData = await ReadAndOverwriteData<MasteryListStatic, StaticDataConstants.MasteryListData>(CreepScore.Region.NA, AppConstants.masteriesData, StaticDataConstants.MasteryListData.All, localFolder, "MasteriesData.json", AppConstants.creepScore.RetrieveMasteriesData);

                loadingText.Text = "fetching current rune data";
                AppConstants.runesData = await ReadAndOverwriteData<RuneListStatic, StaticDataConstants.RuneListData>(CreepScore.Region.NA, AppConstants.runesData, StaticDataConstants.RuneListData.All, localFolder, "RunesData.json", AppConstants.creepScore.RetrieveRunesData);

                loadingText.Text = "fetching current summoner spell data";
                AppConstants.summonerSpellsData = await ReadAndOverwriteData<SummonerSpellListStatic, StaticDataConstants.SpellData>(CreepScore.Region.NA, AppConstants.summonerSpellsData, StaticDataConstants.SpellData.All, localFolder, "SummonerSpellsData.json", AppConstants.creepScore.RetrieveSummonerSpellsData);

                loadingText.Text = "fetching version data";
                LoadVersions(CreepScore.Region.NA);
                LaunchIntoApplication();
            }
        }

        /// <summary>
        /// Reads data from a file and returns it, contains data by ID field
        /// </summary>
        /// <typeparam name="T1">The object type to return</typeparam>
        /// <typeparam name="T2">The data section enum</typeparam>
        /// <param name="region">The region to load information from</param>
        /// <param name="data">The object we should store the loaded data in</param>
        /// <param name="staticDataType">The data section we should load</param>
        /// <param name="localFolder">The folder the file resides in</param>
        /// <param name="fileName">The filename</param>
        /// <param name="loadFunction">The function that will load the data</param>
        /// <param name="retrieveFunction">The function that will retrieve the data</param>
        /// <param name="locale">The locale we should use</param>
        /// <param name="version">The patch number we should load data from</param>
        /// <param name="dataById">If we want to load data using IDs</param>
        /// <returns>Returns the object (T1) with loaded data</returns>
        async Task<T1> ReadAndLoadData<T1, T2>(CreepScore.Region region,
            T1 data,
            T2 staticDataType,
            StorageFolder localFolder,
            string fileName,
            Func<JObject, T1> loadFunction,
            Func<CreepScore.Region, T2, string, string, bool, Task<T1>> retrieveFunction, string locale = "", string version = "", bool dataById = false)
        {
            StorageFile file;
#if WINDOWS_APP
            file = await localFolder.TryGetItemAsync(fileName) as StorageFile;
#endif
#if WINDOWS_PHONE_APP
            file = await AppConstants.TryGetItemAsync(localFolder, fileName);
#endif

            // look for the specified file
            if (file != null)
            {
                // if the file is there read it and load the data from it
                string fileString = await FileIO.ReadTextAsync(file);
                JObject fileObject = JObject.Parse(fileString);
                data = loadFunction(fileObject);
                return data;
            }
            else
            {
                data = await CreateData<T1, T2>(region, data, staticDataType, localFolder, file, fileName, retrieveFunction);
                return data;
            }
        }

        /// <summary>
        /// Reads data from a file and returns it. If the file doesn't exist it creates the file and retrieves the data. Doesn't contain data by ID field
        /// </summary>
        /// <typeparam name="T1">The object type to return</typeparam>
        /// <typeparam name="T2">The data section enum</typeparam>
        /// <param name="region">The region to load information from</param>
        /// <param name="data">The object we should store the loaded data in</param>
        /// <param name="staticDataType">The data section we should load</param>
        /// <param name="localFolder">The folder the file resides in</param>
        /// <param name="fileName">The filename</param>
        /// <param name="loadFunction"></param>
        /// <param name="retrieveFunction"></param>
        /// <param name="locale">The locale we should use</param>
        /// <param name="version">The patch number we should load data from</param>
        /// <returns>Returns the object (T1) with loaded data</returns>
        async Task<T1> ReadAndLoadData<T1, T2>(CreepScore.Region region,
            T1 data,
            T2 staticDataType,
            StorageFolder localFolder,
            string fileName,
            Func<JObject, T1> loadFunction,
            Func<CreepScore.Region, T2, string, string, Task<T1>> retrieveFunction, string locale = "", string version = "")
        {
            StorageFile file;
#if WINDOWS_APP
            file = await localFolder.TryGetItemAsync(fileName) as StorageFile;
#endif
#if WINDOWS_PHONE_APP
            file = await AppConstants.TryGetItemAsync(localFolder, fileName);
#endif

            // look for the specified file
            if (file != null)
            {
                // if the file is there read it and load the data from it
                string fileString = await FileIO.ReadTextAsync(file);
                JObject fileObject = JObject.Parse(fileString);
                data = loadFunction(fileObject);
                return data;
            }
            else
            {
                data = await CreateData<T1, T2>(region, data, staticDataType, localFolder, file, fileName, retrieveFunction);
                return data;
            }
        }

        /// <summary>
        /// Overwrites a file with newly retrieved data. Contains data by ID field.
        /// </summary>
        /// <typeparam name="T1">The object type to return</typeparam>
        /// <typeparam name="T2">The data section enum</typeparam>
        /// <param name="region">The region to load information from</param>
        /// <param name="data">The object we should store the loaded data in</param>
        /// <param name="staticDataType">The data section we should load</param>
        /// <param name="localFolder">The folder the file resides in</param>
        /// <param name="fileName">The filename</param>
        /// <param name="loadFunction">The function that will load the data</param>
        /// <param name="retrieveFunction">The function that will retrieve the data</param>
        /// <param name="locale">The locale we should use</param>
        /// <param name="version">The patch number we should load data from</param>
        /// <param name="dataById">If we want to load data using IDs</param>
        /// <returns>Returns the object (T1) with loaded data</returns>
        async Task<T1> ReadAndOverwriteData<T1, T2>(CreepScore.Region region,
            T1 data,
            T2 staticDataType,
            StorageFolder localFolder,
            string fileName,
            Func<CreepScore.Region, T2, string, string, bool, Task<T1>> retrieveFunction, string locale = "", string version = "", bool dataById = false)
        {
            StorageFile file;
#if WINDOWS_APP
            file = await localFolder.TryGetItemAsync(fileName) as StorageFile;
#endif
#if WINDOWS_PHONE_APP
            file = await AppConstants.TryGetItemAsync(localFolder, fileName);
#endif

            // look for the specified file
            if (file != null)
            {
                data = await OverwriteData<T1, T2>(region, data, staticDataType, localFolder, file, fileName, retrieveFunction);
                return data;
            }
            else
            {
                data = await CreateData<T1, T2>(region, data, staticDataType, localFolder, file, fileName, retrieveFunction);
                return data;
            }
        }

        /// <summary>
        /// Overwrites a file with newly retrieved data. Doesn't contain data by ID field
        /// </summary>
        /// <typeparam name="T1">The object type to return</typeparam>
        /// <typeparam name="T2">The data section enum</typeparam>
        /// <param name="region">The region to load information from</param>
        /// <param name="data">The object we should store the loaded data in</param>
        /// <param name="staticDataType">The data section we should load</param>
        /// <param name="localFolder">The folder the file resides in</param>
        /// <param name="fileName">The filename</param>
        /// <param name="loadFunction"></param>
        /// <param name="retrieveFunction"></param>
        /// <param name="locale">The locale we should use</param>
        /// <param name="version">The patch number we should load data from</param>
        /// <returns>Returns the object (T1) with loaded data</returns>
        async Task<T1> ReadAndOverwriteData<T1, T2>(CreepScore.Region region,
            T1 data,
            T2 staticDataType,
            StorageFolder localFolder,
            string fileName,
            Func<CreepScore.Region, T2, string, string, Task<T1>> retrieveFunction, string locale = "", string version = "")
        {
            StorageFile file;
#if WINDOWS_APP
            file = await localFolder.TryGetItemAsync(fileName) as StorageFile;
#endif
#if WINDOWS_PHONE_APP
            file = await AppConstants.TryGetItemAsync(localFolder, fileName);
#endif

            // look for the specified file
            if (file != null)
            {
                data = await OverwriteData<T1, T2>(region, data, staticDataType, localFolder, file, fileName, retrieveFunction);
                return data;
            }
            else
            {
                data = await CreateData<T1, T2>(region, data, staticDataType, localFolder, file, fileName, retrieveFunction);
                return data;
            }
        }

        async Task<T1> CreateData<T1, T2>(CreepScore.Region region,
            T1 data,
            T2 staticDataType,
            StorageFolder localFolder,
            StorageFile file,
            string fileName,
            Func<CreepScore.Region, T2, string, string, bool, Task<T1>> retrieveFunction, string locale = "", string version = "", bool dataById = false)
        {
            // create file
            file = await CreateFile(fileName, localFolder, file);
            data = await OverwriteData<T1, T2>(region, data, staticDataType, localFolder, file, fileName, retrieveFunction);
            return data;
        }

        async Task<T1> CreateData<T1, T2>(CreepScore.Region region,
            T1 data,
            T2 staticDataType,
            StorageFolder localFolder,
            StorageFile file,
            string fileName,
            Func<CreepScore.Region, T2, string, string, Task<T1>> retrieveFunction, string locale = "", string version = "")
        {
            // create file
            file = await CreateFile(fileName, localFolder, file);
            data = await OverwriteData<T1, T2>(region, data, staticDataType, localFolder, file, fileName, retrieveFunction);
            return data;
        }

        /// <summary>
        /// Creates a file
        /// </summary>
        /// <param name="fileName">The name of the file</param>
        /// <param name="localFolder">The storage folder</param>
        /// <param name="file">The file</param>
        /// <returns>A new file</returns>
        async Task<StorageFile> CreateFile(string fileName, StorageFolder localFolder, StorageFile file)
        {
            file = await localFolder.CreateFileAsync(fileName);
            return file;
        }

        // Same as create file 
        async Task<T1> OverwriteData<T1, T2>(CreepScore.Region region,
            T1 data,
            T2 staticDataType,
            StorageFolder localFolder,
            StorageFile file,
            string fileName,
            Func<CreepScore.Region, T2, string, string, bool, Task<T1>> retrieveFunction, string locale = "", string version = "", bool dataById = false)
        {
            // load data from API
            Task task = Task.Run(async () => { data = await retrieveFunction(region, staticDataType, locale, version, dataById); });
            task.Wait();
            // then write it out to file
            await FileIO.WriteTextAsync(file, data.ToString());
            return data;
        }

        async Task<T1> OverwriteData<T1, T2>(CreepScore.Region region,
            T1 data,
            T2 staticDataType,
            StorageFolder localFolder,
            StorageFile file,
            string fileName,
            Func<CreepScore.Region, T2, string, string, Task<T1>> retrieveFunction, string locale = "", string version = "")
        {
            // load data from API
            Task task = Task.Run(async () => { data = await retrieveFunction(region, staticDataType, locale, version); });
            task.Wait();
            // then write it out to file
            await FileIO.WriteTextAsync(file, data.ToString());
            return data;
        }

        void LoadVersions(CreepScore.Region region)
        {
            Task task = Task.Run(async () => { AppConstants.versions = await AppConstants.creepScore.RetrieveVersions(region); });
            task.Wait();
        }

        void LaunchIntoApplication()
        {
            Frame.Navigate(typeof(MainPage));
        }
    }
}
