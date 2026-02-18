"use client";

import { useEffect, useState } from "react";
import { useRouter } from "next/navigation";
import { toast } from "sonner";
import api from "@/lib/api";
import { LoadingSkeleton } from "@/components/LoadingSkeleton";
import { EmptyState } from "@/components/EmptyState";
import { Button } from "@/components/ui/button";
import { Card } from "@/components/ui/card";
import type { PharmacyDetails } from "@/types";

export default function PharmaciesPage() {
  const [pharmacies, setPharmacies] = useState<PharmacyDetails[]>([]);
  const [loading, setLoading] = useState(true);
  const router = useRouter();

  useEffect(() => {
    const fetchPharmacies = async () => {
      try {
        const response = await api.get<PharmacyDetails[]>("/pharmacies");
        setPharmacies(response.data);
      } catch (error: any) {
        toast.error(error?.response?.data?.message ?? "Failed to load pharmacies");
      } finally {
        setLoading(false);
      }
    };

    void fetchPharmacies();
  }, []);

  const handlePharmacySelect = (pharmacyId: string) => {
    router.push(`/pharmacies/${pharmacyId}`);
  };

  return (
    <div className="space-y-6">
      <div className="space-y-2">
        <h1 className="text-3xl font-semibold">Pharmacies</h1>
        <p className="text-sm text-muted-foreground">
          Browse all pharmacies and view their medicine stock.
        </p>
      </div>

      {loading ? (
        <LoadingSkeleton rows={3} />
      ) : pharmacies.length === 0 ? (
        <EmptyState title="No pharmacies available" description="Check back later for updates." />
      ) : (
        <div className="grid gap-4 md:grid-cols-2 lg:grid-cols-3">
          {pharmacies.map((pharmacy) => (
            <Card
              key={pharmacy.id}
              className="flex flex-col justify-between rounded-2xl border border-border/60 bg-card/70 p-6 transition-all hover:border-border hover:bg-card"
            >
              <div className="space-y-3">
                <h3 className="text-lg font-semibold">{pharmacy.name}</h3>
                <div className="space-y-1 text-sm text-muted-foreground">
                  <p>📍 Coordinates: ({pharmacy.latitude}, {pharmacy.longitude})</p>
                  <p>
                    💊 Stock Items: <span className="font-medium text-foreground">{pharmacy.stocks?.length ?? 0}</span>
                  </p>
                </div>
              </div>
              <Button
                className="mt-4 rounded-xl w-full"
                onClick={() => handlePharmacySelect(pharmacy.id)}
              >
                View Stock
              </Button>
            </Card>
          ))}
        </div>
      )}
    </div>
  );
}
