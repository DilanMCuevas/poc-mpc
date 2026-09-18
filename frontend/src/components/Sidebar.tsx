import { BarChart3, BookOpen, CalendarDays, Database, GraduationCap, MessageCircle, Network, Server, Sparkles, UserRound, UsersRound } from 'lucide-react'

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
        <Icon size={23} /><span>{label}</span>
      </button>)}
    </nav>
    <details className="system-panel">
      <summary>Sistema</summary>
      <ul>{services.map(({ label, icon: Icon }) => <li key={label}><Icon size={15} /><span>{label}</span></li>)}</ul>
    </details>
  </aside>
}
