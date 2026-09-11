"use client"

import { BookshelfBooksResponse, BookshelfDetails, BookshelfPageParams } from "@/types/bookshelves/bookshelf-types";
import BookshelfBook from "./bookshelf-book";
import { Skeleton } from "../ui/skeleton";
import BookshelfPagination from "./bookshelf-pagination";
import { use, useState } from "react";
import { BookItem } from "@/types/books/books-search-response";
import { X } from "lucide-react";
import { Button } from "../ui/button";

export default function BookshelfBooksList({ params, booksPromise }: { params: BookshelfPageParams, booksPromise: Promise<BookshelfBooksResponse> }) {
    const [isSelectMode, setIsSelectMode] = useState(false);
    const [selectedBooks, setSelectedBooks] = useState<Record<BookItem["id"], boolean>>({});

    const handleSelected = (val: boolean, id: BookItem["id"]) => {
        setSelectedBooks(books => ({ ...books, [id]: val }))
    }

    const toggleSelectMode = () => {
        setSelectedBooks({});
        setIsSelectMode(x => !x);
    }


    const res = use(booksPromise);
    const books = res.results;
    const maxPage = Math.ceil(res.total / res.pageSize);
    if (books.length == 0) {
        return <p className="font-extralight mt-10">This list is empty!</p>
    }
    return (
        <ul className="flex flex-col gap-6">
            <Button variant={"outline"} className="px-2 py-1 w-fit" onClick={toggleSelectMode}>{isSelectMode ? "Cancel" : "Edit"}</Button>
            {books.map((book, i) => (
                <BookshelfBook book={book} key={book.id} isSelectMode={isSelectMode} isSelected={!!selectedBooks[book.id]} handleSelected={handleSelected} />
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