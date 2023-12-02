using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HelloWorld.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ThirdPartyAccessController : ControllerBase
    {

        private string api_key = "qqvu5wi56wwyifvnqisqoyx34j2nmcxy7c3bl265kthy2aztqfva";  //This is a fake API key for demonstration purposes

        //Simulated sensitive data (AWS credentials)
        private string aws_access_key_id = "AKIAIOSFODNN7EXAMPLE";
        private string aws_secret_access_key = "wJalrXUtnFEMI/K7MDENG/bPxRfiCYEXAMPLEKEY";


        [HttpGet("DoSomething")]
        public IActionResult GetSecret()
        {
            var myConnectionString = $"https://myservice?apikey={api_key}";
            return Ok("Used some secrets for fake purposes");
        }
        
    }
}
