"use client";

import { useCallback, useEffect, useState } from "react";
import { toast } from "sonner";
import type { JwtUser } from "@/types";

export function useAuth() {
  const [user, setUser] = useState<JwtUser | null>(null);
  const [loading, setLoading] = useState(true);

  const fetchMe = useCallback(async () => {
    try {
      setLoading(true);
      const response = await fetch("/api/auth/me", { credentials: "include" });
      if (!response.ok) {
        setUser(null);
        return;
      }
      const data = await response.json();
      setUser(data.user ?? null);
    } finally {
      setLoading(false);
    }
  }, []);

  const logout = useCallback(async () => {
    await fetch("/api/auth/logout", { method: "POST" });
    toast.success("Logged out");
    setUser(null);
    window.location.href = "/login";
  }, []);

  useEffect(() => {
    void fetchMe();
  }, [fetchMe]);

  return { user, loading, refresh: fetchMe, logout };
}
