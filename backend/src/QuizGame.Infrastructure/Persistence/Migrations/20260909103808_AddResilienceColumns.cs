using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuizGame.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddResilienceColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SerializedResponse",
                schema: "dbo",
                table: "IdempotencyRecords",
                newName: "ResponsePayload");

            migrationBuilder.RenameColumn(
                name: "RequestName",
                schema: "dbo",
                table: "IdempotencyRecords",
                newName: "CommandName");

            migrationBuilder.AddColumn<int>(
                name: "AttemptCount",
                schema: "dbo",
                table: "OutboxMessages",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpiresOnUtc",
                schema: "dbo",
                table: "IdempotencyRecords",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "ResourceId",
                schema: "dbo",
                table: "IdempotencyRecords",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StatusCode",
                schema: "dbo",
                table: "IdempotencyRecords",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_IdempotencyRecords_ExpiresOnUtc",
                schema: "dbo",
                table: "IdempotencyRecords",
                column: "ExpiresOnUtc");

            migrationBuilder.CreateIndex(
                name: "UQ_IdempotencyRecords_RequestId",
                schema: "dbo",
                table: "IdempotencyRecords",
                column: "RequestId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_IdempotencyRecords_ExpiresOnUtc",
                schema: "dbo",
                table: "IdempotencyRecords");

            migrationBuilder.DropIndex(
                name: "UQ_IdempotencyRecords_RequestId",
                schema: "dbo",
                table: "IdempotencyRecords");

            migrationBuilder.DropColumn(
                name: "AttemptCount",
                schema: "dbo",
                table: "OutboxMessages");

            migrationBuilder.DropColumn(
                name: "ExpiresOnUtc",
                schema: "dbo",
                table: "IdempotencyRecords");

            migrationBuilder.DropColumn(
                name: "ResourceId",
                schema: "dbo",
                table: "IdempotencyRecords");

            migrationBuilder.DropColumn(
                name: "StatusCode",
                schema: "dbo",
                table: "IdempotencyRecords");

            migrationBuilder.RenameColumn(
                name: "ResponsePayload",
                schema: "dbo",
                table: "IdempotencyRecords",
                newName: "SerializedResponse");

            migrationBuilder.RenameColumn(
                name: "CommandName",
                schema: "dbo",
                table: "IdempotencyRecords",
                newName: "RequestName");
        }
    }
}
