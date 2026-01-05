<script setup lang="ts">
import { ref } from 'vue'

const props = defineProps<{
  text: string
  label?: string
}>()

const copied = ref(false)

const copy = async () => {
  try {
    await navigator.clipboard.writeText(props.text)
    copied.value = true
    setTimeout(() => {
      copied.value = false
    }, 2000)
  } catch (err) {
    console.error('Failed to copy:', err)
  }
}
</script>

<template>
  <button
    @click="copy"
    class="inline-flex items-center gap-1.5 px-3 py-1.5 text-sm font-medium rounded-lg transition-colors"
    :class="copied
      ? 'bg-green-100 dark:bg-green-900/30 text-green-700 dark:text-green-400'
      : 'bg-gray-100 dark:bg-dark-700 text-gray-700 dark:text-gray-300 hover:bg-gray-200 dark:hover:bg-dark-600'"
  >
    <svg v-if="!copied" class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 16H6a2 2 0 01-2-2V6a2 2 0 012-2h8a2 2 0 012 2v2m-6 12h8a2 2 0 002-2v-8a2 2 0 00-2-2h-8a2 2 0 00-2 2v8a2 2 0 002 2z" />
    </svg>
    <svg v-else class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M5 13l4 4L19 7" />
    </svg>
    <span>{{ copied ? 'Copied!' : (label || 'Copy') }}</span>
  </button>
</template>
