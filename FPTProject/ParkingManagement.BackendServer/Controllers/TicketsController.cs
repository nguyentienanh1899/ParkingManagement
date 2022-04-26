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
    public class TicketsController : BaseController
    {
        private readonly ApplicationDbContext _context;
        public TicketsController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpGet("{tripId}/{licensePlate}/tickets/filter")]
        [ClaimRequirement(FunctionCode.MANAGEMENT_TICKET, CommandCode.VIEW)]
        public async Task<IActionResult> GetTicketsPaging(int tripId, string licensePlate, string filter, int pageIndex, int pageSize)
        {
            var query = _context.Tickets.Where(x => x.TripId == tripId && x.LicensePlate == licensePlate).AsQueryable();
            if (!string.IsNullOrEmpty(filter))
            {
                query = query.Where(x => x.CustomerName.Contains(filter)
                                    || x.LicensePlate.Contains(filter)
                                    || x.TripId.ToString().Contains(filter));
            }
            var totalRecords = await query.CountAsync();
            var items = await query.Skip((pageIndex - 1 * pageSize))
                .Take(pageSize)
                .Select(c => new TicketVm()
                {
                    Id = c.Id,
                    CustomerName = c.CustomerName,
                    LicensePlate = c.LicensePlate,
                    BookingTime = c.BookingTime,
                    TripId = c.TripId,
                })
                .ToListAsync();

            var pagination = new Pagination<TicketVm>
            {
                Items = items,
                TotalRecords = totalRecords,
            };
            return Ok(pagination);
        }
        [HttpGet("{tripId}/{licensePlate}/tickets/{ticketId}")]
        [ClaimRequirement(FunctionCode.MANAGEMENT_TICKET, CommandCode.VIEW)]
        public async Task<IActionResult> GetTicketDetail(int ticketId)
        {
            var ticket = await _context.Tickets.FindAsync(ticketId);
            if (ticket == null)
                return NotFound();

            var ticketVm = new TicketVm()
            {
                Id = ticket.Id,
                CustomerName = ticket.CustomerName,
                LicensePlate = ticket.LicensePlate,
                BookingTime = ticket.BookingTime,
                TripId = ticket.TripId,
            };

            return Ok(ticketVm);
        }
        [HttpPost("{tripId}/{licensePlate}/tickets")]
        [ClaimRequirement(FunctionCode.MANAGEMENT_TICKET, CommandCode.CREATE)]
        [ApiValidationFilter]
        public async Task<IActionResult> PostTicket(int tripId,string licensePlate,  [FromBody] TicketCreateRequest request)
        {
            var ticket = new Ticket()
            {
                LicensePlate = request.LicensePlate,
                BookingTime = request.BookingTime,
                TripId = request.TripId,
                CustomerName = request.CustomerName,
            };
            _context.Tickets.Add(ticket);

            var trip = await _context.Trips.FindAsync(tripId);
            if (trip == null) { return BadRequest(); }
            trip.NumberOfTicketsAvailable = trip.MaximumOnlineTicketNumber.GetValueOrDefault() - 1;
            _context.Trips.Update(trip);

            var result = await _context.SaveChangesAsync();
            if (result > 0)
            {
                return CreatedAtAction(nameof(GetTicketDetail), new { id = tripId, strlicensePlate = licensePlate, ticketId = ticket.Id }, request);
            }
            else
            {
                return BadRequest();
            }
        }
        [HttpPut("{tripId}/{licensePlate}/tickets/{ticketId}")]
        [ClaimRequirement(FunctionCode.MANAGEMENT_TICKET, CommandCode.UPDATE)]
        public async Task<IActionResult> PutTicket(int ticketId, [FromBody] TicketCreateRequest request)
        {
            var ticket = await _context.Tickets.FindAsync(ticketId);
            if (ticket == null)
                return NotFound();

            ticket.CustomerName = request.CustomerName;

            var result = await _context.SaveChangesAsync();

            if (result > 0)
            {
                return NoContent();
            }
            return BadRequest();
        }
        [HttpDelete("{tripId}/{licensePlate}/tickets/{ticketId}")]
        [ClaimRequirement(FunctionCode.MANAGEMENT_TICKET, CommandCode.DELETE)]
        public async Task<IActionResult> DeleteTicket(int tripId, int ticketId)
        {
            var ticket = await _context.Tickets.FindAsync(ticketId);
            if (ticket == null)
                return NotFound();

            _context.Tickets.Remove(ticket);

            var trip = await _context.Trips.FindAsync(tripId);
            if (trip == null) { return BadRequest(); }

            trip.NumberOfTicketsAvailable = trip.NumberOfTicketsAvailable.GetValueOrDefault(0) + 1;
            _context.Trips.Update(trip);

            var result = await _context.SaveChangesAsync();
            if (result > 0)
            {
                var ticketVm = new TicketVm()
                {
                    Id = ticket.Id,
                    BookingTime = ticket.BookingTime,
                    CustomerName = ticket.CustomerName,
                    LicensePlate = ticket.LicensePlate,
                    TripId = ticket.TripId,
                };
                return Ok(ticket);
            }
            return BadRequest();
        }
    }
}
