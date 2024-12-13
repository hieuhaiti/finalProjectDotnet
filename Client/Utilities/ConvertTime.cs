public class ConvertDt
{
    public DateTime UnixTimeStampToDateTime(double unixTimeStamp)
    {
        // Unix timestamp is seconds past epoch
        DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        dateTime = dateTime.AddSeconds(unixTimeStamp).ToLocalTime();
        return dateTime;
    }
    public long DateTimeToUnixTimeStamp(DateTime dateTime)
    {
        var roundedDt = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, 0, 0);
        return new DateTimeOffset(roundedDt).ToUnixTimeSeconds();
    }
}