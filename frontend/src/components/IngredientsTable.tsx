import { useLanguage } from '../i18n/LanguageContext'
import type { Ingredient } from '../types'

export function IngredientsTable({ ingredients }: { ingredients: Ingredient[] }) {
  const { t } = useLanguage()

  return (
    <section className="rounded-2xl bg-white p-5 shadow-sm">
      <h2 className="text-lg font-semibold text-forest">{t('recipesIngredients')}</h2>
      <div className="mt-4 overflow-hidden rounded-xl border border-forest/10">
        <table className="w-full text-sm">
          <thead className="bg-cream">
            <tr className="text-left text-xs uppercase tracking-wide text-forest/70">
              <th className="px-4 py-2.5 font-semibold">{t('recipesColIngredient')}</th>
              <th className="px-4 py-2.5 text-right font-semibold">{t('recipesColAmount')}</th>
            </tr>
          </thead>
          <tbody>
            {ingredients.map((item, index) => (
              <tr
                key={`${item.name}-${index}`}
                className={index % 2 === 0 ? 'bg-white' : 'bg-cream/60'}
              >
                <td className="px-4 py-3 text-forest">{item.name}</td>
                <td className="px-4 py-3 text-right font-semibold tabular-nums text-forest">
                  {item.amount?.trim() ? item.amount : '-'}
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </section>
  )
}
