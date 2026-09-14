"use client"
import { useRouter } from "next/navigation";
import { SyntheticEvent, useState } from "react"
import { Button } from "@/components/ui/button"
import { Field } from "@/components/ui/field"
import { Input } from "@/components/ui/input"


export default function SearchBar({ initialValue }: { initialValue?: string }) {
    const router = useRouter();
    const [value, setValue] = useState(initialValue || "");
    function handleSubmit(e: SyntheticEvent<HTMLFormElement>) {
        e.preventDefault();
        if (value.trim() === "") return; // prevent empty searches

        router.push("/books/search?title=" + encodeURIComponent(value));
    }

    return (
        <form onSubmit={handleSubmit}>
            <Field orientation="horizontal">
                <Input className="h-10" type="search" placeholder="Search for anything..." value={value} onChange={(e) => { setValue(e.target.value) }} />
                <Button size="lg">Search</Button>
            </Field>
        </form>
    )
}