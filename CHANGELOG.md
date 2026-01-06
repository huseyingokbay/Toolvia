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
**Dosya:** `frontend/src/views/tools/format/Json.vue`

Escaped/stringified JSON'ı normal JSON'a çeviren "Unescape" butonu eklendi.

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
