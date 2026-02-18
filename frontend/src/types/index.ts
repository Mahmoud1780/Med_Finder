export type AuthResponse = {
  token: string;
  expiresAt: string;
  userId: string;
  email: string;
  fullName: string;
  role: string;
};

export type MedicineSearchResult = {
  medicineId: string;
  medicineName: string;
  category: string;
  activeIngredient: string;
  pharmacyId: string;
  pharmacyName: string;
  latitude: number;
  longitude: number;
  quantity: number;
  availability: "InStock" | "OutOfStock" | string | number;
  distanceKm?: number | null;
};

export type MedicineAlternative = {
  medicineId: string;
  name: string;
  category: string;
  activeIngredient: string;
  trendingScore: number;
};

export type SearchResponse = {
  results: MedicineSearchResult[];
  alternatives: MedicineAlternative[];
  message?: string | null;
};

export type PharmacyStock = {
  medicineId: string;
  medicineName: string;
  quantity: number;
};

export type PharmacyDetails = {
  id: string;
  name: string;
  latitude: number;
  longitude: number;
  stocks: PharmacyStock[];
};

export type Reservation = {
  id: string;
  userId: string;
  pharmacyId: string;
  medicineId: string;
  quantity: number;
  status: string;
  rejectionReason?: string | null;
  createdAt: string;
};

export type UserReservationItem = {
  id: string;
  medicineId: string;
  medicineName: string;
  pharmacyId: string;
  pharmacyName: string;
  quantity: number;
  status: string;
  rejectionReason?: string | null;
  createdAt: string;
};

export type PendingReservation = {
  id: string;
  userId: string;
  userFullName: string;
  userEmail: string;
  medicineId: string;
  medicineName: string;
  pharmacyId: string;
  pharmacyName: string;
  quantity: number;
  createdAt: string;
};

export type StockEntry = {
  pharmacyId: string;
  pharmacyName: string;
  medicineId: string;
  medicineName: string;
  quantity: number;
};

export type JwtUser = {
  id: string;
  email: string;
  fullName: string;
  role: string;
};

export type ReservationUpdatedEvent = {
  reservationId: string;
  userId: string;
  pharmacyId: string;
  medicineId: string;
  quantity: number;
  status: string;
  rejectionReason?: string | null;
  updatedAt: string;
};
