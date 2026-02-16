using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ETL.Migrations
{
    /// <inheritdoc />
    public partial class AddedRowIdentity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsSynced",
                table: "SaleReturnRecords",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "RowIdentity",
                table: "SaleReturnRecords",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SyncKey",
                table: "SaleReturnRecords",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "SyncedAt",
                table: "SaleReturnRecords",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RowIdentity",
                table: "SaleRecords",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsSynced",
                table: "ProductRecords",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "RowIdentity",
                table: "ProductRecords",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SyncKey",
                table: "ProductRecords",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "SyncedAt",
                table: "ProductRecords",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsSynced",
                table: "HesabRecords",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "RowIdentity",
                table: "HesabRecords",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SyncKey",
                table: "HesabRecords",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "SyncedAt",
                table: "HesabRecords",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsSynced",
                table: "CounterParties",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "RowIdentity",
                table: "CounterParties",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SyncKey",
                table: "CounterParties",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "SyncedAt",
                table: "CounterParties",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsSynced",
                table: "SaleReturnRecords");

            migrationBuilder.DropColumn(
                name: "RowIdentity",
                table: "SaleReturnRecords");

            migrationBuilder.DropColumn(
                name: "SyncKey",
                table: "SaleReturnRecords");

            migrationBuilder.DropColumn(
                name: "SyncedAt",
                table: "SaleReturnRecords");

            migrationBuilder.DropColumn(
                name: "RowIdentity",
                table: "SaleRecords");

            migrationBuilder.DropColumn(
                name: "IsSynced",
                table: "ProductRecords");

            migrationBuilder.DropColumn(
                name: "RowIdentity",
                table: "ProductRecords");

            migrationBuilder.DropColumn(
                name: "SyncKey",
                table: "ProductRecords");

            migrationBuilder.DropColumn(
                name: "SyncedAt",
                table: "ProductRecords");

            migrationBuilder.DropColumn(
                name: "IsSynced",
                table: "HesabRecords");

            migrationBuilder.DropColumn(
                name: "RowIdentity",
                table: "HesabRecords");

            migrationBuilder.DropColumn(
                name: "SyncKey",
                table: "HesabRecords");

            migrationBuilder.DropColumn(
                name: "SyncedAt",
                table: "HesabRecords");

            migrationBuilder.DropColumn(
                name: "IsSynced",
                table: "CounterParties");

            migrationBuilder.DropColumn(
                name: "RowIdentity",
                table: "CounterParties");

            migrationBuilder.DropColumn(
                name: "SyncKey",
                table: "CounterParties");

            migrationBuilder.DropColumn(
                name: "SyncedAt",
                table: "CounterParties");
        }
    }
}
