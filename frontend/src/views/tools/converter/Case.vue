<script setup lang="ts">
import { ref, computed } from 'vue'
import ToolLayout from '../../../components/common/ToolLayout.vue'
import CopyButton from '../../../components/common/CopyButton.vue'

const input = ref('')

const toTitleCase = (str: string): string => {
  return str.toLowerCase().replace(/(?:^|\s|[-_])\w/g, match => match.toUpperCase())
}

const toSentenceCase = (str: string): string => {
  return str.toLowerCase().replace(/(^\w|[.!?]\s+\w)/g, match => match.toUpperCase())
}

const toCamelCase = (str: string): string => {
  return str.toLowerCase()
    .replace(/[^a-zA-Z0-9]+(.)/g, (_, chr) => chr.toUpperCase())
    .replace(/^./, chr => chr.toLowerCase())
}

const toPascalCase = (str: string): string => {
  return str.toLowerCase()
    .replace(/[^a-zA-Z0-9]+(.)/g, (_, chr) => chr.toUpperCase())
    .replace(/^./, chr => chr.toUpperCase())
}

const toSnakeCase = (str: string): string => {
  return str
    .replace(/([a-z])([A-Z])/g, '$1_$2')
    .replace(/[\s\-]+/g, '_')
    .toLowerCase()
}

const toKebabCase = (str: string): string => {
  return str
    .replace(/([a-z])([A-Z])/g, '$1-$2')
    .replace(/[\s_]+/g, '-')
    .toLowerCase()
}

const toConstantCase = (str: string): string => {
  return str
    .replace(/([a-z])([A-Z])/g, '$1_$2')
    .replace(/[\s\-]+/g, '_')
    .toUpperCase()
}

const toDotCase = (str: string): string => {
  return str
    .replace(/([a-z])([A-Z])/g, '$1.$2')
    .replace(/[\s_\-]+/g, '.')
    .toLowerCase()
}

const toPathCase = (str: string): string => {
  return str
    .replace(/([a-z])([A-Z])/g, '$1/$2')
    .replace(/[\s_\-]+/g, '/')
    .toLowerCase()
}

interface CaseOption {
  id: string
  name: string
  example: string
  transform: (str: string) => string
}

const caseOptions: CaseOption[] = [
  { id: 'lower', name: 'lowercase', example: 'hello world', transform: (s) => s.toLowerCase() },
  { id: 'upper', name: 'UPPERCASE', example: 'HELLO WORLD', transform: (s) => s.toUpperCase() },
  { id: 'title', name: 'Title Case', example: 'Hello World', transform: toTitleCase },
  { id: 'sentence', name: 'Sentence case', example: 'Hello world', transform: toSentenceCase },
  { id: 'camel', name: 'camelCase', example: 'helloWorld', transform: toCamelCase },
  { id: 'pascal', name: 'PascalCase', example: 'HelloWorld', transform: toPascalCase },
  { id: 'snake', name: 'snake_case', example: 'hello_world', transform: toSnakeCase },
  { id: 'kebab', name: 'kebab-case', example: 'hello-world', transform: toKebabCase },
  { id: 'constant', name: 'CONSTANT_CASE', example: 'HELLO_WORLD', transform: toConstantCase },
  { id: 'dot', name: 'dot.case', example: 'hello.world', transform: toDotCase },
  { id: 'path', name: 'path/case', example: 'hello/world', transform: toPathCase },
]

const results = computed(() => {
  if (!input.value) return []
  return caseOptions.map(opt => ({
    ...opt,
    result: opt.transform(input.value)
  }))
})

const clear = () => {
  input.value = ''
}
</script>

<template>
  <ToolLayout title="Case Converter" description="Convert text between different case styles">
    <div class="space-y-6">
      <!-- Input -->
      <div class="space-y-2">
        <div class="flex items-center justify-between">
          <label class="text-sm font-medium text-gray-700 dark:text-gray-300">Input Text</label>
          <span class="text-xs text-gray-500">{{ input.length }} chars</span>
        </div>
        <textarea
          v-model="input"
          placeholder="Enter text to convert (e.g., 'Hello World' or 'helloWorld')..."
          class="tool-textarea h-32"
        />
        <button @click="clear" class="tool-button-secondary text-sm">
          Clear
        </button>
      </div>

      <!-- Results -->
      <div v-if="input" class="space-y-3">
        <h3 class="text-sm font-medium text-gray-700 dark:text-gray-300">Conversions</h3>
        <div class="grid gap-3">
          <div
            v-for="opt in results"
            :key="opt.id"
            class="flex items-center justify-between p-3 bg-gray-50 dark:bg-dark-800 rounded-lg"
          >
            <div class="min-w-0 flex-1">
              <div class="text-xs text-gray-500 dark:text-gray-400 mb-1">{{ opt.name }}</div>
              <div class="font-mono text-sm text-gray-900 dark:text-white truncate">{{ opt.result }}</div>
            </div>
            <CopyButton :text="opt.result" class="ml-3 flex-shrink-0" />
          </div>
        </div>
      </div>

      <!-- Empty State -->
      <div v-else class="text-center py-12 text-gray-500 dark:text-gray-400">
        <svg class="w-12 h-12 mx-auto mb-4 opacity-50" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z" />
        </svg>
        <p>Enter text above to see conversions</p>
      </div>
    </div>
  </ToolLayout>
</template>
