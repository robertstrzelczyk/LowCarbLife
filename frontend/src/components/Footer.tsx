import { Link } from 'react-router'
import { useLanguage } from '../i18n/LanguageContext'

const PHONE = '+48 500 123 456'
const EMAIL = 'kontakt@lowcarblife.pl'
const WHATSAPP = 'https://wa.me/48500123456'
const INSTAGRAM = 'https://instagram.com/lowcarblife'

export function Footer() {
  const { t } = useLanguage()
  const year = new Date().getFullYear()

  return (
    <footer className="border-t border-forest/10 bg-forest text-cream">
      <div className="mx-auto grid max-w-6xl gap-10 px-4 py-10 md:grid-cols-2">
        <section>
          <h2 className="text-lg font-semibold">{t('footerContactTitle')}</h2>
          <ul className="mt-4 space-y-2 text-sm text-cream/85">
            <li>
              {t('footerPhone')}:{' '}
              <a className="hover:text-orange" href={`tel:${PHONE.replaceAll(' ', '')}`}>
                {PHONE}
              </a>
            </li>
            <li>
              {t('footerEmail')}:{' '}
              <a className="hover:text-orange" href={`mailto:${EMAIL}`}>
                {EMAIL}
              </a>
            </li>
            <li>
              {t('footerWhatsapp')}:{' '}
              <a className="hover:text-orange" href={WHATSAPP} target="_blank" rel="noreferrer">
                {PHONE}
              </a>
            </li>
            <li>
              {t('footerInstagram')}:{' '}
              <a className="hover:text-orange" href={INSTAGRAM} target="_blank" rel="noreferrer">
                @lowcarblife
              </a>
            </li>
          </ul>
        </section>

        <section>
          <h2 className="text-lg font-semibold">{t('footerLinksTitle')}</h2>
          <ul className="mt-4 grid gap-2 text-sm text-cream/85 sm:grid-cols-2">
            <li>
              <Link className="hover:text-orange" to="/">
                {t('navStart')}
              </Link>
            </li>
            <li>
              <Link className="hover:text-orange" to="/blog">
                {t('navBlog')}
              </Link>
            </li>
            <li>
              <Link className="hover:text-orange" to="/przepisy/keto">
                {t('navKetoRecipes')}
              </Link>
            </li>
            <li>
              <Link className="hover:text-orange" to="/przepisy/lowcarb">
                {t('navLowcarbRecipes')}
              </Link>
            </li>
            <li>
              <Link className="hover:text-orange" to="/kontakt">
                {t('navContact')}
              </Link>
            </li>
            <li>
              <Link className="hover:text-orange" to="/logowanie">
                {t('navLogin')}
              </Link>
            </li>
          </ul>
        </section>
      </div>
      <p className="border-t border-white/10 py-4 text-center text-xs text-cream/60">{t('footerCopy', { year })}</p>
    </footer>
  )
}
