"use client"
import { usePathname, useSearchParams } from "next/navigation";
import { Pagination, PaginationContent, PaginationItem, PaginationPrevious, PaginationLink, PaginationEllipsis, PaginationNext } from "../ui/pagination";
import { BookshelfPageParams } from "@/types/bookshelves/bookshelf-types";


export default function BookshelfPagination({ page, maxPage }: { page: number, maxPage: number }) {
    if (maxPage <= 1) return null;
    const params = useSearchParams();

    function generatePageLink(page: number): string {
        const newParams = new URLSearchParams(params.toString());
        newParams.set("page", page.toString());
        return '?' + newParams.toString();

    }
    // Show current page +/- 1, clamped to the valid page range
    const windowStart = Math.max(1, page - 1);
    const windowEnd = Math.min(maxPage, page + 1);
    const window = Array.from({ length: windowEnd - windowStart + 1 }, (_, i) => windowStart + i);

    return (
        <Pagination>
            <PaginationContent>
                {
                    page > 1 && (
                        <PaginationItem>
                            <PaginationPrevious href={generatePageLink(page - 1)} />
                        </PaginationItem>
                    )
                }
                {
                    windowStart > 1 && (
                        <PaginationItem key={1}>
                            <PaginationLink href={generatePageLink(1)}>1</PaginationLink>
                        </PaginationItem>
                    )
                }
                {
                    windowStart > 2 && (
                        <PaginationItem>
                            <PaginationEllipsis />
                        </PaginationItem>
                    )
                }
                {
                    window.map((p) => (
                        <PaginationItem key={p}>
                            <PaginationLink href={generatePageLink(p)} isActive={p === page}>
                                {p}
                            </PaginationLink>
                        </PaginationItem>
                    ))
                }
                {
                    windowEnd < maxPage - 1 && (
                        <PaginationItem>
                            <PaginationEllipsis />
                        </PaginationItem>
                    )
                }
                {
                    windowEnd < maxPage && (
                        <PaginationItem key={maxPage}>
                            <PaginationLink href={generatePageLink(maxPage)}>{maxPage}</PaginationLink>
                        </PaginationItem>
                    )
                }
                {
                    page < maxPage && (
                        <PaginationItem>
                            <PaginationNext href={generatePageLink(page + 1)} />
                        </PaginationItem>
                    )
                }
            </PaginationContent>
        </Pagination>
    );
}
