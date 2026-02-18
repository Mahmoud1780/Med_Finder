import { Inbox } from "lucide-react";

export function EmptyState({ title, description }: { title: string; description?: string }) {
  return (
    <div className="flex flex-col items-center justify-center gap-2 rounded-3xl border border-dashed border-border/70 bg-card/60 p-10 text-center">
      <Inbox className="h-8 w-8 text-muted-foreground" />
      <p className="text-sm font-semibold">{title}</p>
      {description && <p className="text-xs text-muted-foreground">{description}</p>}
    </div>
  );
}
