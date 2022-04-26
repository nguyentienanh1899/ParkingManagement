using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ParkingManagement.BackendServer.Data;
using ParkingManagement.ViewModels.Systems.ViewModels;
using System.Linq;
using System.Threading.Tasks;

namespace ParkingManagement.BackendServer.Controllers
{
    public class CommandsController : BaseController
    {
        private readonly ApplicationDbContext _context;
        public CommandsController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<IActionResult> GetCommands()
        {
            var listCommands = _context.Commands;
            var listCommandsVm = await listCommands.Select(c => new CommandVm()
            {
                Id = c.Id,
                Name = c.Name
            }).ToListAsync();
            return Ok(listCommandsVm);
        }
    }
}
