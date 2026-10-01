import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { useEffect, useState } from 'react'
import { useNavigate, useParams } from 'react-router'
import { api } from '../api/client'
import { ImageField } from '../components/ImageField'
import { useLanguage } from '../i18n/LanguageContext'
import type { BlogPostDetail } from '../types'

export function BlogFormPage() {
  const { t } = useLanguage()
  const { id } = useParams()
  const navigate = useNavigate()
  const queryClient = useQueryClient()
  const isEdit = Boolean(id)
  const existing = useQuery({
    queryKey: ['blog', id],
    queryFn: () => api<BlogPostDetail>(`/api/blog/${id}`),
    enabled: isEdit,
  })

  const [title, setTitle] = useState('')
  const [content, setContent] = useState('')
  const [imageUrl, setImageUrl] = useState('')
  const [error, setError] = useState('')

  useEffect(() => {
    if (existing.data) {
      setTitle(existing.data.title)
      setContent(existing.data.content)
      setImageUrl(existing.data.imageUrl ?? '')
    }
  }, [existing.data])

  const save = useMutation({
    mutationFn: () =>
      api<BlogPostDetail>(isEdit ? `/api/blog/${id}` : '/api/blog', {
        method: isEdit ? 'PUT' : 'POST',
        body: JSON.stringify({ title, content, imageUrl: imageUrl || null }),
      }),
    onSuccess: async (post) => {
      await queryClient.invalidateQueries({ queryKey: ['blog'] })
      navigate(`/blog/${post.id}`)
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
      <h1 className="text-2xl font-semibold text-forest">{isEdit ? t('blogEditTitle') : t('blogNew')}</h1>
      {error ? <p className="text-sm text-orange-dark">{error}</p> : null}
      <label className="block text-sm font-medium text-forest">
        {t('fieldTitle')}
        <input className="mt-1 w-full rounded-lg border border-forest/20 px-3 py-2" value={title} onChange={(e) => setTitle(e.target.value)} required />
      </label>
      <label className="block text-sm font-medium text-forest">
        {t('fieldContent')}
        <textarea className="mt-1 min-h-48 w-full rounded-lg border border-forest/20 px-3 py-2" value={content} onChange={(e) => setContent(e.target.value)} required />
      </label>
      <ImageField value={imageUrl} onChange={setImageUrl} />
      <button type="submit" className="rounded-full bg-forest px-5 py-2 font-semibold text-cream" disabled={save.isPending}>
        {t('save')}
      </button>
    </form>
  )
}
