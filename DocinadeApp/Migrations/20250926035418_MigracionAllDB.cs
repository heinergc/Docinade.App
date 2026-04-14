using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RubricasApp.Web.Migrations
{
    /// <inheritdoc />
    public partial class MigracionAllDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Apellidos = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    LastLoginDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NumeroIdentificacion = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Institucion = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Departamento = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    UltimoAcceso = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    Observaciones = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AuditLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Action = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    EntityType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    EntityId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    EntityName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    OldValues = table.Column<string>(type: "NVARCHAR(MAX)", nullable: true),
                    NewValues = table.Column<string>(type: "NVARCHAR(MAX)", nullable: true),
                    IpAddress = table.Column<string>(type: "nvarchar(45)", maxLength: 45, nullable: false),
                    UserAgent = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    LogLevel = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "Information"),
                    AdditionalInfo = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Success = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    ErrorMessage = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    DurationMs = table.Column<long>(type: "bigint", nullable: true),
                    SessionId = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                    ClientInfo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Referrer = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Metadata = table.Column<string>(type: "NVARCHAR(MAX)", nullable: true),
                    HttpMethod = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    RequestUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ResponseStatusCode = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ConfiguracionesSistema",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Clave = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Valor = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    UsuarioModificacion = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfiguracionesSistema", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GruposCalificacion",
                columns: table => new
                {
                    IdGrupo = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombreGrupo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Estado = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GruposCalificacion", x => x.IdGrupo);
                });

            migrationBuilder.CreateTable(
                name: "InstrumentosEvaluacion",
                columns: table => new
                {
                    InstrumentoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InstrumentosEvaluacion", x => x.InstrumentoId);
                });

            migrationBuilder.CreateTable(
                name: "Materias",
                columns: table => new
                {
                    MateriaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Codigo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Creditos = table.Column<int>(type: "int", nullable: false),
                    Activa = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    Tipo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CicloSugerido = table.Column<int>(type: "int", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Estado = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Materias", x => x.MateriaId);
                });

            migrationBuilder.CreateTable(
                name: "PeriodosAcademicos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Año = table.Column<int>(type: "int", nullable: false),
                    Anio = table.Column<int>(type: "int", nullable: false),
                    Ciclo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    FechaInicio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaFin = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    Codigo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Tipo = table.Column<int>(type: "int", nullable: false),
                    NumeroPeriodo = table.Column<int>(type: "int", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Estado = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true, defaultValue: "Activo")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PeriodosAcademicos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TiposGrupo",
                columns: table => new
                {
                    IdTipoGrupo = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    Estado = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "Activo")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposGrupo", x => x.IdTipoGrupo);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AuditoriasOperaciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TipoOperacion = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TablaAfectada = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    RegistroId = table.Column<int>(type: "int", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Motivo = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    DireccionIP = table.Column<string>(type: "nvarchar(45)", maxLength: 45, nullable: true),
                    UserAgent = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    FechaOperacion = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    UsuarioId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    OperacionExitosa = table.Column<bool>(type: "bit", nullable: false),
                    MensajeError = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    DatosAnteriores = table.Column<string>(type: "ntext", nullable: true),
                    DatosNuevos = table.Column<string>(type: "ntext", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditoriasOperaciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AuditoriasOperaciones_AspNetUsers_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "NivelesCalificacion",
                columns: table => new
                {
                    IdNivel = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombreNivel = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    OrdenNivel = table.Column<int>(type: "int", nullable: true),
                    IdGrupo = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NivelesCalificacion", x => x.IdNivel);
                    table.ForeignKey(
                        name: "FK_NivelesCalificacion_GruposCalificacion_IdGrupo",
                        column: x => x.IdGrupo,
                        principalTable: "GruposCalificacion",
                        principalColumn: "IdGrupo",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Rubricas",
                columns: table => new
                {
                    IdRubrica = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombreRubrica = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    Estado = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true, defaultValue: "ACTIVO"),
                    EsPublica = table.Column<int>(type: "int", nullable: false),
                    IdGrupo = table.Column<int>(type: "int", nullable: true),
                    CreadoPorId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    ModificadoPorId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Titulo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Vigente = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rubricas", x => x.IdRubrica);
                    table.ForeignKey(
                        name: "FK_Rubricas_AspNetUsers_CreadoPorId",
                        column: x => x.CreadoPorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Rubricas_AspNetUsers_ModificadoPorId",
                        column: x => x.ModificadoPorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Rubricas_GruposCalificacion_IdGrupo",
                        column: x => x.IdGrupo,
                        principalTable: "GruposCalificacion",
                        principalColumn: "IdGrupo",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "MateriaRequisitos",
                columns: table => new
                {
                    MateriaId = table.Column<int>(type: "int", nullable: false),
                    RequisitoId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MateriaRequisitos", x => new { x.MateriaId, x.RequisitoId });
                    table.ForeignKey(
                        name: "FK_MateriaRequisitos_Materias_MateriaId",
                        column: x => x.MateriaId,
                        principalTable: "Materias",
                        principalColumn: "MateriaId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MateriaRequisitos_Materias_RequisitoId",
                        column: x => x.RequisitoId,
                        principalTable: "Materias",
                        principalColumn: "MateriaId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CuadernosCalificadores",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MateriaId = table.Column<int>(type: "int", nullable: false),
                    PeriodoAcademicoId = table.Column<int>(type: "int", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    Estado = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "ACTIVO"),
                    FechaCierre = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CuadernosCalificadores", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CuadernosCalificadores_Materias_MateriaId",
                        column: x => x.MateriaId,
                        principalTable: "Materias",
                        principalColumn: "MateriaId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CuadernosCalificadores_PeriodosAcademicos_PeriodoAcademicoId",
                        column: x => x.PeriodoAcademicoId,
                        principalTable: "PeriodosAcademicos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Estudiantes",
                columns: table => new
                {
                    IdEstudiante = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Apellidos = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NumeroId = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    DireccionCorreo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Institucion = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Grupos = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Año = table.Column<int>(type: "int", nullable: false),
                    PeriodoAcademicoId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Estudiantes", x => x.IdEstudiante);
                    table.ForeignKey(
                        name: "FK_Estudiantes_PeriodosAcademicos_PeriodoAcademicoId",
                        column: x => x.PeriodoAcademicoId,
                        principalTable: "PeriodosAcademicos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InstrumentoMaterias",
                columns: table => new
                {
                    InstrumentoEvaluacionId = table.Column<int>(type: "int", nullable: false),
                    MateriaId = table.Column<int>(type: "int", nullable: false),
                    PeriodoAcademicoId = table.Column<int>(type: "int", nullable: false),
                    FechaAsignacion = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    OrdenPresentacion = table.Column<int>(type: "int", nullable: true),
                    EsObligatorio = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    Observaciones = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InstrumentoMaterias", x => new { x.InstrumentoEvaluacionId, x.MateriaId });
                    table.ForeignKey(
                        name: "FK_InstrumentoMaterias_InstrumentosEvaluacion_InstrumentoEvaluacionId",
                        column: x => x.InstrumentoEvaluacionId,
                        principalTable: "InstrumentosEvaluacion",
                        principalColumn: "InstrumentoId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InstrumentoMaterias_Materias_MateriaId",
                        column: x => x.MateriaId,
                        principalTable: "Materias",
                        principalColumn: "MateriaId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InstrumentoMaterias_PeriodosAcademicos_PeriodoAcademicoId",
                        column: x => x.PeriodoAcademicoId,
                        principalTable: "PeriodosAcademicos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MateriaPeriodos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MateriaId = table.Column<int>(type: "int", nullable: false),
                    PeriodoAcademicoId = table.Column<int>(type: "int", nullable: false),
                    Cupo = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    Estado = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "Abierta"),
                    FechaPublicacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    Observaciones = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    PeriodoAcademicoId1 = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MateriaPeriodos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MateriaPeriodos_Materias_MateriaId",
                        column: x => x.MateriaId,
                        principalTable: "Materias",
                        principalColumn: "MateriaId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MateriaPeriodos_PeriodosAcademicos_PeriodoAcademicoId",
                        column: x => x.PeriodoAcademicoId,
                        principalTable: "PeriodosAcademicos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MateriaPeriodos_PeriodosAcademicos_PeriodoAcademicoId1",
                        column: x => x.PeriodoAcademicoId1,
                        principalTable: "PeriodosAcademicos",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "GruposEstudiantes",
                columns: table => new
                {
                    GrupoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Codigo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IdTipoGrupo = table.Column<int>(type: "int", nullable: false),
                    TipoGrupo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Nivel = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CapacidadMaxima = table.Column<int>(type: "int", nullable: true),
                    PeriodoAcademicoId = table.Column<int>(type: "int", nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreadoPorId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    Observaciones = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GruposEstudiantes", x => x.GrupoId);
                    table.ForeignKey(
                        name: "FK_GruposEstudiantes_AspNetUsers_CreadoPorId",
                        column: x => x.CreadoPorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_GruposEstudiantes_PeriodosAcademicos_PeriodoAcademicoId",
                        column: x => x.PeriodoAcademicoId,
                        principalTable: "PeriodosAcademicos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GruposEstudiantes_TiposGrupo_IdTipoGrupo",
                        column: x => x.IdTipoGrupo,
                        principalTable: "TiposGrupo",
                        principalColumn: "IdTipoGrupo",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "InstrumentoRubricas",
                columns: table => new
                {
                    InstrumentoEvaluacionId = table.Column<int>(type: "int", nullable: false),
                    RubricaId = table.Column<int>(type: "int", nullable: false),
                    InstrumentoId = table.Column<int>(type: "int", nullable: false),
                    FechaAsignacion = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    OrdenPresentacion = table.Column<int>(type: "int", nullable: true),
                    EsObligatorio = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    Ponderacion = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false, defaultValue: 0m)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InstrumentoRubricas", x => new { x.InstrumentoEvaluacionId, x.RubricaId });
                    table.ForeignKey(
                        name: "FK_InstrumentoRubricas_InstrumentosEvaluacion_InstrumentoEvaluacionId",
                        column: x => x.InstrumentoEvaluacionId,
                        principalTable: "InstrumentosEvaluacion",
                        principalColumn: "InstrumentoId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InstrumentoRubricas_Rubricas_RubricaId",
                        column: x => x.RubricaId,
                        principalTable: "Rubricas",
                        principalColumn: "IdRubrica",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ItemsEvaluacion",
                columns: table => new
                {
                    IdItem = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdRubrica = table.Column<int>(type: "int", nullable: false),
                    NombreItem = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OrdenItem = table.Column<int>(type: "int", nullable: true),
                    Peso = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false, defaultValue: 0m)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemsEvaluacion", x => x.IdItem);
                    table.ForeignKey(
                        name: "FK_ItemsEvaluacion_Rubricas_IdRubrica",
                        column: x => x.IdRubrica,
                        principalTable: "Rubricas",
                        principalColumn: "IdRubrica",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RubricaNiveles",
                columns: table => new
                {
                    IdRubrica = table.Column<int>(type: "int", nullable: false),
                    IdNivel = table.Column<int>(type: "int", nullable: false),
                    OrdenEnRubrica = table.Column<int>(type: "int", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RubricaNiveles", x => new { x.IdRubrica, x.IdNivel });
                    table.ForeignKey(
                        name: "FK_RubricaNiveles_NivelesCalificacion_IdNivel",
                        column: x => x.IdNivel,
                        principalTable: "NivelesCalificacion",
                        principalColumn: "IdNivel",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RubricaNiveles_Rubricas_IdRubrica",
                        column: x => x.IdRubrica,
                        principalTable: "Rubricas",
                        principalColumn: "IdRubrica",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CuadernoInstrumentos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CuadernoCalificadorId = table.Column<int>(type: "int", nullable: false),
                    RubricaId = table.Column<int>(type: "int", nullable: false),
                    PonderacionPorcentaje = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    Orden = table.Column<int>(type: "int", nullable: false),
                    EsObligatorio = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CuadernoInstrumentos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CuadernoInstrumentos_CuadernosCalificadores_CuadernoCalificadorId",
                        column: x => x.CuadernoCalificadorId,
                        principalTable: "CuadernosCalificadores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CuadernoInstrumentos_Rubricas_RubricaId",
                        column: x => x.RubricaId,
                        principalTable: "Rubricas",
                        principalColumn: "IdRubrica",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Evaluaciones",
                columns: table => new
                {
                    IdEvaluacion = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdEstudiante = table.Column<int>(type: "int", nullable: false),
                    IdRubrica = table.Column<int>(type: "int", nullable: false),
                    FechaEvaluacion = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    TotalPuntos = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: true),
                    Observaciones = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Estado = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "BORRADOR"),
                    EvaluadoPorId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    FechaFinalizacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TiempoEvaluacionMinutos = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Evaluaciones", x => x.IdEvaluacion);
                    table.ForeignKey(
                        name: "FK_Evaluaciones_AspNetUsers_EvaluadoPorId",
                        column: x => x.EvaluadoPorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Evaluaciones_Estudiantes_IdEstudiante",
                        column: x => x.IdEstudiante,
                        principalTable: "Estudiantes",
                        principalColumn: "IdEstudiante",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Evaluaciones_Rubricas_IdRubrica",
                        column: x => x.IdRubrica,
                        principalTable: "Rubricas",
                        principalColumn: "IdRubrica",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EstudianteGrupos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EstudianteId = table.Column<int>(type: "int", nullable: false),
                    GrupoId = table.Column<int>(type: "int", nullable: false),
                    FechaAsignacion = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    FechaDesasignacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Estado = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "Activo"),
                    AsignadoPorId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    MotivoAsignacion = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    EsGrupoPrincipal = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EstudianteGrupos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EstudianteGrupos_AspNetUsers_AsignadoPorId",
                        column: x => x.AsignadoPorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_EstudianteGrupos_Estudiantes_EstudianteId",
                        column: x => x.EstudianteId,
                        principalTable: "Estudiantes",
                        principalColumn: "IdEstudiante",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EstudianteGrupos_GruposEstudiantes_GrupoId",
                        column: x => x.GrupoId,
                        principalTable: "GruposEstudiantes",
                        principalColumn: "GrupoId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GrupoMaterias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GrupoId = table.Column<int>(type: "int", nullable: false),
                    MateriaId = table.Column<int>(type: "int", nullable: false),
                    FechaAsignacion = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    Estado = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "Activo"),
                    Observaciones = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GrupoMaterias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GrupoMaterias_GruposEstudiantes_GrupoId",
                        column: x => x.GrupoId,
                        principalTable: "GruposEstudiantes",
                        principalColumn: "GrupoId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GrupoMaterias_Materias_MateriaId",
                        column: x => x.MateriaId,
                        principalTable: "Materias",
                        principalColumn: "MateriaId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ValoresRubrica",
                columns: table => new
                {
                    IdValor = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdRubrica = table.Column<int>(type: "int", nullable: false),
                    IdItem = table.Column<int>(type: "int", nullable: false),
                    IdNivel = table.Column<int>(type: "int", nullable: false),
                    ValorPuntos = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false, defaultValue: 0m)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ValoresRubrica", x => x.IdValor);
                    table.ForeignKey(
                        name: "FK_ValoresRubrica_ItemsEvaluacion_IdItem",
                        column: x => x.IdItem,
                        principalTable: "ItemsEvaluacion",
                        principalColumn: "IdItem",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ValoresRubrica_NivelesCalificacion_IdNivel",
                        column: x => x.IdNivel,
                        principalTable: "NivelesCalificacion",
                        principalColumn: "IdNivel",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ValoresRubrica_Rubricas_IdRubrica",
                        column: x => x.IdRubrica,
                        principalTable: "Rubricas",
                        principalColumn: "IdRubrica",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DetallesEvaluacion",
                columns: table => new
                {
                    IdDetalle = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdEvaluacion = table.Column<int>(type: "int", nullable: false),
                    IdItem = table.Column<int>(type: "int", nullable: false),
                    IdNivel = table.Column<int>(type: "int", nullable: false),
                    PuntosObtenidos = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DetallesEvaluacion", x => x.IdDetalle);
                    table.ForeignKey(
                        name: "FK_DetallesEvaluacion_Evaluaciones_IdEvaluacion",
                        column: x => x.IdEvaluacion,
                        principalTable: "Evaluaciones",
                        principalColumn: "IdEvaluacion",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DetallesEvaluacion_ItemsEvaluacion_IdItem",
                        column: x => x.IdItem,
                        principalTable: "ItemsEvaluacion",
                        principalColumn: "IdItem",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DetallesEvaluacion_NivelesCalificacion_IdNivel",
                        column: x => x.IdNivel,
                        principalTable: "NivelesCalificacion",
                        principalColumn: "IdNivel",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_CreatedDate",
                table: "AspNetUsers",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_IsActive",
                table: "AspNetUsers",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_LastLoginDate",
                table: "AspNetUsers",
                column: "LastLoginDate");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_NumeroIdentificacion",
                table: "AspNetUsers",
                column: "NumeroIdentificacion",
                unique: true,
                filter: "[NumeroIdentificacion] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_Action",
                table: "AuditLogs",
                column: "Action");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_EntityType",
                table: "AuditLogs",
                column: "EntityType");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_EntityType_EntityId",
                table: "AuditLogs",
                columns: new[] { "EntityType", "EntityId" });

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_Timestamp",
                table: "AuditLogs",
                column: "Timestamp");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_UserId",
                table: "AuditLogs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditoriaOperacion_FechaOperacion",
                table: "AuditoriasOperaciones",
                column: "FechaOperacion");

            migrationBuilder.CreateIndex(
                name: "IX_AuditoriaOperacion_RegistroId",
                table: "AuditoriasOperaciones",
                column: "RegistroId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditoriaOperacion_Tabla_Registro",
                table: "AuditoriasOperaciones",
                columns: new[] { "TablaAfectada", "RegistroId" });

            migrationBuilder.CreateIndex(
                name: "IX_AuditoriaOperacion_TablaAfectada",
                table: "AuditoriasOperaciones",
                column: "TablaAfectada");

            migrationBuilder.CreateIndex(
                name: "IX_AuditoriaOperacion_UsuarioId",
                table: "AuditoriasOperaciones",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_ConfiguracionesSistema_Clave",
                table: "ConfiguracionesSistema",
                column: "Clave",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CuadernoInstrumentos_CuadernoCalificadorId",
                table: "CuadernoInstrumentos",
                column: "CuadernoCalificadorId");

            migrationBuilder.CreateIndex(
                name: "IX_CuadernoInstrumentos_RubricaId",
                table: "CuadernoInstrumentos",
                column: "RubricaId");

            migrationBuilder.CreateIndex(
                name: "IX_CuadernosCalificadores_MateriaId",
                table: "CuadernosCalificadores",
                column: "MateriaId");

            migrationBuilder.CreateIndex(
                name: "IX_CuadernosCalificadores_PeriodoAcademicoId",
                table: "CuadernosCalificadores",
                column: "PeriodoAcademicoId");

            migrationBuilder.CreateIndex(
                name: "IX_DetallesEvaluacion_IdEvaluacion",
                table: "DetallesEvaluacion",
                column: "IdEvaluacion");

            migrationBuilder.CreateIndex(
                name: "IX_DetallesEvaluacion_IdItem",
                table: "DetallesEvaluacion",
                column: "IdItem");

            migrationBuilder.CreateIndex(
                name: "IX_DetallesEvaluacion_IdNivel",
                table: "DetallesEvaluacion",
                column: "IdNivel");

            migrationBuilder.CreateIndex(
                name: "IX_EstudianteGrupos_AsignadoPorId",
                table: "EstudianteGrupos",
                column: "AsignadoPorId");

            migrationBuilder.CreateIndex(
                name: "IX_EstudianteGrupos_Estado",
                table: "EstudianteGrupos",
                column: "Estado");

            migrationBuilder.CreateIndex(
                name: "IX_EstudianteGrupos_FechaAsignacion",
                table: "EstudianteGrupos",
                column: "FechaAsignacion");

            migrationBuilder.CreateIndex(
                name: "IX_EstudianteGrupos_GrupoId",
                table: "EstudianteGrupos",
                column: "GrupoId");

            migrationBuilder.CreateIndex(
                name: "IX_EstudianteGrupos_Unique_Activo",
                table: "EstudianteGrupos",
                columns: new[] { "EstudianteId", "GrupoId", "Estado" },
                filter: "[Estado] = 'Activo'");

            migrationBuilder.CreateIndex(
                name: "IX_Estudiantes_DireccionCorreo",
                table: "Estudiantes",
                column: "DireccionCorreo");

            migrationBuilder.CreateIndex(
                name: "IX_Estudiantes_NumeroId",
                table: "Estudiantes",
                column: "NumeroId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Estudiantes_PeriodoAcademicoId",
                table: "Estudiantes",
                column: "PeriodoAcademicoId");

            migrationBuilder.CreateIndex(
                name: "IX_Evaluaciones_EvaluadoPorId",
                table: "Evaluaciones",
                column: "EvaluadoPorId");

            migrationBuilder.CreateIndex(
                name: "IX_Evaluaciones_IdEstudiante",
                table: "Evaluaciones",
                column: "IdEstudiante");

            migrationBuilder.CreateIndex(
                name: "IX_Evaluaciones_IdRubrica",
                table: "Evaluaciones",
                column: "IdRubrica");

            migrationBuilder.CreateIndex(
                name: "IX_GrupoMaterias_MateriaId",
                table: "GrupoMaterias",
                column: "MateriaId");

            migrationBuilder.CreateIndex(
                name: "IX_GrupoMaterias_Unique",
                table: "GrupoMaterias",
                columns: new[] { "GrupoId", "MateriaId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GruposEstudiantes_Codigo_Periodo",
                table: "GruposEstudiantes",
                columns: new[] { "Codigo", "PeriodoAcademicoId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GruposEstudiantes_CreadoPorId",
                table: "GruposEstudiantes",
                column: "CreadoPorId");

            migrationBuilder.CreateIndex(
                name: "IX_GruposEstudiantes_Estado",
                table: "GruposEstudiantes",
                column: "Estado");

            migrationBuilder.CreateIndex(
                name: "IX_GruposEstudiantes_IdTipoGrupo",
                table: "GruposEstudiantes",
                column: "IdTipoGrupo");

            migrationBuilder.CreateIndex(
                name: "IX_GruposEstudiantes_PeriodoAcademicoId",
                table: "GruposEstudiantes",
                column: "PeriodoAcademicoId");

            migrationBuilder.CreateIndex(
                name: "IX_InstrumentoMaterias_MateriaId",
                table: "InstrumentoMaterias",
                column: "MateriaId");

            migrationBuilder.CreateIndex(
                name: "IX_InstrumentoMaterias_PeriodoAcademicoId",
                table: "InstrumentoMaterias",
                column: "PeriodoAcademicoId");

            migrationBuilder.CreateIndex(
                name: "IX_InstrumentoRubricas_RubricaId",
                table: "InstrumentoRubricas",
                column: "RubricaId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemsEvaluacion_IdRubrica",
                table: "ItemsEvaluacion",
                column: "IdRubrica");

            migrationBuilder.CreateIndex(
                name: "IX_MateriaPeriodos_MateriaId",
                table: "MateriaPeriodos",
                column: "MateriaId");

            migrationBuilder.CreateIndex(
                name: "IX_MateriaPeriodos_PeriodoAcademicoId",
                table: "MateriaPeriodos",
                column: "PeriodoAcademicoId");

            migrationBuilder.CreateIndex(
                name: "IX_MateriaPeriodos_PeriodoAcademicoId1",
                table: "MateriaPeriodos",
                column: "PeriodoAcademicoId1");

            migrationBuilder.CreateIndex(
                name: "IX_MateriaRequisitos_RequisitoId",
                table: "MateriaRequisitos",
                column: "RequisitoId");

            migrationBuilder.CreateIndex(
                name: "IX_Materias_Codigo",
                table: "Materias",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_NivelesCalificacion_IdGrupo",
                table: "NivelesCalificacion",
                column: "IdGrupo");

            migrationBuilder.CreateIndex(
                name: "IX_PeriodosAcademicos_Estado",
                table: "PeriodosAcademicos",
                column: "Estado");

            migrationBuilder.CreateIndex(
                name: "IX_PeriodosAcademicos_FechaFin",
                table: "PeriodosAcademicos",
                column: "FechaFin");

            migrationBuilder.CreateIndex(
                name: "IX_PeriodosAcademicos_FechaInicio",
                table: "PeriodosAcademicos",
                column: "FechaInicio");

            migrationBuilder.CreateIndex(
                name: "IX_RubricaNiveles_IdNivel",
                table: "RubricaNiveles",
                column: "IdNivel");

            migrationBuilder.CreateIndex(
                name: "IX_Rubricas_CreadoPorId",
                table: "Rubricas",
                column: "CreadoPorId");

            migrationBuilder.CreateIndex(
                name: "IX_Rubricas_IdGrupo",
                table: "Rubricas",
                column: "IdGrupo");

            migrationBuilder.CreateIndex(
                name: "IX_Rubricas_ModificadoPorId",
                table: "Rubricas",
                column: "ModificadoPorId");

            migrationBuilder.CreateIndex(
                name: "IX_TiposGrupo_Estado",
                table: "TiposGrupo",
                column: "Estado");

            migrationBuilder.CreateIndex(
                name: "IX_TiposGrupo_FechaRegistro",
                table: "TiposGrupo",
                column: "FechaRegistro");

            migrationBuilder.CreateIndex(
                name: "IX_ValoresRubrica_IdItem",
                table: "ValoresRubrica",
                column: "IdItem");

            migrationBuilder.CreateIndex(
                name: "IX_ValoresRubrica_IdNivel",
                table: "ValoresRubrica",
                column: "IdNivel");

            migrationBuilder.CreateIndex(
                name: "IX_ValorRubrica_Unique",
                table: "ValoresRubrica",
                columns: new[] { "IdRubrica", "IdItem", "IdNivel" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "AuditLogs");

            migrationBuilder.DropTable(
                name: "AuditoriasOperaciones");

            migrationBuilder.DropTable(
                name: "ConfiguracionesSistema");

            migrationBuilder.DropTable(
                name: "CuadernoInstrumentos");

            migrationBuilder.DropTable(
                name: "DetallesEvaluacion");

            migrationBuilder.DropTable(
                name: "EstudianteGrupos");

            migrationBuilder.DropTable(
                name: "GrupoMaterias");

            migrationBuilder.DropTable(
                name: "InstrumentoMaterias");

            migrationBuilder.DropTable(
                name: "InstrumentoRubricas");

            migrationBuilder.DropTable(
                name: "MateriaPeriodos");

            migrationBuilder.DropTable(
                name: "MateriaRequisitos");

            migrationBuilder.DropTable(
                name: "RubricaNiveles");

            migrationBuilder.DropTable(
                name: "ValoresRubrica");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "CuadernosCalificadores");

            migrationBuilder.DropTable(
                name: "Evaluaciones");

            migrationBuilder.DropTable(
                name: "GruposEstudiantes");

            migrationBuilder.DropTable(
                name: "InstrumentosEvaluacion");

            migrationBuilder.DropTable(
                name: "ItemsEvaluacion");

            migrationBuilder.DropTable(
                name: "NivelesCalificacion");

            migrationBuilder.DropTable(
                name: "Materias");

            migrationBuilder.DropTable(
                name: "Estudiantes");

            migrationBuilder.DropTable(
                name: "TiposGrupo");

            migrationBuilder.DropTable(
                name: "Rubricas");

            migrationBuilder.DropTable(
                name: "PeriodosAcademicos");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "GruposCalificacion");
        }
    }
}
