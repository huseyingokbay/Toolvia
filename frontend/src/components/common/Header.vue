<script setup lang="ts">
import { useSidebarStore } from '../../stores/sidebar'
import ThemeToggle from './ThemeToggle.vue'

const sidebarStore = useSidebarStore()
</script>

<template>
  <header class="sticky top-0 z-30 h-16 bg-white/80 dark:bg-dark-900/80 backdrop-blur-sm border-b border-gray-200 dark:border-dark-800">
    <div class="h-full px-4 flex items-center justify-between">
      <!-- Left -->
      <div class="flex items-center gap-4">
        <!-- Mobile Menu Button -->
        <button
          @click="sidebarStore.toggleMobile()"
          class="lg:hidden p-2 rounded-lg text-gray-500 dark:text-gray-400 hover:bg-gray-100 dark:hover:bg-dark-800"
        >
          <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 6h16M4 12h16M4 18h16" />
          </svg>
        </button>

        <!-- Desktop Sidebar Toggle -->
        <button
          @click="sidebarStore.toggle()"
          class="hidden lg:block p-2 rounded-lg text-gray-500 dark:text-gray-400 hover:bg-gray-100 dark:hover:bg-dark-800"
        >
          <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 6h16M4 12h16M4 18h16" />
          </svg>
        </button>

        <!-- Search -->
        <div class="hidden sm:block relative">
          <input
            type="text"
            placeholder="Search tools..."
            class="w-64 pl-10 pr-4 py-2 text-sm rounded-lg border border-gray-200 dark:border-dark-700 bg-gray-50 dark:bg-dark-800 text-gray-900 dark:text-gray-100 placeholder-gray-400 dark:placeholder-gray-500 focus:ring-2 focus:ring-primary-500 focus:border-transparent outline-none"
          />
          <svg class="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z" />
          </svg>
        </div>
      </div>

      <!-- Right -->
      <div class="flex items-center gap-2">
        <ThemeToggle />
      </div>
    </div>
  </header>
</template>
