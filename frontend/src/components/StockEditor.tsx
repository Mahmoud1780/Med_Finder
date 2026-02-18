"use client";

import { useState } from "react";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { StockEntry } from "@/types";

export function StockEditor({
  entry,
  onSave,
  disabled,
}: {
  entry: StockEntry;
  onSave: (nextQuantity: number) => Promise<void> | void;
  disabled?: boolean;
}) {
  const [value, setValue] = useState(entry.quantity.toString());

  return (
    <div className="flex flex-col gap-2 rounded-2xl border border-border/60 bg-background/50 p-4 md:flex-row md:items-center md:justify-between">
      <div>
        <p className="text-sm font-medium">{entry.medicineName}</p>
        <p className="text-xs text-muted-foreground">{entry.pharmacyName}</p>
      </div>
      <div className="flex items-center gap-2">
        <Input
          type="number"
          value={value}
          onChange={(event) => setValue(event.target.value)}
          className="h-9 w-24 rounded-xl"
          min={0}
        />
        <Button
          size="sm"
          className="rounded-xl"
          disabled={disabled}
          onClick={() => onSave(Number(value))}
        >
          Save
        </Button>
      </div>
    </div>
  );
}
