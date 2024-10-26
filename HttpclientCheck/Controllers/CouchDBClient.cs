//using Microsoft.AspNetCore.Mvc;
//using CouchDB.Client;
//using System.Threading.Tasks;
//using Microsoft.Extensions.Logging;
//using RestSharp;

//namespace HttpclientCheck.Controllers
//{
//    [ApiController]
//    [Route("[controller]")]
//    public class CouchDBclientController : ControllerBase
//    {
//        private readonly ILogger<CouchDBclientController> _logger;
//        private readonly CouchClient _couchClient;

//        public CouchDBclientController(ILogger<CouchDBclientController> logger)
//        {
//            _logger = logger;
//            _couchClient = new CouchClient("http://admin:admin@localhost:5984/");
//        }

//        [HttpGet("{id}")]
//        public async Task<ActionResult> Get(string id)
//        {
//            try
//            {
//                var database = _couchClient.GetDatabaseAsync("laptop-prices");
//                CouchDatabase DB = database.Result;
//                var document = DB.GetAsync(id);

//                if (document == null)
//                {
//                    return NotFound();
//                }

//                return Ok(document);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error fetching document.");
//                return StatusCode(500, "Internal server error");
//            }
//        }
//    }
//}
