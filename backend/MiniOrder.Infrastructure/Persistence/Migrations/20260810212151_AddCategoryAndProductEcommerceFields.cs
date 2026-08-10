using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniOrder.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddCategoryAndProductEcommerceFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table
                        .Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Slug = table.Column<string>(type: "TEXT", maxLength: 120, nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                }
            );

            migrationBuilder.CreateIndex(
                name: "IX_Categories_Slug",
                table: "Categories",
                column: "Slug",
                unique: true
            );

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "Name", "Slug", "IsActive" },
                values: new object[,]
                {
                    { 1, "Computers", "computers", true },
                    { 2, "Accessories", "accessories", true },
                    { 3, "Monitors", "monitors", true },
                    { 4, "Audio", "audio", true },
                }
            );

            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "Products",
                type: "INTEGER",
                nullable: true
            );

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Products",
                type: "TEXT",
                maxLength: 1000,
                nullable: false,
                defaultValue: ""
            );

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Products",
                type: "INTEGER",
                nullable: false,
                defaultValue: true
            );

            migrationBuilder.Sql(
                """
                UPDATE Products
                SET CategoryId = 1,
                    Description = 'High-performance laptop for everyday work and productivity.'
                WHERE StockCode = 'ELC-LPT-001';
                """
            );

            migrationBuilder.Sql(
                """
                UPDATE Products
                SET CategoryId = 2,
                    Description = 'Wireless mouse designed for comfortable everyday use.'
                WHERE StockCode = 'ACC-MSE-002';
                """
            );

            migrationBuilder.Sql(
                """
                UPDATE Products
                SET CategoryId = 2,
                    Description = 'Mechanical keyboard designed for productivity and gaming.'
                WHERE StockCode = 'ACC-KEY-003';
                """
            );

            migrationBuilder.Sql(
                """
                UPDATE Products
                SET CategoryId = 3,
                    Description = '27-inch monitor suitable for work and entertainment.'
                WHERE StockCode = 'ELC-MON-004';
                """
            );

            migrationBuilder.Sql(
                """
                UPDATE Products
                SET CategoryId = 4,
                    Description = 'USB headset with microphone for calls and everyday use.'
                WHERE StockCode = 'ACC-HDS-005';
                """
            );

            migrationBuilder.AlterColumn<int>(
                name: "CategoryId",
                table: "Products",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_Products_CategoryId",
                table: "Products",
                column: "CategoryId"
            );

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Categories_CategoryId",
                table: "Products",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_Categories_CategoryId",
                table: "Products"
            );

            migrationBuilder.DropTable(name: "Categories");

            migrationBuilder.DropIndex(name: "IX_Products_CategoryId", table: "Products");

            migrationBuilder.DropColumn(name: "CategoryId", table: "Products");

            migrationBuilder.DropColumn(name: "Description", table: "Products");

            migrationBuilder.DropColumn(name: "IsActive", table: "Products");
        }
    }
}
