//CONFIGURACIÓN

const databaseName = process.env.MONGO_INITDB_DATABASE;
const appUser = process.env.MONGO_USER;
const appPassword = process.env.MONGO_PASSWORD;

if (!databaseName) {
    throw new Error("MONGO_INITDB_DATABASE no está definido");
}

if (!appUser) {
    throw new Error("MONGO_USER no está definido");
}

if (!appPassword) {
    throw new Error("MONGO_PASSWORD no está definido");
}

// Selecciona la base configurada desde .env
db = db.getSiblingDB(databaseName);

// USUARIO DE APLICACIÓN
//El usuario administrativo de MongoDB es creado por la imagen
// oficial utilizando:
// MONGO_INITDB_ROOT_USERNAME
// MONGO_INITDB_ROOT_PASSWORD
// Este usuario es independiente del utilizado por MCP.
// MCP utilizará exclusivamente el usuario definido mediante:
// MONGO_USER
// MONGO_PASSWORD
// y tendrá permisos readWrite únicamente sobre esta base.

if (!db.getUser(appUser)) {
    db.createUser({
        user: appUser,
        pwd: appPassword,
        roles: [
            {
                role: "readWrite",
                db: databaseName
            }
        ]
    });
}

// HISTORIAL ACADÉMICO
// Un documento representa el historial académico completo
// de un estudiante.
// MySQL conserva:
// - Estudiantes
// - Materias
// - Profesores
// - Grupos
// - Oferta de materias
// MongoDB conserva los resultados históricos.
// Los IDs permiten realizar consultas híbridas y recuperar
// posteriormente la información descriptiva desde MySQL.


db.createCollection("historial_academico", {
    validator: {
        $jsonSchema: {
            bsonType: "object",

            required: [
                "estudiante_id",
                "periodos",
                "fecha_creacion",
                "fecha_actualizacion"
            ],

            properties: {

                estudiante_id: {
                    bsonType: "int",
                    minimum: 1,
                    description: "ID del estudiante registrado en MySQL"
                },

                periodos: {
                    bsonType: "array",
                    minItems: 1,
                    description: "Periodos académicos cursados por el estudiante",

                    items: {
                        bsonType: "object",

                        required: [
                            "periodo",
                            "semestre",
                            "materias"
                        ],

                        properties: {

                            periodo: {
                                bsonType: "string",
                                pattern: "^[0-9]{4}-[12]$",
                                description: "Periodo académico con formato YYYY-1 o YYYY-2"
                            },

                            semestre: {
                                bsonType: "int",
                                minimum: 1,
                                description: "Semestre cursado durante el periodo"
                            },

                            materias: {
                                bsonType: "array",
                                minItems: 1,
                                description: "Materias cursadas durante el periodo",

                                items: {
                                    bsonType: "object",

                                    required: [
                                        "grupo_materia_id",
                                        "materia_id",
                                        "grupo_id",
                                        "profesor_id",
                                        "estado",
                                        "tipo_curso",
                                        "intento"
                                    ],

                                    properties: {

                                        grupo_materia_id: {
                                            bsonType: "int",
                                            minimum: 1,
                                            description: "ID de grupos_materia en MySQL"
                                        },

                                        materia_id: {
                                            bsonType: "int",
                                            minimum: 1,
                                            description: "ID de la materia en MySQL"
                                        },

                                        grupo_id: {
                                            bsonType: "int",
                                            minimum: 1,
                                            description: "ID del grupo en MySQL"
                                        },

                                        profesor_id: {
                                            bsonType: "int",
                                            minimum: 1,
                                            description: "ID del profesor que impartió la materia"
                                        },

                                        calificacion: {
                                            bsonType: ["double", "int"],
                                            minimum: 0,
                                            maximum: 10,
                                            description: "Calificación final cuando exista"
                                        },

                                        estado: {
                                            enum: [
                                                "APROBADA",
                                                "REPROBADA",
                                                "BAJA",
                                                "NO_PRESENTADA"
                                            ],
                                            description: "Resultado final de la materia"
                                        },

                                        tipo_curso: {
                                            enum: [
                                                "ORDINARIO",
                                                "RECURSAMIENTO"
                                            ],
                                            description: "Modalidad en la que se cursó la materia"
                                        },

                                        intento: {
                                            bsonType: "int",
                                            minimum: 1,
                                            description: "Número de intento en que se cursó la materia"
                                        },

                                        fecha_cierre: {
                                            bsonType: "date",
                                            description: "Fecha de cierre de la materia cuando corresponda"
                                        },

                                        observaciones: {
                                            bsonType: "string",
                                            description: "Observaciones relacionadas con el resultado"
                                        }
                                    }
                                }
                            }
                        }
                    }
                },

                fecha_creacion: {
                    bsonType: "date",
                    description: "Fecha de creación del historial"
                },

                fecha_actualizacion: {
                    bsonType: "date",
                    description: "Fecha de la última actualización"
                }
            }
        }
    },

    validationLevel: "strict",
    validationAction: "error"
});

// ndices: historial académico

// Un estudiante solo puede tener un documento de historial.
db.historial_academico.createIndex(
    {
        estudiante_id: 1
    },
    {
        unique: true,
        name: "uq_historial_estudiante"
    }
);


// Consulta por materia cursada.
db.historial_academico.createIndex(
    {
        "periodos.materias.materia_id": 1
    },
    {
        name: "idx_historial_materia"
    }
);


// Consulta por oferta concreta de grupo-materia.
db.historial_academico.createIndex(
    {
        "periodos.materias.grupo_materia_id": 1
    },
    {
        name: "idx_historial_grupo_materia"
    }
);


// Consulta por estado académico.
db.historial_academico.createIndex(
    {
        "periodos.materias.estado": 1
    },
    {
        name: "idx_historial_estado"
    }
);


// Consulta por profesor histórico.
db.historial_academico.createIndex(
    {
        "periodos.materias.profesor_id": 1
    },
    {
        name: "idx_historial_profesor"
    }
);

// TRAYECTORIA ACADÉMICA
// Registra acontecimientos relevantes de la vida académica.
// A diferencia de historial_academico, esta colección describe
// QUÉ SUCEDIÓ y CUÁNDO sucedió.
// Los eventos pueden tener estructuras diferentes.
// Ejemplos:
// - inscripción
// - baja
// - cambio de grupo
// - reprobación
// - aprobación
// - recursamiento
// - acreditación
// Los campos que dependen del evento son opcionales.

db.createCollection("trayectoria_academica", {
    validator: {
        $jsonSchema: {
            bsonType: "object",

            required: [
                "estudiante_id",
                "eventos",
                "fecha_creacion",
                "fecha_actualizacion"
            ],

            properties: {

                estudiante_id: {
                    bsonType: "int",
                    minimum: 1,
                    description: "ID del estudiante registrado en MySQL"
                },

                eventos: {
                    bsonType: "array",
                    minItems: 1,
                    description: "Eventos registrados durante la trayectoria académica",

                    items: {
                        bsonType: "object",

                        required: [
                            "tipo",
                            "fecha"
                        ],

                        properties: {

                            tipo: {
                                enum: [
                                    "INSCRIPCION",
                                    "BAJA_MATERIA",
                                    "CAMBIO_GRUPO",
                                    "REPROBACION",
                                    "APROBACION",
                                    "RECURSAMIENTO",
                                    "ACREDITACION"
                                ],
                                description: "Tipo de evento académico"
                            },

                            fecha: {
                                bsonType: "date",
                                description: "Fecha del evento"
                            },

                            periodo: {
                                bsonType: "string",
                                pattern: "^[0-9]{4}-[12]$",
                                description: "Periodo académico relacionado"
                            },

                            grupo_materia_id: {
                                bsonType: "int",
                                minimum: 1,
                                description: "ID de grupos_materia relacionado con el evento"
                            },

                            materia_id: {
                                bsonType: "int",
                                minimum: 1,
                                description: "ID de la materia relacionada"
                            },

                            grupo_id: {
                                bsonType: "int",
                                minimum: 1,
                                description: "ID del grupo relacionado"
                            },

                            profesor_id: {
                                bsonType: "int",
                                minimum: 1,
                                description: "ID del profesor relacionado"
                            },

                            calificacion: {
                                bsonType: ["double", "int"],
                                minimum: 0,
                                maximum: 10,
                                description: "Calificación asociada al evento cuando corresponda"
                            },

                            motivo: {
                                bsonType: "string",
                                description: "Motivo del evento"
                            },

                            observaciones: {
                                bsonType: "string",
                                description: "Observaciones adicionales"
                            },

                            detalles: {
                                bsonType: "object",
                                description: "Información específica y variable del evento"
                            }
                        }
                    }
                },

                fecha_creacion: {
                    bsonType: "date",
                    description: "Fecha de creación de la trayectoria"
                },

                fecha_actualizacion: {
                    bsonType: "date",
                    description: "Fecha de la última actualización"
                }
            }
        }
    },

    validationLevel: "strict",
    validationAction: "error"
});


// Índices: trayectoria académica

db.trayectoria_academica.createIndex(
    {
        estudiante_id: 1
    },
    {
        unique: true,
        name: "uq_trayectoria_estudiante"
    }
);


db.trayectoria_academica.createIndex(
    {
        "eventos.tipo": 1
    },
    {
        name: "idx_trayectoria_tipo_evento"
    }
);


db.trayectoria_academica.createIndex(
    {
        "eventos.materia_id": 1
    },
    {
        name: "idx_trayectoria_materia"
    }
);


db.trayectoria_academica.createIndex(
    {
        "eventos.grupo_materia_id": 1
    },
    {
        name: "idx_trayectoria_grupo_materia"
    }
);


db.trayectoria_academica.createIndex(
    {
        "eventos.fecha": 1
    },
    {
        name: "idx_trayectoria_fecha"
    }
);

// EVALUACIONES
// Un documento representa:
//estudiante + materia + periodo + intento
// Esta colección almacena cómo se obtuvo un determinado
// resultado académico.
// Las evaluaciones pueden variar dependiendo de la naturaleza
// de cada materia.
// La propiedad "detalles" permite almacenar información
// específica sin imponer una estructura única para todas las
// evaluaciones.

db.createCollection("evaluaciones", {
    validator: {
        $jsonSchema: {
            bsonType: "object",

            required: [
                "estudiante_id",
                "grupo_materia_id",
                "materia_id",
                "grupo_id",
                "profesor_id",
                "periodo",
                "intento",
                "evaluaciones",
                "fecha_creacion",
                "fecha_actualizacion"
            ],

            properties: {

                estudiante_id: {
                    bsonType: "int",
                    minimum: 1,
                    description: "ID del estudiante en MySQL"
                },

                grupo_materia_id: {
                    bsonType: "int",
                    minimum: 1,
                    description: "ID de grupos_materia en MySQL"
                },

                materia_id: {
                    bsonType: "int",
                    minimum: 1,
                    description: "ID de la materia en MySQL"
                },

                grupo_id: {
                    bsonType: "int",
                    minimum: 1,
                    description: "ID del grupo en MySQL"
                },

                profesor_id: {
                    bsonType: "int",
                    minimum: 1,
                    description: "ID del profesor en MySQL"
                },

                periodo: {
                    bsonType: "string",
                    pattern: "^[0-9]{4}-[12]$",
                    description: "Periodo académico con formato YYYY-1 o YYYY-2"
                },

                intento: {
                    bsonType: "int",
                    minimum: 1,
                    description: "Número de ocasión en que se cursa la materia"
                },

                evaluaciones: {
                    bsonType: "array",
                    minItems: 1,
                    description: "Evaluaciones realizadas en la materia",

                    items: {
                        bsonType: "object",

                        required: [
                            "tipo",
                            "nombre",
                            "calificacion",
                            "fecha"
                        ],

                        properties: {

                            tipo: {
                                enum: [
                                    "PARCIAL",
                                    "PROYECTO",
                                    "PRACTICA",
                                    "LABORATORIO",
                                    "EXPOSICION",
                                    "TAREA",
                                    "EXAMEN_FINAL",
                                    "OTRO"
                                ],
                                description: "Tipo de evaluación"
                            },

                            nombre: {
                                bsonType: "string",
                                description: "Nombre o descripción de la evaluación"
                            },

                            calificacion: {
                                bsonType: ["double", "int"],
                                minimum: 0,
                                maximum: 10,
                                description: "Resultado obtenido"
                            },

                            ponderacion: {
                                bsonType: ["double", "int"],
                                minimum: 0,
                                maximum: 100,
                                description: "Porcentaje de la evaluación dentro de la materia"
                            },

                            fecha: {
                                bsonType: "date",
                                description: "Fecha de evaluación"
                            },

                            observaciones: {
                                bsonType: "string",
                                description: "Observaciones sobre el resultado"
                            },

                            detalles: {
                                bsonType: "object",
                                description: "Información específica de la evaluación"
                            },

                            entregables: {
                                bsonType: "array",
                                description: "Entregables relacionados con la evaluación",

                                items: {
                                    bsonType: "object",

                                    required: [
                                        "nombre",
                                        "tipo"
                                    ],

                                    properties: {

                                        nombre: {
                                            bsonType: "string",
                                            description: "Nombre del entregable"
                                        },

                                        tipo: {
                                            bsonType: "string",
                                            description: "Tipo de entregable"
                                        },

                                        referencia: {
                                            bsonType: "string",
                                            description: "Referencia lógica al recurso entregado"
                                        }
                                    }
                                }
                            }
                        }
                    }
                },

                calificacion_final: {
                    bsonType: ["double", "int"],
                    minimum: 0,
                    maximum: 10,
                    description: "Calificación final obtenida en la materia"
                },

                fecha_creacion: {
                    bsonType: "date",
                    description: "Fecha de creación del documento"
                },

                fecha_actualizacion: {
                    bsonType: "date",
                    description: "Fecha de la última actualización"
                }
            }
        }
    },

    validationLevel: "strict",
    validationAction: "error"
});

// Índices: evaluaciones
// Un estudiante puede cursar la misma materia múltiples veces.
// El intento permite distinguir cada ocasión.

db.evaluaciones.createIndex(
    {
        estudiante_id: 1,
        materia_id: 1,
        periodo: 1,
        intento: 1
    },
    {
        unique: true,
        name: "uq_evaluacion_estudiante_materia_periodo_intento"
    }
);


db.evaluaciones.createIndex(
    {
        grupo_materia_id: 1
    },
    {
        name: "idx_evaluaciones_grupo_materia"
    }
);


db.evaluaciones.createIndex(
    {
        materia_id: 1
    },
    {
        name: "idx_evaluaciones_materia"
    }
);


db.evaluaciones.createIndex(
    {
        profesor_id: 1
    },
    {
        name: "idx_evaluaciones_profesor"
    }
);


db.evaluaciones.createIndex(
    {
        "evaluaciones.tipo": 1
    },
    {
        name: "idx_evaluaciones_tipo"
    }
);


db.evaluaciones.createIndex(
    {
        "evaluaciones.calificacion": 1
    },
    {
        name: "idx_evaluaciones_calificacion"
    }
);
print(`MongoDB inicializado correctamente.`);
print(`Base de datos: ${databaseName}`);
print(`Usuario de aplicación: ${appUser}`);
print("Colecciones creadas:");
print("- historial_academico");
print("- trayectoria_academica");
print("- evaluaciones");