import { Link } from 'react-router'
import { useAuth } from '../auth/AuthContext'
import { useLanguage } from '../i18n/LanguageContext'

export function HomePage() {
  const { user } = useAuth()
  const { t } = useLanguage()

  return (
    <div className="space-y-10">
      <section className="overflow-hidden rounded-3xl bg-forest px-8 py-14 text-cream shadow-sm">
        <p className="text-sm uppercase tracking-[0.2em] text-orange">{t('homeEyebrow')}</p>
        <h1 className="mt-3 max-w-2xl text-4xl font-semibold leading-tight md:text-5xl">{t('homeTitle')}</h1>
        <p className="mt-4 max-w-xl text-cream/80">{t('homeLead')}</p>
        <div className="mt-8 flex flex-wrap gap-3">
          <Link to="/blog" className="rounded-full bg-orange px-5 py-2.5 font-semibold text-white hover:bg-orange-dark">
            {t('homeReadBlog')}
          </Link>
          <Link to="/przepisy/keto" className="rounded-full bg-white/10 px-5 py-2.5 font-semibold text-cream hover:bg-white/20">
            {t('homeSeeRecipes')}
          </Link>
        </div>
        {user?.isAdmin ? <p className="mt-6 text-sm text-cream/70">{t('homeAdminHint')}</p> : null}
      </section>

      <section className="grid gap-6 md:grid-cols-2">
        <article className="rounded-2xl bg-white p-6 shadow-sm">
          <h2 className="text-xl font-semibold text-forest">{t('homeBlogCardTitle')}</h2>
          <p className="mt-2 text-forest/70">{t('homeBlogCardText')}</p>
        </article>
        <article className="rounded-2xl bg-white p-6 shadow-sm">
          <h2 className="text-xl font-semibold text-forest">{t('homeRecipesCardTitle')}</h2>
          <p className="mt-2 text-forest/70">{t('homeRecipesCardText')}</p>
        </article>
      </section>
    </div>
  )
}
