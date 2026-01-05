<script setup lang="ts">
import { ref, computed } from 'vue'
import ToolLayout from '../../../components/common/ToolLayout.vue'

const text1 = ref('')
const text2 = ref('')
const ignoreCase = ref(false)
const ignoreWhitespace = ref(false)

interface DiffLine {
  type: 'added' | 'removed' | 'unchanged'
  content: string
  lineNumber1?: number
  lineNumber2?: number
}

const normalizeText = (text: string): string => {
  let result = text
  if (ignoreCase.value) {
    result = result.toLowerCase()
  }
  if (ignoreWhitespace.value) {
    result = result.replace(/\s+/g, ' ').trim()
  }
  return result
}

const computeDiff = (lines1: string[], lines2: string[]): DiffLine[] => {
  const result: DiffLine[] = []
  const m = lines1.length
  const n = lines2.length

  // LCS table
  const dp: number[][] = Array(m + 1).fill(null).map(() => Array(n + 1).fill(0))

  for (let i = 1; i <= m; i++) {
    for (let j = 1; j <= n; j++) {
      if (normalizeText(lines1[i - 1]) === normalizeText(lines2[j - 1])) {
        dp[i][j] = dp[i - 1][j - 1] + 1
      } else {
        dp[i][j] = Math.max(dp[i - 1][j], dp[i][j - 1])
      }
    }
  }

  // Backtrack to find diff
  let i = m, j = n
  const diff: DiffLine[] = []

  while (i > 0 || j > 0) {
    if (i > 0 && j > 0 && normalizeText(lines1[i - 1]) === normalizeText(lines2[j - 1])) {
      diff.unshift({
        type: 'unchanged',
        content: lines1[i - 1],
        lineNumber1: i,
        lineNumber2: j
      })
      i--
      j--
    } else if (j > 0 && (i === 0 || dp[i][j - 1] >= dp[i - 1][j])) {
      diff.unshift({
        type: 'added',
        content: lines2[j - 1],
        lineNumber2: j
      })
      j--
    } else {
      diff.unshift({
        type: 'removed',
        content: lines1[i - 1],
        lineNumber1: i
      })
      i--
    }
  }

  return diff
}

const diff = computed(() => {
  const lines1 = text1.value.split('\n')
  const lines2 = text2.value.split('\n')
  return computeDiff(lines1, lines2)
})

const stats = computed(() => {
  const added = diff.value.filter(d => d.type === 'added').length
  const removed = diff.value.filter(d => d.type === 'removed').length
  const unchanged = diff.value.filter(d => d.type === 'unchanged').length
  return { added, removed, unchanged }
})

const clear = () => {
  text1.value = ''
  text2.value = ''
}

const swap = () => {
  const temp = text1.value
  text1.value = text2.value
  text2.value = temp
}

const loadSample = () => {
  text1.value = `function greet(name) {
  console.log("Hello, " + name);
  return true;
}

const result = greet("World");`

  text2.value = `function greet(name, greeting = "Hello") {
  console.log(greeting + ", " + name + "!");
  return true;
}

const message = greet("World", "Hi");
console.log(message);`
}
</script>

<template>
  <ToolLayout title="Text Diff" description="Compare two texts and highlight differences">
    <div class="space-y-6">
      <!-- Options -->
      <div class="flex flex-wrap items-center gap-4">
        <label class="flex items-center gap-2 text-sm text-gray-600 dark:text-gray-400">
          <input
            type="checkbox"
            v-model="ignoreCase"
            class="rounded border-gray-300 dark:border-dark-600 text-primary-600 focus:ring-primary-500"
          />
          Ignore case
        </label>

        <label class="flex items-center gap-2 text-sm text-gray-600 dark:text-gray-400">
          <input
            type="checkbox"
            v-model="ignoreWhitespace"
            class="rounded border-gray-300 dark:border-dark-600 text-primary-600 focus:ring-primary-500"
          />
          Ignore whitespace
        </label>

        <div class="flex gap-2 ml-auto">
          <button @click="swap" class="tool-button-secondary">
            Swap
          </button>
          <button @click="loadSample" class="tool-button-secondary">
            Sample
          </button>
          <button @click="clear" class="tool-button-secondary">
            Clear
          </button>
        </div>
      </div>

      <!-- Input Areas -->
      <div class="grid grid-cols-1 lg:grid-cols-2 gap-4">
        <div class="space-y-2">
          <label class="text-sm font-medium text-gray-700 dark:text-gray-300">Original Text</label>
          <textarea
            v-model="text1"
            placeholder="Paste original text here..."
            class="tool-textarea h-48 font-mono text-sm"
          />
        </div>

        <div class="space-y-2">
          <label class="text-sm font-medium text-gray-700 dark:text-gray-300">Modified Text</label>
          <textarea
            v-model="text2"
            placeholder="Paste modified text here..."
            class="tool-textarea h-48 font-mono text-sm"
          />
        </div>
      </div>

      <!-- Stats -->
      <div v-if="text1 || text2" class="flex items-center gap-4 text-sm">
        <span class="px-2 py-1 rounded bg-green-100 dark:bg-green-900/30 text-green-700 dark:text-green-400">
          +{{ stats.added }} added
        </span>
        <span class="px-2 py-1 rounded bg-red-100 dark:bg-red-900/30 text-red-700 dark:text-red-400">
          -{{ stats.removed }} removed
        </span>
        <span class="px-2 py-1 rounded bg-gray-100 dark:bg-dark-700 text-gray-600 dark:text-gray-400">
          {{ stats.unchanged }} unchanged
        </span>
      </div>

      <!-- Diff Output -->
      <div v-if="diff.length" class="space-y-2">
        <label class="text-sm font-medium text-gray-700 dark:text-gray-300">Differences</label>
        <div class="border border-gray-200 dark:border-dark-700 rounded-lg overflow-hidden">
          <div class="max-h-96 overflow-y-auto scrollbar-thin">
            <div
              v-for="(line, index) in diff"
              :key="index"
              :class="[
                'flex font-mono text-sm border-b border-gray-100 dark:border-dark-800 last:border-b-0',
                line.type === 'added' ? 'bg-green-50 dark:bg-green-900/20' :
                line.type === 'removed' ? 'bg-red-50 dark:bg-red-900/20' :
                'bg-white dark:bg-dark-900'
              ]"
            >
              <!-- Line Numbers -->
              <div class="flex-shrink-0 w-20 flex text-xs text-gray-400 dark:text-gray-500 border-r border-gray-200 dark:border-dark-700">
                <span class="w-10 px-2 py-1 text-right bg-gray-50 dark:bg-dark-800">
                  {{ line.lineNumber1 || '' }}
                </span>
                <span class="w-10 px-2 py-1 text-right bg-gray-50 dark:bg-dark-800">
                  {{ line.lineNumber2 || '' }}
                </span>
              </div>

              <!-- Indicator -->
              <div class="flex-shrink-0 w-6 flex items-center justify-center">
                <span
                  :class="[
                    'font-bold',
                    line.type === 'added' ? 'text-green-600 dark:text-green-400' :
                    line.type === 'removed' ? 'text-red-600 dark:text-red-400' :
                    'text-gray-400'
                  ]"
                >
                  {{ line.type === 'added' ? '+' : line.type === 'removed' ? '-' : ' ' }}
                </span>
              </div>

              <!-- Content -->
              <div
                :class="[
                  'flex-1 px-2 py-1 whitespace-pre',
                  line.type === 'added' ? 'text-green-800 dark:text-green-200' :
                  line.type === 'removed' ? 'text-red-800 dark:text-red-200' :
                  'text-gray-700 dark:text-gray-300'
                ]"
              >
                {{ line.content || ' ' }}
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Empty State -->
      <div v-else-if="!text1 && !text2" class="text-center py-12 text-gray-500 dark:text-gray-400">
        <svg class="w-12 h-12 mx-auto mb-4 opacity-50" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5H7a2 2 0 00-2 2v12a2 2 0 002 2h10a2 2 0 002-2V7a2 2 0 00-2-2h-2M9 5a2 2 0 002 2h2a2 2 0 002-2M9 5a2 2 0 012-2h2a2 2 0 012 2" />
        </svg>
        <p>Enter text in both fields to see differences</p>
      </div>
    </div>
  </ToolLayout>
</template>
