"use client";

import * as React from "react";
import { toast } from "sonner";
import { SearchBar } from "@/components/SearchBar";
import { MedicineCard } from "@/components/MedicineCard";
import { ReservationModal } from "@/components/ReservationModal";
import { LoadingSkeleton } from "@/components/LoadingSkeleton";
import { EmptyState } from "@/components/EmptyState";
import api from "@/lib/api";
import type { MedicineSearchResult, SearchResponse } from "@/types";
import { useSignalR } from "@/hooks/useSignalR";

export default function SearchPage() {
  const [keyword, setKeyword] = React.useState("");
  const [inStockOnly, setInStockOnly] = React.useState(false);
  const [sortBy, setSortBy] = React.useState<"highestStock" | "nearest">("highestStock");
  const [latitude, setLatitude] = React.useState("");
  const [longitude, setLongitude] = React.useState("");
  const [results, setResults] = React.useState<MedicineSearchResult[]>([]);
  const [alternatives, setAlternatives] = React.useState<SearchResponse["alternatives"]>([]);
  const [message, setMessage] = React.useState<string | undefined>();
  const [loading, setLoading] = React.useState(false);
  const [selected, setSelected] = React.useState<MedicineSearchResult | null>(null);
  const [highlightKey, setHighlightKey] = React.useState<string | null>(null);

  const debouncedKeyword = useDebouncedValue(keyword, 400);

  const handleSearch = React.useCallback(async () => {
    if (!debouncedKeyword.trim()) {
      setResults([]);
      setAlternatives([]);
      setMessage(undefined);
      return;
    }

    if (sortBy === "nearest" && (!latitude || !longitude)) {
      setMessage("Provide latitude and longitude to sort by nearest.");
      return;
    }

    setLoading(true);
    try {
      const response = await api.get<SearchResponse>("/medicines/search", {
        params: {
          keyword: debouncedKeyword,
          inStockOnly,
          sortBy: sortBy === "nearest" ? "Nearest" : "HighestStock",
          latitude: latitude || undefined,
          longitude: longitude || undefined,
        },
      });

      setResults(response.data.results ?? []);
      setAlternatives(response.data.alternatives ?? []);
      setMessage(response.data.message ?? undefined);
    } catch (error: any) {
      toast.error(error?.response?.data?.message ?? "Search failed");
    } finally {
      setLoading(false);
    }
  }, [debouncedKeyword, inStockOnly, sortBy, latitude, longitude]);

  React.useEffect(() => {
    void handleSearch();
  }, [handleSearch]);

  useSignalR((pharmacyId, medicineId, quantity) => {
    setResults((prev) => {
      const updated = prev.map((item) =>
        item.pharmacyId === pharmacyId && item.medicineId === medicineId
          ? { ...item, quantity, availability: quantity > 0 ? "InStock" : "OutOfStock" }
          : item
      );
      if (sortBy === "highestStock") {
        return [...updated].sort((a, b) => b.quantity - a.quantity);
      }
      return updated;
    });
    setHighlightKey(`${pharmacyId}-${medicineId}`);
    setTimeout(() => setHighlightKey(null), 1200);
  });

  const onUseLocation = () => {
    if (!navigator.geolocation) {
      toast.error("Geolocation not supported");
      return;
    }
    navigator.geolocation.getCurrentPosition(
      (pos) => {
        setLatitude(pos.coords.latitude.toFixed(4));
        setLongitude(pos.coords.longitude.toFixed(4));
      },
      () => toast.error("Unable to fetch location")
    );
  };

  return (
    <div className="space-y-8">
      <div className="space-y-2">
        <h1 className="text-3xl font-semibold">Search medicines</h1>
        <p className="text-sm text-muted-foreground">
          Live stock across pharmacies with real-time updates.
        </p>
      </div>

      <SearchBar
        keyword={keyword}
        onKeywordChange={setKeyword}
        inStockOnly={inStockOnly}
        onInStockChange={setInStockOnly}
        sortBy={sortBy}
        onSortChange={setSortBy}
        latitude={latitude}
        longitude={longitude}
        onLatitudeChange={setLatitude}
        onLongitudeChange={setLongitude}
        onUseLocation={onUseLocation}
      />

      {loading ? (
        <LoadingSkeleton rows={3} />
      ) : results.length > 0 ? (
        <div className="grid gap-4">
          {results.map((result) => (
            <MedicineCard
              key={`${result.pharmacyId}-${result.medicineId}`}
              result={result}
              onReserve={setSelected}
              highlight={highlightKey === `${result.pharmacyId}-${result.medicineId}`}
            />
          ))}
        </div>
      ) : (
        <EmptyState
          title={message ?? "Start typing to see results"}
          description="We will surface alternatives when possible."
        />
      )}

      {!loading && alternatives.length > 0 && (
        <div className="space-y-3">
          <h2 className="text-lg font-semibold">Alternatives</h2>
          <div className="grid gap-3 md:grid-cols-3">
            {alternatives.map((alt) => (
              <div
                key={alt.medicineId}
                className="rounded-2xl border border-border/60 bg-card/70 p-4"
              >
                <p className="text-sm font-medium">{alt.name}</p>
                <p className="text-xs text-muted-foreground">{alt.category}</p>
                <p className="text-xs text-muted-foreground">{alt.activeIngredient}</p>
              </div>
            ))}
          </div>
        </div>
      )}

      <ReservationModal
        open={!!selected}
        onOpenChange={(open) => !open && setSelected(null)}
        medicine={selected}
        onSuccess={() => setSelected(null)}
      />
    </div>
  );
}

function useDebouncedValue<T>(value: T, delay: number) {
  const [debounced, setDebounced] = React.useState(value);

  React.useEffect(() => {
    const timer = setTimeout(() => setDebounced(value), delay);
    return () => clearTimeout(timer);
  }, [value, delay]);

  return debounced;
}
