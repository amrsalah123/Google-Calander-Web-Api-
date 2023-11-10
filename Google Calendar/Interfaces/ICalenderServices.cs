using Google.Apis.Calendar.v3.Data;
using Google_Calendar.DTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Google_Calendar.Interfaces
{
    public interface ICalenderServices
    {
        Task<Event> CreateGoogleCalendar(CalenderDTO request);
        //  Task<List<string>> GetAllEventIds();
        // Task<List<DateTime>> GetGoogleCalendarEventStartDates();
        Task<bool> DeleteGoogleCalendarEvent(string eventId);
        Task<List<string>> GetAllGoogleCalendarEventIds();
        Task<List<CalenderDTO>> GetAllEvents(string location, string desc, string summary, DateTime start, DateTime end, string pageToken = null, int pageSize = 10);
    }
}
