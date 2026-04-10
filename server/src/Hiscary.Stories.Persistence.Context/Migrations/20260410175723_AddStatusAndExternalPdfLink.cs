using Hiscary.Shared.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hiscary.Stories.Persistence.Context.Migrations
{
    /// <inheritdoc />
    public partial class AddStatusAndExternalPdfLink : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<ExternalPdf>(
                name: "ExternalPdf",
                schema: "stories",
                table: "Stories",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ExternalPdfPageCount",
                schema: "stories",
                table: "Stories",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                schema: "stories",
                table: "Stories",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExternalPdf",
                schema: "stories",
                table: "Stories");

            migrationBuilder.DropColumn(
                name: "ExternalPdfPageCount",
                schema: "stories",
                table: "Stories");

            migrationBuilder.DropColumn(
                name: "Status",
                schema: "stories",
                table: "Stories");
        }
    }
}
