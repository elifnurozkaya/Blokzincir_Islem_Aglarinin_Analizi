# Proje Durum Raporu: Blokzincir İşlem Ağlarının Analizi

*Tarih:* Güncel Durum
*Aşama:* Faz 1 (Altyapı ve İskelet Kurulumu)

## 1. Genel Mimari ve Mikroservis Yaklaşımı
Proje, proje değerlendirme standartlarında istenen "asenkron çalışma" şartlarına uygun olarak *Mikroservis Mimarisi* ile tasarlanmıştır. Sistem şu anda iki bağımsız ana bileşene (servise) bölünmüştür:
1. *Frontend Servisi (Web Arayüzü)*
2. *Backend Servisi (Veri Yapıları ve Algoritmalar)*


## 2. Frontend (Kullanıcı Arayüzü) Durumu
Kullanıcı arayüzü, sektör standardı teknolojiler kullanılarak sıfırdan inşa edilmiştir.
* *Kullanılan Teknolojiler:* Hızlı derleme için *Vite, modern arayüz bileşenleri için **React (TypeScript)* tercih edilmiştir.
* *Görselleştirme:* Faz 3'teki node-link diyagramı isterleri için react-force-graph kütüphanesi altyapıya dahil edilmiştir.
* *Tasarım:* Sadelik ön planda tutularak, modern bir *Koyu Tema (Dark Mode)* arayüzü (Sol Menü, Graf Alanı ve Merkle Ağacı paneli) şablon olarak kodlanmıştır.

## 3. Backend ve Veri Yapıları
Backend tarafında *C# (.NET Core)* kullanılmış olup, 4 kişilik proje ekibinin Git üzerinde çakışma (conflict) yaşamadan eşzamanlı çalışabilmesi için tüm sınıfların "İskelet (Skeleton)" yapıları oluşturulmuştur. Tüm algoritmaların içi şu an throw new NotImplementedException(); fırlatacak şekilde boş bırakılmıştır. Görev dağılımı şu şekildedir:
* *Elifnur Özkaya - Onur Karadaş:* Yönlü Graf (Directed Graph) yapısının inşası ile BFS/DFS dolaşım algoritmalarının yazılması (TransactionGraph.cs).
* *Abdullah Eren Korkmaz:* Veri bütünlüğünü doğrulamak için Merkle Ağacının oluşturulması (MerkleTree.cs).
* *Yiğit Sağım:* Cüzdan adreslerine O(1) hızında erişim sağlayacak Karma Tablonun (Hash Table) yazılması (WalletHashTable.cs).
* Hazır kütüphane kullanmadan BFS ve DFS'e destek verecek olan Kuyruk ve Yığıt sınıflarının inşası (CustomQueue.cs ve CustomStack.cs).
* (Ortak Modeller olan Wallet ve Transaction sınıfları ise herkesin kullanımı için merkeze eklenmiştir.)

## 4. Docker Entegrasyon Durumu
* *Frontend:* Arayüzün tek başına ayağa kalkabilmesi için Nginx tabanlı Dockerfile hazırlanmıştır.
* *Backend:* C# API projesinin kendi içerisinde bir Dockerfile şablonu mevcuttur.
