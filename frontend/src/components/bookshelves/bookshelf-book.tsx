import { BookshelfBookItem } from "@/types/bookshelves/bookshelf-types";
import Link from "next/link";
import CoverImageWithFallback from "../books-list/image-with-fallback";
import { Checkbox } from "../ui/checkbox";
import { BookItem } from "@/types/books/books-search-response";

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


export default function BookshelfBook({ book, isSelectMode, isSelected, handleSelected }: { book: BookshelfBookItem, isSelectMode: boolean, isSelected: boolean, handleSelected: (val: boolean, id: BookItem["id"]) => void }) {
    const subtitle = getSubtitle(book.authorName, book.firstPublishedYear?.toString() ?? null);

    return (
        <li className="flex flex-row items-center gap-2">
            {isSelectMode && (
                <Checkbox className="bg-accent border-foreground" defaultChecked={isSelected} onCheckedChange={(val) => handleSelected(!!val, book.id)} />
            )}
            <Link href={"/books/" + book.id} className="flex flex-row gap-4 items-center justify-left">
                <div className="relative w-20 min-w-20 h-34">
                    <CoverImageWithFallback title={book.title ?? ""} url={book.coverImageUrl} />
                </div>
                <div className="flex flex-col gap-4 max-w-100">
                    <h3 className="text-sm lg:text-sm inline-xs font-semibold">{book.title}</h3>
                    <p className="text-gray-600 dark:text-gray-300 font-light text-xs lg:text-sm">{subtitle}</p>
                </div>
            </Link>
        </li>
    )
}