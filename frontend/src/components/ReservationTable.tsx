import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from "@/components/ui/table";
import { StatusBadge } from "@/components/StatusBadge";
import { UserReservationItem } from "@/types";

export function ReservationTable({ reservations }: { reservations: UserReservationItem[] }) {
  return (
    <div className="rounded-3xl border border-border/60 bg-card/80 shadow-sm">
      <Table>
        <TableHeader>
          <TableRow>
            <TableHead>Medicine</TableHead>
            <TableHead>Pharmacy</TableHead>
            <TableHead>Quantity</TableHead>
            <TableHead>Status</TableHead>
            <TableHead>Rejection</TableHead>
            <TableHead>Created</TableHead>
          </TableRow>
        </TableHeader>
        <TableBody>
          {reservations.map((reservation) => (
            <TableRow key={reservation.id}>
              <TableCell className="font-medium">{reservation.medicineName}</TableCell>
              <TableCell>{reservation.pharmacyName}</TableCell>
              <TableCell>{reservation.quantity}</TableCell>
              <TableCell>
                <StatusBadge value={reservation.status} />
              </TableCell>
              <TableCell className="text-muted-foreground">
                {reservation.rejectionReason ?? "—"}
              </TableCell>
              <TableCell className="text-muted-foreground">
                {new Date(reservation.createdAt).toLocaleString()}
              </TableCell>
            </TableRow>
          ))}
        </TableBody>
      </Table>
    </div>
  );
}
