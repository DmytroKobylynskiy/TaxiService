using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Identity.Data.Migrations
{
    public partial class OwnerIDMigr : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OrderOwnerId",
                table: "TaxiOrders",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OfferOwnerId",
                table: "TaxiOffers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OrderOwnerId",
                table: "TaxiOrders");

            migrationBuilder.DropColumn(
                name: "OfferOwnerId",
                table: "TaxiOffers");
        }
    }
}
