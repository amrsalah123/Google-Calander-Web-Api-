using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Calendar.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Google_Calendar.Interfaces;
using Google_Calendar.DTO;
using System.IO;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using System;
using Google;
using System.Net;
using System.Linq;
using Google_Calendar.Helper;
namespace Google_Calendar.Services
{
    public class CalenderServices:ICalenderServices
    {
        string[] Scopes = { "https://www.googleapis.com/auth/calendar" };
        string ApplicationName = "Google Canlendar Api";
        UserCredential credential;
        CalendarService services;
        public CalenderServices()
        {
            using (var stream = new FileStream(Path.Combine(Directory.GetCurrentDirectory(), "Cre", "cre.json"), FileMode.Open, FileAccess.Read))
            {
                string credPath = "token.json";
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
            }
            // define services
             services = new CalendarService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });

        }
        public  async Task<Event> CreateGoogleCalendar(CalenderDTO request)
        {
                       // define request
            Event eventCalendar = new Event()
            {
                Summary = request.Summary,
                Location = request.Location,
                Start = new EventDateTime
                {
                    DateTime = request.Start,
                    TimeZone = "Africa/Cairo"
                },
                End = new EventDateTime
                {
                    DateTime = request.End,
                    TimeZone = "Africa/Cairo"
                },
                Description = request.Description
            };
           
            var eventRequest = services.Events.Insert(eventCalendar, "primary");
            var requestCreate = await eventRequest.ExecuteAsync();
            return requestCreate;
        }
   
        public async Task<bool> DeleteGoogleCalendarEvent(string eventId)
        {
            
            try
            {
                // Check if event exists
                var getEventRequest = services.Events.Get("primary", eventId);
                await getEventRequest.ExecuteAsync();
            }
            catch (GoogleApiException ex) when (ex.HttpStatusCode == HttpStatusCode.NotFound)
            {
                // Event does not exist
                return false;
            }
            catch (Exception ex)
            {
                // An error occurred while retrieving the event
                Console.WriteLine($"Error retrieving event: {ex.Message}");
                return false;
            }

            // Event exists, proceed with deletion
            try
            {
                var deleteRequest = services.Events.Delete("primary", eventId);
                await deleteRequest.ExecuteAsync();
                return true;
            }
            catch (Exception ex)
            {
                // An error occurred while deleting the event
                Console.WriteLine($"Error deleting event: {ex.Message}");
                return false;
            }
        }

        public async Task<List<string>> GetAllGoogleCalendarEventIds()
        {
            
            List<string> eventIds = new List<string>();

            try
            {
                var listRequest = services.Events.List("primary");
                listRequest.MaxResults = 1000; // Adjust as per your requirement

                do
                {
                    var events = await listRequest.ExecuteAsync();

                    if (events.Items != null && events.Items.Count > 0)
                    {
                        foreach (var eventItem in events.Items)
                        {
                            eventIds.Add(eventItem.Id);
                        }
                    }

                    listRequest.PageToken = events.NextPageToken;
                } while (!string.IsNullOrEmpty(listRequest.PageToken));

                return eventIds;
            }
            catch (Exception ex)
            {
                // Handle any exceptions here
                Console.WriteLine("An error occurred: " + ex.Message);
                return null;
            }
        }


        public async Task<List<CalenderDTO>> GetAllEvents(string location, string desc, string summary, DateTime start, DateTime end, string pageToken = null, int pageSize = 10)
        {
            
            // Define request to retrieve events
            EventsResource.ListRequest request = services.Events.List("primary");
            request.TimeMin = DateTime.Now;
            request.ShowDeleted = false;
            request.SingleEvents = true;
            request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;
            request.PageToken = pageToken;
            request.MaxResults = pageSize;

            // Execute request and get the list of events
            Events events = await request.ExecuteAsync();
            List<Event> allEvents = events.Items.ToList();
            List<CalenderDTO> returnEventList = new List<CalenderDTO>();
            allEvents = Filter.Fliter_Data(allEvents, location, desc, summary, start, end);
            foreach (var item in allEvents)
            {
                CalenderDTO obj = new CalenderDTO()
                {
                    Description = item.Description,
                    Summary = item.Summary,
                    Location = item.Location,
                    Start = item.Start.DateTime.Value,
                    End = item.End.DateTime.Value
                };
                returnEventList.Add(obj);
            }

            // Check if there are more events
            if (!string.IsNullOrEmpty(events.NextPageToken))
            {
                // Recursively fetch the next page of events and add them to the list
                List<CalenderDTO> nextPageEvents = await GetAllEvents(location, desc, summary, start, end, events.NextPageToken, pageSize);
                returnEventList.AddRange(nextPageEvents);
            }

            return returnEventList;
        }


    }
   

}
