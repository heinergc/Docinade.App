import { defineConfig } from 'astro/config';
import starlight from '@astrojs/starlight';

export default defineConfig({
	integrations: [
		starlight({
			title: 'RubricasApp - Documentación',
			description: 'Sistema de gestión de rúbricas educativas para el MEP de Costa Rica',
			defaultLocale: 'es',
			locales: {
				es: {
					label: 'Español',
				},
			},
			logo: {
				src: './src/assets/logo.svg',
			},
			social: {
				github: 'https://github.com/tu-usuario/RubricasApp.Web',
			},
			sidebar: [
				{
					label: '📘 Introducción',
					link: '/introduccion/',
				},
				{
					label: '📖 Primeros Pasos',
					autogenerate: { directory: 'primeros-pasos' },
				},
				{
					label: '👥 Grupos',
					autogenerate: { directory: 'grupos' },
				},
				{
					label: '👨‍🏫 Profesores',
					autogenerate: { directory: 'profesores' },
				},
				{
					label: '📚 Currículos',
					autogenerate: { directory: 'curriculos' },
				},
				{
					label: '🔗 Asignaciones',
					autogenerate: { directory: 'asignaciones' },
				},
				{
					label: '🎓 Estudiantes',
					autogenerate: { directory: 'estudiantes' },
				},
				{
					label: '📋 Empadronamiento',
					autogenerate: { directory: 'empadronamiento' },
				},
				{
					label: '📝 Rúbricas',
					autogenerate: { directory: 'rubricas' },
				},
				{
					label: '⚖️ Conducta',
					autogenerate: { directory: 'conducta' },
				},
				{
					label: '✅ Asistencia',
					autogenerate: { directory: 'asistencia' },
				},
				{
					label: '❓ Preguntas Frecuentes',
					autogenerate: { directory: 'faq' },
				},
			],
			customCss: [
				'./src/styles/custom.css',
			],
		}),
	],
});
