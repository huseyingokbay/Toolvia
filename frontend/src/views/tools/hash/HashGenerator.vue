<script setup lang="ts">
import { ref, computed, watch } from 'vue'
import CryptoJS from 'crypto-js'
import ToolLayout from '../../../components/common/ToolLayout.vue'
import CopyButton from '../../../components/common/CopyButton.vue'

// Hash algorithm definitions
const hashCategories = [
  {
    name: 'MD',
    algorithms: [
      { id: 'md5', name: 'MD5', bits: 128 }
    ]
  },
  {
    name: 'SHA-1',
    algorithms: [
      { id: 'sha1', name: 'SHA-1', bits: 160 }
    ]
  },
  {
    name: 'SHA-2',
    algorithms: [
      { id: 'sha224', name: 'SHA-224', bits: 224 },
      { id: 'sha256', name: 'SHA-256', bits: 256 },
      { id: 'sha384', name: 'SHA-384', bits: 384 },
      { id: 'sha512', name: 'SHA-512', bits: 512 }
    ]
  },
  {
    name: 'SHA-3',
    algorithms: [
      { id: 'sha3-224', name: 'SHA3-224', bits: 224 },
      { id: 'sha3-256', name: 'SHA3-256', bits: 256 },
      { id: 'sha3-384', name: 'SHA3-384', bits: 384 },
      { id: 'sha3-512', name: 'SHA3-512', bits: 512 }
    ]
  },
  {
    name: 'RIPEMD',
    algorithms: [
      { id: 'ripemd160', name: 'RIPEMD-160', bits: 160 }
    ]
  }
]

const allAlgorithms = hashCategories.flatMap(cat => cat.algorithms)

const inputMode = ref<'text' | 'file'>('text')
const textInput = ref('')
const fileInput = ref<File | null>(null)
const fileContent = ref<ArrayBuffer | null>(null)
const selectedAlgorithm = ref('sha256')
const uppercase = ref(false)
const isProcessing = ref(false)
const output = ref('')

// Hash functions
const hashFunctions: Record<string, (input: string | CryptoJS.lib.WordArray) => string> = {
  'md5': (input) => CryptoJS.MD5(input).toString(),
  'sha1': (input) => CryptoJS.SHA1(input).toString(),
  'sha224': (input) => CryptoJS.SHA224(input).toString(),
  'sha256': (input) => CryptoJS.SHA256(input).toString(),
  'sha384': (input) => CryptoJS.SHA384(input).toString(),
  'sha512': (input) => CryptoJS.SHA512(input).toString(),
  'sha3-224': (input) => CryptoJS.SHA3(input, { outputLength: 224 }).toString(),
  'sha3-256': (input) => CryptoJS.SHA3(input, { outputLength: 256 }).toString(),
  'sha3-384': (input) => CryptoJS.SHA3(input, { outputLength: 384 }).toString(),
  'sha3-512': (input) => CryptoJS.SHA3(input, { outputLength: 512 }).toString(),
  'ripemd160': (input) => CryptoJS.RIPEMD160(input).toString()
}

const selectedAlgorithmInfo = computed(() => {
  return allAlgorithms.find(a => a.id === selectedAlgorithm.value)
})

const formattedOutput = computed(() => {
  return uppercase.value ? output.value.toUpperCase() : output.value.toLowerCase()
})

const hashLength = computed(() => {
  const algo = selectedAlgorithmInfo.value
  return algo ? algo.bits / 4 : 0
})

// Process text input
const processText = () => {
  if (!textInput.value) {
    output.value = ''
    return
  }
  const hashFn = hashFunctions[selectedAlgorithm.value]
  if (hashFn) {
    output.value = hashFn(textInput.value)
  }
}

// Process file input
const processFile = async () => {
  if (!fileContent.value) {
    output.value = ''
    return
  }

  isProcessing.value = true
  try {
    // Convert ArrayBuffer to WordArray for CryptoJS
    const wordArray = CryptoJS.lib.WordArray.create(fileContent.value as any)
    const hashFn = hashFunctions[selectedAlgorithm.value]
    if (hashFn) {
      output.value = hashFn(wordArray)
    }
  } finally {
    isProcessing.value = false
  }
}

// Watch for changes
watch([textInput, selectedAlgorithm], () => {
  if (inputMode.value === 'text') {
    processText()
  }
}, { immediate: true })

watch([fileContent, selectedAlgorithm], () => {
  if (inputMode.value === 'file' && fileContent.value) {
    processFile()
  }
})

watch(inputMode, () => {
  output.value = ''
  if (inputMode.value === 'text') {
    processText()
  } else if (fileContent.value) {
    processFile()
  }
})

// File handling
const handleFileSelect = (event: Event) => {
  const target = event.target as HTMLInputElement
  const file = target.files?.[0]
  if (file) {
    fileInput.value = file
    const reader = new FileReader()
    reader.onload = (e) => {
      fileContent.value = e.target?.result as ArrayBuffer
    }
    reader.readAsArrayBuffer(file)
  }
}

const clearFile = () => {
  fileInput.value = null
  fileContent.value = null
  output.value = ''
}

const clear = () => {
  textInput.value = ''
  fileInput.value = null
  fileContent.value = null
  output.value = ''
}

const formatFileSize = (bytes: number): string => {
  if (bytes < 1024) return bytes + ' B'
  if (bytes < 1024 * 1024) return (bytes / 1024).toFixed(2) + ' KB'
  return (bytes / (1024 * 1024)).toFixed(2) + ' MB'
}
</script>

<template>
  <ToolLayout title="Hash Generator" description="Generate cryptographic hashes from text or files">
    <div class="space-y-6">
      <!-- Algorithm Selection -->
      <div class="space-y-3">
        <label class="text-sm font-medium text-gray-700 dark:text-gray-300">Select Algorithm</label>
        <div class="grid grid-cols-2 sm:grid-cols-3 md:grid-cols-4 lg:grid-cols-5 gap-2">
          <template v-for="category in hashCategories" :key="category.name">
            <button
              v-for="algo in category.algorithms"
              :key="algo.id"
              @click="selectedAlgorithm = algo.id"
              :class="[
                'px-3 py-2 text-sm rounded-lg border transition-colors',
                selectedAlgorithm === algo.id
                  ? 'border-primary-500 bg-primary-50 dark:bg-primary-900/20 text-primary-700 dark:text-primary-300'
                  : 'border-gray-200 dark:border-dark-700 hover:border-gray-300 dark:hover:border-dark-600 text-gray-700 dark:text-gray-300'
              ]"
            >
              {{ algo.name }}
            </button>
          </template>
        </div>
      </div>

      <!-- Input Mode Toggle -->
      <div class="flex gap-2">
        <button
          @click="inputMode = 'text'"
          :class="[
            'px-4 py-2 text-sm font-medium rounded-lg transition-colors',
            inputMode === 'text'
              ? 'bg-primary-600 text-white'
              : 'bg-gray-100 dark:bg-dark-800 text-gray-700 dark:text-gray-300 hover:bg-gray-200 dark:hover:bg-dark-700'
          ]"
        >
          Text Input
        </button>
        <button
          @click="inputMode = 'file'"
          :class="[
            'px-4 py-2 text-sm font-medium rounded-lg transition-colors',
            inputMode === 'file'
              ? 'bg-primary-600 text-white'
              : 'bg-gray-100 dark:bg-dark-800 text-gray-700 dark:text-gray-300 hover:bg-gray-200 dark:hover:bg-dark-700'
          ]"
        >
          File Input
        </button>
      </div>

      <!-- Text Input -->
      <div v-if="inputMode === 'text'" class="space-y-2">
        <div class="flex items-center justify-between">
          <label class="text-sm font-medium text-gray-700 dark:text-gray-300">Input Text</label>
          <span class="text-xs text-gray-500">{{ textInput.length }} chars</span>
        </div>
        <textarea
          v-model="textInput"
          placeholder="Enter text to hash..."
          class="tool-textarea h-40"
        />
      </div>

      <!-- File Input -->
      <div v-else class="space-y-2">
        <label class="text-sm font-medium text-gray-700 dark:text-gray-300">Select File</label>
        <div
          class="border-2 border-dashed border-gray-300 dark:border-dark-600 rounded-lg p-8 text-center"
          :class="{ 'border-primary-500 bg-primary-50 dark:bg-primary-900/10': fileInput }"
        >
          <input
            type="file"
            @change="handleFileSelect"
            class="hidden"
            id="file-input"
          />
          <label for="file-input" class="cursor-pointer">
            <div v-if="!fileInput" class="space-y-2">
              <svg class="w-12 h-12 mx-auto text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M7 16a4 4 0 01-.88-7.903A5 5 0 1115.9 6L16 6a5 5 0 011 9.9M15 13l-3-3m0 0l-3 3m3-3v12" />
              </svg>
              <p class="text-gray-600 dark:text-gray-400">Click to select a file or drag and drop</p>
            </div>
            <div v-else class="space-y-2">
              <svg class="w-12 h-12 mx-auto text-primary-500" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z" />
              </svg>
              <p class="font-medium text-gray-900 dark:text-white">{{ fileInput.name }}</p>
              <p class="text-sm text-gray-500">{{ formatFileSize(fileInput.size) }}</p>
            </div>
          </label>
          <button
            v-if="fileInput"
            @click.stop="clearFile"
            class="mt-4 px-4 py-2 text-sm text-red-600 hover:text-red-700 dark:text-red-400"
          >
            Remove File
          </button>
        </div>
      </div>

      <!-- Output -->
      <div class="space-y-2">
        <div class="flex items-center justify-between">
          <label class="text-sm font-medium text-gray-700 dark:text-gray-300">
            {{ selectedAlgorithmInfo?.name }} Hash
          </label>
          <div class="flex items-center gap-3">
            <label class="flex items-center gap-2 text-sm text-gray-600 dark:text-gray-400">
              <input
                type="checkbox"
                v-model="uppercase"
                class="rounded border-gray-300 dark:border-dark-600 text-primary-600 focus:ring-primary-500"
              />
              Uppercase
            </label>
            <CopyButton v-if="output" :text="formattedOutput" />
          </div>
        </div>
        <div class="relative">
          <input
            :value="isProcessing ? 'Processing...' : formattedOutput"
            readonly
            placeholder="Hash will appear here..."
            class="tool-input bg-gray-50 dark:bg-dark-800 pr-24 font-mono text-sm"
          />
          <span v-if="output && !isProcessing" class="absolute right-3 top-1/2 -translate-y-1/2 text-xs text-gray-400">
            {{ hashLength }} chars
          </span>
        </div>
      </div>

      <!-- Actions -->
      <div class="flex gap-2">
        <button @click="clear" class="tool-button-secondary">
          Clear
        </button>
      </div>

      <!-- Info -->
      <div class="p-4 bg-gray-50 dark:bg-dark-800 rounded-lg">
        <h3 class="text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">
          About {{ selectedAlgorithmInfo?.name }}
        </h3>
        <p class="text-sm text-gray-600 dark:text-gray-400">
          {{ selectedAlgorithmInfo?.name }} produces a {{ selectedAlgorithmInfo?.bits }}-bit
          ({{ hashLength }}-character hexadecimal) hash value.
          <template v-if="selectedAlgorithm === 'md5'">
            <br><strong class="text-amber-600 dark:text-amber-400">Warning:</strong> MD5 is cryptographically broken and should not be used for security purposes.
          </template>
          <template v-if="selectedAlgorithm === 'sha1'">
            <br><strong class="text-amber-600 dark:text-amber-400">Warning:</strong> SHA-1 is considered weak and should be avoided for security-critical applications.
          </template>
        </p>
      </div>
    </div>
  </ToolLayout>
</template>
