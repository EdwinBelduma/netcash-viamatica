using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NetCash.Infraestructura.Persistencia.Migraciones
{
    /// <inheritdoc />
    public partial class AgregarFuncionTurnosAtendidos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // ==========================================
            // CREAR FUNCION DE TURNOS ATENDIDOS
            // ==========================================

            migrationBuilder.Sql(
                """
                CREATE OR REPLACE FUNCTION fn_turnos_atendidos(
                    p_desde TIMESTAMPTZ,
                    p_hasta TIMESTAMPTZ,
                    p_cajero_id INTEGER DEFAULT NULL
                )
                RETURNS BIGINT
                LANGUAGE SQL
                STABLE
                AS $$
                    SELECT COUNT(*)
                    FROM "Turnos"
                    WHERE "Estado" = 3
                        AND "Activo" = TRUE
                        AND "FechaEliminacion" IS NULL
                        AND "FechaFinAtencion" >= p_desde
                        AND "FechaFinAtencion" < p_hasta
                        AND (
                            p_cajero_id IS NULL
                            OR "CajeroId" = p_cajero_id
                        );
                $$;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // ==========================================
            // ELIMINAR FUNCION EN CASO DE REVERSION
            // ==========================================

            migrationBuilder.Sql(
                """
                DROP FUNCTION IF EXISTS fn_turnos_atendidos(
                    TIMESTAMPTZ,
                    TIMESTAMPTZ,
                    INTEGER
                );
                """);
        }
    }
}