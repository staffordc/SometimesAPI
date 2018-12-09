using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Http;

namespace sometimes_api.Controllers
{
    [Route("v1/api/requests")]
    public class RequestsController : Controller
    {
        private readonly IHttpClientFactory _clientFactory;

        private int[] FauxStatus = new[] { 0, 400, 404, 500 };
        private string[] FauxResponses = new[] { null, "null", "", "{}", "[]" };
        Random Randomizer = new Random();

        public RequestsController(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }
        
        [HttpGet]
        public async Task<string> Get([FromQuery] string url)
        {
            if (url == null)
            {
                Response.StatusCode = 400;
                return "pwease pwodive a quuewwy pawamiter: url";
            }
            if (!Uri.IsWellFormedUriString(url, UriKind.RelativeOrAbsolute))
            {
                Response.StatusCode = 400;
                return $"please provide a url that works {url} did not work";
            }

            if (Randomizer.Next(1, 3) == 2)
            {
                int index = Randomizer.Next(FauxResponses.Length);
                Response.StatusCode = FauxStatus[Randomizer.Next(FauxStatus.Length)];
                return FauxResponses[index];
            }

            var request = new HttpRequestMessage(HttpMethod.Get, url);
            var client = _clientFactory.CreateClient();
            var response = await client.SendAsync(request);

            var ResponseBody = await response.Content.ReadAsStringAsync();
            var status = response.StatusCode;
            Response.StatusCode = (int)status;
            return ResponseBody;
        }
    }
}
