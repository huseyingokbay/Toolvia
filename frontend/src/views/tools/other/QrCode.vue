<script setup lang="ts">
import { ref, watch, onMounted } from 'vue'
import QRCode from 'qrcode'
import ToolLayout from '../../../components/common/ToolLayout.vue'

const input = ref('https://example.com')
const size = ref(256)
const darkColor = ref('#000000')
const lightColor = ref('#ffffff')
const errorLevel = ref<'L' | 'M' | 'Q' | 'H'>('M')
const canvasRef = ref<HTMLCanvasElement | null>(null)
const qrDataUrl = ref('')

const errorLevels = [
  { value: 'L', label: 'Low (7%)', description: '7% error correction' },
  { value: 'M', label: 'Medium (15%)', description: '15% error correction' },
  { value: 'Q', label: 'Quartile (25%)', description: '25% error correction' },
  { value: 'H', label: 'High (30%)', description: '30% error correction' },
]

const generateQR = async () => {
  if (!input.value || !canvasRef.value) return

  try {
    await QRCode.toCanvas(canvasRef.value, input.value, {
      width: size.value,
      margin: 2,
      color: {
        dark: darkColor.value,
        light: lightColor.value,
      },
      errorCorrectionLevel: errorLevel.value,
    })
    qrDataUrl.value = canvasRef.value.toDataURL('image/png')
  } catch (err) {
    console.error('QR generation error:', err)
  }
}

const download = (format: 'png' | 'svg') => {
  if (format === 'png' && qrDataUrl.value) {
    const link = document.createElement('a')
    link.download = 'qrcode.png'
    link.href = qrDataUrl.value
    link.click()
  } else if (format === 'svg') {
    QRCode.toString(input.value, {
      type: 'svg',
      width: size.value,
      margin: 2,
      color: {
        dark: darkColor.value,
        light: lightColor.value,
      },
      errorCorrectionLevel: errorLevel.value,
    }).then(svg => {
      const blob = new Blob([svg], { type: 'image/svg+xml' })
      const url = URL.createObjectURL(blob)
      const link = document.createElement('a')
      link.download = 'qrcode.svg'
      link.href = url
      link.click()
      URL.revokeObjectURL(url)
    })
  }
}

watch([input, size, darkColor, lightColor, errorLevel], generateQR)

onMounted(() => {
  generateQR()
})
</script>

<template>
  <ToolLayout title="QR Code Generator" description="Generate QR codes from text or URLs">
    <div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
      <!-- Input Section -->
      <div class="space-y-6">
        <!-- Text Input -->
        <div class="space-y-2">
          <label class="text-sm font-medium text-gray-700 dark:text-gray-300">Content</label>
          <textarea
            v-model="input"
            placeholder="Enter text or URL..."
            class="tool-textarea h-32"
          />
          <p class="text-xs text-gray-500">{{ input.length }} characters</p>
        </div>

        <!-- Options -->
        <div class="space-y-4 p-4 bg-gray-50 dark:bg-dark-800 rounded-lg">
          <h3 class="text-sm font-medium text-gray-700 dark:text-gray-300">Options</h3>

          <div class="space-y-2">
            <label class="text-sm text-gray-600 dark:text-gray-400">Size: {{ size }}px</label>
            <input
              v-model.number="size"
              type="range"
              min="128"
              max="512"
              step="32"
              class="w-full h-2 bg-gray-200 dark:bg-dark-700 rounded-lg appearance-none cursor-pointer accent-primary-600"
            />
          </div>

          <div class="space-y-2">
            <label class="text-sm text-gray-600 dark:text-gray-400">Error Correction</label>
            <select v-model="errorLevel" class="tool-input">
              <option v-for="level in errorLevels" :key="level.value" :value="level.value">
                {{ level.label }}
              </option>
            </select>
          </div>

          <div class="grid grid-cols-2 gap-4">
            <div class="space-y-2">
              <label class="text-sm text-gray-600 dark:text-gray-400">Foreground</label>
              <div class="flex gap-2">
                <input
                  type="color"
                  v-model="darkColor"
                  class="w-10 h-10 rounded cursor-pointer"
                />
                <input
                  v-model="darkColor"
                  type="text"
                  class="tool-input flex-1 uppercase"
                />
              </div>
            </div>

            <div class="space-y-2">
              <label class="text-sm text-gray-600 dark:text-gray-400">Background</label>
              <div class="flex gap-2">
                <input
                  type="color"
                  v-model="lightColor"
                  class="w-10 h-10 rounded cursor-pointer"
                />
                <input
                  v-model="lightColor"
                  type="text"
                  class="tool-input flex-1 uppercase"
                />
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Preview Section -->
      <div class="space-y-4">
        <div class="flex items-center justify-between">
          <label class="text-sm font-medium text-gray-700 dark:text-gray-300">Preview</label>
          <div class="flex gap-2">
            <button @click="download('png')" class="tool-button-secondary text-sm">
              Download PNG
            </button>
            <button @click="download('svg')" class="tool-button-secondary text-sm">
              Download SVG
            </button>
          </div>
        </div>

        <div class="flex items-center justify-center p-8 bg-gray-100 dark:bg-dark-800 rounded-lg">
          <canvas
            ref="canvasRef"
            class="rounded shadow-lg"
            :style="{ maxWidth: '100%', height: 'auto' }"
          />
        </div>

        <!-- Quick Presets -->
        <div class="space-y-2">
          <label class="text-sm font-medium text-gray-700 dark:text-gray-300">Quick Presets</label>
          <div class="flex flex-wrap gap-2">
            <button
              v-for="preset in [
                { label: 'Default', dark: '#000000', light: '#ffffff' },
                { label: 'Blue', dark: '#1d4ed8', light: '#dbeafe' },
                { label: 'Green', dark: '#15803d', light: '#dcfce7' },
                { label: 'Purple', dark: '#7c3aed', light: '#ede9fe' },
                { label: 'Inverted', dark: '#ffffff', light: '#000000' },
              ]"
              :key="preset.label"
              @click="darkColor = preset.dark; lightColor = preset.light"
              class="px-3 py-1.5 text-sm rounded-lg bg-gray-100 dark:bg-dark-700 text-gray-600 dark:text-gray-400 hover:bg-gray-200 dark:hover:bg-dark-600"
            >
              {{ preset.label }}
            </button>
          </div>
        </div>
      </div>
    </div>
  </ToolLayout>
</template>
