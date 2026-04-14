# Guía para Probar el Registro de Usuarios desde el Panel Admin

## 🎯 Objetivo
Verificar que el sistema de creación de usuarios desde `/Admin/Users/Create` funcione correctamente.

## ✅ Estado Actual
- ✅ Servidor ejecutándose en https://localhost:18163
- ✅ Endpoint `/Admin/Users/Create` configurado y respondiendo
- ✅ Redirección a login funcionando correctamente (seguridad)
- ✅ Formulario de creación de usuarios implementado
- ✅ Todos los campos requeridos presentes
- ✅ Proyecto compilando sin errores

## 🚀 Instrucciones de Prueba

### Paso 1: Acceso al Sistema
1. Abre tu navegador y navega a: `https://localhost:18163`
2. Inicia sesión con una cuenta que tenga permisos de administrador
3. Una vez autenticado, navega a: `https://localhost:18163/Admin/Users/Create`

### Paso 2: Verificación del Formulario
El formulario debe mostrar los siguientes campos:
- **Email** (requerido)
- **Contraseña** (requerido)
- **Confirmar Contraseña** (requerido)
- **Nombre** (requerido)
- **Apellido** (requerido)
- **Teléfono** (opcional)
- **Número de Identificación** (opcional)
- **Institución** (opcional)
- **Departamento** (opcional)
- **Usuario Activo** (checkbox)
- **Email Confirmado** (checkbox)
- **Autenticación de Dos Factores** (checkbox)
- **Enviar Email de Bienvenida** (checkbox)

### Paso 3: Prueba Automática con Script de Browser

#### Opción A: Uso del Script Automático
1. Una vez en la página `/Admin/Users/Create`
2. Abre las herramientas de desarrollador (F12)
3. Ve a la pestaña "Consola"
4. Ejecuta el siguiente comando para cargar el script de prueba:
   ```javascript
   fetch('/Tests/browser-admin-test.js').then(r => r.text()).then(eval);
   ```
5. O copia y pega el contenido del archivo `Tests/browser-admin-test.js` directamente en la consola

#### Opción B: Prueba Manual
Llena el formulario manualmente con datos como:
```
Email: test.user@ejemplo.com
Contraseña: TestPassword123!
Nombre: Usuario
Apellido: Prueba
Teléfono: +506 8888-8888
Número de Identificación: 1-2345-6789
Institución: Universidad de Prueba
Departamento: Departamento de Pruebas
☑️ Usuario Activo
☑️ Email Confirmado
```

### Paso 4: Envío y Verificación
1. Haz clic en "Crear Usuario"
2. Verifica que:
   - No aparezcan errores de validación
   - Seas redirigido a la página de detalles del usuario creado
   - Aparezca un mensaje de éxito
   - Los datos del usuario estén guardados correctamente

### Paso 5: Verificación en Base de Datos (Opcional)
Si tienes acceso a la base de datos, verifica que:
- El usuario se haya creado en la tabla `AspNetUsers`
- Los campos personalizados estén poblados (NumeroIdentificacion, Institucion, Departamento)
- El usuario esté marcado como activo

## 🔧 Archivos Relevantes

### Controlador
- `Areas/Admin/Controllers/UsersController.cs` - Lógica de creación de usuarios

### Vista
- `Areas/Admin/Views/Users/Create.cshtml` - Formulario de creación

### ViewModel
- `ViewModels/Admin/UserManagementViewModels.cs` - Modelo de datos

### Scripts de Prueba
- `Tests/check-admin-users.js` - Verificación de conectividad
- `Tests/browser-admin-test.js` - Prueba automática en navegador

## 🐛 Solución de Problemas

### Error 401/403 o Redirección a Login
- **Causa**: Usuario no autenticado o sin permisos de admin
- **Solución**: Asegúrate de estar logueado con una cuenta administrador

### Error 404 en `/Admin/Users/Create`
- **Causa**: Routing no configurado correctamente
- **Solución**: Verifica que el controlador tenga el atributo `[Area("Admin")]`

### Errores de Validación en el Formulario
- **Causa**: Campos requeridos vacíos o formato incorrecto
- **Solución**: Revisa que todos los campos marcados como requeridos estén completados

### Error 500 al Crear Usuario
- **Causa**: Error interno del servidor (posiblemente base de datos)
- **Solución**: Revisa los logs del servidor o la consola de desarrollo

## 📊 Resultados Esperados

### Flujo Exitoso:
1. ✅ Acceso al formulario sin errores
2. ✅ Validación del formulario funcionando
3. ✅ Usuario creado exitosamente
4. ✅ Redirección a página de detalles
5. ✅ Mensaje de éxito mostrado
6. ✅ Datos guardados en base de datos

### Indicadores de Éxito:
- URL cambia a `/Admin/Users/Details/{id}` después de la creación
- Mensaje verde de éxito: "Usuario {email} creado exitosamente"
- Página de detalles muestra todos los datos ingresados
- Usuario puede ser encontrado en la lista de usuarios admin

## 🔄 Próximos Pasos
Una vez verificado que funciona:
1. Probar con diferentes combinaciones de roles y permisos
2. Verificar que el email de bienvenida se envíe (si configurado)
3. Probar edición y eliminación de usuarios creados
4. Verificar auditoría y logs de creación de usuarios
