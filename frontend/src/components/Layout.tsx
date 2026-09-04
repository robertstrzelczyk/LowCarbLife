import type { ReactNode } from 'react'
import { NavLink, Outlet } from 'react-router'
import { AssistantChat } from './AssistantChat'
import { Footer } from './Footer'
import { Navbar } from './Navbar'

export function Layout() {
  return (
    <div className="flex min-h-svh flex-col">
      <Navbar />
      <main className="mx-auto w-full max-w-6xl flex-1 px-4 py-8">
        <Outlet />
      </main>
      <Footer />
      <AssistantChat />
    </div>
  )
}

export function PageHeader({
  title,
  subtitle,
  action,
}: {
  title: string
  subtitle?: string
  action?: ReactNode
}) {
  return (
    <div className="mb-8 flex flex-wrap items-end justify-between gap-4">
      <div>
        <h1 className="text-3xl font-semibold text-forest">{title}</h1>
        {subtitle ? <p className="mt-2 max-w-2xl text-forest/70">{subtitle}</p> : null}
      </div>
      {action}
    </div>
  )
}

export function AdminButton({ to, children }: { to: string; children: ReactNode }) {
  return (
    <NavLink
      to={to}
      className="rounded-full bg-orange px-4 py-2 text-sm font-semibold text-white hover:bg-orange-dark"
    >
      {children}
    </NavLink>
  )
}
