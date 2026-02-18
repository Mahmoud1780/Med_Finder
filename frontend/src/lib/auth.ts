import { cookies } from "next/headers";
import { jwtVerify } from "jose";
import type { JwtPayload } from "jose";
import { JwtUser } from "@/types";

export const COOKIE_NAME = "mf_token";

const encoder = new TextEncoder();

export async function verifyJwt(token: string): Promise<JwtPayload> {
  const secret = encoder.encode(process.env.JWT_SECRET || "");
  const { payload } = await jwtVerify(token, secret);
  return payload;
}

export async function getTokenFromCookies(): Promise<string | null> {
  const store = await cookies();
  const token = store.get(COOKIE_NAME)?.value;
  return token ?? null;
}

export function extractRole(payload: JwtPayload): string {
  const roleClaim =
    payload["role"] ??
    payload["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"];
  if (Array.isArray(roleClaim)) {
    return roleClaim[0] ?? "";
  }
  return (roleClaim as string) ?? "";
}

export function toJwtUser(payload: JwtPayload): JwtUser {
  return {
    id:
      (payload["sub"] as string) ||
      (payload["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"] as string) ||
      "",
    email:
      (payload["email"] as string) ||
      (payload["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress"] as string) ||
      "",
    fullName:
      (payload["name"] as string) ||
      (payload["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"] as string) ||
      "",
    role: extractRole(payload),
  };
}