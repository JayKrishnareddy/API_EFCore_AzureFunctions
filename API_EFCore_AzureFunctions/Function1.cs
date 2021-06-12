using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using API_EFCore_AzureFunctions.Models;
using Microsoft.EntityFrameworkCore;

namespace API_EFCore_AzureFunctions
{
    public class Function1
    {
        #region Property
        public const string Route = "api";
        private readonly AppDbContext _appDbContext;
        #endregion

        #region Constructor
        public Function1(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }
        #endregion

        
        [FunctionName("Get")]
        public async Task<IActionResult> Get(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string name = req.Query["name"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data?.name;

            string responseMessage = string.IsNullOrEmpty(name)
                ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
                : $"Hello, {name}. This HTTP triggered function executed successfully.";

            return new OkObjectResult(responseMessage);
        }
        #region Function Get Employees
        /// <summary>
        /// Get List of Employees
        /// </summary>
        /// <param name="req"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        [FunctionName("GetEmployees")]
        public async Task<IActionResult> GetEmployees(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = Route)]
          HttpRequest req, ILogger log)
        {
            log.LogInformation("Getting Employee list items");
            return new OkObjectResult(await _appDbContext.Employees.ToListAsync());
        }
        #endregion

    }
}
