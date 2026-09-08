import { authenticatedFetch } from "@/lib/authenticated-fetch";
import { BookshelfBookItem, BookshelfBooksPagingParams, BookshelfBooksResponse } from "@/types/bookshelves/bookshelf-types";
import { bookshelfBooksResponseSchema } from "@/zod/books/bookshelf-schemas";
import BookshelfBook from "./bookshelf-book";

async function fetchBookshelfBooks(bookshelfId: number, params: BookshelfBooksPagingParams) {
    const url = new URL(process.env.API_URL + `bookshelves/${bookshelfId}/books`);
    const { page } = params;
    url.searchParams.append("page", page);
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

export default async function BookshelfBooksList({ bookshelfId, params }: { bookshelfId: number, params: BookshelfBooksPagingParams }) {
    const res = await fetchBookshelfBooks(bookshelfId, params);
    const books = res.results;

    return (
        <ul className="flex flex-col gap-6">
            {books.map((book) => (
                <BookshelfBook book={book} key={book.id} />
            ))}
        </ul>
    )
}

export function BookshelfBooksListSkeleton() {
    <ul className="flex flex-col gap-6">

    </ul>
}