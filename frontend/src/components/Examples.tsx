import { useId } from 'react'
import { ArrowUpRight, Lightbulb } from 'lucide-react'
import { examples } from '../lib/catalog'

export function Examples({ onSelect, disabled }: { onSelect: (text: string) => void; disabled: boolean }) {
  const id = useId()
  return <section className="examples-panel glass" aria-labelledby={id}>
    <div className="panel-title"><Lightbulb className="bulb-icon" size={28} /><div><h2 id={id}>Ejemplos de consultas</h2><p>Una idea para empezar</p></div></div>
    <div className="example-list">{examples.map(example => <button key={example.text} disabled={disabled} onClick={() => onSelect(example.text)} title={example.text}>
      <span>{example.label}</span><ArrowUpRight size={16} />
    </button>)}</div>
    <p className="examples-hint">Elige un ejemplo y revísalo antes de enviar.</p>
  </section>
}
