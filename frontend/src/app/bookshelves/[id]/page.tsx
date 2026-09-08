import BookshelfBook from "@/components/bookshelves/bookshelf-book";
import BookshelfBooksList from "@/components/bookshelves/bookshelf-books-list";
import { authenticatedFetch } from "@/lib/authenticated-fetch";
import { bookshelfDetailsSchema, bookshelfBooksPagingParams } from "@/zod/books/bookshelf-schemas";
import Link from "next/link";

async function fetchBookshelf(id: number) {
    const res = await authenticatedFetch(process.env.API_URL + `bookshelves/${id}`);

    console.log("Fetching bookshelf");
    const unvalidated = await res.json();
    const validation = bookshelfDetailsSchema.safeParse(unvalidated);
    console.log("Unvalidated:", unvalidated);

    if (!validation.success) {
        console.error("Validation error in fetch bookshelf", validation.error);
        throw new Error("Something went wrong when validating bookshelf response");
    }

    const data = validation.data;
    console.log("Data:", data);
    return data;
}

export default async function BookshelfPage(props: PageProps<"/bookshelves/[id]">) {
    const { id } = await props.params;
    const params = await props.searchParams;
    const parsedParams = await bookshelfBooksPagingParams.safeParseAsync(params);
    if (!parsedParams.success) {
        throw new Error("Invalid query parameters provided");
    }

    const bookshelfId = parseInt(id);
    const bookshelf = await fetchBookshelf(bookshelfId);
    return (
        <main>
            <header className="w-full h-48">
                <h2 className="m-auto text-xl font-bold text-center">{bookshelf.name}</h2>
            </header>
            <ul className="flex flex-col gap-4">
                <BookshelfBooksList bookshelfId={bookshelf.id} params={parsedParams.data} />
            </ul>

        </main>

    )
}