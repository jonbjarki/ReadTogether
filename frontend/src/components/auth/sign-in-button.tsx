"use client"

import { signIn } from "next-auth/react"
import { Button } from "../ui/button"

export default function SignInButton({ text, redirectTo }: { text: string, redirectTo: string }) {
    return (
        <Button variant="outline" onClick={() => signIn(undefined, { redirectTo })}>{text}</Button>
    )
}