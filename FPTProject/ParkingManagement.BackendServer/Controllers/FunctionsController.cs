using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ParkingManagement.BackendServer.Authorization;
using ParkingManagement.BackendServer.Constants;
using ParkingManagement.BackendServer.Data;
using ParkingManagement.BackendServer.Data.Entities;
using ParkingManagement.BackendServer.Helpers;
using ParkingManagement.ViewModels.Commons;
using ParkingManagement.ViewModels.Systems.RequestModels;
using ParkingManagement.ViewModels.Systems.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ParkingManagement.BackendServer.Controllers
{
    public class FunctionsController : BaseController
    {
        private readonly ApplicationDbContext _context;
        public FunctionsController(ApplicationDbContext context)
        {
            _context = context;
        }
        #region CRUD with Function
        [HttpPost]
        [ClaimRequirement(FunctionCode.SYSTEM_FUNCTION, CommandCode.CREATE)]
        [ApiValidationFilter]
        public async Task<IActionResult> PostFuntion([FromBody] FunctionCreateRequest reuqest)
        {
            var dbFunction = await _context.Functions.FindAsync(reuqest.Id);
            if (dbFunction != null)
            {
                return BadRequest(new ApiBadRequestResponse($"Function with ID:{reuqest.Id} is existed."));
            }
            var function = new Function()
            {
                Id = reuqest.Id,
                Name = reuqest.Name,
                Url = reuqest.Url,
                SortOrder = reuqest.SortOrder,
                ParentId = reuqest.ParentId
            };
            await _context.Functions.AddAsync(function);
            var result = await _context.SaveChangesAsync();
            if (result > 0)
            {
                return CreatedAtAction(nameof(GetById), new { id = function.Id }, reuqest);
            }
            else
            {
                return BadRequest(new ApiBadRequestResponse("Create new function failed "));
            }
        }

        [HttpDelete("{id}")]
        [ClaimRequirement(FunctionCode.SYSTEM_FUNCTION, CommandCode.DELETE)]
        public async Task<IActionResult> DeleteFunction(string id)
        {
            var dbFunction = await _context.Functions.FindAsync(id);
            if(dbFunction == null)
            {
                return NotFound();
            }
            _context.Functions.Remove(dbFunction);
            var result = await _context.SaveChangesAsync();
            if(result > 0)
            {
                var functionVM = new FunctionVm()
                {
                    Id = dbFunction.Id,
                    Name=dbFunction.Name,
                    Url=dbFunction.Url,
                    SortOrder=dbFunction.SortOrder,
                    ParentId=dbFunction.ParentId
                };
                return Ok(functionVM);

            }
            else
            {
                return BadRequest(new ApiBadRequestResponse($"Delete function with id:{dbFunction.Id} failed "));
            }
        }

        [HttpPut("{id}")]
        [ClaimRequirement(FunctionCode.SYSTEM_FUNCTION, CommandCode.UPDATE)]
        public async Task<IActionResult> PutFunction(string id,[FromBody] FunctionCreateRequest request)
        {
            var dbFunction = await _context.Functions.FindAsync(id);
            if(dbFunction == null)
            {
                return NotFound();
            }

            dbFunction.Name = request.Name;
            dbFunction.Url = request.Url;
            dbFunction.SortOrder = request.SortOrder;
            dbFunction.ParentId = request.ParentId;

            _context.Functions.Update(dbFunction);
            var result = await _context.SaveChangesAsync();
            if (result > 0)
            {
                return NoContent();
            }
            return BadRequest(new ApiBadRequestResponse($"Update function with id:{dbFunction.Id} failed "));
        }

        [HttpGet]
        [ClaimRequirement(FunctionCode.SYSTEM_FUNCTION, CommandCode.VIEW)]
        public async Task<IActionResult> GetFunctions()
        {
            var functions = _context.Functions;

            var functionvms = await functions.Select(u => new FunctionVm()
            {
                Id = u.Id,
                Name = u.Name,
                Url = u.Url,
                SortOrder = u.SortOrder,
                ParentId = u.ParentId
            }).ToListAsync();

            return Ok(functionvms);
        }

        [HttpGet("filter")]
        [ClaimRequirement(FunctionCode.SYSTEM_FUNCTION, CommandCode.VIEW)]
        public async Task<IActionResult> GetFunctionsPaging(string filter, int pageIndex, int pageSize)
        {
            var query = _context.Functions.AsQueryable();
            if (!string.IsNullOrEmpty(filter))
            {
                query = query.Where(x => x.Name.Contains(filter)
                || x.Id.Contains(filter)
                || x.Url.Contains(filter));
            }
            var totalRecords = await query.CountAsync();
            var items = await query.Skip((pageIndex - 1 * pageSize))
                .Take(pageSize)
                .Select(u => new FunctionVm()
                {
                    Id = u.Id,
                    Name = u.Name,
                    Url = u.Url,
                    SortOrder = u.SortOrder,
                    ParentId = u.ParentId
                })
                .ToListAsync();

            var pagination = new Pagination<FunctionVm>
            {
                Items = items,
                TotalRecords = totalRecords,
            };
            return Ok(pagination);
        }

        [HttpGet("{id}")]
        [ClaimRequirement(FunctionCode.SYSTEM_FUNCTION, CommandCode.VIEW)]
        public async Task<IActionResult> GetById(string id)
        {
            var function = await _context.Functions.FindAsync(id);
            if (function == null)
                return NotFound();

            var functionVm = new FunctionVm()
            {
                Id = function.Id,
                Name = function.Name,
                Url = function.Url,
                SortOrder = function.SortOrder,
                ParentId = function.ParentId
            };
            return Ok(functionVm);
        }
        #endregion
        #region API for Command and Function
        [HttpGet("{functionId}/commands")]
        [ClaimRequirement(FunctionCode.SYSTEM_FUNCTION, CommandCode.VIEW)]
        public async Task<IActionResult> GetCommandsInFunctions(string functionId)
        {
            var query = from a in _context.Commands
                        join cif in _context.CommandInFunctions on a.Id equals cif.CommandId into result1
                        from commandInFunction in result1.DefaultIfEmpty()
                        join f in _context.Functions on commandInFunction.FunctionId equals f.Id into result2
                        from function in result2.DefaultIfEmpty()
                        select new
                        {
                            a.Id,
                            a.Name,
                            commandInFunction.FunctionId
                        };
            query = query.Where(x=>x.FunctionId == functionId);
            var data = await query.Select(x => new CommandVm()
            {
                Id = x.Id,
                Name = x.Name,
            }).ToListAsync(); 
            return Ok(data);
        }
        [HttpGet("{functionId}/commands/not-in-function")]
        [ClaimRequirement(FunctionCode.SYSTEM_FUNCTION, CommandCode.VIEW)]
        public async Task<IActionResult> GetCommantsNotInFunction(string functionId)
        {
            var query = from a in _context.Commands
                        join cif in _context.CommandInFunctions on a.Id equals cif.CommandId into result1
                        from commandInFunction in result1.DefaultIfEmpty()
                        join f in _context.Functions on commandInFunction.FunctionId equals f.Id into result2
                        from function in result2.DefaultIfEmpty()
                        select new
                        {
                            a.Id,
                            a.Name,
                            commandInFunction.FunctionId
                        };
            query = query.Where(x=>x.FunctionId!=functionId);
            var data = await query.Select(x => new CommandVm()
            {
                Id = x.Id,
                Name = x.Name,
            }).ToListAsync();
            return Ok(data);
        }
        [HttpPost("{functionId}/commands")]
        [ClaimRequirement(FunctionCode.SYSTEM_FUNCTION, CommandCode.CREATE)]
        [ApiValidationFilter]
        public async Task<IActionResult> PostCommandToFunction(string functionId,[FromBody] AddCommandToFunctionRequest request)
        {
            var commandInFunction = await _context.CommandInFunctions.FindAsync(request.CommandId,request.FunctionId);
            if(commandInFunction != null)
            {
                return BadRequest(new ApiBadRequestResponse($"This command has been added to function"));
            }
            var newCommandInFunction = new CommandInFunction()
            {
                CommandId = request.CommandId,
                FunctionId = request.FunctionId
            };
            _context.CommandInFunctions.Add(newCommandInFunction);
            var result = await _context.SaveChangesAsync();
            if (result > 0)
            {
                return CreatedAtAction(nameof(GetById), new { commandId = request.CommandId, functionId = request.FunctionId },request);
            }
            else
            {
                return BadRequest(new ApiBadRequestResponse($"Create command failed"));
            }
        }
        [HttpDelete("{functionId}/commands/{commandId}")]
        [ClaimRequirement(FunctionCode.SYSTEM_FUNCTION, CommandCode.DELETE)]
        public async Task<IActionResult> DeleteCommandToFunction(string functionId, string commandId)
        {
            var commandInFunction = await _context.CommandInFunctions.FindAsync(functionId, commandId);
            if (commandInFunction == null)
                return BadRequest(new ApiBadRequestResponse($"This command is not existed in function"));

            var oldCommandInFunction = new CommandInFunction()
            {
                CommandId = commandId,
                FunctionId = functionId
            };
            _context.CommandInFunctions.Remove(oldCommandInFunction);
            var result = await _context.SaveChangesAsync();

            if (result > 0)
            {
                return Ok();
            }
            else
            {
                return BadRequest(new ApiBadRequestResponse($"Delete failed"));
            }
        }
        #endregion

    }
}
