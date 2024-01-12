using KhlBot.Model;
using System.Text.RegularExpressions;

namespace KhlBot.Logic
{
    /// <summary>
    /// Парсер html с расписанием игр с сайта khl.ru
    /// </summary>
    public class KhlRuCalendarHtmlParser : ICalendarHtmlParser
    {
        public List<MatchInfo> ParseMatches(string html, DateTime date)
        {
            List<MatchInfo> result = new List<MatchInfo>();
            string scheduleStr;

            do
            {
                scheduleStr = GetTag(html, "div", "card-games_list");        // списка может быть 2 - будущие и прошедшие

                if (string.IsNullOrEmpty(scheduleStr))
                    break;

                html = html.Replace(scheduleStr, ""); // убираем обрабатываемый список матчей

                string matchStr = "";
                do
                {
                    matchStr = GetTag(scheduleStr, "div", "card-game--calendar");       // --calendar только у будущих матчей

                    if (string.IsNullOrEmpty(matchStr))
                        matchStr = GetTag(scheduleStr, "div", "card-game");

                    if (string.IsNullOrEmpty(matchStr))
                        break;

                    scheduleStr = scheduleStr.Replace(matchStr, "");  // убираем обработанный матч

                    result.Add(ParseMatch(matchStr, date));
                } while (!string.IsNullOrEmpty(matchStr));

            } while (!string.IsNullOrEmpty(scheduleStr));



            return result;
        }

        private string GetTag(string scheduleStr, string tag, string cssClass)
        {
            string start = FindTagStartWithClass(scheduleStr, tag, cssClass);

            if (string.IsNullOrEmpty(start))
                return "";

            string result = FindEndTag(start, tag);

            return result;
        }

        private MatchInfo ParseMatch(string matchStr, DateTime date)
        {
            MatchInfo match = new MatchInfo();

            // хозяева
            string leftInfo = GetTag(matchStr, "a", "card-game__club_left");
            string leftNameTag = GetTag(leftInfo, "p", "card-game__club-name");
            string leftCityTag = GetTag(leftInfo, "p", "card-game__club-local");

            match.Left = new Club()
            {
                Name = GetTagValue(leftNameTag),
                City = GetTagValue(leftCityTag),
            };

            // гости
            string rightInfo = GetTag(matchStr, "a", "card-game__club_right");
            string rightNameTag = GetTag(rightInfo, "p", "card-game__club-name");
            string rightCityTag = GetTag(rightInfo, "p", "card-game__club-local");

            match.Right = new Club()
            {
                Name = GetTagValue(rightNameTag),
                City = GetTagValue(rightCityTag),
            };

            // общее - счет или время игры
            string centerTag = GetTag(matchStr, "div", "card-game__center");
            string timeTag = GetTag(centerTag, "p", "card-game__center-time");      // только у будущих матчей

            if (!string.IsNullOrEmpty(timeTag))
            {
                string timeStr = GetTagValue(timeTag);
                string[] timeParts = timeStr.Split(':');
                // вычитаем 3, так как время на сайте московское            
                match.DateAndTime = new DateTime(date.Year, date.Month, date.Day, int.Parse(timeParts[0]) - 3, int.Parse(timeParts[1]), 0, DateTimeKind.Utc);
            }
            else
            {
                // матч завершен, смотрим результат
                string leftScoreTag = GetTag(centerTag, "span", "card-game__center-score-left");
                string rightScoreTag = GetTag(centerTag, "span", "card-game__center-score-right");

                match.Result = $"{GetTagValue(leftScoreTag)} - {GetTagValue(rightScoreTag)}";
            }

            // трансляции
            string hoverTag = GetTag(matchStr, "div", "card-game__hover");
            //match.Translations = 

            return match;
        }

        private string GetTagValue(string leftNameTag)
        {
            Regex regex = new Regex(">(?<body>.*?)<", RegexOptions.Multiline | RegexOptions.Singleline);

            if (!regex.IsMatch(leftNameTag))
                return "";

            Match m = regex.Match(leftNameTag);
            return m.Groups["body"]?.Value?.Trim();
        }


        /// <summary>
        /// Возвращает подстроку с первым заданным тегом с заданным классом и до конца строки
        /// </summary>
        /// <param name="text"></param>
        /// <param name="tag"></param>
        /// <param name="cssClass"></param>
        /// <returns></returns>
        private string FindTagStartWithClass(string text, string tag, string cssClass)
        {
            Regex regex = new Regex($"<{tag}[^>]*{cssClass}\\W.*?>.*", RegexOptions.Multiline | RegexOptions.Singleline);

            return regex.Match(text)?.Value;
        }


        /// <summary>
        /// Возвращает подстроку, с начала и до закрытия текущего заданного тега
        /// </summary>
        /// <param name="text"></param>
        /// <param name="tag"></param>
        /// <returns></returns>
        private string FindEndTag(string text, string tag)
        {
            // циклом найдем конец этого дива
            int divCount = 1;
            int index = 1;
            do
            {
                int openIndex = text.IndexOf($"<{tag}", index);
                int closeIndex = text.IndexOf($"</{tag}", index);

                if (openIndex == -1 && closeIndex == -1)
                    break;

                if (openIndex < closeIndex && openIndex >= 0)
                {
                    index = openIndex + $"<{tag}>".Length;
                    divCount++;
                }
                else if (closeIndex >= 0)
                {
                    index = closeIndex + $"</{tag}>".Length;
                    divCount--;
                }

            } while (divCount > 0);

            return text.Substring(0, index);
        }
    }
}
