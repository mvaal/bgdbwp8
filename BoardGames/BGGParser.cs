using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace BoardGames
{
    public class BGGParser
    {
        public async Task<IEnumerable<BoardGame>> Parse(string boardGameName)
        {
            IEnumerable<int> boardGameIds = await GetBoardGameIds(boardGameName);
            IEnumerable<BoardGame> boardGames = Enumerable.Empty<BoardGame>();
            if (boardGameIds.Any())
            {
                boardGames = await GetBoardGames(boardGameIds);
            }

            return boardGames;
        }

        public async Task<IEnumerable<int>> GetBoardGameIds(string boardGameName)
        {
            using (Stream stream = await BoardGameNameSearchUrlStream(boardGameName))
            {
                using (XmlReader xmlReader = XmlReader.Create(stream))
                {
                    XmlSerializer deserializer = new XmlSerializer(typeof(BoardGameList));
                    BoardGameList query = deserializer.Deserialize(xmlReader) as BoardGameList;
                    return query.BoardGames.Select(bg => bg.ObjectId);
                }
            }
        }

        protected virtual async Task<Stream> BoardGameNameSearchUrlStream(string boardGameName)
        {
            string searchUrl = "http://www.boardgamegeek.com/xmlapi/search?search=" + boardGameName;
            var client = new WebClient();
            return await client.OpenReadTaskAsync(searchUrl);
        }

        public async Task<IEnumerable<BoardGame>> GetBoardGames(IEnumerable<int> boardGameIds)
        {
            IEnumerable<BoardGame> result = Enumerable.Empty<BoardGame>();
            if (boardGameIds.Any())
            {
                using (Stream stream = await BoardGameIdsSearchUrlStream(boardGameIds))
                {
                    using (XmlReader xmlReader = XmlReader.Create(stream))
                    {
                        XmlSerializer deserializer = new XmlSerializer(typeof(BoardGameList));
                        BoardGameList query = deserializer.Deserialize(xmlReader) as BoardGameList;
                        result = query.BoardGames;
                    }
                }
            }
            return result;
        }

        protected virtual async Task<Stream> BoardGameIdsSearchUrlStream(IEnumerable<int> boardGameIds)
        {
            string joinedIdList = string.Join(",", boardGameIds);
            string detailedUrl = "http://www.boardgamegeek.com/xmlapi/boardgame/" + joinedIdList + "?stats=1";
            var client = new WebClient();
            return await client.OpenReadTaskAsync(detailedUrl);
        }
    }

    [XmlRootAttribute("boardgames", IsNullable = false)]
    public class BoardGameList
    {
        [XmlAttribute("termsofuse")]
        public string termsOfUse;

        [XmlIgnore]
        public string TermsOfUse
        {
            get
            {
                return termsOfUse;
            }
            set
            {
                termsOfUse = value;
            }
        }

        [XmlIgnore]
        private BoardGame[] boardGames;

        [XmlElement("boardgame")]
        public BoardGame[] BoardGames
        {
            get
            {
                return boardGames ?? new BoardGame[0];
            }
            set
            {
                boardGames = value;
            }
        }
    }

    public class BoardGame
    {
        [XmlAttribute("objectid")]
        public int ObjectId { get; set; }

        [XmlAttribute("subtypemismatch")]
        public bool SubtypeMismatch { get; set; }

        [XmlIgnore]
        public string ImageName
        {
            get
            {
                return ObjectId + ".jpg";
            }
        }

        [XmlElement("yearpublished")]
        public string YearPublishedString { get; set; }

        [XmlIgnore]
        public int? YearPublished
        {
            get
            {
                int result;
                if (int.TryParse(YearPublishedString, out result))
                {
                    if (result == 0)
                    {
                        return null;
                    }
                    return result;
                }
                return null;
            }
        }

        [XmlElement("minplayers")]
        public string MinPlayersString { get; set; }

        [XmlIgnore]
        public int? MinPlayers
        {
            get
            {
                int result;
                if (int.TryParse(MinPlayersString, out result))
                {
                    return result;
                }
                return result;
            }
        }

        [XmlElement("maxplayers")]
        public string MaxPlayersString { get; set; }

        [XmlIgnore]
        public int? MaxPlayers
        {
            get
            {
                int result;
                if (int.TryParse(MaxPlayersString, out result))
                {
                    return result;
                }
                return null;
            }
        }

        [XmlIgnore]
        public string FormattedPlayers
        {
            get
            {
                if (MinPlayersString != null && MaxPlayersString != null)
                {
                    if (MinPlayersString == MaxPlayersString)
                    {
                        return MinPlayersString;
                    }
                    else
                    {
                        return MinPlayersString + "-" + MaxPlayersString;
                    }
                }
                else if (MinPlayersString != null)
                {
                    return MinPlayersString;
                }
                else if (MaxPlayersString != null)
                {
                    return MaxPlayersString;
                }
                return null;
            }
        }

        [XmlElement("playingtime")]
        public string PlayingTimeString { get; set; }

        [XmlIgnore]
        public int? PlayingTime
        {
            get
            {
                int result;
                if (int.TryParse(PlayingTimeString, out result))
                {
                    return result;
                }
                return null;
            }
        }

        [XmlElement("age")]
        public string AgeString { get; set; }

        [XmlIgnore]
        public int? Age
        {
            get
            {
                int result;
                if (int.TryParse(AgeString, out result))
                {
                    return result;
                }
                return null;
            }
        }

        [XmlIgnore]
        public string FormattedAge
        {
            get
            {
                if (AgeString != null)
                {
                    return Age + "+";
                }
                return null;
            }
        }

        [XmlIgnore]
        public Name[] names;

        [XmlElement("name")]
        public Name[] Names
        {
            get
            {
                return names ?? new Name[0];
            }
            set
            {
                names = value;
            }
        }

        private string name;
        [XmlIgnore]
        public string Name
        {
            get
            {
                if (name == null)
                {
                    name = Names.Where(n => n.Primary).Select(n => n.Value).Single();
                }
                return name;
            }
            set
            {
                name = value;
            }
        }

        [XmlElement("description")]
        public string Description { get; set; }

        [XmlIgnore]
        public string DecodedDescription
        {
            get
            {
                return WebUtility.HtmlDecode(Description).Replace("<br/>", "\n");
            }
        }

        [XmlElement("thumbnail")]
        public string Thumbnail { get; set; }

        [XmlIgnore]
        public Uri ThumbnailUri
        {
            get
            {
                return Thumbnail != null ? new Uri(Thumbnail) : null;
            }
        }

        [XmlIgnore]
        public Uri ThumbnailUriDisplay
        {
            get
            {
                return ThumbnailUri ?? new Uri("/Assets/AppStoreIcon.png", UriKind.Relative);
            }
        }

        [XmlElement("image")]
        public string Image { get; set; }

        [XmlIgnore]
        private BGGObject[] boardGameExpansions;

        [XmlElement("boardgameexpansion")]
        public BGGObject[] BoardGameExpansions
        {
            get
            {
                return boardGameExpansions ?? new BGGObject[0];
            }
            set
            {
                boardGameExpansions = value;
            }
        }

        [XmlIgnore]
        private BGGObject[] boardGameHonors;

        [XmlElement("boardgamehonor")]
        public BGGObject[] BoardGameHonors
        {
            get
            {
                return boardGameHonors ?? new BGGObject[0];
            }
            set
            {
                boardGameHonors = value;
            }
        }

        [XmlIgnore]
        private BGGObject[] boardGamePublishers;

        [XmlElement("boardgamepublisher")]
        public BGGObject[] BoardGamePublishers
        {
            get
            {
                return boardGamePublishers ?? new BGGObject[0];
            }
            set
            {
                boardGamePublishers = value;
            }
        }

        [XmlIgnore]
        private BGGObject[] boardGameVersions;

        [XmlElement("boardgameversion")]
        public BGGObject[] BoardGameVersions
        {
            get
            {
                return boardGameVersions ?? new BGGObject[0];
            }
            set
            {
                boardGameVersions = value;
            }
        }

        [XmlIgnore]
        private BGGObject[] boardGameFamilies;

        [XmlElement("boardgamefamily")]
        public BGGObject[] BoardGameFamilies
        {
            get
            {
                return boardGameFamilies ?? new BGGObject[0];
            }
            set
            {
                boardGameFamilies = value;
            }
        }

        [XmlIgnore]
        private BGGObject[] boardGameMechanics;

        [XmlElement("boardgamemechanic")]
        public BGGObject[] BoardGameMechanics
        {
            get
            {
                return boardGameMechanics ?? new BGGObject[0];
            }
            set
            {
                boardGameMechanics = value;
            }
        }

        [XmlIgnore]
        private BGGObject[] boardGamePodcastEpisodes;

        [XmlElement("boardgamepodcastepisode")]
        public BGGObject[] BoardGamePodcastEpisodes
        {
            get
            {
                return boardGamePodcastEpisodes ?? new BGGObject[0];
            }
            set
            {
                boardGamePodcastEpisodes = value;
            }
        }

        [XmlIgnore]
        private BGGObject[] boardGameArtists;

        [XmlElement("boardgameartist")]
        public BGGObject[] BoardGameArtists
        {
            get
            {
                return boardGameArtists ?? new BGGObject[0];
            }
            set
            {
                boardGameArtists = value;
            }
        }

        [XmlIgnore]
        private BGGObject[] boardGameCategories;

        [XmlElement("boardgamecategory")]
        public BGGObject[] BoardGameCategories
        {
            get
            {
                return boardGameCategories ?? new BGGObject[0];
            }
            set
            {
                boardGameCategories = value;
            }
        }

        [XmlIgnore]
        public String FormattedBoardGameSubdomains
        {
            get
            {
                String result = "";
                if (boardGameSubdomains != null)
                {
                    result = String.Join("/", boardGameSubdomains.Select(t => t.Value));
                    return result;
                }
                return result;
            }
        }

        [XmlIgnore]
        private BGGObject[] boardGameSubdomains;

        [XmlElement("boardgamesubdomain")]
        public BGGObject[] BoardGameSubdomains
        {
            get
            {
                return boardGameSubdomains ?? new BGGObject[0];
            }
            set
            {
                boardGameSubdomains = value;
            }
        }

        [XmlIgnore]
        private BGGObject[] videoGameBGs;

        [XmlElement("videogamebg")]
        public BGGObject[] VideoGameBGs
        {
            get
            {
                return videoGameBGs ?? new BGGObject[0];
            }
            set
            {
                videoGameBGs = value;
            }
        }

        [XmlIgnore]
        private BGGObject[] boardGameDesigners;

        [XmlElement("boardgamedesigner")]
        public BGGObject[] BoardGameDesigners
        {
            get
            {
                return boardGameDesigners ?? new BGGObject[0];
            }
            set
            {
                boardGameDesigners = value;
            }
        }

        [XmlIgnore]
        private Poll[] polls;

        [XmlElement("poll")]
        public Poll[] Polls
        {
            get
            {
                return polls ?? new Poll[0];
            }
            set
            {
                polls = value;
            }
        }

        [XmlIgnore]
        private Statistics[] statistics;

        [XmlElement("statistics")]
        public Statistics[] Statistics
        {
            get
            {
                return statistics ?? new Statistics[0];
            }
            set
            {
                statistics = value;
            }
        }

        private List<Rank> rankList = new List<Rank>();
        private bool hasRankListBeenSet;

        [XmlIgnore]
        public List<Rank> RankList
        {
            get
            {
                if (hasRankListBeenSet)
                {
                    return this.rankList;
                }
                foreach (var stats in Statistics)
                {
                    foreach (var ratings in stats.Ratings)
                    {
                        foreach (var rank in ratings.Ranks.RankList.Where(r => r.BayesAverage != null))
                        {
                            rankList.Add(rank);
                        }
                    }
                }
                hasRankListBeenSet = true;
                return rankList;
            }
            set
            {
                rankList = value;
                hasRankListBeenSet = true;
            }
        }

        private int? rank = null;
        private bool hasRankBeenSet;

        public int? Rank
        {
            get
            {
                if (hasRankBeenSet)
                {
                    return this.rank;
                }
                this.rank = RankList.Where(r => (r.Id == 1)).Select(r => r.Value).SingleOrDefault();
                return this.rank;
            }
            set
            {
                this.rank = value;
                hasRankBeenSet = true;
            }
        }
    }

    public class Name
    {
        [XmlAttribute("primary")]
        public bool Primary { get; set; }

        [XmlAttribute("sortindex")]
        public string SortIndex { get; set; }

        [XmlText]
        public string Value { get; set; }
    }

    public class BGGObject
    {
        [XmlAttribute("objectid")]
        public int ObjectId { get; set; }

        [XmlAttribute("inbound")]
        public bool Inbound { get; set; }

        [XmlText]
        public string Value { get; set; }
    }

    public class Poll
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("title")]
        public string Title { get; set; }

        [XmlAttribute("totalvotes")]
        public int TotalVotes { get; set; }

        [XmlIgnore]
        private Results[] results;

        [XmlElement("results")]
        public Results[] Results
        {
            get
            {
                return results ?? new Results[0];
            }
            set
            {
                results = value;
            }
        }
    }

    public class Results
    {
        [XmlAttribute("numplayers")]
        public string NumPlayers { get; set; }

        [XmlIgnore]
        private Result[] resultList;

        [XmlElement("result")]
        public Result[] ResultList
        {
            get
            {
                return resultList ?? new Result[0];
            }
            set
            {
                resultList = value;
            }
        }
    }

    public class Result
    {
        [XmlAttribute("level")]
        public string LevelString;

        [XmlIgnore]
        public int? Level
        {
            get
            {
                int result;
                if (int.TryParse(LevelString, out result))
                {
                    return result;
                }
                return null;
            }
        }

        [XmlAttribute("value")]
        public string Value { get; set; }

        [XmlAttribute("numvotes")]
        public int NumVotes { get; set; }
    }

    public class Statistics
    {
        [XmlAttribute("page")]
        public int Page { get; set; }

        [XmlIgnore]
        private Ratings[] ratings;

        [XmlElement("ratings")]
        public Ratings[] Ratings
        {
            get
            {
                return ratings ?? new Ratings[0];
            }
            set
            {
                ratings = value;
            }
        }
    }

    public class Ratings
    {
        [XmlElement("usersrated")]
        public string UsersRatedString;

        [XmlIgnore]
        public int? UsersRated
        {
            get
            {
                int result;
                if (int.TryParse(UsersRatedString, out result))
                {
                    return result;
                }
                return null;
            }
        }

        [XmlElement("average")]
        public string AverageString;

        [XmlIgnore]
        public double? Average
        {
            get
            {
                double result;
                if (double.TryParse(AverageString, out result))
                {
                    return result;
                }
                return null;
            }
        }

        [XmlElement("bayesaverage")]
        public string BayesaAverageString;

        [XmlIgnore]
        public double? BayesaAverage
        {
            get
            {
                double result;
                if (double.TryParse(BayesaAverageString, out result))
                {
                    return result;
                }
                return null;
            }
        }

        [XmlIgnore]
        private Ranks ranks;

        [XmlElement("ranks")]
        public Ranks Ranks
        {
            get
            {
                return ranks ?? new Ranks();
            }
            set
            {
                ranks = value;
            }
        }

        [XmlElement("stddev")]
        public string StdDevString;

        [XmlIgnore]
        public double? StdDev
        {
            get
            {
                double result;
                if (double.TryParse(StdDevString, out result))
                {
                    return result;
                }
                return null;
            }
        }

        [XmlElement("median")]
        public string MedianString;

        [XmlIgnore]
        public int? Median
        {
            get
            {
                int result;
                if (int.TryParse(MedianString, out result))
                {
                    return result;
                }
                return null;
            }
        }

        [XmlElement("owned")]
        public string OwnedString;

        [XmlIgnore]
        public int? Owned
        {
            get
            {
                int result;
                if (int.TryParse(OwnedString, out result))
                {
                    return result;
                }
                return null;
            }
        }

        [XmlElement("trading")]
        public string TradingString;

        [XmlIgnore]
        public int? Trading
        {
            get
            {
                int result;
                if (int.TryParse(TradingString, out result))
                {
                    return result;
                }
                return null;
            }
        }

        [XmlElement("wanting")]
        public string WantingString;

        [XmlIgnore]
        public int? Wanting
        {
            get
            {
                int result;
                if (int.TryParse(WantingString, out result))
                {
                    return result;
                }
                return null;
            }
        }

        [XmlElement("wishing")]
        public string WishingString;

        [XmlIgnore]
        public int? Wishing
        {
            get
            {
                int result;
                if (int.TryParse(WishingString, out result))
                {
                    return result;
                }
                return null;
            }
        }

        [XmlElement("numcomments")]
        public string NumCommentsString;

        [XmlIgnore]
        public int? NumComments
        {
            get
            {
                int result;
                if (int.TryParse(NumCommentsString, out result))
                {
                    return result;
                }
                return null;
            }
        }

        [XmlElement("numweights")]
        public string NumWeightsString;

        [XmlIgnore]
        public int? NumWeights
        {
            get
            {
                int result;
                if (int.TryParse(NumWeightsString, out result))
                {
                    return result;
                }
                return null;
            }
        }

        [XmlElement("averageweight")]
        public string AverageWeightString;

        [XmlIgnore]
        public double? AverageWeight
        {
            get
            {
                double result;
                if (double.TryParse(AverageWeightString, out result))
                {
                    return result;
                }
                return null;
            }
        }
    }

    public class Ranks
    {
        [XmlIgnore]
        private Rank[] rankList;

        [XmlElement("rank")]
        public Rank[] RankList
        {
            get
            {
                return rankList ?? new Rank[0];
            }
            set
            {
                rankList = value;
            }
        }
    }

    public class Rank
    {
        public static readonly int BOARD_GAME_RANK_ID = 1;

        [XmlAttribute("type")]
        public string Type { get; set; }

        [XmlAttribute("id")]
        public int Id { get; set; }

        [XmlAttribute("friendlyname")]
        public string FriendlyName { get; set; }

        [XmlAttribute("value")]
        public string ValueString;

        private int? value = null;
        private bool hasValueBeenSet;
        [XmlIgnore]
        public int? Value
        {
            get
            {
                if (hasValueBeenSet)
                {
                    return this.value;
                }
                int result;
                if (int.TryParse(ValueString, out result))
                {
                    this.value = result;
                }
                this.hasValueBeenSet = true;
                return this.value;
            }
            set
            {
                this.value = value;
                this.hasValueBeenSet = true;
            }
        }

        [XmlAttribute("bayesaverage")]
        public string BayesAverageString;

        private double? bayesAverage = null;
        private bool hasBayesAverageBeenSet;
        [XmlIgnore]
        public double? BayesAverage
        {
            get
            {
                if (hasBayesAverageBeenSet)
                {
                    return this.bayesAverage;
                }
                double result;
                if (double.TryParse(BayesAverageString, out result))
                {
                    this.bayesAverage = result;
                }
                this.hasBayesAverageBeenSet = true;
                return this.bayesAverage;
            }
            set
            {
                this.bayesAverage = value;
                this.hasBayesAverageBeenSet = true;
            }
        }
    }
}
