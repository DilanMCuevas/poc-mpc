CREATE DATABASE IF NOT EXISTS escuela;

USE escuela;


-- PROGRAMAS ACADÉMICOS

CREATE TABLE programas_academicos (
    id INT NOT NULL AUTO_INCREMENT,
    nombre VARCHAR(150) NOT NULL,
    clave VARCHAR(20) NOT NULL,
    creditos_totales INT NOT NULL,

    PRIMARY KEY (id),
    CONSTRAINT uq_programas_clave
        UNIQUE (clave)
);


-- ESTUDIANTES

CREATE TABLE estudiantes (
    id INT NOT NULL AUTO_INCREMENT,
    programa_academico_id INT NOT NULL,
    boleta VARCHAR(20) NOT NULL,
    nombre VARCHAR(150) NOT NULL,
    correo VARCHAR(100) NOT NULL,
    telefono VARCHAR(20) NOT NULL,

    PRIMARY KEY (id),
    UNIQUE (boleta),
    UNIQUE (telefono),

    CONSTRAINT fk_estudiantes_programas
        FOREIGN KEY (programa_academico_id)
        REFERENCES programas_academicos(id)
        ON DELETE RESTRICT
        ON UPDATE CASCADE
);


-- MATERIAS

CREATE TABLE materias (
    id INT NOT NULL AUTO_INCREMENT,
    programa_academico_id INT NOT NULL,
    clave VARCHAR(50) NOT NULL,
    nombre VARCHAR(150) NOT NULL,
    creditos INT NOT NULL,
    semestre INT NOT NULL,

    PRIMARY KEY (id),
    UNIQUE (clave),

    CONSTRAINT fk_materias_programas
        FOREIGN KEY (programa_academico_id)
        REFERENCES programas_academicos(id)
        ON DELETE RESTRICT
        ON UPDATE CASCADE
);


-- PROFESORES

CREATE TABLE profesores (
    id INT NOT NULL AUTO_INCREMENT,
    numero_empleado VARCHAR(50) NOT NULL,
    nombre VARCHAR(50) NOT NULL,
    correo VARCHAR(50) NOT NULL,
    telefono VARCHAR(20) NOT NULL,

    PRIMARY KEY (id),
    UNIQUE (numero_empleado),
    UNIQUE (correo),
    UNIQUE (telefono)
);


-- SALONES

CREATE TABLE salones (
    id INT NOT NULL AUTO_INCREMENT,
    numero VARCHAR(50) NOT NULL,
    capacidad INT NOT NULL,
    tipo VARCHAR(50) NOT NULL,

    PRIMARY KEY (id),
    UNIQUE (numero)
);


-- GRUPOS

CREATE TABLE grupos (
    id INT NOT NULL AUTO_INCREMENT,
    clave_grupo VARCHAR(20) NOT NULL,
    periodo VARCHAR(20) NOT NULL,
    semestre INT NOT NULL,

    PRIMARY KEY (id),

    CONSTRAINT uq_grupos_clave_periodo
        UNIQUE (clave_grupo, periodo)
);


-- MATERIAS OFERTADAS POR GRUPO

CREATE TABLE grupos_materia (
    id INT NOT NULL AUTO_INCREMENT,
    grupo_id INT NOT NULL,
    materia_id INT NOT NULL,
    profesor_id INT NOT NULL,
    cupo INT NOT NULL,

    PRIMARY KEY (id),

    CONSTRAINT uq_grupo_materia
        UNIQUE (grupo_id, materia_id),

    CONSTRAINT fk_grupos_materias_grupos
        FOREIGN KEY (grupo_id)
        REFERENCES grupos(id)
        ON DELETE RESTRICT
        ON UPDATE CASCADE,

    CONSTRAINT fk_grupo_materias_materias
        FOREIGN KEY (materia_id)
        REFERENCES materias(id)
        ON DELETE RESTRICT
        ON UPDATE CASCADE,

    CONSTRAINT fk_grupo_materias_profesores
        FOREIGN KEY (profesor_id)
        REFERENCES profesores(id)
        ON DELETE RESTRICT
        ON UPDATE CASCADE
);


-- HORARIOS

CREATE TABLE horarios (
    id INT NOT NULL AUTO_INCREMENT,
    grupo_materia_id INT NOT NULL,
    salon_id INT NOT NULL,
    dia ENUM(
        'LUNES',
        'MARTES',
        'MIERCOLES',
        'JUEVES',
        'VIERNES'
    ) NOT NULL,
    hora_inicio TIME NOT NULL,
    hora_fin TIME NOT NULL,

    PRIMARY KEY (id),

    CONSTRAINT fk_horario_grupo_materia
        FOREIGN KEY (grupo_materia_id)
        REFERENCES grupos_materia(id)
        ON DELETE RESTRICT
        ON UPDATE CASCADE,

    CONSTRAINT fk_horarios_salones
        FOREIGN KEY (salon_id)
        REFERENCES salones(id)
        ON DELETE RESTRICT
        ON UPDATE CASCADE,

    CONSTRAINT ck_hora_valida
        CHECK (
            TIMESTAMPDIFF(MINUTE, hora_inicio, hora_fin) >= 90
        )
);


-- INSCRIPCIONES

CREATE TABLE inscripciones (
    id INT NOT NULL AUTO_INCREMENT,
    estudiante_id INT NOT NULL,
    grupo_materia_id INT NOT NULL,
    fecha_inscripcion DATE NOT NULL,
    estado ENUM(
        'ACTIVA',
        'INACTIVA'
    ) NOT NULL DEFAULT 'ACTIVA',

    PRIMARY KEY (id),

    CONSTRAINT uq_inscripciones_estudiante_materia
        UNIQUE (estudiante_id, grupo_materia_id),

    CONSTRAINT fk_inscripciones_estudiantes
        FOREIGN KEY (estudiante_id)
        REFERENCES estudiantes(id)
        ON DELETE RESTRICT
        ON UPDATE CASCADE,

    CONSTRAINT fk_inscripciones_grupo_materias
        FOREIGN KEY (grupo_materia_id)
        REFERENCES grupos_materia(id)
        ON DELETE RESTRICT
        ON UPDATE CASCADE
);