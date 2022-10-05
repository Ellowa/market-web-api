using Microsoft.EntityFrameworkCore.Migrations;

namespace Data.Migrations
{
    public partial class addCompositePKToReceiptDetailTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ReceiptsDetails",
                table: "ReceiptsDetails");

            migrationBuilder.DropIndex(
                name: "IX_ReceiptsDetails_ReceiptId",
                table: "ReceiptsDetails");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ReceiptsDetails",
                table: "ReceiptsDetails",
                columns: new[] { "ReceiptId", "ProductId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ReceiptsDetails",
                table: "ReceiptsDetails");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ReceiptsDetails",
                table: "ReceiptsDetails",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_ReceiptsDetails_ReceiptId",
                table: "ReceiptsDetails",
                column: "ReceiptId");
        }
    }
}
