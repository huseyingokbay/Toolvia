import { defineStore } from 'pinia'
import { ref } from 'vue'

export const useSidebarStore = defineStore('sidebar', () => {
  const isOpen = ref(true)
  const isMobileOpen = ref(false)

  const toggle = () => {
    isOpen.value = !isOpen.value
  }

  const toggleMobile = () => {
    isMobileOpen.value = !isMobileOpen.value
  }

  const closeMobile = () => {
    isMobileOpen.value = false
  }

  return {
    isOpen,
    isMobileOpen,
    toggle,
    toggleMobile,
    closeMobile,
  }
})
