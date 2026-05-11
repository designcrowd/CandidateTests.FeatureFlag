<script setup lang="ts">
import { ref } from 'vue'
import { FlaskConical, Play } from 'lucide-vue-next'

const props = defineProps<{
  evaluate: (userId: string, flagKey: string) => Promise<boolean>
  flagKeys: string[]
}>()

const userId = ref('user-123')
const flagKey = ref('')
const result = ref<boolean | null>(null)
const checking = ref(false)

async function check() {
  if (!flagKey.value) return
  checking.value = true
  try {
    result.value = await props.evaluate(userId.value, flagKey.value)
  } finally {
    checking.value = false
  }
}
</script>

<template>
  <section class="rounded-lg border border-border bg-surface">
    <header class="flex items-center gap-2.5 border-b border-border px-5 py-4">
      <FlaskConical :size="14" :stroke-width="2" class="text-accent-soft" />
      <h2 class="text-sm font-semibold uppercase tracking-[0.18em] text-text">Test as user</h2>
    </header>

    <div class="space-y-4 px-5 py-5">
      <div class="space-y-1.5">
        <label class="text-[10px] font-medium uppercase tracking-[0.18em] text-muted" for="tu-user">
          User ID
        </label>
        <input
          id="tu-user"
          v-model="userId"
          placeholder="user-123"
          class="block w-full rounded-md border border-border bg-elevated px-3 py-2 font-mono text-sm text-text placeholder:text-muted/60 transition-colors focus:border-accent focus:outline-none focus:ring-1 focus:ring-accent"
        />
      </div>

      <div class="space-y-1.5">
        <label class="text-[10px] font-medium uppercase tracking-[0.18em] text-muted" for="tu-flag">
          Flag
        </label>
        <select
          id="tu-flag"
          v-model="flagKey"
          class="block w-full rounded-md border border-border bg-elevated px-3 py-2 font-mono text-sm text-text transition-colors focus:border-accent focus:outline-none focus:ring-1 focus:ring-accent"
        >
          <option value="" disabled>Choose a flag…</option>
          <option v-for="k in flagKeys" :key="k" :value="k">{{ k }}</option>
        </select>
      </div>

      <button
        type="button"
        class="flex w-full items-center justify-center gap-2 rounded-md bg-accent px-4 py-2.5 text-sm font-medium text-white transition-all hover:brightness-110 active:brightness-95 disabled:cursor-not-allowed disabled:bg-elevated disabled:text-muted disabled:hover:brightness-100"
        :disabled="!flagKey || checking"
        @click="check"
      >
        <Play :size="13" :stroke-width="2.25" :class="checking ? 'animate-pulse' : ''" />
        {{ checking ? 'Evaluating…' : 'Evaluate' }}
      </button>

      <Transition
        enter-active-class="transition-all duration-200 ease-out"
        enter-from-class="opacity-0 translate-y-1"
        enter-to-class="opacity-100 translate-y-0"
      >
        <div
          v-if="result !== null"
          class="flex items-center justify-between rounded-md border border-border bg-elevated/50 px-3 py-2.5"
        >
          <span class="text-[10px] uppercase tracking-[0.18em] text-muted">Result</span>
          <span
            :class="[
              'rounded-full px-2.5 py-0.5 font-mono text-[10px] font-semibold uppercase tracking-wider',
              result ? 'bg-accent/20 text-accent-soft' : 'bg-danger/20 text-danger'
            ]"
          >
            {{ result ? 'enabled' : 'disabled' }}
          </span>
        </div>
      </Transition>
    </div>
  </section>
</template>
