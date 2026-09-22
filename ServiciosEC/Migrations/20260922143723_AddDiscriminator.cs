using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServiciosEC.Migrations
{
    /// <inheritdoc />
    public partial class AddDiscriminator : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Agregar la columna como NULLABLE primero
            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "Personas",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            // 2. Poblar los valores
            migrationBuilder.Sql(@"
        UPDATE p
        SET p.Discriminator = 'Cliente'
        FROM Personas p
        INNER JOIN Clientes c ON p.id_persona = c.id_persona;
        ");

            migrationBuilder.Sql(@"
        UPDATE p
        SET p.Discriminator = 'Usuario'
        FROM Personas p
        INNER JOIN Usuarios u ON p.id_persona = u.id_persona;
        ");

            // 3. Los huérfanos (sin Cliente ni Usuario) → marcarlos como 'Usuario' por defecto
            //    (o borrarlos, depende de tu lógica de negocio)
            migrationBuilder.Sql(@"
        UPDATE Personas
        SET Discriminator = 'Usuario'
        WHERE Discriminator IS NULL;
         ");

            // 4. Hacerla NOT NULL
            migrationBuilder.AlterColumn<string>(
                name: "Discriminator",
                table: "Personas",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(8)",
                oldMaxLength: 8,
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "Personas");
        }
    }
}