# 🚀 Guía Rápida de Inicio

## Instalación y Ejecución (5 minutos)

### 1. Navegar a la carpeta del proyecto

```powershell
cd d:\Fuentes_gitHub\RubricasApp.Web\docs-site
```

### 2. Instalar dependencias

```powershell
npm install
```

**Tiempo estimado**: 2-3 minutos

### 3. Iniciar servidor de desarrollo

```powershell
npm run dev
```

### 4. Abrir en el navegador

Abre: **http://localhost:4321**

¡Listo! Tu documentación está corriendo localmente.

---

## Para Producción

### Build

```powershell
npm run build
```

Los archivos quedarán en `dist/`

### Vista Previa

```powershell
npm run preview
```

---

## Despliegue Rápido Options

### ☁️ Netlify (Recomendado)

1. Sube el código a GitHub
2. Conecta Netlify a tu repo
3. Configuración automática detectada
4. Deploy en 2 minutos

**URL ejemplo**: https://rubricas-docs.netlify.app

### ☁️ Vercel

Similar a Netlify, deploy automático.

### 🌐 GitHub Pages

```powershell
npm run build
# Sube dist/ a rama gh-pages
```

**URL**: https://tu-usuario.github.io/RubricasApp.Web

---

## Estructura Actual

```
docs-site/
├── 📘 Introducción
├── 📖 Primeros Pasos (3 páginas)
├── 👥 Grupos
├── 🎓 Estudiantes  
├── 📝 Rúbricas
├── ⚖️ Conducta
├── ✅ Asistencia
└── ❓ FAQ
```

**Total**: 10+ páginas de documentación profesional

---

## Próximos Pasos

1. **Personalizar el logo**: Reemplaza `src/assets/logo.svg`
2. **Actualizar GitHub URL**: En `astro.config.mjs`
3. **Agregar screenshots**: Crea carpeta `public/images/`
4. **Expandir contenido**: Agrega más páginas en `src/content/docs/`

---

## Ayuda Rápida

- **Agregar página**: Crea archivo `.md` en `src/content/docs/`
- **Cambiar colores**: Edita `src/styles/custom.css`
- **Ver errores**: Revisa la consola del servidor

**Documentación completa**: Ver [README.md](README.md)
