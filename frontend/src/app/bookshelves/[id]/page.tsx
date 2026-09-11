import { fetchBookshelf } from "@/actions/bookshelf-actions";
import { fetchBookshelfBooks } from "@/actions/bookshelf-queries";
import Bookshelf from "@/components/bookshelves/bookshelf";
import { bookshelfBooksPagingParams } from "@/zod/books/bookshelf-schemas";
import { notFound } from "next/navigation";


export default async function BookshelfPage(props: PageProps<"/bookshelves/[id]">) {
    const { id } = await props.params;
    const params = await props.searchParams;

    const bookshelfId = parseInt(id);
    if (Number.isNaN(bookshelfId)) {
        notFound();
    }

    const parsedParams = await bookshelfBooksPagingParams.safeParseAsync(params);
    if (!parsedParams.success) {
        throw new Error("Invalid query parameters provided");
    }
    const { page, pageSize, orderBy, orderDir } = parsedParams.data;

    const bookshelf = await fetchBookshelf(bookshelfId);
    // Not awaited so it can be streamed in with use() client-side
    const booksPromise = fetchBookshelfBooks(bookshelfId, { page, pageSize, orderBy, orderDir });

    return (
        <Bookshelf bookshelf={bookshelf} params={parsedParams.data} booksPromise={booksPromise} />
    )
}