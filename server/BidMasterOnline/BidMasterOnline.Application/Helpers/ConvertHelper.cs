namespace BidMasterOnline.Application.Helpers
{
    public static class ConvertHelper
    {
        public static string TimeSpanTicksToString(long ticks)
        {
            var timeSpan = new TimeSpan(ticks);

            return $"{timeSpan.Days} d {timeSpan.Hours} hrs";
        }
    }
}
