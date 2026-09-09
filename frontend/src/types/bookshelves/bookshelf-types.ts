import { z } from "zod";
import { bookshelfDetailsSchema, bookshelfListItemSchema, bookshelfListBookItemSchema, bookshelfBooksPagingParams as bookshelfPageParams, bookshelfBooksResponseSchema } from "@/zod/books/bookshelf-schemas";

export type BookshelfListItem = z.infer<typeof bookshelfListItemSchema>;
export type BookshelfDetails = z.infer<typeof bookshelfDetailsSchema>;
export type BookshelfBookItem = z.infer<typeof bookshelfListBookItemSchema>;
export type BookshelfPageParams = z.infer<typeof bookshelfPageParams>;
export type BookshelfBooksResponse = z.infer<typeof bookshelfBooksResponseSchema>;

export const SORT_OPTIONS = ["title", "dateAdded", "year"] as const;
export const DIR_OPTIONS = ["asc", "desc"] as const;
export type SortByOptions = typeof SORT_OPTIONS[number];
export type SortDirectionOptions = typeof DIR_OPTIONS[number];