namespace KhlBot.Logic
{
    public interface ICalendarHtmlProvider
    {
        Task<string> GetHtmlAsync(DateTime date);
    }
}
