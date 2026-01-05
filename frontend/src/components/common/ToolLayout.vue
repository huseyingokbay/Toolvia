<script setup lang="ts">
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

    <!-- Content -->
    <div class="tool-card p-6">
      <slot />
    </div>
  </div>
</template>
