# Úkol C: Stripe integrace — Prompt (identický pro všechny nástroje)

Implementuj platební bránu Stripe v rezervačním systému. Požadavky: 1) Stripe Checkout Session — po vytvoření rezervace přesměrovat na Stripe platební stránku. 2) Webhook endpoint (POST /api/webhooks/stripe) — zpracování událostí: checkout.session.completed → aktualizace stavu rezervace na Paid, checkout.session.expired → zrušení rezervace. 3) Propojení s existujícím BookingEntity — přidat PaymentStatus, StripeSessionId, StripePaymentIntentId. 4) Konfigurace: Stripe API keys v appsettings.json. 5) Frontend: tlačítko Zaplatit u neplacené rezervace → redirect na Stripe Checkout. 6) Webhook signature verification.

Výstup: funkční platební flow od vytvoření rezervace po potvrzení platby.
