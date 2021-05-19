using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Microsoft.Extensions.Primitives;
using Microsoft.Extensions.Primitive;
using System.Linq;
using ApiServer.Models;

namespace ApiServer
{
	public class WorkerManager
	{
		private static WorkerManager _instance = new WorkerManager();

		public static WorkerManager Instance => _instance;

		/// <summary>
		/// Api settings for search modules
		/// </summary>
		private SearchEngineApiSettings _apiSettings;

		/// <summary>
		/// redis distributed cache for search results
		/// </summary>
		private ICacheModule _cache;

		/// <summary>
		/// List of available search modules
		/// </summary>
		private List<ISearchModule> _searchModules = new List<ISearchModule>();

		private static char[] Separators = new char[] { ' ' };

		public void Initialize(SearchEngineApiSettings apiSettings, ICacheModule cache)
		{
			// inject IOptions
			_apiSettings = apiSettings;

			// inject cache
			_cache = cache;

			TryAddSearchModule(GoogleCustomSearchModule.Instance);
		}

		/// <summary>
		/// Try to activate search module and add to available search module list.
		/// </summary>
		/// <param name="searchModule">target search module</param>
		/// <returns>true if successfully added</returns>
		private bool TryAddSearchModule(ISearchModule searchModule)
		{
			// error: already added
			if (_searchModules.Contains(searchModule))
			{
				return false;
			}

			// error: couldn't initialize
			if (!searchModule.Initialize(_apiSettings))
			{
				return false;
			}

			_searchModules.Add(searchModule);

			return true;
		}

		private ISearchModule DecideSearchModule()
		{
			if (_searchModules == null || _searchModules.Count == 0)
			{
				return null;
			}

			for (int i = _searchModules.Count - 1; i >= 0; --i)
			{
				if (!_searchModules[i].IsAvailable)
				{
					continue;
				}

				return _searchModules[i];
			}

			return null;
		}

		public async Task<SearchResultCache> GetResult(string keyword)
		{
			SearchResultCache cached = await _cache.GetAsync(keyword);

			// 검색 결과 캐시가 없다면
			if (cached == null)
			{
				cached = new SearchResultCache();

				// TODO: handling for not found available module
				cached.Results = DecideSearchModule()?.Search(keyword);

				var scores = new ConcurrentDictionary<string, uint>();

				// TODO: use worker
				foreach (var result in cached.Results)
				{
					if (result.Snippet == null)
					{
						continue;
					}

					var tokens = new StringTokenizer(result.Snippet, Separators);

					tokens.Score(ref scores);
				}

				var sorted = scores.OrderByDescending(item => item.Value);

				cached.AssociativeWords = sorted.Take(Constants.MaxAssociativeWords).Select(x => x.Key).ToList();

				// TODO: search failed handling
				await _cache.SetAsync(keyword, cached);
			}

			return cached;
		}

		public async Task RemoveCachedResult(string keyword)
		{
			await _cache.RemoveAsync(keyword);
		}
	}
}
