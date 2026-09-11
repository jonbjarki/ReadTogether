"use client"
import { BookshelfBooksResponse, BookshelfDetails, BookshelfPageParams, OrderByOptions, OrderDirectionOptions } from "@/types/bookshelves/bookshelf-types";
import { Suspense, useState } from "react";
import BookshelfBooksList, { BookshelfBooksListSkeleton } from "./bookshelf-books-list";
import BookshelfBookFilters from "./bookshelf-book-filter";

export default function Bookshelf({ bookshelf, params, booksPromise }: { bookshelf: BookshelfDetails, params: BookshelfPageParams, booksPromise: Promise<BookshelfBooksResponse> }) {
    return (
        <main>
            <header className="w-full min-h-30 flex flex-col justify-center gap-6">
                <h2 className="mx-auto text-2xl font-bold text-center">{bookshelf.name}</h2>
                <div className="flex flex-row justify-between">
                    <p className="text-sm ml-2">{bookshelf.totalBooks} books</p>
                    <BookshelfBookFilters />
                </div>
            </header>
            <ul className="flex flex-col gap-4">
                <Suspense fallback={<BookshelfBooksListSkeleton />}>
                    <BookshelfBooksList params={params} booksPromise={booksPromise} />
                </Suspense>
            </ul>

        </main>
    )
}