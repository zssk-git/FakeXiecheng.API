using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FakeXiecheng.API.Migrations
{
    public partial class OrderMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "OrderId",
                table: "LineItems",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    UserId = table.Column<string>(nullable: true),
                    State = table.Column<int>(nullable: false),
                    CraeteDateUTC = table.Column<DateTime>(nullable: false),
                    TransactionMetadata = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Orders_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "822bb235-faba-485f-816e-7da4bd47170a",
                column: "ConcurrencyStamp",
                value: "3cc4f5d0-026a-42c9-acc9-959641ad04f1");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "4eb5db6d-b327-4aed-9785-a7863fd7d5d5",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "4aa2495c-9cd4-48cb-af83-7437a93d6fb7", "AQAAAAEAACcQAAAAENJrCRvNI4Hm71i84j+WD663BwoLKz3QEF2nA3nADgB1TbgdKymqYxwwyv1ekzd7eg==", "5f84d12c-e8a2-40c2-a76b-571362953755" });

            migrationBuilder.CreateIndex(
                name: "IX_LineItems_OrderId",
                table: "LineItems",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_UserId",
                table: "Orders",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_LineItems_Orders_OrderId",
                table: "LineItems",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LineItems_Orders_OrderId",
                table: "LineItems");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_LineItems_OrderId",
                table: "LineItems");

            migrationBuilder.DropColumn(
                name: "OrderId",
                table: "LineItems");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "822bb235-faba-485f-816e-7da4bd47170a",
                column: "ConcurrencyStamp",
                value: "a9ee210e-4f4c-449b-ab36-21b68988207a");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "4eb5db6d-b327-4aed-9785-a7863fd7d5d5",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "8d5d180f-2ff5-4bf6-9ace-fc0e8da2fbae", "AQAAAAEAACcQAAAAEHraqK0GYdR0Sprg9b6l6mYdWP6AYZKOwnOiAyy5Tl54Qq7ot+pqb2Ju/ELyY7VIhw==", "1d8b0c9c-a8cd-49bd-8d6e-fe93346dc21d" });
        }
    }
}
