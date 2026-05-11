<script setup lang="ts">
import type { FlagInput } from '~/types/flag'
import { Lock, X } from 'lucide-vue-next'

const props = defineProps<{
  modelValue: FlagInput
  isEditing: boolean
}>()

const emit = defineEmits<{
  'update:modelValue': [value: FlagInput]
  save: []
  cancel: []
}>()

function update<K extends keyof FlagInput>(field: K, value: FlagInput[K]) {
  emit('update:modelValue', { ...props.modelValue, [field]: value })
}
</script>

<template>
  <!-- Backdrop -->
  <div
    class="fixed inset-0 z-40 bg-bg/70 backdrop-blur-sm animate-fade-in"
    @click="emit('cancel')"
  />

  <!-- Drawer -->
  <aside
    class="fixed inset-y-0 right-0 z-50 flex w-full max-w-md flex-col border-l border-border bg-surface shadow-2xl animate-slide-in-right"
    role="dialog"
    aria-modal="true"
  >
    <header class="flex items-baseline justify-between border-b border-border px-6 py-5">
      <div>
        <p class="text-[10px] uppercase tracking-[0.2em] text-muted">
          {{ isEditing ? 'Editing' : 'New' }}
        </p>
        <h2 class="mt-1 text-lg font-semibold tracking-tight text-text">
          {{ isEditing ? modelValue.key : 'Create flag' }}
        </h2>
      </div>
      <button
        type="button"
        class="flex h-8 w-8 items-center justify-center rounded-md text-muted transition-colors hover:bg-elevated hover:text-text"
        aria-label="Close"
        @click="emit('cancel')"
      >
        <X :size="16" :stroke-width="1.75" />
      </button>
    </header>

    <form class="flex-1 space-y-5 overflow-y-auto px-6 py-6" @submit.prevent="emit('save')">
      <div class="space-y-1.5">
        <label class="flex items-center gap-1.5 text-[10px] font-medium uppercase tracking-[0.18em] text-muted" for="flag-key">
          Key
          <Lock v-if="isEditing" :size="11" :stroke-width="2" aria-label="Key cannot be changed" />
        </label>
        <input
          id="flag-key"
          :value="modelValue.key"
          :disabled="isEditing"
          placeholder="my-feature"
          class="block w-full rounded-md border border-border bg-elevated px-3 py-2.5 font-mono text-sm text-text placeholder:text-muted/60 transition-colors focus:border-accent focus:outline-none focus:ring-1 focus:ring-accent disabled:cursor-not-allowed disabled:bg-elevated/30 disabled:text-muted"
          @input="update('key', ($event.target as HTMLInputElement).value)"
        />
      </div>

      <div class="space-y-1.5">
        <label class="text-[10px] font-medium uppercase tracking-[0.18em] text-muted" for="flag-name">
          Name
        </label>
        <input
          id="flag-name"
          :value="modelValue.name"
          placeholder="A human-readable label"
          class="block w-full rounded-md border border-border bg-elevated px-3 py-2.5 text-sm text-text placeholder:text-muted/60 transition-colors focus:border-accent focus:outline-none focus:ring-1 focus:ring-accent"
          @input="update('name', ($event.target as HTMLInputElement).value)"
        />
      </div>

      <!-- Enabled toggle -->
      <div class="flex items-center justify-between rounded-md border border-border bg-elevated/40 px-3 py-2.5">
        <div>
          <p class="text-sm text-text">Enabled</p>
          <p class="text-xs text-muted">Master switch. If off, rollout is ignored.</p>
        </div>
        <label class="relative inline-flex cursor-pointer items-center">
          <input
            type="checkbox"
            class="peer sr-only"
            :checked="modelValue.enabled"
            @change="update('enabled', ($event.target as HTMLInputElement).checked)"
          />
          <span class="block h-5 w-9 rounded-full bg-border transition-colors peer-checked:bg-accent" />
          <span class="absolute left-0.5 top-0.5 h-4 w-4 rounded-full bg-bg transition-transform peer-checked:translate-x-4" />
        </label>
      </div>

      <!-- Rollout: slider + numeric, bound together -->
      <div class="space-y-2">
        <div class="flex items-baseline justify-between">
          <label class="text-[10px] font-medium uppercase tracking-[0.18em] text-muted" for="flag-rollout">
            Rollout
          </label>
          <span class="font-mono text-sm tabular-nums text-text">{{ modelValue.rolloutPercent }}%</span>
        </div>
        <input
          id="flag-rollout"
          type="range"
          min="0"
          max="100"
          :value="modelValue.rolloutPercent"
          class="block w-full"
          @input="update('rolloutPercent', Number(($event.target as HTMLInputElement).value))"
        />
        <div class="flex justify-between font-mono text-[10px] text-muted">
          <span>0</span>
          <span>50</span>
          <span>100</span>
        </div>
      </div>
    </form>

    <footer class="flex items-center justify-end gap-2 border-t border-border bg-surface px-6 py-4">
      <button
        type="button"
        class="rounded-md px-4 py-2 text-sm text-muted transition-colors hover:bg-elevated hover:text-text"
        @click="emit('cancel')"
      >
        Cancel
      </button>
      <button
        type="button"
        class="rounded-md bg-accent px-4 py-2 text-sm font-medium text-white transition-all hover:brightness-110 active:brightness-95"
        @click="emit('save')"
      >
        {{ isEditing ? 'Save changes' : 'Create flag' }}
      </button>
    </footer>
  </aside>
</template>
