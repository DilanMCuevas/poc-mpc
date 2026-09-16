USE escuela;

-- PROGRAMAS ACADÉMICOS
INSERT INTO programas_academicos
    (id, nombre, clave, creditos_totales)
VALUES
    (1, 'Ingeniería en Sistemas Computacionales', 'ISC', 220),
    (2, 'Ingeniería en Inteligencia Artificial', 'IIA', 220);

-- ESTUDIANTES
INSERT INTO estudiantes
    (id, programa_academico_id, boleta, nombre, correo, telefono)
VALUES
    (1, 1, '2025000001', 'Ana Martínez López',
        'ana.martinez@alumno.mx', '5510000001'),

    (2, 1, '2025000002', 'Luis Hernández García',
        'luis.hernandez@alumno.mx', '5510000002'),

    (3, 1, '2025000003', 'Sofía Ramírez Torres',
        'sofia.ramirez@alumno.mx', '5510000003'),

    (4, 1, '2025000004', 'Diego Sánchez Ruiz',
        'diego.sanchez@alumno.mx', '5510000004'),

    (5, 1, '2024000005', 'Valeria Cruz Mendoza',
        'valeria.cruz@alumno.mx', '5510000005'),

    (6, 1, '2025000006', 'Mateo Flores Jiménez',
        'mateo.flores@alumno.mx', '5510000006'),

    (7, 2, '2025000007', 'Camila Ortega Reyes',
        'camila.ortega@alumno.mx', '5510000007'),

    (8, 2, '2025000008', 'Emiliano Vargas Soto',
        'emiliano.vargas@alumno.mx', '5510000008'),

    (9, 2, '2025000009', 'Regina Navarro Díaz',
        'regina.navarro@alumno.mx', '5510000009'),

    (10, 2, '2025000010', 'Daniel Morales Vega',
        'daniel.morales@alumno.mx', '5510000010');

-- PROFESORES
INSERT INTO profesores
    (id, numero_empleado, nombre, correo, telefono)
VALUES
    (1, 'EMP001', 'Carlos Ramírez',
        'carlos.ramirez@escuela.mx', '5520000001'),

    (2, 'EMP002', 'Laura Hernández',
        'laura.hernandez@escuela.mx', '5520000002'),

    (3, 'EMP003', 'Fernando López',
        'fernando.lopez@escuela.mx', '5520000003'),

    (4, 'EMP004', 'Mariana Torres',
        'mariana.torres@escuela.mx', '5520000004'),

    (5, 'EMP005', 'Jorge Mendoza',
        'jorge.mendoza@escuela.mx', '5520000005'),

    (6, 'EMP006', 'Patricia Sánchez',
        'patricia.sanchez@escuela.mx', '5520000006'),

    (7, 'EMP007', 'Ricardo Gómez',
        'ricardo.gomez@escuela.mx', '5520000007'),

    (8, 'EMP008', 'Alejandra Rivera',
        'alejandra.rivera@escuela.mx', '5520000008'),

    (9, 'EMP009', 'Miguel Castro',
        'miguel.castro@escuela.mx', '5520000009'),

    (10, 'EMP010', 'Gabriela Ortiz',
        'gabriela.ortiz@escuela.mx', '5520000010');

-- SALONES
INSERT INTO salones
    (id, numero, capacidad, tipo)
VALUES
    (1, '4012', 40, 'AULA'),
    (2, '4013', 40, 'AULA'),
    (3, '4014', 35, 'AULA'),
    (4, 'LBD1', 30, 'LABORATORIO'),
    (5, 'LIA1', 30, 'LABORATORIO'),
    (6, 'LDD1', 25, 'LABORATORIO');

-- MATERIAS - ISC
INSERT INTO materias
    (id, programa_academico_id, clave, nombre, creditos, semestre)
VALUES
    (1, 1, 'ISC101', 'Programación I', 8, 1),
    (2, 1, 'ISC102', 'Matemáticas Discretas', 8, 1),

    (3, 1, 'ISC201', 'Programación Orientada a Objetos', 8, 2),
    (4, 1, 'ISC202', 'Bases de Datos', 8, 2),
    (5, 1, 'ISC203', 'Análisis y Diseño de Algoritmos', 8, 2),
    (6, 1, 'ISC204', 'Ecuaciones Diferenciales', 8, 2),
    (7, 1, 'ISC205', 'Administración Financiera', 6, 2),
    (8, 1, 'ISC206', 'Diseño Digital', 8, 2),

    (9, 1, 'ISC301', 'Sistemas Operativos', 8, 3),
    (10, 1, 'ISC302', 'Probabilidad', 8, 3),

    (11, 1, 'ISC401', 'Machine Learning', 8, 4),
    (12, 1, 'ISC402', 'Inteligencia Artificial', 8, 4);

-- MATERIAS - IIA
INSERT INTO materias
    (id, programa_academico_id, clave, nombre, creditos, semestre)
VALUES
    (13, 2, 'IIA101', 'Fundamentos de Inteligencia Artificial', 8, 1),
    (14, 2, 'IIA102', 'Programación para Inteligencia Artificial', 8, 1),

    (15, 2, 'IIA201', 'Aprendizaje Automático', 8, 2),
    (16, 2, 'IIA202', 'Estadística para Inteligencia Artificial', 8, 2),

    (17, 2, 'IIA301', 'Procesamiento de Lenguaje Natural', 8, 3),
    (18, 2, 'IIA302', 'Visión Artificial', 8, 3);

-- GRUPOS
-- Se incluyen grupos históricos y actuales porque Mongo puede
-- hacer referencia a ofertas académicas anteriores.
INSERT INTO grupos
    (id, clave_grupo, periodo, semestre)
VALUES
    (1, '1CM1', '2025-1', 1),

    (2, '2CM1', '2025-2', 2),
    (3, '2CV1', '2025-2', 2),

    (4, '3CM1', '2026-1', 3),

    (5, '2CM2', '2026-2', 2),
    (6, '2CV2', '2026-2', 2),
    (7, '3CM1', '2026-2', 3),
    (8, '4CV1', '2026-2', 4),

    (9, '2AM1', '2025-2', 2),
    (10, '3AM1', '2026-1', 3),
    (11, '3AM1', '2026-2', 3);

-- OFERTA ACADÉMICA: GRUPOS_MATERIA
INSERT INTO grupos_materia
    (id, grupo_id, materia_id, profesor_id, cupo)
VALUES

    -- 1CM1 / 2025-1
    (1, 1, 1, 1, 35),
    (2, 1, 2, 2, 35),

    -- 2CM1 / 2025-2
    (3, 2, 3, 3, 35),
    (4, 2, 4, 4, 35),
    (5, 2, 5, 5, 35),
    (6, 2, 6, 6, 35),
    (7, 2, 7, 7, 35),
    (8, 2, 8, 8, 30),

    -- 2CV1 / 2025-2
    (9, 3, 3, 9, 35),
    (10, 3, 4, 1, 35),
    (11, 3, 5, 5, 35),
    (12, 3, 6, 2, 35),

    -- 3CM1 / 2026-1
    (13, 4, 9, 3, 35),
    (14, 4, 10, 6, 35),

    -- 2CM2 / 2026-2
    (15, 5, 3, 3, 35),
    (16, 5, 4, 1, 35),
    (17, 5, 5, 5, 35),
    (18, 5, 6, 6, 35),
    (19, 5, 7, 7, 35),
    (20, 5, 8, 8, 30),

    -- 2CV2 / 2026-2
    (21, 6, 3, 9, 35),
    (22, 6, 4, 4, 35),
    (23, 6, 5, 10, 35),
    (24, 6, 6, 2, 35),

    -- 3CM1 / 2026-2
    (25, 7, 9, 3, 35),
    (26, 7, 10, 6, 35),

    -- 4CV1 / 2026-2
    (27, 8, 11, 10, 30),
    (28, 8, 12, 5, 30),

    -- IIA histórico
    (29, 9, 15, 10, 30),
    (30, 9, 16, 6, 30),

    (31, 10, 17, 5, 30),
    (32, 10, 18, 8, 30),

    -- IIA actual
    (33, 11, 17, 5, 30),
    (34, 11, 18, 8, 30);

-- HORARIOS
-- Hay materias en aula y otras con sesiones de laboratorio.
-- Todos los bloques cumplen el CHECK de mínimo 90 minutos.
INSERT INTO horarios
    (id, grupo_materia_id, salon_id, dia, hora_inicio, hora_fin)
VALUES

    -- 2CM2
    (1, 15, 1, 'LUNES',     '08:30:00', '10:00:00'),
    (2, 15, 4, 'MIERCOLES', '08:30:00', '10:00:00'),

    (3, 16, 1, 'LUNES',     '10:30:00', '12:00:00'),
    (4, 16, 4, 'JUEVES',    '10:30:00', '12:00:00'),

    (5, 17, 1, 'MARTES',    '12:30:00', '14:00:00'),
    (6, 17, 1, 'JUEVES',    '12:30:00', '14:00:00'),

    (7, 18, 1, 'MARTES',    '14:30:00', '16:00:00'),
    (8, 18, 1, 'VIERNES',   '14:30:00', '16:00:00'),

    (9, 19, 1, 'LUNES',     '16:30:00', '18:00:00'),

    (10, 20, 1, 'MIERCOLES','16:30:00', '18:00:00'),
    (11, 20, 6, 'VIERNES',  '16:30:00', '18:00:00'),

    -- 2CV2
    (12, 21, 2, 'LUNES',     '08:30:00', '10:00:00'),
    (13, 22, 2, 'MARTES',    '10:30:00', '12:00:00'),
    (14, 22, 4, 'JUEVES',    '10:30:00', '12:00:00'),
    (15, 23, 2, 'MIERCOLES', '12:30:00', '14:00:00'),
    (16, 24, 2, 'VIERNES',   '12:30:00', '14:00:00'),

    -- 3CM1
    (17, 25, 3, 'LUNES',     '10:30:00', '12:00:00'),
    (18, 25, 5, 'MIERCOLES', '10:30:00', '12:00:00'),

    (19, 26, 3, 'MARTES',    '08:30:00', '10:00:00'),
    (20, 26, 3, 'JUEVES',    '08:30:00', '10:00:00'),

    -- 4CV1
    (21, 27, 3, 'LUNES',     '12:30:00', '14:00:00'),
    (22, 27, 5, 'MIERCOLES', '12:30:00', '14:00:00'),

    (23, 28, 3, 'MARTES',    '14:30:00', '16:00:00'),
    (24, 28, 5, 'JUEVES',    '14:30:00', '16:00:00'),

    -- IIA
    (25, 33, 2, 'LUNES',     '14:30:00', '16:00:00'),
    (26, 33, 5, 'MIERCOLES', '14:30:00', '16:00:00'),

    (27, 34, 2, 'MARTES',    '16:30:00', '18:00:00'),
    (28, 34, 5, 'JUEVES',    '16:30:00', '18:00:00');

-- INSCRIPCIONES ACTUALES
INSERT INTO inscripciones
    (id, estudiante_id, grupo_materia_id, fecha_inscripcion, estado)
VALUES

    -- Ana: actualmente recursa BD con Carlos
    (1, 1, 15, '2026-07-20', 'ACTIVA'),
    (2, 1, 16, '2026-07-20', 'ACTIVA'),
    (3, 1, 17, '2026-07-20', 'ACTIVA'),

    -- Luis
    (4, 2, 15, '2026-07-20', 'ACTIVA'),
    (5, 2, 17, '2026-07-20', 'ACTIVA'),
    (6, 2, 18, '2026-07-20', 'ACTIVA'),

    -- Sofía
    (7, 3, 25, '2026-07-20', 'ACTIVA'),
    (8, 3, 26, '2026-07-20', 'ACTIVA'),

    -- Diego
    (9, 4, 16, '2026-07-21', 'ACTIVA'),
    (10, 4, 18, '2026-07-21', 'ACTIVA'),

    -- Valeria
    (11, 5, 27, '2026-07-21', 'ACTIVA'),
    (12, 5, 28, '2026-07-21', 'ACTIVA'),

    -- Mateo
    (13, 6, 16, '2026-07-21', 'ACTIVA'),
    (14, 6, 17, '2026-07-21', 'ACTIVA'),

    -- Camila
    (15, 7, 33, '2026-07-22', 'ACTIVA'),
    (16, 7, 34, '2026-07-22', 'ACTIVA'),

    -- Emiliano
    (17, 8, 33, '2026-07-22', 'ACTIVA'),

    -- Regina
    (18, 9, 33, '2026-07-22', 'ACTIVA'),
    (19, 9, 34, '2026-07-22', 'ACTIVA'),

    -- Daniel
    (20, 10, 34, '2026-07-22', 'ACTIVA');