using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using ParkingManagement.ViewModels.Systems.ViewModels;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace ParkingManagement.BackendServer.Controllers
{
    public class PermisssionController : BaseController
    {
        private readonly IConfiguration _configuration;
        public PermisssionController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        /// <summary>
        /// Show list function with corressponding action included in each functions
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetCommandsView()
        {
            using (SqlConnection defaultConnection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                if (defaultConnection.State == ConnectionState.Closed)
                {
                    await defaultConnection.OpenAsync();
                }
                var sql = @"SELECT f.Id,f.Name,f.ParentId,
                                sum(when case c.Id = 'CREATE' then 1 else 0 end) as HasCreate,
                                sum(when case c.Id = 'UPDATE' then 1 else 0 end) as HasUpdate,
                                sum(when case c.Id = 'VIEW' then 1 else 0 end) as HasView,
                                sum(when case c.Id = 'DELETE' then 1 else 0 end) as HasDelete,
                                sum(when case c.Id = 'APPROVE' then 1 else 0 end) as HasApprove
                            from Functions f join CommandInFunctions cif on f.Id = cif.FunctionId
                                            left join Commands c on cif.CommandId = c.Id
                            GROUP BY f.Id,f.Name, f.ParentId
                            order BY f.ParentId";
                var data = await defaultConnection.QueryAsync<PermissionScreenVm>(sql, null, null, 120, CommandType.Text);
                return Ok(data.ToList());
            }
        }
    }
}
