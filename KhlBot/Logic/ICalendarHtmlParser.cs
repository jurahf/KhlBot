using KhlBot.Model;

namespace KhlBot.Logic
{
    public interface ICalendarHtmlParser
    {
        List<MatchInfo> ParseMatches(string html);
    }
}
