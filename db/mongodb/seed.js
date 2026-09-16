// ============================================================
// CONFIGURACIÓN
// ============================================================

const databaseName = process.env.MONGO_INITDB_DATABASE;

if (!databaseName) {
    throw new Error("MONGO_INITDB_DATABASE no está definido");
}

db = db.getSiblingDB(databaseName);


// ============================================================
// HISTORIAL ACADÉMICO
// ============================================================

db.historial_academico.insertMany([

    // --------------------------------------------------------
    // 1. ANA MARTÍNEZ
    // Caso:
    // - Buen primer semestre.
    // - Reprobó Bases de Datos en 2025-2.
    // - Actualmente la recursa con otro profesor.
    // --------------------------------------------------------

    {
        estudiante_id: NumberInt(1),

        periodos: [
            {
                periodo: "2025-1",
                semestre: NumberInt(1),

                materias: [
                    {
                        grupo_materia_id: NumberInt(1),
                        materia_id: NumberInt(1),
                        grupo_id: NumberInt(1),
                        profesor_id: NumberInt(1),

                        calificacion: 8.5,
                        estado: "APROBADA",
                        tipo_curso: "ORDINARIO",
                        intento: NumberInt(1),

                        fecha_cierre: ISODate("2025-06-20T00:00:00Z")
                    },

                    {
                        grupo_materia_id: NumberInt(2),
                        materia_id: NumberInt(2),
                        grupo_id: NumberInt(1),
                        profesor_id: NumberInt(2),

                        calificacion: 8.0,
                        estado: "APROBADA",
                        tipo_curso: "ORDINARIO",
                        intento: NumberInt(1),

                        fecha_cierre: ISODate("2025-06-20T00:00:00Z")
                    }
                ]
            },

            {
                periodo: "2025-2",
                semestre: NumberInt(2),

                materias: [
                    {
                        grupo_materia_id: NumberInt(3),
                        materia_id: NumberInt(3),
                        grupo_id: NumberInt(2),
                        profesor_id: NumberInt(3),

                        calificacion: 8.2,
                        estado: "APROBADA",
                        tipo_curso: "ORDINARIO",
                        intento: NumberInt(1),

                        fecha_cierre: ISODate("2025-12-15T00:00:00Z")
                    },

                    {
                        grupo_materia_id: NumberInt(4),
                        materia_id: NumberInt(4),
                        grupo_id: NumberInt(2),
                        profesor_id: NumberInt(4),

                        calificacion: 5.0,
                        estado: "REPROBADA",
                        tipo_curso: "ORDINARIO",
                        intento: NumberInt(1),

                        fecha_cierre: ISODate("2025-12-15T00:00:00Z"),

                        observaciones:
                            "No alcanzó la calificación mínima aprobatoria."
                    }
                ]
            }
        ],

        fecha_creacion: ISODate("2025-06-20T00:00:00Z"),
        fecha_actualizacion: ISODate("2025-12-15T00:00:00Z")
    },


    // --------------------------------------------------------
    // 2. LUIS HERNÁNDEZ
    // Caso:
    // - Aprobó primer semestre.
    // - Dio de baja POO.
    // - Actualmente vuelve a cursarla.
    // --------------------------------------------------------

    {
        estudiante_id: NumberInt(2),

        periodos: [
            {
                periodo: "2025-1",
                semestre: NumberInt(1),

                materias: [
                    {
                        grupo_materia_id: NumberInt(1),
                        materia_id: NumberInt(1),
                        grupo_id: NumberInt(1),
                        profesor_id: NumberInt(1),

                        calificacion: 7.8,
                        estado: "APROBADA",
                        tipo_curso: "ORDINARIO",
                        intento: NumberInt(1),

                        fecha_cierre: ISODate("2025-06-20T00:00:00Z")
                    },

                    {
                        grupo_materia_id: NumberInt(2),
                        materia_id: NumberInt(2),
                        grupo_id: NumberInt(1),
                        profesor_id: NumberInt(2),

                        calificacion: 7.2,
                        estado: "APROBADA",
                        tipo_curso: "ORDINARIO",
                        intento: NumberInt(1),

                        fecha_cierre: ISODate("2025-06-20T00:00:00Z")
                    }
                ]
            },

            {
                periodo: "2025-2",
                semestre: NumberInt(2),

                materias: [
                    {
                        grupo_materia_id: NumberInt(3),
                        materia_id: NumberInt(3),
                        grupo_id: NumberInt(2),
                        profesor_id: NumberInt(3),

                        estado: "BAJA",
                        tipo_curso: "ORDINARIO",
                        intento: NumberInt(1),

                        observaciones:
                            "El estudiante solicitó la baja durante el periodo."
                    },

                    {
                        grupo_materia_id: NumberInt(5),
                        materia_id: NumberInt(5),
                        grupo_id: NumberInt(2),
                        profesor_id: NumberInt(5),

                        calificacion: 7.5,
                        estado: "APROBADA",
                        tipo_curso: "ORDINARIO",
                        intento: NumberInt(1),

                        fecha_cierre: ISODate("2025-12-15T00:00:00Z")
                    }
                ]
            }
        ],

        fecha_creacion: ISODate("2025-06-20T00:00:00Z"),
        fecha_actualizacion: ISODate("2025-12-15T00:00:00Z")
    },


    // --------------------------------------------------------
    // 3. SOFÍA RAMÍREZ
    // Caso:
    // - Alumna de rendimiento alto.
    // - Sin bajas ni reprobaciones.
    // --------------------------------------------------------

    {
        estudiante_id: NumberInt(3),

        periodos: [
            {
                periodo: "2025-1",
                semestre: NumberInt(1),

                materias: [
                    {
                        grupo_materia_id: NumberInt(1),
                        materia_id: NumberInt(1),
                        grupo_id: NumberInt(1),
                        profesor_id: NumberInt(1),

                        calificacion: 9.6,
                        estado: "APROBADA",
                        tipo_curso: "ORDINARIO",
                        intento: NumberInt(1),

                        fecha_cierre: ISODate("2025-06-20T00:00:00Z")
                    },

                    {
                        grupo_materia_id: NumberInt(2),
                        materia_id: NumberInt(2),
                        grupo_id: NumberInt(1),
                        profesor_id: NumberInt(2),

                        calificacion: 9.3,
                        estado: "APROBADA",
                        tipo_curso: "ORDINARIO",
                        intento: NumberInt(1),

                        fecha_cierre: ISODate("2025-06-20T00:00:00Z")
                    }
                ]
            },

            {
                periodo: "2025-2",
                semestre: NumberInt(2),

                materias: [
                    {
                        grupo_materia_id: NumberInt(9),
                        materia_id: NumberInt(3),
                        grupo_id: NumberInt(3),
                        profesor_id: NumberInt(9),

                        calificacion: 9.5,
                        estado: "APROBADA",
                        tipo_curso: "ORDINARIO",
                        intento: NumberInt(1),

                        fecha_cierre: ISODate("2025-12-15T00:00:00Z")
                    },

                    {
                        grupo_materia_id: NumberInt(10),
                        materia_id: NumberInt(4),
                        grupo_id: NumberInt(3),
                        profesor_id: NumberInt(1),

                        calificacion: 9.8,
                        estado: "APROBADA",
                        tipo_curso: "ORDINARIO",
                        intento: NumberInt(1),

                        fecha_cierre: ISODate("2025-12-15T00:00:00Z")
                    },

                    {
                        grupo_materia_id: NumberInt(11),
                        materia_id: NumberInt(5),
                        grupo_id: NumberInt(3),
                        profesor_id: NumberInt(5),

                        calificacion: 9.4,
                        estado: "APROBADA",
                        tipo_curso: "ORDINARIO",
                        intento: NumberInt(1),

                        fecha_cierre: ISODate("2025-12-15T00:00:00Z")
                    }
                ]
            }
        ],

        fecha_creacion: ISODate("2025-06-20T00:00:00Z"),
        fecha_actualizacion: ISODate("2025-12-15T00:00:00Z")
    },


    // --------------------------------------------------------
    // 4. DIEGO SÁNCHEZ
    // Caso:
    // - No presentó Ecuaciones Diferenciales.
    // - Actualmente vuelve a cursarla.
    // --------------------------------------------------------

    {
        estudiante_id: NumberInt(4),

        periodos: [
            {
                periodo: "2025-2",
                semestre: NumberInt(2),

                materias: [
                    {
                        grupo_materia_id: NumberInt(4),
                        materia_id: NumberInt(4),
                        grupo_id: NumberInt(2),
                        profesor_id: NumberInt(4),

                        calificacion: 7.2,
                        estado: "APROBADA",
                        tipo_curso: "ORDINARIO",
                        intento: NumberInt(1),

                        fecha_cierre: ISODate("2025-12-15T00:00:00Z")
                    },

                    {
                        grupo_materia_id: NumberInt(6),
                        materia_id: NumberInt(6),
                        grupo_id: NumberInt(2),
                        profesor_id: NumberInt(6),

                        estado: "NO_PRESENTADA",
                        tipo_curso: "ORDINARIO",
                        intento: NumberInt(1),

                        observaciones:
                            "No presentó la evaluación final."
                    }
                ]
            }
        ],

        fecha_creacion: ISODate("2025-12-15T00:00:00Z"),
        fecha_actualizacion: ISODate("2025-12-15T00:00:00Z")
    },


    // --------------------------------------------------------
    // 5. VALERIA CRUZ
    // Caso:
    // - Trayectoria mixta.
    // - Reprobó Probabilidad.
    // - Actualmente cursa Machine Learning e IA.
    // --------------------------------------------------------

    {
        estudiante_id: NumberInt(5),

        periodos: [
            {
                periodo: "2025-2",
                semestre: NumberInt(2),

                materias: [
                    {
                        grupo_materia_id: NumberInt(9),
                        materia_id: NumberInt(3),
                        grupo_id: NumberInt(3),
                        profesor_id: NumberInt(9),

                        calificacion: 8.4,
                        estado: "APROBADA",
                        tipo_curso: "ORDINARIO",
                        intento: NumberInt(1),

                        fecha_cierre: ISODate("2025-12-15T00:00:00Z")
                    },

                    {
                        grupo_materia_id: NumberInt(10),
                        materia_id: NumberInt(4),
                        grupo_id: NumberInt(3),
                        profesor_id: NumberInt(1),

                        calificacion: 8.0,
                        estado: "APROBADA",
                        tipo_curso: "ORDINARIO",
                        intento: NumberInt(1),

                        fecha_cierre: ISODate("2025-12-15T00:00:00Z")
                    }
                ]
            },

            {
                periodo: "2026-1",
                semestre: NumberInt(3),

                materias: [
                    {
                        grupo_materia_id: NumberInt(13),
                        materia_id: NumberInt(9),
                        grupo_id: NumberInt(4),
                        profesor_id: NumberInt(3),

                        calificacion: 8.3,
                        estado: "APROBADA",
                        tipo_curso: "ORDINARIO",
                        intento: NumberInt(1),

                        fecha_cierre: ISODate("2026-06-19T00:00:00Z")
                    },

                    {
                        grupo_materia_id: NumberInt(14),
                        materia_id: NumberInt(10),
                        grupo_id: NumberInt(4),
                        profesor_id: NumberInt(6),

                        calificacion: 5.5,
                        estado: "REPROBADA",
                        tipo_curso: "ORDINARIO",
                        intento: NumberInt(1),

                        fecha_cierre: ISODate("2026-06-19T00:00:00Z")
                    }
                ]
            }
        ],

        fecha_creacion: ISODate("2025-12-15T00:00:00Z"),
        fecha_actualizacion: ISODate("2026-06-19T00:00:00Z")
    },


    // --------------------------------------------------------
    // 6. MATEO FLORES
    // Caso:
    // - Reprobó Bases de Datos.
    // - Actualmente la recursa con otro profesor.
    // --------------------------------------------------------

    {
        estudiante_id: NumberInt(6),

        periodos: [
            {
                periodo: "2025-2",
                semestre: NumberInt(2),

                materias: [
                    {
                        grupo_materia_id: NumberInt(3),
                        materia_id: NumberInt(3),
                        grupo_id: NumberInt(2),
                        profesor_id: NumberInt(3),

                        calificacion: 7.0,
                        estado: "APROBADA",
                        tipo_curso: "ORDINARIO",
                        intento: NumberInt(1),

                        fecha_cierre: ISODate("2025-12-15T00:00:00Z")
                    },

                    {
                        grupo_materia_id: NumberInt(4),
                        materia_id: NumberInt(4),
                        grupo_id: NumberInt(2),
                        profesor_id: NumberInt(4),

                        calificacion: 5.7,
                        estado: "REPROBADA",
                        tipo_curso: "ORDINARIO",
                        intento: NumberInt(1),

                        fecha_cierre: ISODate("2025-12-15T00:00:00Z")
                    }
                ]
            }
        ],

        fecha_creacion: ISODate("2025-12-15T00:00:00Z"),
        fecha_actualizacion: ISODate("2025-12-15T00:00:00Z")
    },


    // --------------------------------------------------------
    // 7. CAMILA ORTEGA
    // Caso:
    // - Estudiante de IIA.
    // - Buen rendimiento general.
    // --------------------------------------------------------

    {
        estudiante_id: NumberInt(7),

        periodos: [
            {
                periodo: "2025-2",
                semestre: NumberInt(2),

                materias: [
                    {
                        grupo_materia_id: NumberInt(29),
                        materia_id: NumberInt(15),
                        grupo_id: NumberInt(9),
                        profesor_id: NumberInt(10),

                        calificacion: 8.7,
                        estado: "APROBADA",
                        tipo_curso: "ORDINARIO",
                        intento: NumberInt(1),

                        fecha_cierre: ISODate("2025-12-15T00:00:00Z")
                    },

                    {
                        grupo_materia_id: NumberInt(30),
                        materia_id: NumberInt(16),
                        grupo_id: NumberInt(9),
                        profesor_id: NumberInt(6),

                        calificacion: 8.9,
                        estado: "APROBADA",
                        tipo_curso: "ORDINARIO",
                        intento: NumberInt(1),

                        fecha_cierre: ISODate("2025-12-15T00:00:00Z")
                    }
                ]
            }
        ],

        fecha_creacion: ISODate("2025-12-15T00:00:00Z"),
        fecha_actualizacion: ISODate("2025-12-15T00:00:00Z")
    },


    // --------------------------------------------------------
    // 8. EMILIANO VARGAS
    // Caso:
    // - Dio de baja PLN en 2026-1.
    // - Actualmente vuelve a cursarla.
    // --------------------------------------------------------

    {
        estudiante_id: NumberInt(8),

        periodos: [
            {
                periodo: "2025-2",
                semestre: NumberInt(2),

                materias: [
                    {
                        grupo_materia_id: NumberInt(29),
                        materia_id: NumberInt(15),
                        grupo_id: NumberInt(9),
                        profesor_id: NumberInt(10),

                        calificacion: 7.3,
                        estado: "APROBADA",
                        tipo_curso: "ORDINARIO",
                        intento: NumberInt(1),

                        fecha_cierre: ISODate("2025-12-15T00:00:00Z")
                    }
                ]
            },

            {
                periodo: "2026-1",
                semestre: NumberInt(3),

                materias: [
                    {
                        grupo_materia_id: NumberInt(31),
                        materia_id: NumberInt(17),
                        grupo_id: NumberInt(10),
                        profesor_id: NumberInt(5),

                        estado: "BAJA",
                        tipo_curso: "ORDINARIO",
                        intento: NumberInt(1),

                        observaciones:
                            "Solicitó baja de Procesamiento de Lenguaje Natural."
                    }
                ]
            }
        ],

        fecha_creacion: ISODate("2025-12-15T00:00:00Z"),
        fecha_actualizacion: ISODate("2026-04-10T00:00:00Z")
    },


    // --------------------------------------------------------
    // 9. REGINA NAVARRO
    // Caso:
    // - Rendimiento sobresaliente.
    // --------------------------------------------------------

    {
        estudiante_id: NumberInt(9),

        periodos: [
            {
                periodo: "2025-2",
                semestre: NumberInt(2),

                materias: [
                    {
                        grupo_materia_id: NumberInt(29),
                        materia_id: NumberInt(15),
                        grupo_id: NumberInt(9),
                        profesor_id: NumberInt(10),

                        calificacion: 9.8,
                        estado: "APROBADA",
                        tipo_curso: "ORDINARIO",
                        intento: NumberInt(1),

                        fecha_cierre: ISODate("2025-12-15T00:00:00Z")
                    },

                    {
                        grupo_materia_id: NumberInt(30),
                        materia_id: NumberInt(16),
                        grupo_id: NumberInt(9),
                        profesor_id: NumberInt(6),

                        calificacion: 9.6,
                        estado: "APROBADA",
                        tipo_curso: "ORDINARIO",
                        intento: NumberInt(1),

                        fecha_cierre: ISODate("2025-12-15T00:00:00Z")
                    }
                ]
            }
        ],

        fecha_creacion: ISODate("2025-12-15T00:00:00Z"),
        fecha_actualizacion: ISODate("2025-12-15T00:00:00Z")
    },


    // --------------------------------------------------------
    // 10. DANIEL MORALES
    // Caso:
    // - Rendimiento medio.
    // - Dificultades principalmente en exámenes.
    // --------------------------------------------------------

    {
        estudiante_id: NumberInt(10),

        periodos: [
            {
                periodo: "2025-2",
                semestre: NumberInt(2),

                materias: [
                    {
                        grupo_materia_id: NumberInt(29),
                        materia_id: NumberInt(15),
                        grupo_id: NumberInt(9),
                        profesor_id: NumberInt(10),

                        calificacion: 6.5,
                        estado: "APROBADA",
                        tipo_curso: "ORDINARIO",
                        intento: NumberInt(1),

                        fecha_cierre: ISODate("2025-12-15T00:00:00Z")
                    },

                    {
                        grupo_materia_id: NumberInt(30),
                        materia_id: NumberInt(16),
                        grupo_id: NumberInt(9),
                        profesor_id: NumberInt(6),

                        calificacion: 6.8,
                        estado: "APROBADA",
                        tipo_curso: "ORDINARIO",
                        intento: NumberInt(1),

                        fecha_cierre: ISODate("2025-12-15T00:00:00Z")
                    }
                ]
            }
        ],

        fecha_creacion: ISODate("2025-12-15T00:00:00Z"),
        fecha_actualizacion: ISODate("2025-12-15T00:00:00Z")
    }
]);


// ============================================================
// TRAYECTORIAS ACADÉMICAS
// ============================================================

db.trayectoria_academica.insertMany([

    // ANA
    {
        estudiante_id: NumberInt(1),

        eventos: [
            {
                tipo: "INSCRIPCION",
                fecha: ISODate("2025-08-15T00:00:00Z"),
                periodo: "2025-2",

                grupo_materia_id: NumberInt(4),
                materia_id: NumberInt(4),
                grupo_id: NumberInt(2),
                profesor_id: NumberInt(4)
            },

            {
                tipo: "REPROBACION",
                fecha: ISODate("2025-12-15T00:00:00Z"),
                periodo: "2025-2",

                grupo_materia_id: NumberInt(4),
                materia_id: NumberInt(4),
                grupo_id: NumberInt(2),
                profesor_id: NumberInt(4),

                calificacion: 5.0
            },

            {
                tipo: "RECURSAMIENTO",
                fecha: ISODate("2026-07-20T00:00:00Z"),
                periodo: "2026-2",

                grupo_materia_id: NumberInt(16),
                materia_id: NumberInt(4),
                grupo_id: NumberInt(5),
                profesor_id: NumberInt(1),

                detalles: {
                    intento: NumberInt(2),
                    cambio_profesor: true
                }
            }
        ],

        fecha_creacion: ISODate("2025-08-15T00:00:00Z"),
        fecha_actualizacion: ISODate("2026-07-20T00:00:00Z")
    },


    // LUIS
    {
        estudiante_id: NumberInt(2),

        eventos: [
            {
                tipo: "INSCRIPCION",
                fecha: ISODate("2025-08-15T00:00:00Z"),
                periodo: "2025-2",

                grupo_materia_id: NumberInt(3),
                materia_id: NumberInt(3),
                grupo_id: NumberInt(2),
                profesor_id: NumberInt(3)
            },

            {
                tipo: "BAJA_MATERIA",
                fecha: ISODate("2025-10-03T00:00:00Z"),
                periodo: "2025-2",

                grupo_materia_id: NumberInt(3),
                materia_id: NumberInt(3),
                grupo_id: NumberInt(2),
                profesor_id: NumberInt(3),

                motivo: "BAJA_SOLICITADA"
            },

            {
                tipo: "RECURSAMIENTO",
                fecha: ISODate("2026-07-20T00:00:00Z"),
                periodo: "2026-2",

                grupo_materia_id: NumberInt(15),
                materia_id: NumberInt(3),
                grupo_id: NumberInt(5),
                profesor_id: NumberInt(3),

                detalles: {
                    intento: NumberInt(2)
                }
            }
        ],

        fecha_creacion: ISODate("2025-08-15T00:00:00Z"),
        fecha_actualizacion: ISODate("2026-07-20T00:00:00Z")
    },


    // SOFÍA
    {
        estudiante_id: NumberInt(3),

        eventos: [
            {
                tipo: "APROBACION",
                fecha: ISODate("2025-12-15T00:00:00Z"),
                periodo: "2025-2",

                grupo_materia_id: NumberInt(10),
                materia_id: NumberInt(4),
                grupo_id: NumberInt(3),
                profesor_id: NumberInt(1),

                calificacion: 9.8
            },

            {
                tipo: "ACREDITACION",
                fecha: ISODate("2025-12-15T00:00:00Z"),
                periodo: "2025-2",

                detalles: {
                    promedio_periodo: 9.56,
                    materias_reprobadas: NumberInt(0)
                }
            }
        ],

        fecha_creacion: ISODate("2025-12-15T00:00:00Z"),
        fecha_actualizacion: ISODate("2025-12-15T00:00:00Z")
    },


    // DIEGO
    {
        estudiante_id: NumberInt(4),

        eventos: [
            {
                tipo: "INSCRIPCION",
                fecha: ISODate("2025-08-15T00:00:00Z"),
                periodo: "2025-2",

                grupo_materia_id: NumberInt(6),
                materia_id: NumberInt(6),
                grupo_id: NumberInt(2),
                profesor_id: NumberInt(6)
            },

            {
                tipo: "REPROBACION",
                fecha: ISODate("2025-12-15T00:00:00Z"),
                periodo: "2025-2",

                grupo_materia_id: NumberInt(6),
                materia_id: NumberInt(6),
                grupo_id: NumberInt(2),
                profesor_id: NumberInt(6),

                motivo: "NO_PRESENTADA"
            },

            {
                tipo: "RECURSAMIENTO",
                fecha: ISODate("2026-07-21T00:00:00Z"),
                periodo: "2026-2",

                grupo_materia_id: NumberInt(18),
                materia_id: NumberInt(6),
                grupo_id: NumberInt(5),
                profesor_id: NumberInt(6)
            }
        ],

        fecha_creacion: ISODate("2025-08-15T00:00:00Z"),
        fecha_actualizacion: ISODate("2026-07-21T00:00:00Z")
    },


    // VALERIA
    {
        estudiante_id: NumberInt(5),

        eventos: [
            {
                tipo: "REPROBACION",
                fecha: ISODate("2026-06-19T00:00:00Z"),
                periodo: "2026-1",

                grupo_materia_id: NumberInt(14),
                materia_id: NumberInt(10),
                grupo_id: NumberInt(4),
                profesor_id: NumberInt(6),

                calificacion: 5.5
            },

            {
                tipo: "INSCRIPCION",
                fecha: ISODate("2026-07-21T00:00:00Z"),
                periodo: "2026-2",

                grupo_materia_id: NumberInt(27),
                materia_id: NumberInt(11),
                grupo_id: NumberInt(8),
                profesor_id: NumberInt(10)
            },

            {
                tipo: "INSCRIPCION",
                fecha: ISODate("2026-07-21T00:00:00Z"),
                periodo: "2026-2",

                grupo_materia_id: NumberInt(28),
                materia_id: NumberInt(12),
                grupo_id: NumberInt(8),
                profesor_id: NumberInt(5)
            }
        ],

        fecha_creacion: ISODate("2026-06-19T00:00:00Z"),
        fecha_actualizacion: ISODate("2026-07-21T00:00:00Z")
    },


    // MATEO
    {
        estudiante_id: NumberInt(6),

        eventos: [
            {
                tipo: "REPROBACION",
                fecha: ISODate("2025-12-15T00:00:00Z"),
                periodo: "2025-2",

                grupo_materia_id: NumberInt(4),
                materia_id: NumberInt(4),
                grupo_id: NumberInt(2),
                profesor_id: NumberInt(4),

                calificacion: 5.7
            },

            {
                tipo: "RECURSAMIENTO",
                fecha: ISODate("2026-07-21T00:00:00Z"),
                periodo: "2026-2",

                grupo_materia_id: NumberInt(16),
                materia_id: NumberInt(4),
                grupo_id: NumberInt(5),
                profesor_id: NumberInt(1),

                detalles: {
                    intento: NumberInt(2),
                    cambio_profesor: true
                }
            }
        ],

        fecha_creacion: ISODate("2025-12-15T00:00:00Z"),
        fecha_actualizacion: ISODate("2026-07-21T00:00:00Z")
    },


    // CAMILA
    {
        estudiante_id: NumberInt(7),

        eventos: [
            {
                tipo: "APROBACION",
                fecha: ISODate("2025-12-15T00:00:00Z"),
                periodo: "2025-2",

                grupo_materia_id: NumberInt(29),
                materia_id: NumberInt(15),
                grupo_id: NumberInt(9),
                profesor_id: NumberInt(10),

                calificacion: 8.7
            },

            {
                tipo: "INSCRIPCION",
                fecha: ISODate("2026-07-22T00:00:00Z"),
                periodo: "2026-2",

                grupo_materia_id: NumberInt(33),
                materia_id: NumberInt(17),
                grupo_id: NumberInt(11),
                profesor_id: NumberInt(5)
            }
        ],

        fecha_creacion: ISODate("2025-12-15T00:00:00Z"),
        fecha_actualizacion: ISODate("2026-07-22T00:00:00Z")
    },


    // EMILIANO
    {
        estudiante_id: NumberInt(8),

        eventos: [
            {
                tipo: "INSCRIPCION",
                fecha: ISODate("2026-01-15T00:00:00Z"),
                periodo: "2026-1",

                grupo_materia_id: NumberInt(31),
                materia_id: NumberInt(17),
                grupo_id: NumberInt(10),
                profesor_id: NumberInt(5)
            },

            {
                tipo: "BAJA_MATERIA",
                fecha: ISODate("2026-04-10T00:00:00Z"),
                periodo: "2026-1",

                grupo_materia_id: NumberInt(31),
                materia_id: NumberInt(17),
                grupo_id: NumberInt(10),
                profesor_id: NumberInt(5),

                motivo: "BAJA_SOLICITADA"
            },

            {
                tipo: "RECURSAMIENTO",
                fecha: ISODate("2026-07-22T00:00:00Z"),
                periodo: "2026-2",

                grupo_materia_id: NumberInt(33),
                materia_id: NumberInt(17),
                grupo_id: NumberInt(11),
                profesor_id: NumberInt(5)
            }
        ],

        fecha_creacion: ISODate("2026-01-15T00:00:00Z"),
        fecha_actualizacion: ISODate("2026-07-22T00:00:00Z")
    },


    // REGINA
    {
        estudiante_id: NumberInt(9),

        eventos: [
            {
                tipo: "ACREDITACION",
                fecha: ISODate("2025-12-15T00:00:00Z"),
                periodo: "2025-2",

                detalles: {
                    promedio_periodo: 9.7,
                    materias_reprobadas: NumberInt(0),
                    rendimiento: "SOBRESALIENTE"
                }
            },

            {
                tipo: "INSCRIPCION",
                fecha: ISODate("2026-07-22T00:00:00Z"),
                periodo: "2026-2",

                grupo_materia_id: NumberInt(33),
                materia_id: NumberInt(17),
                grupo_id: NumberInt(11),
                profesor_id: NumberInt(5)
            }
        ],

        fecha_creacion: ISODate("2025-12-15T00:00:00Z"),
        fecha_actualizacion: ISODate("2026-07-22T00:00:00Z")
    },


    // DANIEL
    {
        estudiante_id: NumberInt(10),

        eventos: [
            {
                tipo: "APROBACION",
                fecha: ISODate("2025-12-15T00:00:00Z"),
                periodo: "2025-2",

                grupo_materia_id: NumberInt(30),
                materia_id: NumberInt(16),
                grupo_id: NumberInt(9),
                profesor_id: NumberInt(6),

                calificacion: 6.8
            },

            {
                tipo: "INSCRIPCION",
                fecha: ISODate("2026-07-22T00:00:00Z"),
                periodo: "2026-2",

                grupo_materia_id: NumberInt(34),
                materia_id: NumberInt(18),
                grupo_id: NumberInt(11),
                profesor_id: NumberInt(8)
            }
        ],

        fecha_creacion: ISODate("2025-12-15T00:00:00Z"),
        fecha_actualizacion: ISODate("2026-07-22T00:00:00Z")
    }
]);


// ============================================================
// EVALUACIONES
// ============================================================

db.evaluaciones.insertMany([

    // --------------------------------------------------------
    // ANA - BASES DE DATOS
    // Reprobó principalmente por parciales bajos.
    // --------------------------------------------------------

    {
        estudiante_id: NumberInt(1),
        grupo_materia_id: NumberInt(4),
        materia_id: NumberInt(4),
        grupo_id: NumberInt(2),
        profesor_id: NumberInt(4),

        periodo: "2025-2",
        intento: NumberInt(1),

        evaluaciones: [
            {
                tipo: "PARCIAL",
                nombre: "Primer parcial",
                calificacion: 4.5,
                ponderacion: 30,
                fecha: ISODate("2025-09-25T00:00:00Z")
            },

            {
                tipo: "PARCIAL",
                nombre: "Segundo parcial",
                calificacion: 4.0,
                ponderacion: 30,
                fecha: ISODate("2025-11-06T00:00:00Z")
            },

            {
                tipo: "PROYECTO",
                nombre: "Proyecto final",
                calificacion: 7.0,
                ponderacion: 40,
                fecha: ISODate("2025-12-05T00:00:00Z"),

                detalles: {
                    motor: "MySQL",
                    trabajo_equipo: true
                },

                entregables: [
                    {
                        nombre: "Modelo entidad-relación",
                        tipo: "DIAGRAMA",
                        referencia: "proyecto_bd/erd"
                    },

                    {
                        nombre: "Script SQL",
                        tipo: "CODIGO",
                        referencia: "proyecto_bd/schema.sql"
                    }
                ]
            }
        ],

        calificacion_final: 5.0,

        fecha_creacion: ISODate("2025-12-15T00:00:00Z"),
        fecha_actualizacion: ISODate("2025-12-15T00:00:00Z")
    },


    // --------------------------------------------------------
    // LUIS - ANÁLISIS DE ALGORITMOS
    // --------------------------------------------------------

    {
        estudiante_id: NumberInt(2),
        grupo_materia_id: NumberInt(5),
        materia_id: NumberInt(5),
        grupo_id: NumberInt(2),
        profesor_id: NumberInt(5),

        periodo: "2025-2",
        intento: NumberInt(1),

        evaluaciones: [
            {
                tipo: "PARCIAL",
                nombre: "Análisis de complejidad",
                calificacion: 7.0,
                ponderacion: 35,
                fecha: ISODate("2025-09-30T00:00:00Z")
            },

            {
                tipo: "PRACTICA",
                nombre: "Algoritmos de ordenamiento",
                calificacion: 8.0,
                ponderacion: 25,
                fecha: ISODate("2025-10-28T00:00:00Z"),

                detalles: {
                    lenguaje: "C",
                    algoritmos: [
                        "quicksort",
                        "mergesort",
                        "heapsort"
                    ]
                }
            },

            {
                tipo: "EXAMEN_FINAL",
                nombre: "Evaluación final",
                calificacion: 7.5,
                ponderacion: 40,
                fecha: ISODate("2025-12-08T00:00:00Z")
            }
        ],

        calificacion_final: 7.5,

        fecha_creacion: ISODate("2025-12-15T00:00:00Z"),
        fecha_actualizacion: ISODate("2025-12-15T00:00:00Z")
    },


    // --------------------------------------------------------
    // SOFÍA - BASES DE DATOS
    // Excelente rendimiento.
    // --------------------------------------------------------

    {
        estudiante_id: NumberInt(3),
        grupo_materia_id: NumberInt(10),
        materia_id: NumberInt(4),
        grupo_id: NumberInt(3),
        profesor_id: NumberInt(1),

        periodo: "2025-2",
        intento: NumberInt(1),

        evaluaciones: [
            {
                tipo: "PARCIAL",
                nombre: "Modelo relacional",
                calificacion: 9.5,
                ponderacion: 25,
                fecha: ISODate("2025-09-24T00:00:00Z")
            },

            {
                tipo: "LABORATORIO",
                nombre: "Consultas SQL",
                calificacion: 10.0,
                ponderacion: 25,
                fecha: ISODate("2025-10-29T00:00:00Z"),

                detalles: {
                    consultas_completadas: NumberInt(20),
                    errores: NumberInt(0)
                }
            },

            {
                tipo: "PROYECTO",
                nombre: "Sistema académico",
                calificacion: 10.0,
                ponderacion: 50,
                fecha: ISODate("2025-12-05T00:00:00Z"),

                detalles: {
                    normalizacion: "3FN",
                    motor: "MySQL"
                }
            }
        ],

        calificacion_final: 9.8,

        fecha_creacion: ISODate("2025-12-15T00:00:00Z"),
        fecha_actualizacion: ISODate("2025-12-15T00:00:00Z")
    },


    // --------------------------------------------------------
    // DIEGO - BASES DE DATOS
    // --------------------------------------------------------

    {
        estudiante_id: NumberInt(4),
        grupo_materia_id: NumberInt(4),
        materia_id: NumberInt(4),
        grupo_id: NumberInt(2),
        profesor_id: NumberInt(4),

        periodo: "2025-2",
        intento: NumberInt(1),

        evaluaciones: [
            {
                tipo: "PARCIAL",
                nombre: "Primer parcial",
                calificacion: 6.5,
                ponderacion: 30,
                fecha: ISODate("2025-09-25T00:00:00Z")
            },

            {
                tipo: "PRACTICA",
                nombre: "Consultas SQL",
                calificacion: 8.0,
                ponderacion: 30,
                fecha: ISODate("2025-11-03T00:00:00Z")
            },

            {
                tipo: "PROYECTO",
                nombre: "Proyecto final",
                calificacion: 7.2,
                ponderacion: 40,
                fecha: ISODate("2025-12-05T00:00:00Z")
            }
        ],

        calificacion_final: 7.2,

        fecha_creacion: ISODate("2025-12-15T00:00:00Z"),
        fecha_actualizacion: ISODate("2025-12-15T00:00:00Z")
    },


    // --------------------------------------------------------
    // VALERIA - PROBABILIDAD
    // Caso reprobado.
    // --------------------------------------------------------

    {
        estudiante_id: NumberInt(5),
        grupo_materia_id: NumberInt(14),
        materia_id: NumberInt(10),
        grupo_id: NumberInt(4),
        profesor_id: NumberInt(6),

        periodo: "2026-1",
        intento: NumberInt(1),

        evaluaciones: [
            {
                tipo: "PARCIAL",
                nombre: "Probabilidad básica",
                calificacion: 5.0,
                ponderacion: 30,
                fecha: ISODate("2026-02-27T00:00:00Z")
            },

            {
                tipo: "PARCIAL",
                nombre: "Variables aleatorias",
                calificacion: 5.5,
                ponderacion: 30,
                fecha: ISODate("2026-04-17T00:00:00Z")
            },

            {
                tipo: "EXAMEN_FINAL",
                nombre: "Examen final",
                calificacion: 6.0,
                ponderacion: 40,
                fecha: ISODate("2026-06-12T00:00:00Z")
            }
        ],

        calificacion_final: 5.5,

        fecha_creacion: ISODate("2026-06-19T00:00:00Z"),
        fecha_actualizacion: ISODate("2026-06-19T00:00:00Z")
    },


    // --------------------------------------------------------
    // MATEO - BASES DE DATOS
    // Segundo alumno con reprobación previa de BD.
    // --------------------------------------------------------

    {
        estudiante_id: NumberInt(6),
        grupo_materia_id: NumberInt(4),
        materia_id: NumberInt(4),
        grupo_id: NumberInt(2),
        profesor_id: NumberInt(4),

        periodo: "2025-2",
        intento: NumberInt(1),

        evaluaciones: [
            {
                tipo: "PARCIAL",
                nombre: "Primer parcial",
                calificacion: 5.0,
                ponderacion: 30,
                fecha: ISODate("2025-09-25T00:00:00Z")
            },

            {
                tipo: "LABORATORIO",
                nombre: "SQL",
                calificacion: 7.5,
                ponderacion: 30,
                fecha: ISODate("2025-11-05T00:00:00Z")
            },

            {
                tipo: "EXAMEN_FINAL",
                nombre: "Examen final",
                calificacion: 4.8,
                ponderacion: 40,
                fecha: ISODate("2025-12-10T00:00:00Z")
            }
        ],

        calificacion_final: 5.7,

        fecha_creacion: ISODate("2025-12-15T00:00:00Z"),
        fecha_actualizacion: ISODate("2025-12-15T00:00:00Z")
    },


    // --------------------------------------------------------
    // CAMILA - APRENDIZAJE AUTOMÁTICO
    // --------------------------------------------------------

    {
        estudiante_id: NumberInt(7),
        grupo_materia_id: NumberInt(29),
        materia_id: NumberInt(15),
        grupo_id: NumberInt(9),
        profesor_id: NumberInt(10),

        periodo: "2025-2",
        intento: NumberInt(1),

        evaluaciones: [
            {
                tipo: "PARCIAL",
                nombre: "Fundamentos de aprendizaje supervisado",
                calificacion: 8.5,
                ponderacion: 30,
                fecha: ISODate("2025-09-29T00:00:00Z")
            },

            {
                tipo: "PRACTICA",
                nombre: "Clasificación",
                calificacion: 9.0,
                ponderacion: 30,
                fecha: ISODate("2025-10-30T00:00:00Z"),

                detalles: {
                    lenguaje: "Python",
                    modelo: "Random Forest",
                    dataset: "clasificacion_demo"
                }
            },

            {
                tipo: "PROYECTO",
                nombre: "Modelo predictivo",
                calificacion: 8.7,
                ponderacion: 40,
                fecha: ISODate("2025-12-05T00:00:00Z")
            }
        ],

        calificacion_final: 8.7,

        fecha_creacion: ISODate("2025-12-15T00:00:00Z"),
        fecha_actualizacion: ISODate("2025-12-15T00:00:00Z")
    },


    // --------------------------------------------------------
    // EMILIANO - APRENDIZAJE AUTOMÁTICO
    // --------------------------------------------------------

    {
        estudiante_id: NumberInt(8),
        grupo_materia_id: NumberInt(29),
        materia_id: NumberInt(15),
        grupo_id: NumberInt(9),
        profesor_id: NumberInt(10),

        periodo: "2025-2",
        intento: NumberInt(1),

        evaluaciones: [
            {
                tipo: "PARCIAL",
                nombre: "Primer parcial",
                calificacion: 6.5,
                ponderacion: 30,
                fecha: ISODate("2025-09-29T00:00:00Z")
            },

            {
                tipo: "PRACTICA",
                nombre: "Clasificación",
                calificacion: 8.0,
                ponderacion: 30,
                fecha: ISODate("2025-10-30T00:00:00Z")
            },

            {
                tipo: "PROYECTO",
                nombre: "Proyecto final",
                calificacion: 7.4,
                ponderacion: 40,
                fecha: ISODate("2025-12-05T00:00:00Z")
            }
        ],

        calificacion_final: 7.3,

        fecha_creacion: ISODate("2025-12-15T00:00:00Z"),
        fecha_actualizacion: ISODate("2025-12-15T00:00:00Z")
    },


    // --------------------------------------------------------
    // REGINA - APRENDIZAJE AUTOMÁTICO
    // Rendimiento sobresaliente.
    // --------------------------------------------------------

    {
        estudiante_id: NumberInt(9),
        grupo_materia_id: NumberInt(29),
        materia_id: NumberInt(15),
        grupo_id: NumberInt(9),
        profesor_id: NumberInt(10),

        periodo: "2025-2",
        intento: NumberInt(1),

        evaluaciones: [
            {
                tipo: "PARCIAL",
                nombre: "Aprendizaje supervisado",
                calificacion: 9.7,
                ponderacion: 30,
                fecha: ISODate("2025-09-29T00:00:00Z")
            },

            {
                tipo: "PRACTICA",
                nombre: "Comparación de clasificadores",
                calificacion: 10.0,
                ponderacion: 30,
                fecha: ISODate("2025-10-30T00:00:00Z"),

                detalles: {
                    modelos: [
                        "SVM",
                        "Random Forest",
                        "MLP"
                    ],

                    mejor_modelo: "SVM"
                }
            },

            {
                tipo: "PROYECTO",
                nombre: "Sistema de clasificación",
                calificacion: 9.8,
                ponderacion: 40,
                fecha: ISODate("2025-12-05T00:00:00Z")
            }
        ],

        calificacion_final: 9.8,

        fecha_creacion: ISODate("2025-12-15T00:00:00Z"),
        fecha_actualizacion: ISODate("2025-12-15T00:00:00Z")
    },


    // --------------------------------------------------------
    // DANIEL - ESTADÍSTICA
    //
    // Caso importante:
    // malos parciales + buenas prácticas/proyectos.
    // --------------------------------------------------------

    {
        estudiante_id: NumberInt(10),
        grupo_materia_id: NumberInt(30),
        materia_id: NumberInt(16),
        grupo_id: NumberInt(9),
        profesor_id: NumberInt(6),

        periodo: "2025-2",
        intento: NumberInt(1),

        evaluaciones: [
            {
                tipo: "PARCIAL",
                nombre: "Primer parcial",
                calificacion: 4.5,
                ponderacion: 25,
                fecha: ISODate("2025-09-26T00:00:00Z")
            },

            {
                tipo: "PARCIAL",
                nombre: "Segundo parcial",
                calificacion: 5.0,
                ponderacion: 25,
                fecha: ISODate("2025-11-07T00:00:00Z")
            },

            {
                tipo: "PRACTICA",
                nombre: "Análisis estadístico con Python",
                calificacion: 8.5,
                ponderacion: 20,
                fecha: ISODate("2025-11-21T00:00:00Z"),

                detalles: {
                    herramienta: "Python",
                    librerias: [
                        "pandas",
                        "numpy"
                    ]
                }
            },

            {
                tipo: "PROYECTO",
                nombre: "Proyecto estadístico",
                calificacion: 8.0,
                ponderacion: 30,
                fecha: ISODate("2025-12-08T00:00:00Z")
            }
        ],

        calificacion_final: 6.8,

        fecha_creacion: ISODate("2025-12-15T00:00:00Z"),
        fecha_actualizacion: ISODate("2025-12-15T00:00:00Z")
    }
]);


print("============================================================");
print("Seed de MongoDB cargado correctamente.");
print(`Base de datos: ${databaseName}`);

print(`Historiales: ${
    db.historial_academico.countDocuments()
}`);

print(`Trayectorias: ${
    db.trayectoria_academica.countDocuments()
}`);

print(`Evaluaciones: ${
    db.evaluaciones.countDocuments()
}`);

print("============================================================");