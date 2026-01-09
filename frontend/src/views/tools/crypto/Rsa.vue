<script setup lang="ts">
import { ref } from 'vue'
import ToolLayout from '../../../components/common/ToolLayout.vue'
import CopyButton from '../../../components/common/CopyButton.vue'

type Tab = 'keygen' | 'encrypt' | 'decrypt' | 'sign' | 'verify'
type KeySize = 1024 | 2048 | 4096
type HashAlgorithm = 'SHA-256' | 'SHA-384' | 'SHA-512'

const activeTab = ref<Tab>('keygen')
const isProcessing = ref(false)
const error = ref('')

// Key generation
const keySize = ref<KeySize>(2048)
const publicKeyPem = ref('')
const privateKeyPem = ref('')

// Encryption/Decryption
const encryptInput = ref('')
const encryptOutput = ref('')
const encryptPublicKey = ref('')
const decryptInput = ref('')
const decryptOutput = ref('')
const decryptPrivateKey = ref('')

// Sign/Verify
const signInput = ref('')
const signOutput = ref('')
const signPrivateKey = ref('')
const signHashAlgorithm = ref<HashAlgorithm>('SHA-256')
const verifyInput = ref('')
const verifySignature = ref('')
const verifyPublicKey = ref('')
const verifyHashAlgorithm = ref<HashAlgorithm>('SHA-256')
const verifyResult = ref<boolean | null>(null)

const keySizes: KeySize[] = [1024, 2048, 4096]
const hashAlgorithms: HashAlgorithm[] = ['SHA-256', 'SHA-384', 'SHA-512']

// Helper functions
const arrayBufferToBase64 = (buffer: ArrayBuffer): string => {
  const bytes = new Uint8Array(buffer)
  let binary = ''
  for (let i = 0; i < bytes.byteLength; i++) {
    binary += String.fromCharCode(bytes[i])
  }
  return btoa(binary)
}

const base64ToArrayBuffer = (base64: string): ArrayBuffer => {
  const binary = atob(base64)
  const bytes = new Uint8Array(binary.length)
  for (let i = 0; i < binary.length; i++) {
    bytes[i] = binary.charCodeAt(i)
  }
  return bytes.buffer
}

const formatPem = (base64: string, type: 'PUBLIC' | 'PRIVATE'): string => {
  const lines = base64.match(/.{1,64}/g) || []
  return `-----BEGIN ${type} KEY-----\n${lines.join('\n')}\n-----END ${type} KEY-----`
}

const parsePem = (pem: string): string => {
  return pem
    .replace(/-----BEGIN.*-----/, '')
    .replace(/-----END.*-----/, '')
    .replace(/\s/g, '')
}

// Generate RSA key pair
const generateKeyPair = async () => {
  isProcessing.value = true
  error.value = ''

  try {
    const keyPair = await window.crypto.subtle.generateKey(
      {
        name: 'RSA-OAEP',
        modulusLength: keySize.value,
        publicExponent: new Uint8Array([1, 0, 1]), // 65537
        hash: 'SHA-256'
      },
      true,
      ['encrypt', 'decrypt']
    )

    // Export public key
    const publicKeyBuffer = await window.crypto.subtle.exportKey('spki', keyPair.publicKey)
    publicKeyPem.value = formatPem(arrayBufferToBase64(publicKeyBuffer), 'PUBLIC')

    // Export private key
    const privateKeyBuffer = await window.crypto.subtle.exportKey('pkcs8', keyPair.privateKey)
    privateKeyPem.value = formatPem(arrayBufferToBase64(privateKeyBuffer), 'PRIVATE')

    // Also set for encryption/signing
    encryptPublicKey.value = publicKeyPem.value
    signPrivateKey.value = privateKeyPem.value
    verifyPublicKey.value = publicKeyPem.value
  } catch (e: any) {
    error.value = e.message || 'Key generation failed'
  } finally {
    isProcessing.value = false
  }
}

// Encrypt with public key
const encrypt = async () => {
  isProcessing.value = true
  error.value = ''
  encryptOutput.value = ''

  if (!encryptInput.value || !encryptPublicKey.value) {
    error.value = 'Please enter text and public key'
    isProcessing.value = false
    return
  }

  try {
    const publicKeyBase64 = parsePem(encryptPublicKey.value)
    const publicKeyBuffer = base64ToArrayBuffer(publicKeyBase64)

    const publicKey = await window.crypto.subtle.importKey(
      'spki',
      publicKeyBuffer,
      {
        name: 'RSA-OAEP',
        hash: 'SHA-256'
      },
      false,
      ['encrypt']
    )

    const encoder = new TextEncoder()
    const data = encoder.encode(encryptInput.value)
    const encrypted = await window.crypto.subtle.encrypt(
      { name: 'RSA-OAEP' },
      publicKey,
      data
    )

    encryptOutput.value = arrayBufferToBase64(encrypted)
  } catch (e: any) {
    error.value = e.message || 'Encryption failed'
  } finally {
    isProcessing.value = false
  }
}

// Decrypt with private key
const decrypt = async () => {
  isProcessing.value = true
  error.value = ''
  decryptOutput.value = ''

  if (!decryptInput.value || !decryptPrivateKey.value) {
    error.value = 'Please enter ciphertext and private key'
    isProcessing.value = false
    return
  }

  try {
    const privateKeyBase64 = parsePem(decryptPrivateKey.value)
    const privateKeyBuffer = base64ToArrayBuffer(privateKeyBase64)

    const privateKey = await window.crypto.subtle.importKey(
      'pkcs8',
      privateKeyBuffer,
      {
        name: 'RSA-OAEP',
        hash: 'SHA-256'
      },
      false,
      ['decrypt']
    )

    const encryptedData = base64ToArrayBuffer(decryptInput.value.replace(/\s/g, ''))
    const decrypted = await window.crypto.subtle.decrypt(
      { name: 'RSA-OAEP' },
      privateKey,
      encryptedData
    )

    const decoder = new TextDecoder()
    decryptOutput.value = decoder.decode(decrypted)
  } catch (e: any) {
    error.value = e.message || 'Decryption failed. Check your private key and ciphertext.'
  } finally {
    isProcessing.value = false
  }
}

// Sign message
const sign = async () => {
  isProcessing.value = true
  error.value = ''
  signOutput.value = ''

  if (!signInput.value || !signPrivateKey.value) {
    error.value = 'Please enter message and private key'
    isProcessing.value = false
    return
  }

  try {
    const privateKeyBase64 = parsePem(signPrivateKey.value)
    const privateKeyBuffer = base64ToArrayBuffer(privateKeyBase64)

    const privateKey = await window.crypto.subtle.importKey(
      'pkcs8',
      privateKeyBuffer,
      {
        name: 'RSASSA-PKCS1-v1_5',
        hash: signHashAlgorithm.value
      },
      false,
      ['sign']
    )

    const encoder = new TextEncoder()
    const data = encoder.encode(signInput.value)
    const signature = await window.crypto.subtle.sign(
      'RSASSA-PKCS1-v1_5',
      privateKey,
      data
    )

    signOutput.value = arrayBufferToBase64(signature)
  } catch (e: any) {
    error.value = e.message || 'Signing failed'
  } finally {
    isProcessing.value = false
  }
}

// Verify signature
const verify = async () => {
  isProcessing.value = true
  error.value = ''
  verifyResult.value = null

  if (!verifyInput.value || !verifySignature.value || !verifyPublicKey.value) {
    error.value = 'Please enter message, signature, and public key'
    isProcessing.value = false
    return
  }

  try {
    const publicKeyBase64 = parsePem(verifyPublicKey.value)
    const publicKeyBuffer = base64ToArrayBuffer(publicKeyBase64)

    const publicKey = await window.crypto.subtle.importKey(
      'spki',
      publicKeyBuffer,
      {
        name: 'RSASSA-PKCS1-v1_5',
        hash: verifyHashAlgorithm.value
      },
      false,
      ['verify']
    )

    const encoder = new TextEncoder()
    const data = encoder.encode(verifyInput.value)
    const signatureBuffer = base64ToArrayBuffer(verifySignature.value.replace(/\s/g, ''))

    verifyResult.value = await window.crypto.subtle.verify(
      'RSASSA-PKCS1-v1_5',
      publicKey,
      signatureBuffer,
      data
    )
  } catch (e: any) {
    error.value = e.message || 'Verification failed'
    verifyResult.value = false
  } finally {
    isProcessing.value = false
  }
}

const useGeneratedKeys = () => {
  encryptPublicKey.value = publicKeyPem.value
  decryptPrivateKey.value = privateKeyPem.value
  signPrivateKey.value = privateKeyPem.value
  verifyPublicKey.value = publicKeyPem.value
}
</script>

<template>
  <ToolLayout title="RSA Encryption/Signing" description="Generate RSA keys, encrypt/decrypt data, and sign/verify messages">
    <div class="space-y-6">
      <!-- Tab Navigation -->
      <div class="flex flex-wrap gap-2 border-b border-gray-200 dark:border-dark-700 pb-2">
        <button
          v-for="tab in [
            { id: 'keygen', label: 'Key Generator' },
            { id: 'encrypt', label: 'Encrypt' },
            { id: 'decrypt', label: 'Decrypt' },
            { id: 'sign', label: 'Sign' },
            { id: 'verify', label: 'Verify' }
          ]"
          :key="tab.id"
          @click="activeTab = tab.id as Tab"
          :class="[
            'px-4 py-2 text-sm font-medium rounded-lg transition-colors',
            activeTab === tab.id
              ? 'bg-primary-600 text-white'
              : 'bg-gray-100 dark:bg-dark-800 text-gray-700 dark:text-gray-300 hover:bg-gray-200 dark:hover:bg-dark-700'
          ]"
        >
          {{ tab.label }}
        </button>
      </div>

      <!-- Key Generator Tab -->
      <div v-if="activeTab === 'keygen'" class="space-y-4">
        <div class="space-y-2">
          <label class="text-sm font-medium text-gray-700 dark:text-gray-300">Key Size</label>
          <select v-model="keySize" class="tool-input">
            <option v-for="size in keySizes" :key="size" :value="size">{{ size }} bits</option>
          </select>
        </div>

        <button
          @click="generateKeyPair"
          :disabled="isProcessing"
          class="tool-button"
        >
          {{ isProcessing ? 'Generating...' : 'Generate Key Pair' }}
        </button>

        <div v-if="publicKeyPem" class="space-y-4">
          <div class="space-y-2">
            <div class="flex items-center justify-between">
              <label class="text-sm font-medium text-gray-700 dark:text-gray-300">Public Key</label>
              <CopyButton :text="publicKeyPem" />
            </div>
            <textarea
              :value="publicKeyPem"
              readonly
              class="tool-textarea h-40 font-mono text-xs bg-gray-50 dark:bg-dark-800"
            />
          </div>

          <div class="space-y-2">
            <div class="flex items-center justify-between">
              <label class="text-sm font-medium text-gray-700 dark:text-gray-300">Private Key</label>
              <CopyButton :text="privateKeyPem" />
            </div>
            <textarea
              :value="privateKeyPem"
              readonly
              class="tool-textarea h-48 font-mono text-xs bg-gray-50 dark:bg-dark-800"
            />
            <p class="text-xs text-amber-600 dark:text-amber-400">
              Keep your private key secure! Never share it with anyone.
            </p>
          </div>

          <button @click="useGeneratedKeys" class="tool-button-secondary text-sm">
            Use Keys in Other Tabs
          </button>
        </div>
      </div>

      <!-- Encrypt Tab -->
      <div v-if="activeTab === 'encrypt'" class="space-y-4">
        <div class="space-y-2">
          <label class="text-sm font-medium text-gray-700 dark:text-gray-300">Plain Text</label>
          <textarea
            v-model="encryptInput"
            placeholder="Enter text to encrypt..."
            class="tool-textarea h-24 font-mono text-sm"
          />
        </div>

        <div class="space-y-2">
          <label class="text-sm font-medium text-gray-700 dark:text-gray-300">Public Key (PEM)</label>
          <textarea
            v-model="encryptPublicKey"
            placeholder="Paste public key here..."
            class="tool-textarea h-32 font-mono text-xs"
          />
        </div>

        <button
          @click="encrypt"
          :disabled="isProcessing"
          class="tool-button"
        >
          {{ isProcessing ? 'Encrypting...' : 'Encrypt' }}
        </button>

        <div v-if="encryptOutput" class="space-y-2">
          <div class="flex items-center justify-between">
            <label class="text-sm font-medium text-gray-700 dark:text-gray-300">Encrypted (Base64)</label>
            <CopyButton :text="encryptOutput" />
          </div>
          <textarea
            :value="encryptOutput"
            readonly
            class="tool-textarea h-24 font-mono text-xs bg-gray-50 dark:bg-dark-800"
          />
        </div>
      </div>

      <!-- Decrypt Tab -->
      <div v-if="activeTab === 'decrypt'" class="space-y-4">
        <div class="space-y-2">
          <label class="text-sm font-medium text-gray-700 dark:text-gray-300">Cipher Text (Base64)</label>
          <textarea
            v-model="decryptInput"
            placeholder="Enter encrypted text..."
            class="tool-textarea h-24 font-mono text-xs"
          />
        </div>

        <div class="space-y-2">
          <label class="text-sm font-medium text-gray-700 dark:text-gray-300">Private Key (PEM)</label>
          <textarea
            v-model="decryptPrivateKey"
            placeholder="Paste private key here..."
            class="tool-textarea h-40 font-mono text-xs"
          />
        </div>

        <button
          @click="decrypt"
          :disabled="isProcessing"
          class="tool-button"
        >
          {{ isProcessing ? 'Decrypting...' : 'Decrypt' }}
        </button>

        <div v-if="decryptOutput" class="space-y-2">
          <div class="flex items-center justify-between">
            <label class="text-sm font-medium text-gray-700 dark:text-gray-300">Decrypted Text</label>
            <CopyButton :text="decryptOutput" />
          </div>
          <textarea
            :value="decryptOutput"
            readonly
            class="tool-textarea h-24 font-mono text-sm bg-gray-50 dark:bg-dark-800"
          />
        </div>
      </div>

      <!-- Sign Tab -->
      <div v-if="activeTab === 'sign'" class="space-y-4">
        <div class="space-y-2">
          <label class="text-sm font-medium text-gray-700 dark:text-gray-300">Message to Sign</label>
          <textarea
            v-model="signInput"
            placeholder="Enter message to sign..."
            class="tool-textarea h-24 font-mono text-sm"
          />
        </div>

        <div class="space-y-2">
          <label class="text-sm font-medium text-gray-700 dark:text-gray-300">Hash Algorithm</label>
          <select v-model="signHashAlgorithm" class="tool-input">
            <option v-for="hash in hashAlgorithms" :key="hash" :value="hash">{{ hash }}</option>
          </select>
        </div>

        <div class="space-y-2">
          <label class="text-sm font-medium text-gray-700 dark:text-gray-300">Private Key (PEM)</label>
          <textarea
            v-model="signPrivateKey"
            placeholder="Paste private key here..."
            class="tool-textarea h-40 font-mono text-xs"
          />
        </div>

        <button
          @click="sign"
          :disabled="isProcessing"
          class="tool-button"
        >
          {{ isProcessing ? 'Signing...' : 'Sign Message' }}
        </button>

        <div v-if="signOutput" class="space-y-2">
          <div class="flex items-center justify-between">
            <label class="text-sm font-medium text-gray-700 dark:text-gray-300">Signature (Base64)</label>
            <CopyButton :text="signOutput" />
          </div>
          <textarea
            :value="signOutput"
            readonly
            class="tool-textarea h-24 font-mono text-xs bg-gray-50 dark:bg-dark-800"
          />
        </div>
      </div>

      <!-- Verify Tab -->
      <div v-if="activeTab === 'verify'" class="space-y-4">
        <div class="space-y-2">
          <label class="text-sm font-medium text-gray-700 dark:text-gray-300">Original Message</label>
          <textarea
            v-model="verifyInput"
            placeholder="Enter original message..."
            class="tool-textarea h-24 font-mono text-sm"
          />
        </div>

        <div class="space-y-2">
          <label class="text-sm font-medium text-gray-700 dark:text-gray-300">Signature (Base64)</label>
          <textarea
            v-model="verifySignature"
            placeholder="Enter signature..."
            class="tool-textarea h-20 font-mono text-xs"
          />
        </div>

        <div class="space-y-2">
          <label class="text-sm font-medium text-gray-700 dark:text-gray-300">Hash Algorithm</label>
          <select v-model="verifyHashAlgorithm" class="tool-input">
            <option v-for="hash in hashAlgorithms" :key="hash" :value="hash">{{ hash }}</option>
          </select>
        </div>

        <div class="space-y-2">
          <label class="text-sm font-medium text-gray-700 dark:text-gray-300">Public Key (PEM)</label>
          <textarea
            v-model="verifyPublicKey"
            placeholder="Paste public key here..."
            class="tool-textarea h-32 font-mono text-xs"
          />
        </div>

        <button
          @click="verify"
          :disabled="isProcessing"
          class="tool-button"
        >
          {{ isProcessing ? 'Verifying...' : 'Verify Signature' }}
        </button>

        <div v-if="verifyResult !== null" class="p-4 rounded-lg" :class="verifyResult ? 'bg-green-50 dark:bg-green-900/20' : 'bg-red-50 dark:bg-red-900/20'">
          <p class="font-medium" :class="verifyResult ? 'text-green-700 dark:text-green-400' : 'text-red-700 dark:text-red-400'">
            {{ verifyResult ? 'Signature is valid!' : 'Signature is invalid!' }}
          </p>
        </div>
      </div>

      <!-- Error -->
      <div v-if="error" class="p-3 bg-red-50 dark:bg-red-900/20 border border-red-200 dark:border-red-800 rounded-lg">
        <p class="text-sm text-red-600 dark:text-red-400">{{ error }}</p>
      </div>

      <!-- Info -->
      <div class="p-4 bg-gray-50 dark:bg-dark-800 rounded-lg">
        <h3 class="text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">About RSA</h3>
        <p class="text-sm text-gray-600 dark:text-gray-400">
          RSA is an asymmetric cryptographic algorithm that uses a pair of keys (public and private).
          The public key is used for encryption and signature verification, while the private key is used for
          decryption and signing. RSA-2048 or higher is recommended for secure applications.
        </p>
      </div>
    </div>
  </ToolLayout>
</template>
