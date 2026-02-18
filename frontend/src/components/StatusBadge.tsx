import { Badge } from "@/components/ui/badge";
import { cn } from "@/lib/utils";

const statusStyles: Record<string, string> = {
  Pending: "bg-amber-100 text-amber-900 border-amber-200 dark:bg-amber-500/20 dark:text-amber-200",
  Approved: "bg-emerald-100 text-emerald-900 border-emerald-200 dark:bg-emerald-500/20 dark:text-emerald-200",
  Rejected: "bg-rose-100 text-rose-900 border-rose-200 dark:bg-rose-500/20 dark:text-rose-200",
  InStock: "bg-emerald-100 text-emerald-900 border-emerald-200 dark:bg-emerald-500/20 dark:text-emerald-200",
  OutOfStock: "bg-rose-100 text-rose-900 border-rose-200 dark:bg-rose-500/20 dark:text-rose-200",
};

function normalize(value: string | number) {
  if (typeof value === "number") {
    return value === 1 ? "InStock" : value === 2 ? "OutOfStock" : value.toString();
  }
  return value;
}

export function StatusBadge({ value }: { value: string | number }) {
  const label = normalize(value);
  return (
    <Badge
      variant="outline"
      className={cn("rounded-full px-3 py-1 text-xs", statusStyles[label] ?? "")}
    >
      {label}
    </Badge>
  );
}
