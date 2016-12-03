using LivrETS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LivrETS.ViewModels
{
    public class FairStaticsViewModels
    {

    }
    public class FlotStatisticsPrice
    {
        public string label { get; set; }
        public double data { get; set; }
    }

    public class FlotStatisticsCount
    {
        public string label { get; set; }
        public int data { get; set; }
    }

    public class TabStatistics{
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Acronym { get; set; }
        public double Ventes { get; set; }
        public int Total { get; set; }
    }

    public class MorrisFairsStatistic
    {
        public string trimester { get; set; }
        public int year { get; set; }
        public int articles { get; set; }
        public int articles_sold { get; set; }
    }

}