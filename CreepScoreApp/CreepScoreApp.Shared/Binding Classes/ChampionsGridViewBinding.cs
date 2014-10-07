using System;
using System.Collections.Generic;
using CreepScoreAPI;

namespace CreepScoreApp
{
    public class ChampionsGridViewBinding
    {
        public Uri Image { get; set; }
        public string Name { get; set; }
        public int ImageWidth { get; set; }
        public int ImageHeight { get; set; }

        public ChampionStatic champion;
        public string dataName;

        public ChampionsGridViewBinding(Uri image, int imageWidth, int imageHeight, string dataName, ChampionStatic champion)
        {
            this.champion = champion;
            this.Image = image;
            this.ImageWidth = imageWidth;
            this.ImageHeight = imageHeight;
            this.dataName = dataName;
            this.Name = champion.name;
        }
    }
}
