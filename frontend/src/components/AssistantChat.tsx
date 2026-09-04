import { useEffect, useRef, useState } from 'react'
import { HubConnection, HubConnectionBuilder, HubConnectionState } from '@microsoft/signalr'
import { useLanguage } from '../i18n/LanguageContext'

type ChatMessage = {
  id: number
  fromAssistant: boolean
  text: string
}

export function AssistantChat() {
  const { t, language } = useLanguage()
  const [open, setOpen] = useState(false)
  const [input, setInput] = useState('')
  const [status, setStatus] = useState<'connecting' | 'ready' | 'offline'>('connecting')
  const [messages, setMessages] = useState<ChatMessage[]>([])
  const connectionRef = useRef<HubConnection | null>(null)
  const nextId = useRef(1)
  const listRef = useRef<HTMLDivElement>(null)

  useEffect(() => {
    const connection = new HubConnectionBuilder()
      .withUrl('/hubs/assistant')
      .withAutomaticReconnect()
      .build()

    connectionRef.current = connection
    connection.on('ReceiveMessage', (text: string) => {
      setMessages((current) => [...current, { id: nextId.current++, fromAssistant: true, text }])
    })
    connection.onreconnected(() => setStatus('ready'))
    connection.onclose(() => setStatus('offline'))

    connection
      .start()
      .then(() => setStatus('ready'))
      .catch(() => setStatus('offline'))

    return () => {
      connection.off('ReceiveMessage')
      void connection.stop()
      connectionRef.current = null
    }
  }, [])

  useEffect(() => {
    listRef.current?.scrollTo({ top: listRef.current.scrollHeight })
  }, [messages, open])

  async function send() {
    const text = input.trim()
    const connection = connectionRef.current
    if (!text || !connection || connection.state !== HubConnectionState.Connected) {
      return
    }

    setMessages((current) => [...current, { id: nextId.current++, fromAssistant: false, text }])
    setInput('')
    await connection.invoke('SendMessage', text, language)
  }

  return (
    <div className="fixed right-4 bottom-4 z-40 flex flex-col items-end gap-3">
      {open ? (
        <section className="flex h-[28rem] w-[min(22rem,calc(100vw-2rem))] flex-col overflow-hidden rounded-2xl bg-white shadow-xl">
          <header className="flex items-center justify-between bg-forest px-4 py-3 text-cream">
            <h2 className="text-sm font-semibold">{t('assistantTitle')}</h2>
            <button type="button" className="text-xs text-cream/80 hover:text-cream" onClick={() => setOpen(false)}>
              {t('assistantClose')}
            </button>
          </header>
          <div ref={listRef} className="flex-1 space-y-2 overflow-y-auto bg-cream/40 p-3 text-sm">
            <p className="rounded-xl bg-white px-3 py-2 text-forest/80 shadow-sm">{t('assistantWelcome')}</p>
            {messages.map((message) => (
              <p
                key={message.id}
                className={`max-w-[90%] rounded-xl px-3 py-2 shadow-sm ${
                  message.fromAssistant ? 'bg-white text-forest/80' : 'ml-auto bg-forest text-cream'
                }`}
              >
                {message.text}
              </p>
            ))}
            {status !== 'ready' ? (
              <p className="text-xs text-forest/50">{status === 'connecting' ? t('assistantConnecting') : t('assistantOffline')}</p>
            ) : null}
          </div>
          <form
            className="flex gap-2 border-t border-forest/10 p-3"
            onSubmit={(event) => {
              event.preventDefault()
              void send()
            }}
          >
            <input
              className="min-w-0 flex-1 rounded-full border border-forest/20 px-3 py-2 text-sm"
              value={input}
              onChange={(event) => setInput(event.target.value)}
              placeholder={t('assistantPlaceholder')}
              disabled={status !== 'ready'}
            />
            <button
              type="submit"
              className="rounded-full bg-orange px-3 py-2 text-sm font-semibold text-white disabled:opacity-50"
              disabled={status !== 'ready'}
            >
              {t('assistantSend')}
            </button>
          </form>
        </section>
      ) : null}

      <button
        type="button"
        className="h-14 w-14 rounded-full bg-orange text-sm font-bold text-white shadow-lg hover:bg-orange-dark"
        onClick={() => setOpen((value) => !value)}
        aria-label={t('assistantOpen')}
      >
        {open ? '×' : '?'}
      </button>
    </div>
  )
}
