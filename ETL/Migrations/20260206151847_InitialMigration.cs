using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ETL.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CounterParties",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    HesabID = table.Column<string>(type: "text", nullable: true),
                    CounterPartyCode = table.Column<string>(type: "text", nullable: true),
                    FullName = table.Column<string>(type: "text", nullable: true),
                    HesabAnbar = table.Column<string>(type: "text", nullable: true),
                    CustomerType = table.Column<string>(type: "text", nullable: true),
                    CustomerName = table.Column<string>(type: "text", nullable: true),
                    CusotmerFamily = table.Column<string>(type: "text", nullable: true),
                    CustomerCode = table.Column<string>(type: "text", nullable: true),
                    CustomerIdentiifre = table.Column<string>(type: "text", nullable: true),
                    CustomerTel = table.Column<string>(type: "text", nullable: true),
                    CustomerPhoneNumber = table.Column<string>(type: "text", nullable: true),
                    CustomerPelak = table.Column<string>(type: "text", nullable: true),
                    CustomerAddress = table.Column<string>(type: "text", nullable: true),
                    CustomerBirthDay = table.Column<string>(type: "text", nullable: true),
                    Active = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CounterParties", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HesabRecords",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Code = table.Column<string>(type: "text", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Type = table.Column<string>(type: "text", nullable: true),
                    Bedehkar = table.Column<string>(type: "text", nullable: true),
                    Bestankar = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HesabRecords", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductRecords",
                columns: table => new
                {
                    ID = table.Column<Guid>(type: "uuid", nullable: false),
                    RHActionDesc = table.Column<string>(type: "text", nullable: true),
                    Type = table.Column<string>(type: "text", nullable: true),
                    DetailedType = table.Column<string>(type: "text", nullable: true),
                    Anbar = table.Column<string>(type: "text", nullable: true),
                    KalaCode = table.Column<string>(type: "text", nullable: true),
                    KDesc = table.Column<string>(type: "text", nullable: true),
                    FactorNum = table.Column<string>(type: "text", nullable: true),
                    UnitValue1 = table.Column<string>(type: "text", nullable: true),
                    UnitValue2 = table.Column<string>(type: "text", nullable: true),
                    UnitValue3 = table.Column<string>(type: "text", nullable: true),
                    Fee = table.Column<string>(type: "text", nullable: true),
                    Price = table.Column<string>(type: "text", nullable: true),
                    RHDesc = table.Column<string>(type: "text", nullable: true),
                    Date = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductRecords", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "SaleRecords",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SandID = table.Column<string>(type: "text", nullable: true),
                    DocNum = table.Column<string>(type: "text", nullable: true),
                    AccountCode = table.Column<string>(type: "text", nullable: true),
                    Article = table.Column<string>(type: "text", nullable: true),
                    HavaleCode = table.Column<string>(type: "text", nullable: true),
                    Type = table.Column<string>(type: "text", nullable: true),
                    Date = table.Column<string>(type: "text", nullable: true),
                    KalaCode = table.Column<string>(type: "text", nullable: true),
                    KalDelimiter = table.Column<string>(type: "text", nullable: true),
                    KDesc = table.Column<string>(type: "text", nullable: true),
                    UnitCode1 = table.Column<string>(type: "text", nullable: true),
                    UnitValue1 = table.Column<string>(type: "text", nullable: true),
                    UnitCode2 = table.Column<string>(type: "text", nullable: true),
                    UnitValue2 = table.Column<string>(type: "text", nullable: true),
                    UnitCode3 = table.Column<string>(type: "text", nullable: true),
                    UnitValue3 = table.Column<string>(type: "text", nullable: true),
                    Price = table.Column<string>(type: "text", nullable: true),
                    TarafHesab = table.Column<string>(type: "text", nullable: true),
                    CustomerType = table.Column<string>(type: "text", nullable: true),
                    CustomerAccount = table.Column<string>(type: "text", nullable: true),
                    CustomerName = table.Column<string>(type: "text", nullable: true),
                    CusotmerFamily = table.Column<string>(type: "text", nullable: true),
                    CustomerCode = table.Column<string>(type: "text", nullable: true),
                    CustomerIdentiifre = table.Column<string>(type: "text", nullable: true),
                    CustomerTel = table.Column<string>(type: "text", nullable: true),
                    CustomerPhoneNumber = table.Column<string>(type: "text", nullable: true),
                    CustomerPelak = table.Column<string>(type: "text", nullable: true),
                    CustomerAddress = table.Column<string>(type: "text", nullable: true),
                    CustomerBirthDay = table.Column<string>(type: "text", nullable: true),
                    Active = table.Column<string>(type: "text", nullable: true),
                    DetailedType = table.Column<string>(type: "text", nullable: true),
                    SyncKey = table.Column<string>(type: "text", nullable: false),
                    IsSynced = table.Column<bool>(type: "boolean", nullable: false),
                    SyncedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SaleRecords", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SaleReturnRecords",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SandID = table.Column<string>(type: "text", nullable: true),
                    DocNum = table.Column<string>(type: "text", nullable: true),
                    AccountCode = table.Column<string>(type: "text", nullable: true),
                    Article = table.Column<string>(type: "text", nullable: true),
                    HavaleCode = table.Column<string>(type: "text", nullable: true),
                    Type = table.Column<string>(type: "text", nullable: true),
                    Date = table.Column<string>(type: "text", nullable: true),
                    KalaCode = table.Column<string>(type: "text", nullable: true),
                    KalDelimiter = table.Column<string>(type: "text", nullable: true),
                    KDesc = table.Column<string>(type: "text", nullable: true),
                    UnitCode1 = table.Column<string>(type: "text", nullable: true),
                    UnitValue1 = table.Column<string>(type: "text", nullable: true),
                    UnitCode2 = table.Column<string>(type: "text", nullable: true),
                    UnitValue2 = table.Column<string>(type: "text", nullable: true),
                    UnitCode3 = table.Column<string>(type: "text", nullable: true),
                    UnitValue3 = table.Column<string>(type: "text", nullable: true),
                    Price = table.Column<string>(type: "text", nullable: true),
                    TarafHesab = table.Column<string>(type: "text", nullable: true),
                    CustomerType = table.Column<string>(type: "text", nullable: true),
                    CustomerAccount = table.Column<string>(type: "text", nullable: true),
                    CustomerName = table.Column<string>(type: "text", nullable: true),
                    CusotmerFamily = table.Column<string>(type: "text", nullable: true),
                    CustomerCode = table.Column<string>(type: "text", nullable: true),
                    CustomerIdentiifre = table.Column<string>(type: "text", nullable: true),
                    CustomerTel = table.Column<string>(type: "text", nullable: true),
                    CustomerPhoneNumber = table.Column<string>(type: "text", nullable: true),
                    CustomerPelak = table.Column<string>(type: "text", nullable: true),
                    CustomerAddress = table.Column<string>(type: "text", nullable: true),
                    CustomerBirthDay = table.Column<string>(type: "text", nullable: true),
                    Active = table.Column<string>(type: "text", nullable: true),
                    DetailedType = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SaleReturnRecords", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CounterParties");

            migrationBuilder.DropTable(
                name: "HesabRecords");

            migrationBuilder.DropTable(
                name: "ProductRecords");

            migrationBuilder.DropTable(
                name: "SaleRecords");

            migrationBuilder.DropTable(
                name: "SaleReturnRecords");
        }
    }
}
