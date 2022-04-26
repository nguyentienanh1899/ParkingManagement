using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ParkingManagement.ViewModels.Contents.ViewModels;
using System.Linq;
using System.Threading.Tasks;

namespace ParkingManagement.BackendServer.Controllers
{
    public partial class ParkingLotBaseController
    {
        #region Attachment For Car
        [HttpGet("{licensePlate}/attachments")]
        public async Task<IActionResult> GetAttachment(string licensePlate)
        {
            var query = await _context.Attachments
                .Where(x => x.LicensePlate == licensePlate)
                .Select(c => new AttachmentVm()
                {
                    Id = c.Id,
                    LastModifiedDate = c.LastModifiedDate,
                    CreateDate = c.CreateDate,
                    FileName = c.FileName,
                    FilePath = c.FilePath,
                    FileSize = c.FileSize,
                    FileType = c.FileType,
                    LicensePlate = c.LicensePlate,
                }).ToListAsync();

            return Ok(query);
        }
        [HttpDelete("{licensePlate}/attachments/{attachmentId}")]
        public async Task<IActionResult> DeleteAttachment(int attachmentId)
        {
            var attachment = await _context.Attachments.FindAsync(attachmentId);
            if (attachment == null)
                return NotFound();

            _context.Attachments.Remove(attachment);

            var result = await _context.SaveChangesAsync();
            if (result > 0)
            {
                return Ok();
            }
            return BadRequest();
        }

        #endregion
    }
}
