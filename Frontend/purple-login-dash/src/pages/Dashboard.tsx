import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Badge } from "@/components/ui/badge";
import { BookOpen, Search, Plus, TrendingUp, Users, Star, Filter } from "lucide-react";
import { useState } from "react";
import { BooksApi, Book } from "@/lib/api";
import { useQuery } from "@tanstack/react-query";
import { CategoriesApi } from "@/lib/CategoriesApi";
import { useNavigate } from "react-router-dom";

// Mock data
const categories = [
  { id: 1, name: "Fiction", count: 45, color: "bg-purple-100 text-purple-700" },
  { id: 2, name: "Non-Fiction", count: 32, color: "bg-blue-100 text-blue-700" },
  { id: 3, name: "Science", count: 28, color: "bg-green-100 text-green-700" },
  { id: 4, name: "Biography", count: 15, color: "bg-orange-100 text-orange-700" },
  { id: 5, name: "Technology", count: 23, color: "bg-pink-100 text-pink-700" },
];

const recentBooks = [
  {
    id: 1,
    title: "The Psychology of Money",
    author: "Morgan Housel",
    category: "Non-Fiction",
    rating: 4.8,
    progress: 65,
    cover: "📚"
  },
  {
    id: 2,
    title: "Atomic Habits",
    author: "James Clear",
    category: "Self-Help",
    rating: 4.9,
    progress: 80,
    cover: "⚡"
  },
  {
    id: 3,
    title: "Dune",
    author: "Frank Herbert",
    category: "Science Fiction",
    rating: 4.7,
    progress: 45,
    cover: "🚀"
  },
  {
    id: 4,
    title: "Clean Code",
    author: "Robert C. Martin",
    category: "Technology",
    rating: 4.6,
    progress: 90,
    cover: "💻"
  },
];

const Dashboard = () => {
  const navigate = useNavigate();
  const [searchTerm, setSearchTerm] = useState("");
  const { data: books, isLoading, isError } = useQuery<Book[]>({
    queryKey: ["books"],
    queryFn: () => BooksApi.getAll(1, 20), 
  });

const { data: bookCount, isLoading: isCountLoading, isError: isCountError } =
  useQuery<{ totalCount: number }>({
    queryKey: ["books-count"],
    queryFn: () => BooksApi.getCount(),
  });

  const { data: categoriesCount } = useQuery({
  queryKey: ["categoriesCount"],
  queryFn: () => CategoriesApi.getCount(),
});


  return (
    <div className="min-h-screen bg-gradient-subtle">
      <header className="bg-card shadow-card border-b">
        <div className="max-w-7xl mx-auto px-6 py-4">
          <div className="flex items-center justify-between">
            <div className="flex items-center space-x-3">
              <div className="p-2 bg-gradient-primary rounded-lg">
                <BookOpen className="h-6 w-6 text-white" />
              </div>
              <h1 className="text-2xl font-bold text-primary">BookVault</h1>
            </div>
            <div className="flex items-center space-x-4">
              <div className="relative">
                <Search className="absolute left-3 top-3 h-4 w-4 text-muted-foreground" />
                <Input
                  type="text"
                  placeholder="Search books..."
                  value={searchTerm}
                  onChange={(e) => setSearchTerm(e.target.value)}
                  className="pl-10 w-64"
                />
              </div>
              <Button
                className="bg-gradient-primary hover:scale-105 transition-bounce"
                onClick={() => navigate("/books/new")}
              >
                <Plus className="h-4 w-4 mr-2" />
                Add Book
              </Button>
            </div>
          </div>
        </div>
      </header>

      {/* Main Content */}
      <main className="max-w-7xl mx-auto px-6 py-8">
        {/* Welcome Section */}
        <div className="mb-8">
          <h2 className="text-3xl font-bold text-foreground mb-2">Welcome back!</h2>
          <p className="text-muted-foreground">Here's what's happening with your library today.</p>
        </div>

        {/* Stats Cards */}
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 mb-8">
          <Card className="bg-gradient-card shadow-card border-0 hover:shadow-primary transition-smooth">
  <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
    <CardTitle className="text-sm font-medium text-muted-foreground">
      Total Books
    </CardTitle>
    <BookOpen className="h-4 w-4 text-primary" />
  </CardHeader>
  <CardContent>
    {isCountLoading && (
      <div className="text-muted-foreground text-sm">Loading...</div>
    )}
    {isCountError && (
      <div className="text-red-500 text-sm">Failed to load</div>
    )}
    {bookCount && (
      <>
        <div className="text-2xl font-bold text-primary">
          {bookCount.totalCount}
        </div>
        <p className="text-xs text-muted-foreground">
          +12 from last month
        </p>
      </>
    )}
  </CardContent>
</Card>


          <Card className="bg-gradient-card shadow-card border-0 hover:shadow-primary transition-smooth">
            <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
              <CardTitle className="text-sm font-medium text-muted-foreground">Reading Progress</CardTitle>
              <TrendingUp className="h-4 w-4 text-primary" />
            </CardHeader>
            <CardContent>
              <div className="text-2xl font-bold text-primary">8</div>
              <p className="text-xs text-muted-foreground">Books in progress</p>
            </CardContent>
          </Card>

          <Card className="bg-gradient-card shadow-card border-0 hover:shadow-primary transition-smooth">
            <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
              <CardTitle className="text-sm font-medium text-muted-foreground">Categories</CardTitle>
              <Filter className="h-4 w-4 text-primary" />
            </CardHeader>
            <CardContent>
  <div className="text-2xl font-bold text-primary">
    {isLoading ? "…" : categoriesCount?.totalCount ?? 0}
  </div>
  <p className="text-xs text-muted-foreground">Different genres</p>
</CardContent>
          </Card>

          <Card className="bg-gradient-card shadow-card border-0 hover:shadow-primary transition-smooth">
            <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
              <CardTitle className="text-sm font-medium text-muted-foreground">Avg Rating</CardTitle>
              <Star className="h-4 w-4 text-primary" />
            </CardHeader>
            <CardContent>
              <div className="text-2xl font-bold text-primary">4.7</div>
              <p className="text-xs text-muted-foreground">Based on your reviews</p>
            </CardContent>
          </Card>
        </div>

        <div className="grid grid-cols-1 lg:grid-cols-3 gap-8">
          {/* Categories Section */}
          <div className="lg:col-span-1">
            <Card className="bg-gradient-card shadow-card border-0">
              <CardHeader>
                <CardTitle className="text-primary">Categories</CardTitle>
                <CardDescription>Browse books by category</CardDescription>
              </CardHeader>
              <CardContent className="space-y-3">
                {categories.map((category) => (
                  <div key={category.id} className="flex items-center justify-between p-3 rounded-lg hover:bg-accent transition-smooth cursor-pointer">
                    <span className="font-medium text-foreground">{category.name}</span>
                    <Badge variant="secondary" className={category.color}>
                      {category.count}
                    </Badge>
                  </div>
                ))}
              </CardContent>
            </Card>
          </div>

          {/* Recent Books Section */}
          <div className="lg:col-span-2">
            <Card className="bg-gradient-card shadow-card border-0">
              <CardHeader>
                <CardTitle className="text-primary">Recent Books</CardTitle>
                <CardDescription>Your latest reading activity</CardDescription>
              </CardHeader>
              {/* <CardContent>
                <div className="space-y-4">
                  {recentBooks.map((book) => (
                    <div key={book.id} className="flex items-center space-x-4 p-4 rounded-lg hover:bg-accent transition-smooth cursor-pointer">
                      <div className="text-3xl">{book.cover}</div>
                      <div className="flex-1 min-w-0">
                        <h3 className="font-semibold text-foreground truncate">{book.title}</h3>
                        <p className="text-sm text-muted-foreground">by {book.author}</p>
                        <div className="flex items-center space-x-2 mt-1">
                          <Badge variant="outline" className="text-xs">
                            {book.category}
                          </Badge>
                          <div className="flex items-center space-x-1">
                            <Star className="h-3 w-3 fill-current text-yellow-500" />
                            <span className="text-xs text-muted-foreground">{book.rating}</span>
                          </div>
                        </div>
                      </div>
                      <div className="text-right">
                        <div className="text-sm font-medium text-primary">{book.progress}%</div>
                        <div className="w-16 bg-muted rounded-full h-2 mt-1">
                          <div 
                            className="bg-gradient-primary h-2 rounded-full transition-smooth" 
                            style={{ width: `${book.progress}%` }}
                          />
                        </div>
                      </div>
                    </div>
                  ))}
                </div>
              </CardContent> */}
              <CardContent>
                <div className="space-y-4">
                  {isLoading && <p className="text-muted-foreground">Loading books...</p>}
                  {isError && <p className="text-red-500">Failed to load books.</p>}

                  {books?.map((book) => (
                    <div
                      key={book.id}
                      className="flex items-center space-x-4 p-4 rounded-lg hover:bg-accent transition-smooth cursor-pointer"
                    >
                      {/* Emoji/cover placeholder */}
                      <div className="text-3xl">📘</div>

                      {/* Book info */}
                      <div className="flex-1 min-w-0">
                        <h3 className="font-semibold text-foreground truncate">
                          {book.title}
                        </h3>
                        <p className="text-sm text-muted-foreground">
                          ISBN: {book.isbn}
                        </p>
                        <div className="flex items-center space-x-2 mt-1">
                          <Badge variant="outline" className="text-xs">
                            Year {book.publishedYear}
                          </Badge>
                        </div>
                      </div>

                      {/* Progress placeholder */}
                      <div className="text-right">
                        <div className="text-sm font-medium text-primary">—</div>
                        <div className="w-16 bg-muted rounded-full h-2 mt-1">
                          <div
                            className="bg-gradient-primary h-2 rounded-full transition-smooth"
                            style={{ width: `0%` }}
                          />
                        </div>
                      </div>
                    </div>
                  ))}
                </div>
              </CardContent>

            </Card>
          </div>
        </div>
      </main>
    </div>
  );
};

export default Dashboard;