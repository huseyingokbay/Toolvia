<script setup lang="ts">
import { onMounted, onUnmounted, ref, nextTick } from 'vue'

defineProps<{
  adSlot: string
  adFormat?: 'auto' | 'horizontal' | 'vertical' | 'rectangle'
  fullWidth?: boolean
}>()

const adContainer = ref<HTMLElement | null>(null)
const adLoaded = ref(false)
let observer: IntersectionObserver | null = null

const canRender = () => {
  if (!adContainer.value) return false
  return adContainer.value.getBoundingClientRect().width >= 300
}

const loadAd = () => {
  if (adLoaded.value) return
  if (!canRender()) return

  if (typeof window !== 'undefined' && (window as any).adsbygoogle) {
    try {
      ;(window as any).adsbygoogle.push({})
      adLoaded.value = true
    } catch {
      // AdSense throws on invalid size – ignore and retry on next intersection
    }
  }
}

onMounted(async () => {
  await nextTick()

  observer = new IntersectionObserver(
    (entries) => {
      entries.forEach((entry) => {
        if (entry.isIntersecting && entry.intersectionRatio > 0) {
          requestAnimationFrame(() => {
            setTimeout(loadAd, 150)
          })
        }
      })
    },
    {
      rootMargin: '100px',
      threshold: 0.01
    }
  )

  if (adContainer.value) {
    observer.observe(adContainer.value)
  }
})

onUnmounted(() => {
  observer?.disconnect()
})
</script>

<template>
  <div ref="adContainer" class="ad-container my-4">
    <ins
      class="adsbygoogle"
      style="display:block;width:100%;min-width:320px;text-align:center"
      data-ad-client="ca-pub-7127218618005764"
      :data-ad-slot="adSlot"
      :data-ad-format="adFormat || 'auto'"
      data-full-width-responsive="true"
    ></ins>
  </div>
</template>

<style scoped>
.ad-container {
  width: 100%;
  min-width: 320px;
  min-height: 90px;
  display: block;
  overflow: hidden;
}

/* Parent max-w, grid, flex vs ne olursa olsun gerçek width propagate edilir */
.ad-container > .adsbygoogle {
  width: 100% !important;
  min-width: 320px;
}
</style>
