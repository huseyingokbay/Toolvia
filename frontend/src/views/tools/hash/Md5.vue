<script setup lang="ts">
import HashTool from '../../../components/tools/HashTool.vue'
import { hashMD5 } from '../../../utils/hash'
</script>

<template>
  <HashTool
    title="MD5 Hash Generator"
    description="Generate MD5 hash from text. Note: MD5 is not recommended for security purposes."
    :hash-function="hashMD5"
    :hash-length="32"
  >
    <template #info>
      MD5 produces a 128-bit (16-byte) hash value, rendered as a 32-character hexadecimal string.
      <strong class="text-amber-600 dark:text-amber-400">Warning:</strong> MD5 is cryptographically broken and should not be used for security purposes.
    </template>
  </HashTool>
</template>
