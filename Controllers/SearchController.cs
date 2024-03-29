using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using ApiServer.Models;

namespace ApiServer.Controllers
{
	[EnableCors("DevelopPolicy")]
	[ApiController]
	[Route("search")]
	public class SearchController : ControllerBase
	{
		public SearchController(IOptions<SearchEngineApiSettings> apiSettingsAccessor, ICacheModule cache)
		{
			// init worker manager with dependencies injection.
			WorkerManager.Instance.Initialize(apiSettingsAccessor.Value, cache);
		}

		/// <summary>
		/// Get result of search by {keyword}
		/// </summary>
		/// <param name="keyword">search target</param>
		[HttpGet("{keyword}")]
		public async Task GetSearchResult(string keyword)
		{
			Response.Headers.Add("Connection", "keep-alive");
			Response.Headers.Add("Cache-Control", "no-cache");
			Response.Headers.Add("Content-Type", "text/event-stream");

			Console.WriteLine(
				$"{{{Request.HttpContext.Connection.Id}:{keyword}}} arrived."
				+ $" RequestAborted: {Response.HttpContext.RequestAborted.IsCancellationRequested}"
				+ $" {DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss")}");

			if (ResultManager.Instance.TryAddConnection(
				Request.HttpContext.Connection.Id, keyword, Response, out var connection))
			{
				Response.StatusCode = 200;
			}
			else
			{
				// Duplicated client secret error! Close connection and return.
				Response.StatusCode = 502;
			}

			try
			{
				while (!Response.HttpContext.RequestAborted.IsCancellationRequested && !connection.IsCancelled())
				{
					await Task.Delay(1000);
				}
			}
			finally
			{
				connection.Cancel();
				Response.Body.Close();

				Console.WriteLine(
					$"{{{Request.HttpContext.Connection.Id}:{keyword}}} closed. Status: {Response.StatusCode}");
			}
		}

		/// <summary>
		/// Block inappropriate associated keywords from parent keyword.
		/// </summary>
		[HttpPut("block")]
		public async Task BlockInappropriateKeywords(InappropriateKeyword keyword)
		{
			Console.WriteLine(
				$"{Request.HttpContext.Connection.Id} - block requested."
				+ $"{{search={keyword.SearchKeyword}, block={keyword.BlockKeyword}}}"
				+ $" {DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss")}");

			await WorkerManager.Instance.BlockAssociativeKeyword(keyword.SearchKeyword, keyword.BlockKeyword);

			Response.StatusCode = 200;
		}

		/// <summary>
		/// Remove cached result in redis cache.
		/// </summary>
		/// <param name="keyword">remove target</param>
		[HttpDelete("{keyword}")]
		public async Task RemoveCachedResult(string keyword)
		{
			Console.WriteLine(
				$"{{{Request.HttpContext.Connection.Id}:{keyword}}} remove requested."
				+ $" {DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss")}");

			await WorkerManager.Instance.RemoveCachedResult(keyword);

			Response.StatusCode = 200;
		}
	}
}
