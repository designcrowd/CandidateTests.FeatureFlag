import { ref, computed } from 'vue'
import type { Flag, FlagInput, EvaluateResponse } from '~/types/flag'

export function useFlags() {
  const config = useRuntimeConfig()
  const apiBase = config.public.apiBase

  const flags = ref<Flag[]>([])
  const isLoading = ref(false)
  const error = ref<string | null>(null)

  const editingKey = ref<string | null>(null)
  const formValues = ref<FlagInput>(emptyForm())

  const isFormOpen = computed(() => editingKey.value !== null)
  const isUpdate = computed(() => editingKey.value !== null && editingKey.value !== '')

  async function loadFlags() {
    isLoading.value = true
    error.value = null
    try {
      flags.value = await $fetch<Flag[]>(`${apiBase}/flags`)
    } catch (e: any) {
      error.value = e?.message ?? 'Failed to load flags'
    } finally {
      isLoading.value = false
    }
  }

  function startCreate() {
    editingKey.value = ''
    formValues.value = emptyForm()
  }

  function startEdit(flag: Flag) {
    editingKey.value = flag.key
    formValues.value = {
      key: flag.key,
      name: flag.name,
      enabled: flag.enabled,
      rolloutPercent: flag.rolloutPercent
    }
  }

  function cancelEdit() {
    editingKey.value = null
  }

  async function saveFlag() {
    const updating = isUpdate.value
    const url = updating
      ? `${apiBase}/flags/${encodeURIComponent(editingKey.value!)}`
      : `${apiBase}/flags`
    const method = updating ? 'PUT' : 'POST'
    await $fetch(url, { method, body: formValues.value })
    editingKey.value = null
    await loadFlags()
  }

  async function deleteFlag(key: string) {
    await $fetch(`${apiBase}/flags/${encodeURIComponent(key)}`, { method: 'DELETE' })
    await loadFlags()
  }

  async function evaluateFlag(userId: string, flagKey: string): Promise<boolean> {
    const result = await $fetch<EvaluateResponse>(`${apiBase}/evaluate`, {
      query: { userId, flagKey }
    })
    return result.enabled
  }

  return {
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
  }
}

function emptyForm(): FlagInput {
  return { key: '', name: '', enabled: true, rolloutPercent: 100 }
}
