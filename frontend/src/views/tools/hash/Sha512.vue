<script setup lang="ts">
import HashTool from '../../../components/tools/HashTool.vue'
import { hashSHA512 } from '../../../utils/hash'
</script>

<template>
  <HashTool
    title="SHA-512 Hash Generator"
    description="Generate SHA-512 hash from text. A secure cryptographic hash function."
    :hash-function="hashSHA512"
    :hash-length="128"
  >
    <template #info>
      SHA-512 produces a 512-bit (64-byte) hash value, rendered as a 128-character hexadecimal string.
      It is part of the SHA-2 family and provides stronger security than SHA-256.
    </template>
  </HashTool>
</template>
