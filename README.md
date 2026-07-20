# EmailSaaS

**A multi-tenant Email Notification SaaS platform built with .NET 10**

EmailSaaS is a centrally hosted email service that allows multiple applications (tenants) to store their own email templates, connect their own email-sending accounts, and send tracked, dynamic emails through a single unified API — similar in concept to commercial transactional email platforms, but built entirely in-house with full control.

> 📌 **This is a public showcase repository.** It contains architecture, documentation, and API reference for the project. The full source code (business logic, database, provider integrations, secrets) lives in a private repository. Happy to walk through the implementation or grant limited access on request.

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

Built using **Clean Architecture** with **CQRS** via **MediatR** — with clear separation between domain, application, infrastructure, and API layers, following the standard dependency rule (inner layers have no knowledge of outer layers).

*(Full internal project/folder structure is kept in the private repo.)*

---

## 🛠️ Tech Stack

| Layer | Technology |
|---|---|
| Framework | .NET 10 (LTS) |
| ORM | Entity Framework Core 8 — SQL Server, Code-First Migrations |
| Messaging | MediatR (CQRS pattern) |
| Validation | FluentValidation |
| SMTP Sending | MailKit / MimeKit |
| Microsoft 365 Sending | Microsoft Graph SDK + Azure.Identity (OAuth2) |
| Encryption | AES-256 (custom AesEncryptionService) |
| Bounce Detection | IMAP via MailKit |
| Background Jobs | IHostedService (Webhook dispatch, Bounce listener) |
| API Docs | Swashbuckle (Swagger UI — Development only) |
| Testing | xUnit, Moq, FluentAssertions |

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
4. Every send is logged. Open, click, delivery, and bounce events are tracked automatically and stored per-event.
5. If a webhook subscription exists for the tenant, a webhook notification is dispatched for each event. Failed deliveries are automatically retried by the background service.

### Email Tracking Events

| Event | How It's Triggered | What Gets Updated |
|---|---|---|
| **Delivered** | Provider webhook callback | `DeliveredAt`, `Status = Delivered` |
| **Opened** | Invisible pixel request | `OpenedAt`, `LastOpenedAt`, `OpenCount` |
| **Clicked** | Link redirect | `ClickedAt`, `LastClickedAt`, `ClickCount` |
| **Bounced** | IMAP bounce listener or callback | `BouncedAt`, `BounceReason` |
| **Failed** | Send-time failure or callback | `ErrorMessage`, `Status = Failed` |

### Supported Email Providers

- **SMTP** — works with Gmail, Zoho, custom mail servers, and any standard SMTP account
- **Microsoft Graph API** — OAuth2-based sending for Office 365 and Outlook accounts (used because Microsoft deprecated SMTP Basic Auth across its mail products)

Provider routing is automatic — based on which credentials are configured on the client record.

---

## 🔌 API Reference (Sample)

For end users integrating with EmailSaaS, only the sending API is required. The service owner provides a `clientId`, `applicationId`, email templates, and an `X-Api-Key`.

### Send Email

**Endpoint:** `POST /api/send-email`

#### Headers

| Header | Type | Description | Required |
|---|---|---|---|
| `X-Api-Key` | string | Unique API key provided by the service owner. | Yes |
| `Content-Type` | string | Must be `application/json` | Yes |

#### Request Body (JSON)

| Property | Type | Description | Required |
|---|---|---|---|
| `applicationId` | integer | Provided Application ID. | Yes |
| `clientId` | integer | Provided Client ID. | Yes |
| `templateCode` | string | Code for the email template to send. | Yes |
| `toEmail` | string | Recipient's email address. | Yes |
| `ccEmail` | string | Optional CC email address. | No |
| `bccEmail` | string | Optional BCC email address. | No |
| `parameters` | dictionary | Key-value pairs matching `{{Placeholder}}` variables in the template. | Yes |
| `createdBy` | string | Identifier for the user/system triggering the email. | Yes |

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
  "toEmail": "test@example.com",
  "ccEmail": "sales@example.com",
  "bccEmail": "",
  "parameters": {
    "FirstName": "John",
    "AccountLink": "https://myapp.com/login"
  },
  "createdBy": "SystemUser"
}
```

#### Example Response

```json
{
  "success": true,
  "messageId": "MSG-20260720-XXXXXX",
  "status": "Sent"
}
```

---

## 🔒 Security

- All sensitive credentials (passwords, API keys, OAuth secrets) are **AES-256 encrypted** before being stored
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

---

## 🗺️ Roadmap

- [ ] Bulk and scheduled email sending
- [ ] Template versioning and A/B testing
- [ ] Rate limiting and usage-based plans
- [ ] Unsubscribe / opt-out management for compliance
- [ ] Analytics dashboard for open/click/bounce rates
- [ ] Additional native provider integrations

---

## 📬 Contact

Interested in the full implementation, a code walkthrough, licensing, or private repo access?

📧 hemanthkv23@outlook.com
📧 hemanthshetty346@gmail.com

---

## 📄 License

This project is proprietary. All rights reserved. Source code is not included in this repository.
