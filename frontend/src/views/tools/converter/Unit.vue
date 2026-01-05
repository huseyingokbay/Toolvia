<script setup lang="ts">
import { ref, watch, computed } from 'vue'
import ToolLayout from '../../../components/common/ToolLayout.vue'

interface UnitCategory {
  name: string
  units: { id: string; name: string; factor: number }[]
}

const categories: UnitCategory[] = [
  {
    name: 'Length',
    units: [
      { id: 'mm', name: 'Millimeter (mm)', factor: 0.001 },
      { id: 'cm', name: 'Centimeter (cm)', factor: 0.01 },
      { id: 'm', name: 'Meter (m)', factor: 1 },
      { id: 'km', name: 'Kilometer (km)', factor: 1000 },
      { id: 'in', name: 'Inch (in)', factor: 0.0254 },
      { id: 'ft', name: 'Foot (ft)', factor: 0.3048 },
      { id: 'yd', name: 'Yard (yd)', factor: 0.9144 },
      { id: 'mi', name: 'Mile (mi)', factor: 1609.344 },
    ]
  },
  {
    name: 'Weight',
    units: [
      { id: 'mg', name: 'Milligram (mg)', factor: 0.000001 },
      { id: 'g', name: 'Gram (g)', factor: 0.001 },
      { id: 'kg', name: 'Kilogram (kg)', factor: 1 },
      { id: 't', name: 'Metric Ton (t)', factor: 1000 },
      { id: 'oz', name: 'Ounce (oz)', factor: 0.0283495 },
      { id: 'lb', name: 'Pound (lb)', factor: 0.453592 },
    ]
  },
  {
    name: 'Temperature',
    units: [
      { id: 'c', name: 'Celsius (°C)', factor: 1 },
      { id: 'f', name: 'Fahrenheit (°F)', factor: 1 },
      { id: 'k', name: 'Kelvin (K)', factor: 1 },
    ]
  },
  {
    name: 'Area',
    units: [
      { id: 'mm2', name: 'Square Millimeter (mm²)', factor: 0.000001 },
      { id: 'cm2', name: 'Square Centimeter (cm²)', factor: 0.0001 },
      { id: 'm2', name: 'Square Meter (m²)', factor: 1 },
      { id: 'km2', name: 'Square Kilometer (km²)', factor: 1000000 },
      { id: 'ha', name: 'Hectare (ha)', factor: 10000 },
      { id: 'ac', name: 'Acre (ac)', factor: 4046.86 },
      { id: 'ft2', name: 'Square Foot (ft²)', factor: 0.092903 },
    ]
  },
  {
    name: 'Volume',
    units: [
      { id: 'ml', name: 'Milliliter (ml)', factor: 0.001 },
      { id: 'l', name: 'Liter (L)', factor: 1 },
      { id: 'm3', name: 'Cubic Meter (m³)', factor: 1000 },
      { id: 'gal', name: 'Gallon (US)', factor: 3.78541 },
      { id: 'qt', name: 'Quart (US)', factor: 0.946353 },
      { id: 'pt', name: 'Pint (US)', factor: 0.473176 },
      { id: 'cup', name: 'Cup (US)', factor: 0.236588 },
      { id: 'floz', name: 'Fluid Ounce (US)', factor: 0.0295735 },
    ]
  },
  {
    name: 'Data',
    units: [
      { id: 'b', name: 'Bit (b)', factor: 0.125 },
      { id: 'B', name: 'Byte (B)', factor: 1 },
      { id: 'KB', name: 'Kilobyte (KB)', factor: 1024 },
      { id: 'MB', name: 'Megabyte (MB)', factor: 1048576 },
      { id: 'GB', name: 'Gigabyte (GB)', factor: 1073741824 },
      { id: 'TB', name: 'Terabyte (TB)', factor: 1099511627776 },
    ]
  },
  {
    name: 'Time',
    units: [
      { id: 'ms', name: 'Millisecond (ms)', factor: 0.001 },
      { id: 's', name: 'Second (s)', factor: 1 },
      { id: 'min', name: 'Minute (min)', factor: 60 },
      { id: 'h', name: 'Hour (h)', factor: 3600 },
      { id: 'd', name: 'Day (d)', factor: 86400 },
      { id: 'wk', name: 'Week (wk)', factor: 604800 },
      { id: 'mo', name: 'Month (avg)', factor: 2629746 },
      { id: 'yr', name: 'Year (yr)', factor: 31556952 },
    ]
  },
]

const selectedCategory = ref(categories[0])
const inputValue = ref<number | null>(null)
const fromUnit = ref(categories[0].units[0].id)
const toUnit = ref(categories[0].units[2].id)

const convertTemperature = (value: number, from: string, to: string): number => {
  let celsius: number
  switch (from) {
    case 'f': celsius = (value - 32) * 5/9; break
    case 'k': celsius = value - 273.15; break
    default: celsius = value
  }
  switch (to) {
    case 'f': return celsius * 9/5 + 32
    case 'k': return celsius + 273.15
    default: return celsius
  }
}

const result = computed(() => {
  if (inputValue.value === null || isNaN(inputValue.value)) return null

  if (selectedCategory.value.name === 'Temperature') {
    return convertTemperature(inputValue.value, fromUnit.value, toUnit.value)
  }

  const fromFactor = selectedCategory.value.units.find(u => u.id === fromUnit.value)?.factor || 1
  const toFactor = selectedCategory.value.units.find(u => u.id === toUnit.value)?.factor || 1
  return (inputValue.value * fromFactor) / toFactor
})

const formattedResult = computed(() => {
  if (result.value === null) return ''
  if (Math.abs(result.value) < 0.0001 || Math.abs(result.value) >= 1e10) {
    return result.value.toExponential(6)
  }
  return result.value.toLocaleString(undefined, { maximumFractionDigits: 10 })
})

watch(selectedCategory, () => {
  fromUnit.value = selectedCategory.value.units[0].id
  toUnit.value = selectedCategory.value.units.length > 2
    ? selectedCategory.value.units[2].id
    : selectedCategory.value.units[1].id
})

const swap = () => {
  const temp = fromUnit.value
  fromUnit.value = toUnit.value
  toUnit.value = temp
}
</script>

<template>
  <ToolLayout title="Unit Converter" description="Convert between different units of measurement">
    <div class="space-y-6">
      <!-- Category Selection -->
      <div class="flex flex-wrap gap-2">
        <button
          v-for="cat in categories"
          :key="cat.name"
          @click="selectedCategory = cat"
          :class="[
            'px-3 py-1.5 rounded-lg text-sm font-medium transition-colors',
            selectedCategory.name === cat.name
              ? 'bg-primary-600 text-white'
              : 'bg-gray-100 dark:bg-dark-700 text-gray-700 dark:text-gray-300 hover:bg-gray-200 dark:hover:bg-dark-600'
          ]"
        >
          {{ cat.name }}
        </button>
      </div>

      <!-- Converter -->
      <div class="grid grid-cols-1 md:grid-cols-[1fr,auto,1fr] gap-4 items-end">
        <!-- From -->
        <div class="space-y-2">
          <label class="text-sm font-medium text-gray-700 dark:text-gray-300">From</label>
          <select v-model="fromUnit" class="tool-input">
            <option v-for="unit in selectedCategory.units" :key="unit.id" :value="unit.id">
              {{ unit.name }}
            </option>
          </select>
          <input
            v-model.number="inputValue"
            type="number"
            placeholder="Enter value..."
            class="tool-input"
          />
        </div>

        <!-- Swap Button -->
        <button
          @click="swap"
          class="p-2 rounded-lg bg-gray-100 dark:bg-dark-700 text-gray-600 dark:text-gray-400 hover:bg-gray-200 dark:hover:bg-dark-600 self-center mb-2"
        >
          <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M7.5 21L3 16.5m0 0L7.5 12M3 16.5h13.5m0-13.5L21 7.5m0 0L16.5 12M21 7.5H7.5" />
          </svg>
        </button>

        <!-- To -->
        <div class="space-y-2">
          <label class="text-sm font-medium text-gray-700 dark:text-gray-300">To</label>
          <select v-model="toUnit" class="tool-input">
            <option v-for="unit in selectedCategory.units" :key="unit.id" :value="unit.id">
              {{ unit.name }}
            </option>
          </select>
          <input
            :value="formattedResult"
            readonly
            placeholder="Result..."
            class="tool-input bg-gray-50 dark:bg-dark-800"
          />
        </div>
      </div>

      <!-- Formula -->
      <div v-if="inputValue !== null && result !== null" class="p-4 bg-gray-50 dark:bg-dark-800 rounded-lg">
        <p class="text-sm text-gray-600 dark:text-gray-400 font-mono">
          {{ inputValue }} {{ fromUnit }} = {{ formattedResult }} {{ toUnit }}
        </p>
      </div>
    </div>
  </ToolLayout>
</template>
