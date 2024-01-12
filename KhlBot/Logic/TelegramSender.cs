using KhlBot.Model;
using Microsoft.Extensions.Options;
using System.Net;
using System.Threading.Channels;

namespace KhlBot.Logic
{
    public class TelegramSender : ITelegramSender
    {
        private readonly TelegramConfig config;

        public TelegramSender(IOptions<TelegramConfig> config) 
        {
            this.config = config.Value;
        }

        public async Task SendText(string text)
        {
            string msg = WebUtility.UrlEncode(text);

            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync($"https://api.telegram.org/bot{config.Token}/sendMessage?chat_id={config.ChatId}&text={msg}&parse_mode=HTML&disable_web_page_preview=true");
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw new Exception($"Couldn't send a message via Telegram. Response from Telegram API: {response.Content.ReadAsStringAsync().Result}");
                }
            }
        }
    }
}
