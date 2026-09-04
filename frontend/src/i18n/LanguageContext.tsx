import { createContext, useContext, useEffect, useMemo, useState, type ReactNode } from 'react'
import { translations, type Language, type TranslationKey } from './translations'

const STORAGE_KEY = 'lcl_lang'

type LanguageContextValue = {
  language: Language
  setLanguage: (language: Language) => void
  t: (key: TranslationKey, vars?: Record<string, string | number>) => string
}

const LanguageContext = createContext<LanguageContextValue | null>(null)

function detectLanguage(): Language {
  const stored = localStorage.getItem(STORAGE_KEY)
  if (stored === 'pl' || stored === 'en') {
    return stored
  }
  return navigator.language.toLowerCase().startsWith('en') ? 'en' : 'pl'
}

export function LanguageProvider({ children }: { children: ReactNode }) {
  const [language, setLanguageState] = useState<Language>(detectLanguage)

  useEffect(() => {
    document.documentElement.lang = language
    localStorage.setItem(STORAGE_KEY, language)
  }, [language])

  const value = useMemo<LanguageContextValue>(
    () => ({
      language,
      setLanguage: (next) => setLanguageState(next),
        t: (key, vars) => {
          let text: string = translations[language][key]
          if (vars) {
            for (const [name, replacement] of Object.entries(vars)) {
              text = text.replaceAll(`{${name}}`, String(replacement))
            }
          }
          return text
        },
    }),
    [language],
  )

  return <LanguageContext.Provider value={value}>{children}</LanguageContext.Provider>
}

export function useLanguage() {
  const context = useContext(LanguageContext)
  if (!context) {
    throw new Error('useLanguage must be used inside LanguageProvider')
  }
  return context
}
