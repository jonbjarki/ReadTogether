import { number, z } from "zod";
import { bookItemSchema } from "./books-schemas";
import { DIR_OPTIONS, ORDER_OPTIONS } from "@/types/bookshelves/bookshelf-types";


export const bookshelfListItemSchema = z.object({
    id: z.number(),
    name: z.string(),
    description: z.string().nullable(),
    isBookInShelf: z.boolean().nullable() // Shows whether provided book is already in the bookshelf. This is only included when fetching user's own bookshelves with a bookId query parameter.
});

export const bookshelfListBookItemSchema = z.object({
    id: z.string(),
    title: z.string(),
    coverImageUrl: z.url().nullable(),
    firstPublishedYear: z.number().nullable(),
    authorName: z.string().nullable()
});

export const bookshelfDetailsSchema = z.object({
    id: z.number(),
    name: z.string(),
    totalBooks: z.number()
});

export const bookshelfBooksPagingParams = z.object({
    page: z.coerce.number().default(1),
    pageSize: z.coerce.number().default(10),
    orderBy: z.enum(ORDER_OPTIONS).optional().default("dateAdded"),
    orderDir: z.enum(DIR_OPTIONS).optional().default("desc")
});

export const bookshelfBooksResponseSchema = z.object({
    page: z.number(),
    pageSize: z.number(),
    total: z.number(),
    results: z.array(bookshelfListBookItemSchema)
});

export const bookshelfListResponseSchema = z.array(bookshelfListItemSchema);
