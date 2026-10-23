# Shared base: Node 22 on Alpine, the working dir for every stage
FROM node:22-alpine AS base
WORKDIR /app

# Install deps first so this layer is cached until package files change
FROM base AS deps
COPY package.json package-lock.json ./
RUN npm ci

# Build the app; output: "standalone" in next.config.ts creates .next/standalone
FROM base AS builder
COPY --from=deps /app/node_modules ./node_modules
COPY . .
ENV NEXT_TELEMETRY_DISABLED=1
RUN npm run build

# Final image: only runtime files, no source or dev dependencies
FROM base AS runner
ENV NODE_ENV=production NEXT_TELEMETRY_DISABLED=1
# HOSTNAME=0.0.0.0 lets the host reach the server through the published port
ENV PORT=3000 HOSTNAME=0.0.0.0
# Run as a non-root user for safety
RUN addgroup --system --gid 1001 nodejs && adduser --system --uid 1001 nextjs
COPY --from=builder /app/public ./public
# Standalone output omits public/ and .next/static/, so copy them separately
COPY --from=builder --chown=nextjs:nodejs /app/.next/standalone ./
COPY --from=builder --chown=nextjs:nodejs /app/.next/static ./.next/static
USER nextjs
EXPOSE 3000
CMD ["node", "server.js"]