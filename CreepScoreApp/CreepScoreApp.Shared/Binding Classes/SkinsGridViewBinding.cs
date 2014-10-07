using System;
using System.Collections.Generic;

namespace CreepScoreApp
{
    public class SkinsGridViewBinding
    {
        public Uri Image { get; set; }
        public string Name { get; set; }

        public SkinsGridViewBinding(Uri image, string name)
        {
            this.Image = image;
            this.Name = name;
        }
    }
}
