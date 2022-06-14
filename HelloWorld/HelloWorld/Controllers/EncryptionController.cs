using HelloWorld.Services;
using Microsoft.AspNetCore.Mvc;

namespace HelloWorld.Controllers
{
    
    public class EncryptionController : Controller
    {
        private IBadEncryptionService _badEncryptionService;

        public EncryptionController(IBadEncryptionService badEncryptionService)
        {
            _badEncryptionService = badEncryptionService;
        }

        [HttpGet("Encryption/EncryptString/{input}")]
        public IActionResult EncryptString(string input)
        {
            return Ok(_badEncryptionService.Encrypt(input));
        }
    }
}
