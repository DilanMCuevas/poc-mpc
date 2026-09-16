USE escuela;
-- Dataset v1: formulas mirrored in db/mongodb/seed.js. No randomness or current clock.
-- Fresh database only. Duplicate keys stop accidental reruns; no DELETE/TRUNCATE.
-- Golden IDs 1..10; Sofia keeps ID/boleta/name and historical assignments 1,2,9,10,11.
CREATE TEMPORARY TABLE seed_digits (n INT PRIMARY KEY);
INSERT INTO seed_digits VALUES (0),(1),(2),(3),(4),(5),(6),(7),(8),(9);
CREATE TEMPORARY TABLE seed_numbers AS
WITH digits AS (SELECT 0 AS n UNION ALL SELECT 1 UNION ALL SELECT 2 UNION ALL SELECT 3 UNION ALL SELECT 4
 UNION ALL SELECT 5 UNION ALL SELECT 6 UNION ALL SELECT 7 UNION ALL SELECT 8 UNION ALL SELECT 9)
SELECT a.n+10*b.n+100*c.n+1000*d.n AS n
FROM digits a CROSS JOIN digits b CROSS JOIN digits c CROSS JOIN digits d;
CREATE TEMPORARY TABLE seed_golden (
 id INT PRIMARY KEY, programa INT, etapa INT, boleta VARCHAR(20), nombre VARCHAR(150), correo VARCHAR(100));
INSERT INTO seed_golden VALUES
(1,1,6,'2025000001','Ana Martínez López','ana.martinez@alumno.mx'),
(2,1,4,'2025000002','Luis Hernández García','luis.hernandez@alumno.mx'),
(3,1,3,'2025000003','Sofía Ramírez Torres','sofia.ramirez@alumno.mx'),
(4,1,3,'2025000004','Diego Sánchez Ruiz','diego.sanchez@alumno.mx'),
(5,1,5,'2024000005','Valeria Cruz Mendoza','valeria.cruz@alumno.mx'),
(6,1,3,'2025000006','Mateo Flores Jiménez','mateo.flores@alumno.mx'),
(7,2,1,'2025000007','Camila Ortega Reyes','camila.ortega@alumno.mx'),
(8,2,6,'2025000008','Emiliano Vargas Soto','emiliano.vargas@alumno.mx'),
(9,2,4,'2025000009','Regina Navarro Díaz','regina.navarro@alumno.mx'),
(10,2,2,'2025000010','Daniel Morales Vega','daniel.morales@alumno.mx');
CREATE TEMPORARY TABLE seed_profiles AS
SELECT n.n+1 AS id,
 CASE WHEN n.n<10 THEN 250+n.n ELSE FLOOR((n.n-10)/40) END AS seccion,
 COALESCE(g.programa,MOD(FLOOR((n.n-10)/40),3)+1) AS programa,
 COALESCE(g.etapa,MOD(FLOOR(FLOOR((n.n-10)/40)/3),6)+1) AS etapa
FROM seed_numbers n LEFT JOIN seed_golden g ON g.id=n.n+1;
CREATE TEMPORARY TABLE seed_periods AS
SELECT n AS t, CONCAT(2024+FLOOR(n/2),'-',MOD(n,2)+1) AS periodo,
 CONCAT(2024+FLOOR(n/2),IF(MOD(n,2)=0,'-01-15','-08-15')) AS inicio
FROM seed_numbers WHERE n<6;
-- Retain known subject IDs. Fill the other subjects programmatically: 3 x 9 x 6 = 162.
CREATE TEMPORARY TABLE seed_legacy (id INT, programa INT, semestre INT, slot INT, nombre VARCHAR(150), creditos INT);
INSERT INTO seed_legacy VALUES
(1,1,1,1,'Programación I',8),(2,1,1,2,'Matemáticas Discretas',8),
(3,1,2,1,'Programación Orientada a Objetos',8),(4,1,2,2,'Bases de Datos',8),
(5,1,2,3,'Análisis y Diseño de Algoritmos',8),(6,1,2,4,'Ecuaciones Diferenciales',8),
(7,1,2,5,'Administración Financiera',6),(8,1,2,6,'Diseño Digital',8),
(9,1,3,1,'Sistemas Operativos',8),(10,1,3,2,'Probabilidad',8),
(11,1,4,1,'Machine Learning',8),(12,1,4,2,'Inteligencia Artificial',8),
(13,2,1,1,'Fundamentos de Inteligencia Artificial',8),(14,2,1,2,'Programación para Inteligencia Artificial',8),
(15,2,2,1,'Aprendizaje Automático',8),(16,2,2,2,'Estadística para Inteligencia Artificial',8),
(17,2,3,1,'Procesamiento de Lenguaje Natural',8),(18,2,3,2,'Visión Artificial',8);
CREATE TEMPORARY TABLE seed_catalog AS
SELECT n, FLOOR(n/54)+1 AS programa, MOD(FLOOR(n/6),9)+1 AS semestre, MOD(n,6)+1 AS slot
FROM seed_numbers WHERE n<162;
-- Separate lookup avoids MySQL's 'Can't reopen table' restriction on temporary tables.
CREATE TEMPORARY TABLE seed_legacy_count AS
SELECT c.n, COUNT(l.id) AS cantidad FROM seed_catalog c LEFT JOIN seed_legacy l
ON (l.programa-1)*54+(l.semestre-1)*6+l.slot-1<=c.n GROUP BY c.n;
START TRANSACTION;
INSERT INTO programas_academicos (id,nombre,clave,creditos_totales) VALUES
(1,'Ingeniería en Sistemas Computacionales','ISC',430),
(2,'Ingeniería en Inteligencia Artificial','IIA',432),
(3,'Licenciatura en Ciencia de Datos','LCD',432);
INSERT INTO estudiantes (id,programa_academico_id,boleta,nombre,correo,telefono)
SELECT p.id,p.programa,COALESCE(g.boleta,CONCAT(2024+FLOOR((6-p.etapa)/2),LPAD(p.id,6,'0'))),
 COALESCE(g.nombre,CONCAT(ELT(MOD(p.id,8)+1,'Andrea','Jorge','Lucía','Pablo','Elena','Raúl','María','Iván'),' ',
 ELT(MOD(FLOOR(p.id/8),8)+1,'García','López','Pérez','Torres','Ruiz','Díaz','Flores','Cruz'),' ',LPAD(p.id,5,'0'))),
 COALESCE(g.correo,CONCAT('alumno',p.id,'@alumno.mx')),CONCAT('551',LPAD(p.id,7,'0'))
FROM seed_profiles p LEFT JOIN seed_golden g ON g.id=p.id;
INSERT INTO profesores (id,numero_empleado,nombre,correo,telefono)
SELECT n+1,CONCAT('EMP',LPAD(n+1,3,'0')),
 CASE WHEN n<10 THEN ELT(n+1,'Carlos Ramírez','Laura Hernández','Fernando López','Mariana Torres','Jorge Mendoza',
 'Patricia Sánchez','Ricardo Gómez','Alejandra Rivera','Miguel Castro','Gabriela Ortiz')
 ELSE CONCAT('Docente ',LPAD(n+1,3,'0')) END,
 CONCAT('profesor',n+1,'@escuela.mx'),CONCAT('552',LPAD(n+1,7,'0')) FROM seed_numbers WHERE n<180;
INSERT INTO salones (id,numero,capacidad,tipo)
SELECT n+1,CONCAT('S',LPAD(n+1,3,'0')),40,IF(MOD(n,5)=0,'LABORATORIO','AULA') FROM seed_numbers WHERE n<70;
INSERT INTO materias (id,programa_academico_id,clave,nombre,creditos,semestre)
SELECT COALESCE(l.id,19+c.n-k.cantidad),c.programa,
 CONCAT(ELT(c.programa,'ISC','IIA','LCD'),c.semestre,LPAD(c.slot,2,'0')),
 COALESCE(l.nombre,CONCAT(ELT(c.programa,'Sistemas','Inteligencia Artificial','Ciencia de Datos'),': ',
 ELT(c.slot,'Fundamentos','Métodos cuantitativos','Modelado','Tecnologías','Aplicaciones','Proyecto integrador'),' ',c.semestre)),
 COALESCE(l.creditos,8),c.semestre
FROM seed_catalog c JOIN seed_legacy_count k ON k.n=c.n
LEFT JOIN seed_legacy l ON l.programa=c.programa AND l.semestre=c.semestre AND l.slot=c.slot;
CREATE TEMPORARY TABLE seed_plans AS
SELECT p.id AS estudiante_id,p.seccion,p.programa,r.t,r.periodo,r.inicio,
 CASE WHEN p.id=3 AND r.t=5 THEN 3 WHEN p.id=3 THEN r.t-1 ELSE p.etapa+r.t-5 END AS semestre,
 CASE WHEN p.id=3 AND r.t=2 THEN 1 WHEN p.id=3 AND r.t=3 THEN 3
 WHEN p.id=3 AND r.t=5 THEN 7 ELSE 100+r.t*260+p.seccion END AS grupo_id,
 CASE WHEN p.id=3 AND r.t=2 THEN 2 WHEN p.id=3 AND r.t=3 THEN 3 ELSE 4+MOD(p.id,3) END AS carga,
 CASE WHEN (MOD(p.id,17)=0 OR p.id=6) AND p.etapa>=2 THEN 1 ELSE 0 END AS recursa,
 CASE WHEN (MOD(p.id,29)=0 OR p.id=8) AND p.etapa>=3 AND MOD(p.id,17)<>0 AND p.id<>6 THEN 1 ELSE 0 END AS recursa_historico
FROM seed_profiles p CROSS JOIN seed_periods r
WHERE (p.id<>3 AND r.t>=6-p.etapa) OR (p.id=3 AND r.t IN (2,3,5));
CREATE TEMPORARY TABLE seed_sections AS
SELECT DISTINCT seccion,programa,t,periodo,semestre,grupo_id FROM seed_plans;
INSERT INTO grupos (id,clave_grupo,periodo,semestre)
SELECT grupo_id,CASE WHEN seccion=252 THEN ELT(semestre,'1CM1','2CV1','3CM1')
 ELSE CONCAT(ELT(programa,'ISC','IIA','LCD'),'-',semestre,'-',LPAD(seccion,3,'0')) END,periodo,semestre FROM seed_sections;
CREATE TEMPORARY TABLE seed_offers AS
SELECT s.*,n.n+1 AS slot,
 CASE WHEN s.seccion=252 AND s.t=2 AND n.n<2 THEN n.n+1
 WHEN s.seccion=252 AND s.t=3 AND n.n<3 THEN 9+n.n
 WHEN s.seccion=252 AND s.t=5 AND n.n<2 THEN 25+n.n
 ELSE 1000+(s.t*260+s.seccion)*7+n.n END AS gm,
 CASE WHEN s.seccion=252 AND s.t=2 AND n.n<2 THEN n.n+1
 WHEN s.seccion=252 AND s.t=3 AND n.n<3 THEN ELT(n.n+1,9,1,5)
 ELSE MOD(MOD(s.seccion,70)+n.n*30,180)+1 END AS profesor
FROM seed_sections s CROSS JOIN seed_numbers n
WHERE n.n<6 OR (n.n=6 AND s.t>=4 AND s.semestre>1);
-- Preserve Sofia's historical teachers by swapping their concurrent assignments.
UPDATE seed_offers SET profesor=CASE
 WHEN t=2 AND slot=1 AND profesor=1 THEN 43
 WHEN t=2 AND slot=2 AND profesor=2 THEN 73
 WHEN t=3 AND slot=1 AND profesor=9 THEN 43
 WHEN t=3 AND slot=2 AND profesor=1 THEN 73
 WHEN t=3 AND slot=3 AND profesor=5 THEN 103
 ELSE profesor END
WHERE seccion<>252 AND FLOOR(seccion/70)=3;
INSERT INTO grupos_materia (id,grupo_id,materia_id,profesor_id,cupo)
SELECT o.gm,o.grupo_id,m.id,o.profesor,40 FROM seed_offers o JOIN materias m
ON m.programa_academico_id=o.programa
AND m.clave=CONCAT(ELT(o.programa,'ISC','IIA','LCD'),IF(o.slot=7,o.semestre-1,o.semestre),LPAD(IF(o.slot=7,1,o.slot),2,'0'));
-- 2 x 90 minutes for slots 1..5, 1 x 90 for slots 6..7; 07:00..22:00, weekdays.
-- Section wave + slot fixes room/time; professors rotate across slots, not concurrent rooms.
CREATE TEMPORARY TABLE seed_schedule AS
SELECT o.gm,MOD(o.seccion,70)+1 AS salon,
 FLOOR(o.seccion/70)*12+IF(o.slot<=5,(o.slot-1)*2, o.slot+4)+b.n AS bloque
FROM seed_offers o JOIN seed_digits b ON b.n<IF(o.slot<=5,2,1);
INSERT INTO horarios (id,grupo_materia_id,salon_id,dia,hora_inicio,hora_fin)
SELECT gm*2+MOD(bloque,2),gm,salon,
 ELT(FLOOR(bloque/10)+1,'LUNES','MARTES','MIERCOLES','JUEVES','VIERNES'),
 SEC_TO_TIME(25200+MOD(bloque,10)*5400),SEC_TO_TIME(30600+MOD(bloque,10)*5400)
FROM seed_schedule;
INSERT INTO inscripciones (id,estudiante_id,grupo_materia_id,fecha_inscripcion,estado)
SELECT ((p.estudiante_id-1)*6+p.t)*6+n.n+1,p.estudiante_id,o.gm,p.inicio,IF(p.t=5,'ACTIVA','INACTIVA')
FROM seed_plans p JOIN seed_digits n ON n.n<p.carga
JOIN seed_offers o ON o.grupo_id=p.grupo_id
AND o.slot=IF(((p.t=5 AND p.recursa=1) OR (p.t=4 AND p.recursa_historico=1)) AND n.n=0,7,n.n+1);
COMMIT;
DROP TEMPORARY TABLE seed_schedule,seed_offers,seed_sections,seed_plans,seed_catalog,seed_legacy_count,seed_legacy,seed_profiles,seed_periods,seed_golden,seed_numbers,seed_digits;
