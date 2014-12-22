using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CreepScoreAPI;
using CreepScoreAPI.Constants;
using CreepScoreApp.Constants;
using Windows.Storage;
using Windows.UI.Xaml.Controls;

namespace CreepScoreApp
{
    public class RegionComboBoxSettings
    {
        ComboBox comboBox;
        bool canChangeSettings;

        public RegionComboBoxSettings(ComboBox comboBox, bool canChangeSettings)
        {
            this.comboBox = comboBox;
            this.canChangeSettings = canChangeSettings;
            comboBox.SelectedIndex = AppConstants.SetCreepScoreRegion((int)AppConstants.preferredRegion);
            comboBox.SelectionChanged += comboBox_SelectionChanged;
        }

        void comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (canChangeSettings)
            {
                AppConstants.preferredRegion = (CreepScore.Region)AppConstants.GetCreepScoreRegion(comboBox.SelectedIndex);
            }
        }

        public  async Task SaveRegionSetting()
        {
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;
            AppConstants.preferredRegion = (CreepScore.Region)AppConstants.GetCreepScoreRegion(comboBox.SelectedIndex);
            StorageFile regionFile = await AppConstants.TryCreateFile("Region.json", localFolder);
            await AppConstants.OverwriteFile(regionFile, UrlConstants.GetRegion(AppConstants.preferredRegion));
        }
    }
}
