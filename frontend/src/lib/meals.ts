import type { TranslationKey } from '../i18n/translations'
import type { DietType, MealCategory } from '../types'

export const coreMealCategories: MealCategory[] = ['Sniadanie', 'Obiad', 'Kolacja']

export const ketoMealCategories: MealCategory[] = [
  ...coreMealCategories,
  'Przekaski',
  'Smoothie',
  'Desery',
  'Salatki',
  'Zupy',
  'Lunchboxy',
]

const mealCategoryLabels: Record<MealCategory, TranslationKey> = {
  Sniadanie: 'mealBreakfast',
  Obiad: 'mealLunch',
  Kolacja: 'mealDinner',
  Przekaski: 'mealSnacks',
  Smoothie: 'mealSmoothie',
  Desery: 'mealDesserts',
  Salatki: 'mealSalads',
  Zupy: 'mealSoups',
  Lunchboxy: 'mealLunchboxes',
}

export function mealCategoriesForDiet(diet: DietType): MealCategory[] {
  return diet === 'Keto' ? ketoMealCategories : coreMealCategories
}

export function mealCategoryLabelKey(category: MealCategory): TranslationKey {
  return mealCategoryLabels[category]
}
