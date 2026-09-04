export function youtubeEmbedUrl(url: string | null | undefined) {
  if (!url) {
    return null
  }

  try {
    const parsed = new URL(url)
    if (parsed.hostname.includes('youtu.be')) {
      const id = parsed.pathname.replace('/', '')
      return id ? `https://www.youtube.com/embed/${id}` : null
    }

    const videoId = parsed.searchParams.get('v')
    if (videoId) {
      return `https://www.youtube.com/embed/${videoId}`
    }

    if (parsed.pathname.startsWith('/embed/')) {
      return url
    }
  } catch {
    return null
  }

  return null
}

export function formatDate(value: string, language: 'pl' | 'en' = 'pl') {
  return new Date(value).toLocaleDateString(language === 'en' ? 'en-GB' : 'pl-PL', {
    day: 'numeric',
    month: 'long',
    year: 'numeric',
  })
}
