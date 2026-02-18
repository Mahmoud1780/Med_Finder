"use client";

import { useEffect, useRef } from "react";
import { HubConnection } from "@microsoft/signalr";
import { createStockHubConnection } from "@/lib/signalr";

export function useSignalR(
  onStockUpdated: (pharmacyId: string, medicineId: string, quantity: number) => void
) {
  const connectionRef = useRef<HubConnection | null>(null);
  const handlerRef = useRef(onStockUpdated);

  useEffect(() => {
    handlerRef.current = onStockUpdated;
  }, [onStockUpdated]);

  useEffect(() => {
    const connection = createStockHubConnection();
    connectionRef.current = connection;

    const handler = (pharmacyId: string, medicineId: string, quantity: number) => {
      handlerRef.current(pharmacyId, medicineId, quantity);
    };

    connection.on("StockUpdated", handler);

    connection.start().catch(() => {
      // Automatic reconnect will retry
    });

    return () => {
      connection.off("StockUpdated", handler);
      connection.stop().catch(() => undefined);
    };
  }, []);
}
