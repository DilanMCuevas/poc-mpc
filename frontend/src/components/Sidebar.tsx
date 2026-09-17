import { BarChart3, BookOpen, CalendarDays, CircleDot, Database, GraduationCap, MessageCircle, Network, Server, Sparkles, UserRound, UsersRound } from 'lucide-react'

const navigation = [
  { label: 'Chat', icon: MessageCircle }, { label: 'Estudiantes', icon: UsersRound },
  { label: 'Profesores', icon: UserRound }, { label: 'Materias', icon: BookOpen },
  { label: 'Grupos', icon: UsersRound }, { label: 'Horarios', icon: CalendarDays },
  { label: 'Reportes', icon: BarChart3 },
]
const services = [
  { label: 'Agent', icon: Sparkles }, { label: 'MCP Server', icon: Network },
  { label: 'Ollama (Qwen3)', icon: Server }, { label: 'MySQL', icon: Database },
  { label: 'MongoDB', icon: Database },
]

export function Sidebar({ onNavigate }: { onNavigate: () => void }) {
  return <aside className="sidebar">
    <a className="brand" href="#chat" onClick={onNavigate} aria-label="MCP Academic, ir al chat">
      <span className="brand-mark"><GraduationCap size={43} strokeWidth={1.6} /></span>
      <span><strong>MCP Academic</strong><small>Datos. Contexto. Decisiones.</small></span>
    </a>
    <nav className="navigation" aria-label="Navegación principal">
      {navigation.map(({ label, icon: Icon }, index) => <button key={label}
        className={`nav-item ${index === 0 ? 'active' : ''}`} disabled={index !== 0}
        title={index !== 0 ? `${label} · Próximamente` : 'Chat'}
        aria-current={index === 0 ? 'page' : undefined} onClick={onNavigate}>
        <Icon size={23} /><span>{label}</span>{index !== 0 && <small>Pronto</small>}
      </button>)}
    </nav>
    <div className="system-panel">
      <h3><CircleDot size={15} /> Sistema</h3>
      <p>Servicios configurados</p>
      <ul>{services.map(({ label, icon: Icon }) => <li key={label}><Icon size={15} /><span>{label}</span><span className="neutral-dot" /></li>)}</ul>
      <small>Estado en vivo no disponible</small>
    </div>
    <footer className="sidebar-footer"><span className="workspace-icon"><GraduationCap size={22} /></span>
      <span><strong>Espacio académico</strong><small>Asistente de datos</small></span>
    </footer>
  </aside>
}
