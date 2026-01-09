<script setup lang="ts">
import { ref, watch } from 'vue'
import CryptoJS from 'crypto-js'
import ToolLayout from '../../../components/common/ToolLayout.vue'
import CopyButton from '../../../components/common/CopyButton.vue'

type Mode = 'encrypt' | 'decrypt'
type InputEncoding = 'utf8' | 'hex' | 'base64'
type OutputEncoding = 'hex' | 'hex-upper' | 'base64'
type CipherMode = 'CBC' | 'ECB' | 'CTR' | 'CFB' | 'OFB'
type Padding = 'Pkcs7' | 'Iso97971' | 'AnsiX923' | 'ZeroPadding' | 'NoPadding'
type KeyType = 'raw' | 'pbkdf2'
type HashAlgorithm = 'SHA256' | 'SHA1' | 'SHA384' | 'SHA512' | 'MD5'
type SaltType = 'random' | 'custom' | 'none'

const mode = ref<Mode>('encrypt')
const input = ref('')
const output = ref('')
const error = ref('')

// Encoding options
const inputEncoding = ref<InputEncoding>('utf8')
const outputEncoding = ref<OutputEncoding>('hex')

// DES options (64-bit key, 64-bit block)
const cipherMode = ref<CipherMode>('CBC')
const padding = ref<Padding>('Pkcs7')

// Key options
const keyType = ref<KeyType>('pbkdf2')
const passphrase = ref('')
const rawKey = ref('')
const iv = ref('')
const autoGenerateIv = ref(true)

// PBKDF2 options
const hashAlgorithm = ref<HashAlgorithm>('SHA256')
const saltType = ref<SaltType>('random')
const customSalt = ref('')
const generatedSalt = ref('')
const customIterations = ref(false)
const iterations = ref(10000)

// Mode options
const cipherModes: CipherMode[] = ['CBC', 'ECB', 'CTR', 'CFB', 'OFB']
const paddings: Padding[] = ['Pkcs7', 'Iso97971', 'AnsiX923', 'ZeroPadding', 'NoPadding']
const hashAlgorithms: HashAlgorithm[] = ['SHA256', 'SHA1', 'SHA384', 'SHA512', 'MD5']

const inputEncodings: { value: InputEncoding; label: string }[] = [
  { value: 'utf8', label: 'UTF-8' },
  { value: 'hex', label: 'Hex' },
  { value: 'base64', label: 'Base64' }
]

const outputEncodings: { value: OutputEncoding; label: string }[] = [
  { value: 'hex', label: 'Hex (Lower Case)' },
  { value: 'hex-upper', label: 'Hex (Upper Case)' },
  { value: 'base64', label: 'Base64' }
]

// Get CryptoJS mode
const getCipherMode = () => {
  const modes: Record<CipherMode, any> = {
    'CBC': CryptoJS.mode.CBC,
    'ECB': CryptoJS.mode.ECB,
    'CTR': CryptoJS.mode.CTR,
    'CFB': CryptoJS.mode.CFB,
    'OFB': CryptoJS.mode.OFB
  }
  return modes[cipherMode.value]
}

// Get CryptoJS padding
const getPadding = () => {
  const pads: Record<Padding, any> = {
    'Pkcs7': CryptoJS.pad.Pkcs7,
    'Iso97971': CryptoJS.pad.Iso97971,
    'AnsiX923': CryptoJS.pad.AnsiX923,
    'ZeroPadding': CryptoJS.pad.ZeroPadding,
    'NoPadding': CryptoJS.pad.NoPadding
  }
  return pads[padding.value]
}

// Get hash function
const getHasher = () => {
  const hashers: Record<HashAlgorithm, any> = {
    'SHA256': CryptoJS.algo.SHA256,
    'SHA1': CryptoJS.algo.SHA1,
    'SHA384': CryptoJS.algo.SHA384,
    'SHA512': CryptoJS.algo.SHA512,
    'MD5': CryptoJS.algo.MD5
  }
  return hashers[hashAlgorithm.value]
}

// Generate random bytes
const generateRandomHex = (bytes: number): string => {
  return CryptoJS.lib.WordArray.random(bytes).toString()
}

// Parse input based on encoding
const parseInput = (text: string, encoding: InputEncoding): CryptoJS.lib.WordArray => {
  switch (encoding) {
    case 'hex':
      return CryptoJS.enc.Hex.parse(text)
    case 'base64':
      return CryptoJS.enc.Base64.parse(text)
    default:
      return CryptoJS.enc.Utf8.parse(text)
  }
}

// Format output based on encoding
const formatOutput = (wordArray: CryptoJS.lib.WordArray, encoding: OutputEncoding): string => {
  switch (encoding) {
    case 'hex-upper':
      return wordArray.toString(CryptoJS.enc.Hex).toUpperCase()
    case 'base64':
      return wordArray.toString(CryptoJS.enc.Base64)
    default:
      return wordArray.toString(CryptoJS.enc.Hex)
  }
}

// Get or generate key (DES uses 64-bit key = 8 bytes)
const getKey = (): CryptoJS.lib.WordArray => {
  const keyBytes = 8 // DES uses 64-bit key

  if (keyType.value === 'raw') {
    return CryptoJS.enc.Hex.parse(rawKey.value.padEnd(keyBytes * 2, '0').slice(0, keyBytes * 2))
  } else {
    let salt: CryptoJS.lib.WordArray

    if (saltType.value === 'random') {
      if (!generatedSalt.value) {
        generatedSalt.value = generateRandomHex(8)
      }
      salt = CryptoJS.enc.Hex.parse(generatedSalt.value)
    } else if (saltType.value === 'custom') {
      salt = CryptoJS.enc.Utf8.parse(customSalt.value)
    } else {
      salt = CryptoJS.lib.WordArray.create([])
    }

    return CryptoJS.PBKDF2(passphrase.value, salt, {
      keySize: keyBytes / 4,
      iterations: customIterations.value ? iterations.value : 10000,
      hasher: getHasher()
    })
  }
}

// Get or generate IV (DES uses 64-bit IV = 8 bytes)
const getIv = (): CryptoJS.lib.WordArray => {
  if (cipherMode.value === 'ECB') {
    return CryptoJS.lib.WordArray.create([])
  }

  if (autoGenerateIv.value && mode.value === 'encrypt') {
    const newIv = generateRandomHex(8)
    iv.value = newIv
    return CryptoJS.enc.Hex.parse(newIv)
  }

  return CryptoJS.enc.Hex.parse(iv.value.padEnd(16, '0').slice(0, 16))
}

// Encrypt
const encrypt = () => {
  error.value = ''
  if (!input.value) {
    output.value = ''
    return
  }

  if (keyType.value === 'pbkdf2' && !passphrase.value) {
    error.value = 'Please enter a passphrase'
    return
  }

  if (keyType.value === 'raw' && !rawKey.value) {
    error.value = 'Please enter a key'
    return
  }

  try {
    const key = getKey()
    const ivData = getIv()
    const inputData = parseInput(input.value, inputEncoding.value)

    const encrypted = CryptoJS.DES.encrypt(inputData, key, {
      iv: ivData,
      mode: getCipherMode(),
      padding: getPadding()
    })

    output.value = formatOutput(encrypted.ciphertext, outputEncoding.value)
  } catch (e: any) {
    error.value = e.message || 'Encryption failed'
    output.value = ''
  }
}

// Decrypt
const decrypt = () => {
  error.value = ''
  if (!input.value) {
    output.value = ''
    return
  }

  if (keyType.value === 'pbkdf2' && !passphrase.value) {
    error.value = 'Please enter a passphrase'
    return
  }

  if (keyType.value === 'raw' && !rawKey.value) {
    error.value = 'Please enter a key'
    return
  }

  if (!iv.value && cipherMode.value !== 'ECB') {
    error.value = 'Please enter the IV used during encryption'
    return
  }

  try {
    const key = getKey()
    const ivData = CryptoJS.enc.Hex.parse(iv.value.padEnd(16, '0').slice(0, 16))

    let ciphertext: CryptoJS.lib.WordArray
    if (inputEncoding.value === 'base64') {
      ciphertext = CryptoJS.enc.Base64.parse(input.value)
    } else {
      ciphertext = CryptoJS.enc.Hex.parse(input.value.toLowerCase().replace(/\s/g, ''))
    }

    const cipherParams = CryptoJS.lib.CipherParams.create({
      ciphertext: ciphertext
    })

    const decrypted = CryptoJS.DES.decrypt(cipherParams, key, {
      iv: ivData,
      mode: getCipherMode(),
      padding: getPadding()
    })

    const decryptedText = decrypted.toString(CryptoJS.enc.Utf8)
    if (!decryptedText && decrypted.sigBytes > 0) {
      output.value = decrypted.toString(CryptoJS.enc.Hex)
    } else {
      output.value = decryptedText
    }
  } catch (e: any) {
    error.value = e.message || 'Decryption failed. Check your key, IV, and settings.'
    output.value = ''
  }
}

// Process
const process = () => {
  if (mode.value === 'encrypt') {
    encrypt()
  } else {
    decrypt()
  }
}

// Generate new salt
const generateNewSalt = () => {
  generatedSalt.value = generateRandomHex(8)
}

// Generate new IV
const generateNewIv = () => {
  iv.value = generateRandomHex(8)
}

// Clear all
const clear = () => {
  input.value = ''
  output.value = ''
  error.value = ''
}

// Watch for auto-process
watch([input, passphrase, rawKey, iv, cipherMode, padding, keyType, hashAlgorithm, saltType, customSalt, iterations, inputEncoding, outputEncoding], () => {
  if (input.value && (passphrase.value || rawKey.value)) {
    process()
  }
})

// Watch mode changes
watch(mode, () => {
  output.value = ''
  error.value = ''
  if (mode.value === 'decrypt') {
    autoGenerateIv.value = false
  }
})
</script>

<template>
  <ToolLayout title="DES Encryption/Decryption" description="Encrypt and decrypt data using DES algorithm">
    <div class="space-y-6">
      <!-- Warning -->
      <div class="p-3 bg-amber-50 dark:bg-amber-900/20 border border-amber-200 dark:border-amber-800 rounded-lg">
        <p class="text-sm text-amber-700 dark:text-amber-400">
          <strong>Warning:</strong> DES is considered insecure due to its small key size (56 bits). Use AES for secure encryption.
        </p>
      </div>

      <!-- Mode Toggle -->
      <div class="flex gap-2">
        <button
          @click="mode = 'encrypt'"
          :class="[
            'flex-1 px-4 py-3 text-sm font-medium rounded-lg transition-colors',
            mode === 'encrypt'
              ? 'bg-primary-600 text-white'
              : 'bg-gray-100 dark:bg-dark-800 text-gray-700 dark:text-gray-300 hover:bg-gray-200 dark:hover:bg-dark-700'
          ]"
        >
          Encrypt
        </button>
        <button
          @click="mode = 'decrypt'"
          :class="[
            'flex-1 px-4 py-3 text-sm font-medium rounded-lg transition-colors',
            mode === 'decrypt'
              ? 'bg-primary-600 text-white'
              : 'bg-gray-100 dark:bg-dark-800 text-gray-700 dark:text-gray-300 hover:bg-gray-200 dark:hover:bg-dark-700'
          ]"
        >
          Decrypt
        </button>
      </div>

      <!-- Input -->
      <div class="space-y-2">
        <div class="flex items-center justify-between">
          <label class="text-sm font-medium text-gray-700 dark:text-gray-300">
            {{ mode === 'encrypt' ? 'Plain Text' : 'Cipher Text' }}
          </label>
          <select
            v-model="inputEncoding"
            class="text-xs border-gray-300 dark:border-dark-600 rounded-md dark:bg-dark-800"
          >
            <option v-for="enc in inputEncodings" :key="enc.value" :value="enc.value">
              {{ enc.label }}
            </option>
          </select>
        </div>
        <textarea
          v-model="input"
          :placeholder="mode === 'encrypt' ? 'Enter text to encrypt...' : 'Enter cipher text to decrypt...'"
          class="tool-textarea h-32 font-mono text-sm"
        />
      </div>

      <!-- Settings Grid -->
      <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
        <!-- Mode -->
        <div class="space-y-2">
          <label class="text-sm font-medium text-gray-700 dark:text-gray-300">Mode</label>
          <select v-model="cipherMode" class="tool-input">
            <option v-for="m in cipherModes" :key="m" :value="m">{{ m }}</option>
          </select>
        </div>

        <!-- Padding -->
        <div class="space-y-2">
          <label class="text-sm font-medium text-gray-700 dark:text-gray-300">Padding</label>
          <select v-model="padding" class="tool-input">
            <option v-for="p in paddings" :key="p" :value="p">{{ p }}</option>
          </select>
        </div>

        <!-- Output Encoding (for encrypt mode) -->
        <div v-if="mode === 'encrypt'" class="space-y-2">
          <label class="text-sm font-medium text-gray-700 dark:text-gray-300">Output Encoding</label>
          <select v-model="outputEncoding" class="tool-input">
            <option v-for="enc in outputEncodings" :key="enc.value" :value="enc.value">
              {{ enc.label }}
            </option>
          </select>
        </div>
      </div>

      <!-- Key Type -->
      <div class="space-y-4 p-4 bg-gray-50 dark:bg-dark-800 rounded-lg">
        <div class="flex gap-4">
          <label class="flex items-center gap-2 cursor-pointer">
            <input
              type="radio"
              v-model="keyType"
              value="pbkdf2"
              class="text-primary-600 focus:ring-primary-500"
            />
            <span class="text-sm text-gray-700 dark:text-gray-300">PBKDF2 (Password-Based)</span>
          </label>
          <label class="flex items-center gap-2 cursor-pointer">
            <input
              type="radio"
              v-model="keyType"
              value="raw"
              class="text-primary-600 focus:ring-primary-500"
            />
            <span class="text-sm text-gray-700 dark:text-gray-300">Raw Key (Hex)</span>
          </label>
        </div>

        <!-- PBKDF2 Options -->
        <div v-if="keyType === 'pbkdf2'" class="space-y-4">
          <div class="space-y-2">
            <label class="text-sm font-medium text-gray-700 dark:text-gray-300">Passphrase</label>
            <input
              v-model="passphrase"
              type="password"
              placeholder="Enter your passphrase..."
              class="tool-input"
            />
          </div>

          <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div class="space-y-2">
              <label class="text-sm font-medium text-gray-700 dark:text-gray-300">Hash Algorithm</label>
              <select v-model="hashAlgorithm" class="tool-input">
                <option v-for="h in hashAlgorithms" :key="h" :value="h">{{ h }}</option>
              </select>
            </div>

            <div class="space-y-2">
              <label class="text-sm font-medium text-gray-700 dark:text-gray-300">Salt Type</label>
              <select v-model="saltType" class="tool-input">
                <option value="random">Random</option>
                <option value="custom">Custom</option>
                <option value="none">None</option>
              </select>
            </div>
          </div>

          <!-- Salt display/input -->
          <div v-if="saltType === 'random'" class="space-y-2">
            <div class="flex items-center justify-between">
              <label class="text-sm font-medium text-gray-700 dark:text-gray-300">Generated Salt (Hex)</label>
              <button @click="generateNewSalt" class="text-xs text-primary-600 hover:text-primary-700">
                Generate New
              </button>
            </div>
            <div class="flex gap-2">
              <input
                v-model="generatedSalt"
                readonly
                placeholder="Salt will be generated..."
                class="tool-input bg-gray-100 dark:bg-dark-700 font-mono text-sm flex-1"
              />
              <CopyButton v-if="generatedSalt" :text="generatedSalt" />
            </div>
          </div>

          <div v-if="saltType === 'custom'" class="space-y-2">
            <label class="text-sm font-medium text-gray-700 dark:text-gray-300">Custom Salt</label>
            <input
              v-model="customSalt"
              placeholder="Enter custom salt..."
              class="tool-input"
            />
          </div>

          <!-- Iterations -->
          <div class="space-y-2">
            <label class="flex items-center gap-2 cursor-pointer">
              <input
                type="checkbox"
                v-model="customIterations"
                class="rounded border-gray-300 dark:border-dark-600 text-primary-600 focus:ring-primary-500"
              />
              <span class="text-sm text-gray-700 dark:text-gray-300">Custom Iterations</span>
            </label>
            <input
              v-if="customIterations"
              v-model.number="iterations"
              type="number"
              min="1000"
              max="100000"
              class="tool-input"
            />
          </div>
        </div>

        <!-- Raw Key Options -->
        <div v-else class="space-y-2">
          <label class="text-sm font-medium text-gray-700 dark:text-gray-300">
            Key (Hex, 16 characters for 64-bit key)
          </label>
          <input
            v-model="rawKey"
            placeholder="Enter 64-bit key in hex..."
            class="tool-input font-mono text-sm"
          />
        </div>
      </div>

      <!-- IV -->
      <div v-if="cipherMode !== 'ECB'" class="space-y-2">
        <div class="flex items-center justify-between">
          <label class="text-sm font-medium text-gray-700 dark:text-gray-300">
            IV (Initialization Vector, 16 hex characters)
          </label>
          <div class="flex items-center gap-3">
            <label v-if="mode === 'encrypt'" class="flex items-center gap-2 text-sm text-gray-600 dark:text-gray-400">
              <input
                type="checkbox"
                v-model="autoGenerateIv"
                class="rounded border-gray-300 dark:border-dark-600 text-primary-600 focus:ring-primary-500"
              />
              Auto Generate
            </label>
            <button @click="generateNewIv" class="text-xs text-primary-600 hover:text-primary-700">
              Generate New
            </button>
          </div>
        </div>
        <div class="flex gap-2">
          <input
            v-model="iv"
            :readonly="autoGenerateIv && mode === 'encrypt'"
            placeholder="Enter or generate IV..."
            class="tool-input font-mono text-sm flex-1"
            :class="{ 'bg-gray-100 dark:bg-dark-700': autoGenerateIv && mode === 'encrypt' }"
          />
          <CopyButton v-if="iv" :text="iv" />
        </div>
        <p class="text-xs text-amber-600 dark:text-amber-400">
          Save the IV! You'll need it for decryption.
        </p>
      </div>

      <!-- Error -->
      <div v-if="error" class="p-3 bg-red-50 dark:bg-red-900/20 border border-red-200 dark:border-red-800 rounded-lg">
        <p class="text-sm text-red-600 dark:text-red-400">{{ error }}</p>
      </div>

      <!-- Output -->
      <div class="space-y-2">
        <div class="flex items-center justify-between">
          <label class="text-sm font-medium text-gray-700 dark:text-gray-300">
            {{ mode === 'encrypt' ? 'Cipher Text' : 'Plain Text' }}
          </label>
          <CopyButton v-if="output" :text="output" />
        </div>
        <textarea
          :value="output"
          readonly
          :placeholder="mode === 'encrypt' ? 'Encrypted output will appear here...' : 'Decrypted output will appear here...'"
          class="tool-textarea h-32 font-mono text-sm bg-gray-50 dark:bg-dark-800"
        />
      </div>

      <!-- Actions -->
      <div class="flex gap-2">
        <button @click="process" class="tool-button">
          {{ mode === 'encrypt' ? 'Encrypt' : 'Decrypt' }}
        </button>
        <button @click="clear" class="tool-button-secondary">
          Clear
        </button>
      </div>

      <!-- Info -->
      <div class="p-4 bg-gray-50 dark:bg-dark-800 rounded-lg">
        <h3 class="text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">About DES</h3>
        <p class="text-sm text-gray-600 dark:text-gray-400">
          DES (Data Encryption Standard) is a symmetric encryption algorithm that uses a 56-bit key (64 bits with parity).
          Due to its small key size, DES is vulnerable to brute-force attacks and is considered insecure for modern applications.
          Use AES or Triple DES instead.
        </p>
      </div>
    </div>
  </ToolLayout>
</template>
