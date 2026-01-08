<script setup lang="ts">
import { onMounted } from 'vue'
import { useThemeStore } from './stores/theme'
import { useSidebarStore } from './stores/sidebar'
import Sidebar from './components/common/Sidebar.vue'
import Header from './components/common/Header.vue'

const themeStore = useThemeStore()
const sidebarStore = useSidebarStore()

onMounted(() => {
  // Initialize theme on mount
  themeStore.setTheme(themeStore.theme)
})
</script>

<template>
  <div class="min-h-screen bg-gray-50 dark:bg-dark-950 transition-colors duration-200">
    <Sidebar />
    <div
      :class="[
        'min-h-screen flex flex-col transition-all duration-200',
        sidebarStore.isOpen ? 'lg:ml-64' : 'lg:ml-0'
      ]"
    >
      <Header />
      <main class="flex-1">
        <router-view />
      </main>
    </div>
  </div>
</template>
