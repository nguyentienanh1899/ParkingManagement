using Microsoft.EntityFrameworkCore.Migrations;

namespace ParkingManagement.BackendServer.Data.Migrations
{
    public partial class Update1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attachments_Cars_CarLicensePlate",
                table: "Attachments");

            migrationBuilder.DropForeignKey(
                name: "FK_Attachments_ParkingLots_ParkingLotId",
                table: "Attachments");

            migrationBuilder.DropForeignKey(
                name: "FK_Cars_ParkingLots_ParkingLotId",
                table: "Cars");

            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_Cars_CarLicensePlate",
                table: "Tickets");

            migrationBuilder.DropIndex(
                name: "IX_Tickets_CarLicensePlate",
                table: "Tickets");

            migrationBuilder.DropIndex(
                name: "IX_Cars_ParkingLotId",
                table: "Cars");

            migrationBuilder.DropIndex(
                name: "IX_Attachments_CarLicensePlate",
                table: "Attachments");

            migrationBuilder.DropIndex(
                name: "IX_Attachments_ParkingLotId",
                table: "Attachments");

            migrationBuilder.DropColumn(
                name: "CarLicensePlate",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "ParkingLotId",
                table: "Cars");

            migrationBuilder.DropColumn(
                name: "CarLicensePlate",
                table: "Attachments");

            migrationBuilder.DropColumn(
                name: "ParkingLotId",
                table: "Attachments");

            migrationBuilder.AddColumn<int>(
                name: "NumberOfAvailableTicketOffices",
                table: "Trips",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NumberOfTicketsAvailable",
                table: "Trips",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "LicensePlate",
                table: "Tickets",
                type: "varchar(50)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LicensePlate",
                table: "Attachments",
                type: "varchar(50)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_LicensePlate",
                table: "Tickets",
                column: "LicensePlate");

            migrationBuilder.CreateIndex(
                name: "IX_Cars_ParkId",
                table: "Cars",
                column: "ParkId");

            migrationBuilder.CreateIndex(
                name: "IX_Attachments_LicensePlate",
                table: "Attachments",
                column: "LicensePlate");

            migrationBuilder.CreateIndex(
                name: "IX_Attachments_ParkId",
                table: "Attachments",
                column: "ParkId");

            migrationBuilder.AddForeignKey(
                name: "FK_Attachments_Cars_LicensePlate",
                table: "Attachments",
                column: "LicensePlate",
                principalTable: "Cars",
                principalColumn: "LicensePlate",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Attachments_ParkingLots_ParkId",
                table: "Attachments",
                column: "ParkId",
                principalTable: "ParkingLots",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Cars_ParkingLots_ParkId",
                table: "Cars",
                column: "ParkId",
                principalTable: "ParkingLots",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_Cars_LicensePlate",
                table: "Tickets",
                column: "LicensePlate",
                principalTable: "Cars",
                principalColumn: "LicensePlate",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attachments_Cars_LicensePlate",
                table: "Attachments");

            migrationBuilder.DropForeignKey(
                name: "FK_Attachments_ParkingLots_ParkId",
                table: "Attachments");

            migrationBuilder.DropForeignKey(
                name: "FK_Cars_ParkingLots_ParkId",
                table: "Cars");

            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_Cars_LicensePlate",
                table: "Tickets");

            migrationBuilder.DropIndex(
                name: "IX_Tickets_LicensePlate",
                table: "Tickets");

            migrationBuilder.DropIndex(
                name: "IX_Cars_ParkId",
                table: "Cars");

            migrationBuilder.DropIndex(
                name: "IX_Attachments_LicensePlate",
                table: "Attachments");

            migrationBuilder.DropIndex(
                name: "IX_Attachments_ParkId",
                table: "Attachments");

            migrationBuilder.DropColumn(
                name: "NumberOfAvailableTicketOffices",
                table: "Trips");

            migrationBuilder.DropColumn(
                name: "NumberOfTicketsAvailable",
                table: "Trips");

            migrationBuilder.AlterColumn<string>(
                name: "LicensePlate",
                table: "Tickets",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CarLicensePlate",
                table: "Tickets",
                type: "varchar(50)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ParkingLotId",
                table: "Cars",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LicensePlate",
                table: "Attachments",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CarLicensePlate",
                table: "Attachments",
                type: "varchar(50)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ParkingLotId",
                table: "Attachments",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_CarLicensePlate",
                table: "Tickets",
                column: "CarLicensePlate");

            migrationBuilder.CreateIndex(
                name: "IX_Cars_ParkingLotId",
                table: "Cars",
                column: "ParkingLotId");

            migrationBuilder.CreateIndex(
                name: "IX_Attachments_CarLicensePlate",
                table: "Attachments",
                column: "CarLicensePlate");

            migrationBuilder.CreateIndex(
                name: "IX_Attachments_ParkingLotId",
                table: "Attachments",
                column: "ParkingLotId");

            migrationBuilder.AddForeignKey(
                name: "FK_Attachments_Cars_CarLicensePlate",
                table: "Attachments",
                column: "CarLicensePlate",
                principalTable: "Cars",
                principalColumn: "LicensePlate",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Attachments_ParkingLots_ParkingLotId",
                table: "Attachments",
                column: "ParkingLotId",
                principalTable: "ParkingLots",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Cars_ParkingLots_ParkingLotId",
                table: "Cars",
                column: "ParkingLotId",
                principalTable: "ParkingLots",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_Cars_CarLicensePlate",
                table: "Tickets",
                column: "CarLicensePlate",
                principalTable: "Cars",
                principalColumn: "LicensePlate",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
