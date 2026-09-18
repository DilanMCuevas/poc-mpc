import { useId } from 'react'
import { ArrowUpRight, Lightbulb } from 'lucide-react'
import { examples } from '../lib/catalog'

export function Examples({ onSelect, disabled }: { onSelect: (text: string) => void; disabled: boolean }) {
  const id = useId()
  return <section className="examples-panel glass" aria-labelledby={id}>
    <div className="panel-title"><Lightbulb className="bulb-icon" size={28} /><div><h2 id={id}>Ejemplos de consultas</h2></div></div>
    <div className="example-list">{examples.slice(0, 3).map(example => <button key={example.text} disabled={disabled} onClick={() => onSelect(example.text)} title={example.text}>
      <span>{example.text}</span><ArrowUpRight size={16} />
    </button>)}</div>
  </section>
}
