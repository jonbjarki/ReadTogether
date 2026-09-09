import { SORT_OPTIONS, SortByOptions, SortDirectionOptions } from "@/types/bookshelves/bookshelf-types"
import { SortAscIcon, SortDescIcon } from "lucide-react"
import { ChangeEvent, ReactEventHandler, SyntheticEvent } from "react"

type BookshelfBookFiltersProps = {
    selectMode: boolean,
    setSelectMode: (val: boolean) => void,
    sortDir: SortDirectionOptions,
    sortBy: SortByOptions,
    setSortDir: (dir: SortDirectionOptions) => void,
    setSortBy: (by: SortByOptions) => void
}

export default function BookshelfBookFilters(props: BookshelfBookFiltersProps) {
    const handleSelect = (e: ChangeEvent<HTMLSelectElement>) => {
        console.log("selected:", e.target.value as SortDirectionOptions);
        props.setSortBy(e.target.value as SortByOptions);
    }

    return (
        <div>
            <input type="checkbox" defaultChecked={props.selectMode} onChange={(e) => props.setSelectMode(e.target.checked)} />
            <select onChange={handleSelect} defaultValue={props.sortBy}>
                {SORT_OPTIONS.map(opt => (
                    <option key={opt} value={opt}>
                        {opt}
                    </option>
                ))}
            </select>
            <button onClick={() => props.setSortDir(props.sortDir == "asc" ? "desc" : "asc")}>
                {props.sortDir == "asc"
                    ? <SortAscIcon /> : <SortDescIcon />}
            </button>
        </div>
    )
}