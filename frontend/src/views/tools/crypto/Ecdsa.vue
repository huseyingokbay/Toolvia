<script setup lang="ts">
import { ref } from 'vue'
import ToolLayout from '../../../components/common/ToolLayout.vue'
import CopyButton from '../../../components/common/CopyButton.vue'

type Tab = 'keygen' | 'sign' | 'verify'
type Curve = 'P-256' | 'P-384' | 'P-521'
type HashAlgorithm = 'SHA-256' | 'SHA-384' | 'SHA-512'

const activeTab = ref<Tab>('keygen')
const isProcessing = ref(false)
const error = ref('')

// Key generation
const curve = ref<Curve>('P-256')
const publicKeyPem = ref('')
const privateKeyPem = ref('')
const publicKeyJwk = ref('')
const privateKeyJwk = ref('')

// Sign
const signInput = ref('')
const signOutput = ref('')
const signPrivateKey = ref('')
const signHashAlgorithm = ref<HashAlgorithm>('SHA-256')

// Verify
const verifyInput = ref('')
const verifySignature = ref('')
const verifyPublicKey = ref('')
const verifyHashAlgorithm = ref<HashAlgorithm>('SHA-256')
const verifyResult = ref<boolean | null>(null)

const curves: { value: Curve; label: string }[] = [
  { value: 'P-256', label: 'P-256 (secp256r1)' },
  { value: 'P-384', label: 'P-384 (secp384r1)' },
  { value: 'P-521', label: 'P-521 (secp521r1)' }
]

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

// Generate ECDSA key pair
const generateKeyPair = async () => {
  isProcessing.value = true
  error.value = ''

  try {
    const keyPair = await window.crypto.subtle.generateKey(
      {
        name: 'ECDSA',
        namedCurve: curve.value
      },
      true,
      ['sign', 'verify']
    )

    // Export public key as SPKI (PEM format)
    const publicKeyBuffer = await window.crypto.subtle.exportKey('spki', keyPair.publicKey)
    publicKeyPem.value = formatPem(arrayBufferToBase64(publicKeyBuffer), 'PUBLIC')

    // Export private key as PKCS8 (PEM format)
    const privateKeyBuffer = await window.crypto.subtle.exportKey('pkcs8', keyPair.privateKey)
    privateKeyPem.value = formatPem(arrayBufferToBase64(privateKeyBuffer), 'PRIVATE')

    // Export as JWK for reference
    const pubJwk = await window.crypto.subtle.exportKey('jwk', keyPair.publicKey)
    publicKeyJwk.value = JSON.stringify(pubJwk, null, 2)

    const privJwk = await window.crypto.subtle.exportKey('jwk', keyPair.privateKey)
    privateKeyJwk.value = JSON.stringify(privJwk, null, 2)

    // Set for signing
    signPrivateKey.value = privateKeyPem.value
    verifyPublicKey.value = publicKeyPem.value
  } catch (e: any) {
    error.value = e.message || 'Key generation failed'
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

    // Try to import with different curves if the first one fails
    let privateKey: CryptoKey | null = null
    const curvesToTry: Curve[] = ['P-256', 'P-384', 'P-521']

    for (const c of curvesToTry) {
      try {
        privateKey = await window.crypto.subtle.importKey(
          'pkcs8',
          privateKeyBuffer,
          {
            name: 'ECDSA',
            namedCurve: c
          },
          false,
          ['sign']
        )
        break
      } catch {
        continue
      }
    }

    if (!privateKey) {
      throw new Error('Could not import private key. Make sure it is a valid ECDSA key.')
    }

    const encoder = new TextEncoder()
    const data = encoder.encode(signInput.value)
    const signature = await window.crypto.subtle.sign(
      {
        name: 'ECDSA',
        hash: signHashAlgorithm.value
      },
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

    // Try to import with different curves if the first one fails
    let publicKey: CryptoKey | null = null
    const curvesToTry: Curve[] = ['P-256', 'P-384', 'P-521']

    for (const c of curvesToTry) {
      try {
        publicKey = await window.crypto.subtle.importKey(
          'spki',
          publicKeyBuffer,
          {
            name: 'ECDSA',
            namedCurve: c
          },
          false,
          ['verify']
        )
        break
      } catch {
        continue
      }
    }

    if (!publicKey) {
      throw new Error('Could not import public key. Make sure it is a valid ECDSA key.')
    }

    const encoder = new TextEncoder()
    const data = encoder.encode(verifyInput.value)
    const signatureBuffer = base64ToArrayBuffer(verifySignature.value.replace(/\s/g, ''))

    verifyResult.value = await window.crypto.subtle.verify(
      {
        name: 'ECDSA',
        hash: verifyHashAlgorithm.value
      },
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
  signPrivateKey.value = privateKeyPem.value
  verifyPublicKey.value = publicKeyPem.value
}
</script>

<template>
  <ToolLayout title="ECDSA Signing" description="Generate ECDSA keys and sign/verify messages using elliptic curve cryptography">
    <div class="space-y-6">
      <!-- Tab Navigation -->
      <div class="flex flex-wrap gap-2 border-b border-gray-200 dark:border-dark-700 pb-2">
        <button
          v-for="tab in [
            { id: 'keygen', label: 'Key Generator' },
            { id: 'sign', label: 'Sign Message' },
            { id: 'verify', label: 'Verify Signature' }
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
          <label class="text-sm font-medium text-gray-700 dark:text-gray-300">Elliptic Curve</label>
          <select v-model="curve" class="tool-input">
            <option v-for="c in curves" :key="c.value" :value="c.value">{{ c.label }}</option>
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
              <label class="text-sm font-medium text-gray-700 dark:text-gray-300">Public Key (PEM)</label>
              <CopyButton :text="publicKeyPem" />
            </div>
            <textarea
              :value="publicKeyPem"
              readonly
              class="tool-textarea h-32 font-mono text-xs bg-gray-50 dark:bg-dark-800"
            />
          </div>

          <div class="space-y-2">
            <div class="flex items-center justify-between">
              <label class="text-sm font-medium text-gray-700 dark:text-gray-300">Private Key (PEM)</label>
              <CopyButton :text="privateKeyPem" />
            </div>
            <textarea
              :value="privateKeyPem"
              readonly
              class="tool-textarea h-36 font-mono text-xs bg-gray-50 dark:bg-dark-800"
            />
            <p class="text-xs text-amber-600 dark:text-amber-400">
              Keep your private key secure! Never share it with anyone.
            </p>
          </div>

          <!-- JWK Format (Collapsible) -->
          <details class="border border-gray-200 dark:border-dark-700 rounded-lg">
            <summary class="px-4 py-2 cursor-pointer text-sm font-medium text-gray-700 dark:text-gray-300 hover:bg-gray-50 dark:hover:bg-dark-800">
              JWK Format (Click to expand)
            </summary>
            <div class="p-4 space-y-4 border-t border-gray-200 dark:border-dark-700">
              <div class="space-y-2">
                <div class="flex items-center justify-between">
                  <label class="text-sm font-medium text-gray-700 dark:text-gray-300">Public Key (JWK)</label>
                  <CopyButton :text="publicKeyJwk" />
                </div>
                <textarea
                  :value="publicKeyJwk"
                  readonly
                  class="tool-textarea h-28 font-mono text-xs bg-gray-50 dark:bg-dark-800"
                />
              </div>

              <div class="space-y-2">
                <div class="flex items-center justify-between">
                  <label class="text-sm font-medium text-gray-700 dark:text-gray-300">Private Key (JWK)</label>
                  <CopyButton :text="privateKeyJwk" />
                </div>
                <textarea
                  :value="privateKeyJwk"
                  readonly
                  class="tool-textarea h-32 font-mono text-xs bg-gray-50 dark:bg-dark-800"
                />
              </div>
            </div>
          </details>

          <button @click="useGeneratedKeys" class="tool-button-secondary text-sm">
            Use Keys in Other Tabs
          </button>
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
            class="tool-textarea h-36 font-mono text-xs"
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
        <h3 class="text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">About ECDSA</h3>
        <p class="text-sm text-gray-600 dark:text-gray-400">
          ECDSA (Elliptic Curve Digital Signature Algorithm) provides digital signatures using elliptic curve cryptography.
          It offers equivalent security to RSA with smaller key sizes. P-256 provides ~128-bit security, P-384 provides ~192-bit,
          and P-521 provides ~256-bit security. ECDSA is used in many protocols including TLS, Bitcoin, and JWT.
        </p>
      </div>
    </div>
  </ToolLayout>
</template>
