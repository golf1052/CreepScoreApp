using System;
using System.Collections.Generic;

namespace CreepScoreApp
{
    public class StatusListViewBinding
    {
        public string Author { get; set; }
        public string Date { get; set; }
        public string Message { get; set; }

        public StatusListViewBinding(string author, string date, string message)
        {
            if (author == null || author == "")
            {
                this.Author = "Riot";
            }
            else
            {
                this.Author = author;
            }
            this.Date = date;
            this.Message = message;
        }
    }
}
