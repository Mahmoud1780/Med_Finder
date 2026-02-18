"use client";

import { ReservationTable } from "@/components/ReservationTable";
import { LoadingSkeleton } from "@/components/LoadingSkeleton";
import { EmptyState } from "@/components/EmptyState";
import { useReservations } from "@/hooks/useReservations";

export default function ReservationsPage() {
  const { reservations, loading } = useReservations();

  return (
    <div className="space-y-6">
      <h1 className="text-3xl font-semibold">My reservations</h1>
      {loading ? (
        <LoadingSkeleton rows={2} />
      ) : reservations.length === 0 ? (
        <EmptyState title="No reservations yet" description="Reserve medicines from the search page." />
      ) : (
        <ReservationTable reservations={reservations} />
      )}
    </div>
  );
}
