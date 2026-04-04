# MoneyFix API Documentation

Welcome to the MoneyFix API documentation. This guide provides comprehensive information about all available endpoints for frontend integration.

## Table of Contents

### Authentication & Identity
- [Authentication Endpoints](./01-auth.md) - Login, Register, Refresh, Logout, Password Reset

### Finance Management
- [Categories Endpoints](./02-categories.md) - Create, Read, Update, Delete Categories
- [Accounts Endpoints](./03-accounts.md) - Create, Read, Update, Delete Accounts
- [Transactions Endpoints](./04-transactions.md) - Create, Read, Update, Delete Transactions

### Reporting & Analytics
- [Reporting Endpoints](./05-reporting.md) - Monthly Summary, Expenses by Category, Balance Evolution, Dashboard

### Audit & Compliance
- [Audit Endpoints](./06-audit.md) - Query audit logs

### Integration & Data Management
- [Integrations Endpoints](./07-integrations.md) - CSV Import/Export Transactions

---

## Base URL

```
http://localhost:5000/api
```

## Authentication

All endpoints (except `/identity/login`, `/identity/register`, `/identity/refresh`, `/identity/forgot-password`, and `/identity/reset-password`) require a valid JWT token in the `Authorization` header:

```
Authorization: Bearer <your_jwt_token>
```

## Error Handling

All endpoints return consistent error responses with appropriate HTTP status codes:

### HTTP Status Codes

| Status | Meaning |
|--------|---------|
| 200    | OK - Request successful |
| 201    | Created - Resource created successfully |
| 204    | No Content - Request successful with no response body |
| 400    | Bad Request - Invalid input data |
| 401    | Unauthorized - Missing or invalid authentication |
| 403    | Forbidden - Insufficient permissions |
| 404    | Not Found - Resource not found |
| 409    | Conflict - Resource conflict (e.g., duplicate) |
| 500    | Internal Server Error - Server error |

### Error Response Format

```json
{
  "error": "Description of the error",
  "message": "Detailed error message",
  "errors": ["error1", "error2"]  // Field-level errors (if applicable)
}
```

## Request/Response Format

- All requests and responses use **JSON** format
- Datetime values are in **ISO 8601** format (UTC): `2026-04-01T10:30:00Z`
- Decimal values (money) use **2 decimal places**
- GUIDs are in **string format** with hyphens: `550e8400-e29b-41d4-a716-446655440000`

## Rate Limiting

Currently no rate limiting is enforced. This may be added in future versions.

## Pagination

Paginated endpoints support:
- `pageNumber`: Page number (1-indexed, default: 1)
- `pageSize`: Items per page (default: 20, max: 100)

Paginated responses include:
```json
{
  "data": [...],
  "pageNumber": 1,
  "pageSize": 20,
  "totalCount": 100
}
```

---

For detailed information about specific endpoints, please refer to the individual documentation files.
