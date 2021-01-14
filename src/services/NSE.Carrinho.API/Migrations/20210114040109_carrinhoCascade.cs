using Microsoft.EntityFrameworkCore.Migrations;

namespace NSE.Carrinho.API.Migrations
{
    public partial class carrinhoCascade : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CarrinhoItems_CarrinhoCliente_CarrinhoId",
                table: "CarrinhoItems");

            migrationBuilder.AddForeignKey(
                name: "FK_CarrinhoItems_CarrinhoCliente_CarrinhoId",
                table: "CarrinhoItems",
                column: "CarrinhoId",
                principalTable: "CarrinhoCliente",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CarrinhoItems_CarrinhoCliente_CarrinhoId",
                table: "CarrinhoItems");

            migrationBuilder.AddForeignKey(
                name: "FK_CarrinhoItems_CarrinhoCliente_CarrinhoId",
                table: "CarrinhoItems",
                column: "CarrinhoId",
                principalTable: "CarrinhoCliente",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
