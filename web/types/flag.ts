export interface Flag {
  id: number
  key: string
  name: string
  enabled: boolean
  rolloutPercent: number
  createdAt: string
  updatedAt: string
}

export interface FlagInput {
  key: string
  name: string
  enabled: boolean
  rolloutPercent: number
}

export interface EvaluateResponse {
  enabled: boolean
}
