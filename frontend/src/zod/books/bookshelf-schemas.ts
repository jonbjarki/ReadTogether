import { number, z } from "zod";
import { bookItemSchema } from "./books-schemas";


export const bookshelfListItemSchema = z.object({
    id: z.number(),
    name: z.string(),
    description: z.string().nullable(),
    isBookInShelf: z.boolean().nullable() // Shows whether provided book is already in the bookshelf. This is only included when fetching user's own bookshelves with a bookId query parameter.
});

export const bookshelfListBookItemSchema = z.object({
    id: z.string(),
    title: z.string(),
    coverImageUrl: z.url().nullable()
});

export const bookshelfDetailsSchema = z.object({
    id: z.number(),
    name: z.string(),
    description: z.string().nullish(),
});

export const bookshelfBooksPagingParams = z.object({
    page: z.string().default("1"),
});

export const bookshelfBooksResponseSchema = z.object({
    page: z.number(),
    pageSize: z.number(),
    numPages: z.number(),
    results: z.array(bookshelfListBookItemSchema)
});

export const bookshelfListResponseSchema = z.array(bookshelfListItemSchema);
