using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BankingApi.EventReceiver.Migrations.EventMessageDb
{
    /// <inheritdoc />
    public partial class EventMessageDbContext_IsProcessedField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsProcessed",
                table: "EventMessages",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsProcessed",
                table: "EventMessages");
        }
    }
}
