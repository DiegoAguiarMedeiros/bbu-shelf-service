using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTradeOfferEntryRelations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ShelfEntryId",
                table: "TradeOfferEntrys",
                newName: "ShelfId");

            migrationBuilder.CreateIndex(
                name: "IX_TradeOfferEntrys_ShelfId",
                table: "TradeOfferEntrys",
                column: "ShelfId");

            migrationBuilder.CreateIndex(
                name: "IX_TradeOfferEntrys_TradeOfferId",
                table: "TradeOfferEntrys",
                column: "TradeOfferId");

            migrationBuilder.AddForeignKey(
                name: "FK_TradeOfferEntry_Shelfs_ShelfId",
                table: "TradeOfferEntrys",
                column: "ShelfId",
                principalTable: "Shelfs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TradeOfferEntry_TradeOffers_TradeOfferId",
                table: "TradeOfferEntrys",
                column: "TradeOfferId",
                principalTable: "TradeOffers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TradeOfferEntry_Shelfs_ShelfId",
                table: "TradeOfferEntrys");

            migrationBuilder.DropForeignKey(
                name: "FK_TradeOfferEntry_TradeOffers_TradeOfferId",
                table: "TradeOfferEntrys");

            migrationBuilder.DropIndex(
                name: "IX_TradeOfferEntrys_ShelfId",
                table: "TradeOfferEntrys");

            migrationBuilder.DropIndex(
                name: "IX_TradeOfferEntrys_TradeOfferId",
                table: "TradeOfferEntrys");

            migrationBuilder.RenameColumn(
                name: "ShelfId",
                table: "TradeOfferEntrys",
                newName: "ShelfEntryId");
        }
    }
}
