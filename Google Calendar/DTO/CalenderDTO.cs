﻿using System;

namespace Google_Calendar.DTO
{
    public class CalenderDTO
    {
        public string Summary { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
    }
}
