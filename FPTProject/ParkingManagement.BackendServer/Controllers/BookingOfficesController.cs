using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ParkingManagement.BackendServer.Authorization;
using ParkingManagement.BackendServer.Constants;
using ParkingManagement.BackendServer.Data.Entities;
using ParkingManagement.BackendServer.Helpers;
using ParkingManagement.ViewModels.Commons;
using ParkingManagement.ViewModels.Contents.RequestModels;
using ParkingManagement.ViewModels.Contents.ViewModels;
using System.Linq;
using System.Threading.Tasks;

namespace ParkingManagement.BackendServer.Controllers
{
    public partial class TripBaseController  
    {
        #region BookingOffice

        [HttpGet("{tripId}/bookingoffices/filter")]
        [ClaimRequirement(FunctionCode.MANAGEMENT_BOOKINGOFFICE, CommandCode.VIEW)]
        public async Task<IActionResult> GetBookingOfficesPaging(int tripId, string filter, int pageIndex, int pageSize)
        {
            var query = _context.BookingOffices.Where(x => x.TripId == tripId).AsQueryable();
            if (!string.IsNullOrEmpty(filter))
            {
                query = query.Where(x => x.OfficeName.Contains(filter)
                                    || x.OfficePlace.Contains(filter)
                                    || x.OfficePhone.Contains(filter));
            }
            var totalRecords = await query.CountAsync();
            var items = await query.Skip((pageIndex - 1 * pageSize))
                .Take(pageSize)
                .Select(c => new BookingOfficeVm()
                {
                    Id = c.Id,
                    StartContractDeadLine = c.StartContractDeadLine,
                    EndContractDeadline = c.EndContractDeadline,
                    OfficeName = c.OfficeName,
                    OfficePhone = c.OfficePhone,
                    OfficePlace = c.OfficePlace,
                    OfficePrice = c.OfficePrice,
                    TripId = c.TripId,
                })
                .ToListAsync();

            var pagination = new Pagination<BookingOfficeVm>
            {
                Items = items,
                TotalRecords = totalRecords,
            };
            return Ok(pagination);
        }
        [HttpGet("{tripId}/bookingoffices/{bookingOfficeId}")]
        [ClaimRequirement(FunctionCode.MANAGEMENT_BOOKINGOFFICE, CommandCode.VIEW)]
        public async Task<IActionResult> GetBookingOfficeDetail(int bookingOfficeId)
        {
            var bookingOffice = await _context.BookingOffices.FindAsync(bookingOfficeId);
            if (bookingOffice == null)
                return NotFound();

            var bookingOfficeVm = new BookingOfficeVm()
            {
                Id = bookingOffice.Id,
                StartContractDeadLine = bookingOffice.StartContractDeadLine,
                EndContractDeadline = bookingOffice.EndContractDeadline,
                OfficeName = bookingOffice.OfficeName,
                OfficePhone = bookingOffice.OfficePhone,
                OfficePlace = bookingOffice.OfficePlace,
                OfficePrice = bookingOffice.OfficePrice,
                TripId = bookingOffice.TripId,
            };

            return Ok(bookingOfficeVm);
        }
        [HttpPost("{tripId}/bookingoffices")]
        [ClaimRequirement(FunctionCode.MANAGEMENT_BOOKINGOFFICE, CommandCode.CREATE)]
        [ApiValidationFilter]
        public async Task<IActionResult> PostBookingOffice(int tripId, [FromBody] BookingOfficeRequest request)
        {
            var bookingOffice = new BookingOffice()
            {
                EndContractDeadline = request.EndContractDeadline,
                StartContractDeadLine = request.StartContractDeadLine,
                OfficeName = request.OfficeName,
                OfficePhone = request.OfficePhone,
                OfficePlace = request.OfficePlace,
                OfficePrice = request.OfficePrice,
                TripId = request.TripId,
            };
            _context.BookingOffices.Add(bookingOffice);

            var trip = await _context.Trips.FindAsync(tripId);
            if (trip == null) { return BadRequest(); }
            trip.NumberOfAvailableTicketOffices = trip.NumberOfAvailableTicketOffices.GetValueOrDefault(0) + 1;
            _context.Trips.Update(trip);

            var result = await _context.SaveChangesAsync();
            if (result > 0)
            {
                return CreatedAtAction(nameof(GetBookingOfficeDetail), new { id = tripId, bookingOfficeId = bookingOffice.Id }, request);
            }
            else
            {
                return BadRequest();
            }
        }
        [HttpPut("{tripId}/bookingoffices/{bookingOfficeId}")]
        [ClaimRequirement(FunctionCode.MANAGEMENT_BOOKINGOFFICE, CommandCode.UPDATE)]
        public async Task<IActionResult> PutBookingOffice(int bookingOfficeId, [FromBody] BookingOfficeRequest request)
        {
            var bookingOffice = await _context.BookingOffices.FindAsync(bookingOfficeId);
            if (bookingOffice == null)
                return NotFound();

            bookingOffice.EndContractDeadline = request.EndContractDeadline;
            bookingOffice.StartContractDeadLine = request.StartContractDeadLine;
            bookingOffice.OfficeName = request.OfficeName;
            bookingOffice.OfficePrice = request.OfficePrice;
            bookingOffice.OfficePhone = request.OfficePhone;
            bookingOffice.OfficePlace = request.OfficePlace;
            bookingOffice.TripId = request.TripId;
            _context.BookingOffices.Update(bookingOffice);

            var result = await _context.SaveChangesAsync();

            if (result > 0)
            {
                return NoContent();
            }
            return BadRequest();
        }
        [HttpDelete("{tripId}/bookingoffices/{bookingOfficeId}")]
        [ClaimRequirement(FunctionCode.MANAGEMENT_BOOKINGOFFICE, CommandCode.DELETE)]
        public async Task<IActionResult> DeleteBookingOffice(int tripId, int bookingOfficeId)
        {
            var bookingOffice = await _context.BookingOffices.FindAsync(bookingOfficeId);
            if (bookingOffice == null)
                return NotFound();

            _context.BookingOffices.Remove(bookingOffice);

            var trip = await _context.Trips.FindAsync(tripId);
            if (trip == null) { return BadRequest(); }

            trip.NumberOfAvailableTicketOffices = trip.NumberOfAvailableTicketOffices.GetValueOrDefault(0) - 1;
            _context.Trips.Update(trip);

            var result = await _context.SaveChangesAsync();
            if (result > 0)
            {
                var bookingOfficeVm = new BookingOfficeVm()
                {
                    Id = bookingOffice.Id,
                    StartContractDeadLine = bookingOffice.StartContractDeadLine,
                    EndContractDeadline = bookingOffice.EndContractDeadline,
                    OfficeName = bookingOffice.OfficeName,
                    OfficePhone = bookingOffice.OfficePhone,
                    OfficePlace = bookingOffice.OfficePlace,
                    OfficePrice = bookingOffice.OfficePrice,
                    TripId = bookingOffice.TripId,
                };
                return Ok(bookingOfficeVm);
            }
            return BadRequest();
        }
        #endregion
    }
}
