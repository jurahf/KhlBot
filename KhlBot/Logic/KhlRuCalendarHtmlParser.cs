using KhlBot.Model;
using System.Text.RegularExpressions;

namespace KhlBot.Logic
{
    /// <summary>
    /// Парсер html с расписанием игр с сайта khl.ru
    /// </summary>
    public class KhlRuCalendarHtmlParser : ICalendarHtmlParser
    {
        public List<MatchInfo> ParseMatches(string html)
        {
            // память расходуем только так
            string scheduleStr = GetTag(html, "div", "card-games_list");

            if (string.IsNullOrEmpty(scheduleStr))
            {
                return new List<MatchInfo>();
            }

            List<MatchInfo> result = new List<MatchInfo>();
            string matchStr = "";
            do
            {
                matchStr = GetTag(scheduleStr, "div", "card-game--calendar");
                if (string.IsNullOrEmpty(matchStr))
                    break;

                scheduleStr = scheduleStr.Replace(matchStr, string.Empty);

                result.Add(ParseMatch(matchStr));
            } while (!string.IsNullOrEmpty(matchStr));

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

        private MatchInfo ParseMatch(string matchStr)
        {
            MatchInfo match = new MatchInfo();

            string leftInfo = GetTag(matchStr, "a", "card-game__club_left");
            string leftNameTag = GetTag(leftInfo, "p", "card-game__club-name");

            match.Left = new Club()
            {
                Name = GetTagValue(leftNameTag)
            };

            string rightInfo = GetTag(matchStr, "a", "card-game__club_right");
            string rightNameTag = GetTag(rightInfo, "p", "card-game__club-name");

            match.Right = new Club()
            {
                Name = GetTagValue(rightNameTag)
            };

            return match;
        }

        private string GetTagValue(string leftNameTag)
        {
            Regex regex = new Regex(">(?<body>.*?)<");

            if (!regex.IsMatch(leftNameTag))
                return "";

            Match m = regex.Match(leftNameTag);
            return m.Groups["body"].Value;
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
            Regex regex = new Regex($"<{tag}[^>]*{cssClass}.*?>.*", RegexOptions.Multiline | RegexOptions.Singleline);

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
