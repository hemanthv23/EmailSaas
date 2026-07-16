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
│
├── EmailSaas.Domain/                          → Core business entities, zero external dependencies
│   ├── Common/
│   │   └── AuditableEntity.cs                 → Base class with CreatedBy, CreatedDate, UpdatedBy, UpdatedDate
│   ├── Constants/
│   │   └── EmailProviderConstants.cs          → Provider name constants (SMTP, Graph, etc.)
│   ├── Entities/
│   │   ├── ApplicationMaster.cs               → Top-level tenant entity
│   │   ├── ClientMaster.cs                    → Client under an Application
│   │   ├── EmailProviderConfig.cs             → Encrypted sending + IMAP credentials per client
│   │   ├── EmailTemplateMaster.cs             → HTML templates with {{Placeholder}} variables
│   │   ├── EmailLog.cs                        → Full audit log per sent email
│   │   ├── EmailEvent.cs                      → Individual tracking events (open/click/bounce etc.)
│   │   ├── EmailLinkClick.cs                  → Per-link click detail record
│   │   ├── WebhookSubscription.cs             → Client webhook endpoint registrations
│   │   └── WebhookDeliveryLog.cs              → Webhook delivery attempts + retry log
│   └── Enums/
│       ├── EmailSendStatus.cs                 → Pending, Sent, Delivered, Opened, Clicked, Bounced, Failed
│       ├── EmailEventType.cs                  → Delivered, Opened, Clicked, Bounced, Failed, Sent
│       ├── WebhookEventType.cs                → Events that trigger webhook dispatch
│       └── CommonStatus.cs                    → Active / Inactive
│
├── EmailSaas.Application/                     → Business logic — CQRS, Handlers, Validators, Interfaces
│   ├── Common/
│   │   ├── Behaviors/                         → MediatR pipeline behaviors (e.g., Validation behavior)
│   │   ├── Exceptions/                        → Custom exception types
│   │   ├── Interfaces/                        → IApplicationDbContext, IEmailSenderService,
│   │   │                                          IEmailTrackingService, IEncryptionService,
│   │   │                                          IWebhookDispatcher
│   │   └── Models/                            → Result<T> wrapper
│   ├── DTOs/                                  → Response DTOs per feature area
│   └── Features/
│       ├── Applications/                      → Create / Get ApplicationMaster
│       ├── Clients/                           → Create / Get ClientMaster
│       ├── EmailProviders/                    → Create / Get EmailProviderConfig (with encryption)
│       ├── EmailTemplates/                    → Create / Get EmailTemplateMaster
│       ├── EmailLogs/                         → Fetch all logs / fetch by ID
│       ├── SendEmail/                         → Core send command (render template → inject tracking → dispatch)
│       ├── Tracking/
│       │   └── Commands/
│       │       ├── RecordEmailDelivered/      → Set DeliveredAt, fire Delivered webhook
│       │       ├── RecordEmailOpen/           → Set OpenedAt / LastOpenedAt / OpenCount, fire Opened webhook
│       │       ├── RecordEmailClick/          → Set ClickedAt / LastClickedAt / ClickCount, fire Clicked webhook
│       │       ├── RecordEmailBounced/        → Set BouncedAt / BounceReason, fire Bounced webhook
│       │       └── RecordEmailFailed/         → Set ErrorMessage / Status = Failed, fire Failed webhook
│       └── Webhooks/                          → Create / list WebhookSubscriptions
│
├── EmailSaas.Infrastructure/                  → EF Core, email sending, encryption, background services
│   ├── Persistence/
│   │   ├── AppDbContext.cs                    → EF Core DbContext
│   │   └── Configurations/                   → Entity type configurations (Fluent API per entity)
│   └── Services/
│       ├── EmailSenderService.cs              → Routes to SMTP (MailKit) or Graph API based on provider
│       ├── EmailTrackingService.cs            → Injects open pixel + rewrites links for click tracking
│       ├── AesEncryptionService.cs            → AES-256 encrypt/decrypt for credentials at rest
│       ├── WebhookDispatcher.cs               → Queues webhook events for background dispatch
│       ├── WebhookDispatchBackgroundService.cs→ Background retry loop for webhook delivery
│       ├── BounceMailboxListenerService.cs    → IMAP listener — supports OAuth2 (Graph) + Basic Auth (SMTP)
│       └── WebhookSettings.cs                 → Webhook configuration model
│
├── EmailSaas.API/                             → HTTP entry point, middleware, controllers
│   ├── Controllers/
│   │   ├── ApplicationsController.cs          → Manage Applications
│   │   ├── ClientsController.cs               → Manage Clients
│   │   ├── EmailProvidersController.cs        → Manage provider configs
│   │   ├── EmailTemplatesController.cs        → Manage templates
│   │   ├── SendEmailController.cs             → Core send-email endpoint
│   │   ├── EmailLogsController.cs             → Query sent email logs
│   │   ├── TrackController.cs                 → Open pixel / click redirect / delivered / bounced / events
│   │   ├── WebhookSubscriptionsController.cs  → Manage webhook subscriptions
│   │   └── TestReceiverController.cs          → Dev-only webhook test receiver
│   ├── Middleware/
│   │   ├── ApiKeyMiddleware.cs                → Per-request tenant authentication via X-Api-Key header
│   │   ├── ExceptionHandlingMiddleware.cs     → Global exception handler, hides stack traces
│   │   ├── WebhookSignatureValidationMiddleware.cs → HMAC signature validation for inbound webhooks
│   │   ├── ErrorResponse.cs                   → Standard error response model
│   │   └── MiddlewareExtensions.cs            → Extension methods to register all middleware
│   ├── Swagger/
│   │   └── ConfigureSwaggerOptions.cs         → Swagger API key header + documentation config
│   ├── appsettings.json                       → Production configuration (hosted DB, tracking base URL)
│   ├── appsettings.Development.json           → Local development configuration (local DB, localhost URL)
│   ├── appsettings.Example.json               → Safe template to commit — no real secrets
│   └── Program.cs                             → App bootstrap, DI registration, middleware pipeline
│
└── EmailSaas.Shared/                          → Shared utilities (reserved for cross-layer use)
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

For end users integrating with EmailSaaS, only the sending API is required. The service developer or owner will provide you with your `clientId`, `applicationId` (Application Code), email templates, and an `X-Api-Key` to be included in the headers.

### Send Email

Send a tracked, templated email.

**Endpoint:** `POST /api/send-email`

#### Headers

| Header | Type | Description | Required |
|---|---|---|---|
| `X-Api-Key` | string | Your unique API key provided by the service owner. | Yes |
| `Content-Type` | string | Must be `application/json` | Yes |

#### Request Body (JSON)

| Property | Type | Description | Required |
|---|---|---|---|
| `applicationId` | integer | Your provided Application ID. | Yes |
| `clientId` | integer | Your provided Client ID. | Yes |
| `templateCode` | string | The code for the email template you wish to send. | Yes |
| `toEmail` | string | The recipient's email address. | Yes |
| `ccEmail` | string | Optional CC email address. | No |
| `bccEmail` | string | Optional BCC email address. | No |
| `parameters` | dictionary | Key-value pairs matching the `{{Placeholder}}` variables in your template. | Yes |
| `createdBy` | string | Identifier for the user or system triggering the email. | Yes |

#### Example Request

```json
POST /api/send-email
Host: api.emailsaas.yourdomain.com
X-Api-Key: your_provided_api_key
Content-Type: application/json

{
  "applicationId": 1,
  "clientId": 101,
  "templateCode": "WELCOME_EMAIL",
  "toEmail": "customer@example.com",
  "ccEmail": "sales@example.com",
  "bccEmail": "",
  "parameters": {
    "FirstName": "John",
    "AccountLink": "https://myapp.com/login"
  },
  "createdBy": "SystemUser"
}
```

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

