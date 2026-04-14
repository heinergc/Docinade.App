using System.ComponentModel.DataAnnotations;
using RubricasApp.Web.DTOs;

namespace RubricasApp.Web.ViewModels
{
    public class EstudianteEmpadronamientoViewModel
    {
        // Información básica del estudiante (solo lectura)
        public int IdEstudiante { get; set; }
        
        [Display(Name = "Nombre Completo")]
        public string? NombreCompletoEstudiante { get; set; }
        
        [Display(Name = "Correo del Estudiante")]
        public string? CorreoEstudiante { get; set; }
        
        [Display(Name = "Institución")]
        public string? InstitucionEstudiante { get; set; }
        
        // Datos de empadronamiento
        public EstudianteEmpadronamientoDto? DatosEmpadronamiento { get; set; }
        
        // Propiedades para navegación entre secciones del formulario
        public int SeccionActual { get; set; } = 1;
        public int TotalSecciones { get; set; } = 5;
        
        // Validación por secciones
        public bool Seccion1Valida => ValidarSeccionPersonal();
        public bool Seccion2Valida => ValidarSeccionContacto();
        public bool Seccion3Valida => ValidarSeccionResponsables();
        public bool Seccion4Valida => ValidarSeccionSalud();
        public bool Seccion5Valida => ValidarSeccionAcademico();
        
        // Etapas del proceso de empadronamiento
        public List<EtapaEmpadronamiento> EtapasDisponibles { get; set; } = new List<EtapaEmpadronamiento>
        {
            new EtapaEmpadronamiento { Id = "PreRegistro", Nombre = "Pre-registro", Orden = 1 },
            new EtapaEmpadronamiento { Id = "RevisionDocumental", Nombre = "Revisión Documental", Orden = 2 },
            new EtapaEmpadronamiento { Id = "Aprobacion", Nombre = "Aprobación", Orden = 3 },
            new EtapaEmpadronamiento { Id = "MatriculaFinalizada", Nombre = "Matrícula Finalizada", Orden = 4 }
        };
        
        // Lista de documentos comunes
        public List<DocumentoRequerido> DocumentosRequeridos { get; set; } = new List<DocumentoRequerido>
        {
            new DocumentoRequerido { Id = "Cedula", Nombre = "Cédula de Identidad", EsObligatorio = true },
            new DocumentoRequerido { Id = "PartidaNacimiento", Nombre = "Partida de Nacimiento", EsObligatorio = true },
            new DocumentoRequerido { Id = "CertificadoNotas", Nombre = "Certificado de Notas", EsObligatorio = true },
            new DocumentoRequerido { Id = "FotoCedula", Nombre = "Foto de Cédula", EsObligatorio = false },
            new DocumentoRequerido { Id = "ComprobantesDomicilio", Nombre = "Comprobantes de Domicilio", EsObligatorio = false },
            new DocumentoRequerido { Id = "ConstanciaMedica", Nombre = "Constancia Médica", EsObligatorio = false }
        };
        
        // Constructor
        public EstudianteEmpadronamientoViewModel()
        {
            DatosEmpadronamiento = new EstudianteEmpadronamientoDto();
        }
        
        // Métodos de validación por sección
        private bool ValidarSeccionPersonal()
        {
            if (DatosEmpadronamiento == null) return false;
            
            return !string.IsNullOrEmpty(DatosEmpadronamiento.NumeroId) &&
                   DatosEmpadronamiento.FechaNacimiento.HasValue;
        }
        
        private bool ValidarSeccionContacto()
        {
            if (DatosEmpadronamiento == null) return true; // Sección opcional
            
            // Validar formato de correo si se proporciona
            if (!string.IsNullOrEmpty(DatosEmpadronamiento.CorreoAlterno))
            {
                return new EmailAddressAttribute().IsValid(DatosEmpadronamiento.CorreoAlterno);
            }
            return true;
        }
        
        private bool ValidarSeccionResponsables()
        {
            if (DatosEmpadronamiento == null) return true; // Sección opcional
            
            // Al menos un responsable debe estar definido
            return !string.IsNullOrEmpty(DatosEmpadronamiento.NombrePadre) ||
                   !string.IsNullOrEmpty(DatosEmpadronamiento.NombreMadre) ||
                   !string.IsNullOrEmpty(DatosEmpadronamiento.NombreTutor) ||
                   !string.IsNullOrEmpty(DatosEmpadronamiento.ContactoEmergencia);
        }
        
        private bool ValidarSeccionSalud()
        {
            // Sección completamente opcional
            return true;
        }
        
        private bool ValidarSeccionAcademico()
        {
            // Sección completamente opcional
            return true;
        }
        
        // Método para obtener el porcentaje de completitud
        public decimal PorcentajeCompletitud()
        {
            int seccionesValidas = 0;
            if (Seccion1Valida) seccionesValidas++;
            if (Seccion2Valida) seccionesValidas++;
            if (Seccion3Valida) seccionesValidas++;
            if (Seccion4Valida) seccionesValidas++;
            if (Seccion5Valida) seccionesValidas++;
            
            return (decimal)seccionesValidas / TotalSecciones * 100;
        }
    }
    
    public class EtapaEmpadronamiento
    {
        public string Id { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public int Orden { get; set; }
        public bool EsActual { get; set; }
        public bool EsCompletada { get; set; }
        public DateTime? FechaCompletado { get; set; }
        public string? UsuarioResponsable { get; set; }
    }
    
    public class DocumentoRequerido
    {
        public string Id { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public bool EsObligatorio { get; set; }
        public bool EsRecibido { get; set; }
        public DateTime? FechaRecepcion { get; set; }
        public string? Observaciones { get; set; }
    }
}