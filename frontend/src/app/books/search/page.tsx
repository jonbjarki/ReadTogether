"use server"

import BooksList from "@/components/books-list/books-list";
import SearchBar from "@/components/search/search-bar";
import { BooksSearchResponse } from "@/types/books/books-search-response";
import { SearchPageParamsType } from "@/types/search-page-types";
import { booksSearchResponseSchema } from "@/zod/books/books-schemas";
import { searchPageParamsSchema } from "@/zod/books/books-schemas";
import { redirect } from "next/navigation";
import z from "zod";

// Server action to fetch search results for books based on a query string
async function searchBooksAction(params: SearchPageParamsType): Promise<BooksSearchResponse> {
    let url = process.env.API_URL + "books/search?title=" + params.title + "&page=" + params.page;
    if (params.author) {
        url += "&author=" + params.author;

    }
    if (params.subject) {
        url += "&subject=" + params.subject;
    }

    const res = await fetch(url, { next: { revalidate: 120, tags: ["books-search"] } }); // cache results for 2 minutes

    if (!res.ok) {
        const errorText = await res.text();
        console.error("Failed to fetch books search results", res, errorText);
        switch (res.status) {
            case 400:
                throw new Error("Invalid search query");
            case 429:
                throw new Error("Too many requests. Please try again later.");
            default:
                throw new Error("Failed to fetch books search results: " + res.status + " " + errorText);
        }
    }

    const data = await res.json(); // raw data not yet typed or validated

    const validation = booksSearchResponseSchema.safeParse(data);
    if (!validation.success) {
        console.error("Invalid books search response format", data, validation.error);
        throw new Error("Invalid response format from server");
    }

    return validation.data; // Returns validated data with correct types
}

export default async function SearchPage({
    searchParams,
}: {
    searchParams?: Promise<SearchPageParamsType>;
}) {
    const params = await searchParams;
    const parseResult = await searchPageParamsSchema.safeParseAsync(params);
    if (!parseResult.success) {
        console.error("Invalid search page parameters", params, parseResult.error);
        throw new Error("Invalid search parameters");
    }

    const parsedParams = parseResult.data;
    const result = await searchBooksAction(parsedParams);
    console.log("Search results for", parsedParams, "Has next page:", result.hasNext, "Has previous page:", result.hasPrevious);

    return (
        <div className="flex flex-col gap-6">
            <SearchBar initialValue={parsedParams.title} />
            <BooksList items={result.results} parsedParams={parsedParams} hasNext={result.hasNext} hasPrevious={result.hasPrevious} />
        </div>
    )

}