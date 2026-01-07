<script setup lang="ts">
import { ref, computed } from 'vue'
import ToolLayout from '../../../components/common/ToolLayout.vue'
import CopyButton from '../../../components/common/CopyButton.vue'

const input = ref('')
const indentSize = ref(2)
const error = ref('')

const formatHtml = (html: string, indent: number): string => {
  const PADDING = ' '.repeat(indent)
  const selfClosingTags = ['area', 'base', 'br', 'col', 'embed', 'hr', 'img', 'input', 'link', 'meta', 'param', 'source', 'track', 'wbr']

  let formatted = ''
  let pad = 0

  // Normalize and split by tags
  html = html.replace(/>\s+</g, '><').trim()
  const tokens = html.split(/(<[^>]+>)/g).filter(t => t.trim())

  tokens.forEach((token) => {
    const isTag = token.startsWith('<')

    if (isTag) {
      const isClosingTag = token.startsWith('</')
      const isSelfClosing = token.endsWith('/>') || selfClosingTags.some(tag =>
        new RegExp(`^<${tag}(\\s|>|/>)`, 'i').test(token)
      )
      const isDoctype = token.toLowerCase().startsWith('<!doctype')
      const isComment = token.startsWith('<!--')

      if (isClosingTag) {
        pad = Math.max(0, pad - 1)
        formatted += PADDING.repeat(pad) + token + '\n'
      } else if (isSelfClosing || isDoctype || isComment) {
        formatted += PADDING.repeat(pad) + token + '\n'
      } else {
        formatted += PADDING.repeat(pad) + token + '\n'
        pad++
      }
    } else {
      // Text content
      const text = token.trim()
      if (text) {
        formatted += PADDING.repeat(pad) + text + '\n'
      }
    }
  })

  return formatted.trim()
}

const minifyHtml = (html: string): string => {
  return html
    .replace(/\n/g, '')
    .replace(/\r/g, '')
    .replace(/\t/g, '')
    .replace(/>\s+</g, '><')
    .replace(/\s{2,}/g, ' ')
    .trim()
}

const validateHtml = (html: string): { valid: boolean; error?: string } => {
  try {
    const parser = new DOMParser()
    const doc = parser.parseFromString(html, 'text/html')
    const parseError = doc.querySelector('parsererror')
    if (parseError) {
      return { valid: false, error: parseError.textContent || 'Invalid HTML' }
    }
    // Check if there's actual content
    if (!html.trim()) {
      return { valid: false, error: '' }
    }
    return { valid: true }
  } catch (e: any) {
    return { valid: false, error: e.message }
  }
}

const validation = computed(() => {
  if (!input.value.trim()) return { valid: false, error: '' }
  return validateHtml(input.value)
})

const formatted = computed(() => {
  if (!input.value.trim()) return ''
  return formatHtml(input.value, indentSize.value)
})

const format = () => {
  if (input.value.trim()) {
    input.value = formatted.value
    error.value = ''
  }
}

const minify = () => {
  if (input.value.trim()) {
    input.value = minifyHtml(input.value)
    error.value = ''
  }
}

const clear = () => {
  input.value = ''
  error.value = ''
}

const loadSample = () => {
  input.value = `<!DOCTYPE html>
<html lang="en">
<head>
  <meta charset="UTF-8">
  <meta name="viewport" content="width=device-width, initial-scale=1.0">
  <title>Sample Page</title>
</head>
<body>
  <header>
    <h1>Welcome to Toolvia</h1>
    <nav>
      <a href="/">Home</a>
      <a href="/tools">Tools</a>
    </nav>
  </header>
  <main>
    <p>This is a sample HTML document.</p>
  </main>
</body>
</html>`
}
</script>

<template>
  <ToolLayout title="HTML Formatter" description="Format, beautify, and minify HTML code">
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
          <button @click="format" class="tool-button" :disabled="!input.trim()">
            Beautify
          </button>
          <button @click="minify" class="tool-button-secondary" :disabled="!input.trim()">
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
          <label class="text-sm font-medium text-gray-700 dark:text-gray-300">HTML Input</label>
          <CopyButton v-if="input.trim()" :text="input" />
        </div>
        <textarea
          v-model="input"
          placeholder="Paste HTML here..."
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
        <span v-else-if="validation.valid">Valid HTML</span>
        <span v-else>Enter HTML to format</span>
      </div>
    </div>
  </ToolLayout>
</template>
