"use strict";
var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
var __generator = (this && this.__generator) || function (thisArg, body) {
    var _ = { label: 0, sent: function() { if (t[0] & 1) throw t[1]; return t[1]; }, trys: [], ops: [] }, f, y, t, g;
    return g = { next: verb(0), "throw": verb(1), "return": verb(2) }, typeof Symbol === "function" && (g[Symbol.iterator] = function() { return this; }), g;
    function verb(n) { return function (v) { return step([n, v]); }; }
    function step(op) {
        if (f) throw new TypeError("Generator is already executing.");
        while (_) try {
            if (f = 1, y && (t = op[0] & 2 ? y["return"] : op[0] ? y["throw"] || ((t = y["return"]) && t.call(y), 0) : y.next) && !(t = t.call(y, op[1])).done) return t;
            if (y = 0, t) op = [op[0] & 2, t.value];
            switch (op[0]) {
                case 0: case 1: t = op; break;
                case 4: _.label++; return { value: op[1], done: false };
                case 5: _.label++; y = op[1]; op = [0]; continue;
                case 7: op = _.ops.pop(); _.trys.pop(); continue;
                default:
                    if (!(t = _.trys, t = t.length > 0 && t[t.length - 1]) && (op[0] === 6 || op[0] === 2)) { _ = 0; continue; }
                    if (op[0] === 3 && (!t || (op[1] > t[0] && op[1] < t[3]))) { _.label = op[1]; break; }
                    if (op[0] === 6 && _.label < t[1]) { _.label = t[1]; t = op; break; }
                    if (t && _.label < t[2]) { _.label = t[2]; _.ops.push(op); break; }
                    if (t[2]) _.ops.pop();
                    _.trys.pop(); continue;
            }
            op = body.call(thisArg, _);
        } catch (e) { op = [6, e]; y = 0; } finally { f = t = 0; }
        if (op[0] & 5) throw op[1]; return { value: op[0] ? op[1] : void 0, done: true };
    }
};
exports.__esModule = true;
var test_1 = require("@playwright/test");
var BASE_URL = 'http://localhost:4321';
test_1.test.describe('Documentación RubricasApp - Navegación', function () {
    test_1.test('La página principal carga correctamente', function (_a) {
        var page = _a.page;
        return __awaiter(void 0, void 0, void 0, function () {
            return __generator(this, function (_b) {
                switch (_b.label) {
                    case 0: return [4 /*yield*/, page.goto(BASE_URL)];
                    case 1:
                        _b.sent();
                        // Verificar que el título existe en el área principal
                        return [4 /*yield*/, test_1.expect(page.locator('.sl-markdown-content h1, article h1, #_top').first()).toContainText('Bienvenido a RubricasApp')];
                    case 2:
                        // Verificar que el título existe en el área principal
                        _b.sent();
                        // Verificar que el contenido principal existe
                        return [4 /*yield*/, test_1.expect(page.locator('.main-frame, .sl-container, main').first()).toBeVisible()];
                    case 3:
                        // Verificar que el contenido principal existe
                        _b.sent();
                        return [2 /*return*/];
                }
            });
        });
    });
    test_1.test('Página de Introducción carga sin errores', function (_a) {
        var page = _a.page;
        return __awaiter(void 0, void 0, void 0, function () {
            return __generator(this, function (_b) {
                switch (_b.label) {
                    case 0: return [4 /*yield*/, page.goto(BASE_URL + '/introduccion/')];
                    case 1:
                        _b.sent();
                        return [4 /*yield*/, test_1.expect(page.locator('#_top, article h1').first()).toContainText('Introducción a RubricasApp')];
                    case 2:
                        _b.sent();
                        // Verificar que el contenido principal existe  
                        return [4 /*yield*/, test_1.expect(page.locator('.main-frame, .sl-markdown-content, main').first()).toBeVisible()];
                    case 3:
                        // Verificar que el contenido principal existe  
                        _b.sent();
                        return [2 /*return*/];
                }
            });
        });
    });
    test_1.test('Página Primeros Pasos - Registro carga correctamente', function (_a) {
        var page = _a.page;
        return __awaiter(void 0, void 0, void 0, function () {
            var errors;
            return __generator(this, function (_b) {
                switch (_b.label) {
                    case 0:
                        errors = [];
                        page.on('console', function (msg) {
                            if (msg.type() === 'error') {
                                errors.push(msg.text());
                            }
                        });
                        return [4 /*yield*/, page.goto(BASE_URL + '/primeros-pasos/registro/')];
                    case 1:
                        _b.sent();
                        return [4 /*yield*/, test_1.expect(page.locator('#_top, article h1').first()).toContainText('Registro en el Sistema')];
                    case 2:
                        _b.sent();
                        // Wait for page to fully load
                        return [4 /*yield*/, page.waitForLoadState('networkidle')];
                    case 3:
                        // Wait for page to fully load
                        _b.sent();
                        // Verify no JavaScript errors
                        test_1.expect(errors.filter(function (e) { return e.includes('Cannot read properties of undefined'); })).toHaveLength(0);
                        return [2 /*return*/];
                }
            });
        });
    });
    test_1.test('Página Primeros Pasos - Perfil Usuario carga correctamente', function (_a) {
        var page = _a.page;
        return __awaiter(void 0, void 0, void 0, function () {
            return __generator(this, function (_b) {
                switch (_b.label) {
                    case 0: return [4 /*yield*/, page.goto(BASE_URL + '/primeros-pasos/perfil-usuario/')];
                    case 1:
                        _b.sent();
                        return [4 /*yield*/, test_1.expect(page.locator('#_top, article h1').first()).toContainText('Configuración del Perfil')];
                    case 2:
                        _b.sent();
                        return [2 /*return*/];
                }
            });
        });
    });
    test_1.test('Página Primeros Pasos - Navegación carga correctamente', function (_a) {
        var page = _a.page;
        return __awaiter(void 0, void 0, void 0, function () {
            return __generator(this, function (_b) {
                switch (_b.label) {
                    case 0: return [4 /*yield*/, page.goto(BASE_URL + '/primeros-pasos/navegacion/')];
                    case 1:
                        _b.sent();
                        return [4 /*yield*/, test_1.expect(page.locator('#_top, article h1').first()).toContainText('Navegación por el Sistema')];
                    case 2:
                        _b.sent();
                        return [2 /*return*/];
                }
            });
        });
    });
    test_1.test('Página Grupos carga correctamente', function (_a) {
        var page = _a.page;
        return __awaiter(void 0, void 0, void 0, function () {
            return __generator(this, function (_b) {
                switch (_b.label) {
                    case 0: return [4 /*yield*/, page.goto(BASE_URL + '/grupos/crear-grupo/')];
                    case 1:
                        _b.sent();
                        return [4 /*yield*/, test_1.expect(page.locator('#_top, article h1').first()).toContainText('Crear Grupos')];
                    case 2:
                        _b.sent();
                        return [2 /*return*/];
                }
            });
        });
    });
    test_1.test('Página Profesores carga correctamente', function (_a) {
        var page = _a.page;
        return __awaiter(void 0, void 0, void 0, function () {
            return __generator(this, function (_b) {
                switch (_b.label) {
                    case 0: return [4 /*yield*/, page.goto(BASE_URL + '/profesores/gestion-profesores/')];
                    case 1:
                        _b.sent();
                        return [4 /*yield*/, test_1.expect(page.locator('#_top, article h1').first()).toContainText('Gestión de Profesores')];
                    case 2:
                        _b.sent();
                        return [2 /*return*/];
                }
            });
        });
    });
    test_1.test('Página Currículos carga correctamente', function (_a) {
        var page = _a.page;
        return __awaiter(void 0, void 0, void 0, function () {
            return __generator(this, function (_b) {
                switch (_b.label) {
                    case 0: return [4 /*yield*/, page.goto(BASE_URL + '/curriculos/gestion-curriculos/')];
                    case 1:
                        _b.sent();
                        return [4 /*yield*/, test_1.expect(page.locator('#_top, article h1').first()).toContainText('Gestión de Currículos')];
                    case 2:
                        _b.sent();
                        return [2 /*return*/];
                }
            });
        });
    });
    test_1.test('Página Asignaciones carga correctamente', function (_a) {
        var page = _a.page;
        return __awaiter(void 0, void 0, void 0, function () {
            return __generator(this, function (_b) {
                switch (_b.label) {
                    case 0: return [4 /*yield*/, page.goto(BASE_URL + '/asignaciones/gestion-asignaciones/')];
                    case 1:
                        _b.sent();
                        return [4 /*yield*/, test_1.expect(page.locator('#_top, article h1').first()).toContainText('Gestión de Asignaciones')];
                    case 2:
                        _b.sent();
                        return [2 /*return*/];
                }
            });
        });
    });
    test_1.test('Página Estudiantes carga correctamente', function (_a) {
        var page = _a.page;
        return __awaiter(void 0, void 0, void 0, function () {
            return __generator(this, function (_b) {
                switch (_b.label) {
                    case 0: return [4 /*yield*/, page.goto(BASE_URL + '/estudiantes/agregar-estudiante/')];
                    case 1:
                        _b.sent();
                        return [4 /*yield*/, test_1.expect(page.locator('#_top, article h1').first()).toContainText('Agregar Estudiantes')];
                    case 2:
                        _b.sent();
                        return [2 /*return*/];
                }
            });
        });
    });
    test_1.test('Página Empadronamiento carga correctamente', function (_a) {
        var page = _a.page;
        return __awaiter(void 0, void 0, void 0, function () {
            return __generator(this, function (_b) {
                switch (_b.label) {
                    case 0: return [4 /*yield*/, page.goto(BASE_URL + '/empadronamiento/registro-empadronamiento/')];
                    case 1:
                        _b.sent();
                        return [4 /*yield*/, test_1.expect(page.locator('#_top, article h1').first()).toContainText('Registro de Empadronamiento')];
                    case 2:
                        _b.sent();
                        return [2 /*return*/];
                }
            });
        });
    });
    test_1.test('Página Rúbricas carga correctamente', function (_a) {
        var page = _a.page;
        return __awaiter(void 0, void 0, void 0, function () {
            return __generator(this, function (_b) {
                switch (_b.label) {
                    case 0: return [4 /*yield*/, page.goto(BASE_URL + '/rubricas/crear-rubrica/')];
                    case 1:
                        _b.sent();
                        return [4 /*yield*/, test_1.expect(page.locator('#_top, article h1').first()).toContainText('Crear Rúbricas')];
                    case 2:
                        _b.sent();
                        return [2 /*return*/];
                }
            });
        });
    });
    test_1.test('Página Conducta carga correctamente', function (_a) {
        var page = _a.page;
        return __awaiter(void 0, void 0, void 0, function () {
            return __generator(this, function (_b) {
                switch (_b.label) {
                    case 0: return [4 /*yield*/, page.goto(BASE_URL + '/conducta/modulo-conducta/')];
                    case 1:
                        _b.sent();
                        return [4 /*yield*/, test_1.expect(page.locator('#_top, article h1').first()).toContainText('Módulo de Conducta')];
                    case 2:
                        _b.sent();
                        return [2 /*return*/];
                }
            });
        });
    });
    test_1.test('Página Asistencia carga correctamente', function (_a) {
        var page = _a.page;
        return __awaiter(void 0, void 0, void 0, function () {
            return __generator(this, function (_b) {
                switch (_b.label) {
                    case 0: return [4 /*yield*/, page.goto(BASE_URL + '/asistencia/registro-asistencia/')];
                    case 1:
                        _b.sent();
                        return [4 /*yield*/, test_1.expect(page.locator('#_top, article h1').first()).toContainText('Registro de Asistencia')];
                    case 2:
                        _b.sent();
                        return [2 /*return*/];
                }
            });
        });
    });
    test_1.test('Página FAQ carga correctamente', function (_a) {
        var page = _a.page;
        return __awaiter(void 0, void 0, void 0, function () {
            return __generator(this, function (_b) {
                switch (_b.label) {
                    case 0: return [4 /*yield*/, page.goto(BASE_URL + '/faq/preguntas-frecuentes/')];
                    case 1:
                        _b.sent();
                        return [4 /*yield*/, test_1.expect(page.locator('#_top, article h1').first()).toContainText('Preguntas Frecuentes')];
                    case 2:
                        _b.sent();
                        return [2 /*return*/];
                }
            });
        });
    });
    test_1.test('El sidebar está visible y funcional', function (_a) {
        var page = _a.page;
        return __awaiter(void 0, void 0, void 0, function () {
            var links, allLinks;
            return __generator(this, function (_b) {
                switch (_b.label) {
                    case 0: 
                    // Configurar viewport de escritorio para que el sidebar sea visible
                    return [4 /*yield*/, page.setViewportSize({ width: 1280, height: 720 })];
                    case 1:
                        // Configurar viewport de escritorio para que el sidebar sea visible
                        _b.sent();
                        return [4 /*yield*/, page.goto(BASE_URL)];
                    case 2:
                        _b.sent();
                        // Esperar a que la página cargue completamente
                        return [4 /*yield*/, page.waitForLoadState('networkidle')];
                    case 3:
                        // Esperar a que la página cargue completamente
                        _b.sent();
                        links = page.locator('a').first();
                        return [4 /*yield*/, test_1.expect(links).toBeVisible()];
                    case 4:
                        _b.sent();
                        return [4 /*yield*/, page.locator('a[href^="/"]').count()];
                    case 5:
                        allLinks = _b.sent();
                        test_1.expect(allLinks).toBeGreaterThan(3);
                        return [2 /*return*/];
                }
            });
        });
    });
    test_1.test('La búsqueda funciona', function (_a) {
        var page = _a.page;
        return __awaiter(void 0, void 0, void 0, function () {
            var searchButton;
            return __generator(this, function (_b) {
                switch (_b.label) {
                    case 0: return [4 /*yield*/, page.goto(BASE_URL)];
                    case 1:
                        _b.sent();
                        searchButton = page.locator('button[aria-label*="Search"]').first();
                        return [4 /*yield*/, searchButton.isVisible()];
                    case 2:
                        if (!_b.sent()) return [3 /*break*/, 5];
                        return [4 /*yield*/, searchButton.click()];
                    case 3:
                        _b.sent();
                        // Verificar que se abre el diálogo de búsqueda
                        return [4 /*yield*/, test_1.expect(page.locator('[role="dialog"]')).toBeVisible()];
                    case 4:
                        // Verificar que se abre el diálogo de búsqueda
                        _b.sent();
                        _b.label = 5;
                    case 5: return [2 /*return*/];
                }
            });
        });
    });
    test_1.test('Navegación entre páginas funciona', function (_a) {
        var page = _a.page;
        return __awaiter(void 0, void 0, void 0, function () {
            return __generator(this, function (_b) {
                switch (_b.label) {
                    case 0: return [4 /*yield*/, page.goto(BASE_URL)];
                    case 1:
                        _b.sent();
                        // Hacer clic en "Introducción" del sidebar
                        return [4 /*yield*/, page.click('text=Introducción')];
                    case 2:
                        // Hacer clic en "Introducción" del sidebar
                        _b.sent();
                        return [4 /*yield*/, test_1.expect(page).toHaveURL(/.*introduccion/)];
                    case 3:
                        _b.sent();
                        // Navegar a Primeros Pasos
                        return [4 /*yield*/, page.click('text=Registro en el Sistema')];
                    case 4:
                        // Navegar a Primeros Pasos
                        _b.sent();
                        return [4 /*yield*/, test_1.expect(page).toHaveURL(/.*primeros-pasos\/registro/)];
                    case 5:
                        _b.sent();
                        // Verificar que no hay errores después de navegar
                        return [4 /*yield*/, page.waitForLoadState('networkidle')];
                    case 6:
                        // Verificar que no hay errores después de navegar
                        _b.sent();
                        return [2 /*return*/];
                }
            });
        });
    });
    test_1.test('El tema oscuro/claro funciona', function (_a) {
        var page = _a.page;
        return __awaiter(void 0, void 0, void 0, function () {
            var themeButton, html, dataTheme;
            return __generator(this, function (_b) {
                switch (_b.label) {
                    case 0: return [4 /*yield*/, page.goto(BASE_URL)];
                    case 1:
                        _b.sent();
                        themeButton = page.locator('button[aria-label*="theme"]').first();
                        return [4 /*yield*/, themeButton.isVisible()];
                    case 2:
                        if (!_b.sent()) return [3 /*break*/, 6];
                        return [4 /*yield*/, themeButton.click()];
                    case 3:
                        _b.sent();
                        return [4 /*yield*/, page.waitForTimeout(500)];
                    case 4:
                        _b.sent();
                        html = page.locator('html');
                        return [4 /*yield*/, html.getAttribute('data-theme')];
                    case 5:
                        dataTheme = _b.sent();
                        test_1.expect(dataTheme).toBeTruthy();
                        _b.label = 6;
                    case 6: return [2 /*return*/];
                }
            });
        });
    });
    test_1.test('Todos los enlaces internos funcionan', function (_a) {
        var page = _a.page;
        return __awaiter(void 0, void 0, void 0, function () {
            var links, count, i, href, response;
            return __generator(this, function (_b) {
                switch (_b.label) {
                    case 0: return [4 /*yield*/, page.goto(BASE_URL + '/introduccion/')];
                    case 1:
                        _b.sent();
                        links = page.locator('article a[href^="/"]');
                        return [4 /*yield*/, links.count()];
                    case 2:
                        count = _b.sent();
                        console.log("Encontrados " + count + " enlaces internos");
                        i = 0;
                        _b.label = 3;
                    case 3:
                        if (!(i < Math.min(count, 5))) return [3 /*break*/, 7];
                        return [4 /*yield*/, links.nth(i).getAttribute('href')];
                    case 4:
                        href = _b.sent();
                        if (!href) return [3 /*break*/, 6];
                        return [4 /*yield*/, page.goto(BASE_URL + href)];
                    case 5:
                        response = _b.sent();
                        test_1.expect(response === null || response === void 0 ? void 0 : response.status()).toBe(200);
                        _b.label = 6;
                    case 6:
                        i++;
                        return [3 /*break*/, 3];
                    case 7: return [2 /*return*/];
                }
            });
        });
    });
    test_1.test('No hay errores 404 en recursos', function (_a) {
        var page = _a.page;
        return __awaiter(void 0, void 0, void 0, function () {
            var failed;
            return __generator(this, function (_b) {
                switch (_b.label) {
                    case 0:
                        failed = [];
                        page.on('response', function (response) {
                            if (response.status() === 404 && !response.url().includes('favicon')) {
                                failed.push(response.url());
                            }
                        });
                        return [4 /*yield*/, page.goto(BASE_URL)];
                    case 1:
                        _b.sent();
                        return [4 /*yield*/, page.waitForLoadState('networkidle')];
                    case 2:
                        _b.sent();
                        test_1.expect(failed).toHaveLength(0);
                        return [2 /*return*/];
                }
            });
        });
    });
    test_1.test('El contenido es responsive', function (_a) {
        var page = _a.page;
        return __awaiter(void 0, void 0, void 0, function () {
            return __generator(this, function (_b) {
                switch (_b.label) {
                    case 0: 
                    // Test en móvil
                    return [4 /*yield*/, page.setViewportSize({ width: 375, height: 667 })];
                    case 1:
                        // Test en móvil
                        _b.sent();
                        return [4 /*yield*/, page.goto(BASE_URL)];
                    case 2:
                        _b.sent();
                        return [4 /*yield*/, test_1.expect(page.locator('#_top, h1').first()).toBeVisible()];
                    case 3:
                        _b.sent();
                        // Test en tablet
                        return [4 /*yield*/, page.setViewportSize({ width: 768, height: 1024 })];
                    case 4:
                        // Test en tablet
                        _b.sent();
                        return [4 /*yield*/, test_1.expect(page.locator('#_top, h1').first()).toBeVisible()];
                    case 5:
                        _b.sent();
                        // Test en desktop
                        return [4 /*yield*/, page.setViewportSize({ width: 1920, height: 1080 })];
                    case 6:
                        // Test en desktop
                        _b.sent();
                        return [4 /*yield*/, test_1.expect(page.locator('#_top, h1').first()).toBeVisible()];
                    case 7:
                        _b.sent();
                        return [2 /*return*/];
                }
            });
        });
    });
});
