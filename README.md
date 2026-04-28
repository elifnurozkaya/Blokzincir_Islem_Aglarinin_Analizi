# Blokzincir_Islem_Aglarinin_Analizi

Bu proje, blokzincir üzerindeki cüzdan hareketlerini ve fon akışlarını veri yapıları (Yönlü Graf, Merkle Ağacı ve Hash Table) kullanarak analiz etmek amacıyla geliştirilmektedir. Sistem, cüzdanlar arası para transferlerini modelleyerek belirli adreslerin işlem geçmişini takip etmeyi ve veri bütünlüğünü doğrulamayı sağlar.

## Ekip İçin Kurulum ve Çalıştırma Rehberi

Projeyi yerelinize (`git pull` veya `git clone` ile) çektikten sonra çalıştırmak için iki farklı servisi (Frontend ve Backend) ayrı ayrı ayağa kaldırmanız gerekmektedir.

### 1. Backend (C# Veri Yapıları) Nasıl Çalıştırılır?
Zorunlu veri yapıları algoritmaları bu projede yer almaktadır.
* Tercih ettiğiniz editörde (Visual Studio, VS Code veya Rider) `BlockChainAnalysis.sln` çözüm dosyasını açın.
* Editör üzerinden projeyi doğrudan "Run/Start" tuşuna basarak başlatın.
* *(Alternatif olarak terminalden `cd BlockChainAnalysis` klasörüne girip `dotnet run` komutuyla da çalıştırabilirsiniz.)*
* Proje şimdilik boş bir şablon olduğu için sadece derlenecek ve hata vermeden çalışacaktır.

### 2. Frontend (React Arayüzü) Nasıl Çalıştırılır?
Grafikleri ve Merkle ağacını görselleştireceğimiz kullanıcı arayüzü bu kısımdadır. Bilgisayarınızda **Node.js** kurulu olmalıdır.
1. Terminali açın ve Frontend klasörüne girin:
   ```bash
   cd BlockchainFrontend
   ```
2. Gerekli kütüphaneleri (react, force-graph vb.) indirmek için:
   ```bash
   npm install
   ```
3. Arayüzü başlatmak için:
   ```bash
   npm run dev
   ```
4. Terminalde size verilen adrese (genellikle `http://localhost:5173/`) tarayıcınızdan tıklayarak arayüzü görebilirsiniz.

> **Not:** Docker ile tek tıkla çalıştırma (`docker-compose up`) özelliği projenin tüm servisleri tamamlandığında eklenecektir. Şu an test aşamasında olduğumuz için yerel geliştirme (dev) ortamında ayrı ayrı çalıştırılmaktadır.
