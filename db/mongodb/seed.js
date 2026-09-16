// Dataset v1. Keep formulas synchronized with db/mysql/seed.sql.
// Fresh collections only; bounded batches, fixed dates/IDs and fixed hash seed.
const databaseName = process.env.MONGO_INITDB_DATABASE;
if (!databaseName) throw new Error('MONGO_INITDB_DATABASE no está definido');
db = db.getSiblingDB(databaseName);
for (const name of ['historial_academico', 'trayectoria_academica', 'evaluaciones']) {
    if (db.getCollection(name).countDocuments({}, {limit: 1}) !== 0)
        throw new Error(`Seed requiere colección vacía: ${name}`);
}
const SEED = 20260916;
const periods = ['2024-1','2024-2','2025-1','2025-2','2026-1','2026-2'];
const stages = [6,4,3,3,5,3,1,6,4,2];
// Canonical curriculum index -> retained subject ID (matches seed_legacy).
const legacy = new Map([[0,1],[1,2],[6,3],[7,4],[8,5],[9,6],[10,7],[11,8],
    [12,9],[13,10],[18,11],[19,12],[54,13],[55,14],[60,15],[61,16],[66,17],[67,18]]);
const hash = (id,t,slot) => (SEED + id*7919 + t*104729 + slot*1543) % 100003;
const date = (t, ending = false) => ISODate(`${2024+Math.floor(t/2)}-${t%2 ? (ending?'12-15':'08-15') : (ending?'06-20':'01-15')}T00:00:00Z`);
const snapshot = ISODate('2026-09-15T00:00:00Z');
function profile(id) {
    const section = id<=10 ? 249+id : Math.floor((id-11)/40);
    return {section, program: id<=10 ? (id<=6?1:2) : section%3+1,
        stage: id<=10 ? stages[id-1] : Math.floor(section/3)%6+1};
}
function subject(program, semester, slot) {
    const index = (program-1)*54+(semester-1)*6+slot-1;
    if (legacy.has(index)) return legacy.get(index);
    return 19+index-[...legacy.keys()].filter(key=>key<=index).length;
}
function assignment(p,t,semester,slot) {
    let group = 100+t*260+p.section;
    let gm = 1000+(t*260+p.section)*7+slot-1;
    let teacher = (p.section%70+(slot-1)*30)%180+1;
    if (p.section===252) {
        group = t===2?1:t===3?3:7;
        if (t===2 && slot<=2) { gm=slot; teacher=slot; }
        if (t===3 && slot<=3) { gm=8+slot; teacher=[9,1,5][slot-1]; }
        if (t===5 && slot<=2) gm=24+slot;
    } else if (Math.floor(p.section/70)===3) {
        const swaps = {'2/1/1':43,'2/2/2':73,'3/1/9':43,'3/2/1':73,'3/3/5':103};
        teacher = swaps[`${t}/${slot}/${teacher}`] ?? teacher;
    }
    return {grupo_materia_id:NumberInt(gm), materia_id:NumberInt(subject(p.program,slot===7?semester-1:semester,slot===7?1:slot)),
        grupo_id:NumberInt(group), profesor_id:NumberInt(teacher)};
}
const batches = {historial_academico:[],trayectoria_academica:[],evaluaciones:[]};
const counts = {historial_academico:0,trayectoria_academica:0,evaluaciones:0};
function flush(name) {
    if (!batches[name].length) return;
    db.getCollection(name).insertMany(batches[name], {ordered:true});
    counts[name] += batches[name].length;
    batches[name] = [];
}
function add(name,document) {
    batches[name].push(document);
    if (batches[name].length>=500) flush(name);
}
// Golden: 1/9 full current evaluations; 2/3 partial; 4 none; 5 failure;
// 6 second attempt; 7 new entrant; 8 advanced; 10 mixed current coverage.
for (let id=1;id<=10000;id++) {
    const p=profile(id), history=[], events=[];
    const retry=(id%17===0 || id===6) && p.stage>=2;
    const historicalRetry=(id%29===0 || id===8) && p.stage>=3 && !retry;
    for (let t=0;t<6;t++) {
        if (id===3 ? ![2,3,5].includes(t) : t<6-p.stage) continue;
        const semester=id===3 ? (t===5?3:t-1) : p.stage+t-5;
        const load=id===3 && t===2?2:id===3 && t===3?3:4+id%3;
        const results=[];
        for (let k=1;k<=load;k++) {
            const slot=((t===5 && retry) || (t===4 && historicalRetry)) && k===1?7:k;
            const refs=assignment(p,t,semester,slot), attempt=slot===7?2:1;
            const failure=(t===4 && retry && k===1) || (id===5 && t===3 && k===2) || (t===3 && historicalRetry && k===1);
            let grade=failure?5.2:Math.round((6+hash(id,t,k)%41/10)*10)/10;
            if(id===3 && t===2) grade=[9.6,9.3][k-1];
            if(id===3 && t===3) grade=[9.5,9.8,9.4][k-1];
            events.push({tipo:slot===7?'RECURSAMIENTO':'INSCRIPCION',fecha:date(t),periodo:periods[t],...refs});
            if(t<5) {
                results.push({...refs,calificacion:grade,estado:failure?'REPROBADA':'APROBADA',
                    tipo_curso:slot===7?'RECURSAMIENTO':'ORDINARIO',intento:NumberInt(attempt),fecha_cierre:date(t,true)});
                events.push({tipo:failure?'REPROBACION':'APROBACION',fecha:date(t,true),periodo:periods[t],...refs,calificacion:grade});
            }
            // 75% current coverage; golden overrides explicitly exercise all/partial/none.
            const available=t<5 || (id!==4 && (id===1 || id===9 || id===6 || (id===3?k<=2:hash(id,t,k)%4!==0)));
            if(!available) continue;
            const complete=t<5 || id===1 || id===9;
            const parts=complete?3:1+hash(id,t,k)%2;
            const assessments=[];
            for(let j=0;j<parts;j++) {
                // 30/30/40; full weighted mean equals stored final grade.
                const mark=j===0?Math.max(0,grade-0.2):j===1?Math.min(10,grade+0.2):grade;
                const assessment={tipo:['PARCIAL','LABORATORIO','PROYECTO'][j],nombre:['Primer parcial','Práctica aplicada','Proyecto integrador'][j],
                    calificacion:Math.round(mark*10)/10,ponderacion:[30,30,40][j],
                    fecha:ISODate(`${2024+Math.floor(t/2)}-${t%2?'09':'03'}-${['01','08','15'][j]}T00:00:00Z`)};
                if(j===1) assessment.detalles={lenguaje:p.program===1?'C':'Python',algoritmos:['quicksort','mergesort','heapsort'],rubrica:{criterios:['correctitud','claridad']}};
                if(j===2) {
                    assessment.detalles={motor:'MySQL',trabajo_equipo:id%2===0};
                    assessment.entregables=[{nombre:'Modelo entidad-relación',tipo:'DIAGRAMA',referencia:`proyecto_${id}_${t}_${k}/erd`},
                        {nombre:'Script SQL',tipo:'CODIGO',referencia:`proyecto_${id}_${t}_${k}/schema.sql`,version:NumberInt(1)}];
                }
                assessments.push(assessment);
            }
            // At the upper bound use equal marks, retaining an exact weighted final.
            if(grade>9.8) assessments.forEach(a=>a.calificacion=grade);
            const document={_id:ObjectId((id*100+t*10+k).toString(16).padStart(24,'0')),estudiante_id:NumberInt(id),...refs,periodo:periods[t],
                intento:NumberInt(attempt),evaluaciones:assessments,fecha_creacion:date(t),fecha_actualizacion:t===5?snapshot:date(t,true)};
            if(complete) document.calificacion_final=grade;
            add('evaluaciones',document);
        }
        if(t<5) history.push({periodo:periods[t],semestre:NumberInt(semester),materias:results});
    }
    if(history.length) add('historial_academico',{_id:ObjectId(id.toString(16).padStart(24,'0')),estudiante_id:NumberInt(id),periodos:history,
        fecha_creacion:date(periods.indexOf(history[0].periodo)),fecha_actualizacion:date(periods.indexOf(history[history.length-1].periodo),true)});
    // Chronological events, including an explicit pause for Sofia in 2026-1 (no invented grades).
    events.sort((a,b)=>a.fecha-b.fecha || Number(a.grupo_materia_id)-Number(b.grupo_materia_id));
    add('trayectoria_academica',{_id:ObjectId(id.toString(16).padStart(24,'0')),estudiante_id:NumberInt(id),eventos:events,
        fecha_creacion:events[0].fecha,fecha_actualizacion:snapshot});
}
for(const name of Object.keys(batches)) flush(name);
print(JSON.stringify(counts));
