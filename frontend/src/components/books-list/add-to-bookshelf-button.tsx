import { addToBookshelfAction, fetchOwnBookshelvesAction, removeFromBookshelfAction } from "@/actions/bookshelf-actions";
import { auth } from "@/auth";
import { Button } from "../ui/button";
import { Link } from "lucide-react";
import { BookItem } from "@/types/books/books-search-response";
import AddToBookshelfDropdown from "./add-to-bookshelf-dropdown";
import SignInButton from "../auth/sign-in-button";

export default async function AddToBookshelfButton({ book, userName }: { book: BookItem, userName: string }) {

    const bookshelves = await fetchOwnBookshelvesAction(userName, book.id);
    const addBookToBookshelf = addToBookshelfAction.bind(null, book);
    const removeBookFromBookshelf = removeFromBookshelfAction.bind(null, book);
    return (
        <AddToBookshelfDropdown bookshelves={bookshelves} addAction={addBookToBookshelf} removeAction={removeBookFromBookshelf} />
    )
}