using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BarbaKidsShop.Data.Migrations
{
    /// <inheritdoc />
    public partial class MakeShippingDetailIdNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_ShippingDetails_ShippingDetailId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_ShippingDetailId",
                table: "Orders");

            migrationBuilder.AlterColumn<int>(
                name: "ShippingDetailId",
                table: "Orders",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_ShippingDetailId",
                table: "Orders",
                column: "ShippingDetailId",
                unique: true,
                filter: "[ShippingDetailId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_ShippingDetails_ShippingDetailId",
                table: "Orders",
                column: "ShippingDetailId",
                principalTable: "ShippingDetails",
                principalColumn: "ShippingDetailId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_ShippingDetails_ShippingDetailId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_ShippingDetailId",
                table: "Orders");

            migrationBuilder.AlterColumn<int>(
                name: "ShippingDetailId",
                table: "Orders",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_ShippingDetailId",
                table: "Orders",
                column: "ShippingDetailId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_ShippingDetails_ShippingDetailId",
                table: "Orders",
                column: "ShippingDetailId",
                principalTable: "ShippingDetails",
                principalColumn: "ShippingDetailId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
