import { NextRequest, NextResponse } from "next/server";
import { jwtVerify } from "jose";

const COOKIE_NAME = "mf_token";

function extractRole(payload: Record<string, unknown>) {
  const roleClaim =
    payload["role"] ??
    payload["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"];
  if (Array.isArray(roleClaim)) {
    return roleClaim[0] ?? "";
  }
  return (roleClaim as string) ?? "";
}

const protectedRoutes = ["/search", "/pharmacies", "/me", "/admin"];
const adminRoutes = ["/admin"];

export async function middleware(request: NextRequest) {
  const { pathname } = request.nextUrl;

  if (
    pathname.startsWith("/api") ||
    pathname.startsWith("/_next") ||
    pathname.startsWith("/favicon") ||
    pathname.startsWith("/login")
  ) {
    return NextResponse.next();
  }

  const needsAuth = protectedRoutes.some((route) => pathname.startsWith(route));
  if (!needsAuth) {
    return NextResponse.next();
  }

  const token = request.cookies.get(COOKIE_NAME)?.value;
  if (!token) {
    return NextResponse.redirect(new URL("/login", request.url));
  }

  try {
    const secret = new TextEncoder().encode(process.env.JWT_SECRET || "");
    const { payload } = await jwtVerify(token, secret);
    const role = extractRole(payload);

    if (adminRoutes.some((route) => pathname.startsWith(route)) && role !== "Admin") {
      return NextResponse.redirect(new URL("/search", request.url));
    }

    return NextResponse.next();
  } catch {
    return NextResponse.redirect(new URL("/login", request.url));
  }
}

export const config = {
  matcher: ["/((?!_next|favicon.ico).*)"],
};
