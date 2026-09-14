import CoverImageWithFallback from "@/components/books-list/image-with-fallback";
import type { BookItem } from "@/types/books/books-search-response";
import DOMPurify from 'dompurify';
import parse from 'html-react-parser';
import { notFound } from "next/navigation";
import { JSDOM } from "jsdom"
import { Button } from "@/components/ui/button";
import { fetchUserBookshelvesAction } from "@/actions/bookshelf-actions";
import { auth } from "@/auth";
import AddToBookshelfButton from "@/components/books-list/add-to-bookshelf-button";
import { Suspense } from "react";
import Link from "next/link";
import { signIn } from "@/auth";
import SignInButton from "@/components/auth/sign-in-button";

async function fetchBookAction(bookId: string): Promise<BookItem> {
    const res = await fetch(process.env.API_URL + "books/" + bookId, { next: { revalidate: 300 }, cache: "force-cache" }); // cache for 5 minutes

    if (!res.ok) {
        if (res.status === 404) {
            return notFound();
        }
        const errorContent = await res.text();
        console.error("error occured fetching book", res.status, errorContent);
        throw new Error("Failed to fetch book details");
    }

    return await res.json() satisfies BookItem;
}

async function signInAction(id: string) {
    "use server"
    await signIn(undefined, { redirectTo: `/books/${id}` });
}

export default async function BookPage({ params }: { params: Promise<{ id: string }> }) {
    const session = await auth();
    const id = (await params).id;

    const book = await fetchBookAction(id);

    // Sanitizes and parses description as there are often simple HTML tags included "<p> <b> <i> etc."
    const window = new JSDOM('').window;
    const purify = DOMPurify(window);
    const sanitizedDescription = purify.sanitize(book.description ?? "", {
        ALLOWED_TAGS: ["<br>"]
    })
    const description = parse(sanitizedDescription);

    return (
        <main className="px-4 py-8 max-w-3xl mx-auto">
            <div className="flex flex-col md:flex-row gap-8 mb-8 items-center">
                <div className="shrink w-full md:w-48">
                    <div className="relative w-full h-0 pb-[150%] md:pb-[150%]">
                        <CoverImageWithFallback url={book.coverImageUrl} title={book.title} />
                    </div>
                </div>

                <div className="flex flex-col shrink-2 basis-0 grow items-start gap-4">
                    <h1 className="text-2xl font-bold">{book.title}</h1>
                    <div className="space-y-1 text-lg">
                        <p>{authorAndYear(book.authorName, book.firstPublishedYear)}</p>
                    </div>

                    {!!session?.user.name ?
                        (<Suspense fallback={<Button variant="outline">Add to bookshelf</Button>}>
                            <AddToBookshelfButton book={book} userName={session.user.name} />
                        </Suspense>)
                        :
                        (<SignInButton redirectTo={`/books/${id}`} text="Add to bookshelf" />)
                    }

                </div>
            </div>
            <p className="leading-7 text-pretty">
                {description}
            </p>
        </main >
    );
}

function authorAndYear(authorName: string | null, year?: string | null) {
    if (authorName && year) {
        return `${authorName} · ${year}`
    }
    else if (authorName && !year) {
        return authorName
    }
    else if (!authorName && year) {
        return year
    }

}