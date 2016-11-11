using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Identity.Data.Migrations
{
    public partial class InitialOrderUp : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Company",
                table: "TaxiOrders");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "TaxiOrders");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "TaxiOrders");

            migrationBuilder.AddColumn<string>(
                name: "Date",
                table: "TaxiOrders",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EndPoint",
                table: "TaxiOrders",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PassengerName",
                table: "TaxiOrders",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PassengerPhone",
                table: "TaxiOrders",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StartPoint",
                table: "TaxiOrders",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Time",
                table: "TaxiOrders",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Date",
                table: "TaxiOrders");

            migrationBuilder.DropColumn(
                name: "EndPoint",
                table: "TaxiOrders");

            migrationBuilder.DropColumn(
                name: "PassengerName",
                table: "TaxiOrders");

            migrationBuilder.DropColumn(
                name: "PassengerPhone",
                table: "TaxiOrders");

            migrationBuilder.DropColumn(
                name: "StartPoint",
                table: "TaxiOrders");

            migrationBuilder.DropColumn(
                name: "Time",
                table: "TaxiOrders");

            migrationBuilder.AddColumn<string>(
                name: "Company",
                table: "TaxiOrders",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "TaxiOrders",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Price",
                table: "TaxiOrders",
                nullable: false,
                defaultValue: 0);
        }
    }
}
