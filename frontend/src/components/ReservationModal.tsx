"use client";

import * as React from "react";
import { z } from "zod";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { toast } from "sonner";
import { Button } from "@/components/ui/button";
import { Dialog, DialogContent, DialogDescription, DialogHeader, DialogTitle } from "@/components/ui/dialog";
import { Input } from "@/components/ui/input";
import api from "@/lib/api";
import { MedicineSearchResult } from "@/types";

const schema = (max: number) =>
  z.object({
    quantity: z.coerce.number().min(1, "Quantity must be at least 1").max(max, `Max ${max}`),
  });

type FormValues = {
  quantity: number;
};

export function ReservationModal({
  open,
  onOpenChange,
  medicine,
  onSuccess,
}: {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  medicine: MedicineSearchResult | null;
  onSuccess?: () => void;
}) {
  const maxQuantity = medicine?.quantity ?? 1;
  const form = useForm<FormValues>({
    resolver: zodResolver(schema(maxQuantity)),
    defaultValues: { quantity: 1 },
    mode: "onChange",
  });

  React.useEffect(() => {
    form.reset({ quantity: 1 });
  }, [medicine, form]);

  const onSubmit = async (values: FormValues) => {
    if (!medicine) return;
    try {
      await api.post("/reservations", {
        pharmacyId: medicine.pharmacyId,
        medicineId: medicine.medicineId,
        quantity: values.quantity,
      });
      toast.success("Reservation created");
      onOpenChange(false);
      onSuccess?.();
    } catch (error: any) {
      toast.error(error?.response?.data?.message ?? "Reservation failed");
    }
  };

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="rounded-3xl">
        <DialogHeader>
          <DialogTitle>Reserve medicine</DialogTitle>
          <DialogDescription>
            Confirm the quantity you need and submit the reservation.
          </DialogDescription>
        </DialogHeader>
        {medicine ? (
          <div className="space-y-4">
            <div className="rounded-2xl border border-border/60 bg-muted/40 p-4">
              <p className="font-medium">{medicine.medicineName}</p>
              <p className="text-sm text-muted-foreground">{medicine.pharmacyName}</p>
              <p className="text-xs text-muted-foreground">Available: {medicine.quantity}</p>
            </div>
            <form className="space-y-4" onSubmit={form.handleSubmit(onSubmit)}>
              <div className="space-y-2">
                <label className="text-xs font-medium text-muted-foreground">Quantity</label>
                <Input type="number" min={1} max={maxQuantity} {...form.register("quantity")} />
                {form.formState.errors.quantity && (
                  <p className="text-xs text-destructive">
                    {form.formState.errors.quantity.message}
                  </p>
                )}
              </div>
              <Button type="submit" className="w-full rounded-2xl" disabled={form.formState.isSubmitting}>
                {form.formState.isSubmitting ? "Reserving..." : "Confirm reservation"}
              </Button>
            </form>
          </div>
        ) : (
          <p className="text-sm text-muted-foreground">Select a medicine first.</p>
        )}
      </DialogContent>
    </Dialog>
  );
}
