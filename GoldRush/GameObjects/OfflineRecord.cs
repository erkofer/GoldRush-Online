using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldRush
{
    class OfflineRecord
    {
        public OfflineRecord()
        {
            
        }

        public Dictionary<int, Item> Items = new Dictionary<int, Item>();
        public long TimeGone { private get; set; }

        public string FormattedTimeGone
        {
            get
            {
                var timeSpan = TimeSpan.FromSeconds(TimeGone);
                var days = (timeSpan.Days > 0) ? timeSpan.Days+" days, ":"";
                var hours = (timeSpan.Hours > 0) ? timeSpan.Hours + " hours, " : "";
                var minutes = (timeSpan.Minutes > 0) ? timeSpan.Minutes + " minutes.": " 1 minute.";
                var timeString = "You were gone for ";
                timeString += days;
                timeString += hours;
                timeString += minutes;
                return timeString;
            }
        }

        public class Item
        {
            public int Id { get; set; }
            public long InitialValue { get; set; }
            public long EndValue { get; set; }
            public long Change { get { return EndValue - InitialValue; }}
        }
    }
}
