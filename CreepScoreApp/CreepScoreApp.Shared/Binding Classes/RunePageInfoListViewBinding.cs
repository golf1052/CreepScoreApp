using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using CreepScoreAPI;
using CreepScoreApp.Constants;

namespace CreepScoreApp
{
    public class RunePageInfoListViewBinding
    {
        public string RuneInfo { get; set; }
        public string RuneValue { get; set; }
        public double value;
        public bool isPercent;
        public bool plus;
        public bool perLevel;

        public RunePageInfoListViewBinding(string runeInfo,
            double value,
            bool isPercent,
            bool plus,
            bool perLevel)
        {
            this.RuneInfo = runeInfo;
            this.value = value;
            this.isPercent = isPercent;
            this.plus = plus;
            this.perLevel = perLevel;
            RuneValue = value.ToString();
        }

        public void SetValue(double value)
        {
            this.value = value;
            RuneValue = value.ToString();
        }

        public void AddToValue(double value)
        {
            this.value += value;
            RuneValue = value.ToString();
        }
    }
}
