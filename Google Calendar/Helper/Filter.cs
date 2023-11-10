using Google.Apis.Calendar.v3.Data;
using System;
using System.Collections.Generic;
using System.Linq;
namespace Google_Calendar.Helper
{
    public class Filter
    {
        public static List<Event> Fliter_Data(List<Event> allEvents, string location, string desc, string summary, DateTime start,
            DateTime end)
        {
            if (end == DateTime.MinValue)
                end = DateTime.MaxValue;
            if (location != null)
                allEvents = allEvents.Where(e => e.Location.Contains(location, StringComparison.OrdinalIgnoreCase)).ToList();
            if (desc != null)
                allEvents = allEvents.Where(e => e.Description.Contains(desc, StringComparison.OrdinalIgnoreCase)).ToList();
            if (summary != null)
                allEvents = allEvents.Where(e => e.Summary.Contains(summary, StringComparison.OrdinalIgnoreCase)).ToList();
            if (end == DateTime.MinValue)
                end = DateTime.MaxValue;
            allEvents = allEvents.Where(e => (e.Start.DateTime.Value >= start && e.End.DateTime.Value <= end)).ToList();
            return allEvents;

        }
    }
}
