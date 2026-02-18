import { Card, CardContent } from "@/components/ui/card";
import { StatusBadge } from "@/components/StatusBadge";
import { PharmacyStock } from "@/types";

export function PharmacyCard({
  name,
  latitude,
  longitude,
  stocks,
}: {
  name: string;
  latitude: number;
  longitude: number;
  stocks: PharmacyStock[];
}) {
  return (
    <Card className="rounded-3xl border border-border/60 bg-card/80 shadow-sm">
      <CardContent className="space-y-4 p-6">
        <div>
          <h2 className="text-xl font-semibold">{name}</h2>
          <p className="text-sm text-muted-foreground">
            {latitude.toFixed(4)}, {longitude.toFixed(4)}
          </p>
        </div>
        <div className="space-y-3">
          {stocks.map((stock) => (
            <div
              key={stock.medicineId}
              className="flex items-center justify-between rounded-2xl border border-border/50 px-4 py-3"
            >
              <div>
                <p className="text-sm font-medium">{stock.medicineName}</p>
                <p className="text-xs text-muted-foreground">Qty: {stock.quantity}</p>
              </div>
              <StatusBadge value={stock.quantity > 0 ? "InStock" : "OutOfStock"} />
            </div>
          ))}
        </div>
      </CardContent>
    </Card>
  );
}
