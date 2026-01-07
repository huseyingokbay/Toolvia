<script setup lang="ts">
import { onMounted, onUnmounted, ref } from 'vue'

defineProps<{
  adSlot: string
  adFormat?: 'auto' | 'horizontal' | 'vertical' | 'rectangle'
  fullWidth?: boolean
}>()

const adContainer = ref<HTMLElement | null>(null)
const adLoaded = ref(false)
let observer: IntersectionObserver | null = null

const loadAd = () => {
  if (adLoaded.value) return

  try {
    // Check if container has width
    if (adContainer.value && adContainer.value.offsetWidth > 0) {
      if (typeof window !== 'undefined' && (window as any).adsbygoogle) {
        (window as any).adsbygoogle.push({})
        adLoaded.value = true
      }
    }
  } catch (e) {
    // Silently handle AdSense errors in development
    if (import.meta.env.PROD) {
      console.error('AdSense error:', e)
    }
  }
}

onMounted(() => {
  // Use IntersectionObserver to load ad when visible
  observer = new IntersectionObserver(
    (entries) => {
      entries.forEach((entry) => {
        if (entry.isIntersecting && entry.intersectionRatio > 0) {
          // Small delay to ensure layout is complete
          setTimeout(loadAd, 100)
        }
      })
    },
    { threshold: 0.1 }
  )

  if (adContainer.value) {
    observer.observe(adContainer.value)
  }
})

onUnmounted(() => {
  if (observer) {
    observer.disconnect()
  }
})
</script>

<template>
  <div ref="adContainer" class="ad-container my-4">
    <ins
      class="adsbygoogle"
      :style="{ display: 'block', textAlign: 'center' }"
      :data-ad-client="'${{ secrets.ADSENSE_CLIENT_ID }}'"
      :data-ad-slot="adSlot"
      :data-ad-format="adFormat || 'auto'"
      :data-full-width-responsive="fullWidth !== false ? 'true' : 'false'"
    ></ins>
  </div>
</template>

<style scoped>
.ad-container {
  min-height: 90px;
  min-width: 300px;
  background-color: transparent;
  display: flex;
  align-items: center;
  justify-content: center;
  overflow: hidden;
}

/* Hide in development when no ads loaded */
.ad-container:has(.adsbygoogle[data-ad-status="unfilled"]) {
  min-height: 0;
}
</style>
