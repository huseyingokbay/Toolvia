<script setup lang="ts">
import { ref, watch } from 'vue'
import ToolLayout from '../../../components/common/ToolLayout.vue'
import CopyButton from '../../../components/common/CopyButton.vue'

const input = ref('')
const output = ref('')
const mode = ref<'encode' | 'decode'>('encode')

const htmlEntities: Record<string, string> = {
  '&': '&amp;',
  '<': '&lt;',
  '>': '&gt;',
  '"': '&quot;',
  "'": '&#39;',
  '/': '&#x2F;',
  '`': '&#x60;',
  '=': '&#x3D;'
}

const encodeHtml = (text: string): string => {
  return text.replace(/[&<>"'`=/]/g, char => htmlEntities[char] || char)
}

const decodeHtml = (text: string): string => {
  const textarea = document.createElement('textarea')
  textarea.innerHTML = text
  return textarea.value
}

const process = () => {
  if (!input.value) {
    output.value = ''
    return
  }

  if (mode.value === 'encode') {
    output.value = encodeHtml(input.value)
  } else {
    output.value = decodeHtml(input.value)
  }
}

watch([input, mode], process, { immediate: true })

const swap = () => {
  const temp = input.value
  input.value = output.value
  output.value = temp
  mode.value = mode.value === 'encode' ? 'decode' : 'encode'
}

const clear = () => {
  input.value = ''
  output.value = ''
}
</script>

<template>
  <ToolLayout title="HTML Encoder/Decoder" description="Encode special characters to HTML entities or decode them">
    <!-- Mode Toggle -->
    <div class="flex gap-2 mb-6">
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

    <div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
      <!-- Input -->
      <div class="space-y-2">
        <div class="flex items-center justify-between">
          <label class="text-sm font-medium text-gray-700 dark:text-gray-300">Input</label>
          <span class="text-xs text-gray-500">{{ input.length }} chars</span>
        </div>
        <textarea
          v-model="input"
          :placeholder="mode === 'encode' ? 'Enter HTML to encode (e.g., <div>Hello</div>)' : 'Enter HTML entities to decode (e.g., &lt;div&gt;)'"
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

    <!-- Info -->
    <div class="mt-6 p-4 bg-gray-50 dark:bg-dark-800 rounded-lg">
      <h3 class="text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">Encoded Characters:</h3>
      <div class="grid grid-cols-2 sm:grid-cols-4 gap-2 text-sm text-gray-600 dark:text-gray-400 font-mono">
        <div>&amp; → &amp;amp;</div>
        <div>&lt; → &amp;lt;</div>
        <div>&gt; → &amp;gt;</div>
        <div>" → &amp;quot;</div>
        <div>' → &amp;#39;</div>
        <div>/ → &amp;#x2F;</div>
        <div>` → &amp;#x60;</div>
        <div>= → &amp;#x3D;</div>
      </div>
    </div>
  </ToolLayout>
</template>
