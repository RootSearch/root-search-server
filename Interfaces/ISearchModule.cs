using System.Collections.Generic;
using ApiServer.Models;

namespace ApiServer
{
	public enum SearchEngineTypes
	{
		/// <summary>
		/// 구글 커스텀 서치 api
		/// <see href="https://developers.google.com/custom-search/v1/overview">Custom Search JSON API v1</see>
		/// </summary>
		GoogleCustomJson,
	}

	public interface ISearchModule
	{
		SearchEngineTypes SearchEngingType { get; }

		bool IsAvailable { get; set; }

		bool Initialize(SearchEngineApiSettings apiSettings);

		List<SearchResultObject> Search(string keyword);
	}
}
