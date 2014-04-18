using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BoardGames
{
    public enum BoardGameType { All, Abstract, Customizable, Childrens, Family, Party, Strategy, Thematic, War }

    /// <summary>
    /// http://www.developer.nokia.com/Community/Wiki/HTML_Page_parsing_using_HTMLAgilityPack
    /// C:\Program Files (x86)\Microsoft SDKs\Silverlight\v4.0\Libraries\Client\System.Xml.XPath.dll
    /// </summary>
    public class BggTopListParser
    {
        public static readonly string ALL_GAMES = "";
        public static readonly string ABSTRACT_GAMES = "abstracts";
        public static readonly string CUSTOMIZABLE_GAMES = "cgs";
        public static readonly string CHILDRENS_GAMES = "childrensgames";
        public static readonly string FAMILY_GAMES = "familygames";
        public static readonly string PARTY_GAMES = "partygames";
        public static readonly string STRATEGY_GAMES = "strategygames";
        public static readonly string THEMATIC_GAMES = "thematic";
        public static readonly string WAR_GAMES = "wargames";

        private static readonly string BASE_URL = "http://www.boardgamegeek.com/";
        private static readonly string BROWSE_BOARDGAME_URL = "/browse/boardgame/page/";

        private int currentPage;
        public int CurrentPage
        {
            get
            {
                return currentPage;
            }
        }

        private BoardGameType boardGameType;
        private bool finished = false;

        public BggTopListParser()
            : this(BoardGameType.All)
        {
        }

        public BggTopListParser(BoardGameType boardGameType)
        {
            currentPage = 1;
            this.boardGameType = boardGameType;
        }

        public BggTopListParser(string boardGameType)
            : this((BoardGameType) Enum.Parse(typeof(BoardGameType), boardGameType))
        {
        }

        public async Task<IEnumerable<int>> NextPage()
        {
            if (finished)
            {
                return Enumerable.Empty<int>();
            }
            string url = SearchUrl(currentPage);
            IEnumerable<int> result = await Parse(url);
            if (!result.Any())
            {
                finished = true;
            }
            currentPage++;
            return result;
        }

        public string SearchUrl()
        {
            return SearchUrl(currentPage);
        }

        public string SearchUrl(int page)
        {
            string stringType;
            switch (boardGameType)
            {
                case BoardGameType.Abstract:
                    stringType = ABSTRACT_GAMES;
                    break;
                case BoardGameType.Customizable:
                    stringType = CUSTOMIZABLE_GAMES;
                    break;
                case BoardGameType.Childrens:
                    stringType = CHILDRENS_GAMES;
                    break;
                case BoardGameType.Family:
                    stringType = FAMILY_GAMES;
                    break;
                case BoardGameType.Party:
                    stringType = PARTY_GAMES;
                    break;
                case BoardGameType.Strategy:
                    stringType = STRATEGY_GAMES;
                    break;
                case BoardGameType.Thematic:
                    stringType = THEMATIC_GAMES;
                    break;
                case BoardGameType.War:
                    stringType = WAR_GAMES;
                    break;
                default:
                    stringType = ALL_GAMES;
                    break;
            }
            return BASE_URL + stringType + BROWSE_BOARDGAME_URL + page;
        }

        private async Task<IEnumerable<int>> Parse(string url)
        {
            return await Parse(new Uri(url));
        }

        private async Task<IEnumerable<int>> Parse(Uri url)
        {
            WebClient client = new WebClient();
            string stringHtml = await client.DownloadStringTaskAsync(url);
            return ParseHtml(stringHtml);
        }

        private IEnumerable<int> ParseHtml(string stringHtml)
        {
            IList<int> result = new List<int>();
            HtmlDocument htmlDocument = new HtmlDocument();
            htmlDocument.OptionFixNestedTags = true;
            htmlDocument.LoadHtml(stringHtml);

            var collectionTableCollection = htmlDocument.DocumentNode.SelectNodes("//table[@class='collection_table']/tr[@id='row_']/td/div[starts-with(@id, 'results_objectname')]/a");

            if (collectionTableCollection != null)
            {
                foreach (HtmlNode row in collectionTableCollection)
                {
                    string gameUrl = row.GetAttributeValue("href", null);
                    string[] splitGameUrl = gameUrl.Split('/');
                    result.Add(int.Parse(splitGameUrl[2]));
                }
            }

            Console.WriteLine(String.Format("Parsed {0} ids from {1}.", result.Count.ToString(), SearchUrl()));

            return result;
        }
    }
}
