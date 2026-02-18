import { Skeleton } from "@/components/ui/skeleton";

export function LoadingSkeleton({ rows = 3 }: { rows?: number }) {
  return (
    <div className="space-y-4">
      {Array.from({ length: rows }).map((_, index) => (
        <div
          key={`skeleton-${index}`}
          className="rounded-3xl border border-border/60 bg-card/80 p-5"
        >
          <Skeleton className="h-5 w-1/2" />
          <Skeleton className="mt-3 h-4 w-1/3" />
          <Skeleton className="mt-6 h-8 w-28" />
        </div>
      ))}
    </div>
  );
}
