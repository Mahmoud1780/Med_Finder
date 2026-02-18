import { HubConnection, HubConnectionBuilder, LogLevel } from "@microsoft/signalr";

export function createStockHubConnection(): HubConnection {
  const baseUrl = process.env.NEXT_PUBLIC_SIGNALR_URL ?? "";
  return new HubConnectionBuilder()
    .withUrl(`${baseUrl}/hubs/stock`)
    .withAutomaticReconnect([0, 2000, 5000, 10000])
    .configureLogging(LogLevel.Warning)
    .build();
}
