using Microsoft.EntityFrameworkCore.Migrations;

namespace PowerMeasure.Migrations
{
    public partial class uem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EnergyMeterCode",
                table: "Meter",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EnergyMeterCode",
                table: "Meter");
        }
    }
}
