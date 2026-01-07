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
