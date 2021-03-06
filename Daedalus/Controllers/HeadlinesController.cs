﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NewsAPI;
using NewsAPI.Models;
using NewsAPI.Constants;
using Microsoft.Extensions.Configuration;

namespace Daedalus.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HeadlinesController : ControllerBase
    {
        private readonly ILogger<HeadlinesController> _logger;
        private readonly NewsApiClient _newsApiClient;
        private readonly IConfiguration _config;

        public HeadlinesController(ILogger<HeadlinesController> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
            _newsApiClient = new NewsApiClient(config.GetValue<string>("AppSettings:NewsApiKey"));
        }

        // Uses /api/headlines/some-search-query
        // "?" in the routing means it's optional
        [HttpGet("{searchQuery?}")]
        public async Task<ActionResult<ArticlesResult>> HeadLines(string searchQuery)
        {
            var headlineRequest = new TopHeadlinesRequest
            {
                Country = Countries.US,
                Language = Languages.EN
            };

            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                headlineRequest.Q = searchQuery;
            }

            var articlesResponse = await _newsApiClient.GetTopHeadlinesAsync(headlineRequest);
            if (articlesResponse.Status.Equals(Statuses.Error))
            {
                _logger.LogError(articlesResponse.Error.Message);
            }

            return articlesResponse; // ActionResult return type converts object to JSON automatically.
        }
    }
}
