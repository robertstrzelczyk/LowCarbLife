export function formatDate(value: string, language: 'pl' | 'en' = 'pl') {
  return new Date(value).toLocaleDateString(language === 'en' ? 'en-GB' : 'pl-PL', {
    day: 'numeric',
    month: 'long',
    year: 'numeric',
  })
}

export function formatNutritionValue(value: number | null | undefined, unit: string) {
  if (value == null || Number.isNaN(Number(value))) {
    return '-'
  }

  const rounded = Math.round(Number(value) * 10) / 10
  const formatted = Number.isInteger(rounded) ? String(rounded) : rounded.toFixed(1)
  return `${formatted} ${unit}`
}

export function parseNutritionInput(value: string) {
  const trimmed = value.trim().replace(',', '.')
  if (!trimmed) {
    return null
  }

  const numeric = Number(trimmed)
  return Number.isFinite(numeric) ? numeric : null
}
