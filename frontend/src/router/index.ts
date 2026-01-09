import { createRouter, createWebHistory } from 'vue-router'
import type { RouteRecordRaw } from 'vue-router'

const routes: RouteRecordRaw[] = [
  {
    path: '/',
    name: 'Home',
    component: () => import('../views/Home.vue'),
  },
  // Encoding Tools
  {
    path: '/tools/encoding/base64',
    name: 'Base64',
    component: () => import('../views/tools/encoding/Base64.vue'),
    meta: { title: 'Base64 Encoder/Decoder', category: 'encoding' }
  },
  {
    path: '/tools/encoding/hex',
    name: 'Hex',
    component: () => import('../views/tools/encoding/Hex.vue'),
    meta: { title: 'Hex Encoder/Decoder', category: 'encoding' }
  },
  {
    path: '/tools/encoding/url',
    name: 'URL',
    component: () => import('../views/tools/encoding/Url.vue'),
    meta: { title: 'URL Encoder/Decoder', category: 'encoding' }
  },
  {
    path: '/tools/encoding/html',
    name: 'HTML',
    component: () => import('../views/tools/encoding/Html.vue'),
    meta: { title: 'HTML Encoder/Decoder', category: 'encoding' }
  },
  // Hash Tools
  {
    path: '/tools/hash',
    name: 'HashGenerator',
    component: () => import('../views/tools/hash/HashGenerator.vue'),
    meta: { title: 'Hash Generator', category: 'hash' }
  },
  // Cryptography Tools
  {
    path: '/tools/crypto/aes',
    name: 'AES',
    component: () => import('../views/tools/crypto/Aes.vue'),
    meta: { title: 'AES Encryption/Decryption', category: 'crypto' }
  },
  {
    path: '/tools/crypto/des',
    name: 'DES',
    component: () => import('../views/tools/crypto/Des.vue'),
    meta: { title: 'DES Encryption/Decryption', category: 'crypto' }
  },
  {
    path: '/tools/crypto/triple-des',
    name: 'TripleDES',
    component: () => import('../views/tools/crypto/TripleDes.vue'),
    meta: { title: 'Triple DES Encryption/Decryption', category: 'crypto' }
  },
  {
    path: '/tools/crypto/rc4',
    name: 'RC4',
    component: () => import('../views/tools/crypto/Rc4.vue'),
    meta: { title: 'RC4 Encryption/Decryption', category: 'crypto' }
  },
  {
    path: '/tools/crypto/rsa',
    name: 'RSA',
    component: () => import('../views/tools/crypto/Rsa.vue'),
    meta: { title: 'RSA Encryption/Signing', category: 'crypto' }
  },
  {
    path: '/tools/crypto/ecdsa',
    name: 'ECDSA',
    component: () => import('../views/tools/crypto/Ecdsa.vue'),
    meta: { title: 'ECDSA Signing', category: 'crypto' }
  },
  {
    path: '/tools/hash/md5',
    name: 'MD5',
    component: () => import('../views/tools/hash/Md5.vue'),
    meta: { title: 'MD5 Hash Generator', category: 'hash' }
  },
  {
    path: '/tools/hash/sha1',
    name: 'SHA1',
    component: () => import('../views/tools/hash/Sha1.vue'),
    meta: { title: 'SHA-1 Hash Generator', category: 'hash' }
  },
  {
    path: '/tools/hash/sha256',
    name: 'SHA256',
    component: () => import('../views/tools/hash/Sha256.vue'),
    meta: { title: 'SHA-256 Hash Generator', category: 'hash' }
  },
  {
    path: '/tools/hash/sha512',
    name: 'SHA512',
    component: () => import('../views/tools/hash/Sha512.vue'),
    meta: { title: 'SHA-512 Hash Generator', category: 'hash' }
  },
  // Converter Tools
  {
    path: '/tools/converter/unit',
    name: 'UnitConverter',
    component: () => import('../views/tools/converter/Unit.vue'),
    meta: { title: 'Unit Converter', category: 'converter' }
  },
  {
    path: '/tools/converter/case',
    name: 'CaseConverter',
    component: () => import('../views/tools/converter/Case.vue'),
    meta: { title: 'Case Converter', category: 'converter' }
  },
  {
    path: '/tools/converter/number-base',
    name: 'NumberBase',
    component: () => import('../views/tools/converter/NumberBase.vue'),
    meta: { title: 'Number Base Converter', category: 'converter' }
  },
  {
    path: '/tools/converter/color',
    name: 'ColorConverter',
    component: () => import('../views/tools/converter/Color.vue'),
    meta: { title: 'Color Converter', category: 'converter' }
  },
  // Format Tools
  {
    path: '/tools/format/json',
    name: 'JSONFormatter',
    component: () => import('../views/tools/format/Json.vue'),
    meta: { title: 'JSON Formatter', category: 'format' }
  },
  {
    path: '/tools/format/xml',
    name: 'XMLFormatter',
    component: () => import('../views/tools/format/Xml.vue'),
    meta: { title: 'XML Formatter', category: 'format' }
  },
  {
    path: '/tools/format/sql',
    name: 'SQLFormatter',
    component: () => import('../views/tools/format/Sql.vue'),
    meta: { title: 'SQL Formatter', category: 'format' }
  },
  {
    path: '/tools/format/html',
    name: 'HTMLFormatter',
    component: () => import('../views/tools/format/HtmlFormatter.vue'),
    meta: { title: 'HTML Formatter', category: 'format' }
  },
  // Generator Tools
  {
    path: '/tools/generator/uuid',
    name: 'UUIDGenerator',
    component: () => import('../views/tools/generator/Uuid.vue'),
    meta: { title: 'UUID Generator', category: 'generator' }
  },
  {
    path: '/tools/generator/password',
    name: 'PasswordGenerator',
    component: () => import('../views/tools/generator/Password.vue'),
    meta: { title: 'Password Generator', category: 'generator' }
  },
  {
    path: '/tools/generator/lorem',
    name: 'LoremIpsum',
    component: () => import('../views/tools/generator/Lorem.vue'),
    meta: { title: 'Lorem Ipsum Generator', category: 'generator' }
  },
  // Other Tools
  {
    path: '/tools/other/qrcode',
    name: 'QRCode',
    component: () => import('../views/tools/other/QrCode.vue'),
    meta: { title: 'QR Code Generator', category: 'other' }
  },
  {
    path: '/tools/other/diff',
    name: 'TextDiff',
    component: () => import('../views/tools/other/Diff.vue'),
    meta: { title: 'Text Diff', category: 'other' }
  },
  // Catch-all
  {
    path: '/:pathMatch(.*)*',
    redirect: '/'
  }
]

const router = createRouter({
  history: createWebHistory(),
  routes,
})

router.beforeEach((to, _from, next) => {
  const title = to.meta.title as string | undefined
  document.title = title ? `${title} | Toolvia` : 'Toolvia'
  next()
})

export default router
