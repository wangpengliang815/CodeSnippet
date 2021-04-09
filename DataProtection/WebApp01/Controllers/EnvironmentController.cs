using System.Collections.Generic;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

using Newtonsoft.Json;

using WebApp01;

namespace DataProtectionTests.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EnvironmentController : ControllerBase
    {
        private readonly IOptions<TestOption> testOption;

        public EnvironmentController(IOptions<TestOption> testOption)
        {
            this.testOption = testOption;
        }

        [HttpGet]
        public IActionResult GetEnvironmentVariables()
        {
            Dictionary<string, string> dicts = new Dictionary<string, string>();
            ConfigurationBuilder builder = new ConfigurationBuilder();
            builder.AddJsonFile("appsettings.json");
            IConfigurationRoot configuration = builder.Build();

            dicts.Add("ProtectionOption.Thumbprint", configuration.GetSection("ProtectionOption:Thumbprint").Value);
            dicts.Add("ProtectionOption.ApplicationName", configuration.GetSection("ProtectionOption:ApplicationName").Value);
            dicts.Add("ProtectionOption.SecretKeyPath", configuration.GetSection("ProtectionOption:SecretKeyPath").Value);
            dicts.Add("ProtectionOption.Purpose", configuration.GetSection("ProtectionOption:Purpose").Value);
            dicts.Add("TestOption.Test1", testOption.Value.Test1);
            dicts.Add("TestOption.Test2", testOption.Value.Test2);
            return Ok(JsonConvert.SerializeObject(dicts));
        }
    }
}
