"use client";

import { BookSearchItem } from "@/types/books/books-search-response";
import CoverImageWithFallback from "./image-with-fallback";
import Link from "next/link";

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



export default function BooksListItem({ item }: { item: BookSearchItem }) {
    const subtitle = getSubtitle(item.author, item.firstPublished);

    return (
        <li>
            <Link href={"/books/" + item.id} className="flex flex-row gap-6 items-center justify-left">
                <div className="relative w-22 min-w-22 h-34">
                    <CoverImageWithFallback title={item.title ?? ""} url={item.coverImageUrl} />
                </div>
                <div className="flex flex-col gap-4 max-w-100">
                    <h3 className="text-md inline-sm font-semibold">{item.title}</h3>
                    <p className="text-gray-600 dark:text-gray-300 font-light text-sm lg:text-sm">{subtitle}</p>
                </div>
            </Link>
        </li>
    )
}