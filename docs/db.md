# Diseño de las Bases de Datos

# 1. Modelo de información

La Prueba de Concepto(Poc) utiliza un dominio académico como escenario de prueba para el diseño de una arquitectura híbrida que integre MySQL y MongoDB. Esta decisión permite representar información con características diferentes. Por un lado los datos estructurados y relacionados, como estudiantes, profesores, materias, grupos e inscripciones y por otro, información histórica y de estructura más "flexible", como resultados, eventos y evaluaciones académicas.

El modelo se diseñó con el objetivo de distribuir los datos entre ambos motores de acuerdo con sus características y permitir que el sistema pueda relacionarlos mediante consultas híbridas. De esta manera una solicitud puede requerir información actual almacenada en MySQL y antecedentes académicos almacenados en MongoDB para obtener un resultado completo.

## 1.1 Entidades principales

Para representar la información del dominio académico se identificaron las siguientes entidades principales:

* **Programas académicos:** representan los diferentes programas educativos a los que pertenecen los estudiantes y las materias.
* **Estudiantes:** contienen la información necesaria para identificar a los alumnos y asociarlos con su programa académico.
* **Materias:** representan las unidades de aprendizaje disponibles dentro de los programas académicos.
* **Profesores:** representan a los docentes que imparten las materias ofertadas.
* **Grupos:** representan las agrupaciones académicas disponibles durante un periodo determinado, por ejemplo, `2CM2`.
* **Grupos_Materia:** representan la oferta de una materia dentro de un grupo y permiten asociarla con el profesor responsable.
* **Horarios:** determinan cuándo y en qué espacio se imparte una materia perteneciente a un grupo.
* **Salones:** representan los espacios físicos utilizados para impartir clases, incluyendo aulas y laboratorios.
* **Inscripciones:** representan las materias que un estudiante se encuentra cursando mediante su asociación con una oferta determinada.

Adicionalmente, se identificaron tres conjuntos de información histórica:

* **Historial académico:** conserva los resultados obtenidos por los estudiantes en materias cursadas durante diferentes periodos.
* **Trayectoria académica:** registra acontecimientos relevantes ocurridos durante la trayectoria del estudiante, como inscripciones, bajas, reprobaciones, aprobaciones o recursamientos.
* **Evaluaciones:** almacena información sobre las actividades mediante las cuales se obtiene un resultado académico, permitiendo representar diferentes tipos de evaluación y los datos asociados a cada una.


## 1.2 Relaciones entre entidades

Un programa académico agrupa a los estudiantes que pertenecen a él y las materias que forman parte de su plan de estudios. Los grupos representan las agrupaciones académicas disponibles en un periodo y se relacionan con las materias mediante `grupos_Materia`, que representa una materia ofertada dentro de un grupo y permite asociarla con el profesor encargado de impartirla.

Cada materia ofertada puede contar con uno o varios horarios, donde se establece el día, la hora y el salón en el que se imparte. Los salones pueden representar tanto aulas como laboratorios.

Los estudiantes se relacionan con las materias ofertadas mediante las inscripciones. Así, un estudiante puede inscribirse en materias específicas sin que la inscripción a un grupo implique cursar todas las materias asociadas a este.

La información histórica permite conservar los resultados, eventos y evaluaciones asociados con la trayectoria de los estudiantes. Estos datos mantienen referencias a las entidades académicas correspondientes, permitiendo posteriormente relacionar los antecedentes de un estudiante con su situación académica actual(consultas hibridas).

---

# 2. Modelo Relacional (MySQL)


## 2.1 Justificación del uso de MySQL

El uso de MySQL como motor de base de datos relacional forma parte de los requisitos establecidos para la Prueba de Concepto. A partir de este requisito, se diseñó un modelo encargado de almacenar principalmente el estado actual de la información académica y las relaciones existentes entre las entidades del dominio.

Dentro de la arquitectura híbrida, MySQL actúa como la fuente de información sobre el estado actual del dominio académico. Esta información puede consultarse de manera independiente o utilizarse en conjunto con los antecedentes almacenados en MongoDB para resolver consultas híbridas.

## 2.2 Modelo entidad-relación

El modelo entidad-relación representa la estructura de la información almacenada en MySQL y las relaciones existentes entre las entidades que conforman el escenario académico.

![EDR](images/escuela_db.png)

El modelo parte de la separación entre los conceptos de grupo y materia ofertada. Un grupo representa una agrupación académica dentro de un periodo, mientras que grupos_materia representa una materia específica impartida dentro de dicho grupo por un profesor determinado. A partir de esta entidad se relacionan también los horarios y las inscripciones de los estudiantes.

Esta separación permite representar las relaciones del dominio sin duplicar información y proporciona una estructura desde la cual es posible consultar la situación académica actual de los estudiantes.


## 2.3 Descripción de las entidades

El modelo relacional está compuesto por nueve entidades, cada una responsable de representar una parte específica del dominio académico.

2.3.1 Programas académicos

Representa los programas educativos disponibles dentro del escenario académico. Cada programa puede tener asociados múltiples estudiantes y materias.

2.3.2 Estudiantes

Almacena la información de los estudiantes registrados en el sistema. Cada estudiante se encuentra asociado con un programa académico y puede contar con múltiples inscripciones a materias ofertadas.

2.3.3 Materias

Representa las unidades de aprendizaje pertenecientes a los programas académicos. Una materia puede ser ofertada en diferentes grupos y periodos.

2.3.4 Profesores

Almacena la información de los docentes. Los profesores se relacionan con las materias que imparten mediante las ofertas representadas en grupos_materia.

2.3.5 Salones

Son los espacios físicos en los que se imparten las clases. Esta entidad contempla tanto aulas como laboratorios, diferenciados mediante el tipo de espacio.

2.3.6 Grupos

Son las agrupaciones académicas disponibles durante un periodo determinado, por ejemplo, 2CM2. El grupo se mantiene separado de las materias que se imparten dentro de él, permitiendo que una misma agrupación tenga asociadas diferentes materias.

2.3.7 Grupos_Materia

Representa la oferta de una materia dentro de un grupo y su asociación con el profesor encargado de impartirla. Esta entidad permite resolver la relación entre grupos y materias y funciona como punto de referencia para horarios e inscripciones.

2.3.8 Horarios

Estos son los bloques en los que se imparte una materia ofertada. Cada horario establece el día, la hora de inicio, la hora de finalización y el salón correspondiente, permitiendo que una misma materia dentro de un grupo pueda utilizar diferentes espacios o bloques durante la semana.

2.3.9 Inscripciones

Es la relación entre un estudiante y una materia ofertada mediante grupos_materia. De esta forma, la inscripción se realiza sobre una materia específica y no sobre el grupo completo, permitiendo que un estudiante seleccione las materias que cursará.

## 2.4 Relaciones entre tablas

El modelo relacional utiliza claves foráneas y tablas intermedias para representar las asociaciones existentes entre las entidades del dominio académico.

Las principales relaciones son:

- **Programas académicos 1:N Estudiantes:** un programa académico puede tener múltiples estudiantes, mientras que cada estudiante pertenece a un único programa.
- **Programas académicos 1:N Materias:** un programa académico puede contener múltiples materias y cada materia pertenece a un único programa.
- **Grupos N:M Materias:** un grupo puede tener múltiples materias y una materia puede ser ofertada en diferentes grupos. Esta relación se representa mediante la tabla `grupos_materia`.
- **Profesores 1:N Grupos_Materia:** un profesor puede impartir diferentes materias ofertadas, mientras que cada registro de `grupos_materia` tiene asociado un profesor.
- **Grupos_Materia 1:N Horarios:** una materia ofertada puede contar con diferentes bloques de horario durante la semana.
- **Salones 1:N Horarios:** un salón puede utilizarse en diferentes horarios, mientras que cada horario se encuentra asociado con un salón.
- **Estudiantes N:M Grupos_Materia:** un estudiante puede inscribirse en diferentes materias ofertadas y una materia ofertada puede tener múltiples estudiantes. Esta relación se representa mediante la tabla `inscripciones`.

La tabla `grupos_materia` ocupa una posición central dentro del modelo, ya que representa la oferta de una materia dentro de un grupo y permite relacionarla con el profesor que la imparte. A partir de esta entidad se asocian también los horarios y las inscripciones correspondientes.

## 2.5 Normalización

Con el propósito de reducir duplicidad de datos y evitar dependencias innecesarias. El modelo relacional se diseñó separando la información en entidades de acuerdo con los conceptos que representa.

Una de las principales consideraciones durante el diseño fue separar el concepto de **grupo** de las materias impartidas dentro de este. La tabla `grupos` almacena únicamente la información propia de la agrupación académica y el periodo al que pertenece, mientras que `grupos_materia` representa las materias ofertadas dentro de cada grupo y el profesor responsable de impartirlas.

Algo similar ocurre con los horarios, se almacenan en una entidad independiente, ya que una materia ofertada puede impartirse en diferentes días, horarios y espacios físicos. Los salones también se mantienen separados para evitar repetir sus características cada vez que son utilizados por una clase.

Las inscripciones relacionan estudiantes con registros de `grupos_materia`, evitando almacenar directamente en cada inscripción información que ya se encuentra representada por otras entidades, como la materia, el profesor o el grupo.

Esta organización permite modificar información en un único lugar y recuperar las relaciones necesarias mediante las asociaciones definidas entre las tablas.

## 2.6 Restricciones e integridad de datos

Todas las tablas cuentan con una clave primaria numérica denominada `id`, generada automáticamente mediante `AUTO_INCREMENT`. Las relaciones entre tablas se implementan mediante claves foráneas que impiden registrar referencias hacia entidades inexistentes.

Las claves foráneas utilizan `ON DELETE RESTRICT`, evitando eliminar registros que todavía son utilizados por otras entidades, y `ON UPDATE CASCADE`, permitiendo propagar cambios realizados sobre las claves referenciadas.

También se utilizan restricciones de unicidad (UNIQUE) para evitar duplicados en atributos y combinaciones que deben identificar registros de manera única. Entre ellas se encuentran las claves de programas académicos y materias, la boleta de los estudiantes, el número de empleado de los profesores y el número de los salones.

Se definieron restricciones compuestas para reglas propias del dominio:

- `UNIQUE (clave_grupo, periodo)` evita registrar dos veces el mismo grupo dentro de un periodo.
- `UNIQUE (grupo_id, materia_id)` evita ofertar dos veces la misma materia dentro de un grupo.
- `UNIQUE (estudiante_id, grupo_materia_id)` evita que un estudiante se inscriba más de una vez en la misma materia ofertada.

Los campos obligatorios utilizan `NOT NULL`, mientras que algunos valores con un conjunto limitado de opciones utilizan `ENUM`. El día de un horario se restringe a los días hábiles definidos en el modelo y el estado de una inscripción puede ser `ACTIVA` o `INACTIVA`.

Finalmente, los horarios incluyen la restricción `ck_hora_valida`, que utiliza `CHECK` para exigir que la diferencia entre la hora de inicio y la hora de finalización sea de al menos 90 minutos.

## 2.7 Índices

El diseño de este modelo de datos utiliza índices para apoyar tanto las restricciones de integridad como el acceso a columnas utilizadas en las relaciones entre tablas.

Las claves primarias generan índices sobre los identificadores de todas las entidades, mientras que las restricciones `UNIQUE` crean índices únicos sobre los atributos o combinaciones que no pueden repetirse.

Tambien las columnas utilizadas como claves foráneas cuentan con índices que facilitan las operaciones de relación entre tablas. Entre ellos se encuentran:

- `idx_estudiantes_programa` sobre `estudiantes.programa_academico_id`.
- `idx_materias_programa` sobre `materias.programa_academico_id`.
- Índices sobre `grupo_id`, `materia_id` y `profesor_id` utilizados por `grupos_materia`.
- Índices sobre `grupo_materia_id` y `salon_id` utilizados por `horarios`.
- Índices sobre las columnas utilizadas por `inscripciones` para relacionar estudiantes con las materias ofertadas.

---

# 3. Modelo Documental (MongoDB)

## 3.1 Justificación del uso de MongoDB

El uso de MongoDB en esta arquitectura responde a una de las especificaciones y requerimientos del proyecto para la gestión híbrida de datos, donde se establece la necesidad de complementar el modelo relacional tradicional con una base de datos no relacional.

Su integración en el sistema se justifica bajo los siguientes puntos:

* **Soporte para estructuras semiestructuradas e históricas:** A diferencia de la información operativa de MySQL (cuya estructura es rígida y altamente normalizada), los registros como la trayectoria académica o el desglose de evaluaciones contienen información evolutiva y atributos variables que varían según la materia, el tipo de evento o el profesor.
* **Optimización de lectura mediante agregación (Embedding):** Permite consolidar el historial académico completo de un estudiante o el desglose fino de sus parciales en un solo documento JSON/BSON. Esto elimina la necesidad de realizar operaciones *JOIN* complejas y costosas sobre tablas con millones de registros históricos.
* **Encapsulamiento y consistencia lógica:** Al almacenar arreglos embebidos (`periodos`, `materias`, `evaluaciones`), se garantiza que la información relacionada con un evento o periodo se consulte y actualice en una sola operación atómica sobre el documento.

## 3.2 Historial académico

El objetivo principal de esta colección es **acelerar las consultas analíticas del alumno sin saturar la base de datos relacional**. 

En un sistema tradicional de escuela, consultar la "boleta histórica" o el kárdex de un estudiante requiere hacer un *JOIN* pesado entre inscripciones, materias, grupos, profesores y calificaciones a lo largo de varios años. En MongoDB, esta colección resuelve ese problema mediante el **patrón de documento único por alumno**:

* **Estructura unificada:** Todo el pasado escolar del estudiante vive dentro de un solo documento agrupado en un arreglo de `periodos` (por ejemplo, `2026-1`) y un sub-arreglo de `materias`.
* **Consolidación de resultados:** Guarda el estado definitivo con el que cerró cada materia (`APROBADA`, `REPROBADA`, `BAJA`), su tipo de curso (`ORDINARIO`, `RECURSAMIENTO`) y el número de intento.
* **Coexistencia híbrida:** La colección no duplica nombres de profesores ni nombres de materias; conserva únicamente sus IDs de MySQL. Esto permite que el agente de IA o el backend consulte rápidamente en MongoDB *qué materias debe o ha pasado el alumno* y solo vaya a MySQL si necesita traducir esos IDs a nombres para el usuario.

## 3.3 Trayectoria académica

Mientras que el Historial Académico responde a la pregunta **"¿Qué materias ha cursado y qué calificaciones obtuvo?"**, la colección de Trayectoria Académica responde a **"¿Qué eventos han ocurrido en la vida universitaria del estudiante y en qué orden?"**.

Esta colección está diseñada bajo la filosofía de una **bitácora de auditoría (*Event Sourcing*)**:

* **Registro cronológico e inmutable:** Almacena una lista ordenada de eventos significativos (`INSCRIPCION`, `BAJA_MATERIA`, `CAMBIO_GRUPO`, `REPROBACION`, etc.) junto con la fecha exacta en que sucedieron.
* **Estructura adaptable por evento:** No todos los eventos manejan los mismos datos. Dar de baja una materia requiere registrar un `motivo`; aprobar una asignatura requiere registrar una `calificacion`. Para evitar tablas relacionales llenas de campos nulos, se utiliza el objeto flexible `detalles`, el cual permite guardar información contextual específica según el tipo de acción.
* **Trazabilidad total:** Permite rastrear la historia del alumno en el tiempo para análisis de comportamiento, detección de riesgo de deserción o auditorías administrativas.


## 3.4 Evaluaciones

Esta colección resuelve el problema de la **diversidad de criterios de evaluación entre materias y profesores**, algo que en bases de datos relacionales suele ser muy rígido y complejo de modelar.

Su lógica de negocio se basa en la **granularidad por intento y ciclo escolar**:

* **Nivel de detalle fino:** Cada documento representa el desglose de una materia específica en un periodo e intento determinado (`estudiante` + `materia` + `periodo` + `intento`).
* **Flexibilidad en el esquema pedagógico:** Un profesor de teoría puede evaluar con 3 exámenes parciales (30% cada uno) y tareas (10%), mientras que un profesor de laboratorio evalúa con prácticas (60%) y un proyecto final (40%). MongoDB permite que ambos esquemas convivan de forma natural dentro del arreglo `evaluaciones` sin modificar la estructura de la base de datos.
* **Soporte para entregables y retroalimentación:** Permite asociar ponderaciones, observaciones del docente y referencias lógicas a archivos entregados por el alumno (como reportes en PDF o código fuente) en cada rubro evaluado.

## 3.5 Validación de documentos

Aunque MongoDB es por naturaleza una base de datos de esquema flexible, permitir una estructura totalmente abierta en una arquitectura escolar o en un sistema de gestión híbrida representa un riesgo elevado de corrupción de datos. Para solucionar esto y proteger la integridad del sistema, se implementa la especificación de validación de esquemas JSON Schema directamente en el motor de la base de datos. Esta capa de validación opera en un nivel estricto, lo que significa que el motor rechaza de forma automática cualquier intento de inserción o actualización que no cumpla con las reglas definidas, lanzando un error inmediato y evitando la entrada de datos incompletos o corruptos.

Entre las reglas más importantes que impone este esquema destaca la estandarización de formatos mediante expresiones regulares, lo que asegura que campos críticos como el periodo académico mantengan siempre una nomenclatura uniforme y sin ambigüedades. De igual forma, la validación restringe los valores de texto a listas cerradas de opciones válidas para representar los estados de las materias, los tipos de cursos o la naturaleza de los eventos. Finalmente, el esquema controla los límites numéricos de las calificaciones y las ponderaciones porcentuales, garantizando que los datos sobre los que operan los procesos del backend o el Agente de IA se mantengan siempre dentro de los rangos permitidos por la institución.

---

## 3.6 Índices

Para mantener tiempos de respuesta óptimos y evitar que el motor realice escaneos completos de las colecciones conforme crece el volumen de información, el modelo incorpora una estrategia de indexación alineada a los patrones de consulta más frecuentes. Esta estructura de índices no solo acelera la localización de información, sino que refuerza reglas de negocio fundamentales directamente desde la infraestructura de la base de datos.

En primer lugar, se definen índices únicos de un solo campo y compuestos que impiden la duplicación de registros críticos. Gracias a ellos, el motor asegura que cada estudiante posea únicamente un documento de historial y un documento de trayectoria, al tiempo que evita que un alumno registre desgloses de evaluación duplicados para una misma materia dentro del mismo ciclo e intento escolar. 

Por otro lado, dado que el modelo almacena información anidada en arreglos, se implementan índices multillave sobre las rutas internas de los subdocumentos. Esto permite al sistema y al Agente de IA consultar eventos específicos o materias cursadas navegando directamente a los elementos relevantes del arreglo sin necesidad de procesar todo el documento. Por último, la indexación de los identificadores provenientes de MySQL optimiza las consultas híbridas del sistema, haciendo que la traducción y cruce de información entre la base de datos relacional y la documental se ejecute con una latencia mínima.

---

# 4. Integración entre MySQL y MongoDB

## 4.1 Relación entre ambos modelos

Aunque MySQL y MongoDB almacenan información de forma independiente, ambos modelos se relacionan mediante identificadores comunes. Los documentos almacenados en MongoDB conservan identificadores correspondientes a entidades definidas en MySQL, como `estudiante_id`, `materia_id`, `grupo_id`, `profesor_id` y `grupo_materia_id`.

Estos identificadores funcionan como referencias lógicas entre ambas bases de datos. A diferencia de las relaciones existentes dentro de MySQL, no se utilizan claves foráneas entre los dos motores, por lo que la asociación de la información se realiza desde la aplicación al ejecutar las operaciones que requieren datos de ambas fuentes.

Entre estas referencias, `grupo_materia_id` permite identificar una oferta académica concreta y relacionar la información histórica almacenada en MongoDB con el grupo, la materia y el profesor correspondientes en MySQL. De la misma forma, `estudiante_id` permite asociar los antecedentes almacenados en MongoDB con la información actual del estudiante.

Esta relación entre ambos modelos permite combinar el estado actual almacenado en MySQL con la información histórica de MongoDB. Las consultas y operaciones que utilizan esta integración se describen posteriormente en la sección de consultas híbridas.

---

# 5. Inicialización y datos de prueba

## 5.1 Inicialización de las bases de datos

La inicialización de MySQL y MongoDB se realiza mediante scripts ejecutados automáticamente al crear por primera vez sus respectivos contenedores. En ambos casos se separó la definición de la estructura de los datos utilizados para las pruebas.

Para MySQL se utilizan los archivos `01-init.sql` y `02-seed.sql`, mientras que MongoDB utiliza `01-init.js` y `02-seed.js`. Los archivos de inicialización contienen la estructura, restricciones, índices y configuración necesaria para cada base de datos, mientras que los archivos `seed` incorporan los datos utilizados durante las pruebas de la PoC.


## 5.2 Datos y escenarios de prueba

Los datos de prueba fueron diseñados para representar situaciones académicas que permitan validar tanto consultas individuales sobre cada base de datos como consultas que requieran combinar información de MySQL y MongoDB.

Además de registros básicos de estudiantes, profesores, materias, grupos e inscripciones, se incluyeron antecedentes académicos relacionados con reprobaciones, bajas, recursamientos, cambios de profesor y diferentes resultados de evaluación. De esta manera, los datos almacenados en ambos motores mantienen relaciones que permiten comprobar el funcionamiento de las consultas híbridas.

Entre los escenarios representados se encuentran estudiantes que actualmente recursan una materia previamente reprobada, alumnos que dieron de baja una materia y posteriormente volvieron a inscribirla, recursamientos con un profesor diferente y estudiantes con distintos comportamientos entre evaluaciones teóricas y prácticas.

Los datos no buscan reproducir el volumen de información de un sistema académico real, sino proporcionar casos controlados que permitan verificar el comportamiento de la arquitectura durante las pruebas.

---
> Esta sección se completará durante la implementación y validación de las consultas realizadas desde el servidor MCP.
# 6. Consultas representativas

## 6.1 Consultas en MySQL

## 6.2 Consultas en MongoDB

## 6.3 Consultas híbridas