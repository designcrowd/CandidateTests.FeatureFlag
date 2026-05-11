<script setup lang="ts">
import { Flag as FlagIcon } from 'lucide-vue-next'
</script>

<template>
  <div class="min-h-screen">
    <div class="mx-auto max-w-6xl px-8 py-10">
      <header class="flex items-end justify-between border-b border-border pb-6 animate-fade-in">
        <div class="flex items-center gap-3">
          <span class="flex h-9 w-9 items-center justify-center rounded-md bg-accent/20 text-accent-soft">
            <FlagIcon :size="18" :stroke-width="2.25" />
          </span>
          <div>
            <h1 class="text-2xl font-semibold tracking-tight text-text">Feature Flags</h1>
            <p class="text-xs uppercase tracking-[0.18em] text-muted">Admin / runtime</p>
          </div>
        </div>
        <div class="hidden text-right font-mono text-xs text-muted sm:block">
          <div>localhost:4000</div>
          <div class="mt-0.5 text-[10px] opacity-60">v0.1</div>
        </div>
      </header>
      <main class="mt-8">
        <NuxtPage />
      </main>
    </div>
  </div>
</template>
