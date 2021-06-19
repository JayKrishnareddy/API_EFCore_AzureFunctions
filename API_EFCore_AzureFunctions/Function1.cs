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
        private const string Route = "func";
        private readonly AppDbContext _appDbContext;
        #endregion

        #region Constructor
        public Function1(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }
        #endregion
        
        #region Function Get Employees
        /// <summary>
        /// Get List of Employees
        /// </summary>
        /// <param name="req"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        [FunctionName("GetAllEmployees")]
        public async Task<IActionResult> GetAllEmployees(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)]
          HttpRequest req, ILogger log) 
        {
            try
            {
                log.LogInformation("Getting Employee list items");
                return new OkObjectResult(await _appDbContext.Employee.ToListAsync());
            }
            catch (System.Exception)
            {
                throw;
            }
            
        }
        #endregion

        #region Get Employee Based on Employee Id
        /// <summary>
        /// Get Employee by Querying with Id
        /// </summary>
        /// <param name="req"></param>
        /// <param name="log"></param>
        /// <param name="Id"></param>
        /// <returns></returns>
        [FunctionName("GetEmployeebyId")]
        public async Task<IActionResult> GetEmployeebyId(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "{Id}")]
          HttpRequest req, ILogger log, int Id)
        {
            try
            {
                var result = await _appDbContext.Employee.FindAsync(Id);
                if (result is null)
                {
                    log.LogInformation($"Item {Id} not found");
                    return new NotFoundResult();
                }
                return new OkObjectResult(result);
            }
            catch (System.Exception)
            {
                throw;
            }
        }
        #endregion

        #region Create Employee
        /// <summary>
        /// Create New Employee
        /// </summary>
        /// <param name="req"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        [FunctionName("CreateEmployee")]
        public async Task<IActionResult> CreateEmployee(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = Route +"/Create")]
          HttpRequest req, ILogger log)
        {
            log.LogInformation("Creating a new employee list item");
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var input = JsonConvert.DeserializeObject<Employee>(requestBody);
            var employee = new Employee { Name = input.Name, Designation = input.Designation,City= input.City };
            await _appDbContext.Employee.AddAsync(employee);
            await _appDbContext.SaveChangesAsync();
            return new OkObjectResult(new { Message = "Record Saved SuccessFully", Data = employee });
        }
        #endregion

        #region Update Employee
      /// <summary>
      /// Update Employee - Changes
      /// </summary>
      /// <param name="req"></param>
      /// <param name="log"></param>
      /// <param name="Id"></param>
      /// <returns></returns>
        [FunctionName("UpdateEmployee")]
        public async Task<IActionResult> UpdateEmployee(
        [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = Route +"/Update")]
          HttpRequest req, ILogger log)
        {
            log.LogInformation("Updating a new employee list item");
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var updated = JsonConvert.DeserializeObject<Employee>(requestBody);
            var employee = await _appDbContext.Employee.FindAsync(updated.Id);
            if(employee is null)
            {
                log.LogError($"Item {updated.Id} not found");
                return new NotFoundResult();
            }
            if(!string.IsNullOrEmpty(updated.Name) && !string.IsNullOrEmpty(updated.Designation))
            {
                employee.Name = updated.Name; employee.Designation = updated.Designation;
                employee.City = updated.City;
            }
            _appDbContext.Employee.Update(employee);
            await _appDbContext.SaveChangesAsync();
            return new OkObjectResult(new { Message = "Record Updated SuccessFully", Data = employee });
        }
        #endregion

        #region Delete Employee
        /// <summary>
        /// Deletion of Employee
        /// </summary>
        /// <param name="req"></param>
        /// <param name="log"></param>
        /// <param name="Id"></param>
        /// <returns></returns>
        [FunctionName("DeleteEmployee")]
        public async Task<IActionResult> DeleteEmployee(
        [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "DeleteEmployee/{Id}")]
          HttpRequest req, ILogger log,int Id)
        {
            log.LogInformation("Updating a new employee list item");
            var employee = await _appDbContext.Employee.FindAsync(Id);
            if (employee is null)
            {
                log.LogError($"Item {Id} not found");
                return new NotFoundResult();
            }
            _appDbContext.Employee.Remove(employee);
            await _appDbContext.SaveChangesAsync();
            return new OkObjectResult("Record Deleted !");
        }
        #endregion

    }
}
