using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrderAPI.Migrations
{
    /// <inheritdoc />
    public partial class FixUUIDGeneration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Order",
                type: "uuid",
                nullable: false,
                defaultValueSql: "(gen_random_uuid())",
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValueSql: "(newid())");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Order",
                type: "uuid",
                nullable: false,
                defaultValueSql: "(newid())",
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValueSql: "(gen_random_uuid())");
        }
    }
}
