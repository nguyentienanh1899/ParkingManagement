using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ParkingManagement.BackendServer.Authorization;
using ParkingManagement.BackendServer.Constants;
using ParkingManagement.BackendServer.Data;
using ParkingManagement.BackendServer.Data.Entities;
using ParkingManagement.BackendServer.Helpers;
using ParkingManagement.ViewModels.Commons;
using ParkingManagement.ViewModels.Contents.RequestModels;
using ParkingManagement.ViewModels.Contents.ViewModels;
using System.Linq;
using System.Threading.Tasks;

namespace ParkingManagement.BackendServer.Controllers
{
    public partial class TripBaseController : BaseController
    {
        private readonly ApplicationDbContext _context;
        public TripBaseController(ApplicationDbContext context)
        {
            _context = context;
        }
        #region CRUD with Trip
        [HttpPost]
        [ClaimRequirement(FunctionCode.MANAGEMENT_TRIP, CommandCode.CREATE)]
        [ApiValidationFilter]
        public async Task<IActionResult> PostTrip([FromBody] TripCreateRequest reuqest)
        {

            var trip = new Trip()
            {
                CategoryId = reuqest.CategoryId,
                BookedTicketNumber = reuqest.BookedTicketNumber,
                CarType = reuqest.CarType,
                DepartureDate = reuqest.DepartureDate,
                DepartureTime = reuqest.DepartureTime,
                Destination = reuqest.Destination,
                Driver = reuqest.Driver,
                MaximumOnlineTicketNumber = reuqest.MaximumOnlineTicketNumber,
            };
            _context.Trips.Add(trip);
            var result = await _context.SaveChangesAsync();
            if (result > 0)
            {
                return CreatedAtAction(nameof(GetById), new { id = trip.Id }, reuqest);
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpDelete("{id}")]
        [ClaimRequirement(FunctionCode.MANAGEMENT_TRIP, CommandCode.DELETE)]
        public async Task<IActionResult> DeleteTrip(string id)
        {
            var trip = await _context.Trips.FindAsync(id);
            if (trip == null)
            {
                return NotFound();
            }
            _context.Trips.Remove(trip);
            var result = await _context.SaveChangesAsync();
            if (result > 0)
            {
                var tripVm = new TripVm()
                {
                    Id = trip.Id,
                    CategoryId = trip.CategoryId,
                    CarType = trip.CarType,
                    BookedTicketNumber = trip.BookedTicketNumber,
                    DepartureDate = trip.DepartureDate,
                    DepartureTime = trip.DepartureTime,
                    Destination = trip.Destination,
                    Driver = trip.Driver,
                    MaximumOnlineTicketNumber =(int) trip.MaximumOnlineTicketNumber,
                    NumberOfTicketsAvailable = (int)trip.NumberOfTicketsAvailable,
                    NumberOfAvailableTicketOffices = (int)trip.NumberOfAvailableTicketOffices,
                };
                return Ok(tripVm);

            }
            else
            {
                return BadRequest();
            }
        }


        [HttpPut("{id}")]
        [ClaimRequirement(FunctionCode.MANAGEMENT_TRIP, CommandCode.UPDATE)]
        public async Task<IActionResult> PutTrip(string id, [FromBody] TripCreateRequest request)
        {
            var dbTrip = await _context.Trips.FindAsync(id);
            if (dbTrip == null)
            {
                return NotFound();
            }

            dbTrip.CategoryId = request.CategoryId;
            dbTrip.BookedTicketNumber = request.BookedTicketNumber;
            dbTrip.CarType = request.CarType;
            dbTrip.DepartureDate = request.DepartureDate;
            dbTrip.DepartureTime = request.DepartureTime;
            dbTrip.Destination = request.Destination;
            dbTrip.Driver = request.Driver;
            dbTrip.MaximumOnlineTicketNumber = request.MaximumOnlineTicketNumber;

            _context.Trips.Update(dbTrip);
            var result = await _context.SaveChangesAsync();
            if (result > 0)
            {
                return NoContent();
            }
            return BadRequest();
        }

        [HttpGet]
        [ClaimRequirement(FunctionCode.MANAGEMENT_TRIP, CommandCode.VIEW)]
        public async Task<IActionResult> GetTrips()
        {
            var trips = _context.Trips;

            var tripVms = await trips.Select(u => new TripQickVm()
            {
                Id = u.Id,
                CategoryId = u.CategoryId,
                CarType = u.CarType,
                DepartureDate = u.DepartureDate,
                DepartureTime = u.DepartureTime,
                Destination = u.Destination,
            }).ToListAsync();

            return Ok(tripVms);
        }

        [HttpGet("filter")]
        [ClaimRequirement(FunctionCode.MANAGEMENT_TRIP, CommandCode.VIEW)]
        public async Task<IActionResult> GetTripsPaging(string filter, int pageIndex, int pageSize)
        {
            var query = _context.Trips.AsQueryable();
            if (!string.IsNullOrEmpty(filter))
            {
                query = query.Where(x => x.Destination.Contains(filter)
                || x.CarType.Contains(filter));
            }
            var totalRecords = await query.CountAsync();
            var items = await query.Skip((pageIndex - 1 * pageSize))
                .Take(pageSize)
                .Select(u => new TripQickVm()
                {
                    Id = u.Id,
                    CategoryId = u.CategoryId,
                    CarType = u.CarType,
                    DepartureDate = u.DepartureDate,
                    DepartureTime = u.DepartureTime,
                    Destination = u.Destination,
                })
                .ToListAsync();

            var pagination = new Pagination<TripQickVm>
            {
                Items = items,
                TotalRecords = totalRecords,
            };
            return Ok(pagination);
        }

        [HttpGet("{id}")]
        [ClaimRequirement(FunctionCode.MANAGEMENT_TRIP, CommandCode.VIEW)]
        public async Task<IActionResult> GetById(int id)
        {
            var trip = await _context.Trips.FindAsync(id);
            if (trip == null)
                return NotFound();

            var tripVm = new TripVm()
            {
                Id = trip.Id,
                CategoryId = trip.CategoryId,
                BookedTicketNumber = trip.BookedTicketNumber,
                CarType = trip.CarType,
                DepartureDate = trip.DepartureDate,
                DepartureTime = trip.DepartureTime,
                Destination = trip.Destination,
                Driver = trip.Driver,
                MaximumOnlineTicketNumber = trip.MaximumOnlineTicketNumber,
                NumberOfTicketsAvailable = trip.NumberOfTicketsAvailable,
                NumberOfAvailableTicketOffices = trip.NumberOfAvailableTicketOffices,
            };
            return Ok(tripVm);
        }


        #endregion
    }
}