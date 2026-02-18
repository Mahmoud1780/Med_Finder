import { describe, expect, test, vi } from "vitest";
import { render, screen, fireEvent } from "@testing-library/react";
import { ReservationModal } from "@/components/ReservationModal";
import type { MedicineSearchResult } from "@/types";

vi.mock("@/lib/api", () => ({
  default: {
    post: vi.fn(),
  },
}));

describe("ReservationModal", () => {
  test("validates quantity against stock", async () => {
    const medicine: MedicineSearchResult = {
      medicineId: "m1",
      medicineName: "PainRelief Plus",
      category: "Pain Relief",
      activeIngredient: "Ibuprofen",
      pharmacyId: "p1",
      pharmacyName: "City Care Pharmacy",
      latitude: 30,
      longitude: 31,
      quantity: 1,
      availability: "InStock",
    };

    render(
      <ReservationModal open={true} onOpenChange={() => {}} medicine={medicine} />
    );

    const input = screen.getByRole("spinbutton");
    fireEvent.change(input, { target: { value: "3" } });

    expect(await screen.findByText(/max 1/i)).toBeInTheDocument();
  });
});
