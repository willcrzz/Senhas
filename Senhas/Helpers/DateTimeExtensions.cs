namespace Senhas.Helpers
{
    public static class DateTimeExtensions
    {
        public static DateTime ToCampoGrande(this DateTimeOffset dt)
        {
            var tz = TimeZoneInfo.FindSystemTimeZoneById("America/Campo_Grande");
            return TimeZoneInfo.ConvertTime(dt, tz).DateTime;
        }
    }
}