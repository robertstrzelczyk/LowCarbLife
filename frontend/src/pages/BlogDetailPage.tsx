import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { Link, useNavigate, useParams } from 'react-router'
import { api } from '../api/client'
import { useAuth } from '../auth/AuthContext'
import { useLanguage } from '../i18n/LanguageContext'
import { formatDate } from '../lib/format'
import type { BlogPostDetail } from '../types'

export function BlogDetailPage() {
  const { id } = useParams()
  const { user } = useAuth()
  const { t, language } = useLanguage()
  const navigate = useNavigate()
  const queryClient = useQueryClient()
  const post = useQuery({
    queryKey: ['blog', id],
    queryFn: () => api<BlogPostDetail>(`/api/blog/${id}`),
    enabled: Boolean(id),
  })

  const remove = useMutation({
    mutationFn: () => api(`/api/blog/${id}`, { method: 'DELETE' }),
    onSuccess: async () => {
      await queryClient.invalidateQueries({ queryKey: ['blog'] })
      navigate('/blog')
    },
  })

  if (post.isLoading) {
    return <p>{t('loading')}</p>
  }
  if (!post.data) {
    return <p>{t('blogNotFound')}</p>
  }

  return (
    <article className="mx-auto max-w-3xl">
      <Link to="/blog" className="text-sm text-sage hover:text-forest">
        {t('blogBack')}
      </Link>
      <h1 className="mt-4 text-4xl font-semibold text-forest">{post.data.title}</h1>
      <p className="mt-2 text-sm text-forest/60">
        {formatDate(post.data.createdAt, language)}
        {post.data.authorName ? ` · ${post.data.authorName}` : ''}
      </p>
      {user?.isAdmin ? (
        <div className="mt-4 flex gap-3">
          <Link to={`/blog/${post.data.id}/edytuj`} className="rounded-full bg-forest px-4 py-2 text-sm font-semibold text-cream">
            {t('blogEdit')}
          </Link>
          <button
            type="button"
            className="rounded-full bg-orange px-4 py-2 text-sm font-semibold text-white"
            onClick={() => {
              if (confirm(t('blogDeleteConfirm'))) {
                remove.mutate()
              }
            }}
          >
            {t('blogDelete')}
          </button>
        </div>
      ) : null}
      {post.data.imageUrl ? <img src={post.data.imageUrl} alt="" className="mt-6 w-full rounded-2xl object-cover" /> : null}
      <div className="mt-6 whitespace-pre-wrap leading-7 text-forest/90">{post.data.content}</div>
    </article>
  )
}
