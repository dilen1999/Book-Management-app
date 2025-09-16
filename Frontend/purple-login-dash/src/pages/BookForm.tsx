import { useState } from "react";
import { useMutation } from "@tanstack/react-query";
import { useNavigate } from "react-router-dom";
// import { BooksApi } from "@/lib/booksApi";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { toast } from "@/hooks/use-toast";

const BookForm = () => {
  const navigate = useNavigate();
  const [form, setForm] = useState({
    title: "",
    isbn: "",
    publishedYear: 0,
    authorIds: [""],
    categoryIds: [""],
  });

//   const mutation = useMutation({
//     mutationFn: () => BooksApi.create(form),
//     onSuccess: () => {
//       toast({ title: "Book added successfully!" });
//       navigate("/dashboard");
//     },
//     onError: () => {
//       toast({ title: "Failed to add book", variant: "destructive" });
//     },
//   });

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;
    setForm((prev) => ({ ...prev, [name]: name.includes("Year") ? Number(value) : value }));
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
            placeholder="Title"
            value={form.title}
            onChange={handleChange}
          />
          <Input
            name="isbn"
            placeholder="ISBN"
            value={form.isbn}
            onChange={handleChange}
          />
          <Input
            name="publishedYear"
            type="number"
            placeholder="Published Year"
            value={form.publishedYear}
            onChange={handleChange}
          />
          <Input
            name="authorIds"
            placeholder="Author ID(s) comma-separated"
            value={form.authorIds.join(",")}
            onChange={(e) =>
              setForm((prev) => ({
                ...prev,
                authorIds: e.target.value.split(",").map((id) => id.trim()),
              }))
            }
          />
          <Input
            name="categoryIds"
            placeholder="Category ID(s) comma-separated"
            value={form.categoryIds.join(",")}
            onChange={(e) =>
              setForm((prev) => ({
                ...prev,
                categoryIds: e.target.value.split(",").map((id) => id.trim()),
              }))
            }
          />

          <Button
            className="w-full bg-gradient-primary"
            // onClick={() => mutation.mutate()}
            // disabled={mutation.isLoading}
          >
            {/* {mutation.isLoading ? "Saving..." : "Save Book"} */}
          </Button>
        </CardContent>
      </Card>
    </div>
  );
};

export default BookForm;
