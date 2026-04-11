# Stripe platebni flow (PhysioBook)

Tato uprava pridava platebni flow Stripe Checkout:

1. Uzivatel vytvori rezervaci.
2. Frontend ihned vytvori Stripe Checkout Session (`POST /api/bookings/{id}/checkout`).
3. Uzivatel je presmerovan na Stripe platebni stranku.
4. Stripe zavola webhook (`POST /api/webhooks/stripe`).
5. Backend overi podpis webhooku a aktualizuje stav rezervace:
   - `checkout.session.completed` -> `BookingStatus=Paid`, `PaymentStatus=Paid`
   - `checkout.session.expired` -> rezervace se zrusi (`BookingStatus=Cancelled`, `PaymentStatus=Expired`)

## Konfigurace

Backend (`PhysioBook/appsettings.Development.json` nebo env vars):

- `Stripe:SecretKey`
- `Stripe:PublishableKey`
- `Stripe:WebhookSecret`
- `Stripe:Currency` (default `czk`)
- `Stripe:FrontendBaseUrl` (default `http://localhost:5173`)

Docker env promene:

- `Stripe__SecretKey`
- `Stripe__PublishableKey`
- `Stripe__WebhookSecret`
- `Stripe__Currency`
- `Stripe__FrontendBaseUrl`

## Rychly lokalni test

1. Spust backend + frontend + db.
2. Vytvor rezervaci ve frontendu.
3. Ocekava se redirect na Stripe Checkout.
4. Spust Stripe CLI a forward webhooku na backend.

Priklad:

```zsh
stripe listen --forward-to http://localhost:5050/api/webhooks/stripe
```

Zobrazene `whsec_...` nastav do `Stripe:WebhookSecret`.

### Docker Compose start

```zsh
cd /Users/marioboss/diplomova-prace/demo-projekt/ukol-C-stripe/copilot
docker compose up -d --build
```

Frontend: `http://localhost:5173`

API: `http://localhost:5050`

### Stripe webhook listener

```zsh
stripe listen \
  --api-key "sk_test_REDACTED" \
  --forward-to http://localhost:5050/api/webhooks/stripe
```

Poznamka: pokud Stripe CLI vypise jiny `whsec_...` nez je v `docker-compose.yml`, prepis `Stripe__WebhookSecret` a restartuj backend kontejner.

## Overeni build/test

```zsh
dotnet test --nologo
cd physiobook-frontend
npm run build
```

