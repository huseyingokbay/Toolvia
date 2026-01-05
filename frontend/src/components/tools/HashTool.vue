<script setup lang="ts">
import { ref, watch, computed } from 'vue'
import ToolLayout from '../common/ToolLayout.vue'
import CopyButton from '../common/CopyButton.vue'

const props = defineProps<{
  title: string
  description: string
  hashFunction: (input: string) => string
  hashLength: number
}>()

const input = ref('')
const output = ref('')
const uppercase = ref(false)

const formattedOutput = computed(() => {
  return uppercase.value ? output.value.toUpperCase() : output.value.toLowerCase()
})

const process = () => {
  if (!input.value) {
    output.value = ''
    return
  }
  output.value = props.hashFunction(input.value)
}

watch(input, process, { immediate: true })

const clear = () => {
  input.value = ''
  output.value = ''
}
</script>

<template>
  <ToolLayout :title="title" :description="description">
    <div class="space-y-6">
      <!-- Input -->
      <div class="space-y-2">
        <div class="flex items-center justify-between">
          <label class="text-sm font-medium text-gray-700 dark:text-gray-300">Input Text</label>
          <span class="text-xs text-gray-500">{{ input.length }} chars</span>
        </div>
        <textarea
          v-model="input"
          placeholder="Enter text to hash..."
          class="tool-textarea h-40"
        />
      </div>

      <!-- Output -->
      <div class="space-y-2">
        <div class="flex items-center justify-between">
          <label class="text-sm font-medium text-gray-700 dark:text-gray-300">Hash Output</label>
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
            :value="formattedOutput"
            readonly
            placeholder="Hash will appear here..."
            class="tool-input bg-gray-50 dark:bg-dark-800 pr-20"
          />
          <span v-if="output" class="absolute right-3 top-1/2 -translate-y-1/2 text-xs text-gray-400">
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
        <h3 class="text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">About {{ title.split(' ')[0] }}</h3>
        <p class="text-sm text-gray-600 dark:text-gray-400">
          <slot name="info">
            This hash function produces a {{ hashLength }}-character hexadecimal string ({{ hashLength * 4 }} bits).
          </slot>
        </p>
      </div>
    </div>
  </ToolLayout>
</template>
