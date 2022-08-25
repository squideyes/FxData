using SquidEyes.Basics;

namespace SquidEyes.FxData.Models;

public static class DateTimeHelper
{
    private static readonly TimeSpan NewYorkOpen = TimeSpan.FromHours(13);
    private static readonly TimeSpan NewYorkClose = new(0, 21, 59, 59, 999);
    private static readonly TimeSpan LondonOpen = TimeSpan.FromHours(7);
    private static readonly TimeSpan LondonClose = new(0, 15, 59, 59, 999);

    internal static (DateTime, DateTime) GetMinAndMaxDateTimes(
          TradeDate tradeDate, Market market)
    {
        DateTime GetDateTime(TimeSpan offset)
        {
            return tradeDate.AsFunc(d => new DateTime(d.Year, d.Month, d.Day,
                0, 0, 0, DateTimeKind.Utc)).Add(offset).ToEasternFromUtc();
        }

        var offsets = market switch
        {
            Market.NewYork => (NewYorkOpen, NewYorkClose),
            Market.London => (LondonOpen, LondonClose),
            Market.Combined => (LondonOpen, NewYorkClose),
            _ => throw new ArgumentOutOfRangeException(nameof(Market)),
        };

        return (GetDateTime(offsets.Item1), GetDateTime(offsets.Item2));
    }
}