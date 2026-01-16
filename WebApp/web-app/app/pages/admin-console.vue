<template>
  <div>
    <nuxt-page></nuxt-page>
  </div>
</template>

<script setup>

definePageMeta({
  middleware: [ 'user' ],
  pageTransition: {
    name: "zoom",
    mode: "out-in"
  },
})
</script>