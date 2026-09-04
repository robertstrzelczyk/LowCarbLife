import { useMutation, useQuery } from '@tanstack/react-query'
import { useState } from 'react'
import { api } from '../api/client'
import { useAuth } from '../auth/AuthContext'
import { PageHeader } from '../components/Layout'
import { useLanguage } from '../i18n/LanguageContext'
import { formatDate } from '../lib/format'
import type { ContactMessage } from '../types'

export function ContactPage() {
  const { user } = useAuth()
  const { t, language } = useLanguage()
  const [name, setName] = useState(user?.displayName ?? '')
  const [email, setEmail] = useState(user?.email ?? '')
  const [message, setMessage] = useState('')
  const [sent, setSent] = useState(false)
  const [error, setError] = useState('')

  const inbox = useQuery({
    queryKey: ['contact'],
    queryFn: () => api<ContactMessage[]>('/api/contact'),
    enabled: Boolean(user?.isAdmin),
  })

  const send = useMutation({
    mutationFn: () =>
      api('/api/contact', {
        method: 'POST',
        body: JSON.stringify({ name, email, message }),
      }),
    onSuccess: () => {
      setSent(true)
      setMessage('')
      setError('')
    },
    onError: (err: Error) => setError(err.message),
  })

  return (
    <div className="grid gap-8 lg:grid-cols-2">
      <div>
        <PageHeader title={t('contactTitle')} subtitle={t('contactSubtitle')} />
        {sent ? <p className="mb-4 rounded-xl bg-sage/20 px-4 py-3 text-forest">{t('contactSent')}</p> : null}
        {error ? <p className="mb-4 text-sm text-orange-dark">{error}</p> : null}
        <form
          className="space-y-4 rounded-2xl bg-white p-6 shadow-sm"
          onSubmit={(event) => {
            event.preventDefault()
            send.mutate()
          }}
        >
          <label className="block text-sm font-medium text-forest">
            {t('fieldName')}
            <input className="mt-1 w-full rounded-lg border border-forest/20 px-3 py-2" value={name} onChange={(e) => setName(e.target.value)} required />
          </label>
          <label className="block text-sm font-medium text-forest">
            {t('fieldEmail')}
            <input type="email" className="mt-1 w-full rounded-lg border border-forest/20 px-3 py-2" value={email} onChange={(e) => setEmail(e.target.value)} required />
          </label>
          <label className="block text-sm font-medium text-forest">
            {t('fieldMessage')}
            <textarea className="mt-1 min-h-32 w-full rounded-lg border border-forest/20 px-3 py-2" value={message} onChange={(e) => setMessage(e.target.value)} required />
          </label>
          <button type="submit" className="rounded-full bg-forest px-5 py-2 font-semibold text-cream" disabled={send.isPending}>
            {t('contactSend')}
          </button>
        </form>
      </div>

      {user?.isAdmin ? (
        <section>
          <h2 className="mb-4 text-xl font-semibold text-forest">{t('contactInbox')}</h2>
          <div className="space-y-3">
            {inbox.data?.map((item) => (
              <article key={item.id} className="rounded-2xl bg-white p-4 shadow-sm">
                <p className="text-sm font-semibold text-forest">
                  {item.name} · {item.email}
                </p>
                <p className="text-xs text-forest/50">{formatDate(item.createdAt, language)}</p>
                <p className="mt-2 whitespace-pre-wrap text-sm text-forest/80">{item.message}</p>
              </article>
            ))}
            {inbox.data?.length === 0 ? <p className="text-forest/70">{t('contactEmpty')}</p> : null}
          </div>
        </section>
      ) : (
        <section className="rounded-2xl bg-white p-6 shadow-sm">
          <h2 className="text-xl font-semibold text-forest">LowCarbLife</h2>
          <p className="mt-2 text-forest/70">{t('contactAside')}</p>
        </section>
      )}
    </div>
  )
}
