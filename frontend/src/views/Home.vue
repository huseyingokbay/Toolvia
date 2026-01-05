<script setup lang="ts">
import { TOOL_CATEGORIES, getToolsByCategory } from '../types/tool'

const categorizedTools = TOOL_CATEGORIES.map(category => ({
  ...category,
  tools: getToolsByCategory(category.id)
})).filter(cat => cat.tools.length > 0)
</script>

<template>
  <div class="p-4 sm:p-6 lg:p-8">
    <!-- Hero -->
    <div class="text-center mb-12">
      <h1 class="text-3xl sm:text-4xl font-bold text-gray-900 dark:text-white mb-4">
        MultiTools
      </h1>
      <p class="text-lg text-gray-600 dark:text-gray-400 max-w-2xl mx-auto">
        A collection of useful online tools for developers and designers.
        Encode, decode, format, generate, and convert data with ease.
      </p>
    </div>

    <!-- Categories -->
    <div class="space-y-12">
      <section v-for="category in categorizedTools" :key="category.id">
        <h2 class="text-lg font-semibold text-gray-900 dark:text-white mb-4 flex items-center gap-2">
          <span class="w-8 h-8 rounded-lg bg-primary-100 dark:bg-primary-900/30 flex items-center justify-center">
            <svg v-if="category.icon === 'CodeBracketIcon'" class="w-4 h-4 text-primary-600 dark:text-primary-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M10 20l4-16m4 4l4 4-4 4M6 16l-4-4 4-4" />
            </svg>
            <svg v-else-if="category.icon === 'FingerPrintIcon'" class="w-4 h-4 text-primary-600 dark:text-primary-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 11c0 3.517-1.009 6.799-2.753 9.571m-3.44-2.04l.054-.09A13.916 13.916 0 008 11a4 4 0 118 0c0 1.017-.07 2.019-.203 3m-2.118 6.844A21.88 21.88 0 0015.171 17m3.839 1.132c.645-2.266.99-4.659.99-7.132A8 8 0 008 4.07M3 15.364c.64-1.319 1-2.8 1-4.364 0-1.457.39-2.823 1.07-4" />
            </svg>
            <svg v-else-if="category.icon === 'ArrowsRightLeftIcon'" class="w-4 h-4 text-primary-600 dark:text-primary-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M7.5 21L3 16.5m0 0L7.5 12M3 16.5h13.5m0-13.5L21 7.5m0 0L16.5 12M21 7.5H7.5" />
            </svg>
            <svg v-else-if="category.icon === 'DocumentTextIcon'" class="w-4 h-4 text-primary-600 dark:text-primary-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19.5 14.25v-2.625a3.375 3.375 0 00-3.375-3.375h-1.5A1.125 1.125 0 0113.5 7.125v-1.5a3.375 3.375 0 00-3.375-3.375H8.25m0 12.75h7.5m-7.5 3H12M10.5 2.25H5.625c-.621 0-1.125.504-1.125 1.125v17.25c0 .621.504 1.125 1.125 1.125h12.75c.621 0 1.125-.504 1.125-1.125V11.25a9 9 0 00-9-9z" />
            </svg>
            <svg v-else-if="category.icon === 'SparklesIcon'" class="w-4 h-4 text-primary-600 dark:text-primary-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9.813 15.904L9 18.75l-.813-2.846a4.5 4.5 0 00-3.09-3.09L2.25 12l2.846-.813a4.5 4.5 0 003.09-3.09L9 5.25l.813 2.846a4.5 4.5 0 003.09 3.09L15.75 12l-2.846.813a4.5 4.5 0 00-3.09 3.09zM18.259 8.715L18 9.75l-.259-1.035a3.375 3.375 0 00-2.455-2.456L14.25 6l1.036-.259a3.375 3.375 0 002.455-2.456L18 2.25l.259 1.035a3.375 3.375 0 002.456 2.456L21.75 6l-1.035.259a3.375 3.375 0 00-2.456 2.456zM16.894 20.567L16.5 21.75l-.394-1.183a2.25 2.25 0 00-1.423-1.423L13.5 18.75l1.183-.394a2.25 2.25 0 001.423-1.423l.394-1.183.394 1.183a2.25 2.25 0 001.423 1.423l1.183.394-1.183.394a2.25 2.25 0 00-1.423 1.423z" />
            </svg>
            <svg v-else class="w-4 h-4 text-primary-600 dark:text-primary-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M11.42 15.17L17.25 21A2.652 2.652 0 0021 17.25l-5.877-5.877M11.42 15.17l2.496-3.03c.317-.384.74-.626 1.208-.766M11.42 15.17l-4.655 5.653a2.548 2.548 0 11-3.586-3.586l6.837-5.63m5.108-.233c.55-.164 1.163-.188 1.743-.14a4.5 4.5 0 004.486-6.336l-3.276 3.277a3.004 3.004 0 01-2.25-2.25l3.276-3.276a4.5 4.5 0 00-6.336 4.486c.091 1.076-.071 2.264-.904 2.95l-.102.085m-1.745 1.437L5.909 7.5H4.5L2.25 3.75l1.5-1.5L7.5 4.5v1.409l4.26 4.26m-1.745 1.437l1.745-1.437m6.615 8.206L15.75 15.75M4.867 19.125h.008v.008h-.008v-.008z" />
            </svg>
          </span>
          {{ category.name }}
        </h2>

        <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-4">
          <router-link
            v-for="tool in category.tools"
            :key="tool.id"
            :to="tool.path"
            class="tool-card p-4 group hover:border-primary-300 dark:hover:border-primary-700"
          >
            <div class="flex items-start gap-3">
              <div class="w-10 h-10 rounded-lg bg-gray-100 dark:bg-dark-700 flex items-center justify-center flex-shrink-0 group-hover:bg-primary-100 dark:group-hover:bg-primary-900/30 transition-colors">
                <svg class="w-5 h-5 text-gray-500 dark:text-gray-400 group-hover:text-primary-600 dark:group-hover:text-primary-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M10 20l4-16m4 4l4 4-4 4M6 16l-4-4 4-4" />
                </svg>
              </div>
              <div class="min-w-0">
                <h3 class="font-medium text-gray-900 dark:text-white group-hover:text-primary-600 dark:group-hover:text-primary-400 transition-colors">
                  {{ tool.name }}
                </h3>
                <p class="text-sm text-gray-500 dark:text-gray-400 mt-0.5">
                  {{ tool.description }}
                </p>
              </div>
            </div>
          </router-link>
        </div>
      </section>
    </div>
  </div>
</template>
