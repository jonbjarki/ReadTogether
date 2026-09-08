import { BookshelfBookItem } from "@/types/bookshelves/bookshelf-types";
import Link from "next/link";
import CoverImageWithFallback from "../books-list/image-with-fallback";

function getSubtitle(author: string | null, firstPublished: string | null) {
    if (author && firstPublished) {
        return `${author} • ${firstPublished}`;
    }
    else if (author) {
        return author;
    }
    else if (firstPublished) {
        return firstPublished;
    }

    return null;
}



export default function BookshelfBook({ book }: { book: BookshelfBookItem }) {
    const subtitle = getSubtitle(book.authorName, book.firstPublishedYear?.toString() ?? null);

    return (
        <li>
            <Link href={"/books/" + book.id} className="flex flex-row gap-4 books-center justify-left">
                <div className="relative w-32 h-48">
                    <CoverImageWithFallback title={book.title ?? ""} url={book.coverImageUrl} />
                </div>
                <div className="flex flex-col gap-4">
                    <h3 className="text-sm lg:text-lg font-semibold">{book.title}</h3>
                    <p className="text-gray-600 dark:text-gray-300 font-light text-xs lg:text-sm">{subtitle}</p>
                </div>
            </Link>
        </li>
    )
}