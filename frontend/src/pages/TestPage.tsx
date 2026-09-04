import { PageHeader } from '../components/Layout'
import { useLanguage } from '../i18n/LanguageContext'

export function TestPage({ number }: { number: 1 | 2 | 3 }) {
  const { t } = useLanguage()

  return (
    <div>
      <PageHeader title={t('testTitle', { n: number })} subtitle={t('testSubtitle')} />
      <div className="rounded-2xl bg-white p-8 text-forest/70 shadow-sm">{t('testBody')}</div>
    </div>
  )
}
