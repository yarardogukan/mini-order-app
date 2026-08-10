using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniOrder.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class EnrichProductCatalog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BrandId",
                table: "Products",
                type: "INTEGER",
                nullable: true
            );

            migrationBuilder.AddColumn<int>(
                name: "ParentCategoryId",
                table: "Categories",
                type: "INTEGER",
                nullable: true
            );

            migrationBuilder.CreateTable(
                name: "Brands",
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
                    table.PrimaryKey("PK_Brands", x => x.Id);
                }
            );

            migrationBuilder.CreateTable(
                name: "CategoryAttributes",
                columns: table => new
                {
                    Id = table
                        .Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CategoryId = table.Column<int>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Code = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    DataType = table.Column<string>(type: "TEXT", maxLength: 30, nullable: false),
                    IsRequired = table.Column<bool>(type: "INTEGER", nullable: false),
                    SortOrder = table.Column<int>(type: "INTEGER", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoryAttributes", x => x.Id);

                    table.ForeignKey(
                        name: "FK_CategoryAttributes_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "ProductImages",
                columns: table => new
                {
                    Id = table
                        .Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ProductId = table.Column<int>(type: "INTEGER", nullable: false),
                    ImageUrl = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    IsCover = table.Column<bool>(type: "INTEGER", nullable: false),
                    SortOrder = table.Column<int>(type: "INTEGER", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductImages", x => x.Id);

                    table.ForeignKey(
                        name: "FK_ProductImages_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "ProductAttributeValues",
                columns: table => new
                {
                    Id = table
                        .Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ProductId = table.Column<int>(type: "INTEGER", nullable: false),
                    CategoryAttributeId = table.Column<int>(type: "INTEGER", nullable: false),
                    Value = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductAttributeValues", x => x.Id);

                    table.ForeignKey(
                        name: "FK_ProductAttributeValues_CategoryAttributes_CategoryAttributeId",
                        column: x => x.CategoryAttributeId,
                        principalTable: "CategoryAttributes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict
                    );

                    table.ForeignKey(
                        name: "FK_ProductAttributeValues_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.InsertData(
                table: "Brands",
                columns: new[] { "Id", "Name", "Slug", "IsActive" },
                values: new object[,]
                {
                    { 1, "Dell", "dell", true },
                    { 2, "Logitech", "logitech", true },
                    { 3, "Keychron", "keychron", true },
                    { 4, "Samsung", "samsung", true },
                    { 5, "Jabra", "jabra", true },
                }
            );

            migrationBuilder.Sql(
                """
                INSERT INTO Categories (Name, Slug, IsActive, ParentCategoryId)
                SELECT 'Laptops', 'laptops', 1, Id
                FROM Categories
                WHERE Slug = 'computers'
                  AND NOT EXISTS (
                      SELECT 1
                      FROM Categories
                      WHERE Slug = 'laptops'
                  );
                """
            );

            migrationBuilder.Sql(
                """
                INSERT INTO Categories (Name, Slug, IsActive, ParentCategoryId)
                SELECT 'Mouse', 'mouse', 1, Id
                FROM Categories
                WHERE Slug = 'accessories'
                  AND NOT EXISTS (
                      SELECT 1
                      FROM Categories
                      WHERE Slug = 'mouse'
                  );
                """
            );

            migrationBuilder.Sql(
                """
                INSERT INTO Categories (Name, Slug, IsActive, ParentCategoryId)
                SELECT 'Keyboards', 'keyboards', 1, Id
                FROM Categories
                WHERE Slug = 'accessories'
                  AND NOT EXISTS (
                      SELECT 1
                      FROM Categories
                      WHERE Slug = 'keyboards'
                  );
                """
            );

            migrationBuilder.Sql(
                """
                INSERT INTO Categories (Name, Slug, IsActive, ParentCategoryId)
                SELECT 'Computer Monitors', 'computer-monitors', 1, Id
                FROM Categories
                WHERE Slug = 'monitors'
                  AND NOT EXISTS (
                      SELECT 1
                      FROM Categories
                      WHERE Slug = 'computer-monitors'
                  );
                """
            );

            migrationBuilder.Sql(
                """
                INSERT INTO Categories (Name, Slug, IsActive, ParentCategoryId)
                SELECT 'Headsets', 'headsets', 1, Id
                FROM Categories
                WHERE Slug = 'audio'
                  AND NOT EXISTS (
                      SELECT 1
                      FROM Categories
                      WHERE Slug = 'headsets'
                  );
                """
            );

            migrationBuilder.Sql(
                """
                UPDATE Products
                SET CategoryId = (
                        SELECT Id
                        FROM Categories
                        WHERE Slug = 'laptops'
                    ),
                    BrandId = (
                        SELECT Id
                        FROM Brands
                        WHERE Slug = 'dell'
                    )
                WHERE StockCode = 'ELC-LPT-001';
                """
            );

            migrationBuilder.Sql(
                """
                UPDATE Products
                SET CategoryId = (
                        SELECT Id
                        FROM Categories
                        WHERE Slug = 'mouse'
                    ),
                    BrandId = (
                        SELECT Id
                        FROM Brands
                        WHERE Slug = 'logitech'
                    )
                WHERE StockCode = 'ACC-MSE-002';
                """
            );

            migrationBuilder.Sql(
                """
                UPDATE Products
                SET CategoryId = (
                        SELECT Id
                        FROM Categories
                        WHERE Slug = 'keyboards'
                    ),
                    BrandId = (
                        SELECT Id
                        FROM Brands
                        WHERE Slug = 'keychron'
                    )
                WHERE StockCode = 'ACC-KEY-003';
                """
            );

            migrationBuilder.Sql(
                """
                UPDATE Products
                SET CategoryId = (
                        SELECT Id
                        FROM Categories
                        WHERE Slug = 'computer-monitors'
                    ),
                    BrandId = (
                        SELECT Id
                        FROM Brands
                        WHERE Slug = 'samsung'
                    )
                WHERE StockCode = 'ELC-MON-004';
                """
            );

            migrationBuilder.Sql(
                """
                UPDATE Products
                SET CategoryId = (
                        SELECT Id
                        FROM Categories
                        WHERE Slug = 'headsets'
                    ),
                    BrandId = (
                        SELECT Id
                        FROM Brands
                        WHERE Slug = 'jabra'
                    )
                WHERE StockCode = 'ACC-HDS-005';
                """
            );

            migrationBuilder.AlterColumn<int>(
                name: "BrandId",
                table: "Products",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_Products_BrandId",
                table: "Products",
                column: "BrandId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_Categories_ParentCategoryId",
                table: "Categories",
                column: "ParentCategoryId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_Brands_Slug",
                table: "Brands",
                column: "Slug",
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_CategoryAttributes_CategoryId_Code",
                table: "CategoryAttributes",
                columns: new[] { "CategoryId", "Code" },
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_ProductAttributeValues_CategoryAttributeId",
                table: "ProductAttributeValues",
                column: "CategoryAttributeId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_ProductAttributeValues_ProductId_CategoryAttributeId",
                table: "ProductAttributeValues",
                columns: new[] { "ProductId", "CategoryAttributeId" },
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_ProductImages_ProductId",
                table: "ProductImages",
                column: "ProductId"
            );

            migrationBuilder.AddForeignKey(
                name: "FK_Categories_Categories_ParentCategoryId",
                table: "Categories",
                column: "ParentCategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict
            );

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Brands_BrandId",
                table: "Products",
                column: "BrandId",
                principalTable: "Brands",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Products -> root categories
            migrationBuilder.Sql(
                """
                UPDATE Products
                SET CategoryId = (
                    SELECT Id
                    FROM Categories
                    WHERE Slug = 'computers'
                )
                WHERE StockCode = 'ELC-LPT-001';
                """
            );

            migrationBuilder.Sql(
                """
                UPDATE Products
                SET CategoryId = (
                    SELECT Id
                    FROM Categories
                    WHERE Slug = 'accessories'
                )
                WHERE StockCode = 'ACC-MSE-002';
                """
            );

            migrationBuilder.Sql(
                """
                UPDATE Products
                SET CategoryId = (
                    SELECT Id
                    FROM Categories
                    WHERE Slug = 'accessories'
                )
                WHERE StockCode = 'ACC-KEY-003';
                """
            );

            migrationBuilder.Sql(
                """
                UPDATE Products
                SET CategoryId = (
                    SELECT Id
                    FROM Categories
                    WHERE Slug = 'monitors'
                )
                WHERE StockCode = 'ELC-MON-004';
                """
            );

            migrationBuilder.Sql(
                """
                UPDATE Products
                SET CategoryId = (
                    SELECT Id
                    FROM Categories
                    WHERE Slug = 'audio'
                )
                WHERE StockCode = 'ACC-HDS-005';
                """
            );

            // Remove new relationships
            migrationBuilder.DropForeignKey(
                name: "FK_Categories_Categories_ParentCategoryId",
                table: "Categories"
            );

            migrationBuilder.DropForeignKey(name: "FK_Products_Brands_BrandId", table: "Products");

            // Dynamic product data first
            migrationBuilder.DropTable(name: "ProductAttributeValues");

            migrationBuilder.DropTable(name: "ProductImages");

            migrationBuilder.DropTable(name: "CategoryAttributes");

            migrationBuilder.DropIndex(name: "IX_Products_BrandId", table: "Products");

            migrationBuilder.DropIndex(name: "IX_Categories_ParentCategoryId", table: "Categories");

            // Remove categories introduced by this migration
            migrationBuilder.Sql(
                """
                DELETE FROM Categories
                WHERE Slug IN (
                    'laptops',
                    'mouse',
                    'keyboards',
                    'computer-monitors',
                    'headsets'
                );
                """
            );

            // BrandId no longer needed
            migrationBuilder.DropColumn(name: "BrandId", table: "Products");

            migrationBuilder.DropTable(name: "Brands");

            migrationBuilder.DropColumn(name: "ParentCategoryId", table: "Categories");
        }
    }
}
