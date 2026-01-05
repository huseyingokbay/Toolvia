<script setup lang="ts">
import HashTool from '../../../components/tools/HashTool.vue'
import { hashSHA1 } from '../../../utils/hash'
</script>

<template>
  <HashTool
    title="SHA-1 Hash Generator"
    description="Generate SHA-1 hash from text. Note: SHA-1 is deprecated for security purposes."
    :hash-function="hashSHA1"
    :hash-length="40"
  >
    <template #info>
      SHA-1 produces a 160-bit (20-byte) hash value, rendered as a 40-character hexadecimal string.
      <strong class="text-amber-600 dark:text-amber-400">Warning:</strong> SHA-1 is considered weak and should not be used for security-sensitive applications.
    </template>
  </HashTool>
</template>
