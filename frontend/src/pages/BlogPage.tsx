import { useQuery } from '@tanstack/react-query'
import { Link } from 'react-router'
import { api } from '../api/client'
import { useAuth } from '../auth/AuthContext'
import { AdminButton, PageHeader } from '../components/Layout'
import { useLanguage } from '../i18n/LanguageContext'
import { formatDate } from '../lib/format'
import type { BlogPostListItem } from '../types'

export function BlogPage() {
  const { user } = useAuth()
  const { t, language } = useLanguage()
  const posts = useQuery({
    queryKey: ['blog'],
    queryFn: () => api<BlogPostListItem[]>('/api/blog'),
  })

  return (
    <div>
      <PageHeader
        title={t('blogTitle')}
        subtitle={t('blogSubtitle')}
        action={user?.isAdmin ? <AdminButton to="/blog/nowy">{t('blogAdd')}</AdminButton> : null}
      />

      {posts.isLoading ? <p>{t('loading')}</p> : null}
      {posts.isError ? <p className="text-orange-dark">{t('blogLoadError')}</p> : null}

      <div className="grid gap-6 md:grid-cols-2">
        {posts.data?.map((post) => (
          <Link
            key={post.id}
            to={`/blog/${post.id}`}
            className="overflow-hidden rounded-2xl bg-white shadow-sm transition hover:-translate-y-0.5 hover:shadow-md"
          >
            {post.imageUrl ? <img src={post.imageUrl} alt="" className="h-44 w-full object-cover" /> : null}
            <div className="p-5">
              <p className="text-xs uppercase tracking-wide text-sage">{formatDate(post.createdAt, language)}</p>
              <h2 className="mt-1 text-xl font-semibold text-forest">{post.title}</h2>
              <p className="mt-2 text-sm text-forest/70">{post.excerpt}</p>
            </div>
          </Link>
        ))}
      </div>
    </div>
  )
}
