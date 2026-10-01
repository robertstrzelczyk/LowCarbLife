const TOKEN_KEY = 'lcl_token'

export function getToken() {
  return localStorage.getItem(TOKEN_KEY)
}

export function setToken(token: string | null) {
  if (token) {
    localStorage.setItem(TOKEN_KEY, token)
  } else {
    localStorage.removeItem(TOKEN_KEY)
  }
}

export class ApiError extends Error {
  status: number

  constructor(status: number, message: string) {
    super(message)
    this.status = status
  }
}

export async function api<T>(path: string, options: RequestInit = {}): Promise<T> {
  const headers = new Headers(options.headers)
  const isFormData = typeof FormData !== 'undefined' && options.body instanceof FormData
  if (options.body && !isFormData && !headers.has('Content-Type')) {
    headers.set('Content-Type', 'application/json')
  }

  const token = getToken()
  if (token) {
    headers.set('Authorization', `Bearer ${token}`)
  }

  const response = await fetch(path, { ...options, headers })
  if (response.status === 204) {
    return undefined as T
  }

  const text = await response.text()
  if (!response.ok) {
    throw new ApiError(response.status, unwrapError(text) || response.statusText)
  }

  return text ? (JSON.parse(text) as T) : (undefined as T)
}

function unwrapError(text: string) {
  try {
    const parsed = JSON.parse(text) as { title?: string; detail?: string }
    return parsed.detail || parsed.title || text.replaceAll('"', '')
  } catch {
    return text.replaceAll('"', '')
  }
}
