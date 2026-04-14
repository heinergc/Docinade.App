# RubricasApp - Documentación

Sistema de documentación profesional para RubricasApp usando Starlight (Astro).

## 🚀 Características

- Documentación completa del sistema
- Interfaz moderna y responsive
- Búsqueda integrada
- Navegación por módulos
- Soporte para tema oscuro/claro
- Optimizado para SEO

## 📦 Estructura del Proyecto

```
docs-site/
├── src/
│   ├── content/
│   │   └── docs/
│   │       ├── introduccion.md
│   │       ├── primeros-pasos/
│   │       │   ├── registro.md
│   │       │   ├── perfil-usuario.md
│   │       │   └── navegacion.md
│   │       ├── grupos/
│   │       │   └── crear-grupo.md
│   │       ├── estudiantes/
│   │       │   └── agregar-estudiante.md
│   │       ├── rubricas/
│   │       │   └── crear-rubrica.md
│   │       ├── conducta/
│   │       │   └── modulo-conducta.md
│   │       ├── asistencia/
│   │       │   └── registro-asistencia.md
│   │       └── faq/
│   │           └── preguntas-frecuentes.md
│   └── styles/
│       └── custom.css
├── public/
├── astro.config.mjs
├── package.json
└── README.md
```

## 🛠️ Instalación

### Requisitos Previos

- Node.js 18+ 
- npm o pnpm

### Instalar Dependencias

```bash
cd docs-site
npm install
```

## 🏃 Desarrollo Local

Inicia el servidor de desarrollo:

```bash
npm run dev
```

La documentación estará disponible en: `http://localhost:4321`

## 🏗️ Build para Producción

Genera el sitio estático:

```bash
npm run build
```

Los archivos se generarán en `dist/`

### Vista Previa de Producción

```bash
npm run preview
```

## 📤 Despliegue

### Opción 1: Netlify

1. Conecta tu repositorio a Netlify
2. Configura:
   - **Build command**: `npm run build`
   - **Publish directory**: `dist/`
3. Deploy automático en cada push

### Opción 2: Vercel

1. Importa el proyecto en Vercel
2. Configura:
   - **Framework Preset**: Astro
   - **Build Command**: `npm run build`
   - **Output Directory**: `dist/`
3. Deploy automático

### Opción 3: GitHub Pages

1. Habilita GitHub Pages en tu repositorio
2. Configura para desplegar desde rama `gh-pages`
3. Ejecuta:

```bash
npm run build
# Luego sube el contenido de dist/ a la rama gh-pages
```

### Opción 4: Servidor Propio

1. Build el proyecto:
```bash
npm run build
```

2. Copia el contenido de `dist/` a tu servidor web
3. Configura tu servidor (Nginx/Apache) para servir archivos estáticos

#### Ejemplo Nginx

```nginx
server {
    listen 80;
    server_name documentacion.rubricas.edu;
    root /var/www/rubricas-docs;
    index index.html;

    location / {
        try_files $uri $uri/ /index.html;
    }
}
```

## ✏️ Agregar Nuevo Contenido

### Crear Nueva Página

1. Crea un archivo `.md` en `src/content/docs/`
2. Agrega el frontmatter:

```md
---
title: Título de la Página
description: Descripción breve
---

# Contenido aquí
```

3. La página aparecerá automáticamente en el sidebar

### Crear Nueva Sección

1. Crea una carpeta en `src/content/docs/`
2. Agrega archivos `.md` dentro
3. Actualiza `astro.config.mjs` si quieres personalizar el orden:

```js
sidebar: [
  {
    label: '📚 Nueva Sección',
    autogenerate: { directory: 'nueva-seccion' },
  },
]
```

## 🎨 Personalización

### Cambiar Colores

Edita `src/styles/custom.css`:

```css
:root {
  --sl-color-accent: #3b82f6; /* Color primario */
  --sl-color-accent-high: #60a5fa;
}
```

### Modificar Logo

1. Agrega tu logo en `src/assets/logo.svg`
2. Actualiza `astro.config.mjs`:

```js
logo: {
  src: './src/assets/logo.svg',
}
```

### Cambiar Enlaces Sociales

En `astro.config.mjs`:

```js
social: {
  github: 'https://github.com/tu-usuario/RubricasApp.Web',
  twitter: 'https://twitter.com/tu-usuario',
}
```

## 📝 Sintaxis Markdown

### Enlaces Internos

```md
[Ver Primeros Pasos](/primeros-pasos/registro)
```

### Imágenes

```md
![Descripción](./imagen.png)
```

### Alertas

```md
> ⚠️ **Advertencia**: Texto importante
> 
> ℹ️ **Nota**: Información adicional
```

### Tablas

```md
| Columna 1 | Columna 2 |
|-----------|-----------|
| Dato 1    | Dato 2    |
```

### Bloques de Código

```md
\`\`\`javascript
console.log('Ejemplo de código');
\`\`\`
```

## 🔧 Configuración Avanzada

### SEO

En cada página, personaliza el SEO con frontmatter:

```md
---
title: Título de la Página
description: Descripción para motores de búsqueda (max 160 caracteres)
---
```

### Internacionalización (i18n)

Para agregar más idiomas, edita `astro.config.mjs`:

```js
locales: {
  es: { label: 'Español' },
  en: { label: 'English' },
}
```

## 🐛 Troubleshooting

### El servidor no inicia

```bash
# Limpia caché y reinstala
rm -rf node_modules package-lock.json
npm install
```

### Los cambios no se reflejan

Reinicia el servidor de desarrollo (Ctrl+C y `npm run dev`)

### Errores de build

Verifica:
- Todas las páginas tengan frontmatter válido
- No haya enlaces rotos
- Imágenes existan en las rutas correctas

## 📚 Recursos Adicionales

- [Documentación de Starlight](https://starlight.astro.build/)
- [Documentación de Astro](https://docs.astro.build/)
- [Markdown Guide](https://www.markdownguide.org/)

## 📄 Licencia

Este proyecto es parte de RubricasApp.Web y comparte la misma licencia.

---

**Desarrollado para el Ministerio de Educación Pública de Costa Rica**
