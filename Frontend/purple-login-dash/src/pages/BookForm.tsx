import { useState } from "react";
import { useMutation } from "@tanstack/react-query";
import { useNavigate } from "react-router-dom";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { toast } from "@/hooks/use-toast";
import { BooksApi, Book, CreateBookRequest } from "@/lib/booksApi";

const BookForm = () => {
  const navigate = useNavigate();

  const [form, setForm] = useState<CreateBookRequest>({
    title: "",
    isbn: "",
    publishedYear: undefined as unknown as number, // no default value
    authorIds: [],
    categoryIds: [],
  });

  const {
    mutate,
    isPending,
  } = useMutation<Book, Error, CreateBookRequest>({
    mutationFn: (payload) => BooksApi.create(payload),
    onSuccess: () => {
      toast({ title: "Book added successfully!" });
      navigate("/dashboard");
    },
    onError: (err) => {
      toast({ title: err.message || "Failed to add book", variant: "destructive" });
    },
  });

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;
    setForm((prev) => ({
      ...prev,
      [name]:
        name.includes("Year") && value !== ""
          ? Number(value)
          : value,
    }));
  };

  return (
    <div className="min-h-screen flex items-center justify-center bg-gradient-subtle">
      <Card className="w-full max-w-lg">
        <CardHeader>
          <CardTitle>Add New Book</CardTitle>
        </CardHeader>
        <CardContent className="space-y-4">
          <Input
            name="title"
            placeholder="Enter book title"
            value={form.title}
            onChange={handleChange}
          />
          <Input
            name="isbn"
            placeholder="Enter ISBN"
            value={form.isbn}
            onChange={handleChange}
          />
          <Input
            name="publishedYear"
            type="text" // 👈 changed to text to remove number spinner
            placeholder="Enter published year (e.g. 2025)"
            value={form.publishedYear ?? ""}
            onChange={handleChange}
          />
          <Input
            name="authorIds"
            placeholder="Enter Author ID(s), comma-separated"
            value={form.authorIds.join(",")}
            onChange={(e) =>
              setForm((prev) => ({
                ...prev,
                authorIds: e.target.value
                  .split(",")
                  .map((s) => s.trim())
                  .filter(Boolean),
              }))
            }
          />
          <Input
            name="categoryIds"
            placeholder="Enter Category ID(s), comma-separated"
            value={form.categoryIds.join(",")}
            onChange={(e) =>
              setForm((prev) => ({
                ...prev,
                categoryIds: e.target.value
                  .split(",")
                  .map((s) => s.trim())
                  .filter(Boolean),
              }))
            }
          />

          <Button
            className="w-full bg-gradient-primary"
            onClick={() => mutate(form)}
            disabled={isPending}
          >
            {isPending ? "Saving..." : "Save Book"}
          </Button>
        </CardContent>
      </Card>
    </div>
  );
};

export default BookForm;
