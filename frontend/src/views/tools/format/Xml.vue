<script setup lang="ts">
import { ref, computed } from 'vue'
import ToolLayout from '../../../components/common/ToolLayout.vue'
import CopyButton from '../../../components/common/CopyButton.vue'

const input = ref('')
const indentSize = ref(2)
const error = ref('')

const formatXml = (xml: string, indent: number): string => {
  const PADDING = ' '.repeat(indent)
  const reg = /(>)(<)(\/*)/g
  let formatted = ''
  let pad = 0

  xml = xml.replace(reg, '$1\r\n$2$3')
  xml.split('\r\n').forEach((node) => {
    let indentLevel = 0
    if (node.match(/.+<\/\w[^>]*>$/)) {
      indentLevel = 0
    } else if (node.match(/^<\/\w/) && pad !== 0) {
      pad -= 1
    } else if (node.match(/^<\w([^>]*[^\/])?>.*$/)) {
      indentLevel = 1
    }
    formatted += PADDING.repeat(pad) + node + '\r\n'
    pad += indentLevel
  })

  return formatted.trim()
}

const minifyXml = (xml: string): string => {
  return xml
    .replace(/>\s+</g, '><')
    .replace(/\s+/g, ' ')
    .replace(/>\s+/g, '>')
    .replace(/\s+</g, '<')
    .trim()
}

const validateXml = (xml: string): { valid: boolean; error?: string } => {
  try {
    const parser = new DOMParser()
    const doc = parser.parseFromString(xml, 'application/xml')
    const parseError = doc.querySelector('parsererror')
    if (parseError) {
      return { valid: false, error: parseError.textContent || 'Invalid XML' }
    }
    return { valid: true }
  } catch (e: any) {
    return { valid: false, error: e.message }
  }
}

const validation = computed(() => {
  if (!input.value.trim()) return { valid: false, error: '' }
  return validateXml(input.value)
})

const formatted = computed(() => {
  if (!validation.value.valid) return ''
  return formatXml(input.value, indentSize.value)
})

const format = () => {
  if (validation.value.valid) {
    input.value = formatted.value
    error.value = ''
  }
}

const minify = () => {
  if (validation.value.valid) {
    input.value = minifyXml(input.value)
    error.value = ''
  }
}

const clear = () => {
  input.value = ''
  error.value = ''
}

const loadSample = () => {
  input.value = `<?xml version="1.0" encoding="UTF-8"?>
<root>
  <person id="1">
    <name>John Doe</name>
    <email>john@example.com</email>
    <roles>
      <role>admin</role>
      <role>user</role>
    </roles>
  </person>
</root>`
}
</script>

<template>
  <ToolLayout title="XML Formatter" description="Format, validate, and minify XML data">
    <div class="space-y-6">
      <!-- Options -->
      <div class="flex flex-wrap items-center gap-4">
        <div class="flex items-center gap-2">
          <label class="text-sm text-gray-600 dark:text-gray-400">Indent:</label>
          <select v-model.number="indentSize" class="tool-input w-auto py-1.5">
            <option :value="2">2 spaces</option>
            <option :value="4">4 spaces</option>
          </select>
        </div>

        <div class="flex gap-2">
          <button @click="format" class="tool-button" :disabled="!validation.valid">
            Format
          </button>
          <button @click="minify" class="tool-button-secondary" :disabled="!validation.valid">
            Minify
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
          <label class="text-sm font-medium text-gray-700 dark:text-gray-300">XML Input</label>
          <CopyButton v-if="formatted" :text="formatted" />
        </div>
        <textarea
          v-model="input"
          placeholder="Paste XML here..."
          class="tool-textarea h-96 font-mono text-sm"
          :class="{ 'border-red-500 focus:ring-red-500': validation.error && input }"
          spellcheck="false"
        />
      </div>

      <!-- Validation Status -->
      <div
        :class="[
          'flex items-center gap-2 p-3 rounded-lg text-sm',
          validation.error && input
            ? 'bg-red-50 dark:bg-red-900/20 text-red-700 dark:text-red-400'
            : validation.valid
              ? 'bg-green-50 dark:bg-green-900/20 text-green-700 dark:text-green-400'
              : 'bg-gray-50 dark:bg-dark-800 text-gray-500 dark:text-gray-400'
        ]"
      >
        <svg v-if="validation.error && input" class="w-5 h-5 flex-shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8v4m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
        </svg>
        <svg v-else-if="validation.valid" class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z" />
        </svg>
        <svg v-else class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13 16h-1v-4h-1m1-4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
        </svg>
        <span v-if="validation.error && input" class="truncate">{{ validation.error }}</span>
        <span v-else-if="validation.valid">Valid XML</span>
        <span v-else>Enter XML to validate</span>
      </div>
    </div>
  </ToolLayout>
</template>
