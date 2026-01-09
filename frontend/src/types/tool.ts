export interface Tool {
  id: string
  name: string
  description: string
  icon: string
  path: string
  category: ToolCategory
}

export type ToolCategory = 'encoding' | 'hash' | 'crypto' | 'converter' | 'format' | 'generator' | 'other'

export interface ToolCategoryInfo {
  id: ToolCategory
  name: string
  icon: string
  description: string
}

export const TOOL_CATEGORIES: ToolCategoryInfo[] = [
  { id: 'encoding', name: 'Encoding', icon: 'CodeBracketIcon', description: 'Encode and decode data' },
  { id: 'hash', name: 'Hash', icon: 'FingerPrintIcon', description: 'Generate hash values' },
  { id: 'crypto', name: 'Cryptography', icon: 'LockClosedIcon', description: 'Encrypt and decrypt data' },
  { id: 'converter', name: 'Converter', icon: 'ArrowsRightLeftIcon', description: 'Convert between formats' },
  { id: 'format', name: 'Format', icon: 'DocumentTextIcon', description: 'Format and validate data' },
  { id: 'generator', name: 'Generator', icon: 'SparklesIcon', description: 'Generate random data' },
  { id: 'other', name: 'Other', icon: 'WrenchIcon', description: 'Other utilities' },
]

export const TOOLS: Tool[] = [
  // Encoding
  { id: 'base64', name: 'Base64', description: 'Encode/decode Base64', icon: 'CodeBracketIcon', path: '/tools/encoding/base64', category: 'encoding' },
  { id: 'hex', name: 'Hex', description: 'Encode/decode Hexadecimal', icon: 'HashtagIcon', path: '/tools/encoding/hex', category: 'encoding' },
  { id: 'url', name: 'URL', description: 'Encode/decode URL', icon: 'LinkIcon', path: '/tools/encoding/url', category: 'encoding' },
  { id: 'html', name: 'HTML', description: 'Encode/decode HTML entities', icon: 'CodeBracketSquareIcon', path: '/tools/encoding/html', category: 'encoding' },

  // Hash
  { id: 'hash-generator', name: 'Hash Generator', description: 'MD5, SHA, SHA3, RIPEMD & more', icon: 'FingerPrintIcon', path: '/tools/hash', category: 'hash' },

  // Cryptography
  { id: 'aes', name: 'AES', description: 'AES encryption/decryption', icon: 'LockClosedIcon', path: '/tools/crypto/aes', category: 'crypto' },
  { id: 'des', name: 'DES', description: 'DES encryption/decryption', icon: 'LockClosedIcon', path: '/tools/crypto/des', category: 'crypto' },
  { id: 'triple-des', name: 'Triple DES', description: '3DES encryption/decryption', icon: 'LockClosedIcon', path: '/tools/crypto/triple-des', category: 'crypto' },
  { id: 'rc4', name: 'RC4', description: 'RC4 encryption/decryption', icon: 'LockClosedIcon', path: '/tools/crypto/rc4', category: 'crypto' },
  { id: 'rsa', name: 'RSA', description: 'RSA key generation, encrypt/decrypt, sign/verify', icon: 'KeyIcon', path: '/tools/crypto/rsa', category: 'crypto' },
  { id: 'ecdsa', name: 'ECDSA', description: 'ECDSA key generation, sign/verify', icon: 'KeyIcon', path: '/tools/crypto/ecdsa', category: 'crypto' },

  // Converter
  { id: 'unit', name: 'Unit Converter', description: 'Convert between units', icon: 'ArrowsRightLeftIcon', path: '/tools/converter/unit', category: 'converter' },
  { id: 'case', name: 'Case Converter', description: 'Convert text case', icon: 'LanguageIcon', path: '/tools/converter/case', category: 'converter' },
  { id: 'number-base', name: 'Number Base', description: 'Convert number bases', icon: 'CalculatorIcon', path: '/tools/converter/number-base', category: 'converter' },
  { id: 'color', name: 'Color Converter', description: 'Convert color formats', icon: 'SwatchIcon', path: '/tools/converter/color', category: 'converter' },

  // Format
  { id: 'json', name: 'JSON Formatter', description: 'Format and validate JSON', icon: 'DocumentTextIcon', path: '/tools/format/json', category: 'format' },
  { id: 'xml', name: 'XML Formatter', description: 'Format and validate XML', icon: 'DocumentTextIcon', path: '/tools/format/xml', category: 'format' },
  { id: 'sql', name: 'SQL Formatter', description: 'Format SQL queries', icon: 'CircleStackIcon', path: '/tools/format/sql', category: 'format' },
  { id: 'html-formatter', name: 'HTML Formatter', description: 'Format and minify HTML', icon: 'CodeBracketSquareIcon', path: '/tools/format/html', category: 'format' },

  // Generator
  { id: 'uuid', name: 'UUID Generator', description: 'Generate UUIDs', icon: 'KeyIcon', path: '/tools/generator/uuid', category: 'generator' },
  { id: 'password', name: 'Password Generator', description: 'Generate secure passwords', icon: 'LockClosedIcon', path: '/tools/generator/password', category: 'generator' },
  { id: 'lorem', name: 'Lorem Ipsum', description: 'Generate placeholder text', icon: 'DocumentIcon', path: '/tools/generator/lorem', category: 'generator' },

  // Other
  { id: 'qrcode', name: 'QR Code', description: 'Generate QR codes', icon: 'QrCodeIcon', path: '/tools/other/qrcode', category: 'other' },
  { id: 'diff', name: 'Text Diff', description: 'Compare text differences', icon: 'DocumentDuplicateIcon', path: '/tools/other/diff', category: 'other' },
]

export function getToolsByCategory(category: ToolCategory): Tool[] {
  return TOOLS.filter(tool => tool.category === category)
}

export function getToolById(id: string): Tool | undefined {
  return TOOLS.find(tool => tool.id === id)
}
