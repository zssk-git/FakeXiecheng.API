using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FakeXiecheng.API.Migrations
{
    public partial class DataSeeding : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "TouristRoutes",
                columns: new[] { "Id", "CreateTime", "DapartureTime", "Description", "DiscountPresent", "Features", "Fees", "Notes", "OriginalPrice", "Title", "UpdateTime" },
                values: new object[] { new Guid("cc3f83ba-3d5e-43a4-8bf5-5e522bfcd4c4"), new DateTime(2021, 4, 26, 8, 41, 37, 261, DateTimeKind.Utc).AddTicks(6626), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "shouming", null, null, null, null, 0m, "ceshititle", null });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "TouristRoutes",
                keyColumn: "Id",
                keyValue: new Guid("cc3f83ba-3d5e-43a4-8bf5-5e522bfcd4c4"));
        }
    }
}
