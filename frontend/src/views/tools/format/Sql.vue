<script setup lang="ts">
import { ref, computed } from 'vue'
import ToolLayout from '../../../components/common/ToolLayout.vue'
import CopyButton from '../../../components/common/CopyButton.vue'

const input = ref('')
const uppercase = ref(true)

const keywords = [
  'SELECT', 'FROM', 'WHERE', 'AND', 'OR', 'NOT', 'IN', 'LIKE', 'BETWEEN',
  'JOIN', 'LEFT', 'RIGHT', 'INNER', 'OUTER', 'FULL', 'CROSS', 'ON',
  'GROUP BY', 'ORDER BY', 'HAVING', 'LIMIT', 'OFFSET',
  'INSERT', 'INTO', 'VALUES', 'UPDATE', 'SET', 'DELETE',
  'CREATE', 'TABLE', 'INDEX', 'VIEW', 'DATABASE', 'DROP', 'ALTER', 'ADD',
  'PRIMARY', 'KEY', 'FOREIGN', 'REFERENCES', 'CONSTRAINT', 'UNIQUE',
  'NULL', 'NOT NULL', 'DEFAULT', 'AUTO_INCREMENT',
  'AS', 'DISTINCT', 'ALL', 'UNION', 'EXCEPT', 'INTERSECT',
  'CASE', 'WHEN', 'THEN', 'ELSE', 'END',
  'COUNT', 'SUM', 'AVG', 'MIN', 'MAX', 'COALESCE', 'NULLIF',
  'ASC', 'DESC', 'EXISTS', 'IS', 'TRUE', 'FALSE'
]

const formatSql = (sql: string): string => {
  let result = sql.trim()

  // Normalize whitespace
  result = result.replace(/\s+/g, ' ')

  // Keywords that start a new line
  const newLineKeywords = [
    'SELECT', 'FROM', 'WHERE', 'AND', 'OR',
    'JOIN', 'LEFT JOIN', 'RIGHT JOIN', 'INNER JOIN', 'OUTER JOIN', 'FULL JOIN', 'CROSS JOIN',
    'GROUP BY', 'ORDER BY', 'HAVING', 'LIMIT', 'OFFSET',
    'INSERT INTO', 'VALUES', 'UPDATE', 'SET', 'DELETE FROM',
    'CREATE TABLE', 'CREATE INDEX', 'DROP TABLE', 'ALTER TABLE',
    'UNION', 'EXCEPT', 'INTERSECT',
    'CASE', 'WHEN', 'THEN', 'ELSE', 'END'
  ]

  // Add newlines before keywords
  newLineKeywords.forEach(keyword => {
    const regex = new RegExp(`\\b(${keyword})\\b`, 'gi')
    result = result.replace(regex, '\n$1')
  })

  // Indent after SELECT, SET
  result = result.replace(/\bSELECT\b/gi, 'SELECT\n  ')
  result = result.replace(/,\s*/g, ',\n  ')

  // Handle parentheses
  result = result.replace(/\(\s*/g, '(\n  ')
  result = result.replace(/\s*\)/g, '\n)')

  // Clean up extra newlines
  result = result.replace(/\n\s*\n/g, '\n')
  result = result.replace(/^\n/, '')

  // Apply case
  if (uppercase.value) {
    keywords.forEach(keyword => {
      const regex = new RegExp(`\\b${keyword}\\b`, 'gi')
      result = result.replace(regex, keyword.toUpperCase())
    })
  } else {
    keywords.forEach(keyword => {
      const regex = new RegExp(`\\b${keyword}\\b`, 'gi')
      result = result.replace(regex, keyword.toLowerCase())
    })
  }

  return result.trim()
}

const minifySql = (sql: string): string => {
  return sql
    .replace(/\s+/g, ' ')
    .replace(/\s*,\s*/g, ',')
    .replace(/\s*\(\s*/g, '(')
    .replace(/\s*\)\s*/g, ')')
    .trim()
}

const formatted = computed(() => {
  if (!input.value.trim()) return ''
  return formatSql(input.value)
})

const format = () => {
  if (input.value.trim()) {
    input.value = formatted.value
  }
}

const minify = () => {
  if (input.value.trim()) {
    input.value = minifySql(input.value)
  }
}

const clear = () => {
  input.value = ''
}

const loadSample = () => {
  input.value = `SELECT u.id, u.name, u.email, COUNT(o.id) as order_count FROM users u LEFT JOIN orders o ON u.id = o.user_id WHERE u.active = true AND u.created_at > '2024-01-01' GROUP BY u.id, u.name, u.email HAVING COUNT(o.id) > 5 ORDER BY order_count DESC LIMIT 10`
}
</script>

<template>
  <ToolLayout title="SQL Formatter" description="Format and beautify SQL queries">
    <div class="space-y-6">
      <!-- Options -->
      <div class="flex flex-wrap items-center gap-4">
        <label class="flex items-center gap-2 text-sm text-gray-600 dark:text-gray-400">
          <input
            type="checkbox"
            v-model="uppercase"
            class="rounded border-gray-300 dark:border-dark-600 text-primary-600 focus:ring-primary-500"
          />
          Uppercase keywords
        </label>

        <div class="flex gap-2">
          <button @click="format" class="tool-button" :disabled="!input.trim()">
            Format
          </button>
          <button @click="minify" class="tool-button-secondary" :disabled="!input.trim()">
            Minify
          </button>
          <button @click="loadSample" class="tool-button-secondary">
            Sample
          </button>
          <button @click="clear" class="tool-button-secondary">
            Clear
          </button>
        </div>
      </div>

      <!-- Editor -->
      <div class="space-y-2">
        <div class="flex items-center justify-between">
          <label class="text-sm font-medium text-gray-700 dark:text-gray-300">SQL Query</label>
          <CopyButton v-if="formatted" :text="formatted" />
        </div>
        <textarea
          v-model="input"
          placeholder="Paste your SQL query here..."
          class="tool-textarea h-96 font-mono text-sm"
          spellcheck="false"
        />
      </div>

      <!-- Preview -->
      <div v-if="formatted" class="space-y-2">
        <label class="text-sm font-medium text-gray-700 dark:text-gray-300">Preview</label>
        <pre class="p-4 bg-gray-50 dark:bg-dark-800 rounded-lg overflow-x-auto font-mono text-sm text-gray-800 dark:text-gray-200">{{ formatted }}</pre>
      </div>
    </div>
  </ToolLayout>
</template>
