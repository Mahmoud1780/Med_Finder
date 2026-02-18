"use client";

import { useEffect, useState } from "react";
import { useParams, useRouter } from "next/navigation";
import { toast } from "sonner";
import api from "@/lib/api";
import { LoadingSkeleton } from "@/components/LoadingSkeleton";
import { EmptyState } from "@/components/EmptyState";
import { ReservationModal } from "@/components/ReservationModal";
import { Button } from "@/components/ui/button";
import { Card } from "@/components/ui/card";
import type { PharmacyDetails, PharmacyStock, MedicineSearchResult } from "@/types";

export default function PharmacyDetailPage() {
  const params = useParams();
  const router = useRouter();
  const pharmacyId = params.id as string;

  const [pharmacy, setPharmacy] = useState<PharmacyDetails | null>(null);
  const [loading, setLoading] = useState(true);
  const [selected, setSelected] = useState<MedicineSearchResult | null>(null);

  useEffect(() => {
    const fetchPharmacyDetails = async () => {
      try {
        const response = await api.get<PharmacyDetails>(`/pharmacies/${pharmacyId}`);
        setPharmacy(response.data);
      } catch (error: any) {
        toast.error(error?.response?.data?.message ?? "Failed to load pharmacy details");
        router.push("/pharmacies");
      } finally {
        setLoading(false);
      }
    };

    if (pharmacyId) {
      void fetchPharmacyDetails();
    }
  }, [pharmacyId, router]);

  const handleReserve = (stock: PharmacyStock) => {
    if (!pharmacy) return;
    
    // Convert PharmacyStock to MedicineSearchResult format for ReservationModal
    const medicineResult: MedicineSearchResult = {
      medicineId: stock.medicineId,
      medicineName: stock.medicineName,
      category: "", // If available in stock, add it
      activeIngredient: "", // If available in stock, add it
      pharmacyId: pharmacy.id,
      pharmacyName: pharmacy.name,
      latitude: pharmacy.latitude,
      longitude: pharmacy.longitude,
      quantity: stock.quantity,
      availability: stock.quantity > 0 ? "InStock" : "OutOfStock",
    };
    
    setSelected(medicineResult);
  };

  if (loading) {
    return <LoadingSkeleton rows={4} />;
  }

  if (!pharmacy) {
    return <EmptyState title="Pharmacy not found" description="The pharmacy you're looking for doesn't exist." />;
  }

  return (
    <div className="space-y-6">
      <div>
        <Button 
          variant="ghost" 
          onClick={() => router.back()}
          className="mb-4 pl-0"
        >
          ← Back
        </Button>
        <div className="space-y-2">
          <h1 className="text-4xl font-semibold">{pharmacy.name}</h1>
          <p className="text-sm text-muted-foreground">
            📍 Location: {pharmacy.latitude.toFixed(4)}, {pharmacy.longitude.toFixed(4)}
          </p>
        </div>
      </div>

      <div>
        <h2 className="text-xl font-semibold mb-4">Medicine Stock</h2>
        {pharmacy.stocks && pharmacy.stocks.length > 0 ? (
          <div className="grid gap-3 md:grid-cols-2 lg:grid-cols-3">
            {pharmacy.stocks.map((stock) => (
              <Card
                key={stock.medicineId}
                className="flex flex-col justify-between rounded-2xl border border-border/60 bg-card/70 p-5 transition-all hover:border-border hover:bg-card"
              >
                <div className="space-y-2">
                  <h3 className="text-base font-semibold">{stock.medicineName}</h3>
                  <div className="flex items-center gap-2">
                    <span className={`text-sm font-medium px-2 py-1 rounded-lg ${
                      stock.quantity > 0 
                        ? 'bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-100' 
                        : 'bg-red-100 text-red-800 dark:bg-red-900 dark:text-red-100'
                    }`}>
                      {stock.quantity} unit{stock.quantity !== 1 ? 's' : ''} in stock
                    </span>
                  </div>
                </div>
                <Button
                  className="mt-4 rounded-xl w-full"
                  disabled={stock.quantity === 0}
                  onClick={() => handleReserve(stock)}
                >
                  {stock.quantity > 0 ? 'Reserve' : 'Out of Stock'}
                </Button>
              </Card>
            ))}
          </div>
        ) : (
          <EmptyState title="No medicines in stock" description="This pharmacy currently has no medicines available." />
        )}
      </div>

      <ReservationModal
        open={!!selected}
        onOpenChange={(open) => !open && setSelected(null)}
        medicine={selected}
        onSuccess={() => setSelected(null)}
      />
    </div>
  );
}
