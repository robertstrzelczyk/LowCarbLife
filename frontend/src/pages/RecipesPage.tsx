import { useState, type ReactNode } from 'react'
import { useQuery } from '@tanstack/react-query'
import { Link, useLocation, useSearchParams } from 'react-router'
import { api } from '../api/client'
import { useAuth } from '../auth/AuthContext'
import { AdminButton, PageHeader } from '../components/Layout'
import { useLanguage } from '../i18n/LanguageContext'
import { mealCategoriesForDiet, mealCategoryLabelKey } from '../lib/meals'
import type { DietType, MealCategory, RecipeListItem } from '../types'

type RecipeView = 'tiles' | 'list'

const VIEW_STORAGE_KEY = 'lcl_recipes_view'

function readRecipeView(): RecipeView {
  try {
    return localStorage.getItem(VIEW_STORAGE_KEY) === 'list' ? 'list' : 'tiles'
  } catch {
    return 'tiles'
  }
}

export function RecipesPage() {
  const { pathname } = useLocation()
  const { user } = useAuth()
  const { t } = useLanguage()
  const [params, setParams] = useSearchParams()
  const [view, setView] = useState<RecipeView>(readRecipeView)
  const dietType: DietType = pathname.includes('lowcarb') ? 'LowCarb' : 'Keto'
  const meals: Array<MealCategory | 'all'> = ['all', ...mealCategoriesForDiet(dietType)]
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

  const mealLabel = (category: MealCategory) => t(mealCategoryLabelKey(category))

  const setRecipeView = (next: RecipeView) => {
    setView(next)
    localStorage.setItem(VIEW_STORAGE_KEY, next)
  }

  return (
    <div>
      <PageHeader
        title={dietType === 'LowCarb' ? t('recipesLowcarbTitle') : t('recipesKetoTitle')}
        subtitle={t('recipesSubtitle')}
        action={user?.isAdmin ? <AdminButton to={`/przepisy/nowy?diet=${dietType}`}>{t('recipesAdd')}</AdminButton> : null}
      />

      <div className="mb-6 flex flex-wrap items-center justify-between gap-3">
        <div className="flex flex-wrap gap-2">
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

        <div
          className="flex rounded-full bg-white p-1 shadow-sm"
          role="group"
          aria-label={t('recipesViewLabel')}
        >
          <ViewButton
            label={t('recipesViewTiles')}
            active={view === 'tiles'}
            onClick={() => setRecipeView('tiles')}
          >
            <TilesIcon />
          </ViewButton>
          <ViewButton
            label={t('recipesViewList')}
            active={view === 'list'}
            onClick={() => setRecipeView('list')}
          >
            <ListIcon />
          </ViewButton>
        </div>
      </div>

      {recipes.isLoading ? <p>{t('loading')}</p> : null}
      {recipes.data?.length === 0 ? <p className="text-forest/70">{t('recipesEmpty')}</p> : null}

      {view === 'tiles' ? (
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
      ) : (
        <div className="flex flex-col gap-3">
          {recipes.data?.map((recipe) => (
            <Link
              key={recipe.id}
              to={`/przepisy/${recipe.id}`}
              className="flex overflow-hidden rounded-2xl bg-white shadow-sm transition hover:-translate-y-0.5 hover:shadow-md"
            >
              {recipe.imageUrl ? (
                <img src={recipe.imageUrl} alt="" className="h-24 w-28 shrink-0 object-cover sm:h-28 sm:w-36" />
              ) : (
                <div className="h-24 w-28 shrink-0 bg-forest/10 sm:h-28 sm:w-36" aria-hidden />
              )}
              <div className="min-w-0 p-3 sm:p-4">
                <p className="text-xs uppercase tracking-wide text-orange">{mealLabel(recipe.mealCategory)}</p>
                <h2 className="mt-0.5 truncate text-lg font-semibold text-forest">{recipe.title}</h2>
                <p className="mt-1 line-clamp-2 text-sm text-forest/70">{recipe.description}</p>
              </div>
            </Link>
          ))}
        </div>
      )}
    </div>
  )
}

function ViewButton({
  label,
  active,
  onClick,
  children,
}: {
  label: string
  active: boolean
  onClick: () => void
  children: ReactNode
}) {
  return (
    <button
      type="button"
      aria-pressed={active}
      aria-label={label}
      title={label}
      className={`inline-flex items-center gap-1.5 rounded-full px-3 py-1.5 text-sm font-medium ${
        active ? 'bg-forest text-cream' : 'text-forest hover:bg-forest/10'
      }`}
      onClick={onClick}
    >
      {children}
      <span className="hidden sm:inline">{label}</span>
    </button>
  )
}

function TilesIcon() {
  return (
    <svg viewBox="0 0 20 20" className="h-4 w-4" fill="currentColor" aria-hidden>
      <rect x="2.5" y="2.5" width="6.5" height="6.5" rx="1.2" />
      <rect x="11" y="2.5" width="6.5" height="6.5" rx="1.2" />
      <rect x="2.5" y="11" width="6.5" height="6.5" rx="1.2" />
      <rect x="11" y="11" width="6.5" height="6.5" rx="1.2" />
    </svg>
  )
}

function ListIcon() {
  return (
    <svg viewBox="0 0 20 20" className="h-4 w-4" fill="currentColor" aria-hidden>
      <rect x="2.5" y="3" width="3" height="3" rx="0.8" />
      <rect x="7.5" y="3.5" width="10" height="2" rx="1" />
      <rect x="2.5" y="8.5" width="3" height="3" rx="0.8" />
      <rect x="7.5" y="9" width="10" height="2" rx="1" />
      <rect x="2.5" y="14" width="3" height="3" rx="0.8" />
      <rect x="7.5" y="14.5" width="10" height="2" rx="1" />
    </svg>
  )
}
