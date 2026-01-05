<script setup lang="ts">
import { ref, computed, watch } from 'vue'
import ToolLayout from '../../../components/common/ToolLayout.vue'
import CopyButton from '../../../components/common/CopyButton.vue'

const length = ref(16)
const includeUppercase = ref(true)
const includeLowercase = ref(true)
const includeNumbers = ref(true)
const includeSymbols = ref(true)
const excludeAmbiguous = ref(false)
const password = ref('')
const history = ref<string[]>([])

const charSets = {
  uppercase: 'ABCDEFGHIJKLMNOPQRSTUVWXYZ',
  uppercaseUnambiguous: 'ABCDEFGHJKLMNPQRSTUVWXYZ',
  lowercase: 'abcdefghijklmnopqrstuvwxyz',
  lowercaseUnambiguous: 'abcdefghjkmnpqrstuvwxyz',
  numbers: '0123456789',
  numbersUnambiguous: '23456789',
  symbols: '!@#$%^&*()_+-=[]{}|;:,.<>?'
}

const availableChars = computed(() => {
  let chars = ''
  if (includeUppercase.value) {
    chars += excludeAmbiguous.value ? charSets.uppercaseUnambiguous : charSets.uppercase
  }
  if (includeLowercase.value) {
    chars += excludeAmbiguous.value ? charSets.lowercaseUnambiguous : charSets.lowercase
  }
  if (includeNumbers.value) {
    chars += excludeAmbiguous.value ? charSets.numbersUnambiguous : charSets.numbers
  }
  if (includeSymbols.value) {
    chars += charSets.symbols
  }
  return chars
})

const strength = computed(() => {
  if (!password.value) return { score: 0, label: 'None', color: 'gray' }

  let score = 0
  const len = password.value.length

  if (len >= 8) score += 1
  if (len >= 12) score += 1
  if (len >= 16) score += 1
  if (/[a-z]/.test(password.value)) score += 1
  if (/[A-Z]/.test(password.value)) score += 1
  if (/[0-9]/.test(password.value)) score += 1
  if (/[^a-zA-Z0-9]/.test(password.value)) score += 1

  if (score <= 2) return { score: 1, label: 'Weak', color: 'red' }
  if (score <= 4) return { score: 2, label: 'Fair', color: 'yellow' }
  if (score <= 5) return { score: 3, label: 'Good', color: 'blue' }
  return { score: 4, label: 'Strong', color: 'green' }
})

const generate = () => {
  if (!availableChars.value) {
    password.value = ''
    return
  }

  const array = new Uint32Array(length.value)
  crypto.getRandomValues(array)

  let result = ''
  for (let i = 0; i < length.value; i++) {
    result += availableChars.value[array[i] % availableChars.value.length]
  }

  password.value = result

  // Add to history
  if (result && !history.value.includes(result)) {
    history.value.unshift(result)
    if (history.value.length > 10) {
      history.value.pop()
    }
  }
}

const clearHistory = () => {
  history.value = []
}

// Generate initial password
generate()
</script>

<template>
  <ToolLayout title="Password Generator" description="Generate secure random passwords">
    <div class="space-y-6">
      <!-- Generated Password -->
      <div class="space-y-2">
        <div class="flex items-center justify-between">
          <label class="text-sm font-medium text-gray-700 dark:text-gray-300">Generated Password</label>
          <CopyButton v-if="password" :text="password" />
        </div>
        <div class="relative">
          <input
            :value="password"
            readonly
            class="tool-input text-lg font-mono tracking-wider pr-24"
          />
          <button
            @click="generate"
            class="absolute right-2 top-1/2 -translate-y-1/2 p-2 rounded-lg bg-primary-100 dark:bg-primary-900/30 text-primary-600 dark:text-primary-400 hover:bg-primary-200 dark:hover:bg-primary-900/50"
          >
            <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 4v5h.582m15.356 2A8.001 8.001 0 004.582 9m0 0H9m11 11v-5h-.581m0 0a8.003 8.003 0 01-15.357-2m15.357 2H15" />
            </svg>
          </button>
        </div>

        <!-- Strength Indicator -->
        <div class="flex items-center gap-2">
          <div class="flex-1 h-2 bg-gray-200 dark:bg-dark-700 rounded-full overflow-hidden">
            <div
              :class="[
                'h-full transition-all duration-300',
                strength.color === 'red' ? 'bg-red-500' :
                strength.color === 'yellow' ? 'bg-yellow-500' :
                strength.color === 'blue' ? 'bg-blue-500' :
                strength.color === 'green' ? 'bg-green-500' : 'bg-gray-300'
              ]"
              :style="{ width: `${strength.score * 25}%` }"
            />
          </div>
          <span
            :class="[
              'text-sm font-medium',
              strength.color === 'red' ? 'text-red-500' :
              strength.color === 'yellow' ? 'text-yellow-500' :
              strength.color === 'blue' ? 'text-blue-500' :
              strength.color === 'green' ? 'text-green-500' : 'text-gray-400'
            ]"
          >
            {{ strength.label }}
          </span>
        </div>
      </div>

      <!-- Options -->
      <div class="space-y-4 p-4 bg-gray-50 dark:bg-dark-800 rounded-lg">
        <div class="space-y-2">
          <div class="flex items-center justify-between">
            <label class="text-sm font-medium text-gray-700 dark:text-gray-300">Length: {{ length }}</label>
          </div>
          <input
            v-model.number="length"
            type="range"
            min="4"
            max="64"
            class="w-full h-2 bg-gray-200 dark:bg-dark-700 rounded-lg appearance-none cursor-pointer accent-primary-600"
          />
          <div class="flex justify-between text-xs text-gray-500">
            <span>4</span>
            <span>64</span>
          </div>
        </div>

        <div class="grid grid-cols-2 gap-3">
          <label class="flex items-center gap-2 text-sm text-gray-600 dark:text-gray-400">
            <input
              type="checkbox"
              v-model="includeUppercase"
              class="rounded border-gray-300 dark:border-dark-600 text-primary-600 focus:ring-primary-500"
            />
            Uppercase (A-Z)
          </label>

          <label class="flex items-center gap-2 text-sm text-gray-600 dark:text-gray-400">
            <input
              type="checkbox"
              v-model="includeLowercase"
              class="rounded border-gray-300 dark:border-dark-600 text-primary-600 focus:ring-primary-500"
            />
            Lowercase (a-z)
          </label>

          <label class="flex items-center gap-2 text-sm text-gray-600 dark:text-gray-400">
            <input
              type="checkbox"
              v-model="includeNumbers"
              class="rounded border-gray-300 dark:border-dark-600 text-primary-600 focus:ring-primary-500"
            />
            Numbers (0-9)
          </label>

          <label class="flex items-center gap-2 text-sm text-gray-600 dark:text-gray-400">
            <input
              type="checkbox"
              v-model="includeSymbols"
              class="rounded border-gray-300 dark:border-dark-600 text-primary-600 focus:ring-primary-500"
            />
            Symbols (!@#$...)
          </label>

          <label class="flex items-center gap-2 text-sm text-gray-600 dark:text-gray-400 col-span-2">
            <input
              type="checkbox"
              v-model="excludeAmbiguous"
              class="rounded border-gray-300 dark:border-dark-600 text-primary-600 focus:ring-primary-500"
            />
            Exclude ambiguous (0, O, l, 1, I)
          </label>
        </div>
      </div>

      <!-- History -->
      <div v-if="history.length" class="space-y-2">
        <div class="flex items-center justify-between">
          <label class="text-sm font-medium text-gray-700 dark:text-gray-300">Recent Passwords</label>
          <button
            @click="clearHistory"
            class="text-sm text-gray-500 dark:text-gray-400 hover:text-red-500"
          >
            Clear
          </button>
        </div>
        <div class="space-y-1">
          <div
            v-for="(pwd, index) in history"
            :key="index"
            class="flex items-center justify-between p-2 bg-gray-50 dark:bg-dark-800 rounded group"
          >
            <code class="font-mono text-sm text-gray-600 dark:text-gray-400 truncate">{{ pwd }}</code>
            <CopyButton :text="pwd" class="opacity-0 group-hover:opacity-100 transition-opacity" />
          </div>
        </div>
      </div>
    </div>
  </ToolLayout>
</template>
