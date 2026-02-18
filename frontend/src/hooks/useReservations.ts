"use client";

import { useCallback, useEffect, useRef, useState } from "react";
import api from "@/lib/api";
import { toast } from "sonner";
import type { UserReservationItem } from "@/types";
import { useAuth } from "@/hooks/useAuth";
import { useReservationSignalR } from "@/hooks/useReservationSignalR";

export function useReservations() {
  const [reservations, setReservations] = useState<UserReservationItem[]>([]);
  const [loading, setLoading] = useState(true);
  const { user } = useAuth();
  const userIdRef = useRef<string | null>(null);

  useEffect(() => {
    userIdRef.current = user?.id ?? null;
  }, [user]);

  const fetchReservations = useCallback(async () => {
    setLoading(true);
    try {
      const response = await api.get<UserReservationItem[]>("/reservations/me");
      setReservations(response.data);
    } catch (error: any) {
      toast.error(error?.response?.data?.message ?? "Failed to load reservations");
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    void fetchReservations();
  }, [fetchReservations]);

  useReservationSignalR((event) => {
    if (!userIdRef.current || event.userId !== userIdRef.current) {
      return;
    }
    setReservations((prev) =>
      prev.map((item) =>
        item.id === event.reservationId
          ? { ...item, status: event.status, rejectionReason: event.rejectionReason ?? null }
          : item
      )
    );
  });

  return { reservations, loading, refresh: fetchReservations };
}
