<script setup lang="ts">
import { ref, computed, watch } from 'vue'
import ToolLayout from '../../../components/common/ToolLayout.vue'
import CopyButton from '../../../components/common/CopyButton.vue'

const hexInput = ref('#3B82F6')
const error = ref('')

interface RGB { r: number; g: number; b: number }
interface HSL { h: number; s: number; l: number }

const hexToRgb = (hex: string): RGB | null => {
  const result = /^#?([a-f\d]{2})([a-f\d]{2})([a-f\d]{2})$/i.exec(hex)
  if (!result) return null
  return {
    r: parseInt(result[1], 16),
    g: parseInt(result[2], 16),
    b: parseInt(result[3], 16)
  }
}

const rgbToHex = (r: number, g: number, b: number): string => {
  return '#' + [r, g, b].map(x => {
    const hex = Math.max(0, Math.min(255, Math.round(x))).toString(16)
    return hex.length === 1 ? '0' + hex : hex
  }).join('').toUpperCase()
}

const rgbToHsl = (r: number, g: number, b: number): HSL => {
  r /= 255; g /= 255; b /= 255
  const max = Math.max(r, g, b), min = Math.min(r, g, b)
  let h = 0, s = 0
  const l = (max + min) / 2

  if (max !== min) {
    const d = max - min
    s = l > 0.5 ? d / (2 - max - min) : d / (max + min)
    switch (max) {
      case r: h = ((g - b) / d + (g < b ? 6 : 0)) / 6; break
      case g: h = ((b - r) / d + 2) / 6; break
      case b: h = ((r - g) / d + 4) / 6; break
    }
  }

  return {
    h: Math.round(h * 360),
    s: Math.round(s * 100),
    l: Math.round(l * 100)
  }
}

const hslToRgb = (h: number, s: number, l: number): RGB => {
  s /= 100; l /= 100
  const c = (1 - Math.abs(2 * l - 1)) * s
  const x = c * (1 - Math.abs((h / 60) % 2 - 1))
  const m = l - c / 2
  let r = 0, g = 0, b = 0

  if (h >= 0 && h < 60) { r = c; g = x; b = 0 }
  else if (h >= 60 && h < 120) { r = x; g = c; b = 0 }
  else if (h >= 120 && h < 180) { r = 0; g = c; b = x }
  else if (h >= 180 && h < 240) { r = 0; g = x; b = c }
  else if (h >= 240 && h < 300) { r = x; g = 0; b = c }
  else { r = c; g = 0; b = x }

  return {
    r: Math.round((r + m) * 255),
    g: Math.round((g + m) * 255),
    b: Math.round((b + m) * 255)
  }
}

const rgb = computed(() => {
  const match = hexInput.value.match(/^#?([a-f\d]{2})([a-f\d]{2})([a-f\d]{2})$/i)
  if (!match) {
    error.value = 'Invalid hex color'
    return null
  }
  error.value = ''
  return hexToRgb(hexInput.value)
})

const hsl = computed(() => {
  if (!rgb.value) return null
  return rgbToHsl(rgb.value.r, rgb.value.g, rgb.value.b)
})

const formats = computed(() => {
  if (!rgb.value || !hsl.value) return []
  const { r, g, b } = rgb.value
  const { h, s, l } = hsl.value

  return [
    { name: 'HEX', value: rgbToHex(r, g, b) },
    { name: 'RGB', value: `rgb(${r}, ${g}, ${b})` },
    { name: 'RGBA', value: `rgba(${r}, ${g}, ${b}, 1)` },
    { name: 'HSL', value: `hsl(${h}, ${s}%, ${l}%)` },
    { name: 'HSLA', value: `hsla(${h}, ${s}%, ${l}%, 1)` },
    { name: 'CSS', value: `--color: ${rgbToHex(r, g, b)};` },
  ]
})

const updateFromRgb = () => {
  if (!rgb.value) return
  hexInput.value = rgbToHex(rgb.value.r, rgb.value.g, rgb.value.b)
}

const setRgb = (component: 'r' | 'g' | 'b', value: number) => {
  if (!rgb.value) return
  const newRgb = { ...rgb.value, [component]: value }
  hexInput.value = rgbToHex(newRgb.r, newRgb.g, newRgb.b)
}

const setHsl = (component: 'h' | 's' | 'l', value: number) => {
  if (!hsl.value) return
  const newHsl = { ...hsl.value, [component]: value }
  const newRgb = hslToRgb(newHsl.h, newHsl.s, newHsl.l)
  hexInput.value = rgbToHex(newRgb.r, newRgb.g, newRgb.b)
}
</script>

<template>
  <ToolLayout title="Color Converter" description="Convert colors between HEX, RGB, and HSL formats">
    <div class="space-y-6">
      <!-- Color Preview -->
      <div class="flex items-center gap-4">
        <div
          class="w-24 h-24 rounded-xl border-2 border-gray-200 dark:border-dark-600 shadow-inner"
          :style="{ backgroundColor: hexInput }"
        />
        <div class="flex-1">
          <label class="text-sm font-medium text-gray-700 dark:text-gray-300 block mb-2">HEX Color</label>
          <div class="flex gap-2">
            <input
              type="color"
              :value="hexInput"
              @input="hexInput = ($event.target as HTMLInputElement).value"
              class="w-12 h-10 rounded cursor-pointer"
            />
            <input
              v-model="hexInput"
              type="text"
              placeholder="#000000"
              class="tool-input flex-1 uppercase"
              :class="{ 'border-red-500': error }"
            />
          </div>
          <p v-if="error" class="text-sm text-red-500 mt-1">{{ error }}</p>
        </div>
      </div>

      <!-- RGB Sliders -->
      <div v-if="rgb" class="space-y-4 p-4 bg-gray-50 dark:bg-dark-800 rounded-lg">
        <h3 class="text-sm font-medium text-gray-700 dark:text-gray-300">RGB Values</h3>
        <div class="space-y-3">
          <div v-for="(value, key) in { r: rgb.r, g: rgb.g, b: rgb.b }" :key="key" class="flex items-center gap-3">
            <span class="w-6 text-sm font-medium text-gray-600 dark:text-gray-400 uppercase">{{ key }}</span>
            <input
              type="range"
              :value="value"
              @input="setRgb(key as 'r' | 'g' | 'b', parseInt(($event.target as HTMLInputElement).value))"
              min="0"
              max="255"
              :class="[
                'flex-1 h-2 rounded-lg appearance-none cursor-pointer',
                key === 'r' ? 'accent-red-500' : key === 'g' ? 'accent-green-500' : 'accent-blue-500'
              ]"
            />
            <input
              type="number"
              :value="value"
              @input="setRgb(key as 'r' | 'g' | 'b', parseInt(($event.target as HTMLInputElement).value) || 0)"
              min="0"
              max="255"
              class="w-16 tool-input py-1 text-center"
            />
          </div>
        </div>
      </div>

      <!-- HSL Sliders -->
      <div v-if="hsl" class="space-y-4 p-4 bg-gray-50 dark:bg-dark-800 rounded-lg">
        <h3 class="text-sm font-medium text-gray-700 dark:text-gray-300">HSL Values</h3>
        <div class="space-y-3">
          <div class="flex items-center gap-3">
            <span class="w-6 text-sm font-medium text-gray-600 dark:text-gray-400">H</span>
            <input
              type="range"
              :value="hsl.h"
              @input="setHsl('h', parseInt(($event.target as HTMLInputElement).value))"
              min="0"
              max="360"
              class="flex-1 h-2 rounded-lg appearance-none cursor-pointer"
              style="background: linear-gradient(to right, #ff0000, #ffff00, #00ff00, #00ffff, #0000ff, #ff00ff, #ff0000)"
            />
            <input
              type="number"
              :value="hsl.h"
              @input="setHsl('h', parseInt(($event.target as HTMLInputElement).value) || 0)"
              min="0"
              max="360"
              class="w-16 tool-input py-1 text-center"
            />
          </div>
          <div class="flex items-center gap-3">
            <span class="w-6 text-sm font-medium text-gray-600 dark:text-gray-400">S</span>
            <input
              type="range"
              :value="hsl.s"
              @input="setHsl('s', parseInt(($event.target as HTMLInputElement).value))"
              min="0"
              max="100"
              class="flex-1 h-2 rounded-lg appearance-none cursor-pointer accent-primary-500"
            />
            <input
              type="number"
              :value="hsl.s"
              @input="setHsl('s', parseInt(($event.target as HTMLInputElement).value) || 0)"
              min="0"
              max="100"
              class="w-16 tool-input py-1 text-center"
            />
          </div>
          <div class="flex items-center gap-3">
            <span class="w-6 text-sm font-medium text-gray-600 dark:text-gray-400">L</span>
            <input
              type="range"
              :value="hsl.l"
              @input="setHsl('l', parseInt(($event.target as HTMLInputElement).value))"
              min="0"
              max="100"
              class="flex-1 h-2 rounded-lg appearance-none cursor-pointer accent-primary-500"
            />
            <input
              type="number"
              :value="hsl.l"
              @input="setHsl('l', parseInt(($event.target as HTMLInputElement).value) || 0)"
              min="0"
              max="100"
              class="w-16 tool-input py-1 text-center"
            />
          </div>
        </div>
      </div>

      <!-- Format Outputs -->
      <div v-if="formats.length" class="space-y-3">
        <h3 class="text-sm font-medium text-gray-700 dark:text-gray-300">Color Formats</h3>
        <div class="grid gap-2">
          <div
            v-for="format in formats"
            :key="format.name"
            class="flex items-center justify-between p-3 bg-gray-50 dark:bg-dark-800 rounded-lg"
          >
            <div class="min-w-0 flex-1">
              <span class="text-xs text-gray-500 dark:text-gray-400">{{ format.name }}</span>
              <div class="font-mono text-sm text-gray-900 dark:text-white">{{ format.value }}</div>
            </div>
            <CopyButton :text="format.value" class="ml-3" />
          </div>
        </div>
      </div>
    </div>
  </ToolLayout>
</template>
