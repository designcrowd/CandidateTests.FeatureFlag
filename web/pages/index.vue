<script setup lang="ts">
import { computed, onMounted } from 'vue'
import { Plus } from 'lucide-vue-next'

const {
  flags,
  isLoading,
  error,
  formValues,
  isFormOpen,
  isUpdate,
  loadFlags,
  startCreate,
  startEdit,
  cancelEdit,
  saveFlag,
  deleteFlag,
  evaluateFlag
} = useFlags()

const flagKeys = computed(() => flags.value.map(f => f.key))

onMounted(() => {
  loadFlags()
})

function onModelUpdate(value: typeof formValues.value) {
  formValues.value = value
}
</script>

<template>
  <div class="grid grid-cols-1 gap-6 lg:grid-cols-[1fr_320px]">
    <!-- Main column: list + toolbar -->
    <div class="flex min-w-0 flex-col gap-4">
      <div class="flex items-center justify-between">
        <p class="text-xs text-muted">
          Manage the on/off state and gradual rollout of features across your app.
        </p>
        <button
          type="button"
          class="flex items-center gap-1.5 rounded-md bg-accent px-3.5 py-2 text-sm font-medium text-white transition-all hover:brightness-110 active:brightness-95"
          @click="startCreate"
        >
          <Plus :size="14" :stroke-width="2.5" />
          New flag
        </button>
      </div>

      <div
        v-if="error"
        class="rounded-md border border-danger/40 bg-danger/10 px-4 py-3 text-sm text-danger"
      >
        {{ error }}
      </div>

      <FlagList
        :flags="flags"
        :is-loading="isLoading"
        @edit="startEdit"
        @delete="deleteFlag"
      />
    </div>

    <!-- Sidebar: test panel -->
    <aside class="lg:sticky lg:top-10 lg:self-start">
      <TestUserPanel
        :evaluate="evaluateFlag"
        :flag-keys="flagKeys"
      />
    </aside>

    <!-- Slide-over drawer -->
    <FlagForm
      v-if="isFormOpen"
      :model-value="formValues"
      :is-editing="isUpdate"
      @update:model-value="onModelUpdate"
      @save="saveFlag"
      @cancel="cancelEdit"
    />
  </div>
</template>
