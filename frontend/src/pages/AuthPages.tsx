import { useState } from 'react'
import { Link, useNavigate } from 'react-router'
import { useAuth } from '../auth/AuthContext'
import { useLanguage } from '../i18n/LanguageContext'

export function LoginPage() {
  const { login } = useAuth()
  const { t } = useLanguage()
  const navigate = useNavigate()
  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')
  const [error, setError] = useState('')

  return (
    <form
      className="mx-auto max-w-md space-y-4 rounded-2xl bg-white p-6 shadow-sm"
      onSubmit={async (event) => {
        event.preventDefault()
        setError('')
        try {
          await login(email, password)
          navigate('/')
        } catch (err) {
          setError(err instanceof Error ? err.message : t('loginFailed'))
        }
      }}
    >
      <h1 className="text-2xl font-semibold text-forest">{t('loginTitle')}</h1>
      {error ? <p className="text-sm text-orange-dark">{error}</p> : null}
      <label className="block text-sm font-medium text-forest">
        {t('fieldEmail')}
        <input type="email" className="mt-1 w-full rounded-lg border border-forest/20 px-3 py-2" value={email} onChange={(e) => setEmail(e.target.value)} required />
      </label>
      <label className="block text-sm font-medium text-forest">
        {t('fieldPassword')}
        <input type="password" className="mt-1 w-full rounded-lg border border-forest/20 px-3 py-2" value={password} onChange={(e) => setPassword(e.target.value)} required />
      </label>
      <button type="submit" className="w-full rounded-full bg-forest py-2 font-semibold text-cream">
        {t('loginSubmit')}
      </button>
      <p className="text-center text-sm text-forest/70">
        {t('loginNoAccount')} <Link to="/rejestracja" className="text-orange">{t('loginRegisterLink')}</Link>
      </p>
    </form>
  )
}

export function RegisterPage() {
  const { register } = useAuth()
  const { t } = useLanguage()
  const navigate = useNavigate()
  const [displayName, setDisplayName] = useState('')
  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')
  const [error, setError] = useState('')

  return (
    <form
      className="mx-auto max-w-md space-y-4 rounded-2xl bg-white p-6 shadow-sm"
      onSubmit={async (event) => {
        event.preventDefault()
        setError('')
        try {
          await register(email, password, displayName)
          navigate('/')
        } catch (err) {
          setError(err instanceof Error ? err.message : t('registerFailed'))
        }
      }}
    >
      <h1 className="text-2xl font-semibold text-forest">{t('registerTitle')}</h1>
      {error ? <p className="text-sm text-orange-dark">{error}</p> : null}
      <label className="block text-sm font-medium text-forest">
        {t('fieldNick')}
        <input className="mt-1 w-full rounded-lg border border-forest/20 px-3 py-2" value={displayName} onChange={(e) => setDisplayName(e.target.value)} />
      </label>
      <label className="block text-sm font-medium text-forest">
        {t('fieldEmail')}
        <input type="email" className="mt-1 w-full rounded-lg border border-forest/20 px-3 py-2" value={email} onChange={(e) => setEmail(e.target.value)} required />
      </label>
      <label className="block text-sm font-medium text-forest">
        {t('fieldPasswordHint')}
        <input type="password" className="mt-1 w-full rounded-lg border border-forest/20 px-3 py-2" value={password} onChange={(e) => setPassword(e.target.value)} required minLength={8} />
      </label>
      <button type="submit" className="w-full rounded-full bg-forest py-2 font-semibold text-cream">
        {t('registerSubmit')}
      </button>
    </form>
  )
}
