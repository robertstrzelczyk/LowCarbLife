export type DietType = 'Keto' | 'LowCarb'
export type MealCategory =
  | 'Sniadanie'
  | 'Obiad'
  | 'Kolacja'
  | 'Przekaski'
  | 'Smoothie'
  | 'Desery'
  | 'Salatki'
  | 'Zupy'
  | 'Lunchboxy'

export type MeResponse = {
  email: string
  displayName: string | null
  isAdmin: boolean
}

export type AuthResponse = MeResponse & {
  token: string
}

export type BlogPostListItem = {
  id: string
  title: string
  excerpt: string
  imageUrl: string | null
  createdAt: string
  authorName: string | null
}

export type BlogPostDetail = {
  id: string
  title: string
  content: string
  imageUrl: string | null
  createdAt: string
  updatedAt: string | null
  authorName: string | null
}

export type Ingredient = {
  name: string
  amount: string | null
}

export type RecipeListItem = {
  id: string
  title: string
  description: string
  imageUrl: string | null
  dietType: DietType
  mealCategory: MealCategory
}

export type RecipeDetail = {
  id: string
  title: string
  description: string
  instructions: string
  youtubeUrl: string | null
  imageUrl: string | null
  dietType: DietType
  mealCategory: MealCategory
  ingredients: Ingredient[]
}

export type ContactMessage = {
  id: string
  name: string
  email: string
  message: string
  createdAt: string
}
