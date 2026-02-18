import { describe, expect, test, vi, beforeEach, afterEach } from "vitest";
import { render, screen, fireEvent, act } from "@testing-library/react";
import SearchPage from "@/app/search/page";
import type { SearchResponse } from "@/types";

vi.mock("@/hooks/useSignalR", () => ({
  useSignalR: () => {},
}));

const mockGet = vi.fn();
vi.mock("@/lib/api", () => ({
  default: {
    get: (...args: any[]) => mockGet(...args),
  },
}));

describe("SearchPage", () => {
  beforeEach(() => {
    vi.useFakeTimers();
    mockGet.mockReset();
  });

  afterEach(() => {
    vi.useRealTimers();
  });

  test("renders results from API", async () => {
    const data: SearchResponse = {
      results: [
        {
          medicineId: "m1",
          medicineName: "PainRelief Plus",
          category: "Pain Relief",
          activeIngredient: "Ibuprofen",
          pharmacyId: "p1",
          pharmacyName: "City Care Pharmacy",
          latitude: 30,
          longitude: 31,
          quantity: 5,
          availability: "InStock",
        },
      ],
      alternatives: [],
    };

    mockGet.mockResolvedValue({ data });

    render(<SearchPage />);
    const input = screen.getByPlaceholderText(/search ibuprofen/i);
    fireEvent.change(input, { target: { value: "Pain" } });

    await act(async () => {
      vi.runAllTimers();
    });

    vi.useRealTimers();

    expect(await screen.findByText("PainRelief Plus")).toBeInTheDocument();
  });
});
