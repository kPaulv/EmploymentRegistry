using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmploymentRegistry.Migrations
{
    public partial class SeedIdentityRoles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "047102ce-92d1-42ff-ade5-12b00c259b8c", "77b64b46-6219-48db-a448-aee7144528a6", "SuperAdmin", "SUPERADMIN" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "5f1d4b8d-2645-4705-86cb-3fd62145a0a1", "d6896d9e-e598-459d-8f2f-433ab29e706f", "Guest", "GUEST" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "92637821-59d1-4482-9200-f111f6bd535a", "bd03cea5-7f62-4383-bc5a-dff7e9a9d32c", "Admin", "ADMIN" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "047102ce-92d1-42ff-ade5-12b00c259b8c");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "5f1d4b8d-2645-4705-86cb-3fd62145a0a1");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "92637821-59d1-4482-9200-f111f6bd535a");
        }
    }
}
