<script setup lang="ts">
import { ref } from 'vue'
import ToolLayout from '../../../components/common/ToolLayout.vue'
import CopyButton from '../../../components/common/CopyButton.vue'

const count = ref(5)
const uppercase = ref(false)
const noDashes = ref(false)
const uuids = ref<string[]>([])

const generateUUID = (): string => {
  let uuid = 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, (c) => {
    const r = (Math.random() * 16) | 0
    const v = c === 'x' ? r : (r & 0x3) | 0x8
    return v.toString(16)
  })

  if (noDashes.value) {
    uuid = uuid.replace(/-/g, '')
  }
  if (uppercase.value) {
    uuid = uuid.toUpperCase()
  }

  return uuid
}

const generate = () => {
  uuids.value = Array.from({ length: count.value }, () => generateUUID())
}

const copyAll = async () => {
  await navigator.clipboard.writeText(uuids.value.join('\n'))
}

// Generate initial UUIDs
generate()
</script>

<template>
  <ToolLayout title="UUID Generator" description="Generate random UUIDs (Universally Unique Identifiers)">
    <div class="space-y-6">
      <!-- Options -->
      <div class="flex flex-wrap items-center gap-4">
        <div class="flex items-center gap-2">
          <label class="text-sm text-gray-600 dark:text-gray-400">Count:</label>
          <select v-model.number="count" class="tool-input w-auto py-1.5">
            <option v-for="n in [1, 5, 10, 20, 50]" :key="n" :value="n">{{ n }}</option>
          </select>
        </div>

        <label class="flex items-center gap-2 text-sm text-gray-600 dark:text-gray-400">
          <input
            type="checkbox"
            v-model="uppercase"
            class="rounded border-gray-300 dark:border-dark-600 text-primary-600 focus:ring-primary-500"
          />
          Uppercase
        </label>

        <label class="flex items-center gap-2 text-sm text-gray-600 dark:text-gray-400">
          <input
            type="checkbox"
            v-model="noDashes"
            class="rounded border-gray-300 dark:border-dark-600 text-primary-600 focus:ring-primary-500"
          />
          No dashes
        </label>

        <button @click="generate" class="tool-button">
          Generate
        </button>
      </div>

      <!-- UUIDs List -->
      <div class="space-y-2">
        <div class="flex items-center justify-between">
          <label class="text-sm font-medium text-gray-700 dark:text-gray-300">Generated UUIDs</label>
          <button
            v-if="uuids.length"
            @click="copyAll"
            class="text-sm text-primary-600 dark:text-primary-400 hover:underline"
          >
            Copy all
          </button>
        </div>

        <div class="space-y-2 max-h-[500px] overflow-y-auto scrollbar-thin">
          <div
            v-for="(uuid, index) in uuids"
            :key="index"
            class="flex items-center justify-between p-3 bg-gray-50 dark:bg-dark-800 rounded-lg group"
          >
            <code class="font-mono text-sm text-gray-900 dark:text-white">{{ uuid }}</code>
            <CopyButton :text="uuid" class="opacity-0 group-hover:opacity-100 transition-opacity" />
          </div>
        </div>
      </div>

      <!-- Info -->
      <div class="p-4 bg-gray-50 dark:bg-dark-800 rounded-lg">
        <h3 class="text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">About UUID v4</h3>
        <p class="text-sm text-gray-600 dark:text-gray-400">
          UUID v4 is a randomly generated 128-bit identifier. The probability of generating a duplicate
          is extremely low (about 1 in 5.3 × 10^36 for each pair).
        </p>
      </div>
    </div>
  </ToolLayout>
</template>
