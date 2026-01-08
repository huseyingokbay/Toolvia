<script setup lang="ts">
import GoogleAd from './GoogleAd.vue'

defineProps<{
  title: string
  description?: string
}>()
</script>

<template>
  <div class="p-4 sm:p-6 lg:p-8 max-w-5xl mx-auto">
    <!-- Header -->
    <div class="mb-6">
      <h1 class="text-2xl font-semibold text-gray-900 dark:text-white">{{ title }}</h1>
      <p v-if="description" class="mt-1 text-sm text-gray-500 dark:text-gray-400">{{ description }}</p>
    </div>

    <!-- Top Ad -->
    <GoogleAd ad-slot="1234567890" ad-format="horizontal" full-width="true" class="mb-6" />

    <!-- Content -->
    <div class="tool-card p-6">
      <slot />
    </div>

    <!-- Bottom Ad -->
    <GoogleAd ad-slot="0987654321" ad-format="horizontal" full-width="true" class="mt-6" />
  </div>
</template>
