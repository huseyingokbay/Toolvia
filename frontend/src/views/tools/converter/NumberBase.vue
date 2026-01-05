<script setup lang="ts">
import { ref, computed, watch } from 'vue'
import ToolLayout from '../../../components/common/ToolLayout.vue'
import CopyButton from '../../../components/common/CopyButton.vue'

const input = ref('')
const fromBase = ref(10)
const error = ref('')

const bases = [
  { value: 2, name: 'Binary (2)', prefix: '0b' },
  { value: 8, name: 'Octal (8)', prefix: '0o' },
  { value: 10, name: 'Decimal (10)', prefix: '' },
  { value: 16, name: 'Hexadecimal (16)', prefix: '0x' },
]

const isValidForBase = (str: string, base: number): boolean => {
  if (!str) return true
  const cleanStr = str.replace(/^(0b|0o|0x)/i, '').replace(/\s/g, '')
  const chars = '0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ'.slice(0, base)
  const regex = new RegExp(`^[${chars}]+$`, 'i')
  return regex.test(cleanStr)
}

const parseInput = (str: string, base: number): number | null => {
  if (!str.trim()) return null
  const cleanStr = str.replace(/^(0b|0o|0x)/i, '').replace(/\s/g, '')
  const num = parseInt(cleanStr, base)
  if (isNaN(num)) return null
  return num
}

const decimalValue = computed(() => {
  error.value = ''
  if (!input.value.trim()) return null

  if (!isValidForBase(input.value, fromBase.value)) {
    error.value = `Invalid characters for base ${fromBase.value}`
    return null
  }

  return parseInput(input.value, fromBase.value)
})

const conversions = computed(() => {
  if (decimalValue.value === null) return []

  return bases.map(base => ({
    ...base,
    result: decimalValue.value!.toString(base.value).toUpperCase(),
    formatted: formatNumber(decimalValue.value!.toString(base.value).toUpperCase(), base.value)
  }))
})

const formatNumber = (str: string, base: number): string => {
  if (base === 2) {
    return str.replace(/(.{4})/g, '$1 ').trim()
  }
  if (base === 16) {
    return str.replace(/(.{2})/g, '$1 ').trim()
  }
  return str
}

const setFromBase = (base: number) => {
  if (decimalValue.value !== null) {
    input.value = decimalValue.value.toString(base).toUpperCase()
  }
  fromBase.value = base
}

const clear = () => {
  input.value = ''
  error.value = ''
}
</script>

<template>
  <ToolLayout title="Number Base Converter" description="Convert numbers between binary, octal, decimal, and hexadecimal">
    <div class="space-y-6">
      <!-- Input Section -->
      <div class="space-y-4">
        <div class="space-y-2">
          <label class="text-sm font-medium text-gray-700 dark:text-gray-300">Input Number</label>
          <input
            v-model="input"
            type="text"
            placeholder="Enter a number..."
            class="tool-input"
            :class="{ 'border-red-500 focus:ring-red-500': error }"
          />
          <p v-if="error" class="text-sm text-red-500">{{ error }}</p>
        </div>

        <div class="space-y-2">
          <label class="text-sm font-medium text-gray-700 dark:text-gray-300">Input Base</label>
          <div class="flex flex-wrap gap-2">
            <button
              v-for="base in bases"
              :key="base.value"
              @click="setFromBase(base.value)"
              :class="[
                'px-4 py-2 rounded-lg text-sm font-medium transition-colors',
                fromBase === base.value
                  ? 'bg-primary-600 text-white'
                  : 'bg-gray-100 dark:bg-dark-700 text-gray-700 dark:text-gray-300 hover:bg-gray-200 dark:hover:bg-dark-600'
              ]"
            >
              {{ base.name }}
            </button>
          </div>
        </div>
      </div>

      <!-- Results -->
      <div v-if="decimalValue !== null" class="space-y-3">
        <h3 class="text-sm font-medium text-gray-700 dark:text-gray-300">Conversions</h3>
        <div class="grid gap-3">
          <div
            v-for="conv in conversions"
            :key="conv.value"
            :class="[
              'flex items-center justify-between p-4 rounded-lg',
              conv.value === fromBase
                ? 'bg-primary-50 dark:bg-primary-900/20 border-2 border-primary-200 dark:border-primary-800'
                : 'bg-gray-50 dark:bg-dark-800'
            ]"
          >
            <div class="min-w-0 flex-1">
              <div class="flex items-center gap-2 mb-1">
                <span class="text-xs text-gray-500 dark:text-gray-400">{{ conv.name }}</span>
                <span v-if="conv.value === fromBase" class="text-xs px-1.5 py-0.5 bg-primary-100 dark:bg-primary-900/30 text-primary-700 dark:text-primary-400 rounded">
                  Input
                </span>
              </div>
              <div class="font-mono text-lg text-gray-900 dark:text-white">
                <span v-if="conv.prefix" class="text-gray-400 dark:text-gray-500">{{ conv.prefix }}</span>{{ conv.formatted }}
              </div>
            </div>
            <CopyButton :text="conv.prefix + conv.result" class="ml-3 flex-shrink-0" />
          </div>
        </div>
      </div>

      <!-- Empty State -->
      <div v-else-if="!error" class="text-center py-12 text-gray-500 dark:text-gray-400">
        <svg class="w-12 h-12 mx-auto mb-4 opacity-50" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 7h6m0 10v-3m-3 3h.01M9 17h.01M9 14h.01M12 14h.01M15 11h.01M12 11h.01M9 11h.01M7 21h10a2 2 0 002-2V5a2 2 0 00-2-2H7a2 2 0 00-2 2v14a2 2 0 002 2z" />
        </svg>
        <p>Enter a number above to see conversions</p>
      </div>

      <!-- Actions -->
      <div class="flex gap-2">
        <button @click="clear" class="tool-button-secondary">
          Clear
        </button>
      </div>

      <!-- Info -->
      <div class="p-4 bg-gray-50 dark:bg-dark-800 rounded-lg">
        <h3 class="text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">Tips</h3>
        <ul class="text-sm text-gray-600 dark:text-gray-400 space-y-1">
          <li>• Binary: Uses digits 0-1</li>
          <li>• Octal: Uses digits 0-7</li>
          <li>• Decimal: Uses digits 0-9</li>
          <li>• Hexadecimal: Uses digits 0-9 and A-F</li>
        </ul>
      </div>
    </div>
  </ToolLayout>
</template>
