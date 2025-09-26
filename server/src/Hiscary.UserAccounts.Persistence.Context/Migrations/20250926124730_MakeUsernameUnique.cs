using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hiscary.UserAccounts.Persistence.Context.Migrations
{
    /// <inheritdoc />
    public partial class MakeUsernameUnique : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_UserAccounts_Username",
                schema: "useraccounts",
                table: "UserAccounts",
                column: "Username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserAccounts_Username",
                schema: "useraccounts",
                table: "UserAccounts");
        }
    }
}
