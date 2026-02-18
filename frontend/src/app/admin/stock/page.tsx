"use client";

import * as React from "react";
import { toast } from "sonner";
import api from "@/lib/api";
import { StockEditor } from "@/components/StockEditor";
import Link from "next/link";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Textarea } from "@/components/ui/textarea";
import { Dialog, DialogContent, DialogHeader, DialogTitle } from "@/components/ui/dialog";
import { LoadingSkeleton } from "@/components/LoadingSkeleton";
import { EmptyState } from "@/components/EmptyState";
import type { PendingReservation, StockEntry } from "@/types";

export default function AdminStockPage() {
  const [stock, setStock] = React.useState<StockEntry[]>([]);
  const [pending, setPending] = React.useState<PendingReservation[]>([]);
  const [loading, setLoading] = React.useState(true);
  const [savingKey, setSavingKey] = React.useState<string | null>(null);
  const [processingId, setProcessingId] = React.useState<string | null>(null);
  const [rejecting, setRejecting] = React.useState<PendingReservation | null>(null);
  const [rejectReason, setRejectReason] = React.useState("");

  const fetchData = React.useCallback(async () => {
    setLoading(true);
    try {
      const [stockRes, pendingRes] = await Promise.all([
        api.get<StockEntry[]>("/admin/stock"),
        api.get<PendingReservation[]>("/admin/reservations/pending"),
      ]);
      setStock(stockRes.data);
      setPending(pendingRes.data);
    } finally {
      setLoading(false);
    }
  }, []);

  React.useEffect(() => {
    void fetchData();
  }, [fetchData]);

  const handleSave = async (entry: StockEntry, quantity: number) => {
    const key = `${entry.pharmacyId}-${entry.medicineId}`;
    setSavingKey(key);
    try {
      await api.put("/admin/stock", {
        pharmacyId: entry.pharmacyId,
        medicineId: entry.medicineId,
        quantity,
      });
      toast.success("Stock updated");
      setStock((prev) =>
        prev.map((item) =>
          item.pharmacyId === entry.pharmacyId && item.medicineId === entry.medicineId
            ? { ...item, quantity }
            : item
        )
      );
    } catch (error: any) {
      toast.error(error?.response?.data?.message ?? "Update failed");
    } finally {
      setSavingKey(null);
    }
  };

  const approve = async (reservation: PendingReservation) => {
    setProcessingId(reservation.id);
    try {
      await api.post(`/reservations/${reservation.id}/approve`);
      toast.success("Reservation approved");
      setPending((prev) => prev.filter((item) => item.id !== reservation.id));
    } catch (error: any) {
      toast.error(error?.response?.data?.message ?? "Approval failed");
    } finally {
      setProcessingId(null);
    }
  };

  const reject = async () => {
    if (!rejecting) return;
    setProcessingId(rejecting.id);
    try {
      await api.post(`/reservations/${rejecting.id}/reject`, { reason: rejectReason });
      toast.success("Reservation rejected");
      setPending((prev) => prev.filter((item) => item.id !== rejecting.id));
      setRejecting(null);
      setRejectReason("");
    } catch (error: any) {
      toast.error(error?.response?.data?.message ?? "Rejection failed");
    } finally {
      setProcessingId(null);
    }
  };

  return (
    <div className="space-y-8">
      <div className="flex flex-wrap items-center justify-between gap-3">
        <div>
          <h1 className="text-3xl font-semibold">Admin stock control</h1>
          <p className="text-sm text-muted-foreground">Manage inventory and reservations in real time.</p>
        </div>
        <Button asChild variant="secondary" className="rounded-2xl">
          <Link href="/admin/reservations">Go to approvals</Link>
        </Button>
      </div>

      <Card className="rounded-3xl border border-border/60 bg-card/80 shadow-sm">
        <CardHeader>
          <CardTitle>Stock entries</CardTitle>
        </CardHeader>
        <CardContent className="space-y-3">
          {loading ? (
            <LoadingSkeleton rows={3} />
          ) : stock.length === 0 ? (
            <EmptyState title="No stock entries" description="Stock will appear once seeded." />
          ) : (
            stock.map((entry) => (
              <StockEditor
                key={`${entry.pharmacyId}-${entry.medicineId}`}
                entry={entry}
                disabled={savingKey === `${entry.pharmacyId}-${entry.medicineId}`}
                onSave={(quantity) => handleSave(entry, quantity)}
              />
            ))
          )}
        </CardContent>
      </Card>

      <Card className="rounded-3xl border border-border/60 bg-card/80 shadow-sm">
        <CardHeader>
          <CardTitle>Pending reservations</CardTitle>
        </CardHeader>
        <CardContent className="space-y-4">
          {loading ? (
            <LoadingSkeleton rows={2} />
          ) : pending.length === 0 ? (
            <EmptyState title="No pending reservations" description="Approvals will appear here." />
          ) : (
            pending.map((reservation) => (
              <div
                key={reservation.id}
                className="rounded-2xl border border-border/60 bg-background/60 p-4"
              >
                <div className="flex flex-wrap items-center justify-between gap-3">
                  <div>
                    <p className="text-sm font-medium">{reservation.medicineName}</p>
                    <p className="text-xs text-muted-foreground">
                      {reservation.pharmacyName} Â· {reservation.userFullName} ({reservation.userEmail})
                    </p>
                    <p className="text-xs text-muted-foreground">Qty: {reservation.quantity}</p>
                  </div>
                  <div className="flex gap-2">
                    <Button
                      size="sm"
                      className="rounded-xl"
                      disabled={processingId === reservation.id}
                      onClick={() => approve(reservation)}
                    >
                      Approve
                    </Button>
                    <Button
                      size="sm"
                      variant="secondary"
                      className="rounded-xl"
                      disabled={processingId === reservation.id}
                      onClick={() => setRejecting(reservation)}
                    >
                      Reject
                    </Button>
                  </div>
                </div>
              </div>
            ))
          )}
        </CardContent>
      </Card>

      <Dialog open={!!rejecting} onOpenChange={(open) => !open && setRejecting(null)}>
        <DialogContent className="rounded-3xl">
          <DialogHeader>
            <DialogTitle>Reject reservation</DialogTitle>
          </DialogHeader>
          <div className="space-y-4">
            <Textarea
              value={rejectReason}
              onChange={(event) => setRejectReason(event.target.value)}
              placeholder="Provide a reason"
              className="min-h-[100px]"
            />
            <Button className="w-full rounded-2xl" onClick={reject} disabled={!rejectReason.trim()}>
              Confirm rejection
            </Button>
          </div>
        </DialogContent>
      </Dialog>
    </div>
  );
}
