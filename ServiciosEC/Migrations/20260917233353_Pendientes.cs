using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServiciosEC.Migrations
{
    /// <inheritdoc />
    public partial class Pendientes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK__Compras__estado___1881A0DE",
                table: "Compras");

            migrationBuilder.DropForeignKey(
                name: "FK__Compras__id_pers__1975C517",
                table: "Compras");

            migrationBuilder.DropPrimaryKey(
                name: "PK__Compras__C4BAA604E22D36F6",
                table: "Compras");

            migrationBuilder.AddColumn<DateTime>(
                name: "fecha_declaracion",
                table: "INGRESOSBRUTOS",
                type: "datetime2(0)",
                precision: 0,
                nullable: false,
                defaultValueSql: "(getdate())");

            migrationBuilder.AddColumn<decimal>(
                name: "grav_0",
                table: "Compras",
                type: "decimal(18,5)",
                nullable: true,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "grav_10_5",
                table: "Compras",
                type: "decimal(18,5)",
                nullable: true,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "grav_21",
                table: "Compras",
                type: "decimal(18,5)",
                nullable: true,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "grav_27",
                table: "Compras",
                type: "decimal(18,5)",
                nullable: true,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "grav_2_5",
                table: "Compras",
                type: "decimal(18,5)",
                nullable: true,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "grav_5",
                table: "Compras",
                type: "decimal(18,5)",
                nullable: true,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "iva_0",
                table: "Compras",
                type: "decimal(18,5)",
                nullable: true,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "iva_10_5",
                table: "Compras",
                type: "decimal(18,5)",
                nullable: true,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "iva_21",
                table: "Compras",
                type: "decimal(18,5)",
                nullable: true,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "iva_27",
                table: "Compras",
                type: "decimal(18,5)",
                nullable: true,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "iva_2_5",
                table: "Compras",
                type: "decimal(18,5)",
                nullable: true,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "iva_5",
                table: "Compras",
                type: "decimal(18,5)",
                nullable: true,
                defaultValue: 0m);

            migrationBuilder.AddPrimaryKey(
                name: "PK__Compras__C4BAA604464DD8E7",
                table: "Compras",
                column: "id_compra");

            migrationBuilder.CreateTable(
                name: "LibroIva",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_persona = table.Column<int>(type: "int", nullable: true),
                    Cuit = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                    Mes = table.Column<int>(type: "int", nullable: true),
                    Año = table.Column<int>(type: "int", nullable: true),
                    Debito_Neto27 = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Debito_Neto21 = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Debito_Neto105 = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Debito_Iva27 = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Debito_Iva21 = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Debito_Iva105 = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Debito_NoGravado = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Debito_Exento = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Debito_NetoOtros = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Debito_IvaOtros = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    RestDebito_Neto27 = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    RestDebito_Neto21 = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    RestDebito_Neto105 = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    RestDebito_Iva27 = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    RestDebito_Iva21 = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    RestDebito_Iva105 = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    RestDebito_NoGravado = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    RestDebito_Exento = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    RestDebito_NetoOtros = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    RestDebito_IvaOtros = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Credito_Neto27 = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Credito_Neto21 = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Credito_Neto105 = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Credito_Iva27 = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Credito_Iva21 = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Credito_Iva105 = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Credito_NoGravado = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Credito_Exento = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Credito_NetoOtros = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Credito_IvaOtros = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    RestCredito_Neto27 = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    RestCredito_Neto21 = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    RestCredito_Neto105 = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    RestCredito_Iva27 = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    RestCredito_Iva21 = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    RestCredito_Iva105 = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    RestCredito_NoGravado = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    RestCredito_Exento = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    RestCredito_NetoOtros = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    RestCredito_IvaOtros = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    SaldoTecnicoAnterior = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    SaldoLibreDisponibilidad = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    RetencionesIVA = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PercepcionesIVA = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    GravadoDebitoFical = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IvaDebitoFiscal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    GravadoCreditoFiscal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IvaCreditoFiscal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SaldoTecnico = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SaldoTecnicoNeto = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    FechaDeclaracion = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: false, defaultValueSql: "(getdate())"),
                    Debito_Neto5 = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Debito_Neto25 = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Debito_Neto0 = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Debito_Iva5 = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Debito_Iva25 = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Debito_Iva0 = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    RestDebito_Neto5 = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    RestDebito_Neto25 = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    RestDebito_Neto0 = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    RestDebito_Iva5 = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    RestDebito_Iva25 = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    RestDebito_Iva0 = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Credito_Neto5 = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Credito_Neto25 = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Credito_Neto0 = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Credito_Iva5 = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Credito_Iva25 = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Credito_Iva0 = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    RestCredito_Neto5 = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    RestCredito_Neto25 = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    RestCredito_Neto0 = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    RestCredito_Iva5 = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    RestCredito_Iva25 = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    RestCredito_Iva0 = table.Column<decimal>(type: "decimal(18,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__LibroIva__3214EC073CF982F2", x => x.Id);
                    table.ForeignKey(
                        name: "FK__LibroIva__id_per__14270015",
                        column: x => x.id_persona,
                        principalTable: "Personas",
                        principalColumn: "id_persona");
                });

            migrationBuilder.CreateIndex(
                name: "IX_LibroIva_id_persona",
                table: "LibroIva",
                column: "id_persona");

            migrationBuilder.AddForeignKey(
                name: "FK__Compras__estado___3F466844",
                table: "Compras",
                column: "estado_id",
                principalTable: "Estados",
                principalColumn: "id_estado");

            migrationBuilder.AddForeignKey(
                name: "FK__Compras__id_pers__403A8C7D",
                table: "Compras",
                column: "id_persona",
                principalTable: "Personas",
                principalColumn: "id_persona");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK__Compras__estado___3F466844",
                table: "Compras");

            migrationBuilder.DropForeignKey(
                name: "FK__Compras__id_pers__403A8C7D",
                table: "Compras");

            migrationBuilder.DropTable(
                name: "LibroIva");

            migrationBuilder.DropPrimaryKey(
                name: "PK__Compras__C4BAA604464DD8E7",
                table: "Compras");

            migrationBuilder.DropColumn(
                name: "fecha_declaracion",
                table: "INGRESOSBRUTOS");

            migrationBuilder.DropColumn(
                name: "grav_0",
                table: "Compras");

            migrationBuilder.DropColumn(
                name: "grav_10_5",
                table: "Compras");

            migrationBuilder.DropColumn(
                name: "grav_21",
                table: "Compras");

            migrationBuilder.DropColumn(
                name: "grav_27",
                table: "Compras");

            migrationBuilder.DropColumn(
                name: "grav_2_5",
                table: "Compras");

            migrationBuilder.DropColumn(
                name: "grav_5",
                table: "Compras");

            migrationBuilder.DropColumn(
                name: "iva_0",
                table: "Compras");

            migrationBuilder.DropColumn(
                name: "iva_10_5",
                table: "Compras");

            migrationBuilder.DropColumn(
                name: "iva_21",
                table: "Compras");

            migrationBuilder.DropColumn(
                name: "iva_27",
                table: "Compras");

            migrationBuilder.DropColumn(
                name: "iva_2_5",
                table: "Compras");

            migrationBuilder.DropColumn(
                name: "iva_5",
                table: "Compras");

            migrationBuilder.AddPrimaryKey(
                name: "PK__Compras__C4BAA604E22D36F6",
                table: "Compras",
                column: "id_compra");

            migrationBuilder.AddForeignKey(
                name: "FK__Compras__estado___1881A0DE",
                table: "Compras",
                column: "estado_id",
                principalTable: "Estados",
                principalColumn: "id_estado");

            migrationBuilder.AddForeignKey(
                name: "FK__Compras__id_pers__1975C517",
                table: "Compras",
                column: "id_persona",
                principalTable: "Personas",
                principalColumn: "id_persona");
        }
    }
}
