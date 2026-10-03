using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddOwnershipHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OwnershipHistorys",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ShelfId = table.Column<Guid>(type: "uuid", nullable: false),
                    FromUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    ToUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    TradeOfferId = table.Column<Guid>(type: "uuid", nullable: true),
                    AcquiredSource = table.Column<string>(type: "text", nullable: false),
                    TransferredAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OwnershipHistorys", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OwnershipHistory_Shelfs_ShelfId",
                        column: x => x.ShelfId,
                        principalTable: "Shelfs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OwnershipHistory_TradeOffers_TradeOfferId",
                        column: x => x.TradeOfferId,
                        principalTable: "TradeOffers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OwnershipHistory_ShelfId_TransferredAt",
                table: "OwnershipHistorys",
                columns: new[] { "ShelfId", "TransferredAt" });

            migrationBuilder.CreateIndex(
                name: "IX_OwnershipHistory_TradeOfferId",
                table: "OwnershipHistorys",
                column: "TradeOfferId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OwnershipHistorys");
        }
    }
}
