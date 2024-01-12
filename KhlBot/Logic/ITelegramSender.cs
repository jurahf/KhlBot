namespace KhlBot.Logic
{
    public interface ITelegramSender
    {
        Task SendText(string text);
    }
}
