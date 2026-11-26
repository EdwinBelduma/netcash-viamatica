using Microsoft.EntityFrameworkCore;
using NetCash.Dominio.Entidades;
using NetCash.Dominio.Enumeraciones;

namespace NetCash.Infraestructura.Persistencia;

public class NetCashDbContext : DbContext
{
    public NetCashDbContext(
        DbContextOptions<NetCashDbContext> options)
        : base(options)
    {
    }

    // ==========================================
    // ENTIDADES DEL SISTEMA
    // ==========================================

    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Rol> Roles => Set<Rol>();
    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<Caja> Cajas => Set<Caja>();
    public DbSet<UsuarioCaja> UsuariosCajas => Set<UsuarioCaja>();
    public DbSet<Turno> Turnos => Set<Turno>();
    public DbSet<Servicio> Servicios => Set<Servicio>();
    public DbSet<Equipo> Equipos => Set<Equipo>();
    public DbSet<Contrato> Contratos => Set<Contrato>();
    public DbSet<FormaPago> FormasPago => Set<FormaPago>();
    public DbSet<Pago> Pagos => Set<Pago>();
    public DbSet<Atencion> Atenciones => Set<Atencion>();
    public DbSet<Menu> Menus => Set<Menu>();
    public DbSet<RolMenu> RolesMenus => Set<RolMenu>();

    // Nueva entidad de recuperacion de contrasena
    public DbSet<TokenRecuperacion> TokensRecuperacion
        => Set<TokenRecuperacion>();

    // ==========================================
    // CONFIGURACION DEL MODELO
    // ==========================================

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ConfigurarUsuarios(modelBuilder);
        ConfigurarClientes(modelBuilder);
        ConfigurarCajas(modelBuilder);
        ConfigurarTurnos(modelBuilder);
        ConfigurarServicios(modelBuilder);
        ConfigurarContratos(modelBuilder);
        ConfigurarPagos(modelBuilder);
        ConfigurarMenus(modelBuilder);

        // Nueva configuracion
        ConfigurarTokensRecuperacion(modelBuilder);

        ConfigurarEliminacionLogica(modelBuilder);
        ConfigurarDatosIniciales(modelBuilder);
    }

    // ==========================================
    // CONFIGURACION DE USUARIOS
    // ==========================================

    private static void ConfigurarUsuarios(ModelBuilder builder)
    {
        builder.Entity<Usuario>(entity =>
        {
            entity.HasIndex(x => x.NombreUsuario)
                .IsUnique();

            entity.Property(x => x.NombreUsuario)
                .HasMaxLength(20)
                .IsRequired();

            entity.Property(x => x.ContrasenaHash)
                .IsRequired();

            entity.Property(x => x.Nombres)
                .HasMaxLength(100);

            entity.Property(x => x.Apellidos)
                .HasMaxLength(100);

            entity.Property(x => x.IdentificacionCifrada)
                .IsRequired();

            entity.Property(x => x.CorreoCifrado)
                .IsRequired();

            entity.HasOne(x => x.Rol)
                .WithMany()
                .HasForeignKey(x => x.RolId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<Rol>()
            .HasIndex(x => x.Tipo)
            .IsUnique();
    }

    // ==========================================
    // CONFIGURACION DE CLIENTES
    // ==========================================

    private static void ConfigurarClientes(ModelBuilder builder)
    {
        builder.Entity<Cliente>(entity =>
        {
            entity.HasIndex(x => x.Identificacion)
                .IsUnique();

            entity.Property(x => x.Identificacion)
                .HasMaxLength(13)
                .IsRequired();

            entity.Property(x => x.Nombres)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(x => x.Apellidos)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(x => x.Correo)
                .HasMaxLength(120);

            entity.Property(x => x.Telefono)
                .HasMaxLength(20);

            entity.Property(x => x.Direccion)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(x => x.ReferenciaDireccion)
                .HasMaxLength(100)
                .IsRequired();
        });
    }

    // ==========================================
    // CONFIGURACION DE CAJAS
    // ==========================================

    private static void ConfigurarCajas(ModelBuilder builder)
    {
        builder.Entity<Caja>()
            .Property(x => x.Descripcion)
            .HasMaxLength(50)
            .IsRequired();

        builder.Entity<UsuarioCaja>(entity =>
        {
            entity.HasIndex(x => new
            {
                x.UsuarioId,
                x.CajaId
            })
            .IsUnique()
            .HasFilter("\"Activo\" = true");

            entity.HasOne(x => x.Usuario)
                .WithMany()
                .HasForeignKey(x => x.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.Gestor)
                .WithMany()
                .HasForeignKey(x => x.GestorId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.Caja)
                .WithMany(x => x.UsuariosCaja)
                .HasForeignKey(x => x.CajaId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

    // ==========================================
    // CONFIGURACION DE TURNOS
    // ==========================================

    private static void ConfigurarTurnos(ModelBuilder builder)
    {
        builder.Entity<Turno>(entity =>
        {
            entity.Property(x => x.Descripcion)
                .HasMaxLength(6)
                .IsRequired();

            entity.HasIndex(x => x.Descripcion)
                .IsUnique();

            entity.HasOne(x => x.Gestor)
                .WithMany()
                .HasForeignKey(x => x.GestorId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.Cajero)
                .WithMany()
                .HasForeignKey(x => x.CajeroId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.Caja)
                .WithMany()
                .HasForeignKey(x => x.CajaId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

    // ==========================================
    // CONFIGURACION DE SERVICIOS Y EQUIPOS
    // ==========================================

    private static void ConfigurarServicios(ModelBuilder builder)
    {
        builder.Entity<Servicio>(entity =>
        {
            entity.Property(x => x.Nombre)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(x => x.Descripcion)
                .HasMaxLength(150);

            entity.Property(x => x.Precio)
                .HasPrecision(12, 2);
        });

        builder.Entity<Equipo>()
            .HasOne(x => x.Servicio)
            .WithMany(x => x.Equipos)
            .HasForeignKey(x => x.ServicioId)
            .OnDelete(DeleteBehavior.Restrict);
    }

    // ==========================================
    // CONFIGURACION DE CONTRATOS
    // ==========================================

    private static void ConfigurarContratos(ModelBuilder builder)
    {
        builder.Entity<Contrato>(entity =>
        {
            entity.Property(x => x.PrecioContratado)
                .HasPrecision(12, 2);

            entity.HasOne(x => x.ContratoAnterior)
                .WithMany()
                .HasForeignKey(x => x.ContratoAnteriorId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

    // ==========================================
    // CONFIGURACION DE PAGOS
    // ==========================================

    private static void ConfigurarPagos(ModelBuilder builder)
    {
        builder.Entity<Pago>()
            .Property(x => x.Monto)
            .HasPrecision(12, 2);
    }

    // ==========================================
    // CONFIGURACION DE MENUS DINAMICOS
    // ==========================================

    private static void ConfigurarMenus(ModelBuilder builder)
    {
        builder.Entity<RolMenu>()
            .HasIndex(x => new
            {
                x.RolId,
                x.MenuId
            })
            .IsUnique();

        builder.Entity<Menu>()
            .HasOne(x => x.MenuPadre)
            .WithMany()
            .HasForeignKey(x => x.MenuPadreId)
            .OnDelete(DeleteBehavior.Restrict);
    }

    // ==========================================
    // CONFIGURACION DE TOKENS DE RECUPERACION
    // ==========================================

    private static void ConfigurarTokensRecuperacion(
        ModelBuilder builder)
    {
        builder.Entity<TokenRecuperacion>(entity =>
        {
            entity.ToTable("TokensRecuperacion");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.TokenHash)
                .HasMaxLength(64)
                .IsRequired();

            entity.HasIndex(x => x.TokenHash)
                .IsUnique();

            entity.HasIndex(x => x.UsuarioId);

            entity.Property(x => x.FechaExpiracion)
                .IsRequired();

            entity.Property(x => x.Utilizado)
                .HasDefaultValue(false);

            entity.HasOne(x => x.Usuario)
                .WithMany()
                .HasForeignKey(x => x.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }

    // ==========================================
    // CONFIGURACION DE ELIMINACION LOGICA
    // ==========================================

    private static void ConfigurarEliminacionLogica(
        ModelBuilder builder)
    {
        builder.Entity<Usuario>()
            .HasQueryFilter(x => x.FechaEliminacion == null);

        builder.Entity<Rol>()
            .HasQueryFilter(x => x.FechaEliminacion == null);

        builder.Entity<Cliente>()
            .HasQueryFilter(x => x.FechaEliminacion == null);

        builder.Entity<Caja>()
            .HasQueryFilter(x => x.FechaEliminacion == null);

        builder.Entity<UsuarioCaja>()
            .HasQueryFilter(x => x.FechaEliminacion == null);

        builder.Entity<Turno>()
            .HasQueryFilter(x => x.FechaEliminacion == null);

        builder.Entity<Servicio>()
            .HasQueryFilter(x => x.FechaEliminacion == null);

        builder.Entity<Equipo>()
            .HasQueryFilter(x => x.FechaEliminacion == null);

        builder.Entity<Contrato>()
            .HasQueryFilter(x => x.FechaEliminacion == null);

        builder.Entity<FormaPago>()
            .HasQueryFilter(x => x.FechaEliminacion == null);

        builder.Entity<Pago>()
            .HasQueryFilter(x => x.FechaEliminacion == null);

        builder.Entity<Atencion>()
            .HasQueryFilter(x => x.FechaEliminacion == null);

        builder.Entity<Menu>()
            .HasQueryFilter(x => x.FechaEliminacion == null);

        builder.Entity<RolMenu>()
            .HasQueryFilter(x => x.FechaEliminacion == null);

        // Eliminacion logica de tokens de recuperacion
        builder.Entity<TokenRecuperacion>()
            .HasQueryFilter(x =>
                x.FechaEliminacion == null &&
                x.Usuario.FechaEliminacion == null);
    }

    // ==========================================
    // CONFIGURACION DE DATOS INICIALES
    // ==========================================

    private static void ConfigurarDatosIniciales(
        ModelBuilder builder)
    {
        var fecha = new DateTime(
            2026, 11, 25, 0, 0, 0,
            DateTimeKind.Utc);

        // Roles iniciales
        builder.Entity<Rol>().HasData(
            new Rol
            {
                Id = 1,
                Nombre = "Administrador",
                Descripcion = "Administracion del sistema",
                Tipo = TipoRol.Administrador,
                FechaCreacion = fecha
            },
            new Rol
            {
                Id = 2,
                Nombre = "Gestor",
                Descripcion = "Gestion de cajas y turnos",
                Tipo = TipoRol.Gestor,
                FechaCreacion = fecha
            },
            new Rol
            {
                Id = 3,
                Nombre = "Cajero",
                Descripcion = "Atencion y procesos de caja",
                Tipo = TipoRol.Cajero,
                FechaCreacion = fecha
            },
            new Rol
            {
                Id = 4,
                Nombre = "Cliente",
                Descripcion = "Cliente del servicio",
                Tipo = TipoRol.Cliente,
                FechaCreacion = fecha
            }
        );

        // Formas de pago iniciales
        builder.Entity<FormaPago>().HasData(
            new FormaPago
            {
                Id = 1,
                Nombre = "Efectivo",
                Descripcion = "Pago en efectivo",
                FechaCreacion = fecha
            },
            new FormaPago
            {
                Id = 2,
                Nombre = "Transferencia",
                Descripcion = "Transferencia bancaria",
                FechaCreacion = fecha
            },
            new FormaPago
            {
                Id = 3,
                Nombre = "Tarjeta",
                Descripcion = "Tarjeta de credito o debito",
                FechaCreacion = fecha
            }
        );
    }
}