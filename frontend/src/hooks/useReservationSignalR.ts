"use client";

import { useEffect, useRef } from "react";
import { HubConnection } from "@microsoft/signalr";
import { createStockHubConnection } from "@/lib/signalr";
import type { ReservationUpdatedEvent } from "@/types";

export function useReservationSignalR(
  onReservationUpdated: (event: ReservationUpdatedEvent) => void
) {
  const connectionRef = useRef<HubConnection | null>(null);
  const handlerRef = useRef(onReservationUpdated);

  useEffect(() => {
    handlerRef.current = onReservationUpdated;
  }, [onReservationUpdated]);

  useEffect(() => {
    const connection = createStockHubConnection();
    connectionRef.current = connection;

    const handler = (event: ReservationUpdatedEvent) => {
      handlerRef.current(event);
    };

    connection.on("ReservationUpdated", handler);

    connection.start().catch(() => {
      // Automatic reconnect will retry
    });

    return () => {
      connection.off("ReservationUpdated", handler);
      connection.stop().catch(() => undefined);
    };
  }, []);
}
