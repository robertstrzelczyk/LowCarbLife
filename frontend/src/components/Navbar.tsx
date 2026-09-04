import { useState } from 'react'
import { Link, NavLink } from 'react-router'
import { useAuth } from '../auth/AuthContext'
import { useLanguage } from '../i18n/LanguageContext'
import type { Language } from '../i18n/translations'

const linkClass = ({ isActive }: { isActive: boolean }) =>
  `rounded-full px-3 py-2 text-sm font-medium transition ${
    isActive ? 'bg-forest text-cream' : 'text-forest hover:bg-forest/10'
  }`

export function Navbar() {
  const { user, logout } = useAuth()
  const { t, language, setLanguage } = useLanguage()
  const [open, setOpen] = useState(false)
  const [recipesOpen, setRecipesOpen] = useState(false)

  function switchLanguage(next: Language) {
    setLanguage(next)
    setOpen(false)
  }

  return (
    <header className="sticky top-0 z-20 border-b border-forest/10 bg-cream/95 backdrop-blur">
      <div className="mx-auto flex max-w-6xl items-center justify-between gap-4 px-4 py-3">
        <Link to="/" className="flex items-center gap-3" onClick={() => setOpen(false)}>
          <img src="/logo.png" alt="LowCarbLife" className="h-12 w-12 rounded-full bg-white object-contain p-0.5 shadow-sm" />
          <span className="text-lg font-semibold tracking-tight text-forest">LowCarbLife</span>
        </Link>

        <button
          type="button"
          className="rounded-md px-3 py-2 text-forest md:hidden"
          onClick={() => setOpen((value) => !value)}
        >
          {t('navMenu')}
        </button>

        <nav className={`${open ? 'flex' : 'hidden'} absolute left-0 right-0 top-full flex-col gap-1 border-b border-forest/10 bg-cream px-4 py-3 md:static md:flex md:flex-row md:items-center md:gap-1 md:border-0 md:bg-transparent md:p-0`}>
          <NavLink to="/" end className={linkClass} onClick={() => setOpen(false)}>
            {t('navStart')}
          </NavLink>
          <NavLink to="/blog" className={linkClass} onClick={() => setOpen(false)}>
            {t('navBlog')}
          </NavLink>

          <div className="relative" onMouseEnter={() => setRecipesOpen(true)} onMouseLeave={() => setRecipesOpen(false)}>
            <button
              type="button"
              className="rounded-full px-3 py-2 text-sm font-medium text-forest hover:bg-forest/10"
              onClick={() => setRecipesOpen((value) => !value)}
            >
              {t('navRecipes')}
            </button>
            {recipesOpen ? (
              <div className="relative z-30 md:absolute md:left-0 md:top-full md:min-w-48 md:rounded-xl md:border md:border-forest/10 md:bg-white md:p-2 md:shadow-lg">
                <NavLink
                  to="/przepisy/keto"
                  className="block rounded-lg px-3 py-2 text-sm text-forest hover:bg-cream"
                  onClick={() => {
                    setRecipesOpen(false)
                    setOpen(false)
                  }}
                >
                  {t('navKetoRecipes')}
                </NavLink>
                <NavLink
                  to="/przepisy/lowcarb"
                  className="block rounded-lg px-3 py-2 text-sm text-forest hover:bg-cream"
                  onClick={() => {
                    setRecipesOpen(false)
                    setOpen(false)
                  }}
                >
                  {t('navLowcarbRecipes')}
                </NavLink>
              </div>
            ) : null}
          </div>

          <NavLink to="/test-1" className={linkClass} onClick={() => setOpen(false)}>
            {t('navTest1')}
          </NavLink>
          <NavLink to="/test-2" className={linkClass} onClick={() => setOpen(false)}>
            {t('navTest2')}
          </NavLink>
          <NavLink to="/test-3" className={linkClass} onClick={() => setOpen(false)}>
            {t('navTest3')}
          </NavLink>
          <NavLink to="/kontakt" className={linkClass} onClick={() => setOpen(false)}>
            {t('navContact')}
          </NavLink>

          <div className="md:ml-3 md:flex md:items-center md:gap-2">
            {user ? (
              <>
                <span className="px-2 text-xs text-forest/70">
                  {user.displayName || user.email}
                  {user.isAdmin ? ` · ${t('navAdmin')}` : ''}
                </span>
                <button
                  type="button"
                  className="rounded-full px-3 py-2 text-sm font-medium text-forest hover:bg-forest/10"
                  onClick={() => {
                    logout()
                    setOpen(false)
                  }}
                >
                  {t('navLogout')}
                </button>
              </>
            ) : (
              <>
                <NavLink to="/logowanie" className={linkClass} onClick={() => setOpen(false)}>
                  {t('navLogin')}
                </NavLink>
                <NavLink to="/rejestracja" className={linkClass} onClick={() => setOpen(false)}>
                  {t('navRegister')}
                </NavLink>
              </>
            )}

            <div className="flex items-center rounded-full border border-forest/15 p-0.5" aria-label={t('langSwitch')}>
              <button
                type="button"
                className={`rounded-full px-2.5 py-1 text-xs font-semibold ${language === 'pl' ? 'bg-forest text-cream' : 'text-forest'}`}
                onClick={() => switchLanguage('pl')}
              >
                {t('langPl')}
              </button>
              <button
                type="button"
                className={`rounded-full px-2.5 py-1 text-xs font-semibold ${language === 'en' ? 'bg-forest text-cream' : 'text-forest'}`}
                onClick={() => switchLanguage('en')}
              >
                {t('langEn')}
              </button>
            </div>
          </div>
        </nav>
      </div>
    </header>
  )
}
