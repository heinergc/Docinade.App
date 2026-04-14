using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RubricasApp.Web.Migrations
{
    /// <inheritdoc />
    public partial class ChangeProfesorGuiaEstadoToBit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Cambiar el tipo de columna Estado en ProfesorGuia de nvarchar a bit
            // Primero, convertir valores existentes: cualquier valor no vacío será true, vacío o null será false
            migrationBuilder.Sql(@"
                UPDATE ProfesorGuia 
                SET Estado = CASE 
                    WHEN Estado IS NULL OR Estado = '' OR Estado = '0' OR Estado = 'false' OR Estado = 'Inactivo' THEN '0'
                    ELSE '1'
                END
                WHERE TRY_CAST(Estado AS bit) IS NULL
            ");

            // Alterar la columna Estado de nvarchar a bit
            migrationBuilder.AlterColumn<bool>(
                name: "Estado",
                table: "ProfesorGuia",
                type: "bit",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EscuelaId",
                table: "Materias",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ParametrosInstitucion",
                columns: table => new
                {
                    IdParametro = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Clave = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Valor = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    TipoDato = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Categoria = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModificadoPorId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParametrosInstitucion", x => x.IdParametro);
                });

            migrationBuilder.CreateTable(
                name: "Provincias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Codigo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Estado = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Provincias", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TiposFalta",
                columns: table => new
                {
                    IdTipoFalta = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Definicion = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    Ejemplos = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    AccionCorrectiva = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    RebajoMinimo = table.Column<int>(type: "int", nullable: false),
                    RebajoMaximo = table.Column<int>(type: "int", nullable: false),
                    Orden = table.Column<int>(type: "int", nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposFalta", x => x.IdTipoFalta);
                });

            migrationBuilder.CreateTable(
                name: "Cantones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProvinciaId = table.Column<int>(type: "int", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Codigo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Estado = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cantones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Cantones_Provincias_ProvinciaId",
                        column: x => x.ProvinciaId,
                        principalTable: "Provincias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BoletasConducta",
                columns: table => new
                {
                    IdBoleta = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdEstudiante = table.Column<int>(type: "int", nullable: false),
                    IdTipoFalta = table.Column<int>(type: "int", nullable: false),
                    IdPeriodo = table.Column<int>(type: "int", nullable: false),
                    RebajoAplicado = table.Column<int>(type: "int", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    RutaEvidencia = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    DocenteEmisorId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    FechaEmision = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ProfesorGuiaId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    FechaNotificacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NotificacionEnviada = table.Column<bool>(type: "bit", nullable: false),
                    ObservacionesProfesorGuia = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Estado = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MotivoAnulacion = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    FechaAnulacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AnuladaPorId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BoletasConducta", x => x.IdBoleta);
                    table.ForeignKey(
                        name: "FK_BoletasConducta_AspNetUsers_AnuladaPorId",
                        column: x => x.AnuladaPorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_BoletasConducta_AspNetUsers_DocenteEmisorId",
                        column: x => x.DocenteEmisorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BoletasConducta_AspNetUsers_ProfesorGuiaId",
                        column: x => x.ProfesorGuiaId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_BoletasConducta_Estudiantes_IdEstudiante",
                        column: x => x.IdEstudiante,
                        principalTable: "Estudiantes",
                        principalColumn: "IdEstudiante",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BoletasConducta_PeriodosAcademicos_IdPeriodo",
                        column: x => x.IdPeriodo,
                        principalTable: "PeriodosAcademicos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BoletasConducta_TiposFalta_IdTipoFalta",
                        column: x => x.IdTipoFalta,
                        principalTable: "TiposFalta",
                        principalColumn: "IdTipoFalta",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Distritos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CantonId = table.Column<int>(type: "int", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Codigo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Estado = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Distritos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Distritos_Cantones_CantonId",
                        column: x => x.CantonId,
                        principalTable: "Cantones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Instituciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Siglas = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    TipoInstitucion = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CodigoMEP = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Telefono = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    SitioWeb = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Direccion = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    DistritoId = table.Column<int>(type: "int", nullable: true),
                    Estado = table.Column<bool>(type: "bit", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Instituciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Instituciones_Distritos_DistritoId",
                        column: x => x.DistritoId,
                        principalTable: "Distritos",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Facultades",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InstitucionId = table.Column<int>(type: "int", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Codigo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Decano = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Telefono = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Estado = table.Column<bool>(type: "bit", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Facultades", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Facultades_Instituciones_InstitucionId",
                        column: x => x.InstitucionId,
                        principalTable: "Instituciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Escuelas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FacultadId = table.Column<int>(type: "int", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Codigo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Director = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Telefono = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Estado = table.Column<bool>(type: "bit", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Escuelas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Escuelas_Facultades_FacultadId",
                        column: x => x.FacultadId,
                        principalTable: "Facultades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Profesores",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombres = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PrimerApellido = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SegundoApellido = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Cedula = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TipoCedula = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Sexo = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: true),
                    FechaNacimiento = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EstadoCivil = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Nacionalidad = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    EmailPersonal = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    EmailInstitucional = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    TelefonoFijo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    TelefonoCelular = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TelefonoOficina = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Extension = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    DireccionExacta = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: true),
                    ProvinciaId = table.Column<int>(type: "int", nullable: true),
                    CantonId = table.Column<int>(type: "int", nullable: true),
                    DistritoId = table.Column<int>(type: "int", nullable: true),
                    CodigoPostal = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    GradoAcademico = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TituloAcademico = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    InstitucionGraduacion = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PaisGraduacion = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AnioGraduacion = table.Column<int>(type: "int", nullable: true),
                    NumeroColegiadoProfesional = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    EscuelaId = table.Column<int>(type: "int", nullable: true),
                    CodigoEmpleado = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    FechaIngreso = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaRetiro = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TipoContrato = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    RegimenLaboral = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CategoriaLaboral = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    TipoJornada = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    HorasLaborales = table.Column<decimal>(type: "decimal(4,2)", nullable: true),
                    SalarioBase = table.Column<decimal>(type: "decimal(12,2)", nullable: true),
                    CuentaBancaria = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    TipoCuenta = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    BancoNombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    EsDirector = table.Column<bool>(type: "bit", nullable: false),
                    EsCoordinador = table.Column<bool>(type: "bit", nullable: false),
                    EsDecano = table.Column<bool>(type: "bit", nullable: false),
                    CargoAdministrativo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    FechaInicioCargoAdmin = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PuedeCrearRubricas = table.Column<bool>(type: "bit", nullable: false),
                    PuedeEvaluarEstudiantes = table.Column<bool>(type: "bit", nullable: false),
                    PuedeVerReportes = table.Column<bool>(type: "bit", nullable: false),
                    EsAdministradorSistema = table.Column<bool>(type: "bit", nullable: false),
                    PuedeGestionarUsuarios = table.Column<bool>(type: "bit", nullable: false),
                    Estado = table.Column<bool>(type: "bit", nullable: false),
                    MotivoInactividad = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    NotificacionesEmail = table.Column<bool>(type: "bit", nullable: false),
                    NotificacionesSMS = table.Column<bool>(type: "bit", nullable: false),
                    AreasEspecializacion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IdiomasHabla = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    NivelIngles = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    ExperienciaDocente = table.Column<int>(type: "int", nullable: true),
                    ContactoEmergenciaNombre = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    ContactoEmergenciaParentesco = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ContactoEmergenciaTelefono = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreadoPor = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModificadoPor = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Version = table.Column<int>(type: "int", nullable: false),
                    UsuarioId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Profesores", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Profesores_Cantones_CantonId",
                        column: x => x.CantonId,
                        principalTable: "Cantones",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Profesores_Distritos_DistritoId",
                        column: x => x.DistritoId,
                        principalTable: "Distritos",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Profesores_Escuelas_EscuelaId",
                        column: x => x.EscuelaId,
                        principalTable: "Escuelas",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Profesores_Provincias_ProvinciaId",
                        column: x => x.ProvinciaId,
                        principalTable: "Provincias",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ProfesorCapacitacion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProfesorId = table.Column<int>(type: "int", nullable: false),
                    NombreCapacitacion = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    InstitucionOrganizadora = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    TipoCapacitacion = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Modalidad = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    FechaInicio = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FechaFin = table.Column<DateTime>(type: "datetime2", nullable: true),
                    HorasCapacitacion = table.Column<int>(type: "int", nullable: true),
                    CertificadoObtenido = table.Column<bool>(type: "bit", nullable: false),
                    CalificacionObtenida = table.Column<decimal>(type: "decimal(4,2)", nullable: true),
                    AreaConocimiento = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProfesorCapacitacion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProfesorCapacitacion_Profesores_ProfesorId",
                        column: x => x.ProfesorId,
                        principalTable: "Profesores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProfesorExperienciaLaboral",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProfesorId = table.Column<int>(type: "int", nullable: false),
                    NombreInstitucion = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CargoDesempenado = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    TipoInstitucion = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    FechaInicio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaFin = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TrabajandoActualmente = table.Column<bool>(type: "bit", nullable: false),
                    DescripcionFunciones = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    TipoContrato = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    JornadaLaboral = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProfesorExperienciaLaboral", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProfesorExperienciaLaboral_Profesores_ProfesorId",
                        column: x => x.ProfesorId,
                        principalTable: "Profesores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProfesorFormacionAcademica",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProfesorId = table.Column<int>(type: "int", nullable: false),
                    TipoFormacion = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TituloObtenido = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    InstitucionEducativa = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    PaisInstitucion = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AnioInicio = table.Column<int>(type: "int", nullable: true),
                    AnioFinalizacion = table.Column<int>(type: "int", nullable: true),
                    EnCurso = table.Column<bool>(type: "bit", nullable: false),
                    PromedioGeneral = table.Column<decimal>(type: "decimal(4,2)", nullable: true),
                    EsTituloReconocidoCONARE = table.Column<bool>(type: "bit", nullable: false),
                    NumeroReconocimiento = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProfesorFormacionAcademica", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProfesorFormacionAcademica_Profesores_ProfesorId",
                        column: x => x.ProfesorId,
                        principalTable: "Profesores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProfesorGrupo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProfesorId = table.Column<int>(type: "int", nullable: false),
                    GrupoId = table.Column<int>(type: "int", nullable: false),
                    PeriodoAcademicoId = table.Column<int>(type: "int", nullable: false),
                    MateriaId = table.Column<int>(type: "int", nullable: false),
                    EsProfesorPrincipal = table.Column<bool>(type: "bit", nullable: false),
                    AulaAsignada = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Estado = table.Column<bool>(type: "bit", nullable: false),
                    FechaAsignacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaInicio = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FechaFin = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Observaciones = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProfesorGrupo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProfesorGrupo_GruposEstudiantes_GrupoId",
                        column: x => x.GrupoId,
                        principalTable: "GruposEstudiantes",
                        principalColumn: "GrupoId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProfesorGrupo_Materias_MateriaId",
                        column: x => x.MateriaId,
                        principalTable: "Materias",
                        principalColumn: "MateriaId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProfesorGrupo_PeriodosAcademicos_PeriodoAcademicoId",
                        column: x => x.PeriodoAcademicoId,
                        principalTable: "PeriodosAcademicos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProfesorGrupo_Profesores_ProfesorId",
                        column: x => x.ProfesorId,
                        principalTable: "Profesores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProfesorGuia",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProfesorId = table.Column<int>(type: "int", nullable: false),
                    GrupoId = table.Column<int>(type: "int", nullable: false),
                    FechaAsignacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaInicio = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FechaFin = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Estado = table.Column<bool>(type: "bit", nullable: false),
                    Observaciones = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProfesorGuia", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProfesorGuia_GruposEstudiantes_GrupoId",
                        column: x => x.GrupoId,
                        principalTable: "GruposEstudiantes",
                        principalColumn: "GrupoId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProfesorGuia_Profesores_ProfesorId",
                        column: x => x.ProfesorId,
                        principalTable: "Profesores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DecisionesProfesionalesConducta",
                columns: table => new
                {
                    IdDecision = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdNotaConducta = table.Column<int>(type: "int", nullable: false),
                    IdEstudiante = table.Column<int>(type: "int", nullable: false),
                    IdPeriodo = table.Column<int>(type: "int", nullable: false),
                    JustificacionPedagogica = table.Column<string>(type: "nvarchar(3000)", maxLength: 3000, nullable: false),
                    ConsideracionesAdicionales = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    DecisionTomada = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NotaAjustada = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    TomaDecisionPorId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    FechaDecision = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NumeroActa = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    FechaActa = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MiembrosComitePresentes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ObservacionesComite = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    RegistradoEnExpediente = table.Column<bool>(type: "bit", nullable: false),
                    FechaRegistroExpediente = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DecisionesProfesionalesConducta", x => x.IdDecision);
                    table.ForeignKey(
                        name: "FK_DecisionesProfesionalesConducta_AspNetUsers_TomaDecisionPorId",
                        column: x => x.TomaDecisionPorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DecisionesProfesionalesConducta_Estudiantes_IdEstudiante",
                        column: x => x.IdEstudiante,
                        principalTable: "Estudiantes",
                        principalColumn: "IdEstudiante",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DecisionesProfesionalesConducta_PeriodosAcademicos_IdPeriodo",
                        column: x => x.IdPeriodo,
                        principalTable: "PeriodosAcademicos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NotasConducta",
                columns: table => new
                {
                    IdNotaConducta = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdEstudiante = table.Column<int>(type: "int", nullable: false),
                    IdPeriodo = table.Column<int>(type: "int", nullable: false),
                    NotaInicial = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalRebajos = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NotaFinal = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    RequiereProgramaAcciones = table.Column<bool>(type: "bit", nullable: false),
                    IdProgramaAcciones = table.Column<int>(type: "int", nullable: true),
                    DecisionProfesionalAplicada = table.Column<bool>(type: "bit", nullable: false),
                    IdDecisionProfesional = table.Column<int>(type: "int", nullable: true),
                    FechaCalculo = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaUltimaActualizacion = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotasConducta", x => x.IdNotaConducta);
                    table.ForeignKey(
                        name: "FK_NotasConducta_DecisionesProfesionalesConducta_IdDecisionProfesional",
                        column: x => x.IdDecisionProfesional,
                        principalTable: "DecisionesProfesionalesConducta",
                        principalColumn: "IdDecision");
                    table.ForeignKey(
                        name: "FK_NotasConducta_Estudiantes_IdEstudiante",
                        column: x => x.IdEstudiante,
                        principalTable: "Estudiantes",
                        principalColumn: "IdEstudiante",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NotasConducta_PeriodosAcademicos_IdPeriodo",
                        column: x => x.IdPeriodo,
                        principalTable: "PeriodosAcademicos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProgramasAccionesInstitucional",
                columns: table => new
                {
                    IdPrograma = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdNotaConducta = table.Column<int>(type: "int", nullable: false),
                    IdEstudiante = table.Column<int>(type: "int", nullable: false),
                    IdPeriodo = table.Column<int>(type: "int", nullable: false),
                    TituloPrograma = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    ObjetivosEspecificos = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    ActividadesARealizar = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    FechaInicio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaFinPrevista = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaFinReal = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ResponsableSupervisionId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    ObservacionesSupervision = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Estado = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ResultadoFinal = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    FechaVerificacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    VerificadoPorId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    ConclusionesComite = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    AprobarConducta = table.Column<bool>(type: "bit", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProgramasAccionesInstitucional", x => x.IdPrograma);
                    table.ForeignKey(
                        name: "FK_ProgramasAccionesInstitucional_AspNetUsers_ResponsableSupervisionId",
                        column: x => x.ResponsableSupervisionId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProgramasAccionesInstitucional_AspNetUsers_VerificadoPorId",
                        column: x => x.VerificadoPorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ProgramasAccionesInstitucional_Estudiantes_IdEstudiante",
                        column: x => x.IdEstudiante,
                        principalTable: "Estudiantes",
                        principalColumn: "IdEstudiante",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProgramasAccionesInstitucional_NotasConducta_IdNotaConducta",
                        column: x => x.IdNotaConducta,
                        principalTable: "NotasConducta",
                        principalColumn: "IdNotaConducta",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProgramasAccionesInstitucional_PeriodosAcademicos_IdPeriodo",
                        column: x => x.IdPeriodo,
                        principalTable: "PeriodosAcademicos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Materias_EscuelaId",
                table: "Materias",
                column: "EscuelaId");

            migrationBuilder.CreateIndex(
                name: "IX_BoletasConducta_AnuladaPorId",
                table: "BoletasConducta",
                column: "AnuladaPorId");

            migrationBuilder.CreateIndex(
                name: "IX_BoletasConducta_DocenteEmisorId",
                table: "BoletasConducta",
                column: "DocenteEmisorId");

            migrationBuilder.CreateIndex(
                name: "IX_BoletasConducta_IdEstudiante",
                table: "BoletasConducta",
                column: "IdEstudiante");

            migrationBuilder.CreateIndex(
                name: "IX_BoletasConducta_IdPeriodo",
                table: "BoletasConducta",
                column: "IdPeriodo");

            migrationBuilder.CreateIndex(
                name: "IX_BoletasConducta_IdTipoFalta",
                table: "BoletasConducta",
                column: "IdTipoFalta");

            migrationBuilder.CreateIndex(
                name: "IX_BoletasConducta_ProfesorGuiaId",
                table: "BoletasConducta",
                column: "ProfesorGuiaId");

            migrationBuilder.CreateIndex(
                name: "IX_Cantones_ProvinciaId",
                table: "Cantones",
                column: "ProvinciaId");

            migrationBuilder.CreateIndex(
                name: "IX_DecisionesProfesionalesConducta_IdEstudiante",
                table: "DecisionesProfesionalesConducta",
                column: "IdEstudiante");

            migrationBuilder.CreateIndex(
                name: "IX_DecisionesProfesionalesConducta_IdNotaConducta",
                table: "DecisionesProfesionalesConducta",
                column: "IdNotaConducta");

            migrationBuilder.CreateIndex(
                name: "IX_DecisionesProfesionalesConducta_IdPeriodo",
                table: "DecisionesProfesionalesConducta",
                column: "IdPeriodo");

            migrationBuilder.CreateIndex(
                name: "IX_DecisionesProfesionalesConducta_TomaDecisionPorId",
                table: "DecisionesProfesionalesConducta",
                column: "TomaDecisionPorId");

            migrationBuilder.CreateIndex(
                name: "IX_Distritos_CantonId",
                table: "Distritos",
                column: "CantonId");

            migrationBuilder.CreateIndex(
                name: "IX_Escuelas_FacultadId",
                table: "Escuelas",
                column: "FacultadId");

            migrationBuilder.CreateIndex(
                name: "IX_Facultades_InstitucionId",
                table: "Facultades",
                column: "InstitucionId");

            migrationBuilder.CreateIndex(
                name: "IX_Instituciones_DistritoId",
                table: "Instituciones",
                column: "DistritoId");

            migrationBuilder.CreateIndex(
                name: "IX_NotasConducta_IdDecisionProfesional",
                table: "NotasConducta",
                column: "IdDecisionProfesional");

            migrationBuilder.CreateIndex(
                name: "IX_NotasConducta_IdEstudiante",
                table: "NotasConducta",
                column: "IdEstudiante");

            migrationBuilder.CreateIndex(
                name: "IX_NotasConducta_IdPeriodo",
                table: "NotasConducta",
                column: "IdPeriodo");

            migrationBuilder.CreateIndex(
                name: "IX_NotasConducta_IdProgramaAcciones",
                table: "NotasConducta",
                column: "IdProgramaAcciones");

            migrationBuilder.CreateIndex(
                name: "IX_ProfesorCapacitacion_ProfesorId",
                table: "ProfesorCapacitacion",
                column: "ProfesorId");

            migrationBuilder.CreateIndex(
                name: "IX_Profesores_CantonId",
                table: "Profesores",
                column: "CantonId");

            migrationBuilder.CreateIndex(
                name: "IX_Profesores_DistritoId",
                table: "Profesores",
                column: "DistritoId");

            migrationBuilder.CreateIndex(
                name: "IX_Profesores_EscuelaId",
                table: "Profesores",
                column: "EscuelaId");

            migrationBuilder.CreateIndex(
                name: "IX_Profesores_ProvinciaId",
                table: "Profesores",
                column: "ProvinciaId");

            migrationBuilder.CreateIndex(
                name: "IX_ProfesorExperienciaLaboral_ProfesorId",
                table: "ProfesorExperienciaLaboral",
                column: "ProfesorId");

            migrationBuilder.CreateIndex(
                name: "IX_ProfesorFormacionAcademica_ProfesorId",
                table: "ProfesorFormacionAcademica",
                column: "ProfesorId");

            migrationBuilder.CreateIndex(
                name: "IX_ProfesorGrupo_GrupoId",
                table: "ProfesorGrupo",
                column: "GrupoId");

            migrationBuilder.CreateIndex(
                name: "IX_ProfesorGrupo_MateriaId",
                table: "ProfesorGrupo",
                column: "MateriaId");

            migrationBuilder.CreateIndex(
                name: "IX_ProfesorGrupo_PeriodoAcademicoId",
                table: "ProfesorGrupo",
                column: "PeriodoAcademicoId");

            migrationBuilder.CreateIndex(
                name: "IX_ProfesorGrupo_ProfesorId",
                table: "ProfesorGrupo",
                column: "ProfesorId");

            migrationBuilder.CreateIndex(
                name: "IX_ProfesorGuia_GrupoId",
                table: "ProfesorGuia",
                column: "GrupoId");

            migrationBuilder.CreateIndex(
                name: "IX_ProfesorGuia_ProfesorId",
                table: "ProfesorGuia",
                column: "ProfesorId");

            migrationBuilder.CreateIndex(
                name: "IX_ProgramasAccionesInstitucional_IdEstudiante",
                table: "ProgramasAccionesInstitucional",
                column: "IdEstudiante");

            migrationBuilder.CreateIndex(
                name: "IX_ProgramasAccionesInstitucional_IdNotaConducta",
                table: "ProgramasAccionesInstitucional",
                column: "IdNotaConducta");

            migrationBuilder.CreateIndex(
                name: "IX_ProgramasAccionesInstitucional_IdPeriodo",
                table: "ProgramasAccionesInstitucional",
                column: "IdPeriodo");

            migrationBuilder.CreateIndex(
                name: "IX_ProgramasAccionesInstitucional_ResponsableSupervisionId",
                table: "ProgramasAccionesInstitucional",
                column: "ResponsableSupervisionId");

            migrationBuilder.CreateIndex(
                name: "IX_ProgramasAccionesInstitucional_VerificadoPorId",
                table: "ProgramasAccionesInstitucional",
                column: "VerificadoPorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Materias_Escuelas_EscuelaId",
                table: "Materias",
                column: "EscuelaId",
                principalTable: "Escuelas",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DecisionesProfesionalesConducta_NotasConducta_IdNotaConducta",
                table: "DecisionesProfesionalesConducta",
                column: "IdNotaConducta",
                principalTable: "NotasConducta",
                principalColumn: "IdNotaConducta",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_NotasConducta_ProgramasAccionesInstitucional_IdProgramaAcciones",
                table: "NotasConducta",
                column: "IdProgramaAcciones",
                principalTable: "ProgramasAccionesInstitucional",
                principalColumn: "IdPrograma");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Revertir el cambio de columna Estado en ProfesorGuia de bit a nvarchar
            migrationBuilder.AlterColumn<string>(
                name: "Estado",
                table: "ProfesorGuia",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit");

            // Convertir valores booleanos de vuelta a texto
            migrationBuilder.Sql(@"
                UPDATE ProfesorGuia 
                SET Estado = CASE 
                    WHEN Estado = 1 THEN 'Activo'
                    ELSE 'Inactivo'
                END
            ");

            migrationBuilder.DropForeignKey(
                name: "FK_Materias_Escuelas_EscuelaId",
                table: "Materias");

            migrationBuilder.DropIndex(
                name: "IX_Materias_EscuelaId",
                table: "Materias");

            migrationBuilder.DropColumn(
                name: "EscuelaId",
                table: "Materias");
        }
    }
}
