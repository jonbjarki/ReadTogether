import { z } from "zod";
import { bookshelfDetailsSchema, bookshelfListItemSchema, bookshelfListBookItemSchema, bookshelfBooksPagingParams as bookshelfBooksPagingParams, bookshelfBooksResponseSchema } from "@/zod/books/bookshelf-schemas";

export type BookshelfListItem = z.infer<typeof bookshelfListItemSchema>;
export type BookshelfDetails = z.infer<typeof bookshelfDetailsSchema>;
export type BookshelfBookItem = z.infer<typeof bookshelfListBookItemSchema>;
export type BookshelfBooksPagingParams = z.infer<typeof bookshelfBooksPagingParams>;
export type BookshelfBooksResponse = z.infer<typeof bookshelfBooksResponseSchema>;