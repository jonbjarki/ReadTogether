import { ORDER_OPTIONS, OrderByOptions, OrderDirectionOptions } from "@/types/bookshelves/bookshelf-types"
import { SortAscIcon, SortDescIcon } from "lucide-react"
import { useRouter, usePathname, useSearchParams } from "next/navigation"
import { ChangeEvent, ReactEventHandler, SyntheticEvent } from "react"

// Maps internal ordering fields to more natural words
const SELECT_MAP: Record<OrderByOptions, string> = {
    "dateAdded": "Date Added",
    "title": "Title",
    "year": "Year"
}

export default function BookshelfBookFilters() {
    const router = useRouter();
    const pathname = usePathname();
    const searchParams = useSearchParams();
    const orderDir = searchParams.get("orderDir") ?? "desc";
    const orderBy = searchParams.get("orderBy") ?? "dateAdded";


    const updateQueryParam = (key: string, value: string) => {
        // 1. Create a editable copy of current search params
        const params = new URLSearchParams(searchParams.toString())

        // 2. Set or update the parameter
        params.set(key, value)

        // 3. Update the URL (scroll: false prevents jumping to the top)
        router.push(`${pathname}?${params.toString()}`, { scroll: false })
    }

    const handleSelect = (e: ChangeEvent<HTMLSelectElement>) => {

        updateQueryParam("orderBy", e.target.value);
    }
    const handleClick = () => {
        updateQueryParam("orderDir", orderDir == "asc" ? "desc" : "asc")
    }

    return (
        <div className="flex flex-row justify-end items-center gap-2">
            <label className="text-sm font-light" htmlFor="orderSelect">ORDER BY</label>
            <select className="border border-foreground p-1 text-sm" id="orderSelect" onChange={handleSelect} defaultValue={orderBy}>
                {ORDER_OPTIONS.map(opt => (
                    <option key={opt} value={opt}>
                        {SELECT_MAP[opt]}
                    </option>
                ))}
            </select>
            <button onClick={handleClick}>
                {orderDir == "asc"
                    ? <SortAscIcon /> : <SortDescIcon />}
            </button>
        </div>
    )
}