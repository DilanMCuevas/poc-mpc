## Tabla de contenido

# 1. Introducción

El desarrollo y la investigación en el campo de la inteligencia artificial ha permitido gener asistentes capaces de comprender instrucciones escritas en lenguaje natural y ejecutar tareas de manera cada vez más autónoma. Uno de los principales retos consiste en conectar estos modelos con sistemas externos de forma segura, estructurada y reutilizable. Así es como surge el Model Context Protocol (MCP), un protocolo diseñado para estandarizar la comunicación entre un modelo de lenguaje y las herramientas o servicios que necesita utilizar para completar una solicitud.

Este proyecto consiste en el desarrollo de una Prueba de Concepto (Proof of Concept o PoC) de un agente inteligente capaz de interactuar con un servidor MCP para realizar operaciones sobre dos motores de bases de datos diferentes: una base de datos relacional usando MySQL y una base de datos no relacional utilizando MongoDB. A través de una interfaz de chat el usuario podrá realizar solicitudes en lenguaje natural, mientras que el agente interpretará la intención del mensaje y ejecutará de forma autónoma las herramientas necesarias para consultar, registrar, modificar o eliminar información.

Como escenario de prueba se utilizará un sistema académico conformado por profesores, alumnos, materias, grupos e historial académico. Este dominio no pretende representar un sistema escolar completo, sino proporcionar un conjunto de entidades suficientemente representativas para demostrar el funcionamiento de una arquitectura híbrida, donde cada tecnología se utilice de acuerdo con sus fortalezas. La información estructurada y las relaciones entre entidades serán administradas mediante MySQL, mientras que MongoDB almacenará información documental y flexible relacionada con el historial académico.

Además del funcionamiento del agente, el proyecto busca documentar las decisiones de diseño adoptadas durante el desarrollo, justificando la selección de tecnologías, la estructura de las bases de datos y la arquitectura utilizada. De esta manera, la PoC no solo demuestra que la integración es técnicamente viable, sino también el criterio empleado para construir una solución organizada, escalable y fácil de mantener.


## 1.1 Alcance

El desarrollo de este proyecto tiene como finalidad construir una prueba de concepto funcional que permita demostrar la comunicación entre un agente basado en inteligencia artificial, un servidor MCP y dos motores de bases de datos con características diferentes. El alcance se limita a validar el funcionamiento de la arquitectura propuesta y no al desarrollo de un sistema académico completo o listo para utilizarse en un entorno de producción.

Como parte del proyecto se implementará una interfaz de chat sencilla desde la cual el usuario podrá realizar solicitudes utilizando lenguaje natural. Dichas solicitudes serán interpretadas por el agente, quien decidirá qué herramientas del servidor MCP utilizar para ejecutar las operaciones correspondientes sobre MySQL o MongoDB. Entre las operaciones consideradas se encuentran el registro, consulta, actualización y eliminación de información relacionada con profesores, estudiantes, materias, grupos e historial académico.

También forma parte del alcance el diseño e implementación de ambas bases de datos, procurando que cada una almacene el tipo de información para el cual resulta más adecuada. El objetivo no es duplicar datos entre motores, sino aprovechar las ventajas que ofrece cada tecnología dentro de una arquitectura híbrida. De igual forma, se documentarán las decisiones tomadas durante el proceso de diseño e implementación con el propósito de justificar la elección de tecnologías, la organización del proyecto y la estructura de los datos.

Por otra parte, quedan fuera del alcance aspectos propios de un sistema empresarial, como mecanismos avanzados de autenticación, administración de usuarios y roles, alta disponibilidad, balanceo de carga, auditorías, optimización para grandes volúmenes de información o interfaces gráficas complejas. Debido a que se trata de una prueba de concepto, el enfoque principal estará centrado en demostrar la integración técnica entre los distintos componentes y el correcto funcionamiento del flujo completo de comunicación entre el usuario, el agente, el servidor MCP y las bases de datos.


## 1.2 Objetivos

Objetivo general

Desarrollar una Prueba de Concepto (Proof of Concept, PoC) basada en el protocolo Model Context Protocol (MCP), capaz de interpretar instrucciones en lenguaje natural mediante un agente de inteligencia artificial y ejecutar operaciones sobre una base de datos relacional (MySQL) y una base de datos no relacional (MongoDB), con el propósito de demostrar la viabilidad de una arquitectura híbrida para la gestión de información académica.

Objetivo específicos

Diseñar e implementar una arquitectura híbrida que integre un agente de inteligencia artificial, un servidor MCP, MySQL y MongoDB, permitiendo la comunicación entre todos los componentes del sistema.
Implementar herramientas (Tools) dentro del servidor MCP que permitan realizar operaciones de consulta, registro, actualización y eliminación de información sobre profesores, estudiantes, materias, grupos e historial académico.
Validar el funcionamiento del sistema mediante una interfaz de chat que permita al usuario interactuar utilizando lenguaje natural, comprobando que el agente interpreta las solicitudes y ejecuta correctamente las operaciones correspondientes en la base de datos adecuada.

---

# 2. Análisis del problema

## 2.1 Planteamiento del problema

## 2.2 Requerimientos funcionales

## 2.3 Requerimientos no funcionales

## 2.4 Restricciones

---

# 3. Diseño del sistema

## 3.1 Arquitectura general

## 3.2 Componentes del sistema

Separación correcta de responsabilidades
Aplicación 1: Agent

Responsabilidades:

Recibir mensajes del usuario
Mantener contexto de conversación
Hablar con Ollama/Qwen
Descubrir las tools del MCP Server
Decidir cuándo invocar una tool
Enviar parámetros a la tool
Recibir resultados
Construir la respuesta final

No debería tener:

MySqlConnector
MongoDB.Driver
Consultas SQL
Consultas Mongo
Credenciales de bases de datos
Aplicación 2: MCP.Server

Responsabilidades:

Exponer tools mediante MCP
Validar parámetros
Ejecutar lógica de cada tool
Consultar MySQL
Consultar MongoDB
Combinar resultados híbridos
Devolver respuestas estructuradas

No debería tener:

Interfaz web
Historial conversacional
Prompts generales del agente
Decisiones del LLM
Lógica para conversar con el usuario

## 3.3 Flujo de funcionamiento
Tool
Recibe la petición MCP y valida parámetros básicos.

Service
Ejecuta la lógica y consulta la base de datos.

Response
Define exactamente qué información se devuelve.

## 3.4 Tecnologías utilizadas

## 3.5 Estructura del proyecto

---

# 4. Implementación

## 4.1 Configuración del entorno

## 4.2 Configuración del repositorio

## 4.3 Configuración con Docker Compose

## 4.4 Desarrollo de los componentes
Usaremos:

Organización principal por funcionalidad
Controllers solamente en Agent, porque recibe HTTP del frontend
Services solamente cuando exista lógica u orquestación
Repositories en MCP.Server, porque ahí se accede a las bases
Integrations en Agent para Ollama y MCP
No usaremos una carpeta global Services/ ni una carpeta global Repositories/

## 4.5 Integración del sistema

---

# 5. Pruebas

## 5.1 Planeación de pruebas

## 5.2 Casos de prueba

## 5.3 Resultados

---

# 6. Conclusiones

## 6.1 Conclusiones

## 6.2 Trabajo futuro