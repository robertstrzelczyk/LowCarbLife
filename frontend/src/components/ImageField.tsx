import { useRef, useState } from 'react'
import { api } from '../api/client'
import { useLanguage } from '../i18n/LanguageContext'

type Props = {
  value: string
  onChange: (url: string) => void
}

export function ImageField({ value, onChange }: Props) {
  const { t } = useLanguage()
  const inputRef = useRef<HTMLInputElement>(null)
  const [uploading, setUploading] = useState(false)
  const [error, setError] = useState('')

  async function upload(file: File) {
    setError('')
    setUploading(true)
    try {
      const data = new FormData()
      data.append('file', file)
      const result = await api<{ url: string }>('/api/uploads/images', {
        method: 'POST',
        body: data,
      })
      onChange(result.url)
    } catch (err) {
      setError(err instanceof Error ? err.message : t('fieldImageUploadFailed'))
    } finally {
      setUploading(false)
      if (inputRef.current) {
        inputRef.current.value = ''
      }
    }
  }

  return (
    <fieldset className="space-y-3">
      <legend className="text-sm font-medium text-forest">{t('fieldImageUrl')}</legend>
      <p className="text-xs text-forest/60">{t('fieldImageHint')}</p>
      <input
        className="w-full rounded-lg border border-forest/20 px-3 py-2"
        value={value}
        onChange={(e) => onChange(e.target.value)}
        placeholder="https://…"
      />
      <div className="flex flex-wrap items-center gap-3">
        <span className="text-xs uppercase tracking-wide text-forest/40">{t('fieldImageOr')}</span>
        <input
          ref={inputRef}
          type="file"
          accept="image/jpeg,image/png,image/webp,image/gif,.jpg,.jpeg,.png,.webp,.gif"
          className="sr-only"
          onChange={(event) => {
            const file = event.target.files?.[0]
            if (file) {
              void upload(file)
            }
          }}
        />
        <button
          type="button"
          className="rounded-full bg-forest/10 px-4 py-2 text-sm font-semibold text-forest hover:bg-forest/15 disabled:opacity-60"
          disabled={uploading}
          onClick={() => inputRef.current?.click()}
        >
          {uploading ? t('fieldImageUploading') : t('fieldImageUpload')}
        </button>
        {value ? (
          <button type="button" className="text-sm text-orange" onClick={() => onChange('')}>
            {t('fieldImageClear')}
          </button>
        ) : null}
      </div>
      {error ? <p className="text-sm text-orange-dark">{error}</p> : null}
      {value ? (
        <img src={value} alt={t('fieldImagePreview')} className="max-h-56 w-full rounded-xl object-cover" />
      ) : null}
    </fieldset>
  )
}
