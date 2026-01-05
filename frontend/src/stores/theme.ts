import { defineStore } from 'pinia'
import { ref } from 'vue'

export type Theme = 'light' | 'dark' | 'system'

export const useThemeStore = defineStore('theme', () => {
  const storedTheme = localStorage.getItem('theme')
  const initialTheme: Theme = (storedTheme === 'light' || storedTheme === 'dark' || storedTheme === 'system') ? storedTheme : 'system'
  const theme = ref<Theme>(initialTheme)
  const isDark = ref(false)

  const updateDarkMode = () => {
    if (theme.value === 'system') {
      isDark.value = window.matchMedia('(prefers-color-scheme: dark)').matches
    } else {
      isDark.value = theme.value === 'dark'
    }

    if (isDark.value) {
      document.documentElement.classList.add('dark')
    } else {
      document.documentElement.classList.remove('dark')
    }
  }

  const setTheme = (newTheme: Theme) => {
    theme.value = newTheme
    localStorage.setItem('theme', newTheme)
    updateDarkMode()
  }

  const toggleTheme = () => {
    const themes: Theme[] = ['light', 'dark', 'system']
    const currentIndex = themes.indexOf(theme.value)
    const nextIndex = (currentIndex + 1) % themes.length
    const nextTheme = themes[nextIndex]
    if (nextTheme) {
      setTheme(nextTheme)
    }
  }

  // Watch for system theme changes
  if (typeof window !== 'undefined') {
    const mediaQuery = window.matchMedia('(prefers-color-scheme: dark)')
    mediaQuery.addEventListener('change', () => {
      if (theme.value === 'system') {
        updateDarkMode()
      }
    })
  }

  // Initialize
  updateDarkMode()

  return {
    theme,
    isDark,
    setTheme,
    toggleTheme,
  }
})
