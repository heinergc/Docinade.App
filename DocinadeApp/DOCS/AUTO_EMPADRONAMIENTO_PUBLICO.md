# 📋 GUÍA DE IMPLEMENTACIÓN - AUTO-EMPADRONAMIENTO PÚBLICO

## 🎯 ESTADO ACTUAL DE LA IMPLEMENTACIÓN

### ✅ COMPLETADO

1. **Configuración del Sistema**
   - ✅ `appsettings.json` actualizado con sección `EmpadronamientoPublico`
   - ✅ Configuración de habilitación/deshabilitación
   - ✅ Límites de intentos por IP
   - ✅ Configuración de envío de emails

2. **ViewModels**
   - ✅ `EmpadronamientoPublicoViewModel.cs` creado
   - ✅ 5 pasos definidos con validaciones
   - ✅ Campos completos para Estudiante y EstudianteEmpadronamiento

3. **Controlador**
   - ✅ `EmpadronamientoPublicoController.cs` creado
   - ✅ Sin atributo [Authorize] - acceso público
   - ✅ Flujo multi-step completo implementado
   - ✅ Validación de duplicados (cédula y correo)
   - ✅ Persistencia con TempData
   - ✅ Control de límites por IP
   - ✅ Guardado en base de datos

### 🔨 PENDIENTE - VISTAS

Necesitas crear las siguientes vistas en `/Views/EmpadronamientoPublico/`:

#### 1. **Index.cshtml** - Página de Bienvenida
```cshtml
@{
    ViewData["Title"] = "Empadronamiento de Estudiantes";
}

<div class="container my-5">
    <div class="row justify-content-center">
        <div class="col-md-8">
            <div class="card shadow-lg">
                <div class="card-header bg-primary text-white text-center py-4">
                    <h2><i class="fas fa-user-graduate"></i> Empadronamiento Estudiantil</h2>
                    <p class="mb-0">Sistema de Registro Público</p>
                </div>
                <div class="card-body p-5">
                    <div class="text-center mb-4">
                        <img src="/images/logo.png" alt="Logo" class="img-fluid" style="max-width: 200px;">
                    </div>
                    
                    <h4 class="mb-4">Bienvenido al Proceso de Empadronamiento</h4>
                    
                    <p class="lead">
                        Complete el siguiente formulario para iniciar su proceso de empadronamiento. 
                        El proceso consta de 5 pasos sencillos que le tomarán aproximadamente 10 minutos.
                    </p>
                    
                    <div class="alert alert-info">
                        <h5><i class="fas fa-info-circle"></i> Información Requerida:</h5>
                        <ul>
                            <li>Datos personales (nombre, cédula, fecha de nacimiento)</li>
                            <li>Información de contacto y residencia</li>
                            <li>Datos de responsables y contacto de emergencia</li>
                            <li>Historial académico</li>
                            <li>Información de salud (opcional pero recomendada)</li>
                        </ul>
                    </div>
                    
                    <div class="alert alert-warning">
                        <strong><i class="fas fa-exclamation-triangle"></i> Importante:</strong>
                        Asegúrese de tener su cédula de identidad a mano. Los datos ingresados 
                        serán verificados antes de la aprobación final.
                    </div>
                    
                    <div class="d-grid gap-2 mt-4">
                        <a asp-action="Iniciar" class="btn btn-primary btn-lg">
                            <i class="fas fa-play-circle"></i> Iniciar Empadronamiento
                        </a>
                    </div>
                    
                    <p class="text-muted text-center mt-4 small">
                        Al continuar, acepta que sus datos serán procesados de acuerdo con nuestra 
                        <a href="#" data-bs-toggle="modal" data-bs-target="#modalPrivacidad">Política de Privacidad</a>
                    </p>
                </div>
            </div>
        </div>
    </div>
</div>

<!-- Modal de Privacidad -->
<div class="modal fade" id="modalPrivacidad" tabindex="-1">
    <div class="modal-dialog modal-lg">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title">Política de Privacidad</h5>
                <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
            </div>
            <div class="modal-body">
                <p>Sus datos personales serán utilizados únicamente para fines académicos y administrativos...</p>
            </div>
        </div>
    </div>
</div>
```

#### 2. **Deshabilitado.cshtml** - Vista cuando está deshabilitado
```cshtml
@{
    ViewData["Title"] = "Servicio No Disponible";
}

<div class="container my-5">
    <div class="row justify-content-center">
        <div class="col-md-6">
            <div class="card shadow">
                <div class="card-body text-center py-5">
                    <i class="fas fa-exclamation-circle fa-5x text-warning mb-4"></i>
                    <h3>Servicio Temporalmente No Disponible</h3>
                    <p class="lead mt-3">@ViewBag.MensajeDeshabilitado</p>
                    <p class="text-muted">Por favor, intente más tarde o contáctenos para más información.</p>
                    <a href="/" class="btn btn-primary mt-3">
                        <i class="fas fa-home"></i> Volver al Inicio
                    </a>
                </div>
            </div>
        </div>
    </div>
</div>
```

#### 3. **Paso1_DatosBasicos.cshtml**
```cshtml
@model RubricasApp.Web.ViewModels.EmpadronamientoPublicoViewModel

@{
    ViewData["Title"] = "Paso 1: Datos Básicos";
}

<!-- Barra de progreso -->
<div class="container my-4">
    <div class="row">
        <div class="col-12">
            <div class="progress" style="height: 30px;">
                <div class="progress-bar progress-bar-striped progress-bar-animated" 
                     role="progressbar" 
                     style="width: 20%;" 
                     aria-valuenow="20" 
                     aria-valuemin="0" 
                     aria-valuemax="100">
                    Paso @Model.PasoActual de @Model.TotalPasos
                </div>
            </div>
        </div>
    </div>
</div>

<div class="container">
    <div class="row justify-content-center">
        <div class="col-md-10">
            <div class="card shadow-lg">
                <div class="card-header bg-primary text-white">
                    <h3 class="mb-0"><i class="fas fa-user"></i> Paso 1: Datos Básicos</h3>
                </div>
                <div class="card-body p-4">
                    <form asp-action="Paso1" method="post">
                        @Html.AntiForgeryToken()
                        
                        <div asp-validation-summary="All" class="alert alert-danger"></div>
                        
                        <!-- Número de Cédula -->
                        <div class="row">
                            <div class="col-md-12 mb-3">
                                <label asp-for="NumeroId" class="form-label">
                                    <i class="fas fa-id-card"></i> @Html.DisplayNameFor(m => m.NumeroId) 
                                    <span class="text-danger">*</span>
                                </label>
                                <div class="input-group">
                                    <input asp-for="NumeroId" class="form-control" id="numeroIdInput" 
                                           placeholder="Ej: 603130190" maxlength="20" required />
                                    <button type="button" class="btn btn-primary" id="btnBuscarCedula">
                                        <i class="fas fa-search"></i> Buscar
                                    </button>
                                </div>
                                <span asp-validation-for="NumeroId" class="text-danger"></span>
                                <small class="form-text text-muted">
                                    <i class="fas fa-info-circle"></i> Ingrese su número de cédula y presione Buscar para autocompletar datos
                                </small>
                            </div>
                        </div>
                        
                        <!-- Nombre y Apellidos -->
                        <div class="row">
                            <div class="col-md-6 mb-3">
                                <label asp-for="Nombre" class="form-label">
                                    @Html.DisplayNameFor(m => m.Nombre) <span class="text-danger">*</span>
                                </label>
                                <input asp-for="Nombre" class="form-control" id="Nombre" 
                                       placeholder="Ingrese su(s) nombre(s)" required />
                                <span asp-validation-for="Nombre" class="text-danger"></span>
                            </div>
                            <div class="col-md-6 mb-3">
                                <label asp-for="Apellidos" class="form-label">
                                    @Html.DisplayNameFor(m => m.Apellidos) <span class="text-danger">*</span>
                                </label>
                                <input asp-for="Apellidos" class="form-control" id="Apellidos" 
                                       placeholder="Ingrese sus apellidos" required />
                                <span asp-validation-for="Apellidos" class="text-danger"></span>
                            </div>
                        </div>
                        
                        <!-- Fecha de Nacimiento y Género -->
                        <div class="row">
                            <div class="col-md-6 mb-3">
                                <label asp-for="FechaNacimiento" class="form-label">
                                    @Html.DisplayNameFor(m => m.FechaNacimiento) <span class="text-danger">*</span>
                                </label>
                                <input asp-for="FechaNacimiento" class="form-control" type="date" required />
                                <span asp-validation-for="FechaNacimiento" class="text-danger"></span>
                            </div>
                            <div class="col-md-6 mb-3">
                                <label asp-for="Genero" class="form-label">
                                    @Html.DisplayNameFor(m => m.Genero) <span class="text-danger">*</span>
                                </label>
                                <select asp-for="Genero" class="form-select" required>
                                    <option value="">Seleccionar...</option>
                                    <option value="Masculino">Masculino</option>
                                    <option value="Femenino">Femenino</option>
                                    <option value="Otro">Otro</option>
                                </select>
                                <span asp-validation-for="Genero" class="text-danger"></span>
                            </div>
                        </div>
                        
                        <!-- Nacionalidad y Estado Civil -->
                        <div class="row">
                            <div class="col-md-6 mb-3">
                                <label asp-for="Nacionalidad" class="form-label">
                                    @Html.DisplayNameFor(m => m.Nacionalidad)
                                </label>
                                <input asp-for="Nacionalidad" class="form-control" value="Costarricense" />
                                <span asp-validation-for="Nacionalidad" class="text-danger"></span>
                            </div>
                            <div class="col-md-6 mb-3">
                                <label asp-for="EstadoCivil" class="form-label">
                                    @Html.DisplayNameFor(m => m.EstadoCivil)
                                </label>
                                <select asp-for="EstadoCivil" class="form-select">
                                    <option value="">Seleccionar...</option>
                                    <option value="Soltero/a">Soltero/a</option>
                                    <option value="Casado/a">Casado/a</option>
                                    <option value="Unión Libre">Unión Libre</option>
                                    <option value="Divorciado/a">Divorciado/a</option>
                                    <option value="Viudo/a">Viudo/a</option>
                                </select>
                                <span asp-validation-for="EstadoCivil" class="text-danger"></span>
                            </div>
                        </div>
                        
                        <div class="d-grid gap-2 d-md-flex justify-content-md-end mt-4">
                            <button type="submit" class="btn btn-primary btn-lg">
                                Siguiente <i class="fas fa-arrow-right"></i>
                            </button>
                        </div>
                    </form>
                </div>
            </div>
        </div>
    </div>
</div>

@section Scripts {
    @{await Html.RenderPartialAsync("_ValidationScriptsPartial");}
    
    <script>
        $(document).ready(function() {
            $('#btnBuscarCedula').on('click', function() {
                const cedula = $('#numeroIdInput').val().trim();
                
                if (!cedula) {
                    Swal.fire({
                        icon: 'warning',
                        title: 'Atención',
                        text: 'Por favor ingrese una cédula',
                        confirmButtonText: 'Entendido'
                    });
                    return;
                }

                const btnBuscar = $(this);
                const btnOriginalHtml = btnBuscar.html();
                btnBuscar.prop('disabled', true).html('<i class="fas fa-spinner fa-spin"></i> Buscando...');

                $.ajax({
                    url: '/Estudiantes/BuscarPorCedula',
                    type: 'GET',
                    data: { cedula: cedula },
                    success: function(response) {
                        if (response.success && response.data) {
                            $('#Nombre').val(response.data.nombre);
                            $('#Apellidos').val(response.data.apellidos);

                            Swal.fire({
                                icon: 'success',
                                title: '¡Datos encontrados!',
                                html: `<p><strong>Nombre:</strong> ${response.data.nombre}</p>
                                       <p><strong>Apellidos:</strong> ${response.data.apellidos}</p>`,
                                confirmButtonText: 'Continuar',
                                timer: 5000
                            });
                        } else {
                            Swal.fire({
                                icon: 'info',
                                title: 'No encontrado',
                                text: 'Complete manualmente los datos',
                                confirmButtonText: 'Entendido'
                            });
                        }
                    },
                    error: function() {
                        Swal.fire({
                            icon: 'error',
                            title: 'Error',
                            text: 'No se pudo conectar con el servicio de búsqueda',
                            confirmButtonText: 'Entendido'
                        });
                    },
                    complete: function() {
                        btnBuscar.prop('disabled', false).html(btnOriginalHtml);
                    }
                });
            });
        });
    </script>
}
```

### CONTINÚA EN SIGUIENTES ARCHIVOS...

#### 📝 NOTAS IMPORTANTES:

1. **Carpeta de Vistas**: Crear `/Views/EmpadronamientoPublico/`

2. **Vistas Restantes a Crear**:
   - `Paso2_ContactoResidencia.cshtml`
   - `Paso3_Responsables.cshtml`
   - `Paso4_Academico.cshtml`
   - `Paso5_Salud.cshtml`
   - `Confirmacion.cshtml`

3. **Cada vista debe incluir**:
   - Barra de progreso mostrando paso actual
   - Botones "Anterior" y "Siguiente"/"Finalizar"
   - Validación con asp-validation-for
   - @section Scripts para validaciones cliente

4. **Para habilitar el sistema**:
   ```json
   "EmpadronamientoPublico": {
     "Habilitado": true
   }
   ```

5. **Ruta de acceso pública**:
   ```
   http://localhost:18163/EmpadronamientoPublico
   ```

6. **Panel de Admin pendiente**: 
   Crear vista en área Admin para activar/desactivar el sistema

7. **Email de confirmación**: 
   Integrar con servicio IEmailService_Rubricas existente

¿Deseas que cree las vistas restantes o prefieres continuar con otra parte de la implementación?
