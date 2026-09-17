export interface ChatResponse {
  response: string
  toolUsed: string | null
}

export interface Message {
  id: string
  role: 'user' | 'assistant' | 'error'
  content: string
  time: Date
  toolUsed?: string | null
  duration?: number
  retryMessage?: string
}

export type Source = 'MySQL' | 'MongoDB' | 'Híbrida'
export interface McpTool {
  name: string
  description: string
  source: Source
  operation: 'Consulta' | 'Crear' | 'Actualizar' | 'Eliminar'
  icon: 'student' | 'book' | 'file' | 'teacher' | 'group' | 'calendar' | 'chart'
}
