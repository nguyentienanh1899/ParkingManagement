using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using ParkingManagement.BackendServer.Authorization;
using ParkingManagement.BackendServer.Constants;
using ParkingManagement.BackendServer.Data.Entities;
using ParkingManagement.BackendServer.Helpers;
using ParkingManagement.ViewModels.Commons;
using ParkingManagement.ViewModels.Contents.RequestModels;
using ParkingManagement.ViewModels.Contents.ViewModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ParkingManagement.BackendServer.Controllers
{
    public partial class ParkingLotBaseController
    {
        #region API Car vs ParkingLot

        [HttpGet("{parkingLotId}/cars/filter")]
        [ClaimRequirement(FunctionCode.MANAGEMENT_CAR, CommandCode.VIEW)]
        public async Task<IActionResult> GetCarsPaging(int parkingLotId, string filter, int pageIndex, int pageSize)
        {
            var query = _context.Cars.Where(x => x.ParkId == parkingLotId).AsQueryable();
            if (!string.IsNullOrEmpty(filter))
            {
                query = query.Where(x => x.LicensePlate.Contains(filter)
                                    || x.CarColor.Contains(filter)
                                    || x.CarType.Contains(filter)
                                    || x.Company.Contains(filter));
            }
            var totalRecords = await query.CountAsync();
            var items = await query.Skip((pageIndex - 1 * pageSize))
                .Take(pageSize)
                .Select(c => new CarVm()
                {
                    LicensePlate = c.LicensePlate,
                    CarColor = c.CarColor,
                    CarType = c.CarType,
                    Company = c.Company,
                    ParkId = c.ParkId
                })
                .ToListAsync();

            var pagination = new Pagination<CarVm>
            {
                Items = items,
                TotalRecords = totalRecords,
            };
            return Ok(pagination);
        }
        [HttpGet("{parkingLotId}/cars/{licensePlate}")]
        [ClaimRequirement(FunctionCode.MANAGEMENT_CAR, CommandCode.VIEW)]
        public async Task<IActionResult> GetCarDetail(string licensePlate)
        {
            var car = await _context.Cars.FindAsync(licensePlate);
            if (car == null)
                return NotFound();

            var attachments = await _context.Attachments
                .Where(x => x.LicensePlate == licensePlate)
                .Select(x => new AttachmentVm()
                {
                    FileName = x.FileName,
                    FilePath = x.FilePath,
                    FileSize = x.FileSize,
                    Id = x.Id,
                    FileType = x.FileType
                }).ToListAsync();

            var carVm = new CarVm()
            {
                LicensePlate = car.LicensePlate,
                CarColor = car.CarColor,
                CarType = car.CarType,
                Company = car.Company,
                ParkId = car.ParkId,
                Attachments = attachments
            };

            return Ok(carVm);
        }

        [HttpPost("{parkingLotId}/car")]
        [ClaimRequirement(FunctionCode.MANAGEMENT_CAR, CommandCode.CREATE)]
        [ApiValidationFilter]
        public async Task<IActionResult> PostComment(int parkingLotId, [FromForm] CarCreateRequest request)
        {
            var car = new Car()
            {
                LicensePlate = request.LicensePlate,
                CarColor = request.CarColor,
                CarType = request.CarType,
                Company = request.Company,
                ParkId = request.ParkId
            };
            //Process attachment
            if (request.Attachments != null && request.Attachments.Count > 0)
            {
                foreach (var attachment in request.Attachments)
                {
                    var attachmentEntity = await SaveFile(request.LicensePlate, attachment);
                    _context.Attachments.Add(attachmentEntity);
                }
            }
            _context.Cars.Add(car);

            var parkingLot = await _context.ParkingLots.FindAsync(parkingLotId);
            if (parkingLot == null) { return BadRequest(); }
            parkingLot.NumberOfCarsInTheParkingLot = parkingLot.NumberOfCarsInTheParkingLot.GetValueOrDefault(0) + 1;
            _context.ParkingLots.Update(parkingLot);

            var result = await _context.SaveChangesAsync();
            if (result > 0)
            {
                return CreatedAtAction(nameof(GetCarDetail), new { id = parkingLotId, licensePlate = car.LicensePlate }, request);
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpPut("{parkingLotId}/cars/{licensePlate}")]
        [ClaimRequirement(FunctionCode.MANAGEMENT_CAR, CommandCode.UPDATE)]
        public async Task<IActionResult> PutCar(string licensePlate, [FromBody] CarCreateRequest request)
        {
            var car = await _context.Cars.FindAsync(licensePlate);
            if (car == null)
                return NotFound();

            car.CarColor = request.CarColor;
            car.CarType = request.CarType;
            car.Company = request.Company;
            car.ParkId = request.ParkId;

            var result = await _context.SaveChangesAsync();

            if (result > 0)
            {
                return NoContent();
            }
            return BadRequest();
        }

        [HttpDelete("{parkingLotId}/cars/{licensePlate}")]
        [ClaimRequirement(FunctionCode.MANAGEMENT_CAR, CommandCode.DELETE)]
        public async Task<IActionResult> DeleteCar(int parkingLotId, string licensePlate)
        {
            var car = await _context.Cars.FindAsync(licensePlate);
            if (car == null)
                return NotFound();

            _context.Cars.Remove(car);

            var parkingLot = await _context.ParkingLots.FindAsync(parkingLotId);
            if (parkingLot == null) { return BadRequest(); }

            parkingLot.NumberOfCarsInTheParkingLot = parkingLot.NumberOfCarsInTheParkingLot.GetValueOrDefault(0) - 1;
            _context.ParkingLots.Update(parkingLot);

            var result = await _context.SaveChangesAsync();
            if (result > 0)
            {
                var carVm = new CarVm()
                {
                    LicensePlate = car.LicensePlate,
                    CarColor = car.CarColor,
                    CarType = car.CarType,
                    Company = car.Company,
                    ParkId = car.ParkId,
                };
                return Ok(carVm);
            }
            return BadRequest();
        }

        [HttpPost("import")]
        public async Task<IActionResult> Import(IFormFile formFile)
        {
            if (formFile == null || formFile.Length <= 0)
            {
                return BadRequest(new ApiBadRequestResponse("Invalid data file"));

            }
            if (!Path.GetExtension(formFile.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest(new ApiBadRequestResponse("Not Support file extension"));
            }
            using (var stream = new MemoryStream())
            {
                await formFile.CopyToAsync(stream);
                var listCar = new List<Car>();
                using (var package = new ExcelPackage(stream))
                {
                    //Get the worksheet to be manipulated in the excel file
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                    //Count rows with data in worksheet
                    var rowCount = worksheet.Dimension.Rows;
                    //Browse row by row retrieves each object on each row
                    for (int row = 2; row <= rowCount; row++)
                    {
                        listCar.Add(new Car
                        {
                            LicensePlate = worksheet.Cells[row, 1].ToString().Trim(),
                            CarColor = worksheet.Cells[row, 2].ToString().Trim(),
                            CarType = worksheet.Cells[row, 3].ToString().Trim(),
                            Company = worksheet.Cells[row, 4].ToString().Trim(),
                            ParkId = int.Parse(worksheet.Cells[row, 5].Value.ToString().Trim()) 
                        }); 
                    }
                    _context.Cars.AddRange(listCar);
                    _context.SaveChanges();
                    return Ok(listCar);
                }
            }
        }
        [HttpGet("export")]
        public async Task<IActionResult> Export(string author,string titleFile)
        {
            var listCar = new List<Car>(_context.Cars);
            var stream = new MemoryStream();

            using (var package = new ExcelPackage(stream))
            {
                //Đặt tên người tạo file
                package.Workbook.Properties.Author = author;
                //Đặt tiêu đề cho file
                package.Workbook.Properties.Title = titleFile;
                // tạo sheet 
                var workSheet = package.Workbook.Worksheets.Add("Sheet1");
                // tạo fontsize và fontfamily cho sheet
                workSheet.Cells.Style.Font.Size = 11;
                workSheet.Cells.Style.Font.Name = "Times New Roman";


                // danh sách các tên cột
                string[] arrColumnHeader = {"STT","LicensePlate", "CarColor","CarType", "Company", "ParkId"};
                // Gán row header
                for (var i = 0; i < arrColumnHeader.Length; i++)
                {
                    workSheet.Cells[3, i + 1].Value = arrColumnHeader[i];
                }

                // chỉnh style cho hàng 1
                workSheet.Cells[1, 1].Value = titleFile.ToUpper();
                workSheet.Cells["A1:I1"].Merge = true;
                workSheet.Cells[1, 1].Style.Font.Size = 16;
                workSheet.Row(1).Style.Font.Bold = true;
                workSheet.Row(1).Style.Font.Name = "Arial";
                workSheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                //Chỉnh style cho header
                workSheet.Cells["A3:I3"].Style.Font.Bold = true;
                workSheet.Cells["A3:I3"].Style.Font.Size = 10;
                workSheet.Cells["A3:I3"].Style.Font.Name = "Arial";
                workSheet.Cells["A3:I3"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                Color colFromHex = System.Drawing.ColorTranslator.FromHtml("#D9D9D9");
                workSheet.Cells["A3:I3"].Style.Fill.BackgroundColor.SetColor(colFromHex);
                workSheet.Cells["A3:I3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                //Chỉnh độ rộng các cột
                for (int i = 0; i < arrColumnHeader.Length; i++)
                {
                    workSheet.Column(i + 1).Width = 15;
                }

                // chỉnh style cho cột
                workSheet.Cells[$"A3:A{3 + listCar.Count}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                workSheet.Column(5).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                // Gán data list vào sheet
                var rowIndex = 4;
                for (int i = 0; i < listCar.Count(); i++)
                {
                    workSheet.Cells[rowIndex, 1].Value = i + 1;
                    workSheet.Cells[rowIndex, 2].Value = listCar[i].LicensePlate;
                    workSheet.Cells[rowIndex, 3].Value = listCar[i].CarColor;
                    workSheet.Cells[rowIndex, 4].Value = listCar[i].CarType;
                    workSheet.Cells[rowIndex, 5].Value = listCar[i].Company;
                    workSheet.Cells[rowIndex, 6].Value = listCar[i].ParkId;

                    for (int j = 0; j < arrColumnHeader.Length; j++)
                    {
                        //Kiểm tra các cột có giá trị rồi định dạng độ rộng nếu thiếu theo độ dài giá trị.
                        if (workSheet.Cells[rowIndex, j + 1].Value != null)
                        {
                            workSheet.Column(j + 1).Width = Math.Max(workSheet.Column(j + 1).Width, workSheet.Cells[rowIndex, j + 1].Value.ToString().Length + 5);
                        }
                    }
                    rowIndex++;
                }
                package.Save();
            }
            stream.Position = 0;
            string excelName = $"{titleFile}-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx";
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
        }
        #endregion
    }
}
