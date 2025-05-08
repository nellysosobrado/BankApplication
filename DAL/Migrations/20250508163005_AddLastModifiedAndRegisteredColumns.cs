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
            // Lägg till kolumnerna i Customers-tabellen
            migrationBuilder.AddColumn<DateTime>(
                name: "Registered",
                table: "Customers",
                nullable: false,
                defaultValue: DateTime.Now);  // Eller ett annat defaultvärde om du vill

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModified",
                table: "Customers",
                nullable: false,
                defaultValue: DateTime.Now);  // Eller ett annat defaultvärde om du vill
        

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Ta bort kolumnerna om migrationen rullas tillbaka
            migrationBuilder.DropColumn(
                name: "Registered",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "LastModified",
                table: "Customers");
        }
    }
}
