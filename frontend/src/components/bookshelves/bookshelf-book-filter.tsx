import { ORDER_OPTIONS, OrderByOptions, OrderDirectionOptions } from "@/types/bookshelves/bookshelf-types"
import { SortAscIcon, SortDescIcon } from "lucide-react"
import { useRouter, usePathname, useSearchParams } from "next/navigation"
import { Select, SelectTrigger, SelectValue, SelectContent, SelectGroup, SelectItem } from "../ui/select"


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
        const params = new URLSearchParams(searchParams.toString())
        params.set(key, value)
        router.push(`${pathname}?${params.toString()}`, { scroll: false })
    }

    const handleSelect = (val: OrderByOptions) => {
        updateQueryParam("orderBy", val);
    }

    const handleClick = () => {
        updateQueryParam("orderDir", orderDir == "asc" ? "desc" : "asc")
    }

    return (
        <div className="flex flex-row justify-end items-center gap-2">
            <label className="text-sm font-light" htmlFor="orderSelect">ORDER BY</label>
            <Select onValueChange={handleSelect} defaultValue={orderBy}>
                <SelectTrigger className="w-46" >
                    <SelectValue />
                </SelectTrigger>
                <SelectContent position="popper">
                    <SelectGroup>
                        {ORDER_OPTIONS.map(opt => (
                            <SelectItem key={opt} value={opt}>
                                {SELECT_MAP[opt]}
                            </SelectItem>
                        ))}
                    </SelectGroup>
                </SelectContent>
            </Select>
            <button onClick={handleClick}>
                {orderDir == "asc"
                    ? <SortAscIcon /> : <SortDescIcon />}
            </button>
        </div>
    )
}