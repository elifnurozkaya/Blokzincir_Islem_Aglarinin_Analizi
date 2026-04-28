import React, { useState } from 'react';
import './App.css';
import { Search, Activity, Box, Database } from 'lucide-react';

function App() {
  const [walletId, setWalletId] = useState('');

  return (
    <div className="app-container">
      {/* Sidebar */}
      <aside className="sidebar">
        <div className="sidebar-header">
          <Database className="icon" />
          <h2>BlockChain Analysis</h2>
        </div>
        
        <div className="search-box">
          <label>Cüzdan / İşlem Arama</label>
          <div className="input-group">
            <Search className="search-icon" size={18} />
            <input 
              type="text" 
              placeholder="Cüzdan ID girin..." 
              value={walletId}
              onChange={(e) => setWalletId(e.target.value)}
            />
          </div>
          <button className="analyze-btn">Analiz Et</button>
        </div>

        <div className="wallet-info">
          <h3>Bakiye Özeti</h3>
          <div className="info-card">
            <span className="label">Toplam Gelen</span>
            <span className="value success">+0.00 BTC</span>
          </div>
          <div className="info-card">
            <span className="label">Toplam Giden</span>
            <span className="value danger">-0.00 BTC</span>
          </div>
          <div className="info-card total">
            <span className="label">Net Bakiye</span>
            <span className="value">0.00 BTC</span>
          </div>
        </div>
      </aside>

      {/* Main Content Area */}
      <main className="main-content">
        {/* Graph Area */}
        <section className="graph-section">
          <div className="section-header">
            <Activity className="icon" />
            <h3>İşlem Ağı (Node-Link Graph)</h3>
          </div>
          <div className="graph-container">
            {/* GraphComponent will be here */}
            <div className="placeholder">Ağ grafiği burada görüntülenecek...</div>
          </div>
        </section>

        {/* Merkle Tree Area */}
        <section className="merkle-section">
          <div className="section-header">
            <Box className="icon" />
            <h3>Merkle Ağacı Doğrulaması</h3>
          </div>
          <div className="merkle-container">
             {/* MerkleTreeComponent will be here */}
             <div className="placeholder">Seçilen işlemin Merkle ağacı burada görüntülenecek...</div>
          </div>
        </section>
      </main>
    </div>
  );
}

export default App;
