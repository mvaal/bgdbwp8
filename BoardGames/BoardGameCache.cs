using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoardGames
{
    public sealed class BoardGameCache
    {
        private static volatile BoardGameCache instance;
        private static object syncRoot = new Object();

        private BoardGameCache()
        {
            Cache = new Dictionary<int, BoardGame>();
        }

        public static BoardGameCache Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                        {
                            instance = new BoardGameCache();
                        }
                    }
                }

                return instance;
            }
        }

        public IDictionary<int, BoardGame> Cache { get; set; }

        public async Task<IEnumerable<BoardGame>> BoardGamesFromName(string boardGameName)
        {
            return await BoardGamesFromIds(await BoardGameIds(boardGameName));
        }

        public async Task<IEnumerable<int>> BoardGameIds(string boardGameName)
        {
            Debug.WriteLine("Search for " + boardGameName);
            BGGParser parser = new BGGParser();
            return await parser.GetBoardGameIds(boardGameName);
        }

        public async Task<IEnumerable<BoardGame>> BoardGamesFromIds(params int[] boardGameIds)
        {
            return await BoardGamesFromIds(boardGameIds.AsEnumerable());
        }

        public async Task<IEnumerable<BoardGame>> BoardGamesFromIds(IEnumerable<int> boardGameIds)
        {
            if (!boardGameIds.Any())
            {
                return Enumerable.Empty<BoardGame>();
            }
            Debug.WriteLine("Searching for ids " + string.Join(",", boardGameIds));

            var nonCachedIds = boardGameIds.Where(id => !Cache.ContainsKey(id)).Select(id => id);

            //BGGParser parser = new BGGParser();

            //nonCachedIds.ForEachBatch(100, async ids => await AddBoardGamesToCache(ids));
            await ForEachBatch(nonCachedIds, 100);
            //await AddBoardGamesToCache(nonCachedIds);

            var boardGames = boardGameIds.Select(id => Cache[id]);
            boardGames = boardGames.OrderBy(bg => !bg.Rank.HasValue).ThenBy(bg => bg.Rank);
            return boardGames;
        }

        private async Task ForEachBatch(IEnumerable<int> boardGameIds, int batchSize)
        {
            while (boardGameIds.Count() > 0)
            {
                int batch = batchSize;
                if (boardGameIds.Count() < batchSize)
                {
                    batch = boardGameIds.Count();
                }

                var batchIds = boardGameIds.Take(batch);

                Debug.WriteLine("Batch search for ids " + string.Join(",", batchIds));

                await AddBoardGamesToCache(batchIds);
            }
        }

        private async Task AddBoardGamesToCache(IEnumerable<int> ids)
        {
            var count = ids.Count();
            BGGParser parser = new BGGParser();
            var bgs = await parser.GetBoardGames(ids);
            lock (syncRoot)
            {
                foreach (BoardGame bg in bgs)
                {
                    if (!IsCached(bg))
                    {
                        Cache.Add(bg.ObjectId, bg);
                    }
                }
            }
        }

        public bool IsCached(BoardGame boardGame)
        {
            return IsCached(boardGame.ObjectId);
        }

        public bool IsCached(string objectIdString)
        {
            int objectId;
            if (int.TryParse(objectIdString, out objectId))
            {
                return IsCached(objectId);
            }
            return false;
        }

        public bool IsCached(int objectId)
        {
            return Cache.ContainsKey(objectId);
        }
    }
}
