<script setup lang="ts">
import type { Flag } from '~/types/flag'
import { Pencil, Trash2, FlagOff } from 'lucide-vue-next'

defineProps<{
  flags: Flag[]
  isLoading: boolean
}>()

defineEmits<{
  edit: [flag: Flag]
  delete: [key: string]
}>()
</script>

<template>
  <section class="overflow-hidden rounded-lg border border-border bg-surface">
    <header class="flex items-baseline justify-between border-b border-border px-5 py-4">
      <div class="flex items-baseline gap-3">
        <h2 class="text-sm font-semibold uppercase tracking-[0.18em] text-text">Flags</h2>
        <span class="font-mono text-xs text-muted">
          {{ flags.length }} <span class="opacity-60">defined</span>
        </span>
      </div>
      <span v-if="isLoading" class="font-mono text-xs text-muted">syncing…</span>
    </header>

    <!-- Skeleton loader on first load -->
    <div v-if="isLoading && flags.length === 0" class="divide-y divide-border">
      <div v-for="i in 4" :key="i" class="flex animate-pulse items-center gap-4 px-5 py-4">
        <div class="h-3 w-32 rounded bg-border" />
        <div class="h-3 w-48 rounded bg-border opacity-60" />
        <div class="ml-auto h-3 w-12 rounded bg-border opacity-40" />
      </div>
    </div>

    <!-- Empty state -->
    <div
      v-else-if="!isLoading && flags.length === 0"
      class="flex flex-col items-center justify-center px-5 py-16 text-center"
    >
      <div class="mb-3 flex h-12 w-12 items-center justify-center rounded-full bg-elevated text-muted">
        <FlagOff :size="20" :stroke-width="1.75" />
      </div>
      <p class="text-sm text-text">No flags yet</p>
      <p class="mt-1 text-xs text-muted">Create your first flag to control rollouts.</p>
    </div>

    <!-- Rows -->
    <ul v-else class="divide-y divide-border">
      <li
        v-for="(flag, idx) in flags"
        :key="flag.key"
        class="group relative flex items-center gap-5 px-5 py-4 transition-colors hover:bg-elevated/60 animate-fade-up"
        :style="{ animationDelay: `${idx * 30}ms` }"
      >
        <!-- Left state strip: solid accent if enabled, striped muted if disabled -->
        <span
          :class="[
            'absolute left-0 top-0 h-full w-[3px]',
            flag.enabled ? 'bg-accent-soft' : 'pattern-stripes bg-muted/30'
          ]"
          aria-hidden="true"
        />

        <!-- Key + name + rollout band -->
        <div class="min-w-0 flex-1">
          <div class="flex items-baseline gap-3">
            <code class="font-mono text-sm font-medium text-text">{{ flag.key }}</code>
            <span class="truncate text-xs text-muted">{{ flag.name }}</span>
          </div>
          <!-- Rollout track -->
          <div class="mt-2.5 flex items-center gap-3">
            <div class="relative h-1 w-full max-w-[280px] overflow-hidden rounded-full bg-elevated">
              <div
                class="absolute inset-y-0 left-0 origin-left animate-rollout-grow rounded-full"
                :class="flag.enabled ? 'bg-accent-soft' : 'bg-muted/30'"
                :style="{ width: `${flag.rolloutPercent}%`, animationDelay: `${idx * 30 + 200}ms` }"
              />
            </div>
            <span class="font-mono text-[11px] tabular-nums text-muted">
              {{ flag.rolloutPercent.toString().padStart(3, ' ') }}%
            </span>
          </div>
        </div>

        <!-- State chip -->
        <span
          :class="[
            'shrink-0 rounded-full px-2.5 py-0.5 font-mono text-[10px] font-medium uppercase tracking-wider',
            flag.enabled
              ? 'bg-accent/20 text-accent-soft'
              : 'bg-muted/15 text-muted'
          ]"
        >
          {{ flag.enabled ? 'on' : 'off' }}
        </span>

        <!-- Row actions: appear on hover -->
        <div class="flex shrink-0 items-center gap-1 opacity-0 transition-opacity duration-150 group-hover:opacity-100 focus-within:opacity-100">
          <button
            type="button"
            class="flex h-8 w-8 items-center justify-center rounded-md text-muted transition-colors hover:bg-elevated hover:text-text focus:outline-none focus:ring-1 focus:ring-accent"
            aria-label="Edit flag"
            @click="$emit('edit', flag)"
          >
            <Pencil :size="14" :stroke-width="1.75" />
          </button>
          <button
            type="button"
            class="flex h-8 w-8 items-center justify-center rounded-md text-muted transition-colors hover:bg-danger/10 hover:text-danger focus:outline-none focus:ring-1 focus:ring-danger"
            aria-label="Delete flag"
            @click="$emit('delete', flag.key)"
          >
            <Trash2 :size="14" :stroke-width="1.75" />
          </button>
        </div>
      </li>
    </ul>
  </section>
</template>
