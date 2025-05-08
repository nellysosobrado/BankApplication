using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddLastModifiedAndRegisteredColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.AddColumn<DateTime>(
                name: "Registered",
                table: "Customers",
                nullable: false,
                defaultValue: DateTime.Now);  

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModified",
                table: "Customers",
                nullable: false,
                defaultValue: DateTime.Now);  
        

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.DropColumn(
                name: "Registered",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "LastModified",
                table: "Customers");
        }
    }
}
