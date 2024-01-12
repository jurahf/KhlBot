namespace KhlBot.Logic
{
    /// <summary>
    /// HTML расписания матчей с сайта khl.ru
    /// </summary>
    public class KhlRuCalendarHtmlProvider : ICalendarHtmlProvider
    {
        public async Task<string> GetHtmlAsync(DateTime date)
        {
            using (HttpClient client = new HttpClient())
            {
                // азаза
                client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36 OPR/106.0.0.0");

                var response = await client.GetAsync(GetUrl(date));

                if (!response.IsSuccessStatusCode)
                    throw new Exception("Ошибка выполнения запроса к источнику данных");

                return await response.Content.ReadAsStringAsync();
            }
        }

        public string GetUrl(DateTime date)
        {
            // TODO: что такое 1217 ?
            return $"https://www.khl.ru/calendar/1217/{date.Month:00}/?day={date.Day}";
        }
    }
}
