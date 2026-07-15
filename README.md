# EmailSaaS

**A multi-tenant Email Notification SaaS platform built with .NET 8**

EmailSaaS is a centrally hosted email service that allows multiple applications (tenants) to store their own email templates, connect their own email-sending accounts, and send tracked, dynamic emails through a single unified API — similar in concept to commercial transactional email platforms, but built entirely in-house with full control.

---

## 🚀 What is EmailSaaS?

EmailSaaS is **not an email provider itself** — it doesn't own or operate mail servers. It's an **orchestration and tracking platform**. Each client connects their own email-sending account (Gmail, Zoho, Office 365, or any SMTP/API-based provider) and manages their own templates. EmailSaaS provides the infrastructure to store, render, send, track, and webhook-notify those emails reliably — all behind a single unified API.

---

## ✨ Key Features

- **Multi-tenant architecture** — Applications → Clients hierarchy, with complete data isolation between tenants
- **API Key authentication** — every tenant gets a unique key; one key can never access another tenant's data
- **Dynamic email templates** — HTML templates with `{{Placeholder}}` variables, rendered at send-time with per-request parameter values
- **Multiple provider support** — SMTP (Gmail, Zoho, custom mail servers) and Microsoft Graph API (Office 365 / Outlook) out of the box, extensible to any provider
- **Built-in email tracking** — open tracking (invisible pixel), click tracking (link wrapping), delivery confirmation, bounce detection — with no third-party tracking dependency
- **Webhook subscriptions** — clients can register webhook endpoints to receive real-time event notifications (Delivered, Opened, Clicked, Bounced, Failed)
- **Webhook retry with background dispatch** — failed webhook deliveries are automatically retried using a background service with configurable retry logic
- **Bounce mailbox listener** — a background IMAP listener monitors a dedicated bounce mailbox and automatically records bounce events
- **AES-encrypted credentials** — all passwords, API keys, and client secrets are encrypted at rest; raw credentials are never returned from the API
- **Clean Architecture + CQRS** — built with MediatR, FluentValidation, and a clear separation of concerns
- **Full test coverage** — unit tests (handlers, validators) and integration tests (controllers, API key enforcement)
- **Environment-aware security** — Swagger and admin endpoints automatically disabled in production

---

## 🏗️ Architecture

Built using **Clean Architecture** with **CQRS** via **MediatR**.

```
EmailSaas.Domain          → Core entities and enums. Zero external dependencies.
EmailSaas.Application     → Business logic — Commands, Queries, Handlers,
                             DTOs, Validators, Interfaces
EmailSaas.Infrastructure  → EF Core persistence, email sending services,
                             encryption, tracking, webhook dispatch, bounce listener
EmailSaas.API             → Controllers, middleware, Swagger, API entry point
```

**Dependency rule:** Domain has no dependencies. Application depends only on Domain. Infrastructure implements Application's interfaces. API wires everything together via dependency injection.

---

## 🛠️ Tech Stack

| Layer | Technology |
|---|---|
| Framework | .NET 8 (LTS) |
| ORM | Entity Framework Core 8 — SQL Server, Code-First Migrations |
| Messaging | MediatR (CQRS pattern) |
| Validation | FluentValidation |
| SMTP Sending | MailKit / MimeKit |
| Microsoft 365 Sending | Microsoft Graph SDK + Azure.Identity (OAuth2) |
| Encryption | AES-256 (custom AesEncryptionService) |
| Bounce Detection | IMAP via MailKit (BounceMailboxListenerService) |
| Background Jobs | IHostedService (Webhook dispatch, Bounce listener) |
| API Docs | Swashbuckle (Swagger UI — Development only) |
| Testing | xUnit, Moq, FluentAssertions |

---

## 📁 Project Structure

```
EmailSaas/
├── EmailSaas.Domain/
│   ├── Entities/
│   │   ├── ApplicationMaster.cs       → Top-level tenant entity
│   │   ├── ClientMaster.cs            → Client under an Application
│   │   ├── EmailProviderConfig.cs     → Encrypted sending credentials per client
│   │   ├── EmailTemplateMaster.cs     → HTML templates with placeholders
│   │   ├── EmailLog.cs                → Full audit log per sent email
│   │   ├── EmailEvent.cs              → Individual tracking events (open/click/bounce etc.)
│   │   ├── EmailLinkClick.cs          → Per-link click detail record
│   │   ├── WebhookSubscription.cs     → Client webhook endpoint registrations
│   │   └── WebhookDeliveryLog.cs      → Delivery attempts + retry log per webhook
│   └── Enums/
│       ├── EmailSendStatus.cs         → Pending, Sent, Delivered, Opened, Clicked, Bounced, Failed
│       ├── EmailEventType.cs          → Delivered, Opened, Clicked, Bounced, Failed
│       ├── WebhookEventType.cs        → Events that trigger webhook dispatch
│       └── CommonStatus.cs            → Active / Inactive
│
├── EmailSaas.Application/
│   ├── Common/                        → Result<T>, interfaces (IApplicationDbContext, IWebhookDispatcher, etc.)
│   ├── DTOs/                          → Response DTOs per feature
│   └── Features/
│       ├── Applications/              → CRUD for ApplicationMaster
│       ├── Clients/                   → CRUD for ClientMaster
│       ├── EmailProviders/            → CRUD + credential encryption for EmailProviderConfig
│       ├── EmailTemplates/            → CRUD for EmailTemplateMaster
│       ├── SendEmail/                 → Core send-email command (template render + provider dispatch)
│       ├── EmailLogs/                 → Query email logs by ID or list
│       ├── Tracking/
│       │   ├── RecordEmailDelivered/  → Mark delivered, set DeliveredAt, fire webhook
│       │   ├── RecordEmailOpen/       → Record open pixel hit, set OpenedAt / LastOpenedAt / OpenCount
│       │   ├── RecordEmailClick/      → Record link click, set ClickedAt / LastClickedAt / ClickCount
│       │   ├── RecordEmailBounced/    → Record bounce, set BouncedAt / BounceReason
│       │   └── RecordEmailFailed/     → Record send failure, set ErrorMessage
│       └── Webhooks/                  → Create / list webhook subscriptions
│
├── EmailSaas.Infrastructure/
│   ├── Persistence/                   → EF Core DbContext + entity configurations + migrations
│   └── Services/
│       ├── EmailSenderService.cs      → Routes to SMTP or Graph based on provider config
│       ├── EmailTrackingService.cs    → Injects pixel + rewrites links for tracking
│       ├── AesEncryptionService.cs    → Encrypts/decrypts provider credentials
│       ├── WebhookDispatcher.cs       → Queues webhook events for dispatch
│       ├── WebhookDispatchBackgroundService.cs → Background retry loop for webhook delivery
│       ├── BounceMailboxListenerService.cs     → IMAP listener for bounce detection
│       └── WebhookSettings.cs         → Webhook configuration model
│
└── EmailSaas.API/
    ├── Controllers/
    │   ├── ApplicationsController.cs      → Manage Applications
    │   ├── ClientsController.cs           → Manage Clients
    │   ├── EmailProvidersController.cs    → Manage provider configs
    │   ├── EmailTemplatesController.cs    → Manage templates
    │   ├── SendEmailController.cs         → Core send-email endpoint
    │   ├── EmailLogsController.cs         → Query sent email logs
    │   ├── TrackController.cs             → Open pixel / click redirect / delivered / bounced / provider webhook receiver
    │   ├── WebhookSubscriptionsController.cs → Manage webhook subscriptions
    │   └── TestReceiverController.cs      → Development-only webhook test receiver
    ├── Middleware/
    │   ├── ApiKeyMiddleware.cs            → Per-request tenant authentication
    │   └── WebhookSignatureValidationMiddleware.cs → Validates signed webhook payloads
    └── Program.cs                         → App bootstrap, DI, middleware pipeline
```

---

## 📖 How It Works

### The Tenant Model

```
EmailSaaS (Platform)
│
├── Application: AppA  (registers, gets an API Key)
│   ├── Client: Client1  → own email provider + own templates
│   └── Client: Client2  → own email provider + own templates
│
└── Application: AppB  (registers, gets its own API Key)
    └── Client: Client3  → own email provider + own templates
```

- An **Application** is a top-level tenant.
- Each Application can have multiple **Clients** underneath it.
- Each Client configures their **own** sending provider and manages their **own** templates.
- One Application's data can never be seen or affected by another Application's API key.

### The Sending Flow

1. An Application registers and configures a Client, an email provider, and one or more templates.
2. The Application's backend calls `POST /api/send-email` with a template code, recipient address, and dynamic parameter values.
3. EmailSaaS resolves the correct template, renders it with the provided parameters, injects the open-tracking pixel and rewrites all links for click tracking, then dispatches the email through the client's configured provider (SMTP or Microsoft Graph).
4. Every send is logged in `EmailLog`. Open, click, delivery, and bounce events are tracked automatically and stored per-event in `EmailEvent`.
5. If a webhook subscription exists for the tenant, a webhook notification is dispatched for each event. Failed deliveries are automatically retried by the background service.

### Email Tracking Events

| Event | How It's Triggered | What Gets Updated |
|---|---|---|
| **Delivered** | Provider webhook callback to `/api/track/events` | `DeliveredAt`, `Status = Delivered` |
| **Opened** | Invisible pixel request to `/api/track/open/{messageId}` | `OpenedAt`, `LastOpenedAt`, `OpenCount` |
| **Clicked** | Link redirect via `/api/track/click/{messageId}?url=...` | `ClickedAt`, `LastClickedAt`, `ClickCount` |
| **Bounced** | IMAP bounce listener or `/api/track/bounced` callback | `BouncedAt`, `BounceReason` |
| **Failed** | Send-time failure or `/api/track/failed` callback | `ErrorMessage`, `Status = Failed` |

### Supported Email Providers

- **SMTP** — works with Gmail, Zoho, custom mail servers, and any standard SMTP account
- **Microsoft Graph API** — OAuth2-based sending for Office 365 and Outlook accounts (used because Microsoft deprecated SMTP Basic Auth across its mail products)

Provider routing is automatic — based on which credentials are configured on the client record.

---

## 🔌 API Endpoints

All endpoints (except tracking pixel and click redirect) require an `X-Api-Key` header.

| Method | Endpoint | Description |
|---|---|---|
| `POST` | `/api/send-email` | Send a tracked, templated email |
| `GET` | `/api/emaillogs` | List all email logs for the tenant |
| `GET` | `/api/emaillogs/{id}` | Get a single email log by ID |
| `GET` | `/api/track/open/{messageId}` | Open tracking pixel (no auth required) |
| `GET` | `/api/track/click/{messageId}?url=...` | Click tracking redirect (no auth required) |
| `POST` | `/api/track/delivered` | Manually mark an email as delivered |
| `POST` | `/api/track/bounced` | Manually record a bounce |
| `POST` | `/api/track/failed` | Manually record a send failure |
| `POST` | `/api/track/events` | Provider webhook receiver (delivered + bounce events) |
| `GET/POST` | `/api/applications` | Manage Applications |
| `GET/POST` | `/api/clients` | Manage Clients |
| `GET/POST` | `/api/emailproviders` | Manage email provider configurations |
| `GET/POST` | `/api/emailtemplates` | Manage email templates |
| `GET/POST` | `/api/webhooksubscriptions` | Manage webhook endpoint subscriptions |

---

## 🔒 Security

- All sensitive credentials (passwords, API keys, OAuth secrets) are **AES-256 encrypted** before being stored in the database
- API responses **never expose raw credentials** back to the caller
- Every request is authenticated via a unique **API key**, and every query is automatically scoped to the calling tenant only
- Global exception handling ensures no internal errors or stack traces are ever leaked in API responses
- In **production**, Swagger UI and all admin endpoints are automatically disabled — only the send-email and tracking endpoints are publicly reachable
- Webhook payloads are protected via **HMAC signature validation**

---

## 🧪 Testing

The solution includes both unit and integration test coverage:

- Unit tests for command handlers and validators using mocked dependencies
- Integration tests exercising full controller behavior, including API key enforcement, using an in-memory test database

```bash
dotnet test
```

---

## 📦 Getting Started

### Prerequisites

- .NET 8 SDK
- SQL Server (any edition)
- A sending account — Gmail / Zoho / Office 365 / any SMTP

### Setup

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd EmailSaas
   ```

2. **Configure `appsettings.json`** (see section below — do NOT commit real values)

3. **Apply database migrations**
   ```bash
   dotnet ef database update --project EmailSaas.Infrastructure --startup-project EmailSaas.API
   ```

4. **Run the API**
   ```bash
   dotnet run --project EmailSaas.API
   ```
   In Development mode, Swagger UI is available at `https://localhost:{port}/swagger`

5. **Register your first Application** via the Applications endpoint to receive an API key

6. **Create a Client**, configure an **Email Provider**, and create your first **Template**

7. **Start sending** via `POST /api/send-email`

### Configuration Keys (`appsettings.json`)

> ⚠️ Never commit real values. Use environment variables or a secrets manager in production.

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "<your-sql-server-connection-string>"
  },
  "EncryptionSettings": {
    "Key": "<32-character-AES-key>",
    "IV":  "<16-character-AES-IV>"
  },
  "TrackingSettings": {
    "BaseUrl": "<your-public-api-base-url>"
  }
}
```

---

## 🔔 Webhook Integration

Clients can register a webhook endpoint to receive real-time notifications when email events occur.

**Register a webhook:**
```http
POST /api/webhooksubscriptions
X-Api-Key: <your-api-key>

{
  "clientId": 1,
  "targetUrl": "https://your-app.com/webhook/email-events",
  "secret": "your-signing-secret",
  "events": ["Delivered", "Opened", "Clicked", "Bounced", "Failed"]
}
```

**Inbound provider webhook (for delivery confirmation):**
```http
POST /api/track/events

[
  {
    "event": "delivered",
    "message_id": "MSG-XXXXXX",
    "timestamp": 1721035541
  }
]
```

---

## 🗺️ Roadmap

- [ ] Bulk and scheduled email sending
- [ ] Template versioning and A/B testing
- [ ] Rate limiting and usage-based plans
- [ ] Unsubscribe / opt-out management for compliance
- [ ] Analytics dashboard for open/click/bounce rates
- [ ] Additional native provider integrations

---

## 📄 License

This project is proprietary. All rights reserved.
