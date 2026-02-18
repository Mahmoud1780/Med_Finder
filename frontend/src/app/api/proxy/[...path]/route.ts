import { NextRequest, NextResponse } from "next/server";
import { COOKIE_NAME } from "@/lib/auth";

export const runtime = "nodejs";

type RouteContext = { params: Promise<{ path?: string[] }> };

async function proxy(request: NextRequest, context: RouteContext) {
  const baseUrl = process.env.API_BASE_URL || "";
  if (!baseUrl) {
    return NextResponse.json({ message: "API_BASE_URL is not configured" }, { status: 500 });
  }

  const resolved = await context.params;
  const segments = resolved?.path ?? [];
  const path = Array.isArray(segments) ? segments.join("/") : "";
  const url = new URL(request.url);
  const targetUrl = `${baseUrl}/api/v1/${path}${url.search}`;

  const headers = new Headers(request.headers);
  headers.delete("host");

  const token = request.cookies.get(COOKIE_NAME)?.value;
  if (token) {
    headers.set("Authorization", `Bearer ${token}`);
  }

  const body = request.method === "GET" || request.method === "HEAD" ? undefined : await request.arrayBuffer();

  const response = await fetch(targetUrl, {
    method: request.method,
    headers,
    body,
  });

  const responseBody = await response.arrayBuffer();
  const nextResponse = new NextResponse(responseBody, {
    status: response.status,
  });
  const contentType = response.headers.get("content-type");
  if (contentType) {
    nextResponse.headers.set("content-type", contentType);
  }
  return nextResponse;
}

export async function GET(request: NextRequest, context: RouteContext) {
  return proxy(request, context);
}

export async function POST(request: NextRequest, context: RouteContext) {
  return proxy(request, context);
}

export async function PUT(request: NextRequest, context: RouteContext) {
  return proxy(request, context);
}

export async function PATCH(request: NextRequest, context: RouteContext) {
  return proxy(request, context);
}

export async function DELETE(request: NextRequest, context: RouteContext) {
  return proxy(request, context);
}