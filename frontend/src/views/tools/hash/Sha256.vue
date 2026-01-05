<script setup lang="ts">
import HashTool from '../../../components/tools/HashTool.vue'
import { hashSHA256 } from '../../../utils/hash'
</script>

<template>
  <HashTool
    title="SHA-256 Hash Generator"
    description="Generate SHA-256 hash from text. A secure cryptographic hash function."
    :hash-function="hashSHA256"
    :hash-length="64"
  >
    <template #info>
      SHA-256 produces a 256-bit (32-byte) hash value, rendered as a 64-character hexadecimal string.
      It is part of the SHA-2 family and is widely used for security applications.
    </template>
  </HashTool>
</template>
