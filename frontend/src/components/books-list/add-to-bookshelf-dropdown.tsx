"use client"

import { DropdownMenu, DropdownMenuContent, DropdownMenuGroup, DropdownMenuItem, DropdownMenuTrigger, DropdownMenuCheckboxItem } from "../ui/dropdown-menu"
import { Button } from "../ui/button"
import { BookshelfListItem } from "@/types/bookshelves/bookshelf-types"
import { BookItem } from "@/types/books/books-search-response"
import { useState, useTransition } from "react"
import { bookshelfListItemSchema } from "@/zod/books/bookshelf-schemas"

export default function AddToBookshelfDropdown({
    bookshelves,
    addAction,
    removeAction,
}: {
    bookshelves: BookshelfListItem[]
    addAction: (bookshelfId: number) => Promise<void>
    removeAction: (bookshelfId: number) => Promise<void>
}) {
    const [isPending, startTransition] = useTransition();

    // Instantiates an array with ids of all bookshelves that already include the book
    const [addedTo, setAddedTo] = useState<BookshelfListItem["id"][]>(bookshelves.reduce((accumulator, current) => {
        if (current.isBookInShelf) {
            accumulator.push(current.id);
        }
        return accumulator;
    }, new Array<number>()));

    const isChecked = (bookshelf: BookshelfListItem) => {
        const checked = !!addedTo.some(id => id == bookshelf.id);
        console.log(addedTo);
        console.log(bookshelf.id);
        console.log(checked);
        return checked;
    }
    console.log("Added to:", addedTo);

    const handleChecked = (bookshelfId: BookshelfListItem["id"], checked: boolean) => {
        if (checked) {
            setAddedTo(prev => [...prev, bookshelfId])
            startTransition(() => {
                addAction(bookshelfId);
            })
        }
        else {
            setAddedTo(prev => prev.filter(x => x !== bookshelfId))
            startTransition(() => {
                removeAction(bookshelfId);
            })
        }
    }
    return (
        <DropdownMenu>
            <DropdownMenuTrigger asChild>
                <Button variant="outline">Add to bookshelf</Button>
            </DropdownMenuTrigger>
            <DropdownMenuContent className="w-50 select-none">
                <DropdownMenuGroup>
                    {bookshelves.map((bookshelf) => (
                        <DropdownMenuCheckboxItem disabled={isPending} className="p-1" key={bookshelf.id} onSelect={(e) => e.preventDefault()} checked={isChecked(bookshelf)} onCheckedChange={(checked => {
                            handleChecked(bookshelf.id, checked);
                        })}>
                            {bookshelf.name}
                        </DropdownMenuCheckboxItem>
                    ))}
                </DropdownMenuGroup>
            </DropdownMenuContent>
        </DropdownMenu>
    )
}