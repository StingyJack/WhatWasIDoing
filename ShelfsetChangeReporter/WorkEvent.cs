namespace ShelfsetChangeReporter
{
    using System;

    public class WorkEvent
    {
        public DateTime EventTime { get; set; }
        public string Day => EventTime.DayOfWeek.ToString();
        public string TimeFrame
        {
            get
            {
                var tod = EventTime.TimeOfDay;
                string timeRound = null;
                if ((tod.Minutes >= 0 && tod.Minutes <= 7)
                    || tod.Minutes >= 53)
                {
                    timeRound = "00";
                }
                else if (tod.Minutes >= 8 && tod.Minutes <= 22)
                {
                    timeRound = "15";
                }
                else if (tod.Minutes >= 23 && tod.Minutes <= 37)
                {
                    timeRound = "30";
                }
                else if (tod.Minutes >= 38 && tod.Minutes <= 52)
                {
                    timeRound = "45";
                }
                return $"{tod.Hours}:{timeRound}";
            }
        }

        public string Event { get; set; }
    }
}