# Chatter LiveChat PC 💬

C# Windows Forms ile yazılmış, Supabase backend'li gerçek zamanlı sohbet uygulaması.

[![GitHub stars](https://img.shields.io/github/stars/tda45/Chatter-LiveChat-PC)](https://github.com/tda45/Chatter-LiveChat-PC/stargazers)
[![GitHub license](https://img.shields.io/github/license/tda45/Chatter-LiveChat-PC)](https://github.com/tda45/Chatter-LiveChat-PC/blob/main/LICENSE)
[![GitHub release](https://img.shields.io/github/v/release/tda45/Chatter-LiveChat-PC)](https://github.com/tda45/Chatter-LiveChat-PC/releases/tag/1.0)

## 🚀 Özellikler

- ✅ **Gerçek zamanlı mesajlaşma** - Supabase Realtime ile anlık iletişim
- ✅ **Kullanıcı adı kontrolü** - Aynı anda aynı isim kullanılamaz
- ✅ **Küfür filtresi** - Yasaklı kelimeleri otomatik engeller
- ✅ **Aktif kullanıcı listesi** - Kimler çevrimiçi görün
- ✅ **Yazıyor bildirimi** (✏️) - Karşı taraf yazarken anında haberin olsun
- ✅ **Sohbet istatistikleri** (`/stats`) - En çok mesaj atanlar, toplam mesaj sayısı
- ✅ **Giriş/Çıkış mesajları** - Sunucuya katılan ve ayrılanlar bildirilir
- ✅ **Gece kırmızı tema** - Göz yormayan özel tasarım
- ✅ **Tam ekran modu** - Görev çubuğuna kadar tüm ekranı kaplar

## 📸 Ekran Görüntüleri

*Yakında eklenecek...*

## 🛠️ Kullanılan Teknolojiler

- **C# / .NET 8.0** - Windows Forms
- **Supabase** - PostgreSQL veritabanı ve Realtime özellikleri
- **Newtonsoft.Json** - JSON işleme

## 📦 Kurulum

### Windows için (İndir ve Çalıştır)

**En Son Sürüm: [v1.0](https://github.com/tda45/Chatter-LiveChat-PC/releases/tag/1.0)**

1. [**Releases**](https://github.com/tda45/Chatter-LiveChat-PC/releases) sayfasına gidin
2. En son sürüm olan **[1.0](https://github.com/tda45/Chatter-LiveChat-PC/releases/tag/1.0)** altındaki **`Chatter-LiveChat-PC-EXELİ.zip`** dosyasını indirin
3. ZIP'i masaüstüne veya istediğiniz klasöre çıkarın
4. `ChatterLiveChat.exe` dosyasına çift tıklayın
5. Kullanıcı adınızı girip sohbete başlayın!

**Not:** .NET runtime kurulu olmasa bile çalışır (self-contained).

### Kaynak Koddan Derleme

```bash
git clone https://github.com/tda45/Chatter-LiveChat-PC.git
cd Chatter-LiveChat-PC/ChatterLiveChat
dotnet restore
dotnet run

📝 Kullanım
Giriş Yapın: Açılışta kullanıcı adınızı girin

Sohbet Edin: Mesajınızı yazın ve Enter'a basın veya "Gönder" butonuna tıklayın

Komutlar:

/stats - Sohbet istatistiklerini gösterir

🔧 Geliştirici Bilgileri
Gereksinimler
.NET 8.0 SDK

Visual Studio 2022 veya VS Code

Supabase hesabı (kendi backend'inizi kurmak için)

Proje Yapısı:
Chatter-LiveChat-PC/
├── ChatterLiveChat/
│   ├── Form1.cs           # Ana form ve UI kodları
│   ├── Program.cs         # Uygulama giriş noktası
│   ├── chat.ico           # Uygulama ikonu
│   └── ChatterLiveChat.csproj
├── .gitignore
└── README.md
Supabase Kurulumu
Kendi Supabase instance'ınızı kullanmak için Form1.cs dosyasındaki:

private readonly string supabaseUrl = "https://sizin-projeniz.supabase.co";
private readonly string supabaseKey = "sizin-anon-key-iniz";
bilgilerini değiştirin.

📜 Sürüm Geçmişi
1.0 - 2026-03-01
İlk kararlı sürüm

Tüm temel özellikler eklendi

Tek EXE olarak yayınlandı

İndir: Chatter-LiveChat-PC-EXELİ.zip

🤝 Katkıda Bulunma
Bu repoyu fork edin

Yeni bir branch oluşturun (git checkout -b yeni-ozellik)

Değişikliklerinizi commit edin (git commit -am 'Yeni özellik eklendi')

Branch'inizi push edin (git push origin yeni-ozellik)

Pull Request oluşturun

📄 Lisans
Bu proje MIT lisansı ile lisanslanmıştır. Detaylar için LICENSE dosyasına bakın.

📬 İletişim
GitHub: @tda45

Proje Bağlantısı: https://github.com/tda45/Chatter-LiveChat-PC

⭐ Destek Ol

Projeyi beğendiyseniz GitHub'da bir ⭐ bırakmayı unutmayın!

## 🚀 **Güncelleme Komutları:**

```powershell
cd D:\Vs Code kiler\Chatter-LiveChat-PC-Yapım Yeri\ChatterLiveChat
git add README.md
git commit -m "README güncellendi: Tüm içerik eklendi, ZIP adı düzeltildi"
git push