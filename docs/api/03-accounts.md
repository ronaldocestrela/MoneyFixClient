# Accounts Endpoints

Base URL: `http://localhost:5000/api/accounts`

**Authentication:** Required (Bearer token)

## 1. Create Account

**Create a new bank/wallet account**

### Request

```
POST /accounts
```

**Headers:**
```
Authorization: Bearer <access_token>
Content-Type: application/json
```

**Body:**
```json
{
  "name": "Main Checking",
  "type": "ContaCorrente",
  "initialBalance": 5000.00,
  "isActive": true
}
```

**Parameters:**
- `name` (string, required): Account name (max 120 characters)
- `type` (string, required): Account type - `ContaCorrente`, `ContaPoupanca`, or `Carteira`
- `initialBalance` (decimal, required): Starting balance (≥ 0)
- `isActive` (boolean, required): Account active status

### Response - Success (201 Created)

```json
{
  "id": "550e8400-e29b-41d4-a716-446655440000",
  "name": "Main Checking",
  "type": "ContaCorrente",
  "initialBalance": 5000.00,
  "currentBalance": 5000.00,
  "isActive": true,
  "createdAtUtc": "2026-04-01T10:30:00Z"
}
```

### Response - Error

**Duplicate Account Name (409 Conflict):**
```json
{
  "message": "An account with this name already exists for your account."
}
```

**Invalid Input (400 Bad Request):**
```json
{
  "errors": [
    "Account name is required",
    "Initial balance cannot be negative",
    "Invalid account type"
  ]
}
```

---

## 2. Get All Accounts

**Retrieve all accounts for current user**

### Request

```
GET /accounts
```

**Headers:**
```
Authorization: Bearer <access_token>
```

**Query Parameters:**
- `isActive` (boolean, optional): Filter by active status

### Response - Success (200 OK)

```json
[
  {
    "id": "550e8400-e29b-41d4-a716-446655440000",
    "name": "Main Checking",
    "type": "ContaCorrente",
    "initialBalance": 5000.00,
    "currentBalance": 4750.25,
    "isActive": true,
    "createdAtUtc": "2026-04-01T10:30:00Z"
  },
  {
    "id": "660f9511-f40c-52e5-b827-557766551111",
    "name": "Wallet",
    "type": "Carteira",
    "initialBalance": 200.00,
    "currentBalance": 150.00,
    "isActive": true,
    "createdAtUtc": "2026-03-15T14:00:00Z"
  }
]
```

---

## 3. Get Account by ID

**Retrieve a specific account**

### Request

```
GET /accounts/{id}
```

**Headers:**
```
Authorization: Bearer <access_token>
```

**Path Parameters:**
- `id` (GUID, required): Account ID

### Response - Success (200 OK)

```json
{
  "id": "550e8400-e29b-41d4-a716-446655440000",
  "name": "Main Checking",
  "type": "ContaCorrente",
  "initialBalance": 5000.00,
  "currentBalance": 4750.25,
  "isActive": true,
  "createdAtUtc": "2026-04-01T10:30:00Z"
}
```

### Response - Error

**Not Found (404 Not Found):**
```json
{
  "message": "Account not found"
}
```

---

## 4. Update Account

**Update an existing account**

### Request

```
PUT /accounts/{id}
```

**Headers:**
```
Authorization: Bearer <access_token>
Content-Type: application/json
```

**Path Parameters:**
- `id` (GUID, required): Account ID

**Body:**
```json
{
  "name": "Primary Checking",
  "initialBalance": 5000.00,
  "isActive": true
}
```

### Response - Success (200 OK)

```json
{
  "id": "550e8400-e29b-41d4-a716-446655440000",
  "name": "Primary Checking",
  "type": "ContaCorrente",
  "initialBalance": 5000.00,
  "currentBalance": 4750.25,
  "isActive": true,
  "createdAtUtc": "2026-04-01T10:30:00Z"
}
```

### Response - Error

**Account Not Found (404 Not Found):**
```json
{
  "message": "Account not found"
}
```

---

## 5. Delete Account

**Delete an account**

### Request

```
DELETE /accounts/{id}
```

**Headers:**
```
Authorization: Bearer <access_token>
```

**Path Parameters:**
- `id` (GUID, required): Account ID

### Response - Success (204 No Content)

No response body.

### Response - Error

**Account Not Found (404 Not Found):**
```json
{
  "message": "Account not found"
}
```

**Account in Use (400 Bad Request):**
```json
{
  "message": "Cannot delete an account with associated transactions"
}
```

---

## Account Types Reference

| Type             | Description      | Use Case                |
|------------------|------------------|------------------------|
| ContaCorrente    | Checking Account | Daily banking, salaries |
| ContaPoupanca    | Savings Account  | Savings, reserves       |
| Carteira         | Wallet/Cash      | Physical cash           |

---

## Implementation Notes

- Account names must be unique per user
- Initial balance can be adjusted when creating an account
- Current balance is automatically calculated based on transactions
- Accounts can only be deleted if they have no associated transactions
- All accounts are private to the authenticated user
- Balance changes are reflected in real-time as transactions are created/updated/deleted
- Initial balance cannot be changed after account creation; only transaction history affects current balance
