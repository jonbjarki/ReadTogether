import { Skeleton } from "@/components/ui/skeleton";

export default function Loading() {
    return (
        <main className="px-4 py-8 max-w-3xl mx-auto">
            <div className="flex flex-col md:flex-row gap-8 mb-8 items-center">
                <div className="shrink w-full md:w-48">
                    <div className="relative w-full h-0 pb-[150%] md:pb-[150%]">
                        <Skeleton className="absolute inset-0 h-full w-full" />
                    </div>
                </div>

                <div className="flex flex-col shrink-2 basis-0 grow items-start gap-4 w-full">
                    <Skeleton className="h-8 w-3/4" />
                    <div className="space-y-1 text-lg w-full">
                        <Skeleton className="h-6 w-1/2" />
                    </div>

                    <Skeleton className="h-9 w-40" />
                </div>
            </div>
            <div className="space-y-2">
                <Skeleton className="h-4 w-full" />
                <Skeleton className="h-4 w-full" />
                <Skeleton className="h-4 w-2/3" />
            </div>
        </main>
    )
}