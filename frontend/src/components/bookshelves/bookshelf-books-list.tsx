"use client"

import { BookshelfBooksResponse, BookshelfPageParams } from "@/types/bookshelves/bookshelf-types";
import BookshelfBook from "./bookshelf-book";
import { Skeleton } from "../ui/skeleton";
import BookshelfPagination from "./bookshelf-pagination";
import { use, useState, useTransition } from "react";
import { BookItem } from "@/types/books/books-search-response";
import { Button } from "../ui/button";

export default function BookshelfBooksList({ params, booksPromise, removeBooksAction }: { params: BookshelfPageParams, booksPromise: Promise<BookshelfBooksResponse>, removeBooksAction: (bookIds: BookItem["id"][]) => void }) {
    const [isSelectMode, setIsSelectMode] = useState(false);
    const [selectedBooks, setSelectedBooks] = useState<Record<BookItem["id"], boolean>>({});
    const [isPending, startTransition] = useTransition();

    console.log(selectedBooks);

    const handleSelected = (val: boolean, id: BookItem["id"]) => {
        if (val) {
            setSelectedBooks(books => ({ ...books, [id]: val }))
        }
        else {
            setSelectedBooks(prev => {
                const copy = { ...prev };
                delete copy[id];
                return copy
            })
        }
    }

    const toggleSelectMode = () => {
        setSelectedBooks({});
        setIsSelectMode(x => !x);
    }

    const handleDelete = () => {
        startTransition(() => {
            removeBooksAction(Object.keys(selectedBooks));
            setSelectedBooks({});
            setIsSelectMode(false);
        })
    }


    const res = use(booksPromise);
    const books = res.results;
    const maxPage = Math.ceil(res.total / res.pageSize);
    if (books.length == 0) {
        return <p className="font-extralight mt-10">This list is empty!</p>
    }
    return (
        <ul className="flex flex-col gap-6">
            <div className="flex gap-2 w-fit min-w-14 self-end">
                {Object.keys(selectedBooks).length > 0 && (
                    <Button onClick={handleDelete} variant="destructive" disabled={isPending}>Delete</Button>
                )}
                <Button variant={"outline"} className="w-fit min-w-14 self-end" onClick={toggleSelectMode}>{isSelectMode ? "Cancel" : "Edit"}</Button>
            </div>
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