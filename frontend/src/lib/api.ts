import type { ChatResponse } from '../types'

const API_BASE_URL = (import.meta.env.VITE_API_BASE_URL?.trim() || 'http://localhost:5062').replace(/\/+$/, '')

export async function sendMessage(message: string, signal: AbortSignal): Promise<ChatResponse> {
  const result = await fetch(`${API_BASE_URL}/chat`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ message }),
    signal,
  })
  if (!result.ok) throw new Error('No pudimos completar la solicitud. Inténtalo de nuevo en un momento.')
  const data: unknown = await result.json()
  if (!data || typeof data !== 'object' || !('response' in data) || typeof data.response !== 'string'
    || !('toolUsed' in data) || (data.toolUsed !== null && typeof data.toolUsed !== 'string')) {
    throw new Error('La respuesta recibida no tiene el formato esperado. Puedes volver a intentarlo.')
  }
  return { response: data.response, toolUsed: data.toolUsed }
}
