import Link from "next/link";
import { Button } from "@/components/ui/button";
import { Card, CardContent } from "@/components/ui/card";

export default function HomePage() {
  return (
    <div className="space-y-10">
      <section className="grid gap-8 lg:grid-cols-[1.1fr_0.9fr]">
        <div className="space-y-6">
          <p className="text-xs uppercase tracking-[0.3em] text-muted-foreground">Medicine Finder</p>
          <h1 className="text-4xl font-semibold leading-tight md:text-5xl">
            Real-time pharmacy inventory with reservation intelligence.
          </h1>
          <p className="text-base text-muted-foreground md:text-lg">
            Search live stock across pharmacies, reserve what you need instantly, and keep
            patients informed with automatic updates.
          </p>
          <div className="flex flex-wrap gap-3">
            <Button asChild className="rounded-2xl">
              <Link href="/search">Start searching</Link>
            </Button>
            <Button asChild variant="secondary" className="rounded-2xl">
              <Link href="/login">Login</Link>
            </Button>
          </div>
        </div>
        <Card className="rounded-3xl border border-border/60 bg-card/80 shadow-sm">
          <CardContent className="space-y-5 p-6">
            <div>
              <p className="text-sm font-medium">Live stock insights</p>
              <p className="text-xs text-muted-foreground">Powered by SignalR updates</p>
            </div>
            <div className="grid gap-3">
              {["Search by ingredient", "Reserve in seconds", "Approve reservations"].map((item) => (
                <div key={item} className="rounded-2xl border border-border/50 p-4">
                  <p className="text-sm font-medium">{item}</p>
                  <p className="text-xs text-muted-foreground">Designed for staff efficiency</p>
                </div>
              ))}
            </div>
          </CardContent>
        </Card>
      </section>
    </div>
  );
}
