using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ParkingManagement.BackendServer.Authorization;
using ParkingManagement.BackendServer.Constants;
using ParkingManagement.BackendServer.Data;
using ParkingManagement.BackendServer.Data.Entities;
using ParkingManagement.BackendServer.Helpers;
using ParkingManagement.BackendServer.Services;
using ParkingManagement.ViewModels.Commons;
using ParkingManagement.ViewModels.Contents.RequestModels;
using ParkingManagement.ViewModels.Contents.ViewModels;
using System;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace ParkingManagement.BackendServer.Controllers
{
    public partial class ParkingLotBaseController : BaseController
    {
        private readonly ApplicationDbContext _context;
        private readonly IStorageService _storageService;
        private readonly ILogger<ParkingLotBaseController> _logger;

        public ParkingLotBaseController(ApplicationDbContext context,
                                     IStorageService storageService,
                                     ILogger<ParkingLotBaseController> logger)
        {
            _context = context;
            _storageService = storageService;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        #region Method Save File
        private async Task<Attachment> SaveFile(string licensePlate, IFormFile file)
        {
            //Gets the raw Content-Disposition header of the uploaded file.
            var originalFileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
            //File Name{(guid) + (Returns the extension (including the period ".") of the specified path string.--Phần đuôi của file bao gồm cả dấu ".")} 
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(originalFileName)}";
            //Call Service SaveFile.
            await _storageService.SaveFileAsync(file.OpenReadStream(), fileName);
            var attachmentEntity = new Attachment()
            {
                FileName = fileName,
                FilePath = _storageService.GetFileUrl(fileName),
                FileSize = file.Length,
                FileType = Path.GetExtension(fileName),
                LicensePlate = licensePlate,
            };
            return attachmentEntity;
        }
        #endregion
        #region CRUD ParkingLot
        [HttpPost]
        [ClaimRequirement(FunctionCode.MANAGEMENT_PARKINGLOT, CommandCode.CREATE)]
        public async Task<IActionResult> PostParkingLot([FromBody] ParkingLotCreateRequest reuqest)
        {
            _logger.LogInformation("Begin PostParkingLot API");
            throw new Exception();
            var parkingLot = new ParkingLot()
            {
                ParkArea = reuqest.ParkArea,
                ParkName = reuqest.ParkName,
                ParkPlace = reuqest.ParkPlace,
                ParkPrice = reuqest.ParkPrice,
                ParkStatus = reuqest.ParkStatus,
                TotalParkingSpace = reuqest.TotalParkingSpace,

            };
            await _context.ParkingLots.AddAsync(parkingLot);
            var result = await _context.SaveChangesAsync();
            if (result > 0)
            {
                _logger.LogInformation("End PostKnowledgeBase API - Success");
                return CreatedAtAction(nameof(GetById), new { id = parkingLot.Id }, reuqest);
            }
            else
            {
                _logger.LogInformation("End PostKnowledgeBase API - Failed");
                return BadRequest();
            }
        }

        [HttpDelete("{id}")]
        [ClaimRequirement(FunctionCode.MANAGEMENT_PARKINGLOT, CommandCode.DELETE)]
        public async Task<IActionResult> DeleteParkingLot(int id)
        {
            var dbParkingLot = await _context.ParkingLots.FindAsync(id);
            if (dbParkingLot == null)
            {
                return NotFound();
            }
            _context.ParkingLots.Remove(dbParkingLot);
            var result = await _context.SaveChangesAsync();
            if (result > 0)
            {
                var parkingLotVm = new ParkingLotVm()
                {
                    Id = dbParkingLot.Id,
                    ParkStatus = dbParkingLot.ParkStatus,
                    ParkPrice = dbParkingLot.ParkPrice,
                    ParkPlace = dbParkingLot.ParkPlace,
                    ParkName = dbParkingLot.ParkName,
                    ParkArea = dbParkingLot.ParkArea,
                    NumberOfCarsInTheParkingLot = dbParkingLot.NumberOfCarsInTheParkingLot,
                    TotalParkingSpace = dbParkingLot.TotalParkingSpace

                };
                return Ok(parkingLotVm);

            }
            else
            {
                return BadRequest();
            }
        }

        [HttpPut("{id}")]
        [ClaimRequirement(FunctionCode.MANAGEMENT_PARKINGLOT, CommandCode.UPDATE)]
        public async Task<IActionResult> PutParkingLot(int id, [FromBody] ParkingLotCreateRequest request)
        {
            var dbParkingLot = await _context.ParkingLots.FindAsync(id);
            if (dbParkingLot == null)
            {
                return NotFound();
            }

             dbParkingLot.ParkStatus = request.ParkStatus;
             dbParkingLot.ParkPrice = request.ParkPrice;    
             dbParkingLot.ParkPlace = request.ParkPlace;
             dbParkingLot.ParkName = request.ParkName;
             dbParkingLot.ParkArea = request.ParkArea;
             dbParkingLot.TotalParkingSpace = request.TotalParkingSpace;

            _context.ParkingLots.Update(dbParkingLot);
            var result = await _context.SaveChangesAsync();
            if (result > 0)
            {
                return NoContent();
            }
            return BadRequest();
        }

        [HttpGet]
        [ClaimRequirement(FunctionCode.MANAGEMENT_PARKINGLOT, CommandCode.VIEW)]
        public async Task<IActionResult> GetParkingLots()
        {
            var parkingLots = _context.ParkingLots;

            var parkingLotVms = await parkingLots.Select(u => new ParkingLotVm()
            {
                Id = u.Id,
                ParkStatus = u.ParkStatus,
                ParkPrice = u.ParkPrice,
                ParkPlace = u.ParkPlace,
                ParkName = u.ParkName,
                ParkArea = u.ParkArea,
                NumberOfCarsInTheParkingLot = u.NumberOfCarsInTheParkingLot,
                TotalParkingSpace = u.TotalParkingSpace,
            }).ToListAsync();

            return Ok(parkingLotVms);
        }

        [HttpGet("filter")]
        [ClaimRequirement(FunctionCode.MANAGEMENT_PARKINGLOT, CommandCode.VIEW)]
        public async Task<IActionResult> GetFunctionsPaging(string filter, int pageIndex, int pageSize)
        {
            var query = _context.ParkingLots.AsQueryable();
            if (!string.IsNullOrEmpty(filter))
            {
                query = query.Where(x => x.ParkName.Contains(filter)
                || x.ParkArea.ToString().Contains(filter)
                || x.ParkStatus.Contains(filter)
                || x.ParkPlace.Contains(filter));
            }
            var totalRecords = await query.CountAsync();
            var items = await query.Skip((pageIndex - 1 * pageSize))
                .Take(pageSize)
                .Select(u => new ParkingLotVm()
                {
                    Id = u.Id,
                    ParkStatus = u.ParkStatus,
                    ParkPrice = u.ParkPrice,
                    ParkPlace = u.ParkPlace,
                    ParkName = u.ParkName,
                    ParkArea = u.ParkArea,
                    NumberOfCarsInTheParkingLot = u.NumberOfCarsInTheParkingLot,
                    TotalParkingSpace = u.TotalParkingSpace,
                })
                .ToListAsync();

            var pagination = new Pagination<ParkingLotVm>
            {
                Items = items,
                TotalRecords = totalRecords,
            };
            return Ok(pagination);
        }

        [HttpGet("{id}")]
        [ClaimRequirement(FunctionCode.MANAGEMENT_PARKINGLOT, CommandCode.VIEW)]
        public async Task<IActionResult> GetById(int id)
        {
            var parkingLot = await _context.ParkingLots.FindAsync(id);
            if (parkingLot == null)
                return NotFound();

            var parkingLotVm = new ParkingLotVm()
            {
                Id = parkingLot.Id,
                ParkStatus = parkingLot.ParkStatus,
                ParkPrice = parkingLot.ParkPrice,
                ParkPlace = parkingLot.ParkPlace,
                ParkName = parkingLot.ParkName,
                ParkArea = parkingLot.ParkArea,
                NumberOfCarsInTheParkingLot = parkingLot.NumberOfCarsInTheParkingLot,
                TotalParkingSpace = parkingLot.TotalParkingSpace,
            };
            return Ok(parkingLotVm);
        }
        #endregion
    }
}
