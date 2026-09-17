import { useEffect, useRef, useState } from 'react'
import type { RefObject } from 'react'
import Markdown from 'react-markdown'
import { ArrowRight, CheckCheck, CircleAlert, Clock3, Ellipsis, GraduationCap, MessageSquare, Plus, RotateCcw, Send, Sparkles, UserRound, X } from 'lucide-react'
import type { Message } from '../types'

interface ChatProps {
  messages: Message[]
  pending: boolean
  draft: string
  onDraft: (value: string) => void
  onSend: (text: string) => void
  onReset: () => void
  inputRef: RefObject<HTMLTextAreaElement | null>
}
const time = (date: Date) => date.toLocaleTimeString('es-MX', { hour: '2-digit', minute: '2-digit' })

export function Chat({ messages, pending, draft, onDraft, onSend, onReset, inputRef }: ChatProps) {
  const bottom = useRef<HTMLDivElement>(null)
  const [infoOpen, setInfoOpen] = useState(false)
  useEffect(() => { bottom.current?.scrollIntoView({ behavior: window.matchMedia('(prefers-reduced-motion: reduce)').matches ? 'auto' : 'smooth', block: 'end' }) }, [messages, pending])
  useEffect(() => {
    const input = inputRef.current
    if (input) { input.style.height = 'auto'; input.style.height = `${Math.min(input.scrollHeight, 128)}px` }
  }, [draft, inputRef])

  return <section className="chat-shell glass" id="chat" aria-labelledby="chat-title">
    <header className="chat-header">
      <span className="chat-emblem"><GraduationCap size={28} /></span>
      <div className="chat-heading"><h2 id="chat-title">Chat <span className="chat-label">Académico</span></h2><p>Modelo: <span>Qwen3:8b</span><i />MCP Tools: <span>13</span></p></div>
      <button className="new-chat secondary-button" onClick={onReset}><Plus size={15} /><span>Nueva conversación</span></button>
      <div className="chat-menu"><button className="icon-button" aria-label="Información de la conversación" aria-expanded={infoOpen} onClick={() => setInfoOpen(!infoOpen)}><Ellipsis size={21} /></button>
        {infoOpen && <div className="info-popover"><button className="close-info" onClick={() => setInfoOpen(false)} aria-label="Cerrar información"><X size={15} /></button><strong>Sobre esta conversación</strong><p>El historial es local y se limpia al recargar. Cada solicitud se envía de forma independiente: incluye los datos necesarios en cada mensaje.</p><p>El agente puede consultar y modificar datos según tu solicitud.</p></div>}
      </div>
    </header>
    <div className="conversation" role="log" aria-label="Conversación" aria-live="polite" aria-relevant="additions text" aria-busy={pending}>
      {messages.length === 0 && <div className="welcome">
        <div className="welcome-emblem"><GraduationCap size={44} strokeWidth={1.4} /><span><Sparkles size={14} /></span></div>
        <span className="eyebrow">TU INFORMACIÓN, CONECTADA</span>
        <h3>Una pregunta.<br /><span>Todo el contexto.</span></h3>
        <p>Explora información académica y realiza operaciones<br className="desktop-break" /> con una conversación en lenguaje natural.</p>
        <div className="welcome-capabilities"><span><MessageSquare size={16} /> Pregunta con naturalidad</span><ArrowRight size={14} /><span><Sparkles size={16} /> El agente conecta los datos</span></div>
        <div className="welcome-note">Comienza escribiendo abajo o elige un ejemplo a la derecha.</div>
      </div>}
      {messages.map(message => <article key={message.id} className={`message message-${message.role}`}>
        <span className="message-avatar" aria-hidden="true">{message.role === 'user' ? <UserRound size={20} /> : message.role === 'error' ? <CircleAlert size={21} /> : <GraduationCap size={23} />}</span>
        <div className="message-body">
          <div className="message-author">{message.role === 'user' ? 'Tú' : message.role === 'error' ? 'No se pudo completar la solicitud' : 'MCP Academic'}</div>
          {message.role === 'assistant' ? <div className="markdown"><Markdown components={{ a: ({ children, href }) => <a href={href} target="_blank" rel="noopener noreferrer">{children}</a> }}>{message.content}</Markdown></div> : <div className="plain-text">{message.content}</div>}
          <div className="message-meta">
            {message.role === 'assistant' && <span className={`tool-result ${message.toolUsed ? '' : 'no-tool'}`}><Sparkles size={12} />{message.toolUsed ? `MCP · ${message.toolUsed}` : 'Respuesta del agente · sin tool'}</span>}
            {message.duration !== undefined && <span className="latency"><Clock3 size={11} />{(message.duration / 1000).toFixed(2)} s</span>}
            <time dateTime={message.time.toISOString()}>{time(message.time)}</time>
            {message.role === 'user' && <CheckCheck size={14} />}
          </div>
          {message.role === 'error' && message.retryMessage && <button className="retry-button" disabled={pending} onClick={() => onSend(message.retryMessage!)}><RotateCcw size={13} />Reintentar solicitud</button>}
        </div>
      </article>)}
      {pending && <div className="message message-assistant processing" role="status"><span className="message-avatar"><GraduationCap size={23} /></span><div className="processing-card"><span className="processing-title">Preparando tu respuesta<span className="thinking-dots"><i /><i /><i /></span></span><div className="processing-track"><span /><span /><span /></div><small>El agente está procesando tu solicitud.</small></div></div>}
      <div ref={bottom} />
    </div>
    <form className="composer" onSubmit={e => { e.preventDefault(); onSend(draft) }}>
      <div className="composer-row"><span className="composer-spark" aria-hidden="true"><Sparkles size={21} /></span><div className="input-wrap"><textarea ref={inputRef} value={draft} disabled={pending} onChange={e => onDraft(e.target.value)} rows={1} placeholder="Escribe tu consulta aquí…" aria-label="Tu consulta académica" onKeyDown={e => { if (e.key === 'Enter' && !e.shiftKey && !e.nativeEvent.isComposing) { e.preventDefault(); onSend(draft) } }} /><span className="input-mode">MCP <span className="neutral-dot" /></span></div><button type="submit" className="send-button" disabled={pending || !draft.trim()} aria-label="Enviar consulta"><Send size={21} /></button></div>
      <div className="composer-note"><span>El agente seleccionará automáticamente la herramienta adecuada.</span><span><kbd>↵</kbd> Enviar <span className="shortcut-divider">·</span> <kbd>⇧ ↵</kbd> Nueva línea</span></div>
    </form>
  </section>
}
