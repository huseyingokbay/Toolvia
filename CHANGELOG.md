# Changelog

Bu dosya projede yapılan değişiklikleri takip eder.

---

## 2026-01-06

### CLAUDE.md Oluşturuldu
- Proje hakkında genel bilgiler eklendi
- Tech stack detayları (Vue.js 3, TypeScript, Vite 7, TailwindCSS v4, .NET 10)
- Sık kullanılan komutlar (frontend/backend)
- Proje yapısı dokümantasyonu

### JSON Formatter - Unescape Özelliği

Escaped/stringified JSON'ı normal JSON'a çeviren "Unescape" özelliği eklendi.

**Frontend:**
- **Dosya:** `frontend/src/views/tools/format/Json.vue`
- "Unescape" butonu eklendi

**Backend:**
- **Dosya:** `backend/Controllers/FormatController.cs`
- **Endpoint:** `POST /api/format/json/unescape`

**Kullanım:**
```
Giriş:  "{\"incoming\":[{\"day\":\"2026-01-05\",\"dailyCount\":889}]}"
Çıkış:  {
          "incoming": [
            {
              "day": "2026-01-05",
              "dailyCount": 889
            }
          ]
        }
```

**Desteklenen escape karakterleri:**
- `\"` → `"`
- `\'` → `'`
- `\\` → `\`
- `\n` → newline
- `\r` → carriage return
- `\t` → tab

**README.md güncellendi:**
- JSON Formatter açıklamasına "unescape" eklendi

### HTML Formatter - Yeni Modül

HTML kodlarını güzelleştiren (beautify) ve tek satır haline getiren (minify) yeni modül eklendi.

**Frontend:**
- **Dosya:** `frontend/src/views/tools/format/HtmlFormatter.vue`
- **Route:** `/tools/format/html`
- Beautify ve Minify butonları
- Indent size seçimi (2/4 space)

**Backend:**
- **Dosya:** `backend/Controllers/FormatController.cs`
- **Endpoint:** `POST /api/format/html/format` - HTML beautify
- **Endpoint:** `POST /api/format/html/minify` - HTML minify

**Özellikler:**
- Self-closing tag desteği (br, hr, img, input, meta, vb.)
- DOCTYPE ve comment desteği
- Otomatik indentation

**README.md güncellendi:**
- Format bölümüne HTML Formatter eklendi

### Google AdSense Entegrasyonu

Sayfalara Google AdSense reklam alanları eklendi.

**Eklenen dosyalar:**
- `frontend/src/components/common/GoogleAd.vue` - Reusable reklam komponenti

**Güncellenen dosyalar:**
- `frontend/index.html` - AdSense script eklendi
- `frontend/src/components/common/ToolLayout.vue` - Üst ve alt reklam alanları
- `frontend/src/views/Home.vue` - Üst ve alt reklam alanları

**Reklam konumları:**
- Ana sayfa: Hero bölümü altında ve sayfa sonunda
- Tool sayfaları: Başlık altında ve içerik sonunda

**Not:** `ca-pub-XXXXXXXXXXXXXXXX` değerini kendi AdSense Publisher ID'nizle değiştirin.

### Hash Generator - Yeni Birleşik Hash Modülü

Tüm hash algoritmalarını tek bir sayfada toplayan yeni Hash Generator eklendi.

**Frontend:**
- **Dosya:** `frontend/src/views/tools/hash/HashGenerator.vue`
- **Route:** `/tools/hash`
- Text ve File input desteği
- Tüm algoritmalar tek sayfada

**Backend:**
- **Dosya:** `backend/Controllers/HashController.cs`
- SHA3 algoritmaları eklendi
- File hashing endpoint eklendi: `POST /api/hash/file`

**Desteklenen Algoritmalar:**
- **MD:** MD5
- **SHA-1:** SHA-1
- **SHA-2:** SHA-224, SHA-256, SHA-384, SHA-512
- **SHA-3:** SHA3-224, SHA3-256, SHA3-384, SHA3-512
- **RIPEMD:** RIPEMD-160

**Özellikler:**
- Text input ile anlık hash hesaplama
- File upload ile dosya hash'leme
- Uppercase/lowercase seçimi
- Copy to clipboard

**API Endpoints:**
- `POST /api/hash/md5`
- `POST /api/hash/sha1`
- `POST /api/hash/sha224`
- `POST /api/hash/sha256`
- `POST /api/hash/sha384`
- `POST /api/hash/sha512`
- `POST /api/hash/sha3-256`
- `POST /api/hash/sha3-384`
- `POST /api/hash/sha3-512`
- `POST /api/hash/file?algorithm=sha256`
- `POST /api/hash/all`
