import type { McpTool } from '../types'

export const tools: McpTool[] = [
  { name: 'consultar_estudiante', description: 'Información general de un estudiante', source: 'MySQL', operation: 'Consulta', icon: 'student' },
  { name: 'consultar_historial', description: 'Historial y resultados académicos', source: 'MongoDB', operation: 'Consulta', icon: 'book' },
  { name: 'consultar_expediente', description: 'Expediente académico completo', source: 'Híbrida', operation: 'Consulta', icon: 'file' },
  { name: 'consultar_profesor', description: 'Datos de un profesor', source: 'MySQL', operation: 'Consulta', icon: 'teacher' },
  { name: 'consultar_materia', description: 'Información de una materia', source: 'MySQL', operation: 'Consulta', icon: 'book' },
  { name: 'consultar_grupo', description: 'Materias y profesores de un grupo', source: 'MySQL', operation: 'Consulta', icon: 'group' },
  { name: 'consultar_horario_estudiante', description: 'Horario del estudiante por periodo', source: 'MySQL', operation: 'Consulta', icon: 'calendar' },
  { name: 'consultar_evaluaciones', description: 'Evaluaciones y calificaciones', source: 'MongoDB', operation: 'Consulta', icon: 'chart' },
  { name: 'consultar_carga_academica', description: 'Inscripciones y evaluaciones del periodo', source: 'Híbrida', operation: 'Consulta', icon: 'file' },
  { name: 'registrar_estudiante', description: 'Registro de un nuevo estudiante', source: 'MySQL', operation: 'Crear', icon: 'student' },
  { name: 'actualizar_estudiante', description: 'Actualización parcial de un estudiante', source: 'MySQL', operation: 'Actualizar', icon: 'student' },
  { name: 'eliminar_estudiante', description: 'Eliminación controlada de un estudiante', source: 'MySQL', operation: 'Eliminar', icon: 'student' },
  { name: 'registrar_evaluacion', description: 'Registro de componentes de evaluación', source: 'MongoDB', operation: 'Crear', icon: 'chart' },
]

export const examples = [
  { label: 'Consulta los datos de un estudiante', text: 'Consulta al estudiante con boleta 2025000003' },
  { label: 'Explora un expediente completo', text: 'Muéstrame el expediente completo del estudiante con boleta 2025000003' },
  { label: 'Revisa las evaluaciones del periodo', text: '¿Qué evaluaciones tiene el estudiante 3 en el periodo 2026-2?' },
  { label: 'Consulta el horario de un estudiante', text: 'Muéstrame el horario del estudiante con boleta 2025000003 en el periodo 2026-2' },
]
