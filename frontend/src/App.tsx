import { useEffect, useRef, useState } from 'react'
import { LayoutGrid, Menu, X } from 'lucide-react'
import { Sidebar } from './components/Sidebar'
import { Chat } from './components/Chat'
import { ToolsPanel } from './components/ToolsPanel'
import { Examples } from './components/Examples'
import { sendMessage } from './lib/api'
import type { Message } from './types'

export default function App() {
  const [messages, setMessages] = useState<Message[]>([])
  const [draft, setDraft] = useState('')
  const [pending, setPending] = useState(false)
  const [drawer, setDrawer] = useState<'navigation' | 'tools' | null>(null)
  const dialog = useRef<HTMLDialogElement>(null)
  const inputRef = useRef<HTMLTextAreaElement>(null)
  const request = useRef<AbortController | null>(null)

  useEffect(() => () => request.current?.abort(), [])
  useEffect(() => {
    const handler = (event: KeyboardEvent) => { if ((event.ctrlKey || event.metaKey) && event.key.toLowerCase() === 'k') { event.preventDefault(); inputRef.current?.focus() } }
    document.addEventListener('keydown', handler)
    return () => document.removeEventListener('keydown', handler)
  }, [])
  useEffect(() => { if (drawer) dialog.current?.showModal(); else dialog.current?.close() }, [drawer])

  function chooseExample(text: string) {
    setDraft(text)
    setDrawer(null)
    window.setTimeout(() => inputRef.current?.focus(), 0)
  }
  function reset() {
    request.current?.abort()
    request.current = null
    setPending(false)
    setMessages([])
    setDraft('')
    window.setTimeout(() => inputRef.current?.focus(), 0)
  }
  async function submit(text: string) {
    const trimmed = text.trim()
    if (!trimmed || request.current) return
    const controller = new AbortController()
    request.current = controller
    setMessages(previous => [...previous, { id: crypto.randomUUID(), role: 'user', content: trimmed, time: new Date() }])
    setDraft('')
    setPending(true)
    const started = performance.now()
    try {
      const result = await sendMessage(trimmed, controller.signal)
      const duration = performance.now() - started
      if (request.current !== controller) return
      setMessages(previous => [...previous, { id: crypto.randomUUID(), role: 'assistant', content: result.response, toolUsed: result.toolUsed, duration, time: new Date() }])
    } catch (error) {
      if (controller.signal.aborted || request.current !== controller) return
      const content = error instanceof TypeError ? 'No pudimos conectar con el agente. Revisa la conexión y la disponibilidad del servicio antes de reintentar.' : error instanceof Error ? error.message : 'Ocurrió un error al enviar la solicitud.'
      setMessages(previous => [...previous, { id: crypto.randomUUID(), role: 'error', content, retryMessage: trimmed, time: new Date() }])
    } finally {
      if (request.current === controller) {
        request.current = null
        setPending(false)
        window.setTimeout(() => inputRef.current?.focus(), 0)
      }
    }
  }

  return <div className="app-layout">
    <div className="desktop-sidebar"><Sidebar onNavigate={() => inputRef.current?.focus()} /></div>
    <main className="workspace">
      <div className="topbar"><button className="icon-button mobile-nav" aria-label="Abrir navegación" onClick={() => setDrawer('navigation')}><Menu size={21} /></button><button className="icon-button tools-toggle" aria-label="Abrir herramientas y ejemplos" onClick={() => setDrawer('tools')}><LayoutGrid size={21} /></button></div>
      <div className="workspace-grid">
        <header className="hero"><div className="eyebrow">AGENTE IA <span>·</span> MCP</div><h1>Tu asistente <span>académico,</span><br className="hero-break" /> potenciado por IA.</h1></header>

        <Chat messages={messages} pending={pending} draft={draft} onDraft={setDraft} onSend={submit} onReset={reset} inputRef={inputRef} />
        <aside className="right-panel" aria-label="Herramientas y ejemplos"><ToolsPanel /><Examples onSelect={chooseExample} disabled={pending} /></aside>
      </div>
    </main>
    <dialog className={`mobile-drawer ${drawer === 'navigation' ? 'navigation-drawer' : ''}`} ref={dialog} onCancel={() => setDrawer(null)} onClose={() => setDrawer(null)} onClick={event => { if (event.target === event.currentTarget) setDrawer(null) }}>
      <div className="drawer-content"><button className="icon-button drawer-close" onClick={() => setDrawer(null)} aria-label="Cerrar panel"><X size={20} /></button>
        {drawer === 'navigation' ? <Sidebar onNavigate={() => { setDrawer(null); inputRef.current?.focus() }} /> : drawer === 'tools' ? <><ToolsPanel /><Examples onSelect={chooseExample} disabled={pending} /></> : null}
      </div>
    </dialog>
  </div>
}
