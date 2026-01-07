<script setup lang="ts">
import { onMounted, ref } from 'vue'

const props = defineProps<{
  adSlot: string
  adFormat?: 'auto' | 'horizontal' | 'vertical' | 'rectangle'
  fullWidth?: boolean
}>()

const adLoaded = ref(false)

onMounted(() => {
  try {
    // Push ad to adsbygoogle queue
    if (typeof window !== 'undefined' && (window as any).adsbygoogle) {
      (window as any).adsbygoogle.push({})
      adLoaded.value = true
    }
  } catch (e) {
    console.error('AdSense error:', e)
  }
})
</script>

<template>
  <div class="ad-container my-4">
    <ins
      class="adsbygoogle"
      :style="{ display: 'block', textAlign: 'center' }"
      :data-ad-client="'ca-pub-XXXXXXXXXXXXXXXX'"
      :data-ad-slot="adSlot"
      :data-ad-format="adFormat || 'auto'"
      :data-full-width-responsive="fullWidth !== false ? 'true' : 'false'"
    ></ins>
  </div>
</template>

<style scoped>
.ad-container {
  min-height: 90px;
  background-color: transparent;
  display: flex;
  align-items: center;
  justify-content: center;
}

/* Placeholder for development */
.ad-container:empty::before {
  content: '';
}
</style>
