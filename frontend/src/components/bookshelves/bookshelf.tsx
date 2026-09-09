"use client"
import { BookshelfBooksResponse, BookshelfDetails, BookshelfPageParams, SortByOptions, SortDirectionOptions } from "@/types/bookshelves/bookshelf-types";
import { Suspense, useState } from "react";
import BookshelfBooksList, { BookshelfBooksListSkeleton } from "./bookshelf-books-list";
import BookshelfBookFilters from "./bookshelf-book-filter";

export default function Bookshelf({ bookshelf, params, booksPromise }: { bookshelf: BookshelfDetails, params: BookshelfPageParams, booksPromise: Promise<BookshelfBooksResponse> }) {
    const [selectMode, setSelectMode] = useState(false);
    const [sortBy, setSortBy] = useState<SortByOptions>("dateAdded");
    const [sortDir, setSortDir] = useState<SortDirectionOptions>("desc");
    return (
        <main>
            <header className="w-full min-h-30 flex flex-col justify-center">
                <h2 className="mx-auto text-2xl font-bold text-center">{bookshelf.name}</h2>
                <p className="text-sm ml-2">{bookshelf.totalBooks} books</p>
            </header>
            <ul className="flex flex-col gap-4">
                <p>{selectMode && "YES"}</p>
                <p>Sort By: {sortBy}</p>
                <p>{sortDir == "asc" ? "ASC" : "DESC"}</p>
                <Suspense fallback={<BookshelfBooksListSkeleton />}>
                    <>
                        <BookshelfBookFilters selectMode={selectMode} setSelectMode={setSelectMode} sortBy={sortBy} sortDir={sortDir} setSortBy={setSortBy} setSortDir={setSortDir} />
                        <BookshelfBooksList params={params} booksPromise={booksPromise} />
                    </>
                </Suspense>
            </ul>

        </main>
    )
}