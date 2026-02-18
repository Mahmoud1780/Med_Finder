"use client";

import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import { Switch } from "@/components/ui/switch";

export type SearchBarProps = {
  keyword: string;
  onKeywordChange: (value: string) => void;
  inStockOnly: boolean;
  onInStockChange: (value: boolean) => void;
  sortBy: "highestStock" | "nearest";
  onSortChange: (value: "highestStock" | "nearest") => void;
  latitude?: string;
  longitude?: string;
  onLatitudeChange: (value: string) => void;
  onLongitudeChange: (value: string) => void;
  onUseLocation?: () => void;
};

export function SearchBar({
  keyword,
  onKeywordChange,
  inStockOnly,
  onInStockChange,
  sortBy,
  onSortChange,
  latitude,
  longitude,
  onLatitudeChange,
  onLongitudeChange,
  onUseLocation,
}: SearchBarProps) {
  return (
    <div className="rounded-3xl border border-border/70 bg-card/70 p-5 shadow-sm backdrop-blur">
      <div className="grid gap-4 md:grid-cols-[2fr_1fr_1fr]">
        <div className="space-y-2">
          <label className="text-xs font-medium text-muted-foreground">Medicine name</label>
          <Input
            value={keyword}
            onChange={(event) => onKeywordChange(event.target.value)}
            placeholder="Search ibuprofen, cetirizine, metformin..."
            className="h-11 rounded-2xl"
          />
        </div>
        <div className="space-y-2">
          <label className="text-xs font-medium text-muted-foreground">Sort by</label>
          <Select value={sortBy} onValueChange={(value) => onSortChange(value as "highestStock" | "nearest")}
          >
            <SelectTrigger className="h-11 rounded-2xl">
              <SelectValue placeholder="Select" />
            </SelectTrigger>
            <SelectContent>
              <SelectItem value="highestStock">Highest stock</SelectItem>
              <SelectItem value="nearest">Nearest pharmacy</SelectItem>
            </SelectContent>
          </Select>
        </div>
        <div className="flex items-center justify-between gap-4 rounded-2xl border border-border/60 bg-background/60 px-4 py-3">
          <div>
            <p className="text-xs font-medium text-muted-foreground">In stock only</p>
            <p className="text-sm">Hide empty inventory</p>
          </div>
          <Switch checked={inStockOnly} onCheckedChange={onInStockChange} />
        </div>
      </div>

      {sortBy === "nearest" && (
        <div className="mt-4 grid gap-3 md:grid-cols-[1fr_1fr_auto]">
          <Input
            value={latitude}
            onChange={(event) => onLatitudeChange(event.target.value)}
            placeholder="Latitude"
            className="h-11 rounded-2xl"
          />
          <Input
            value={longitude}
            onChange={(event) => onLongitudeChange(event.target.value)}
            placeholder="Longitude"
            className="h-11 rounded-2xl"
          />
          <Button
            type="button"
            variant="secondary"
            className="h-11 rounded-2xl"
            onClick={onUseLocation}
          >
            Use my location
          </Button>
        </div>
      )}
    </div>
  );
}
