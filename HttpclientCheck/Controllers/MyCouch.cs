using Microsoft.AspNetCore.Mvc;
using MyCouch;
using MyCouch.Requests;
using MyCouch.Responses;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MyCouchCheck.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MyCouchClientController : ControllerBase
    {
        private readonly ILogger<MyCouchClientController> _logger;
        private readonly MyCouchClient _couchClient;

        public MyCouchClientController(ILogger<MyCouchClientController> logger)
        {
            _logger = logger;
            _couchClient = new MyCouchClient("http://admin:admin@localhost:5984/", "laptop-prices");
        }
        [HttpGet("find_doc")]
        public async Task<IActionResult> Get(string id)
        {

            var request = new QueryViewRequest("find_document", "by_id").Configure(q => q.Key(id));
            var response = await _couchClient.Views.QueryAsync(request);

            return Ok(response.Rows);
        }

        [HttpGet("range")]
        public async Task<IActionResult> GetDocumentsInRange(string startKey, string endKey)
        {
            var request = new QueryViewRequest("find_in_range", "by_price")
                .Configure(q => q.StartKey(startKey).EndKey(endKey));
            var response = await _couchClient.Views.QueryAsync(request);
            return Ok(response.Rows);
        }

        [HttpGet("reduce-sum")]
        public async Task<IActionResult> GetSum()
        {
            var request = new QueryViewRequest("aggregate_sum", "sum_price").Configure(q => q.Reduce(true).Group(true));
            var response = await _couchClient.Views.QueryAsync(request);
            return Ok(response.Rows);
        }

        [HttpGet("reduce-count")]
        public async Task<IActionResult> GetCount()
        {
            var request = new QueryViewRequest("aggregate_count", "count_by_company").Configure(q => q.Reduce(true).Group(true));
            var response = await _couchClient.Views.QueryAsync(request);
            return Ok(response.Rows);
        }

        [HttpGet("reduce-stats")]
        public async Task<IActionResult> GetStats()
        {
            var request = new QueryViewRequest("aggregate_stats", "stats_price").Configure(q => q.Reduce(true).Group(true));
            var response = await _couchClient.Views.QueryAsync(request);
            return Ok(response.Rows);
        }

        [HttpGet("custom-group")]
        public async Task<IActionResult> GetCustomGroupedData()
        {
            var request = new QueryViewRequest("custom_aggregation", "average_price_by_type")
                .Configure(q => q.Reduce(true).Group(true).GroupLevel(1));
            var response = await _couchClient.Views.QueryAsync(request);
            return Ok(response.Rows);
        }

        [HttpGet("Mango-Query")]
        public async Task<IActionResult> RunMangoQueryAsync(string field, string value)
        {
            try
            {
                var selector = new { selector = new Dictionary<string, string> { { field, value } } };
                var request = new FindRequest { SelectorExpression = JsonConvert.SerializeObject(selector) };
                var response = await _couchClient.Queries.FindAsync(request);

                if (!response.IsSuccess)
                {
                    return StatusCode((int)response.StatusCode, response.Reason);
                }

                var documents = response.Docs.Select(doc => JsonConvert.DeserializeObject<IDictionary<string, object>>(doc)).ToList();
                return Ok(documents);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching document.");
                return StatusCode(500, "Internal server error");

            }
        }
    }
}
