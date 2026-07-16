# EmailSaaS API Integration Guide

Welcome to the EmailSaaS platform! This guide provides everything you need to start sending dynamic, tracked emails through our service.

## 🚀 Overview

EmailSaaS is a managed email orchestration platform. As an end user, you don't need to worry about configuring mail servers or managing email templates. You simply invoke a single API endpoint to trigger your emails.

Your service developer or account manager will provide you with the following credentials and details required to use the API:
- **X-Api-Key**: Your secure authentication key to be passed in the HTTP headers.
- **Application ID**: The identifier for your application (Application Code).
- **Client ID**: The identifier for your specific client account.
- **Template Codes**: The identifiers for the pre-configured email templates you can use.

---

## 🔌 API Reference

### Send Email

Send a templated, dynamic email with built-in tracking. 

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

## 🔒 Security & Authentication

- Every API request must include the `X-Api-Key` header.
- Keep your API key secure. Never expose it in client-side code (e.g., frontend JavaScript or mobile apps). All calls should be made from your secure backend servers.

## 📖 How It Works

1. **Prepare your payload:** Gather your credentials (`applicationId`, `clientId`), the `templateCode`, and the recipient's details.
2. **Set the parameters:** Any dynamic content in the template (like `{{FirstName}}`) will be replaced by the key-value pairs provided in the `parameters` dictionary.
3. **Send the request:** Call the `/api/send-email` endpoint.
4. **Platform handles the rest:** EmailSaaS automatically injects open and click tracking, connects to the configured provider, and dispatches the email reliably.

If you have any questions or need new email templates added, please contact your service developer or account manager.
