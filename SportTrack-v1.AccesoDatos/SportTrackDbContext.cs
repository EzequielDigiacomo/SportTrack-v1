using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SportTrack_v1.Entidades.Entidades;
using SportTrack_v1.Entidades.Enums;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace SportTrack.AccessDatos
{
    public class SportTrackDbContext : DbContext
    {
        public SportTrackDbContext(DbContextOptions<SportTrackDbContext> options) : base(options)
        {
        }

        // Tablas Maestras
        public DbSet<Sexo> Sexos { get; set; }
        public DbSet<Bote> Botes { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Distancia> Distancias { get; set; }
        public DbSet<Club> Clubes { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }

        // Tablas Principales
        public DbSet<Evento> Eventos { get; set; }
        public DbSet<Prueba> Pruebas { get; set; }
        public DbSet<EventoPrueba> EventoPruebas { get; set; }
        public DbSet<Participante> Participantes { get; set; }
        public DbSet<Inscripcion> Inscripciones { get; set; }
        public DbSet<InscripcionTripulante> InscripcionTripulantes { get; set; }
        public DbSet<Etapa> Etapas { get; set; }
        public DbSet<Fase> Fases { get; set; }
        public DbSet<ReglaProgresion> ReglasProgresion { get; set; }
        public DbSet<Resultado> Resultados { get; set; }
        public DbSet<Penalizacion> Penalizaciones { get; set; }
        public DbSet<Auditoria> Auditoria { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración para PostgreSQL: Asegurar que todos los DateTime sean tratados como UTC
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var property in entityType.GetProperties())
                {
                    if (property.ClrType == typeof(DateTime) || property.ClrType == typeof(DateTime?))
                    {
                        property.SetValueConverter(new ValueConverter<DateTime, DateTime>(
                            v => v.Kind == DateTimeKind.Utc ? v : DateTime.SpecifyKind(v, DateTimeKind.Utc),
                            v => v.Kind == DateTimeKind.Utc ? v : DateTime.SpecifyKind(v, DateTimeKind.Utc)
                        ));
                    }
                }
            }

            // ============================================
            // CONFIGURACIÓN DE TABLAS MAESTRAS
            // ============================================

            // Tabla: Sexo
            modelBuilder.Entity<Sexo>(entity =>
            {
                entity.ToTable("Sexos", "catalogos");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Nombre)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.HasIndex(e => e.Nombre)
                    .IsUnique()
                    .HasDatabaseName("IX_Sexos_Nombre");
            });

            // Tabla: Bote
            modelBuilder.Entity<Bote>(entity =>
            {
                entity.ToTable("Botes", "catalogos");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Tipo)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasIndex(e => e.Tipo)
                    .IsUnique()
                    .HasDatabaseName("IX_Botes_Tipo");
            });

            // Tabla: Categoria
            modelBuilder.Entity<Categoria>(entity =>
            {
                entity.ToTable("Categorias", "catalogos");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Nombre)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.EdadMin)
                    .IsRequired(false);

                entity.Property(e => e.EdadMax)
                    .IsRequired(false);

                entity.HasIndex(e => e.Nombre)
                    .IsUnique()
                    .HasDatabaseName("IX_Categorias_Nombre");
            });

            // Tabla: Distancia
            modelBuilder.Entity<Distancia>(entity =>
            {
                entity.ToTable("Distancias", "catalogos");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd();

                // Configurar el enum como int en la BD
                entity.Property(e => e.DistanciaRegata)
                    .IsRequired()
                    .HasConversion<int>();

                // Campo calculado (no se persiste)
                entity.Ignore(e => e.Metros);
                entity.Ignore(e => e.Descripcion);

                // Índice único para la distancia
                entity.HasIndex(e => e.DistanciaRegata)
                    .IsUnique()
                    .HasDatabaseName("IX_Distancias_DistanciaRegata");
            });

            // Tabla: Club
            modelBuilder.Entity<Club>(entity =>
            {
                entity.ToTable("Clubes", "catalogos");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(100);
                entity.HasIndex(e => e.Nombre).IsUnique().HasDatabaseName("IX_Clubes_Nombre");
            });

            // Tabla: Usuario
            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.ToTable("Usuarios", "seguridad");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Username).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Rol).IsRequired().HasMaxLength(20);
                
                entity.HasOne(e => e.Club)
                    .WithMany(c => c.Usuarios)
                    .HasForeignKey(e => e.ClubId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasIndex(e => e.Username).IsUnique();
                entity.HasIndex(e => e.Email).IsUnique();
            });

            // ============================================
            // CONFIGURACIÓN DE TABLAS PRINCIPALES
            // ============================================


            // Tabla: Evento
            modelBuilder.Entity<Evento>(entity =>
            {
                entity.ToTable("Eventos", "regatas");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Nombre)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Fecha)
                    .IsRequired();

                entity.Property(e => e.Ubicacion)
                    .HasMaxLength(200);

                // Configurar el enum como string en la BD
                entity.Property(e => e.Estado)
                    .IsRequired()
                    .HasConversion(new EstadoEventoEnumConverter())
                    .HasMaxLength(20)
                    .HasDefaultValue(EstadoEventoEnum.Programada)
                    .HasSentinel(EstadoEventoEnum.Programada);

                entity.Property(e => e.FechaCreacion)
                    .IsRequired()
                    .HasDefaultValueSql("NOW()");

                entity.HasIndex(e => e.Nombre)
                    .HasDatabaseName("IX_Eventos_Nombre");

                entity.HasIndex(e => e.Fecha)
                    .HasDatabaseName("IX_Eventos_Fecha");

                entity.HasIndex(e => e.Estado)
                    .HasDatabaseName("IX_Eventos_Estado");

                // Relación con Club
                entity.HasOne(e => e.Club)
                    .WithMany()
                    .HasForeignKey(e => e.ClubId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // Tabla: Prueba
            modelBuilder.Entity<Prueba>(entity =>
            {
                entity.ToTable("Pruebas", "regatas");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Nombre)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Descripcion)
                    .HasMaxLength(500);

                // Foreign Keys
                entity.HasOne(e => e.Bote)
                    .WithMany(b => b.Pruebas)
                    .HasForeignKey(e => e.BoteId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_Pruebas_Botes");

                entity.HasOne(e => e.Categoria)
                    .WithMany(c => c.Pruebas)
                    .HasForeignKey(e => e.CategoriaId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_Pruebas_Categorias");

                entity.HasOne(e => e.Distancia)
                    .WithMany(d => d.Pruebas)
                    .HasForeignKey(e => e.DistanciaId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_Pruebas_Distancias");

                entity.HasOne(e => e.Sexo)
                    .WithMany(s => s.Pruebas)
                    .HasForeignKey(e => e.SexoId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_Pruebas_Sexos");

                // Índice único compuesto
                entity.HasIndex(e => new { e.BoteId, e.CategoriaId, e.DistanciaId, e.SexoId })
                    .IsUnique()
                    .HasDatabaseName("IX_Pruebas_Unica");

                entity.HasIndex(e => e.Nombre)
                    .HasDatabaseName("IX_Pruebas_Nombre");
            });

            // Tabla: EventoPrueba
            modelBuilder.Entity<EventoPrueba>(entity =>
            {
                entity.ToTable("EventoPruebas", "regatas");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.FechaHora)
                    .IsRequired();

                entity.Property(e => e.MaxParticipantes)
                    .HasDefaultValue(0);

                entity.Property(e => e.Pista)
                    .HasMaxLength(50);

                // Configurar el enum como string en la BD
                entity.Property(e => e.Estado)
                    .IsRequired()
                    .HasConversion(new EstadoEventoEnumConverter())
                    .HasMaxLength(20)
                    .HasDefaultValue(EstadoEventoEnum.Programada)
                    .HasSentinel(EstadoEventoEnum.Programada);

                // Foreign Keys
                entity.HasOne(e => e.Evento)
                    .WithMany(ev => ev.EventoPruebas)
                    .HasForeignKey(e => e.EventoId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_EventoPruebas_Eventos");

                entity.HasOne(e => e.Prueba)
                    .WithMany(p => p.EventoPruebas)
                    .HasForeignKey(e => e.PruebaId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_EventoPruebas_Pruebas");

                // Índice único que permite varias series (Fases) de una misma prueba en un evento, siempre que tengan distinto horario
                entity.HasIndex(e => new { e.EventoId, e.PruebaId, e.FechaHora })
                    .IsUnique()
                    .HasDatabaseName("IX_EventoPruebas_EventoPrueba_Fecha");

                entity.HasIndex(e => e.FechaHora)
                    .HasDatabaseName("IX_EventoPruebas_FechaHora");

                entity.HasIndex(e => e.Estado)
                    .HasDatabaseName("IX_EventoPruebas_Estado");
            });

            // Tabla: Participante
            modelBuilder.Entity<Participante>(entity =>
            {
                entity.ToTable("Participantes", "regatas");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Nombre)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Apellido)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.FechaNacimiento)
                    .IsRequired()
                    .HasColumnType("date");

                entity.Property(e => e.Pais)
                    .HasMaxLength(50);



                entity.Property(e => e.Email)
                    .HasMaxLength(100);

                entity.Property(e => e.Telefono)
                    .HasMaxLength(20);

                entity.Property(e => e.Direccion)
                    .HasMaxLength(200);

                // Campos calculados (no se persisten)
                entity.Ignore(e => e.Edad);

                // Foreign Keys
                entity.HasOne(e => e.Sexo)
                    .WithMany(s => s.Participantes)
                    .HasForeignKey(e => e.SexoId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_Participantes_Sexos");

                entity.HasOne(e => e.Categoria)
                    .WithMany(c => c.Participantes)
                    .HasForeignKey(e => e.CategoriaId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_Participantes_Categorias");

                // Índices
                entity.HasIndex(e => new { e.Nombre, e.Apellido })
                    .HasDatabaseName("IX_Participantes_NombreApellido");

                // CORRECCIÓN AQUÍ: Cambiado de [Email] a "Email"
                entity.HasIndex(e => e.Email)
                    .IsUnique()
                    .HasDatabaseName("IX_Participantes_Email")
                    .HasFilter("\"Email\" IS NOT NULL"); // CORREGIDO

                entity.HasOne(e => e.Club)
                    .WithMany(c => c.Participantes)
                    .HasForeignKey(e => e.ClubId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_Participantes_Clubes");

                entity.HasIndex(e => e.ClubId)
                    .HasDatabaseName("IX_Participantes_ClubId");

                entity.HasIndex(e => e.Pais)
                    .HasDatabaseName("IX_Participantes_Pais");

                entity.HasIndex(e => e.SexoId)
                    .HasDatabaseName("IX_Participantes_SexoId");
            });

            // Tabla: Inscripcion
            modelBuilder.Entity<Inscripcion>(entity =>
            {
                entity.ToTable("Inscripciones", "regatas");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.FechaInscripcion).IsRequired().HasDefaultValueSql("NOW()");
                entity.Property(e => e.NumeroCompetidor).IsRequired().HasMaxLength(20);
                entity.Property(e => e.EsCabezaDeSerie).IsRequired().HasDefaultValue(false);

                // Configurar el enum como string en la BD
                entity.Property(e => e.Estado)
                    .IsRequired()
                    .HasConversion(new EstadoInscripcionEnumConverter())
                    .HasMaxLength(20)
                    .HasDefaultValue(EstadoInscripcionEnum.Inscrito)
                    .HasSentinel(EstadoInscripcionEnum.Inscrito);

                // Foreign Keys
                entity.HasOne(e => e.EventoPrueba)
                    .WithMany(ep => ep.Inscripciones)
                    .HasForeignKey(e => e.EventoPruebaId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_Inscripciones_EventoPruebas");

                entity.HasOne(e => e.Participante)
                    .WithMany(p => p.Inscripciones)
                    .HasForeignKey(e => e.ParticipanteId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired(false)
                    .HasConstraintName("FK_Inscripciones_Participantes");

                // Quitamos el IsUnique porque un numero de dorsal puede repetirse a lo largo de las distintas series/eventos
                entity.HasIndex(e => e.NumeroCompetidor).HasDatabaseName("IX_Inscripciones_NumeroCompetidor");
                entity.HasIndex(e => e.Estado).HasDatabaseName("IX_Inscripciones_Estado");
                entity.HasIndex(e => e.FechaInscripcion).HasDatabaseName("IX_Inscripciones_FechaInscripcion");
                entity.HasIndex(e => e.ParticipanteId).HasDatabaseName("IX_Inscripciones_ParticipanteId");
            });

            // Tabla: InscripcionTripulante
            modelBuilder.Entity<InscripcionTripulante>(entity =>
            {
                entity.ToTable("InscripcionTripulantes", "regatas");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.PosicionEnBote)
                    .IsRequired(false);

                // Foreign Keys
                entity.HasOne(e => e.Inscripcion)
                    .WithMany(i => i.Tripulantes)
                    .HasForeignKey(e => e.InscripcionId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_InscripcionTripulantes_Inscripciones");

                entity.HasOne(e => e.Participante)
                    .WithMany()
                    .HasForeignKey(e => e.ParticipanteId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_InscripcionTripulantes_Participantes");

                // Índices
                entity.HasIndex(e => new { e.InscripcionId, e.ParticipanteId })
                    .IsUnique()
                    .HasDatabaseName("IX_InscripcionTripulantes_Unica");
            });

            // Tabla: Etapa
            modelBuilder.Entity<Etapa>(entity =>
            {
                entity.ToTable("Etapas", "regatas");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(50);
                
                entity.HasOne(e => e.EventoPrueba)
                    .WithMany(ep => ep.Etapas)
                    .HasForeignKey(e => e.EventoPruebaId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_Etapas_EventoPruebas");
            });

            // Tabla: Fase
            modelBuilder.Entity<Fase>(entity =>
            {
                entity.ToTable("Fases", "regatas");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.NombreFase).IsRequired().HasMaxLength(50);
                entity.Property(e => e.NumeroFase).IsRequired();
                entity.Property(e => e.FechaHoraProgramada).IsRequired(false);
                entity.Property(e => e.Estado).IsRequired().HasMaxLength(50);
                
                entity.HasOne(e => e.Etapa)
                    .WithMany(et => et.Fases)
                    .HasForeignKey(e => e.EtapaId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_Fases_Etapas");
            });

            // Tabla: ReglaProgresion
            modelBuilder.Entity<ReglaProgresion>(entity =>
            {
                entity.ToTable("ReglasProgresion", "regatas");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                entity.HasOne(e => e.EventoPrueba)
                    .WithMany(ep => ep.ReglasProgresion)
                    .HasForeignKey(e => e.EventoPruebaId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_ReglasProgresion_EventoPruebas");

                entity.HasOne(e => e.EtapaOrigen)
                    .WithMany(et => et.ReglasComoOrigen)
                    .HasForeignKey(e => e.EtapaOrigenId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_ReglasProgresion_Etapas_Origen");

                entity.HasOne(e => e.EtapaDestino)
                    .WithMany(et => et.ReglasComoDestino)
                    .HasForeignKey(e => e.EtapaDestinoId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_ReglasProgresion_Etapas_Destino");
            });

            // Tabla: Resultado
            modelBuilder.Entity<Resultado>(entity =>
            {
                entity.ToTable("Resultados", "regatas");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                
                entity.Property(e => e.TiempoOficial).HasColumnType("interval");
                entity.Property(e => e.Puntos).HasPrecision(10, 2);
                entity.Property(e => e.VelocidadMedia).HasPrecision(10, 2);
                
                entity.Property(e => e.Estado)
                    .IsRequired()
                    .HasConversion(new EstadoResultadoEnumConverter())
                    .HasMaxLength(20)
                    .HasDefaultValue(EstadoResultadoEnum.Pendiente)
                    .HasSentinel(EstadoResultadoEnum.Pendiente);

                entity.Property(e => e.Observaciones).HasColumnType("text");
                entity.Property(e => e.FechaRegistro).IsRequired().HasDefaultValueSql("NOW()");

                entity.HasOne(e => e.Fase)
                    .WithMany(m => m.Resultados)
                    .HasForeignKey(e => e.FaseId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_Resultados_Fases");

                entity.HasOne(e => e.Inscripcion)
                    .WithMany(i => i.Resultados)
                    .HasForeignKey(e => e.InscripcionId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_Resultados_Inscripciones");
                    
                entity.HasIndex(e => new { e.FaseId, e.Carril }).HasDatabaseName("IX_Resultados_Carril");
            });

            // Tabla: Penalizacion
            modelBuilder.Entity<Penalizacion>(entity =>
            {
                entity.ToTable("Penalizaciones", "regatas");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd();

                // Configurar enums como strings en la BD
                entity.Property(e => e.TipoPenalizacion)
                    .IsRequired()
                    .HasConversion<string>()
                    .HasMaxLength(50);

                entity.Property(e => e.Severidad)
                    .IsRequired()
                    .HasConversion<string>()
                    .HasMaxLength(20);

                entity.Property(e => e.Descripcion)
                    .HasColumnType("text");

                entity.Property(e => e.TiempoPenalizacion)
                    .HasColumnType("interval");

                entity.Property(e => e.FechaRegistro)
                    .IsRequired()
                    .HasDefaultValueSql("NOW()");

                entity.Property(e => e.JuezAsignado)
                    .HasMaxLength(100);

                // Foreign Key
                entity.HasOne(e => e.Resultado)
                    .WithMany(r => r.Penalizaciones)
                    .HasForeignKey(e => e.ResultadoId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_Penalizaciones_Resultados");

                // Índices
                entity.HasIndex(e => e.ResultadoId)
                    .HasDatabaseName("IX_Penalizaciones_ResultadoId");

                entity.HasIndex(e => new { e.ResultadoId, e.TipoPenalizacion })
                    .HasDatabaseName("IX_Penalizaciones_ResultadoTipo");

                entity.HasIndex(e => e.Severidad)
                    .HasDatabaseName("IX_Penalizaciones_Severidad");

                entity.HasIndex(e => e.JuezAsignado)
                    .HasDatabaseName("IX_Penalizaciones_JuezAsignado");

                entity.HasIndex(e => e.TipoPenalizacion)
                    .HasDatabaseName("IX_Penalizaciones_TipoPenalizacion");
            });

            // ============================================
            // SEED DATA PARA TABLAS MAESTRAS
            // ============================================

            modelBuilder.Entity<Sexo>().HasData(
                new Sexo { Id = 1, Nombre = "Masculino" },
                new Sexo { Id = 2, Nombre = "Femenino" },
                new Sexo { Id = 3, Nombre = "Mixto" }
            );

            modelBuilder.Entity<Bote>().HasData(
                new Bote { Id = 1, Tipo = "Kayak Individual" },
                new Bote { Id = 2, Tipo = "Kayak Doble" },
                new Bote { Id = 3, Tipo = "Kayak Cuadruple" },
                new Bote { Id = 4, Tipo = "Canoa Individual" },
                new Bote { Id = 5, Tipo = "Canoa Doble" },
                new Bote { Id = 6, Tipo = "Canoa Cuadruple" }
            );

            modelBuilder.Entity<Categoria>().HasData(
                new Categoria { Id = 1, Nombre = "Pre-Infantil", EdadMin = 9, EdadMax = 10 },
                new Categoria { Id = 2, Nombre = "Infantil", EdadMin = 10, EdadMax = 11 },
                new Categoria { Id = 3, Nombre = "Menor", EdadMin = 12, EdadMax = 13 },
                new Categoria { Id = 4, Nombre = "Cadete", EdadMin = 14, EdadMax = 15 },
                new Categoria { Id = 5, Nombre = "Junior", EdadMin = 16, EdadMax = 17 },
                new Categoria { Id = 6, Nombre = "Sub-23", EdadMin = 18, EdadMax = 22 },
                new Categoria { Id = 7, Nombre = "Senior", EdadMin = 18, EdadMax = 35 },
                new Categoria { Id = 8, Nombre = "Master A", EdadMin = 40, EdadMax = 49 },
                new Categoria { Id = 9, Nombre = "Master B", EdadMin = 50, EdadMax = 59 },
                new Categoria { Id = 10, Nombre = "Master C", EdadMin = 60, EdadMax = 80 },
                new Categoria { Id = 11, Nombre = "Control", EdadMin = 0, EdadMax = 99 }
            );

            modelBuilder.Entity<Distancia>().HasData(
                new Distancia { Id = 1, DistanciaRegata = DistanciaRegataEnum.Metros200 },
                new Distancia { Id = 2, DistanciaRegata = DistanciaRegataEnum.Metros350 },
                new Distancia { Id = 3, DistanciaRegata = DistanciaRegataEnum.Metros400 },
                new Distancia { Id = 4, DistanciaRegata = DistanciaRegataEnum.Metros450 },
                new Distancia { Id = 5, DistanciaRegata = DistanciaRegataEnum.Metros500 },
                new Distancia { Id = 6, DistanciaRegata = DistanciaRegataEnum.Metros1000 },
                new Distancia { Id = 7, DistanciaRegata = DistanciaRegataEnum.Metros1500 },
                new Distancia { Id = 8, DistanciaRegata = DistanciaRegataEnum.Metros2000 },
                new Distancia { Id = 9, DistanciaRegata = DistanciaRegataEnum.Metros3000 },
                new Distancia { Id = 10, DistanciaRegata = DistanciaRegataEnum.Metros5000 },
                new Distancia { Id = 11, DistanciaRegata = DistanciaRegataEnum.Metros10000 },
                new Distancia { Id = 12, DistanciaRegata = DistanciaRegataEnum.Metros12000 },
                new Distancia { Id = 13, DistanciaRegata = DistanciaRegataEnum.Metros15000 },
                new Distancia { Id = 14, DistanciaRegata = DistanciaRegataEnum.Metros18000 },
                new Distancia { Id = 15, DistanciaRegata = DistanciaRegataEnum.Metros22000 },
                new Distancia { Id = 16, DistanciaRegata = DistanciaRegataEnum.Metros30000 }
            );

            // Usuario inicial administrador
            modelBuilder.Entity<Usuario>().HasData(
                new Usuario { 
                    Id = 1, 
                    Username = "admin", 
                    PasswordHash = "$2a$12$R9h/lSAbvI125hcnyqvQDu9fAKDLn6Y8yK/.Vz0uI3492M0h0mY3.", // admin123
                    Email = "admin@sporttrack.com", 
                    Rol = "Admin",
                    FechaCreacion = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    Activo = true
                }
            );
        }

        public class EstadoEventoEnumConverter : ValueConverter<EstadoEventoEnum, string>
        {
            public EstadoEventoEnumConverter()
                : base(
                    v => v.ToString(),
                    v => (EstadoEventoEnum)Enum.Parse(typeof(EstadoEventoEnum), v)
                )
            { }
        }

        public class EstadoInscripcionEnumConverter : ValueConverter<EstadoInscripcionEnum, string>
        {
            public EstadoInscripcionEnumConverter()
                : base(
                    v => v.ToString(),
                    v => (EstadoInscripcionEnum)Enum.Parse(typeof(EstadoInscripcionEnum), v)
                )
            { }
        }

        public class EstadoResultadoEnumConverter : ValueConverter<EstadoResultadoEnum, string>
        {
            public EstadoResultadoEnumConverter()
                : base(
                    v => v.ToString(),
                    v => (EstadoResultadoEnum)Enum.Parse(typeof(EstadoResultadoEnum), v)
                )
            { }
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            // Actualizar automáticamente las fechas de actualización
            var entries = ChangeTracker
                .Entries()
                .Where(e => e.Entity is Resultado &&
                    (e.State == EntityState.Modified));

            foreach (var entityEntry in entries)
            {
                ((Resultado)entityEntry.Entity).FechaActualizacion = DateTime.UtcNow;
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
