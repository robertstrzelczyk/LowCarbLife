import { useLanguage } from '../i18n/LanguageContext'
import { formatNutritionValue } from '../lib/format'
import type { Nutrition } from '../types'

export function NutritionTable({ nutrition }: { nutrition: Nutrition | null | undefined }) {
  const { t } = useLanguage()
  const values = nutrition ?? {
    caloriesKcal: null,
    proteinGrams: null,
    fatGrams: null,
    carbsGrams: null,
    fiberGrams: null,
  }

  const rows = [
    { label: t('nutritionCalories'), value: formatNutritionValue(values.caloriesKcal, 'kcal') },
    { label: t('nutritionProtein'), value: formatNutritionValue(values.proteinGrams, 'g') },
    { label: t('nutritionFat'), value: formatNutritionValue(values.fatGrams, 'g') },
    { label: t('nutritionCarbs'), value: formatNutritionValue(values.carbsGrams, 'g') },
    { label: t('nutritionFiber'), value: formatNutritionValue(values.fiberGrams, 'g') },
  ]

  return (
    <section className="rounded-2xl bg-white p-5 shadow-sm">
      <h2 className="text-lg font-semibold text-forest">{t('recipesNutritionTitle')}</h2>
      <p className="mt-1 text-xs uppercase tracking-wide text-orange">{t('recipesNutritionPerServing')}</p>
      <div className="mt-4 overflow-x-auto">
        <table className="w-full min-w-[28rem] table-fixed text-center">
          <thead>
            <tr>
              {rows.map((row) => (
                <th key={row.label} className="px-2 pb-2 text-lg font-semibold text-forest">
                  {row.value}
                </th>
              ))}
            </tr>
          </thead>
          <tbody>
            <tr>
              {rows.map((row) => (
                <td key={row.label} className="px-2 pt-1 text-xs text-forest/60">
                  {row.label}
                </td>
              ))}
            </tr>
          </tbody>
        </table>
      </div>
    </section>
  )
}
