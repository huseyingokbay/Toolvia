<script setup lang="ts">
import { onMounted } from 'vue'
import { useThemeStore } from './stores/theme'
import Sidebar from './components/common/Sidebar.vue'
import Header from './components/common/Header.vue'

const themeStore = useThemeStore()

onMounted(() => {
  // Initialize theme on mount
  themeStore.setTheme(themeStore.theme)
})
</script>

<template>
  <div class="min-h-screen bg-gray-50 dark:bg-dark-950 transition-colors duration-200">
    <div class="flex">
      <Sidebar />
      <div class="flex-1 min-h-screen flex flex-col">
        <Header />
        <main class="flex-1">
          <router-view />
        </main>
      </div>
    </div>
  </div>
</template>
