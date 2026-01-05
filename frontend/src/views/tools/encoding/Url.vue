<script setup lang="ts">
import { ref, watch } from 'vue'
import ToolLayout from '../../../components/common/ToolLayout.vue'
import CopyButton from '../../../components/common/CopyButton.vue'

const input = ref('')
const output = ref('')
const mode = ref<'encode' | 'decode'>('encode')
const encodeType = ref<'component' | 'full'>('component')
const error = ref('')

const process = () => {
  error.value = ''
  if (!input.value) {
    output.value = ''
    return
  }

  try {
    if (mode.value === 'encode') {
      output.value = encodeType.value === 'component'
        ? encodeURIComponent(input.value)
        : encodeURI(input.value)
    } else {
      output.value = encodeType.value === 'component'
        ? decodeURIComponent(input.value)
        : decodeURI(input.value)
    }
  } catch (e) {
    error.value = 'Invalid input for URL decoding'
    output.value = ''
  }
}

watch([input, mode, encodeType], process, { immediate: true })

const swap = () => {
  const temp = input.value
  input.value = output.value
  output.value = temp
  mode.value = mode.value === 'encode' ? 'decode' : 'encode'
}

const clear = () => {
  input.value = ''
  output.value = ''
  error.value = ''
}
</script>

<template>
  <ToolLayout title="URL Encoder/Decoder" description="Encode or decode URL components">
    <!-- Mode Toggle -->
    <div class="flex flex-wrap gap-4 mb-6">
      <div class="flex gap-2">
        <button
          @click="mode = 'encode'"
          :class="[
            'px-4 py-2 rounded-lg text-sm font-medium transition-colors',
            mode === 'encode'
              ? 'bg-primary-600 text-white'
              : 'bg-gray-100 dark:bg-dark-700 text-gray-700 dark:text-gray-300 hover:bg-gray-200 dark:hover:bg-dark-600'
          ]"
        >
          Encode
        </button>
        <button
          @click="mode = 'decode'"
          :class="[
            'px-4 py-2 rounded-lg text-sm font-medium transition-colors',
            mode === 'decode'
              ? 'bg-primary-600 text-white'
              : 'bg-gray-100 dark:bg-dark-700 text-gray-700 dark:text-gray-300 hover:bg-gray-200 dark:hover:bg-dark-600'
          ]"
        >
          Decode
        </button>
      </div>

      <div class="flex items-center gap-2">
        <label class="text-sm text-gray-600 dark:text-gray-400">Type:</label>
        <select v-model="encodeType" class="tool-input w-auto py-1.5">
          <option value="component">Component (encodeURIComponent)</option>
          <option value="full">Full URL (encodeURI)</option>
        </select>
      </div>
    </div>

    <div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
      <!-- Input -->
      <div class="space-y-2">
        <div class="flex items-center justify-between">
          <label class="text-sm font-medium text-gray-700 dark:text-gray-300">Input</label>
          <span class="text-xs text-gray-500">{{ input.length }} chars</span>
        </div>
        <textarea
          v-model="input"
          :placeholder="mode === 'encode' ? 'Enter text or URL to encode...' : 'Enter URL-encoded text to decode...'"
          class="tool-textarea h-64"
        />
      </div>

      <!-- Output -->
      <div class="space-y-2">
        <div class="flex items-center justify-between">
          <label class="text-sm font-medium text-gray-700 dark:text-gray-300">Output</label>
          <CopyButton v-if="output" :text="output" />
        </div>
        <textarea
          :value="output"
          readonly
          placeholder="Output will appear here..."
          class="tool-textarea h-64 bg-gray-50 dark:bg-dark-800"
        />
        <p v-if="error" class="text-sm text-red-500">{{ error }}</p>
      </div>
    </div>

    <!-- Actions -->
    <div class="flex gap-2 mt-6">
      <button @click="swap" class="tool-button-secondary">
        <svg class="w-4 h-4 inline mr-1" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M7.5 21L3 16.5m0 0L7.5 12M3 16.5h13.5m0-13.5L21 7.5m0 0L16.5 12M21 7.5H7.5" />
        </svg>
        Swap
      </button>
      <button @click="clear" class="tool-button-secondary">
        Clear
      </button>
    </div>
  </ToolLayout>
</template>
