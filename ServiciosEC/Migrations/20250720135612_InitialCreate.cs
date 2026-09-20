using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServiciosEC.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Estados",
                columns: table => new
                {
                    id_estado = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    descripcion = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Estados__86989FB2DB6BFC98", x => x.id_estado);
                });

            migrationBuilder.CreateTable(
                name: "EventosAgenda",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Titulo = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    FechaInicio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaFin = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TodoElDia = table.Column<bool>(type: "bit", nullable: false),
                    Color = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventosAgenda", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "IVA",
                columns: table => new
                {
                    IdIVA = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Descripcion = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Porcentaje = table.Column<decimal>(type: "decimal(5,3)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__IVA__0C143A82B0D52CCA", x => x.IdIVA);
                });

            migrationBuilder.CreateTable(
                name: "Jurisdicciones",
                columns: table => new
                {
                    id_jurisdiccion = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    descripcion = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Jurisdic__2B87043A6E7627D6", x => x.id_jurisdiccion);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    id_rol = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    descripcion = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Roles__6ABCB5E02351A66A", x => x.id_rol);
                });

            migrationBuilder.CreateTable(
                name: "Personas",
                columns: table => new
                {
                    id_persona = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nombre = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    apellido = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    dni = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    email = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    telefono = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    fecha_alta = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: false, defaultValueSql: "(getdate())"),
                    estado_id = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    rol_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Personas__228148B0F120C233", x => x.id_persona);
                    table.ForeignKey(
                        name: "FK__Personas__estado__603D47BB",
                        column: x => x.estado_id,
                        principalTable: "Estados",
                        principalColumn: "id_estado");
                    table.ForeignKey(
                        name: "FK__Personas__rol_id__61316BF4",
                        column: x => x.rol_id,
                        principalTable: "Roles",
                        principalColumn: "id_rol");
                });

            migrationBuilder.CreateTable(
                name: "Auditorias",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    tabla = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    accion = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    fecha = table.Column<DateTime>(type: "datetime2(0)", precision: 0, nullable: false, defaultValueSql: "(getdate())"),
                    id_persona = table.Column<int>(type: "int", nullable: true),
                    datos_antes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    datos_despues = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Auditori__3213E83FFDCBBAAC", x => x.id);
                    table.ForeignKey(
                        name: "FK_Auditorias_Personas_id_persona",
                        column: x => x.id_persona,
                        principalTable: "Personas",
                        principalColumn: "id_persona",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Clientes",
                columns: table => new
                {
                    id_persona = table.Column<int>(type: "int", nullable: false),
                    cuit = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    dom_fiscal = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    razon_social = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clientes", x => x.id_persona);
                    table.ForeignKey(
                        name: "FK_Clientes_Personas_id_persona",
                        column: x => x.id_persona,
                        principalTable: "Personas",
                        principalColumn: "id_persona",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Compras",
                columns: table => new
                {
                    id_compra = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_persona = table.Column<int>(type: "int", nullable: false),
                    fecha = table.Column<DateOnly>(type: "date", nullable: false),
                    tipo_fact = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    punto_venta = table.Column<int>(type: "int", nullable: true),
                    nro_desde = table.Column<int>(type: "int", nullable: false),
                    nro_hasta = table.Column<int>(type: "int", nullable: true),
                    tipo_doc_vendedor = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    nro_doc_vendedor = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    denom_vendedor = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    tipo_cambio = table.Column<int>(type: "int", nullable: true),
                    moneda = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    neto_gravado = table.Column<decimal>(type: "decimal(18,5)", nullable: true),
                    no_gravado = table.Column<decimal>(type: "decimal(18,5)", nullable: true),
                    exento = table.Column<decimal>(type: "decimal(18,5)", nullable: true),
                    iva = table.Column<decimal>(type: "decimal(18,5)", nullable: true),
                    total = table.Column<decimal>(type: "decimal(18,5)", nullable: true),
                    estado_id = table.Column<int>(type: "int", nullable: false, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Compras__C4BAA604E22D36F6", x => x.id_compra);
                    table.ForeignKey(
                        name: "FK__Compras__estado___1881A0DE",
                        column: x => x.estado_id,
                        principalTable: "Estados",
                        principalColumn: "id_estado");
                    table.ForeignKey(
                        name: "FK__Compras__id_pers__1975C517",
                        column: x => x.id_persona,
                        principalTable: "Personas",
                        principalColumn: "id_persona");
                });

            migrationBuilder.CreateTable(
                name: "INGRESOSBRUTOS",
                columns: table => new
                {
                    id_ingresosbrutos = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_persona = table.Column<int>(type: "int", nullable: false),
                    periodo = table.Column<int>(type: "int", nullable: false),
                    anio = table.Column<int>(type: "int", nullable: false),
                    jurisdiccion_id = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    coeficiente = table.Column<decimal>(type: "decimal(6,5)", nullable: false, defaultValue: 1m),
                    gravado_pais = table.Column<decimal>(type: "decimal(18,5)", nullable: true, defaultValue: 0m),
                    gravado_jurisdiccion = table.Column<decimal>(type: "decimal(18,5)", nullable: true, defaultValue: 0m),
                    alicuota = table.Column<decimal>(type: "decimal(5,4)", nullable: true, defaultValue: 0m),
                    impuesto_determinado = table.Column<decimal>(type: "decimal(18,5)", nullable: true, computedColumnSql: "(CONVERT([decimal](18,5),[gravado_jurisdiccion]*[alicuota]))", stored: true),
                    retenciones = table.Column<decimal>(type: "decimal(18,5)", nullable: true, defaultValue: 0m),
                    retenciones_bancarias = table.Column<decimal>(type: "decimal(18,5)", nullable: true, defaultValue: 0m),
                    percepciones = table.Column<decimal>(type: "decimal(18,5)", nullable: true, defaultValue: 0m),
                    aduaneras = table.Column<decimal>(type: "decimal(18,5)", nullable: true, defaultValue: 0m),
                    saldo = table.Column<decimal>(type: "decimal(18,5)", nullable: true, defaultValue: 0m)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__INGRESOS__1A12CAE131987BF6", x => x.id_ingresosbrutos);
                    table.ForeignKey(
                        name: "FK__INGRESOSB__id_pe__07C12930",
                        column: x => x.id_persona,
                        principalTable: "Personas",
                        principalColumn: "id_persona");
                    table.ForeignKey(
                        name: "FK__INGRESOSB__juris__08B54D69",
                        column: x => x.jurisdiccion_id,
                        principalTable: "Jurisdicciones",
                        principalColumn: "id_jurisdiccion");
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    id_persona = table.Column<int>(type: "int", nullable: false),
                    nombre_usuario = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    contrasenia = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.id_persona);
                    table.ForeignKey(
                        name: "FK_Usuarios_Personas_id_persona",
                        column: x => x.id_persona,
                        principalTable: "Personas",
                        principalColumn: "id_persona",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Ventas",
                columns: table => new
                {
                    id_venta = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    id_persona = table.Column<int>(type: "int", nullable: false),
                    fecha = table.Column<DateOnly>(type: "date", nullable: false),
                    tipo_fact = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    punto_venta = table.Column<int>(type: "int", nullable: true),
                    nro_desde = table.Column<int>(type: "int", nullable: false),
                    nro_hasta = table.Column<int>(type: "int", nullable: true),
                    tipo_doc_comprador = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    nro_doc_comprador = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    denom_comprador = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    tipo_cambio = table.Column<int>(type: "int", nullable: true),
                    moneda = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    neto_gravado = table.Column<decimal>(type: "decimal(18,5)", nullable: true),
                    no_gravado = table.Column<decimal>(type: "decimal(18,5)", nullable: true),
                    exento = table.Column<decimal>(type: "decimal(18,5)", nullable: true),
                    iva = table.Column<decimal>(type: "decimal(18,5)", nullable: true),
                    total = table.Column<decimal>(type: "decimal(18,5)", nullable: true),
                    estado_id = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    grav_0 = table.Column<decimal>(type: "decimal(18,5)", nullable: true, defaultValue: 0m),
                    grav_2_5 = table.Column<decimal>(type: "decimal(18,5)", nullable: true, defaultValue: 0m),
                    grav_5 = table.Column<decimal>(type: "decimal(18,5)", nullable: true, defaultValue: 0m),
                    grav_10_5 = table.Column<decimal>(type: "decimal(18,5)", nullable: true, defaultValue: 0m),
                    grav_21 = table.Column<decimal>(type: "decimal(18,5)", nullable: true, defaultValue: 0m),
                    grav_27 = table.Column<decimal>(type: "decimal(18,5)", nullable: true, defaultValue: 0m),
                    iva_0 = table.Column<decimal>(type: "decimal(18,5)", nullable: true, defaultValue: 0m),
                    iva_2_5 = table.Column<decimal>(type: "decimal(18,5)", nullable: true, defaultValue: 0m),
                    iva_5 = table.Column<decimal>(type: "decimal(18,5)", nullable: true, defaultValue: 0m),
                    iva_10_5 = table.Column<decimal>(type: "decimal(18,5)", nullable: true, defaultValue: 0m),
                    iva_21 = table.Column<decimal>(type: "decimal(18,5)", nullable: true, defaultValue: 0m),
                    iva_27 = table.Column<decimal>(type: "decimal(18,5)", nullable: true, defaultValue: 0m)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Ventas__459533BF093E752F", x => x.id_venta);
                    table.ForeignKey(
                        name: "FK__Ventas__estado_i__3A81B327",
                        column: x => x.estado_id,
                        principalTable: "Estados",
                        principalColumn: "id_estado");
                    table.ForeignKey(
                        name: "FK__Ventas__id_perso__3B75D760",
                        column: x => x.id_persona,
                        principalTable: "Personas",
                        principalColumn: "id_persona");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Auditorias_id_persona",
                table: "Auditorias",
                column: "id_persona");

            migrationBuilder.CreateIndex(
                name: "UQ_Clientes_CUIT",
                table: "Clientes",
                column: "cuit",
                unique: true,
                filter: "[cuit] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Compras_estado_id",
                table: "Compras",
                column: "estado_id");

            migrationBuilder.CreateIndex(
                name: "IX_Compras_id_persona",
                table: "Compras",
                column: "id_persona");

            migrationBuilder.CreateIndex(
                name: "IX_INGRESOSBRUTOS_id_persona",
                table: "INGRESOSBRUTOS",
                column: "id_persona");

            migrationBuilder.CreateIndex(
                name: "IX_INGRESOSBRUTOS_jurisdiccion_id",
                table: "INGRESOSBRUTOS",
                column: "jurisdiccion_id");

            migrationBuilder.CreateIndex(
                name: "IX_Personas_estado_id",
                table: "Personas",
                column: "estado_id");

            migrationBuilder.CreateIndex(
                name: "IX_Personas_rol_id",
                table: "Personas",
                column: "rol_id");

            migrationBuilder.CreateIndex(
                name: "UQ__Personas__email",
                table: "Personas",
                column: "email",
                unique: true,
                filter: "([email] IS NOT NULL)");

            migrationBuilder.CreateIndex(
                name: "UQ_Personas_dni",
                table: "Personas",
                column: "dni",
                unique: true,
                filter: "([dni] IS NOT NULL)");

            migrationBuilder.CreateIndex(
                name: "IX_Ventas_estado_id",
                table: "Ventas",
                column: "estado_id");

            migrationBuilder.CreateIndex(
                name: "IX_Ventas_id_persona",
                table: "Ventas",
                column: "id_persona");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Auditorias");

            migrationBuilder.DropTable(
                name: "Clientes");

            migrationBuilder.DropTable(
                name: "Compras");

            migrationBuilder.DropTable(
                name: "EventosAgenda");

            migrationBuilder.DropTable(
                name: "INGRESOSBRUTOS");

            migrationBuilder.DropTable(
                name: "IVA");

            migrationBuilder.DropTable(
                name: "Usuarios");

            migrationBuilder.DropTable(
                name: "Ventas");

            migrationBuilder.DropTable(
                name: "Jurisdicciones");

            migrationBuilder.DropTable(
                name: "Personas");

            migrationBuilder.DropTable(
                name: "Estados");

            migrationBuilder.DropTable(
                name: "Roles");
        }
    }
}
