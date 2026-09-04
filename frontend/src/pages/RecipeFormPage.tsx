import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { useEffect, useState } from 'react'
import { useNavigate, useParams, useSearchParams } from 'react-router'
import { api } from '../api/client'
import { useLanguage } from '../i18n/LanguageContext'
import type { DietType, Ingredient, MealCategory, RecipeDetail } from '../types'

const emptyIngredient: Ingredient = { name: '', amount: '' }

export function RecipeFormPage() {
  const { t } = useLanguage()
  const { id } = useParams()
  const [params] = useSearchParams()
  const navigate = useNavigate()
  const queryClient = useQueryClient()
  const isEdit = Boolean(id)
  const existing = useQuery({
    queryKey: ['recipe', id],
    queryFn: () => api<RecipeDetail>(`/api/recipes/${id}`),
    enabled: isEdit,
  })

  const [title, setTitle] = useState('')
  const [description, setDescription] = useState('')
  const [instructions, setInstructions] = useState('')
  const [youtubeUrl, setYoutubeUrl] = useState('')
  const [imageUrl, setImageUrl] = useState('')
  const [dietType, setDietType] = useState<DietType>((params.get('diet') as DietType) || 'Keto')
  const [mealCategory, setMealCategory] = useState<MealCategory>('Sniadanie')
  const [ingredients, setIngredients] = useState<Ingredient[]>([emptyIngredient])
  const [error, setError] = useState('')

  useEffect(() => {
    if (existing.data) {
      setTitle(existing.data.title)
      setDescription(existing.data.description)
      setInstructions(existing.data.instructions)
      setYoutubeUrl(existing.data.youtubeUrl ?? '')
      setImageUrl(existing.data.imageUrl ?? '')
      setDietType(existing.data.dietType)
      setMealCategory(existing.data.mealCategory)
      setIngredients(existing.data.ingredients.length ? existing.data.ingredients : [emptyIngredient])
    }
  }, [existing.data])

  const save = useMutation({
    mutationFn: () =>
      api<RecipeDetail>(isEdit ? `/api/recipes/${id}` : '/api/recipes', {
        method: isEdit ? 'PUT' : 'POST',
        body: JSON.stringify({
          title,
          description,
          instructions,
          youtubeUrl: youtubeUrl || null,
          imageUrl: imageUrl || null,
          dietType,
          mealCategory,
          ingredients: ingredients.filter((item) => item.name.trim()),
        }),
      }),
    onSuccess: async (recipe) => {
      await queryClient.invalidateQueries({ queryKey: ['recipes'] })
      navigate(`/przepisy/${recipe.id}`)
    },
    onError: (err: Error) => setError(err.message),
  })

  return (
    <form
      className="mx-auto max-w-2xl space-y-4 rounded-2xl bg-white p-6 shadow-sm"
      onSubmit={(event) => {
        event.preventDefault()
        save.mutate()
      }}
    >
      <h1 className="text-2xl font-semibold text-forest">{isEdit ? t('recipesEditTitle') : t('recipesNew')}</h1>
      {error ? <p className="text-sm text-orange-dark">{error}</p> : null}
      <label className="block text-sm font-medium text-forest">
        {t('fieldTitle')}
        <input className="mt-1 w-full rounded-lg border border-forest/20 px-3 py-2" value={title} onChange={(e) => setTitle(e.target.value)} required />
      </label>
      <div className="grid gap-4 md:grid-cols-2">
        <label className="block text-sm font-medium text-forest">
          {t('fieldDiet')}
          <select className="mt-1 w-full rounded-lg border border-forest/20 px-3 py-2" value={dietType} onChange={(e) => setDietType(e.target.value as DietType)}>
            <option value="Keto">{t('dietKeto')}</option>
            <option value="LowCarb">{t('dietLowcarb')}</option>
          </select>
        </label>
        <label className="block text-sm font-medium text-forest">
          {t('fieldMeal')}
          <select className="mt-1 w-full rounded-lg border border-forest/20 px-3 py-2" value={mealCategory} onChange={(e) => setMealCategory(e.target.value as MealCategory)}>
            <option value="Sniadanie">{t('mealBreakfast')}</option>
            <option value="Obiad">{t('mealLunch')}</option>
            <option value="Kolacja">{t('mealDinner')}</option>
          </select>
        </label>
      </div>
      <label className="block text-sm font-medium text-forest">
        {t('fieldDescription')}
        <textarea className="mt-1 min-h-24 w-full rounded-lg border border-forest/20 px-3 py-2" value={description} onChange={(e) => setDescription(e.target.value)} required />
      </label>
      <label className="block text-sm font-medium text-forest">
        {t('fieldInstructions')}
        <textarea className="mt-1 min-h-36 w-full rounded-lg border border-forest/20 px-3 py-2" value={instructions} onChange={(e) => setInstructions(e.target.value)} required />
      </label>
      <label className="block text-sm font-medium text-forest">
        {t('fieldYoutubeUrl')}
        <input className="mt-1 w-full rounded-lg border border-forest/20 px-3 py-2" value={youtubeUrl} onChange={(e) => setYoutubeUrl(e.target.value)} />
      </label>
      <label className="block text-sm font-medium text-forest">
        {t('fieldImageUrl')}
        <input className="mt-1 w-full rounded-lg border border-forest/20 px-3 py-2" value={imageUrl} onChange={(e) => setImageUrl(e.target.value)} />
      </label>
      <fieldset>
        <legend className="text-sm font-medium text-forest">{t('recipesIngredients')}</legend>
        <div className="mt-2 space-y-2">
          {ingredients.map((item, index) => (
            <div key={index} className="grid grid-cols-[1fr_8rem_auto] gap-2">
              <input
                className="rounded-lg border border-forest/20 px-3 py-2"
                placeholder={t('fieldIngredientName')}
                value={item.name}
                onChange={(e) =>
                  setIngredients((current) => current.map((row, i) => (i === index ? { ...row, name: e.target.value } : row)))
                }
              />
              <input
                className="rounded-lg border border-forest/20 px-3 py-2"
                placeholder={t('fieldIngredientAmount')}
                value={item.amount ?? ''}
                onChange={(e) =>
                  setIngredients((current) => current.map((row, i) => (i === index ? { ...row, amount: e.target.value } : row)))
                }
              />
              <button
                type="button"
                className="text-sm text-orange"
                onClick={() => setIngredients((current) => current.filter((_, i) => i !== index))}
              >
                {t('blogDelete')}
              </button>
            </div>
          ))}
        </div>
        <button type="button" className="mt-3 text-sm font-medium text-forest" onClick={() => setIngredients((current) => [...current, { name: '', amount: '' }])}>
          {t('recipesAddIngredient')}
        </button>
      </fieldset>
      <button type="submit" className="rounded-full bg-forest px-5 py-2 font-semibold text-cream" disabled={save.isPending}>
        {t('save')}
      </button>
    </form>
  )
}
