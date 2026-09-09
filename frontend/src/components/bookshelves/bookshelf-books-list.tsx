"use client"

import { BookshelfBooksResponse, BookshelfPageParams } from "@/types/bookshelves/bookshelf-types";
import BookshelfBook from "./bookshelf-book";
import { Skeleton } from "../ui/skeleton";
import BookshelfPagination from "./bookshelf-pagination";
import { use } from "react";

export default function BookshelfBooksList({ params, booksPromise }: { params: BookshelfPageParams, booksPromise: Promise<BookshelfBooksResponse> }) {
    const res = use(booksPromise);
    const books = res.results;
    const maxPage = Math.ceil(res.total / res.pageSize);
    if (books.length == 0) {
        return <p className="font-extralight mt-10">This list is empty!</p>
    }
    return (
        <ul className="flex flex-col gap-6">
            {books.map((book, i) => (
                <BookshelfBook book={book} key={book.id} index={((res.page - 1) * res.pageSize) + (i + 1)} />
            ))}
            <BookshelfPagination page={params.page} maxPage={maxPage} />
        </ul>
    )
}


// Loading skeleton
export function BookshelfBooksListSkeleton() {
    return (
        <ul className="flex flex-col gap-6">
            {
                new Array(5).fill(0).map((_, i) => (
                    <li key={i} className="flex flex-row items-center gap-6">
                        <Skeleton className="relative w-20 min-w-22 h-34">
                        </Skeleton>
                        <div className="flex flex-col gap-4 max-w-100">
                            <Skeleton className="text-sm lg:text-lg font-semibold w-64 h-4"></Skeleton>
                            <Skeleton className="text-gray-600 dark:text-gray-300 font-light text-xs lg:text-sm w-32 h-4"></Skeleton>
                        </div>
                    </li>
                ))
            }
        </ul>
    )
}