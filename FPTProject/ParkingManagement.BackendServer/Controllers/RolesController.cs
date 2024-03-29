﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ParkingManagement.BackendServer.Authorization;
using ParkingManagement.BackendServer.Constants;
using ParkingManagement.BackendServer.Data;
using ParkingManagement.BackendServer.Data.Entities;
using ParkingManagement.BackendServer.Helpers;
using ParkingManagement.ViewModels.Commons;
using ParkingManagement.ViewModels.Systems;
using ParkingManagement.ViewModels.Systems.RequestModels;
using ParkingManagement.ViewModels.Systems.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ParkingManagement.BackendServer.Controllers
{
    public class RolesController : BaseController
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;

        public RolesController(RoleManager<IdentityRole> roleManager, ApplicationDbContext context)
        {
            _roleManager = roleManager;
            _context = context;
        }
        #region Crud Role
        /// <summary>
        /// Get All Role 
        /// URL: GET: http://localhost:5001/api/roles/ 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ClaimRequirement(FunctionCode.SYSTEM_ROLE, CommandCode.VIEW)]
        public async Task<IActionResult> GetRoles()
        {
            var roles = await _roleManager.Roles.Select(x=> new RoleVm()
            {
                Id = x.Id,
                Name = x.Name,
            }).ToListAsync();
            return Ok(roles);
        }
        /// <summary>
        /// Get Roles Paging
        /// URL: GET: http://localhost:5001/api/roles/?filter={filter}&pageIndex=1&pageSize=10
        /// </summary>
        /// <param name="filter">filter</param>
        /// <param name="pageIndex">pageIndex</param>
        /// <param name="pageSize">pageSize</param>
        /// <returns></returns>
        [HttpGet("filter")]
        [ClaimRequirement(FunctionCode.SYSTEM_ROLE, CommandCode.VIEW)]
        public async Task<IActionResult> GetRoles(string filter, int pageIndex,int pageSize)
        {
            var query = _roleManager.Roles;
            if (!string.IsNullOrEmpty(filter))
            {
                //Find records that satisfy the filter condition
                query = query.Where(x=>x.Id.Contains(filter)||x.Name.Contains(filter));
            }
            var totalRecords = await query.CountAsync();
            //Retrieve records by page
            var items = await query.Skip((pageIndex - 1 * pageSize))
                .Take(pageSize)
                .Select(r => new RoleVm()
                {
                    Id = r.Id,
                    Name = r.Name
                })
                .ToListAsync();
            var pagination = new Pagination<RoleVm>
            {
                Items = items,
                TotalRecords = totalRecords,
            };
            return Ok(pagination);
        }
        /// <summary>
        /// Get role by id
        /// </summary>
        /// <param name="id">id</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ClaimRequirement(FunctionCode.SYSTEM_ROLE, CommandCode.VIEW)]
        public async Task<IActionResult> GetById(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
                return NotFound();

            var roleVm = new RoleVm()
            {
                Id = role.Id,
                Name = role.Name,
            };
            return Ok(roleVm);
        }
        /// <summary>
        /// Create new Role
        /// URL: POST: http://localhost:5001/api/roles
        /// </summary>
        /// <param name="roleVm">roleVm</param>
        /// <returns></returns>
        [HttpPost]
        [ClaimRequirement(FunctionCode.SYSTEM_ROLE, CommandCode.CREATE)]
        [ApiValidationFilter]
        public async Task<IActionResult> PostRoles(RoleCreateRequest request )
        {
            var role = new IdentityRole()
            {
                Id = request.Id,
                Name = request.Name,
                NormalizedName = request.Name.ToUpper()
            };
            var result = await _roleManager.CreateAsync(role);
            if (result.Succeeded)
            {
                return CreatedAtAction(nameof(GetById), new { id = role.Id }, request);
            }
            else
            {
                return BadRequest(result.Errors);
            }
        }
        /// <summary>
        /// Edit Role
        /// URL: PUT: http://localhost:5001/api/roles/{id}
        /// </summary>
        /// <param name="id">id</param>
        /// <param name="roleVm">roleVM</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [ClaimRequirement(FunctionCode.SYSTEM_ROLE, CommandCode.UPDATE)]
        public async Task<IActionResult> PutRole(string id, RoleCreateRequest request)
        {
            if (id != request.Id)
                return BadRequest();

            var role = await _roleManager.FindByIdAsync(id);
            //check if role exists
            if (role == null)
                return NotFound();
            //if exists reassign information
            role.Name = request.Name;
            role.NormalizedName = request.Name.ToUpper();
            
            var result = await _roleManager.UpdateAsync(role);

            if (result.Succeeded)
            {
                return NoContent();
            }
            return BadRequest(result.Errors);
        }
        /// <summary>
        /// Delete Role by id
        /// URL: DELETE: http://localhost:5001/api/roles/{id}
        /// </summary>
        /// <param name="id">id</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [ClaimRequirement(FunctionCode.SYSTEM_ROLE, CommandCode.DELETE)]
        public async Task<IActionResult> DeleteRole(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
                return NotFound();

            var result = await _roleManager.DeleteAsync(role);

            if (result.Succeeded)
            {
                var rolevm = new RoleVm()
                {
                    Id = role.Id,
                    Name = role.Name
                };
                return Ok(rolevm);
            }
            return BadRequest(result.Errors);
        }
        #endregion
        #region Permissions
        [HttpGet("{roleId}/permissions")]
        [ClaimRequirement(FunctionCode.SYSTEM_PERMISSION, CommandCode.VIEW)]
        public async Task<IActionResult> GetPermissionsByRoleId(string roleId)
        {
            var permissions = from p in _context.Permissions
                              join c in _context.Commands on p.CommandId equals c.Id
                              where p.RoleId == roleId
                              select new PermissionVm()
                              {
                                  FunctionId = p.FunctionId,
                                  CommandId = p.CommandId,
                                  RoleId = p.RoleId
                              };
            return Ok(await permissions.ToListAsync());
        }
        [HttpPut("{roleId}/permissions")]
        [ClaimRequirement(FunctionCode.SYSTEM_PERMISSION, CommandCode.UPDATE)]
        public async Task<IActionResult> PutPermissionByRoleId(string roleId,[FromBody] UpdatePermissionRequest request)
        {
            var newPermissions = new List<Permission>();
            foreach (var p in request.Permissions)
            {
                newPermissions.Add(new Permission(p.FunctionId, roleId, p.CommandId));
            }
            var existingPermissions = _context.Permissions.Where(x => x.RoleId == roleId);
            _context.Permissions.RemoveRange(existingPermissions);
            _context.Permissions.AddRange(newPermissions);
            var result = await _context.SaveChangesAsync();
            if (result > 0)
            {
                return NoContent();
            }
            return BadRequest();
        }
        #endregion
    }
}
