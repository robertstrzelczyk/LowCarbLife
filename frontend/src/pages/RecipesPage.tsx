import { useQuery } from '@tanstack/react-query'
import { Link, useLocation, useSearchParams } from 'react-router'
import { api } from '../api/client'
import { useAuth } from '../auth/AuthContext'
import { AdminButton, PageHeader } from '../components/Layout'
import { useLanguage } from '../i18n/LanguageContext'
import type { DietType, MealCategory, RecipeListItem } from '../types'

const meals: Array<MealCategory | 'all'> = ['all', 'Sniadanie', 'Obiad', 'Kolacja']

export function RecipesPage() {
  const { pathname } = useLocation()
  const { user } = useAuth()
  const { t } = useLanguage()
  const [params, setParams] = useSearchParams()
  const dietType: DietType = pathname.includes('lowcarb') ? 'LowCarb' : 'Keto'
  const meal = (params.get('meal') as MealCategory | null) ?? undefined

  const recipes = useQuery({
    queryKey: ['recipes', dietType, meal],
    queryFn: () => {
      const search = new URLSearchParams({ diet: dietType })
      if (meal) {
        search.set('meal', meal)
      }
      return api<RecipeListItem[]>(`/api/recipes?${search.toString()}`)
    },
  })

  const mealLabel = (category: MealCategory) =>
    category === 'Sniadanie' ? t('mealBreakfast') : category === 'Obiad' ? t('mealLunch') : t('mealDinner')

  return (
    <div>
      <PageHeader
        title={dietType === 'LowCarb' ? t('recipesLowcarbTitle') : t('recipesKetoTitle')}
        subtitle={t('recipesSubtitle')}
        action={user?.isAdmin ? <AdminButton to={`/przepisy/nowy?diet=${dietType}`}>{t('recipesAdd')}</AdminButton> : null}
      />

      <div className="mb-6 flex flex-wrap gap-2">
        {meals.map((item) => {
          const active = item === 'all' ? !meal : meal === item
          return (
            <button
              key={item}
              type="button"
              className={`rounded-full px-4 py-2 text-sm font-medium ${
                active ? 'bg-forest text-cream' : 'bg-white text-forest hover:bg-forest/10'
              }`}
              onClick={() => {
                const next = new URLSearchParams(params)
                if (item === 'all') {
                  next.delete('meal')
                } else {
                  next.set('meal', item)
                }
                setParams(next)
              }}
            >
              {item === 'all' ? t('recipesAll') : mealLabel(item)}
            </button>
          )
        })}
      </div>

      {recipes.isLoading ? <p>{t('loading')}</p> : null}
      {recipes.data?.length === 0 ? <p className="text-forest/70">{t('recipesEmpty')}</p> : null}

      <div className="grid gap-6 md:grid-cols-3">
        {recipes.data?.map((recipe) => (
          <Link
            key={recipe.id}
            to={`/przepisy/${recipe.id}`}
            className="overflow-hidden rounded-2xl bg-white shadow-sm transition hover:-translate-y-0.5 hover:shadow-md"
          >
            {recipe.imageUrl ? <img src={recipe.imageUrl} alt="" className="h-40 w-full object-cover" /> : null}
            <div className="p-4">
              <p className="text-xs uppercase tracking-wide text-orange">{mealLabel(recipe.mealCategory)}</p>
              <h2 className="mt-1 text-lg font-semibold text-forest">{recipe.title}</h2>
              <p className="mt-2 line-clamp-3 text-sm text-forest/70">{recipe.description}</p>
            </div>
          </Link>
        ))}
      </div>
    </div>
  )
}
