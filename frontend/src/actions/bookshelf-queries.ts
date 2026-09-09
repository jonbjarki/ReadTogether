import { authenticatedFetch } from "@/lib/authenticated-fetch";
import { BookshelfPageParams, BookshelfBooksResponse } from "@/types/bookshelves/bookshelf-types";
import { bookshelfBooksResponseSchema } from "@/zod/books/bookshelf-schemas";

// Not a Server Action: its promise is streamed unresolved into a Client Component's `use()`,
// which requires a plain async function rather than a "use server" action reference.
export async function fetchBookshelfBooks(bookshelfId: number, params: BookshelfPageParams) {
    const url = new URL(process.env.API_URL + `bookshelves/${bookshelfId}/books`);
    const { page } = params;
    url.searchParams.append("page", page.toString());

    const res = await authenticatedFetch(url);
    const unvalidated = await res.json();
    const validation = await bookshelfBooksResponseSchema.safeParseAsync(unvalidated);

    if (!validation.success) {
        console.log("res:", await unvalidated);
        console.log("errors:", validation.error);
        throw new Error("Unexpected response received from server");
    }

    return validation.data as BookshelfBooksResponse;
}
