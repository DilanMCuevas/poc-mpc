import { useId, useState } from 'react'
import { BookOpen, CalendarDays, ChartNoAxesCombined, ChevronDown, FileText, LayoutGrid, Search, UserRound, UsersRound } from 'lucide-react'
import { tools } from '../lib/catalog'

const icons = { student: UserRound, book: BookOpen, file: FileText, teacher: UserRound, group: UsersRound, calendar: CalendarDays, chart: ChartNoAxesCombined }
export function ToolsPanel() {
  const id = useId()
  const [search, setSearch] = useState('')
  const [expanded, setExpanded] = useState<string | null>(null)
  const filtered = tools.filter(tool => `${tool.name} ${tool.description} ${tool.source} ${tool.operation}`.toLocaleLowerCase('es').includes(search.toLocaleLowerCase('es')))
  return <section className="tools-panel glass" aria-labelledby={`${id}-title`}>
    <div className="panel-title"><span className="panel-symbol"><LayoutGrid size={24} /></span><div><h2 id={`${id}-title`}>Herramientas MCP</h2><p>13 herramientas disponibles</p></div><span className="count-badge">13</span></div>
    <label className="tool-search"><Search size={16} /><input value={search} onChange={e => setSearch(e.target.value)} placeholder="Buscar herramienta…" aria-label="Buscar herramienta MCP" /></label>
    <div className="source-legend"><span><i className="mysql-dot" />MySQL</span><span><i className="mongo-dot" />MongoDB</span><span><i className="hybrid-dot" />Híbrida</span></div>
    <div className="tool-list">
      {filtered.map(tool => { const Icon = icons[tool.icon]; const open = expanded === tool.name
        return <div key={tool.name} className={`tool-item ${open ? 'expanded' : ''}`}>
          <button className="tool-button" onClick={() => setExpanded(open ? null : tool.name)} aria-expanded={open} aria-controls={`${id}-${tool.name}`}>
            <span className={`tool-icon ${tool.source === 'MySQL' ? 'mysql' : tool.source === 'MongoDB' ? 'mongo' : 'hybrid'}`}><Icon size={20} /></span>
            <span className="tool-copy"><span>{tool.name}</span><small>{tool.description}</small></span><ChevronDown size={14} className="tool-chevron" />
          </button>
          {open && <div id={`${id}-${tool.name}`} className="tool-detail"><span className="source-tag">{tool.source}</span><span className="source-tag">{tool.operation === 'Consulta' ? 'Consulta' : `CRUD · ${tool.operation}`}</span><p>Describe tu solicitud en el chat. El agente selecciona la herramienta y solicita los datos que falten.</p></div>}
        </div>
      })}
      {filtered.length === 0 && <p className="no-tools">No hay herramientas que coincidan con tu búsqueda.</p>}
    </div>
    <div className="tools-footnote">Catálogo MCP <span>·</span> Selección automática por el agente</div>
  </section>
}
