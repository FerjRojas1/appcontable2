
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace ServiciosEC.Models;

public partial class ECContext : DbContext
{
    public ECContext()
    {
    }

    public ECContext(DbContextOptions<ECContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Auditoria> Auditorias { get; set; }

    public virtual DbSet<Cliente> Clientes { get; set; }

    public virtual DbSet<Compra> Compras { get; set; }

    public virtual DbSet<EventoAgenda> EventosAgenda { get; set; }

    public virtual DbSet<Estado> Estados { get; set; }

    public virtual DbSet<Ingresosbrutos> Ingresosbrutos { get; set; }

    public virtual DbSet<Jurisdicciones> Jurisdicciones { get; set; }

    public virtual DbSet<LibroIva> LibrosIva { get; set; }

    public virtual DbSet<Persona> Personas { get; set; }

    public virtual DbSet<Iva> Ivas { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    public virtual DbSet<Venta> Ventas { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Auditoria>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Auditori__3213E83FFDCBBAAC");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Accion)
                .HasMaxLength(50)
                .HasColumnName("accion");
            entity.Property(e => e.DatosAntes).HasColumnName("datos_antes");
            entity.Property(e => e.DatosDespues).HasColumnName("datos_despues");
            entity.Property(e => e.Fecha)
                .HasPrecision(0)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("fecha");
            entity.Property(e => e.IdPersona).HasColumnName("id_persona");
            entity.Property(e => e.Tabla)
                .HasMaxLength(100)
                .HasColumnName("tabla");
        });



        modelBuilder.Entity<Compra>(entity =>
        {
            entity.HasKey(e => e.IdCompra).HasName("PK__Compras__C4BAA604464DD8E7");

            entity.Property(e => e.IdCompra).HasColumnName("id_compra");
            entity.Property(e => e.DenomVendedor)
                .HasMaxLength(100)
                .HasColumnName("denom_vendedor");
            entity.Property(e => e.EstadoId)
                .HasDefaultValue(1)
                .HasColumnName("estado_id");
            entity.Property(e => e.Exento)
                .HasColumnType("decimal(18, 5)")
                .HasColumnName("exento");
            entity.Property(e => e.Fecha).HasColumnName("fecha");
            entity.Property(e => e.Grav0)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 5)")
                   .HasColumnName("grav_0");
            entity.Property(e => e.Grav105)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 5)")
                   .HasColumnName("grav_10_5");
            entity.Property(e => e.Grav21)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 5)")
                    .HasColumnName("grav_21");
            entity.Property(e => e.Grav25)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 5)")
                   .HasColumnName("grav_2_5");
            entity.Property(e => e.Grav27)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 5)")
                   .HasColumnName("grav_27");
            entity.Property(e => e.Grav5)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 5)")
                  .HasColumnName("grav_5");
            entity.Property(e => e.IdPersona).HasColumnName("id_persona");
            entity.Property(e => e.Iva)
                .HasColumnType("decimal(18, 5)")
                .HasColumnName("iva");
            entity.Property(e => e.Iva0)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 5)")
                   .HasColumnName("iva_0");
            entity.Property(e => e.Iva105)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 5)")
               .HasColumnName("iva_10_5");
            entity.Property(e => e.Iva21)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 5)")
                   .HasColumnName("iva_21");
            entity.Property(e => e.Iva25)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 5)")
                .HasColumnName("iva_2_5");
            entity.Property(e => e.Iva27)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 5)")
                .HasColumnName("iva_27");
            entity.Property(e => e.Iva5)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 5)")
                .HasColumnName("iva_5");
            entity.Property(e => e.Moneda)
                .HasMaxLength(50)
                .HasColumnName("moneda");
            entity.Property(e => e.NetoGravado)
                .HasColumnType("decimal(18, 5)")
                .HasColumnName("neto_gravado");
            entity.Property(e => e.NoGravado)
                .HasColumnType("decimal(18, 5)")
                .HasColumnName("no_gravado");
            entity.Property(e => e.NroDesde).HasColumnName("nro_desde");
            entity.Property(e => e.NroDocVendedor)
                .HasMaxLength(50)
                .HasColumnName("nro_doc_vendedor");
            entity.Property(e => e.NroHasta).HasColumnName("nro_hasta");
            entity.Property(e => e.PuntoVenta).HasColumnName("punto_venta");
            entity.Property(e => e.TipoCambio).HasColumnName("tipo_cambio");
            entity.Property(e => e.TipoDocVendedor)
                .HasMaxLength(50)
                .HasColumnName("tipo_doc_vendedor");
            entity.Property(e => e.TipoFact)
                .HasMaxLength(50)
                .HasColumnName("tipo_fact");
            entity.Property(e => e.Total)
                .HasColumnType("decimal(18, 5)")
                .HasColumnName("total");

            entity.HasOne(d => d.Estado).WithMany(p => p.Compras)
                .HasForeignKey(d => d.EstadoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Compras__estado___3F466844");

            entity.HasOne(d => d.IdPersonaNavigation).WithMany(p => p.Compras)
                .HasForeignKey(d => d.IdPersona)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Compras__id_pers__403A8C7D");
        });

        modelBuilder.Entity<Ingresosbrutos>(entity =>
        {
            entity.HasKey(e => e.IdIngresosbrutos).HasName("PK__INGRESOS__1A12CAE131987BF6");

            entity.ToTable("INGRESOSBRUTOS");

            entity.Property(e => e.IdIngresosbrutos).HasColumnName("id_ingresosbrutos");
            entity.Property(e => e.Aduaneras)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 5)")
                .HasColumnName("aduaneras");
            entity.Property(e => e.Alicuota)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(5, 4)")
                .HasColumnName("alicuota");
            entity.Property(e => e.Anio).HasColumnName("anio");
            entity.Property(e => e.Coeficiente)
                .HasDefaultValue(1m)
                .HasColumnType("decimal(6, 5)")
                .HasColumnName("coeficiente");
            entity.Property(e => e.FechaDeclaracion)
                .HasPrecision(0)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("fecha_declaracion");
            entity.Property(e => e.GravadoJurisdiccion)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 5)")
                .HasColumnName("gravado_jurisdiccion");
            entity.Property(e => e.GravadoPais)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 5)")
                .HasColumnName("gravado_pais");
            entity.Property(e => e.IdPersona).HasColumnName("id_persona");
            entity.Property(e => e.ImpuestoDeterminado)
                .HasComputedColumnSql("(CONVERT([decimal](18,5),[gravado_jurisdiccion]*[alicuota]))", true)
                .HasColumnType("decimal(18, 5)")
                .HasColumnName("impuesto_determinado");
            entity.Property(e => e.JurisdiccionId)
                .HasDefaultValue(1)
                .HasColumnName("jurisdiccion_id");
            entity.Property(e => e.Percepciones)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 5)")
                .HasColumnName("percepciones");
            entity.Property(e => e.Periodo).HasColumnName("periodo");
            entity.Property(e => e.Retenciones)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 5)")
                .HasColumnName("retenciones");
            entity.Property(e => e.RetencionesBancarias)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 5)")
                .HasColumnName("retenciones_bancarias");
            entity.Property(e => e.Saldo)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 5)")
                .HasColumnName("saldo");

            entity.HasOne(d => d.IdPersonaNavigation).WithMany(p => p.Ingresosbrutos)
                .HasForeignKey(d => d.IdPersona)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__INGRESOSB__id_pe__07C12930");

            entity.HasOne(d => d.Jurisdiccion).WithMany(p => p.Ingresosbrutos)
                .HasForeignKey(d => d.JurisdiccionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__INGRESOSB__juris__08B54D69");
        });


        modelBuilder.Entity<Jurisdicciones>(entity =>
        {
            entity.HasKey(e => e.IdJurisdiccion).HasName("PK__Jurisdic__2B87043A6E7627D6");

            entity.Property(e => e.IdJurisdiccion).HasColumnName("id_jurisdiccion");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(150)
                .HasColumnName("descripcion");
        });

        modelBuilder.Entity<LibroIva>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__LibroIva__3214EC073CF982F2");

            entity.ToTable("LibroIva");

            entity.Property(e => e.CreditoExento)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("Credito_Exento");
            entity.Property(e => e.CreditoIva0)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("Credito_Iva0");
            entity.Property(e => e.CreditoIva105)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("Credito_Iva105");
            entity.Property(e => e.CreditoIva21)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("Credito_Iva21");
            entity.Property(e => e.CreditoIva25)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("Credito_Iva25");
            entity.Property(e => e.CreditoIva27)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("Credito_Iva27");
            entity.Property(e => e.CreditoIva5)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("Credito_Iva5");
            entity.Property(e => e.CreditoIvaOtros)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("Credito_IvaOtros");
            entity.Property(e => e.CreditoNeto0)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("Credito_Neto0");
            entity.Property(e => e.CreditoNeto105)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("Credito_Neto105");
            entity.Property(e => e.CreditoNeto21)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("Credito_Neto21");
            entity.Property(e => e.CreditoNeto25)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("Credito_Neto25");
            entity.Property(e => e.CreditoNeto27)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("Credito_Neto27");
            entity.Property(e => e.CreditoNeto5)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("Credito_Neto5");
            entity.Property(e => e.CreditoNetoOtros)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("Credito_NetoOtros");
            entity.Property(e => e.CreditoNoGravado)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("Credito_NoGravado");
            entity.Property(e => e.Cuit)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.DebitoExento)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("Debito_Exento");
            entity.Property(e => e.DebitoIva0)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("Debito_Iva0");
            entity.Property(e => e.DebitoIva105)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("Debito_Iva105");
            entity.Property(e => e.DebitoIva21)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("Debito_Iva21");
            entity.Property(e => e.DebitoIva25)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("Debito_Iva25");
            entity.Property(e => e.DebitoIva27)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("Debito_Iva27");
            entity.Property(e => e.DebitoIva5)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("Debito_Iva5");
            entity.Property(e => e.DebitoIvaOtros)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("Debito_IvaOtros");
            entity.Property(e => e.DebitoNeto0)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("Debito_Neto0");
            entity.Property(e => e.DebitoNeto105)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("Debito_Neto105");
            entity.Property(e => e.DebitoNeto21)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("Debito_Neto21");
            entity.Property(e => e.DebitoNeto25)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("Debito_Neto25");
            entity.Property(e => e.DebitoNeto27)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("Debito_Neto27");
            entity.Property(e => e.DebitoNeto5)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("Debito_Neto5");
            entity.Property(e => e.DebitoNetoOtros)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("Debito_NetoOtros");
            entity.Property(e => e.DebitoNoGravado)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("Debito_NoGravado");
            entity.Property(e => e.FechaDeclaracion)
                .HasPrecision(0)
                .HasDefaultValueSql("(getdate())");
            entity.Property(e => e.GravadoCreditoFiscal).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.GravadoDebitoFical).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.IdPersona).HasColumnName("id_persona");
            entity.Property(e => e.IvaCreditoFiscal).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.IvaDebitoFiscal).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.PercepcionesIva)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("PercepcionesIVA");
            entity.Property(e => e.RestCreditoExento)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("RestCredito_Exento");
            entity.Property(e => e.RestCreditoIva0)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("RestCredito_Iva0");
            entity.Property(e => e.RestCreditoIva105)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("RestCredito_Iva105");
            entity.Property(e => e.RestCreditoIva21)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("RestCredito_Iva21");
            entity.Property(e => e.RestCreditoIva25)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("RestCredito_Iva25");
            entity.Property(e => e.RestCreditoIva27)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("RestCredito_Iva27");
            entity.Property(e => e.RestCreditoIva5)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("RestCredito_Iva5");
            entity.Property(e => e.RestCreditoIvaOtros)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("RestCredito_IvaOtros");
            entity.Property(e => e.RestCreditoNeto0)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("RestCredito_Neto0");
            entity.Property(e => e.RestCreditoNeto105)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("RestCredito_Neto105");
            entity.Property(e => e.RestCreditoNeto21)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("RestCredito_Neto21");
            entity.Property(e => e.RestCreditoNeto25)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("RestCredito_Neto25");
            entity.Property(e => e.RestCreditoNeto27)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("RestCredito_Neto27");
            entity.Property(e => e.RestCreditoNeto5)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("RestCredito_Neto5");
            entity.Property(e => e.RestCreditoNetoOtros)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("RestCredito_NetoOtros");
            entity.Property(e => e.RestCreditoNoGravado)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("RestCredito_NoGravado");
            entity.Property(e => e.RestDebitoExento)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("RestDebito_Exento");
            entity.Property(e => e.RestDebitoIva0)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("RestDebito_Iva0");
            entity.Property(e => e.RestDebitoIva105)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("RestDebito_Iva105");
            entity.Property(e => e.RestDebitoIva21)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("RestDebito_Iva21");
            entity.Property(e => e.RestDebitoIva25)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("RestDebito_Iva25");
            entity.Property(e => e.RestDebitoIva27)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("RestDebito_Iva27");
            entity.Property(e => e.RestDebitoIva5)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("RestDebito_Iva5");
            entity.Property(e => e.RestDebitoIvaOtros)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("RestDebito_IvaOtros");
            entity.Property(e => e.RestDebitoNeto0)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("RestDebito_Neto0");
            entity.Property(e => e.RestDebitoNeto105)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("RestDebito_Neto105");
            entity.Property(e => e.RestDebitoNeto21)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("RestDebito_Neto21");
            entity.Property(e => e.RestDebitoNeto25)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("RestDebito_Neto25");
            entity.Property(e => e.RestDebitoNeto27)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("RestDebito_Neto27");
            entity.Property(e => e.RestDebitoNeto5)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("RestDebito_Neto5");
            entity.Property(e => e.RestDebitoNetoOtros)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("RestDebito_NetoOtros");
            entity.Property(e => e.RestDebitoNoGravado)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("RestDebito_NoGravado");
            entity.Property(e => e.RetencionesIva)
                .HasColumnType("decimal(18, 2)")
                .HasColumnName("RetencionesIVA");
            entity.Property(e => e.SaldoLibreDisponibilidad).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.SaldoTecnico).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.SaldoTecnicoAnterior).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.SaldoTecnicoNeto).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.IdPersonaNavigation).WithMany(p => p.LibrosIva)
                .HasForeignKey(d => d.IdPersona)
                .HasConstraintName("FK__LibroIva__id_per__14270015");
        });

        modelBuilder.Entity<Estado>(entity =>
        {
            entity.HasKey(e => e.IdEstado).HasName("PK__Estados__86989FB2DB6BFC98");

            entity.Property(e => e.IdEstado).HasColumnName("id_estado");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(50)
                .HasColumnName("descripcion");
        });

        modelBuilder.Entity<Persona>(entity =>
        {
            entity.HasKey(e => e.IdPersona).HasName("PK__Personas__228148B0F120C233");

            entity.HasIndex(e => e.Dni, "UQ_Personas_dni")
                .IsUnique()
                .HasFilter("([dni] IS NOT NULL)");

            entity.HasIndex(e => e.Email, "UQ__Personas__email")
                .IsUnique()
                .HasFilter("([email] IS NOT NULL)");

            entity.Property(e => e.IdPersona).HasColumnName("id_persona");
            entity.Property(e => e.Apellido)
                .HasMaxLength(50)
                .HasColumnName("apellido");
            entity.Property(e => e.Dni)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("dni");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .HasColumnName("email");
            entity.Property(e => e.EstadoId)
                .HasDefaultValue(1)
                .HasColumnName("estado_id");
            entity.Property(e => e.FechaAlta)
                .HasPrecision(0)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("fecha_alta");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .HasColumnName("nombre");
            entity.Property(e => e.RolId).HasColumnName("rol_id");
            entity.Property(e => e.Telefono)
                .HasMaxLength(50)
                .HasColumnName("telefono");

            entity.HasOne(d => d.Estado).WithMany(p => p.Personas)
                .HasForeignKey(d => d.EstadoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Personas__estado__603D47BB");

            entity.HasOne(d => d.Rol).WithMany(p => p.Personas)
                .HasForeignKey(d => d.RolId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Personas__rol_id__61316BF4");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.IdRol).HasName("PK__Roles__6ABCB5E02351A66A");

            entity.Property(e => e.IdRol).HasColumnName("id_rol");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(50)
                .HasColumnName("descripcion");
        });

        modelBuilder.Entity<Iva>(entity =>
        {
            entity.HasKey(e => e.IdIva).HasName("PK__IVA__0C143A82B0D52CCA");

            entity.ToTable("IVA");

            entity.Property(e => e.IdIva).HasColumnName("IdIVA");
            entity.Property(e => e.Descripcion).HasMaxLength(100);
            entity.Property(e => e.Porcentaje).HasColumnType("decimal(5, 3)");
        });

        modelBuilder.Entity<Venta>(entity =>
        {
            entity.HasKey(e => e.IdVenta).HasName("PK__Ventas__459533BF093E752F");

            entity.Property(e => e.IdVenta).HasColumnName("id_venta");
            entity.Property(e => e.DenomComprador)
                .HasMaxLength(100)
                .HasColumnName("denom_comprador");
            entity.Property(e => e.EstadoId)
                .HasDefaultValue(1)
                .HasColumnName("estado_id");
            entity.Property(e => e.Exento)
                .HasColumnType("decimal(18, 5)")
                .HasColumnName("exento");
            entity.Property(e => e.Fecha).HasColumnName("fecha");
            entity.Property(e => e.Grav0)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 5)")
                .HasColumnName("grav_0");
            entity.Property(e => e.Grav105)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 5)")
                  .HasColumnName("grav_10_5");
            entity.Property(e => e.Grav21)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 5)")
                .HasColumnName("grav_21");
            entity.Property(e => e.Grav25)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 5)")
               .HasColumnName("grav_2_5");
            entity.Property(e => e.Grav27)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 5)")
                .HasColumnName("grav_27");
            entity.Property(e => e.Grav5)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 5)")
                .HasColumnName("grav_5");
            entity.Property(e => e.IdPersona).HasColumnName("id_persona");
            entity.Property(e => e.Iva)
                .HasColumnType("decimal(18, 5)")
                .HasColumnName("iva");
            entity.Property(e => e.Iva0)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 5)")
                .HasColumnName("iva_0");
            entity.Property(e => e.Iva105)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 5)")
                .HasColumnName("iva_10_5");
            entity.Property(e => e.Iva21)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 5)")
                .HasColumnName("iva_21");
            entity.Property(e => e.Iva25)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 5)")
                .HasColumnName("iva_2_5");
            entity.Property(e => e.Iva27)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 5)")
                .HasColumnName("iva_27");
            entity.Property(e => e.Iva5)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(18, 5)")
                .HasColumnName("iva_5");
            entity.Property(e => e.Moneda)
                .HasMaxLength(50)
                .HasColumnName("moneda");
            entity.Property(e => e.NetoGravado)
                .HasColumnType("decimal(18, 5)")
                .HasColumnName("neto_gravado");
            entity.Property(e => e.NoGravado)
                .HasColumnType("decimal(18, 5)")
                .HasColumnName("no_gravado");
            entity.Property(e => e.NroDesde).HasColumnName("nro_desde");
            entity.Property(e => e.NroDocComprador)
                .HasMaxLength(50)
                .HasColumnName("nro_doc_comprador");
            entity.Property(e => e.NroHasta).HasColumnName("nro_hasta");
            entity.Property(e => e.PuntoVenta).HasColumnName("punto_venta");
            entity.Property(e => e.TipoCambio).HasColumnName("tipo_cambio");
            entity.Property(e => e.TipoDocComprador)
                .HasMaxLength(50)
                .HasColumnName("tipo_doc_comprador");
            entity.Property(e => e.TipoFact)
                .HasMaxLength(50)
                .HasColumnName("tipo_fact");
            entity.Property(e => e.Total)
                .HasColumnType("decimal(18, 5)")
                .HasColumnName("total");

            entity.HasOne(d => d.Estado).WithMany(p => p.Venta)
                .HasForeignKey(d => d.EstadoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Ventas__estado_i__3A81B327");

            entity.HasOne(d => d.IdPersonaNavigation).WithMany(p => p.Venta)
                .HasForeignKey(d => d.IdPersona)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Ventas__id_perso__3B75D760");
        });

        modelBuilder.Entity<Cliente>();
        modelBuilder.Entity<Usuario>();

        modelBuilder.Entity<Persona>()
            .HasDiscriminator<string>("Discriminator")
            .HasValue<Cliente>("Cliente")
            .HasValue<Usuario>("Usuario");

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
