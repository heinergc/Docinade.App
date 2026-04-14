-- =====================================================================
-- SCRIPT PARA INSERTAR MATERIAS DEL SISTEMA EDUCATIVO COSTARRICENSE (MEP)
-- Basado en la estructura curricular oficial del Ministerio de Educación Pública
-- =====================================================================

-- Limpiar materias existentes si es necesario (comentar si no se desea)
-- DELETE FROM Materias WHERE Codigo LIKE 'MEP%';

-- =====================================================================
-- MATERIAS BÁSICAS - III CICLO (7°, 8°, 9°)
-- =====================================================================

-- ESPAÑOL
INSERT INTO Materias (Codigo, Nombre, Descripcion, Creditos, Activa, Tipo, CicloSugerido, Estado, FechaCreacion) VALUES
('MEP-ESP-7', 'Español 7°', 'Comunicación oral y escrita para séptimo año', 6, 1, 'Básica', 7, 'ACTIVO', datetime('now')),
('MEP-ESP-8', 'Español 8°', 'Comunicación oral y escrita para octavo año', 6, 1, 'Básica', 8, 'ACTIVO', datetime('now')),
('MEP-ESP-9', 'Español 9°', 'Comunicación oral y escrita para noveno año', 6, 1, 'Básica', 9, 'ACTIVO', datetime('now'));

-- MATEMÁTICAS
INSERT INTO Materias (Codigo, Nombre, Descripcion, Creditos, Activa, Tipo, CicloSugerido, Estado, FechaCreacion) VALUES
('MEP-MAT-7', 'Matemáticas 7°', 'Matemáticas para séptimo año', 6, 1, 'Básica', 7, 'ACTIVO', datetime('now')),
('MEP-MAT-8', 'Matemáticas 8°', 'Matemáticas para octavo año', 6, 1, 'Básica', 8, 'ACTIVO', datetime('now')),
('MEP-MAT-9', 'Matemáticas 9°', 'Matemáticas para noveno año', 6, 1, 'Básica', 9, 'ACTIVO', datetime('now'));

-- CIENCIAS
INSERT INTO Materias (Codigo, Nombre, Descripcion, Creditos, Activa, Tipo, CicloSugerido, Estado, FechaCreacion) VALUES
('MEP-CIE-7', 'Ciencias 7°', 'Ciencias naturales integradas para séptimo año', 4, 1, 'Básica', 7, 'ACTIVO', datetime('now')),
('MEP-CIE-8', 'Ciencias 8°', 'Ciencias naturales integradas para octavo año', 4, 1, 'Básica', 8, 'ACTIVO', datetime('now')),
('MEP-CIE-9', 'Ciencias 9°', 'Ciencias naturales integradas para noveno año', 4, 1, 'Básica', 9, 'ACTIVO', datetime('now'));

-- ESTUDIOS SOCIALES
INSERT INTO Materias (Codigo, Nombre, Descripcion, Creditos, Activa, Tipo, CicloSugerido, Estado, FechaCreacion) VALUES
('MEP-SS-7', 'Estudios Sociales 7°', 'Historia, geografía y cívica para séptimo año', 4, 1, 'Básica', 7, 'ACTIVO', datetime('now')),
('MEP-SS-8', 'Estudios Sociales 8°', 'Historia, geografía y cívica para octavo año', 4, 1, 'Básica', 8, 'ACTIVO', datetime('now')),
('MEP-SS-9', 'Estudios Sociales 9°', 'Historia, geografía y cívica para noveno año', 4, 1, 'Básica', 9, 'ACTIVO', datetime('now'));

-- CÍVICA
INSERT INTO Materias (Codigo, Nombre, Descripcion, Creditos, Activa, Tipo, CicloSugerido, Estado, FechaCreacion) VALUES
('MEP-CIV-7', 'Cívica 7°', 'Educación cívica para séptimo año', 2, 1, 'Básica', 7, 'ACTIVO', datetime('now')),
('MEP-CIV-8', 'Cívica 8°', 'Educación cívica para octavo año', 2, 1, 'Básica', 8, 'ACTIVO', datetime('now')),
('MEP-CIV-9', 'Cívica 9°', 'Educación cívica para noveno año', 2, 1, 'Básica', 9, 'ACTIVO', datetime('now'));

-- INGLÉS
INSERT INTO Materias (Codigo, Nombre, Descripcion, Creditos, Activa, Tipo, CicloSugerido, Estado, FechaCreacion) VALUES
('MEP-ING-7', 'Inglés 7°', 'Idioma inglés para séptimo año', 4, 1, 'Básica', 7, 'ACTIVO', datetime('now')),
('MEP-ING-8', 'Inglés 8°', 'Idioma inglés para octavo año', 4, 1, 'Básica', 8, 'ACTIVO', datetime('now')),
('MEP-ING-9', 'Inglés 9°', 'Idioma inglés para noveno año', 4, 1, 'Básica', 9, 'ACTIVO', datetime('now'));

-- =====================================================================
-- MATERIAS COMPLEMENTARIAS - III CICLO
-- =====================================================================

-- EDUCACIÓN FÍSICA
INSERT INTO Materias (Codigo, Nombre, Descripcion, Creditos, Activa, Tipo, CicloSugerido, Estado, FechaCreacion) VALUES
('MEP-EF-7', 'Educación Física 7°', 'Actividad física y deportes para séptimo año', 2, 1, 'Complementaria', 7, 'ACTIVO', datetime('now')),
('MEP-EF-8', 'Educación Física 8°', 'Actividad física y deportes para octavo año', 2, 1, 'Complementaria', 8, 'ACTIVO', datetime('now')),
('MEP-EF-9', 'Educación Física 9°', 'Actividad física y deportes para noveno año', 2, 1, 'Complementaria', 9, 'ACTIVO', datetime('now'));

-- EDUCACIÓN MUSICAL
INSERT INTO Materias (Codigo, Nombre, Descripcion, Creditos, Activa, Tipo, CicloSugerido, Estado, FechaCreacion) VALUES
('MEP-MUS-7', 'Educación Musical 7°', 'Apreciación y práctica musical para séptimo año', 2, 1, 'Artística', 7, 'ACTIVO', datetime('now')),
('MEP-MUS-8', 'Educación Musical 8°', 'Apreciación y práctica musical para octavo año', 2, 1, 'Artística', 8, 'ACTIVO', datetime('now')),
('MEP-MUS-9', 'Educación Musical 9°', 'Apreciación y práctica musical para noveno año', 2, 1, 'Artística', 9, 'ACTIVO', datetime('now'));

-- ARTES PLÁSTICAS
INSERT INTO Materias (Codigo, Nombre, Descripcion, Creditos, Activa, Tipo, CicloSugerido, Estado, FechaCreacion) VALUES
('MEP-ART-7', 'Artes Plásticas 7°', 'Expresión artística visual para séptimo año', 2, 1, 'Artística', 7, 'ACTIVO', datetime('now')),
('MEP-ART-8', 'Artes Plásticas 8°', 'Expresión artística visual para octavo año', 2, 1, 'Artística', 8, 'ACTIVO', datetime('now')),
('MEP-ART-9', 'Artes Plásticas 9°', 'Expresión artística visual para noveno año', 2, 1, 'Artística', 9, 'ACTIVO', datetime('now'));

-- EDUCACIÓN PARA EL HOGAR
INSERT INTO Materias (Codigo, Nombre, Descripcion, Creditos, Activa, Tipo, CicloSugerido, Estado, FechaCreacion) VALUES
('MEP-HOG-7', 'Educación para el Hogar 7°', 'Habilidades domésticas y vida familiar para séptimo año', 2, 1, 'Técnica', 7, 'ACTIVO', datetime('now')),
('MEP-HOG-8', 'Educación para el Hogar 8°', 'Habilidades domésticas y vida familiar para octavo año', 2, 1, 'Técnica', 8, 'ACTIVO', datetime('now')),
('MEP-HOG-9', 'Educación para el Hogar 9°', 'Habilidades domésticas y vida familiar para noveno año', 2, 1, 'Técnica', 9, 'ACTIVO', datetime('now'));

-- ARTES INDUSTRIALES
INSERT INTO Materias (Codigo, Nombre, Descripcion, Creditos, Activa, Tipo, CicloSugerido, Estado, FechaCreacion) VALUES
('MEP-IND-7', 'Artes Industriales 7°', 'Técnicas y manualidades industriales para séptimo año', 2, 1, 'Técnica', 7, 'ACTIVO', datetime('now')),
('MEP-IND-8', 'Artes Industriales 8°', 'Técnicas y manualidades industriales para octavo año', 2, 1, 'Técnica', 8, 'ACTIVO', datetime('now')),
('MEP-IND-9', 'Artes Industriales 9°', 'Técnicas y manualidades industriales para noveno año', 2, 1, 'Técnica', 9, 'ACTIVO', datetime('now'));

-- ORIENTACIÓN
INSERT INTO Materias (Codigo, Nombre, Descripcion, Creditos, Activa, Tipo, CicloSugerido, Estado, FechaCreacion) VALUES
('MEP-ORI-7', 'Orientación 7°', 'Desarrollo personal y vocacional para séptimo año', 1, 1, 'Complementaria', 7, 'ACTIVO', datetime('now')),
('MEP-ORI-8', 'Orientación 8°', 'Desarrollo personal y vocacional para octavo año', 1, 1, 'Complementaria', 8, 'ACTIVO', datetime('now')),
('MEP-ORI-9', 'Orientación 9°', 'Desarrollo personal y vocacional para noveno año', 1, 1, 'Complementaria', 9, 'ACTIVO', datetime('now'));

-- EDUCACIÓN RELIGIOSA
INSERT INTO Materias (Codigo, Nombre, Descripcion, Creditos, Activa, Tipo, CicloSugerido, Estado, FechaCreacion) VALUES
('MEP-REL-7', 'Educación Religiosa 7°', 'Formación en valores espirituales para séptimo año', 2, 1, 'Complementaria', 7, 'ACTIVO', datetime('now')),
('MEP-REL-8', 'Educación Religiosa 8°', 'Formación en valores espirituales para octavo año', 2, 1, 'Complementaria', 8, 'ACTIVO', datetime('now')),
('MEP-REL-9', 'Educación Religiosa 9°', 'Formación en valores espirituales para noveno año', 2, 1, 'Complementaria', 9, 'ACTIVO', datetime('now'));

-- =====================================================================
-- EDUCACIÓN DIVERSIFICADA (10° y 11°)
-- =====================================================================

-- ESPAÑOL Y LITERATURA
INSERT INTO Materias (Codigo, Nombre, Descripcion, Creditos, Activa, Tipo, CicloSugerido, Estado, FechaCreacion) VALUES
('MEP-ESP-10', 'Español 10°', 'Comunicación avanzada y literatura para décimo año', 6, 1, 'Básica', 10, 'ACTIVO', datetime('now')),
('MEP-ESP-11', 'Español 11°', 'Comunicación avanzada y literatura para undécimo año', 6, 1, 'Básica', 11, 'ACTIVO', datetime('now')),
('MEP-LIT-10', 'Literatura 10°', 'Análisis literario para décimo año', 4, 1, 'Básica', 10, 'ACTIVO', datetime('now')),
('MEP-LIT-11', 'Literatura 11°', 'Análisis literario para undécimo año', 4, 1, 'Básica', 11, 'ACTIVO', datetime('now'));

-- MATEMÁTICAS
INSERT INTO Materias (Codigo, Nombre, Descripcion, Creditos, Activa, Tipo, CicloSugerido, Estado, FechaCreacion) VALUES
('MEP-MAT-10', 'Matemática General 10°', 'Matemática general para décimo año', 6, 1, 'Básica', 10, 'ACTIVO', datetime('now')),
('MEP-MAT-11', 'Matemática Superior 11°', 'Matemática superior para undécimo año', 6, 1, 'Básica', 11, 'ACTIVO', datetime('now')),
('MEP-GEO-10', 'Geometría y Trigonometría 10°', 'Geometría y trigonometría para décimo año', 4, 1, 'Básica', 10, 'ACTIVO', datetime('now'));

-- CIENCIAS ESPECÍFICAS
INSERT INTO Materias (Codigo, Nombre, Descripcion, Creditos, Activa, Tipo, CicloSugerido, Estado, FechaCreacion) VALUES
('MEP-BIO-10', 'Biología 10°', 'Biología general para décimo año', 5, 1, 'Básica', 10, 'ACTIVO', datetime('now')),
('MEP-BIO-11', 'Biología 11°', 'Biología avanzada para undécimo año', 5, 1, 'Básica', 11, 'ACTIVO', datetime('now')),
('MEP-FIS-10', 'Física 10°', 'Física general para décimo año', 5, 1, 'Básica', 10, 'ACTIVO', datetime('now')),
('MEP-FIS-11', 'Física 11°', 'Física avanzada para undécimo año', 5, 1, 'Básica', 11, 'ACTIVO', datetime('now')),
('MEP-QUI-10', 'Química 10°', 'Química general para décimo año', 5, 1, 'Básica', 10, 'ACTIVO', datetime('now')),
('MEP-QUI-11', 'Química 11°', 'Química avanzada para undécimo año', 5, 1, 'Básica', 11, 'ACTIVO', datetime('now'));

-- ESTUDIOS SOCIALES ESPECÍFICOS
INSERT INTO Materias (Codigo, Nombre, Descripcion, Creditos, Activa, Tipo, CicloSugerido, Estado, FechaCreacion) VALUES
('MEP-HIS-10', 'Historia 10°', 'Historia universal y nacional para décimo año', 4, 1, 'Básica', 10, 'ACTIVO', datetime('now')),
('MEP-HIS-11', 'Historia 11°', 'Historia contemporánea para undécimo año', 4, 1, 'Básica', 11, 'ACTIVO', datetime('now')),
('MEP-GEO-10S', 'Geografía 10°', 'Geografía física y humana para décimo año', 4, 1, 'Básica', 10, 'ACTIVO', datetime('now')),
('MEP-GEO-11S', 'Geografía 11°', 'Geografía económica para undécimo año', 4, 1, 'Básica', 11, 'ACTIVO', datetime('now'));

-- CÍVICA DIVERSIFICADA
INSERT INTO Materias (Codigo, Nombre, Descripcion, Creditos, Activa, Tipo, CicloSugerido, Estado, FechaCreacion) VALUES
('MEP-CIV-10', 'Cívica 10°', 'Educación cívica avanzada para décimo año', 3, 1, 'Básica', 10, 'ACTIVO', datetime('now')),
('MEP-CIV-11', 'Cívica 11°', 'Educación cívica y democracia para undécimo año', 3, 1, 'Básica', 11, 'ACTIVO', datetime('now'));

-- FILOSOFÍA
INSERT INTO Materias (Codigo, Nombre, Descripcion, Creditos, Activa, Tipo, CicloSugerido, Estado, FechaCreacion) VALUES
('MEP-FIL-11', 'Filosofía 11°', 'Introducción al pensamiento filosófico', 3, 1, 'Básica', 11, 'ACTIVO', datetime('now')),
('MEP-LOG-11', 'Lógica 11°', 'Lógica y argumentación', 2, 1, 'Complementaria', 11, 'ACTIVO', datetime('now'));

-- INGLÉS DIVERSIFICADA
INSERT INTO Materias (Codigo, Nombre, Descripcion, Creditos, Activa, Tipo, CicloSugerido, Estado, FechaCreacion) VALUES
('MEP-ING-10', 'Inglés 10°', 'Inglés intermedio para décimo año', 4, 1, 'Básica', 10, 'ACTIVO', datetime('now')),
('MEP-ING-11', 'Inglés 11°', 'Inglés avanzado para undécimo año', 4, 1, 'Básica', 11, 'ACTIVO', datetime('now'));

-- EDUCACIÓN FÍSICA DIVERSIFICADA
INSERT INTO Materias (Codigo, Nombre, Descripcion, Creditos, Activa, Tipo, CicloSugerido, Estado, FechaCreacion) VALUES
('MEP-EF-10', 'Educación Física 10°', 'Actividad física y salud para décimo año', 2, 1, 'Complementaria', 10, 'ACTIVO', datetime('now')),
('MEP-EF-11', 'Educación Física 11°', 'Actividad física y salud para undécimo año', 2, 1, 'Complementaria', 11, 'ACTIVO', datetime('now'));

-- ORIENTACIÓN DIVERSIFICADA
INSERT INTO Materias (Codigo, Nombre, Descripcion, Creditos, Activa, Tipo, CicloSugerido, Estado, FechaCreacion) VALUES
('MEP-ORI-10', 'Orientación 10°', 'Orientación vocacional para décimo año', 1, 1, 'Complementaria', 10, 'ACTIVO', datetime('now')),
('MEP-ORI-11', 'Orientación 11°', 'Preparación para la vida universitaria', 1, 1, 'Complementaria', 11, 'ACTIVO', datetime('now'));

-- EDUCACIÓN RELIGIOSA DIVERSIFICADA
INSERT INTO Materias (Codigo, Nombre, Descripcion, Creditos, Activa, Tipo, CicloSugerido, Estado, FechaCreacion) VALUES
('MEP-REL-10', 'Educación Religiosa 10°', 'Valores y espiritualidad para décimo año', 2, 1, 'Complementaria', 10, 'ACTIVO', datetime('now')),
('MEP-REL-11', 'Educación Religiosa 11°', 'Ética y valores para undécimo año', 2, 1, 'Complementaria', 11, 'ACTIVO', datetime('now'));

-- =====================================================================
-- MATERIAS TÉCNICAS (PARA COLEGIOS TÉCNICOS)
-- =====================================================================

-- INFORMÁTICA
INSERT INTO Materias (Codigo, Nombre, Descripcion, Creditos, Activa, Tipo, CicloSugerido, Estado, FechaCreacion) VALUES
('MEP-INF-TEC', 'Informática Técnica', 'Especialidad técnica en informática y computación', 8, 1, 'Técnica Especializada', 10, 'ACTIVO', datetime('now')),
('MEP-PROG-TEC', 'Programación Técnica', 'Desarrollo de software y programación', 8, 1, 'Técnica Especializada', 11, 'ACTIVO', datetime('now'));

-- CONTABILIDAD
INSERT INTO Materias (Codigo, Nombre, Descripcion, Creditos, Activa, Tipo, CicloSugerido, Estado, FechaCreacion) VALUES
('MEP-CONT-TEC', 'Contabilidad Técnica', 'Especialidad técnica en contabilidad y finanzas', 8, 1, 'Técnica Especializada', 10, 'ACTIVO', datetime('now')),
('MEP-ADM-TEC', 'Administración Técnica', 'Administración de empresas nivel técnico', 8, 1, 'Técnica Especializada', 11, 'ACTIVO', datetime('now'));

-- SECRETARIADO
INSERT INTO Materias (Codigo, Nombre, Descripcion, Creditos, Activa, Tipo, CicloSugerido, Estado, FechaCreacion) VALUES
('MEP-SEC-TEC', 'Secretariado Técnico', 'Especialidad técnica en secretariado ejecutivo', 8, 1, 'Técnica Especializada', 10, 'ACTIVO', datetime('now')),
('MEP-OFIC-TEC', 'Técnicas de Oficina', 'Manejo integral de oficina', 8, 1, 'Técnica Especializada', 11, 'ACTIVO', datetime('now'));

-- ELECTRÓNICA
INSERT INTO Materias (Codigo, Nombre, Descripcion, Creditos, Activa, Tipo, CicloSugerido, Estado, FechaCreacion) VALUES
('MEP-ELEC-TEC', 'Electrónica Técnica', 'Especialidad técnica en electrónica', 8, 1, 'Técnica Especializada', 10, 'ACTIVO', datetime('now')),
('MEP-CIRC-TEC', 'Circuitos Electrónicos', 'Diseño y reparación de circuitos', 8, 1, 'Técnica Especializada', 11, 'ACTIVO', datetime('now'));

-- MECÁNICA AUTOMOTRIZ
INSERT INTO Materias (Codigo, Nombre, Descripcion, Creditos, Activa, Tipo, CicloSugerido, Estado, FechaCreacion) VALUES
('MEP-MECA-TEC', 'Mecánica Automotriz', 'Especialidad técnica en mecánica de vehículos', 8, 1, 'Técnica Especializada', 10, 'ACTIVO', datetime('now')),
('MEP-MOTO-TEC', 'Motores y Sistemas', 'Sistemas automotrices avanzados', 8, 1, 'Técnica Especializada', 11, 'ACTIVO', datetime('now'));

-- CONSTRUCCIÓN CIVIL
INSERT INTO Materias (Codigo, Nombre, Descripcion, Creditos, Activa, Tipo, CicloSugerido, Estado, FechaCreacion) VALUES
('MEP-CONS-TEC', 'Construcción Civil', 'Especialidad técnica en construcción', 8, 1, 'Técnica Especializada', 10, 'ACTIVO', datetime('now')),
('MEP-ARQU-TEC', 'Dibujo Arquitectónico', 'Planos y diseño de construcciones', 8, 1, 'Técnica Especializada', 11, 'ACTIVO', datetime('now'));

-- AGROPECUARIA
INSERT INTO Materias (Codigo, Nombre, Descripcion, Creditos, Activa, Tipo, CicloSugerido, Estado, FechaCreacion) VALUES
('MEP-AGRO-TEC', 'Agropecuaria Técnica', 'Especialidad técnica agropecuaria', 8, 1, 'Técnica Especializada', 10, 'ACTIVO', datetime('now')),
('MEP-ZOOT-TEC', 'Zootecnia Técnica', 'Manejo de animales y producción', 8, 1, 'Técnica Especializada', 11, 'ACTIVO', datetime('now'));

-- TURISMO
INSERT INTO Materias (Codigo, Nombre, Descripcion, Creditos, Activa, Tipo, CicloSugerido, Estado, FechaCreacion) VALUES
('MEP-TUR-TEC', 'Turismo Técnico', 'Especialidad técnica en turismo', 8, 1, 'Técnica Especializada', 10, 'ACTIVO', datetime('now')),
('MEP-HOST-TEC', 'Hotelería y Hospedaje', 'Servicios hoteleros y turísticos', 8, 1, 'Técnica Especializada', 11, 'ACTIVO', datetime('now'));

-- =====================================================================
-- MATERIAS COMPLEMENTARIAS Y ELECTIVAS
-- =====================================================================

-- TECNOLOGÍAS DE INFORMACIÓN
INSERT INTO Materias (Codigo, Nombre, Descripcion, Creditos, Activa, Tipo, CicloSugerido, Estado, FechaCreacion) VALUES
('MEP-TIC-7', 'TIC 7°', 'Tecnologías de la información y comunicación', 2, 1, 'Tecnológica', 7, 'ACTIVO', datetime('now')),
('MEP-TIC-8', 'TIC 8°', 'Tecnologías de la información y comunicación', 2, 1, 'Tecnológica', 8, 'ACTIVO', datetime('now')),
('MEP-TIC-9', 'TIC 9°', 'Tecnologías de la información y comunicación', 2, 1, 'Tecnológica', 9, 'ACTIVO', datetime('now')),
('MEP-TIC-10', 'TIC 10°', 'Tecnologías avanzadas de información', 2, 1, 'Tecnológica', 10, 'ACTIVO', datetime('now')),
('MEP-TIC-11', 'TIC 11°', 'Tecnologías emergentes', 2, 1, 'Tecnológica', 11, 'ACTIVO', datetime('now'));

-- FRANCÉS (OPTATIVA)
INSERT INTO Materias (Codigo, Nombre, Descripcion, Creditos, Activa, Tipo, CicloSugerido, Estado, FechaCreacion) VALUES
('MEP-FRA-10', 'Francés 10°', 'Idioma francés básico', 3, 1, 'Electiva', 10, 'ACTIVO', datetime('now')),
('MEP-FRA-11', 'Francés 11°', 'Idioma francés intermedio', 3, 1, 'Electiva', 11, 'ACTIVO', datetime('now'));

-- PSICOLOGÍA
INSERT INTO Materias (Codigo, Nombre, Descripcion, Creditos, Activa, Tipo, CicloSugerido, Estado, FechaCreacion) VALUES
('MEP-PSI-11', 'Psicología 11°', 'Introducción a la psicología', 3, 1, 'Electiva', 11, 'ACTIVO', datetime('now'));

-- ESTADÍSTICA
INSERT INTO Materias (Codigo, Nombre, Descripcion, Creditos, Activa, Tipo, CicloSugerido, Estado, FechaCreacion) VALUES
('MEP-EST-11', 'Estadística 11°', 'Estadística descriptiva e inferencial', 3, 1, 'Complementaria', 11, 'ACTIVO', datetime('now'));

-- TALLERES ARTÍSTICOS
INSERT INTO Materias (Codigo, Nombre, Descripcion, Creditos, Activa, Tipo, CicloSugerido, Estado, FechaCreacion) VALUES
('MEP-BANDA', 'Banda Musical', 'Taller de banda escolar', 2, 1, 'Artística', 0, 'ACTIVO', datetime('now')),
('MEP-DANZA', 'Danza Folclórica', 'Danzas tradicionales costarricenses', 2, 1, 'Artística', 0, 'ACTIVO', datetime('now')),
('MEP-TEATRO', 'Teatro Escolar', 'Expresión dramática y teatro', 2, 1, 'Artística', 0, 'ACTIVO', datetime('now')),
('MEP-PERIO', 'Periodismo Escolar', 'Comunicación y medios escolares', 2, 1, 'Complementaria', 0, 'ACTIVO', datetime('now'));

-- TALLERES ESPECIALES
INSERT INTO Materias (Codigo, Nombre, Descripcion, Creditos, Activa, Tipo, CicloSugerido, Estado, FechaCreacion) VALUES
('MEP-ECOL', 'Ecología y Ambiente', 'Educación ambiental y sostenibilidad', 2, 1, 'Complementaria', 0, 'ACTIVO', datetime('now')),
('MEP-PAUX', 'Primeros Auxilios', 'Técnicas básicas de primeros auxilios', 1, 1, 'Complementaria', 0, 'ACTIVO', datetime('now')),
('MEP-EMPRE', 'Emprendimiento', 'Desarrollo de proyectos emprendedores', 3, 1, 'Complementaria', 0, 'ACTIVO', datetime('now'));

-- =====================================================================
-- VERIFICACIÓN DEL SCRIPT
-- =====================================================================

-- Contar materias insertadas por tipo
SELECT 
    Tipo, 
    COUNT(*) as Cantidad,
    GROUP_CONCAT(CicloSugerido) as Ciclos
FROM Materias 
WHERE Codigo LIKE 'MEP%'
GROUP BY Tipo
ORDER BY Tipo;

-- Mostrar resumen por ciclo educativo
SELECT 
    CASE 
        WHEN CicloSugerido = 7 THEN 'Séptimo'
        WHEN CicloSugerido = 8 THEN 'Octavo'
        WHEN CicloSugerido = 9 THEN 'Noveno'
        WHEN CicloSugerido = 10 THEN 'Décimo'
        WHEN CicloSugerido = 11 THEN 'Undécimo'
        ELSE 'Transversal'
    END as Nivel,
    COUNT(*) as CantidadMaterias
FROM Materias 
WHERE Codigo LIKE 'MEP%'
GROUP BY CicloSugerido
ORDER BY CicloSugerido;

-- Verificar total de materias MEP
SELECT 'Total Materias MEP' as Descripcion, COUNT(*) as Total
FROM Materias 
WHERE Codigo LIKE 'MEP%';
