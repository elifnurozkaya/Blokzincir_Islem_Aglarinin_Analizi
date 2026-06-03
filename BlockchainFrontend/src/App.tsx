import { useState, useCallback } from 'react';
import './App.css';
import { Search, Activity, Box, Database, Plus, Trash2, CheckCircle, XCircle, Loader } from 'lucide-react';
import ForceGraph2D from 'react-force-graph-2d';

const API = 'http://localhost:5084/api/blockchain';

interface Transaction {
  transactionId: string;
  fromWalletId: string;
  toWalletId: string;
  amount: number;
  timestamp: string;
}

interface WalletInfo {
  walletId: string;
  balance: number;
}

function App() {
  const [walletId, setWalletId] = useState('');
  const [algorithm, setAlgorithm] = useState<'bfs' | 'dfs'>('bfs');
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [walletInfo, setWalletInfo] = useState<WalletInfo | null>(null);
  const [transactions, setTransactions] = useState<Transaction[]>([]);
  const [totalIn, setTotalIn] = useState(0);
  const [totalOut, setTotalOut] = useState(0);
  const [merkleRoot, setMerkleRoot] = useState<string | null>(null);
  const [verified, setVerified] = useState<boolean | null>(null);
  const [showWalletForm, setShowWalletForm] = useState(false);
  const [newWalletId, setNewWalletId] = useState('');
  const [showTxForm, setShowTxForm] = useState(false);
  const [txForm, setTxForm] = useState({ transactionId: '', fromWalletId: '', toWalletId: '', amount: '' });

  const handleAnalyze = useCallback(async () => {
    if (!walletId.trim()) return;
    setLoading(true); setError(null); setTransactions([]); setWalletInfo(null); setMerkleRoot(null); setVerified(null);
    try {
      const wRes = await fetch(`${API}/wallet/${walletId.trim()}`);
      if (!wRes.ok) throw new Error((await wRes.json()).message ?? 'Cuzdan bulunamadi.');
      setWalletInfo(await wRes.json());
      const txRes = await fetch(`${API}/${algorithm}/${walletId.trim()}`);
      const txData = txRes.ok ? await txRes.json() : { transactions: [] };
      const txs: Transaction[] = txData.transactions ?? [];
      setTransactions(txs);
      const id = walletId.trim();
      setTotalIn(txs.filter(t => t.toWalletId === id).reduce((s, t) => s + t.amount, 0));
      setTotalOut(txs.filter(t => t.fromWalletId === id).reduce((s, t) => s + t.amount, 0));
      const mRes = await fetch(`${API}/merkle/root`);
      if (mRes.ok) setMerkleRoot((await mRes.json()).merkleRoot);
    } catch (e: any) {
      setError(e.message ?? 'Bir hata olustu.');
    } finally {
      setLoading(false);
    }
  }, [walletId, algorithm]);

  const handleVerify = useCallback(async () => {
    try {
      const res = await fetch(`${API}/merkle/verify`, { method: 'POST' });
      setVerified((await res.json()).isValid);
    } catch { setError('Dogrulama sirasinda hata olustu.'); }
  }, []);

  const handleAddWallet = useCallback(async () => {
    if (!newWalletId.trim()) return;
    try {
      const res = await fetch(`${API}/wallet`, { method: 'POST', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify({ walletId: newWalletId.trim(), balance: 0 }) });
      if (!res.ok) throw new Error((await res.json()).message);
      setNewWalletId(''); setShowWalletForm(false);
    } catch (e: any) { setError(e.message ?? 'Cuzdan eklenemedi.'); }
  }, [newWalletId]);

  const handleDeleteWallet = useCallback(async () => {
    if (!walletId.trim()) return;
    try {
      await fetch(`${API}/wallet/${walletId.trim()}`, { method: 'DELETE' });
      setWalletInfo(null); setTransactions([]); setWalletId('');
    } catch (e: any) { setError(e.message ?? 'Cuzdan silinemedi.'); }
  }, [walletId]);

  const handleAddTransaction = useCallback(async () => {
    const { transactionId, fromWalletId, toWalletId, amount } = txForm;
    if (!transactionId || !fromWalletId || !toWalletId || !amount) return;
    try {
      const res = await fetch(`${API}/transaction`, { method: 'POST', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify({ transactionId, fromWalletId, toWalletId, amount: parseFloat(amount), timestamp: new Date().toISOString() }) });
      if (!res.ok) throw new Error((await res.json()).message);
      setTxForm({ transactionId: '', fromWalletId: '', toWalletId: '', amount: '' }); setShowTxForm(false);
    } catch (e: any) { setError(e.message ?? 'Islem eklenemedi.'); }
  }, [txForm]);

  const renderGraph = () => {
    if (transactions.length === 0) return <div className="placeholder">Bir cuzdan ID girin ve Analiz Et e basin</div>;
    
    const nodeSet = new Set<string>();
    transactions.forEach(t => { nodeSet.add(t.fromWalletId); nodeSet.add(t.toWalletId); });
    
    const nodes = Array.from(nodeSet).map(id => ({ id, name: id }));
    
    // Aynı cüzdanlar arasındaki birden fazla işlemi ayırt edebilmek için eğim (curvature) hesapla
    const pairMap = new Map<string, number>();
    const links = transactions.map(t => {
      const pairId = [t.fromWalletId, t.toWalletId].sort().join('-');
      const count = pairMap.get(pairId) || 0;
      pairMap.set(pairId, count + 1);
      // count 0 ise düz (0), sonraki aynı oklarda ise kavisli (0.2, -0.2, 0.4 vb.) yap
      const curvature = count === 0 ? 0 : (count % 2 === 0 ? 1 : -1) * 0.2 * Math.ceil(count / 2);
      
      return { source: t.fromWalletId, target: t.toWalletId, amount: t.amount, name: `${t.amount} BTC`, curvature };
    });

    return (
      <div style={{ width: '100%', height: '100%', overflow: 'hidden', borderRadius: '12px', display: 'flex', alignItems: 'center', justifyContent: 'center' }}>
        <ForceGraph2D
          width={800}
          height={400}
          graphData={{ nodes, links }}
          nodeLabel="name"
          nodeColor={node => node.id === walletId ? '#ffffff' : '#ea580c'}
          nodeRelSize={6}
          linkColor={(link: any) => {
            const sourceId = link.source?.id || link.source;
            const targetId = link.target?.id || link.target;
            if (sourceId === walletId) return '#ef4444'; // Giden ok kırmızı
            if (targetId === walletId) return '#22c55e'; // Gelen ok yeşil
            return '#3b82f6'; // Diğerleri mavi
          }}
          linkDirectionalArrowLength={5}
          linkDirectionalArrowRelPos={1}
          linkCurvature="curvature"
          linkLabel="name"
          enableNodeDrag={true}
          backgroundColor="#0b0f19"
        />
      </div>
    );
  };

  const net = totalIn - totalOut;

  return (
    <div className="app-container">
      <aside className="sidebar">
        <div className="sidebar-header">
          <Database className="icon" />
          <h2>BlockChain Analysis</h2>
        </div>
        <div className="search-box">
          <label>Cüzdan / İşlem Arama</label>
          <div className="input-group">
            <Search className="search-icon" size={18} />
            <input type="text" placeholder="Cüzdan ID girin..." value={walletId} onChange={e => setWalletId(e.target.value)} onKeyDown={e => e.key === 'Enter' && handleAnalyze()} />
          </div>
          <div className="algo-toggle">
            <button className={`algo-btn ${algorithm === 'bfs' ? 'active' : ''}`} onClick={() => setAlgorithm('bfs')}>BFS</button>
            <button className={`algo-btn ${algorithm === 'dfs' ? 'active' : ''}`} onClick={() => setAlgorithm('dfs')}>DFS</button>
          </div>
          <button className="analyze-btn" onClick={handleAnalyze} disabled={loading}>
            {loading ? <Loader size={15} className="spin" /> : <Search size={15} />}
            {loading ? 'Analiz ediliyor...' : 'Analiz Et'}
          </button>
          {error && <div className="error-msg">{error}</div>}
        </div>
        <div className="wallet-info">
          <h3>Bakiye Özeti</h3>
          {walletInfo && <div className="wallet-id-badge">{walletInfo.walletId}</div>}
          <div className="info-card">
            <span className="label">Toplam Gelen</span>
            <span className="value success">+{totalIn.toFixed(2)} BTC</span>
          </div>
          <div className="info-card">
            <span className="label">Toplam Giden</span>
            <span className="value danger">-{totalOut.toFixed(2)} BTC</span>
          </div>
          <div className="info-card total">
            <span className="label">Net Bakiye</span>
            <span className={`value ${net >= 0 ? 'success' : 'danger'}`}>{net >= 0 ? '+' : ''}{net.toFixed(2)} BTC</span>
          </div>
        </div>
        <div className="quick-actions">
          <h3>Hızlı İşlemler</h3>
          <button className="action-btn" onClick={() => setShowWalletForm(v => !v)}><Plus size={13} /> Cüzdan Ekle</button>
          {showWalletForm && (
            <div className="mini-form">
              <input placeholder="Cüzdan ID" value={newWalletId} onChange={e => setNewWalletId(e.target.value)} />
              <button className="confirm-btn" onClick={handleAddWallet}>Ekle</button>
            </div>
          )}
          <button className="action-btn" onClick={() => setShowTxForm(v => !v)}><Plus size={13} /> İşlem Ekle</button>
          {showTxForm && (
            <div className="mini-form">
              <input placeholder="İşlem ID" value={txForm.transactionId} onChange={e => setTxForm(p => ({ ...p, transactionId: e.target.value }))} />
              <input placeholder="Gönderen" value={txForm.fromWalletId} onChange={e => setTxForm(p => ({ ...p, fromWalletId: e.target.value }))} />
              <input placeholder="Alıcı" value={txForm.toWalletId} onChange={e => setTxForm(p => ({ ...p, toWalletId: e.target.value }))} />
              <input placeholder="Miktar" value={txForm.amount} onChange={e => setTxForm(p => ({ ...p, amount: e.target.value }))} type="number" />
              <button className="confirm-btn" onClick={handleAddTransaction}>Ekle</button>
            </div>
          )}
          {walletInfo && <button className="action-btn danger" onClick={handleDeleteWallet}><Trash2 size={13} /> Cüzdanı Sil</button>}
        </div>
      </aside>
      <main className="main-content">
        <section className="graph-section">
          <div className="section-header">
            <Activity className="icon" />
            <h3>İşlem Ağı — {algorithm.toUpperCase()} Görünümü</h3>
            {transactions.length > 0 && <span className="badge">{transactions.length} işlem</span>}
          </div>
          <div className="graph-container">{renderGraph()}</div>
        </section>
        <section className="merkle-section">
          <div className="section-header">
            <Box className="icon" />
            <h3>Merkle Ağacı Doğrulaması</h3>
            {merkleRoot && <button className="verify-btn" onClick={handleVerify}>Bütünlüğü Doğrula</button>}
          </div>
          <div className="merkle-container">
            {!merkleRoot ? (
              <div className="placeholder">Seçilen işlemin Merkle ağacı burada görüntülenecek...</div>
            ) : (
              <div className="merkle-info">
                <div className="merkle-root-box">
                  <span className="merkle-label">Merkle Root</span>
                  <code className="merkle-hash">{merkleRoot}</code>
                </div>
                {verified !== null && (
                  <div className={`integrity-badge ${verified ? 'valid' : 'invalid'}`}>
                    {verified ? <><CheckCircle size={16} /> Veri bütünlüğü doğrulandı</> : <><XCircle size={16} /> UYARI: Bütünlük bozulmuş!</>}
                  </div>
                )}
                <div className="merkle-visual">
                  <div className="merkle-node root-node">ROOT</div>
                  <div className="merkle-branches">
                    <div className="merkle-node branch-node">SOL</div>
                    <div className="merkle-node branch-node">SAĞ</div>
                  </div>
                  <div className="merkle-leaves">
                    {transactions.slice(0, 4).map((_, i) => <div key={i} className="merkle-node leaf-node">TX{i + 1}</div>)}
                    {transactions.length > 4 && <div className="merkle-node leaf-node">+{transactions.length - 4}</div>}
                  </div>
                </div>
              </div>
            )}
          </div>
        </section>
      </main>
    </div>
  );
}

export default App;
