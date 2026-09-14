import { auth, signOut, signIn } from "@/auth";
import SearchBar from "@/components/search/search-bar";
import { StrictMode, Suspense } from "react";

export default async function HomePage() {
  return (
    <StrictMode>
      <main>
        <Suspense>
          <SearchBar />
        </Suspense>
      </main>
    </StrictMode>
  )
}