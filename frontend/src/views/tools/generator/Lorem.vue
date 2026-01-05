<script setup lang="ts">
import { ref, computed } from 'vue'
import ToolLayout from '../../../components/common/ToolLayout.vue'
import CopyButton from '../../../components/common/CopyButton.vue'

const type = ref<'paragraphs' | 'sentences' | 'words'>('paragraphs')
const count = ref(3)
const startWithLorem = ref(true)

const words = [
  'lorem', 'ipsum', 'dolor', 'sit', 'amet', 'consectetur', 'adipiscing', 'elit',
  'sed', 'do', 'eiusmod', 'tempor', 'incididunt', 'ut', 'labore', 'et', 'dolore',
  'magna', 'aliqua', 'enim', 'ad', 'minim', 'veniam', 'quis', 'nostrud',
  'exercitation', 'ullamco', 'laboris', 'nisi', 'aliquip', 'ex', 'ea', 'commodo',
  'consequat', 'duis', 'aute', 'irure', 'in', 'reprehenderit', 'voluptate',
  'velit', 'esse', 'cillum', 'fugiat', 'nulla', 'pariatur', 'excepteur', 'sint',
  'occaecat', 'cupidatat', 'non', 'proident', 'sunt', 'culpa', 'qui', 'officia',
  'deserunt', 'mollit', 'anim', 'id', 'est', 'laborum', 'at', 'vero', 'eos',
  'accusamus', 'iusto', 'odio', 'dignissimos', 'ducimus', 'blanditiis',
  'praesentium', 'voluptatum', 'deleniti', 'atque', 'corrupti', 'quos', 'dolores',
  'quas', 'molestias', 'excepturi', 'obcaecati', 'cupiditate', 'provident'
]

const getRandomWord = (): string => {
  return words[Math.floor(Math.random() * words.length)]
}

const capitalize = (str: string): string => {
  return str.charAt(0).toUpperCase() + str.slice(1)
}

const generateSentence = (wordCount?: number): string => {
  const len = wordCount || Math.floor(Math.random() * 10) + 5
  const sentence = Array.from({ length: len }, () => getRandomWord()).join(' ')
  return capitalize(sentence) + '.'
}

const generateParagraph = (): string => {
  const sentenceCount = Math.floor(Math.random() * 4) + 3
  return Array.from({ length: sentenceCount }, () => generateSentence()).join(' ')
}

const output = computed(() => {
  let result = ''

  switch (type.value) {
    case 'words':
      result = Array.from({ length: count.value }, () => getRandomWord()).join(' ')
      break
    case 'sentences':
      result = Array.from({ length: count.value }, () => generateSentence()).join(' ')
      break
    case 'paragraphs':
      result = Array.from({ length: count.value }, () => generateParagraph()).join('\n\n')
      break
  }

  if (startWithLorem.value && result) {
    const loremStart = 'Lorem ipsum dolor sit amet, consectetur adipiscing elit.'
    if (type.value === 'words') {
      result = 'Lorem ipsum ' + result.split(' ').slice(2).join(' ')
    } else if (type.value === 'sentences') {
      result = loremStart + ' ' + result.split('. ').slice(1).join('. ')
    } else {
      const paragraphs = result.split('\n\n')
      paragraphs[0] = loremStart + ' ' + paragraphs[0].split('. ').slice(1).join('. ')
      result = paragraphs.join('\n\n')
    }
  }

  return result
})

const stats = computed(() => {
  const text = output.value
  const wordCount = text.split(/\s+/).filter(Boolean).length
  const charCount = text.length
  const paragraphCount = text.split('\n\n').filter(Boolean).length
  return { wordCount, charCount, paragraphCount }
})
</script>

<template>
  <ToolLayout title="Lorem Ipsum Generator" description="Generate placeholder text for design and development">
    <div class="space-y-6">
      <!-- Options -->
      <div class="flex flex-wrap items-center gap-4">
        <div class="flex items-center gap-2">
          <label class="text-sm text-gray-600 dark:text-gray-400">Generate:</label>
          <select v-model.number="count" class="tool-input w-auto py-1.5">
            <option v-for="n in [1, 2, 3, 4, 5, 10, 20]" :key="n" :value="n">{{ n }}</option>
          </select>
          <select v-model="type" class="tool-input w-auto py-1.5">
            <option value="paragraphs">Paragraphs</option>
            <option value="sentences">Sentences</option>
            <option value="words">Words</option>
          </select>
        </div>

        <label class="flex items-center gap-2 text-sm text-gray-600 dark:text-gray-400">
          <input
            type="checkbox"
            v-model="startWithLorem"
            class="rounded border-gray-300 dark:border-dark-600 text-primary-600 focus:ring-primary-500"
          />
          Start with "Lorem ipsum..."
        </label>
      </div>

      <!-- Output -->
      <div class="space-y-2">
        <div class="flex items-center justify-between">
          <label class="text-sm font-medium text-gray-700 dark:text-gray-300">Generated Text</label>
          <div class="flex items-center gap-3">
            <span class="text-xs text-gray-500">
              {{ stats.wordCount }} words | {{ stats.charCount }} chars
            </span>
            <CopyButton :text="output" />
          </div>
        </div>
        <textarea
          :value="output"
          readonly
          class="tool-textarea h-80"
        />
      </div>

      <!-- Quick Copy Buttons -->
      <div class="flex flex-wrap gap-2">
        <button
          v-for="preset in [
            { label: '1 Paragraph', type: 'paragraphs', count: 1 },
            { label: '3 Paragraphs', type: 'paragraphs', count: 3 },
            { label: '5 Sentences', type: 'sentences', count: 5 },
            { label: '50 Words', type: 'words', count: 50 },
          ]"
          :key="preset.label"
          @click="type = preset.type as any; count = preset.count"
          class="px-3 py-1.5 text-sm rounded-lg bg-gray-100 dark:bg-dark-700 text-gray-600 dark:text-gray-400 hover:bg-gray-200 dark:hover:bg-dark-600"
        >
          {{ preset.label }}
        </button>
      </div>
    </div>
  </ToolLayout>
</template>
