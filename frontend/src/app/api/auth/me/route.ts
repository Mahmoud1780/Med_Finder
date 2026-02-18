import { NextResponse } from "next/server";
import { getTokenFromCookies, toJwtUser, verifyJwt } from "@/lib/auth";

export const runtime = "nodejs";

export async function GET() {
  try {
    const token = await getTokenFromCookies();
    if (!token) {
      return NextResponse.json({ user: null }, { status: 401 });
    }

    const payload = await verifyJwt(token);
    return NextResponse.json({ user: toJwtUser(payload) });
  } catch {
    return NextResponse.json({ user: null }, { status: 401 });
  }
}