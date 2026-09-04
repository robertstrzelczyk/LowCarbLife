import type { ReactNode } from 'react'
import { Navigate, Route, Routes } from 'react-router'
import { useAuth } from './auth/AuthContext'
import { Layout } from './components/Layout'
import { useLanguage } from './i18n/LanguageContext'
import { LoginPage, RegisterPage } from './pages/AuthPages'
import { BlogDetailPage } from './pages/BlogDetailPage'
import { BlogFormPage } from './pages/BlogFormPage'
import { BlogPage } from './pages/BlogPage'
import { ContactPage } from './pages/ContactPage'
import { HomePage } from './pages/HomePage'
import { RecipeDetailPage } from './pages/RecipeDetailPage'
import { RecipeFormPage } from './pages/RecipeFormPage'
import { RecipesPage } from './pages/RecipesPage'
import { TestPage } from './pages/TestPage'

function AdminRoute({ children }: { children: ReactNode }) {
  const { user, ready } = useAuth()
  const { t } = useLanguage()
  if (!ready) {
    return <p>{t('loading')}</p>
  }
  if (!user?.isAdmin) {
    return <Navigate to="/logowanie" replace />
  }
  return children
}

export default function App() {
  return (
    <Routes>
      <Route element={<Layout />}>
        <Route path="/" element={<HomePage />} />
        <Route path="/blog" element={<BlogPage />} />
        <Route
          path="/blog/nowy"
          element={
            <AdminRoute>
              <BlogFormPage />
            </AdminRoute>
          }
        />
        <Route path="/blog/:id" element={<BlogDetailPage />} />
        <Route
          path="/blog/:id/edytuj"
          element={
            <AdminRoute>
              <BlogFormPage />
            </AdminRoute>
          }
        />
        <Route path="/przepisy/keto" element={<RecipesPage />} />
        <Route path="/przepisy/lowcarb" element={<RecipesPage />} />
        <Route
          path="/przepisy/nowy"
          element={
            <AdminRoute>
              <RecipeFormPage />
            </AdminRoute>
          }
        />
        <Route path="/przepisy/:id" element={<RecipeDetailPage />} />
        <Route
          path="/przepisy/:id/edytuj"
          element={
            <AdminRoute>
              <RecipeFormPage />
            </AdminRoute>
          }
        />
        <Route path="/test-1" element={<TestPage number={1} />} />
        <Route path="/test-2" element={<TestPage number={2} />} />
        <Route path="/test-3" element={<TestPage number={3} />} />
        <Route path="/kontakt" element={<ContactPage />} />
        <Route path="/logowanie" element={<LoginPage />} />
        <Route path="/rejestracja" element={<RegisterPage />} />
      </Route>
    </Routes>
  )
}
