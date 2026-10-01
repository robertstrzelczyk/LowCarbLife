import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { Link, useNavigate, useParams } from 'react-router'
import { api } from '../api/client'
import { useAuth } from '../auth/AuthContext'
import { IngredientsTable } from '../components/IngredientsTable'
import { NutritionTable } from '../components/NutritionTable'
import { useLanguage } from '../i18n/LanguageContext'
import { mealCategoryLabelKey } from '../lib/meals'
import type { RecipeDetail } from '../types'

export function RecipeDetailPage() {
  const { id } = useParams()
  const { user } = useAuth()
  const { t } = useLanguage()
  const navigate = useNavigate()
  const queryClient = useQueryClient()
  const recipe = useQuery({
    queryKey: ['recipe', id],
    queryFn: () => api<RecipeDetail>(`/api/recipes/${id}`),
    enabled: Boolean(id),
  })

  const remove = useMutation({
    mutationFn: () => api(`/api/recipes/${id}`, { method: 'DELETE' }),
    onSuccess: async () => {
      await queryClient.invalidateQueries({ queryKey: ['recipes'] })
      navigate('/przepisy/keto')
    },
  })

  if (recipe.isLoading) {
    return <p>{t('loading')}</p>
  }
  if (!recipe.data) {
    return <p>{t('recipesNotFound')}</p>
  }

  const backTo = recipe.data.dietType === 'LowCarb' ? '/przepisy/lowcarb' : '/przepisy/keto'
  const dietLabel = recipe.data.dietType === 'LowCarb' ? t('dietLowcarb') : t('dietKeto')
  const mealLabel = t(mealCategoryLabelKey(recipe.data.mealCategory))

  return (
    <article className="grid gap-8 lg:grid-cols-[1.2fr_0.8fr]">
      <div>
        <Link to={backTo} className="text-sm text-sage hover:text-forest">
          {t('recipesBack')}
        </Link>
        <p className="mt-4 text-xs uppercase tracking-wide text-orange">
          {dietLabel} · {mealLabel}
        </p>
        <h1 className="mt-2 text-4xl font-semibold text-forest">{recipe.data.title}</h1>
        {user?.isAdmin ? (
          <div className="mt-4 flex gap-3">
            <Link to={`/przepisy/${recipe.data.id}/edytuj`} className="rounded-full bg-forest px-4 py-2 text-sm font-semibold text-cream">
              {t('blogEdit')}
            </Link>
            <button
              type="button"
              className="rounded-full bg-orange px-4 py-2 text-sm font-semibold text-white"
              onClick={() => {
                if (confirm(t('recipesDeleteConfirm'))) {
                  remove.mutate()
                }
              }}
            >
              {t('blogDelete')}
            </button>
          </div>
        ) : null}
        {recipe.data.imageUrl ? (
          <img src={recipe.data.imageUrl} alt="" className="mt-6 aspect-[4/3] w-full rounded-2xl object-cover" />
        ) : null}
        <p className="mt-6 leading-7 text-forest/80">{recipe.data.description}</p>
        <h2 className="mt-8 text-xl font-semibold text-forest">{t('recipesHowTo')}</h2>
        <div className="mt-3 whitespace-pre-wrap leading-7 text-forest/90">{recipe.data.instructions}</div>
      </div>

      <aside className="space-y-6">
        <IngredientsTable ingredients={recipe.data.ingredients} />

        <NutritionTable nutrition={recipe.data.nutrition} />
      </aside>
    </article>
  )
}
