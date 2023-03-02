using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ParkingManagement.BackendServer.Authorization;
using ParkingManagement.BackendServer.Constants;
using ParkingManagement.BackendServer.Data;
using ParkingManagement.BackendServer.Data.Entities;
using ParkingManagement.BackendServer.Helpers;
using ParkingManagement.BackendServer.Services;
using ParkingManagement.ViewModels.Commons;
using ParkingManagement.ViewModels.Contents.RequestModels;
using ParkingManagement.ViewModels.Contents.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ParkingManagement.BackendServer.Controllers
{
    public class CategoriesController : BaseController
    {
        private readonly ApplicationDbContext _context;
        private readonly ICacheService _cacheService;
        public CategoriesController(ApplicationDbContext context, ICacheService cacheService)
        {
            _context = context;
            _cacheService = cacheService;
        }
        [HttpPost]
        [ClaimRequirement(FunctionCode.MANAGEMENT_CATEGORY, CommandCode.CREATE)]
        [ApiValidationFilter]
        public async Task<IActionResult> PostCategory([FromBody] CategoryCreateRequest request)
        {
            var category = new Category()
            {
                Name = request.Name,
                ParentId = request.ParentId,
                SortOrder = request.SortOrder,
                SeoAlias = request.SeoAlias,
                SeoDescription = request.SeoDescription
            };
            _context.Categories.Add(category);
            var result = await _context.SaveChangesAsync();

            if (result > 0)
            {
                return CreatedAtAction(nameof(GetById), new { id = category.Id }, request);
            }
            else
            {
                return BadRequest(new ApiBadRequestResponse("Create category is failed"));
            }
        }

        [HttpGet]
        [ClaimRequirement(FunctionCode.MANAGEMENT_CATEGORY, CommandCode.VIEW)]
        public async Task<IActionResult> GetCategories()
        {
            /* var categorys = await _context.Categories.ToListAsync();

             var categoryvms = categorys.Select(c => CreateCategoryVm(c)).ToList();

             return Ok(categoryvms);*/
            var cachedData = await _cacheService.GetAsync<List<CategoryVm>>(CacheConstants.Categories);
            if (cachedData == null)
            {
                var categorys = await _context.Categories.ToListAsync();

                var categoryVms = categorys.Select(c => CreateCategoryVm(c)).ToList();

                await _cacheService.SetAsync(CacheConstants.Categories, categoryVms,3);

                cachedData = categoryVms;
            }

            return Ok(cachedData);
        }

        [HttpGet("filter")]
        [ClaimRequirement(FunctionCode.MANAGEMENT_CATEGORY, CommandCode.VIEW)]
        public async Task<IActionResult> GetCategoriesPaging(string filter, int pageIndex, int pageSize)
        {
            var query = _context.Categories.AsQueryable();
            if (!string.IsNullOrEmpty(filter))
            {
                query = query.Where(x => x.Name.Contains(filter));
            }
            var totalRecords = await query.CountAsync();
            var items = await query.Skip((pageIndex - 1 * pageSize))
                .Take(pageSize).ToListAsync();

            var data = items.Select(c => CreateCategoryVm(c)).ToList();

            var pagination = new Pagination<CategoryVm>
            {
                Items = data,
                TotalRecords = totalRecords,
            };
            return Ok(pagination);
        }

        [HttpGet("{id}")]
        [ClaimRequirement(FunctionCode.MANAGEMENT_CATEGORY, CommandCode.VIEW)]
        public async Task<IActionResult> GetById(string id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
                return NotFound();

            CategoryVm categoryvm = CreateCategoryVm(category);

            return Ok(categoryvm);
        }

        [HttpPut("{id}")]
        [ClaimRequirement(FunctionCode.MANAGEMENT_CATEGORY, CommandCode.UPDATE)]
        public async Task<IActionResult> PutCategory(int id, [FromBody] CategoryCreateRequest request)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
                return NotFound();

            if (id == request.ParentId)
            {
                return BadRequest(new ApiBadRequestResponse("Category cannot be a child itself."));
            }

            category.Name = request.Name;
            category.ParentId = request.ParentId;
            category.SortOrder = request.SortOrder;
            category.SeoDescription = request.SeoDescription;
            category.SeoAlias = request.SeoAlias;

            _context.Categories.Update(category);
            var result = await _context.SaveChangesAsync();

            if (result > 0)
            {
                return NoContent();
            }
            return BadRequest(new ApiBadRequestResponse($"Update category with id:{category.Id} failed "));
        }

        [HttpDelete("{id}")]
        [ClaimRequirement(FunctionCode.MANAGEMENT_CATEGORY, CommandCode.DELETE)]
        public async Task<IActionResult> DeleteCategory(string id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
                return NotFound();

            _context.Categories.Remove(category);
            var result = await _context.SaveChangesAsync();
            if (result > 0)
            {
                CategoryVm categoryvm = CreateCategoryVm(category);
                return Ok(categoryvm);
            }
            return BadRequest(new ApiBadRequestResponse($"Delete category with id:{category.Id} failed "));
        }

        private static CategoryVm CreateCategoryVm(Category category)
        {
            return new CategoryVm()
            {
                Id = category.Id,
                Name = category.Name,
                SortOrder = category.SortOrder,
                ParentId = category.ParentId,
                NumberOfTrips = category.NumberOfTrips,
                SeoDescription = category.SeoDescription,
                SeoAlias = category.SeoDescription
            };
        }
    }
}


