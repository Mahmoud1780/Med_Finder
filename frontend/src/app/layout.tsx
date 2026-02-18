import type { Metadata } from "next";
import { Sora, Space_Grotesk } from "next/font/google";
import "./globals.css";
import { ThemeProvider } from "@/components/theme-provider";
import { Navbar } from "@/components/Navbar";
import { TooltipProvider } from "@/components/ui/tooltip";
import { Toaster } from "@/components/ui/sonner";

const sora = Sora({
  subsets: ["latin"],
  variable: "--font-sora",
});

const space = Space_Grotesk({
  subsets: ["latin"],
  variable: "--font-space",
});

export const metadata: Metadata = {
  title: "Medicine Finder",
  description: "Real-time medicine availability with smart reservations.",
};

export default function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
    <html lang="en" suppressHydrationWarning>
      <body className={`${sora.variable} ${space.variable} antialiased`}>
        <ThemeProvider>
          <TooltipProvider>
            <div className="min-h-screen bg-[radial-gradient(circle_at_top,theme(colors.muted.DEFAULT),transparent_50%)]">
              <Navbar />
              <main className="mx-auto max-w-6xl px-4 py-8">{children}</main>
            </div>
            <Toaster richColors />
          </TooltipProvider>
        </ThemeProvider>
      </body>
    </html>
  );
}
