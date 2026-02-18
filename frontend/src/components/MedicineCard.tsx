"use client";

import { Button } from "@/components/ui/button";
import { Card, CardContent } from "@/components/ui/card";
import { StatusBadge } from "@/components/StatusBadge";
import { cn } from "@/lib/utils";
import { MedicineSearchResult } from "@/types";

export function MedicineCard({
  result,
  onReserve,
  highlight,
}: {
  result: MedicineSearchResult;
  onReserve: (result: MedicineSearchResult) => void;
  highlight?: boolean;
}) {
  const isInStock = result.quantity > 0;
  const availabilityLabel =
    typeof result.availability === "number"
      ? result.availability === 1
        ? "InStock"
        : "OutOfStock"
      : result.availability;

  return (
    <Card
      className={cn(
        "rounded-3xl border border-border/60 bg-card/80 shadow-sm transition",
        highlight && "animate-pulse border-primary/40"
      )}
    >
      <CardContent className="flex flex-col gap-4 p-5">
        <div className="space-y-2">
          <div className="flex items-center justify-between">
            <h3 className="text-lg font-semibold">{result.medicineName}</h3>
            <StatusBadge value={availabilityLabel} />
          </div>
          <p className="text-sm text-muted-foreground">
            {result.category} · {result.activeIngredient}
          </p>
        </div>
        <div className="flex flex-wrap items-center justify-between gap-3">
          <div className="space-y-1 text-sm">
            <p className="font-medium">{result.pharmacyName}</p>
            <p className="text-muted-foreground">
              Qty: <span className="font-semibold text-foreground">{result.quantity}</span>
              {typeof result.distanceKm === "number" && (
                <span className="ml-2 text-xs text-muted-foreground">
                  {result.distanceKm.toFixed(2)} km away
                </span>
              )}
            </p>
          </div>
          <Button
            disabled={!isInStock}
            className="rounded-2xl"
            onClick={() => onReserve(result)}
          >
            Reserve
          </Button>
        </div>
      </CardContent>
    </Card>
  );
}
