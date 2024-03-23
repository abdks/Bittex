using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace test.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Value",
                table: "CryptoCurrencies",
                newName: "Deger");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "CryptoCurrencies",
                newName: "ParaCinsi");

            migrationBuilder.AddColumn<string>(
                name: "KriptoPara",
                table: "CryptoCurrencies",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "Tarih",
                table: "CryptoCurrencies",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "KriptoPara",
                table: "CryptoCurrencies");

            migrationBuilder.DropColumn(
                name: "Tarih",
                table: "CryptoCurrencies");

            migrationBuilder.RenameColumn(
                name: "ParaCinsi",
                table: "CryptoCurrencies",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "Deger",
                table: "CryptoCurrencies",
                newName: "Value");
        }
    }
}
