# 📊 FICHA TÉCNICA - RubricasApp
## Sistema Integral de Gestión Educativa

---

## 🔧 ESPECIFICACIONES TÉCNICAS

### Stack Tecnológico

**Backend:**
- Framework: ASP.NET Core 8.0 (LTS)
- Lenguaje: C# 12.0
- Arquitectura: MVC (Model-View-Controller)
- ORM: Entity Framework Core 8.0
- Base de Datos: SQL Server 2019+ / SQLite (dev)

**Frontend:**
- HTML5 / CSS3
- JavaScript (ES6+)
- jQuery 3.7+
- Bootstrap 5.3
- Chart.js para gráficos
- DataTables para tablas dinámicas
- Select2 para selectores mejorados

**Librerías Principales:**
- EPPlus (Generación de Excel)
- iTextSharp / PDFSharp (Generación de PDF)
- AutoMapper (Mapeo de objetos)
- Serilog (Logging estructurado)
- FluentValidation (Validación de modelos)

### Arquitectura del Sistema

```
┌─────────────────────────────────────────┐
│          CAPA DE PRESENTACIÓN           │
│  (Views Razor + JavaScript/jQuery)      │
└─────────────────┬───────────────────────┘
                  │
┌─────────────────▼───────────────────────┐
│        CAPA DE CONTROLADORES            │
│   (MVC Controllers + API Controllers)   │
└─────────────────┬───────────────────────┘
                  │
┌─────────────────▼───────────────────────┐
│         CAPA DE SERVICIOS               │
│  (Business Logic + Domain Services)     │
└─────────────────┬───────────────────────┘
                  │
┌─────────────────▼───────────────────────┐
│      CAPA DE ACCESO A DATOS             │
│   (Entity Framework Core + Repositories)│
└─────────────────┬───────────────────────┘
                  │
┌─────────────────▼───────────────────────┐
│          BASE DE DATOS                  │
│        (SQL Server / SQLite)            │
└─────────────────────────────────────────┘
```

---

## 📦 MÓDULOS FUNCIONALES

### 1. Gestión de Usuarios y Seguridad
- ✅ Sistema de autenticación ASP.NET Core Identity
- ✅ Roles y permisos granulares
- ✅ Autenticación de dos factores (2FA) *opcional*
- ✅ Control de sesiones
- ✅ Auditoría de acciones de usuarios
- ✅ Recuperación de contraseñas por email
- ✅ Políticas de contraseñas configurables

### 2. Gestión de Estudiantes
- ✅ CRUD completo de estudiantes
- ✅ Formulario de empadronamiento multi-paso (6 pasos)
- ✅ Gestión de responsables legales (hasta 6)
- ✅ Información de salud y alergias
- ✅ Ubicación geográfica (provincia, cantón, distrito)
- ✅ Historial académico
- ✅ Fotografía del estudiante
- ✅ Búsqueda y filtros avanzados
- ✅ Exportación a Excel

### 3. Gestión de Profesores
- ✅ CRUD completo de profesores
- ✅ Formulario de registro multi-paso (6 pasos)
- ✅ Formación académica (títulos, universidades)
- ✅ Experiencia laboral previa
- ✅ Capacitaciones recibidas
- ✅ Asignación de materias y grupos
- ✅ Eliminación en cascada con confirmación
- ✅ Expediente digital completo
- ✅ Búsqueda y filtros avanzados
- ✅ Exportación a Excel

### 4. Gestión Académica
- ✅ Configuración de periodos académicos
- ✅ Gestión de niveles educativos
- ✅ Gestión de secciones/grupos
- ✅ Gestión de materias
- ✅ Asignación profesor-materia-grupo
- ✅ Horarios (en desarrollo)
- ✅ Calendario académico

### 5. Instrumentos de Evaluación
- ✅ Creación de rúbricas personalizadas
- ✅ Criterios de evaluación configurables
- ✅ Escalas de calificación múltiples
- ✅ Ponderación automática
- ✅ Importación de rúbricas desde Excel
- ✅ Biblioteca de rúbricas institucionales
- ✅ Clonación de instrumentos
- ✅ Versionado de rúbricas

### 6. Evaluaciones y Calificaciones
- ✅ Aplicación de rúbricas a grupos
- ✅ Registro individual y masivo
- ✅ Cálculo automático de calificaciones
- ✅ Conversión entre escalas (10, 100, porcentaje)
- ✅ Observaciones por estudiante
- ✅ Historial de evaluaciones
- ✅ Estadísticas por grupo/materia
- ✅ Identificación de estudiantes en riesgo

### 7. Cuaderno Calificador
- ✅ Vista tipo Excel interactiva
- ✅ Edición rápida de notas
- ✅ Cálculo automático de promedios
- ✅ Colores según rendimiento
- ✅ Exportación a Excel
- ✅ Impresión formato MEP
- ✅ Alertas de calificaciones bajas
- ✅ Conversión entre escalas

### 8. Control de Asistencia
- ✅ Registro diario de asistencia
- ✅ Estados: Presente/Ausente/Tardía/Justificada
- ✅ Observaciones por registro
- ✅ Reportes de ausentismo
- ✅ Estadísticas por estudiante
- ✅ Alertas de inasistencias reiteradas
- ✅ Justificaciones con documentos adjuntos
- ✅ Exportación a Excel

### 9. Reportes y Documentos
- ✅ Boletines de calificaciones (formato MEP)
- ✅ Certificados de notas
- ✅ Listas de estudiantes
- ✅ Reportes estadísticos
- ✅ Análisis de rendimiento
- ✅ Reportes de asistencia
- ✅ Exportación masiva PDF/Excel
- ✅ Plantillas personalizables

### 10. Sistema de Auditoría
- ✅ Registro de todas las acciones críticas
- ✅ Trazabilidad completa
- ✅ Logs estructurados
- ✅ Reportes de auditoría
- ✅ Búsqueda y filtrado de eventos
- ✅ Exportación de logs

### 11. Configuración y Personalización
- ✅ Logo institucional personalizado
- ✅ Colores corporativos
- ✅ Configuración de periodos
- ✅ Escalas de calificación personalizadas
- ✅ Parámetros del sistema
- ✅ Configuración de emails
- ✅ Respaldos de configuración

---

## 💾 BASE DE DATOS

### Modelo de Datos (Principales Entidades)

**Entidades Principales:**
- Usuario (ApplicationUser)
- Estudiante
- Profesor
- Materia
- Grupo
- PeriodoAcademico
- InstrumentoEvaluacion (Rúbrica)
- Evaluacion
- Calificacion
- Asistencia
- Nivel
- Seccion
- Provincia / Canton / Distrito
- Responsable
- FormacionAcademica
- ExperienciaLaboral
- Capacitacion

**Relaciones:**
- 1:N - Profesor tiene muchas FormacionesAcademicas
- N:M - Profesor-Materia-Grupo (tabla intermedia)
- 1:N - Estudiante tiene muchos Responsables
- 1:N - Grupo tiene muchos Estudiantes
- 1:N - InstrumentoEvaluacion tiene muchas Evaluaciones
- N:1 - Evaluacion pertenece a un Estudiante

### Scripts de Base de Datos
- ✅ Migrations de Entity Framework Core
- ✅ Scripts de seed data (datos iniciales)
- ✅ Scripts de backup/restore
- ✅ Procedimientos almacenados para reportes complejos

---

## 🔒 SEGURIDAD

### Autenticación y Autorización
- ✅ Encriptación de contraseñas (PBKDF2)
- ✅ Tokens de sesión seguros
- ✅ Protección contra CSRF
- ✅ Validación de entrada de datos
- ✅ Sanitización de HTML
- ✅ Rate limiting para login
- ✅ Lockout después de intentos fallidos

### Protección de Datos
- ✅ Encriptación en tránsito (HTTPS/TLS 1.3)
- ✅ Encriptación en reposo (SQL Server TDE)
- ✅ Backup automático encriptado
- ✅ Logs de acceso a datos sensibles
- ✅ Cumplimiento GDPR / LOPD
- ✅ Anonimización de datos para reportes

### Políticas de Seguridad
- ✅ Política de contraseñas fuertes
- ✅ Expiración de sesiones configurable
- ✅ Auditoría de cambios críticos
- ✅ Separación de privilegios
- ✅ Principio de menor privilegio

---

## 📈 RENDIMIENTO Y ESCALABILIDAD

### Optimizaciones
- ✅ Caché de consultas frecuentes (Memory Cache)
- ✅ Lazy loading de relaciones
- ✅ Paginación en listados grandes
- ✅ Índices en columnas de búsqueda
- ✅ Compresión de respuestas HTTP
- ✅ Bundling y minificación de JS/CSS
- ✅ CDN para recursos estáticos

### Capacidad
- **Usuarios concurrentes:** 100-500 (según hardware)
- **Estudiantes por institución:** Ilimitados (probado hasta 5,000)
- **Evaluaciones por periodo:** Ilimitadas
- **Almacenamiento:** Escalable según necesidad
- **Tiempo de respuesta promedio:** < 200ms

### Escalabilidad Horizontal
- ✅ Diseño stateless (sin estado en servidor)
- ✅ Sesiones en base de datos distribuida
- ✅ Compatible con load balancers
- ✅ Preparado para contenedores (Docker)
- ✅ Compatible con Kubernetes

---

## 🖥️ REQUISITOS DEL SISTEMA

### Servidor (On-Premise)

**Mínimo:**
- CPU: 4 cores (2.5 GHz)
- RAM: 8 GB
- Disco: 50 GB SSD
- OS: Windows Server 2016+ / Linux (Ubuntu 20.04+)
- .NET 8.0 Runtime
- SQL Server 2019 Express

**Recomendado:**
- CPU: 8 cores (3.0 GHz)
- RAM: 16 GB
- Disco: 100 GB SSD (RAID 1)
- OS: Windows Server 2022 / Ubuntu 22.04 LTS
- .NET 8.0 Runtime
- SQL Server 2022 Standard

**Producción (>500 usuarios):**
- CPU: 16 cores (3.5 GHz)
- RAM: 32 GB
- Disco: 500 GB SSD NVMe (RAID 10)
- OS: Windows Server 2022 / Ubuntu 22.04 LTS
- .NET 8.0 Runtime
- SQL Server 2022 Enterprise
- Load Balancer

### Cliente (Navegador)

**Navegadores Soportados:**
- ✅ Chrome 90+ (recomendado)
- ✅ Firefox 88+
- ✅ Edge 90+
- ✅ Safari 14+
- ⚠️ Internet Explorer NO soportado

**Requisitos Mínimos:**
- Resolución: 1366x768 o superior
- JavaScript habilitado
- Cookies habilitadas
- Conexión: 2 Mbps mínimo

**Dispositivos:**
- ✅ Desktop / Laptop
- ✅ Tablets (10" o más)
- ⚠️ Móviles (funcional pero no optimizado)

---

## 🌐 OPCIONES DE DESPLIEGUE

### 1. On-Premise (En las instalaciones del cliente)
**Ventajas:**
- Control total de datos
- No depende de internet para funcionar
- Cumplimiento estricto de privacidad

**Requisitos:**
- Servidor físico o virtual
- Administrador de sistemas
- Licencia de Windows Server o Linux
- Licencia de SQL Server

### 2. Cloud Hosting (Cliente gestiona)
**Plataformas Compatibles:**
- ✅ Azure App Service
- ✅ AWS Elastic Beanstalk
- ✅ Google Cloud Platform
- ✅ DigitalOcean Droplets
- ✅ Linode / Vultr

**Ventajas:**
- Escalabilidad automática
- Alta disponibilidad
- Backups automáticos
- Menor costo inicial

### 3. SaaS (Nosotros gestionamos)
**Ventajas:**
- Cero mantenimiento para el cliente
- Actualizaciones automáticas
- Soporte incluido
- Acceso desde cualquier lugar

**Infraestructura:**
- Azure / AWS
- CDN global
- Backups diarios
- 99.9% uptime SLA

---

## 🔄 INTEGRACIONES

### APIs Disponibles
- ✅ REST API para integraciones externas
- ✅ Webhooks para eventos importantes
- ✅ Importación desde Excel/CSV
- ✅ Exportación a múltiples formatos

### Integraciones Futuras (Roadmap)
- 🔜 API del MEP (Ministerio de Educación)
- 🔜 Plataformas LMS (Moodle, Canvas)
- 🔜 Sistemas de pago (SINPE, tarjetas)
- 🔜 Notificaciones SMS/WhatsApp
- 🔜 Single Sign-On (SSO)
- 🔜 Microsoft 365 / Google Workspace

---

## 📦 LICENCIAMIENTO Y DISTRIBUCIÓN

### Código Fuente
- Lenguaje: C# (.NET 8)
- Líneas de código: ~35,000-50,000
- Arquitectura: Modular y extensible
- Documentación: XML comments + README

### Dependencias
- Todas las librerías son de código abierto o con licencias comerciales adquiridas
- No hay dependencias con restricciones de redistribución

### Opciones de Licencia
1. **Uso (Licencia Perpetua):** Cliente puede usar, no modificar
2. **Código Fuente Completo:** Cliente obtiene todo el código sin restricciones
3. **White Label:** Cliente puede rebrandear y revender

---

## 🛠️ MANTENIMIENTO Y SOPORTE

### Niveles de Soporte

**Básico (Incluido 1er año):**
- Email support: Respuesta en 24-48 hrs
- Actualizaciones de seguridad
- Bugs críticos: Fix en 72 hrs

**Estándar ($2,500/año):**
- Email + Teléfono: Respuesta en 4-8 hrs
- Actualizaciones mensuales
- Bugs críticos: Fix en 24 hrs
- Capacitación anual

**Premium ($5,000/año):**
- Soporte 24/7 por email
- Respuesta en menos de 2 hrs
- Bugs críticos: Fix en 4 hrs
- Desarrollos personalizados incluidos (20 hrs/año)
- Capacitación trimestral

### Actualizaciones
- Seguridad: Inmediatas
- Bugs: Cada 2 semanas
- Features: Mensual o trimestral
- Versiones mayores: Anual

---

## 📚 DOCUMENTACIÓN DISPONIBLE

### Para Usuarios
- ✅ Manual de usuario (español)
- ✅ Videos tutoriales
- ✅ Guías rápidas por módulo
- ✅ FAQs

### Para Administradores
- ✅ Guía de instalación
- ✅ Guía de configuración
- ✅ Guía de backup/restore
- ✅ Troubleshooting

### Para Desarrolladores (Código Fuente)
- ✅ Documentación de arquitectura
- ✅ Diagramas de clases
- ✅ API documentation
- ✅ Guía de desarrollo
- ✅ Convenciones de código

---

## 🗺️ ROADMAP 2025-2026

### Q1 2025 (Completado ✅)
- Sistema de rúbricas completo
- Cuaderno calificador
- Control de asistencia
- Gestión de profesores mejorada

### Q2 2025 (En Desarrollo)
- App móvil (iOS/Android)
- Portal para padres
- Notificaciones push
- Chat interno

### Q3 2025 (Planificado)
- Integración MEP
- Gestión de horarios
- Reserva de recursos
- Sistema de tareas/deberes

### Q4 2025 (Futuro)
- Biblioteca digital
- Foro estudiantil
- Gamificación
- Analytics con IA

---

## 📞 INFORMACIÓN TÉCNICA DE CONTACTO

**Soporte Técnico:**
- Email: soporte@rubricasapp.com
- Teléfono: +506 XXXX-XXXX
- Horario: Lunes a Viernes, 8am-5pm (GMT-6)

**Desarrollo y Personalización:**
- Email: desarrollo@rubricasapp.com
- Consultas técnicas: dev@rubricasapp.com

---

**Versión Documento:** 1.0  
**Última Actualización:** Octubre 2025  
**Sistema Versión:** 2.0.0
