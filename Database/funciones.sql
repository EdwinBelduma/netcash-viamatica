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