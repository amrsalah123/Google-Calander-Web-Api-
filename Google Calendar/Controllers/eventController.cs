using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Google_Calendar.Services;
using Google_Calendar.Interfaces;
using Google_Calendar.DTO;

namespace Google_Calendar.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class eventController : ControllerBase
    {
        private readonly ICalenderServices _CalenderServices;

        public eventController(ICalenderServices calenderServices)
        {
            this._CalenderServices = calenderServices;
        }
        [HttpPost]
        public async Task<IActionResult> CreateGoogleCalendar([FromBody] CalenderDTO request)
        {
            if (ModelState.IsValid)
            {
              var day=  request.Start.DayOfWeek.ToString();
                if (day == "Friday" || day == "Saturday" || request.Start < DateTime.Now)
                    return BadRequest("Friday or Satueday or past not avilable");
                 return StatusCode(201, await _CalenderServices.CreateGoogleCalendar(request));
            }
            return BadRequest();
        }

        
        [HttpDelete("{EventId}")]
        public async Task<IActionResult> Delete(string EventId)
        {
            if (EventId == "")
                return BadRequest("id is null");
            else
            { 
                var b=await _CalenderServices.DeleteGoogleCalendarEvent(EventId);
                if (b)
                    return NoContent();
                return BadRequest("no event");
            }
        }

        [HttpGet("ids")]
        public async Task<IActionResult> get() => Ok( await _CalenderServices.GetAllGoogleCalendarEventIds());
        
        [HttpGet]
        public async Task<IActionResult> getevent(string location, string desc, string summary, DateTime start,
            DateTime end,string pagetoken,int pagesize)
            => Ok(await _CalenderServices.GetAllEvents(location,  desc,  summary,  start,
             end,pagetoken=null,pagesize=10));

        [HttpGet("test")]
        public IActionResult test(string dt)
        {
            
            string main = "hijij";
            main += "";
            if (main.Contains(dt))
                return Ok();
            return BadRequest();

        }


    }
}
