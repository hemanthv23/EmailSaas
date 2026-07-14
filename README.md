# EmailSaaS

**A multi-tenant Email Notification SaaS platform built with .NET 8**

EmailSaaS is a centrally hosted email service that allows multiple applications (tenants) to store their own email templates, connect their own email-sending accounts, and send tracked, dynamic emails through a single unified API — similar in concept to SendGrid or Mailchimp, but built entirely in-house.

---

## 🚀 What is EmailSaaS?

EmailSaaS is **not an email provider itself** — it doesn't own or operate mail servers. It's an **orchestration platform**. Each client connects their own email-sending account (Gmail, Zoho, Office 365, or any SMTP/API-based provider) and their own templates. EmailSaaS provides the infrastructure to store, render, send, and track those emails reliably.

Think of it like Twilio or Postman for email — it orchestrates sending through whatever provider a client already has, rather than being the provider itself.

---

## ✨ Key Features

- **Multi-tenant architecture** — Applications → Clients, with complete data isolation between tenants
- **API Key authentication** — every tenant gets a unique key; one key can never see another tenant's data
- **Dynamic email templates** — HTML templates with `{{Placeholder}}` variables, rendered at send-time
- **Multiple provider support** — SMTP (Gmail, Zoho, custom mail servers) and Microsoft Graph API (Office 365/Outlook) out of the box, extensible to any provider
- **Built-in email tracking** — open tracking (invisible pixel) and click tracking (link wrapping), with no third-party dependency
- **AES-encrypted credentials** — all passwords, API keys, and client secrets are encrypted at rest
- **Clean Architecture + CQRS** — built with MediatR, FluentValidation, and a clear separation of concerns
- **Full test coverage** — unit tests (handlers, validators) and integration tests (controllers, API key enforcement)
- **Environment-aware security** — Swagger and admin endpoints automatically disabled in production, leaving only the send-email API publicly exposed

---

## 🏗️ Architecture

Built using **Clean Architecture** with **CQRS** via **MediatR**.

```
EmailSaaS.Domain          → Core entities and enums, zero dependencies
EmailSaaS.Application     → Business logic — Commands, Queries, Handlers,
                             DTOs, Validators, Interfaces
EmailSaaS.Infrastructure  → EF Core persistence, email sending services,
                             encryption, tracking logic
EmailSaaS.API             → Controllers, middleware, API entry point
```

**Dependency rule:** Domain has no dependencies. Application depends only on Domain. Infrastructure implements Application's interfaces. API wires everything together.

---

## 🛠️ Tech Stack

- **.NET 8** (LTS)
- **Entity Framework Core 8** with SQL Server (Code-First Migrations)
- **CQRS** via **MediatR**
- **FluentValidation** for request validation
- **AES Encryption** for sensitive credential storage
- **MailKit / MimeKit** for SMTP sending
- **Microsoft Graph SDK + Azure.Identity** for OAuth2-based Microsoft 365 sending
- **xUnit, Moq, FluentAssertions** for testing
- **Swashbuckle (Swagger)** for API documentation

---

## 📖 How It Works

### The Tenant Model

```
EmailSaaS (Platform)
│
├── Application: JustBill  (registers, gets an API Key)
│   ├── Client: TCS       → own email provider + own templates
│   └── Client: Wipro     → own email provider + own templates
│
└── Application: ProGuru  (registers, gets its own API Key)
    └── Client: Infosys   → own email provider + own templates
```

- An **Application** is a top-level tenant (e.g. a SaaS company using EmailSaaS).
- Each Application can have multiple **Clients** underneath it (companies using the Application's platform).
- Each Client configures their **own** sending provider and manages their **own** templates.
- One Application's data can never be seen or affected by another Application's API key.

### The Sending Flow

1. An Application registers and configures a Client, an email provider, and one or more templates.
2. The Application's backend calls a single endpoint — `send-email` — passing a template code, recipient, and dynamic parameters.
3. EmailSaaS resolves the correct template and provider, replaces placeholders, injects tracking, and sends the email through the configured provider (SMTP or Microsoft Graph).
4. Every send is logged, and open/click events are tracked automatically as the recipient interacts with the email.

### Supported Email Providers

- **SMTP** — works with Gmail, Zoho, custom mail servers, and any standard SMTP account
- **Microsoft Graph API** — OAuth2-based sending for Office 365 and Outlook.com accounts, used specifically because Microsoft has deprecated SMTP Basic Authentication across its mail products
- **API-key based providers** — a generic pattern for any REST-based transactional email service

Routing between provider types is automatic and based on which credentials are configured — no hardcoded provider list.

### Email Tracking

EmailSaaS implements its own tracking system, with no external webhook dependency:

- An invisible tracking pixel is embedded in every email to detect opens
- Every link in an email is automatically rewritten to route through a tracking redirect before reaching its real destination, to detect clicks
- Delivery status is recorded the moment the send call succeeds

---

## 🔒 Security

- All sensitive credentials (passwords, API keys, OAuth secrets) are encrypted using AES before being stored
- API responses never expose raw credentials back to the caller
- Every request is authenticated via a unique API key, and every query is automatically scoped to the calling tenant only
- Global exception handling ensures no internal errors or stack traces are ever leaked in API responses
- In production, only the send-email endpoint is publicly reachable — all administrative endpoints and Swagger documentation are automatically disabled outside of development

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

1. Clone the repository
2. Configure your connection string and encryption keys in `appsettings.json`
3. Run the EF Core migrations to set up the database
4. Run the API — in Development mode, Swagger UI is available for full exploration of all endpoints
5. Register your first Application to receive an API key, then create a Client, configure an email provider, and create your first template
6. Start sending emails through the `send-email` endpoint

---

## 🗺️ Roadmap

- Bulk and scheduled email sending
- Template versioning and A/B testing
- Rate limiting and usage-based plans
- Unsubscribe management for compliance
- Analytics dashboard for open/click/bounce rates
- Additional native provider integrations (Gmail API, AWS SES)

---

## 📄 License

Built and maintained by **Hemanth**.
hemanthkv23@outlook.com
