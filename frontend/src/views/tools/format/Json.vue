<script setup lang="ts">
import { ref, computed } from 'vue'
import ToolLayout from '../../../components/common/ToolLayout.vue'
import CopyButton from '../../../components/common/CopyButton.vue'

const input = ref('')
const indentSize = ref(2)
const error = ref('')

const parsedJson = computed(() => {
  if (!input.value.trim()) return null
  try {
    error.value = ''
    return JSON.parse(input.value)
  } catch (e: any) {
    error.value = e.message
    return null
  }
})

const formatted = computed(() => {
  if (parsedJson.value === null) return ''
  return JSON.stringify(parsedJson.value, null, indentSize.value)
})

const minified = computed(() => {
  if (parsedJson.value === null) return ''
  return JSON.stringify(parsedJson.value)
})

const stats = computed(() => {
  if (!formatted.value) return null
  const lines = formatted.value.split('\n').length
  const chars = formatted.value.length
  const minChars = minified.value.length
  return { lines, chars, minChars }
})

const format = () => {
  if (parsedJson.value !== null) {
    input.value = formatted.value
  }
}

const minify = () => {
  if (parsedJson.value !== null) {
    input.value = minified.value
  }
}

const unescape = () => {
  const trimmed = input.value.trim()
  if (!trimmed) return

  try {
    // First, try to parse as a JSON string (e.g., "{\"key\": \"value\"}")
    let unescaped = trimmed

    // If it starts and ends with quotes, it might be a stringified JSON
    if ((trimmed.startsWith('"') && trimmed.endsWith('"')) ||
        (trimmed.startsWith("'") && trimmed.endsWith("'"))) {
      // Try to parse as JSON string first
      try {
        unescaped = JSON.parse(trimmed)
      } catch {
        // If that fails, manually unescape common patterns
        unescaped = trimmed.slice(1, -1)
          .replace(/\\"/g, '"')
          .replace(/\\'/g, "'")
          .replace(/\\\\/g, '\\')
          .replace(/\\n/g, '\n')
          .replace(/\\r/g, '\r')
          .replace(/\\t/g, '\t')
      }
    } else {
      // Not wrapped in quotes, just unescape the content
      unescaped = trimmed
        .replace(/\\"/g, '"')
        .replace(/\\'/g, "'")
        .replace(/\\\\/g, '\\')
        .replace(/\\n/g, '\n')
        .replace(/\\r/g, '\r')
        .replace(/\\t/g, '\t')
    }

    // Verify it's valid JSON and format it
    const parsed = JSON.parse(unescaped)
    input.value = JSON.stringify(parsed, null, indentSize.value)
    error.value = ''
  } catch (e: any) {
    error.value = 'Failed to unescape: ' + e.message
  }
}

const clear = () => {
  input.value = ''
  error.value = ''
}

const loadSample = () => {
  input.value = `{
  "name": "MultiTools",
  "version": "1.0.0",
  "features": ["encoding", "hash", "format"],
  "config": {
    "theme": "dark",
    "language": "en"
  },
  "active": true
}`
}
</script>

<template>
  <ToolLayout title="JSON Formatter" description="Format, validate, and minify JSON data">
    <div class="space-y-6">
      <!-- Options -->
      <div class="flex flex-wrap items-center gap-4">
        <div class="flex items-center gap-2">
          <label class="text-sm text-gray-600 dark:text-gray-400">Indent:</label>
          <select v-model.number="indentSize" class="tool-input w-auto py-1.5">
            <option :value="2">2 spaces</option>
            <option :value="4">4 spaces</option>
            <option :value="1">Tab</option>
          </select>
        </div>

        <div class="flex gap-2">
          <button @click="format" class="tool-button" :disabled="!parsedJson">
            Format
          </button>
          <button @click="minify" class="tool-button-secondary" :disabled="!parsedJson">
            Minify
          </button>
          <button @click="unescape" class="tool-button-secondary">
            Unescape
          </button>
          <button @click="loadSample" class="tool-button-secondary">
            Sample
          </button>
          <button @click="clear" class="tool-button-secondary">
            Clear
          </button>
        </div>
      </div>

      <!-- Editor -->
      <div class="space-y-2">
        <div class="flex items-center justify-between">
          <label class="text-sm font-medium text-gray-700 dark:text-gray-300">JSON Input</label>
          <div class="flex items-center gap-3">
            <span v-if="stats" class="text-xs text-gray-500">
              {{ stats.lines }} lines | {{ stats.chars }} chars (minified: {{ stats.minChars }})
            </span>
            <CopyButton v-if="formatted" :text="formatted" />
          </div>
        </div>
        <textarea
          v-model="input"
          placeholder='Paste JSON here (e.g., {"key": "value"})'
          class="tool-textarea h-96 font-mono text-sm"
          :class="{ 'border-red-500 focus:ring-red-500': error }"
          spellcheck="false"
        />
      </div>

      <!-- Validation Status -->
      <div
        :class="[
          'flex items-center gap-2 p-3 rounded-lg text-sm',
          error
            ? 'bg-red-50 dark:bg-red-900/20 text-red-700 dark:text-red-400'
            : parsedJson !== null
              ? 'bg-green-50 dark:bg-green-900/20 text-green-700 dark:text-green-400'
              : 'bg-gray-50 dark:bg-dark-800 text-gray-500 dark:text-gray-400'
        ]"
      >
        <svg v-if="error" class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8v4m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
        </svg>
        <svg v-else-if="parsedJson !== null" class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z" />
        </svg>
        <svg v-else class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13 16h-1v-4h-1m1-4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
        </svg>
        <span v-if="error">{{ error }}</span>
        <span v-else-if="parsedJson !== null">Valid JSON</span>
        <span v-else>Enter JSON to validate</span>
      </div>
    </div>
  </ToolLayout>
</template>
