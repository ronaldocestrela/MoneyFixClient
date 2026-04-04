# Transactions Endpoints

Base URL: `http://localhost:5000/api/transactions`

**Authentication:** Required (Bearer token)

## 1. Create Transaction

**Record a new financial transaction**

### Request

```
POST /transactions
```

**Headers:**
```
Authorization: Bearer <access_token>
Content-Type: application/json
```

**Body:**
```json
{
  "accountId": "550e8400-e29b-41d4-a716-446655440000",
  "categoryId": "660f9511-f40c-52e5-b827-557766551111",
  "type": "Saida",
  "amount": 125.50,
  "occurredAt": "2026-04-01T14:30:00Z",
  "description": "Grocery shopping at supermarket"
}
```

**Parameters:**
- `accountId` (GUID, required): Target account
- `categoryId` (GUID, required): Transaction category
- `type` (string, required): `Entrada` (income) or `Saida` (expense)
- `amount` (decimal, required): Transaction amount (> 0)
- `occurredAt` (datetime, required): When the transaction occurred
- `description` (string, optional): Transaction notes (max 500 characters)

### Response - Success (201 Created)

```json
{
  "id": "770g0622-g51d-63f6-c938-668877662222",
  "accountId": "550e8400-e29b-41d4-a716-446655440000",
  "categoryId": "660f9511-f40c-52e5-b827-557766551111",
  "type": "Saida",
  "amount": 125.50,
  "occurredAt": "2026-04-01T14:30:00Z",
  "description": "Grocery shopping at supermarket",
  "createdAtUtc": "2026-04-01T15:00:00Z"
}
```

### Response - Error

**Invalid Category from Another User (400 Bad Request):**
```json
{
  "message": "Selected category does not belong to you"
}
```

**Invalid Account from Another User (400 Bad Request):**
```json
{
  "message": "Selected account does not belong to you"
}
```

**Invalid Input (400 Bad Request):**
```json
{
  "errors": [
    "Amount must be greater than 0",
    "Account is required",
    "Category is required"
  ]
}
```

---

## 2. Get All Transactions

**Retrieve transactions with filtering options**

### Request

```
GET /transactions
```

**Headers:**
```
Authorization: Bearer <access_token>
```

**Query Parameters:**
- `from` (datetime, optional): Start date for filtering
- `to` (datetime, optional): End date for filtering
- `type` (string, optional): Filter by type - `Entrada` or `Saida`
- `categoryId` (GUID, optional): Filter by category
- `accountId` (GUID, optional): Filter by account

### Response - Success (200 OK)

```json
[
  {
    "id": "770g0622-g51d-63f6-c938-668877662222",
    "accountId": "550e8400-e29b-41d4-a716-446655440000",
    "accountName": "Main Checking",
    "categoryId": "660f9511-f40c-52e5-b827-557766551111",
    "categoryName": "Groceries",
    "type": "Saida",
    "amount": 125.50,
    "occurredAt": "2026-04-01T14:30:00Z",
    "description": "Grocery shopping",
    "createdAtUtc": "2026-04-01T15:00:00Z"
  },
  {
    "id": "880h1733-h62e-74g7-d049-779988773333",
    "accountId": "550e8400-e29b-41d4-a716-446655440000",
    "accountName": "Main Checking",
    "categoryId": "990i2844-i73f-85h8-e150-880099884444",
    "categoryName": "Salary",
    "type": "Entrada",
    "amount": 5000.00,
    "occurredAt": "2026-04-01T09:00:00Z",
    "description": "Monthly salary",
    "createdAtUtc": "2026-04-01T09:30:00Z"
  }
]
```

---

## 3. Get Transaction by ID

**Retrieve a specific transaction**

### Request

```
GET /transactions/{id}
```

**Headers:**
```
Authorization: Bearer <access_token>
```

**Path Parameters:**
- `id` (GUID, required): Transaction ID

### Response - Success (200 OK)

```json
{
  "id": "770g0622-g51d-63f6-c938-668877662222",
  "accountId": "550e8400-e29b-41d4-a716-446655440000",
  "accountName": "Main Checking",
  "categoryId": "660f9511-f40c-52e5-b827-557766551111",
  "categoryName": "Groceries",
  "type": "Saida",
  "amount": 125.50,
  "occurredAt": "2026-04-01T14:30:00Z",
  "description": "Grocery shopping",
  "createdAtUtc": "2026-04-01T15:00:00Z"
}
```

### Response - Error

**Not Found (404 Not Found):**
```json
{
  "message": "Transaction not found"
}
```

---

## 4. Update Transaction

**Edit an existing transaction**

### Request

```
PUT /transactions/{id}
```

**Headers:**
```
Authorization: Bearer <access_token>
Content-Type: application/json
```

**Path Parameters:**
- `id` (GUID, required): Transaction ID

**Body:**
```json
{
  "accountId": "550e8400-e29b-41d4-a716-446655440000",
  "categoryId": "660f9511-f40c-52e5-b827-557766551111",
  "type": "Saida",
  "amount": 135.75,
  "occurredAt": "2026-04-01T14:30:00Z",
  "description": "Grocery shopping - updated amount"
}
```

### Response - Success (200 OK)

```json
{
  "id": "770g0622-g51d-63f6-c938-668877662222",
  "accountId": "550e8400-e29b-41d4-a716-446655440000",
  "accountName": "Main Checking",
  "categoryId": "660f9511-f40c-52e5-b827-557766551111",
  "categoryName": "Groceries",
  "type": "Saida",
  "amount": 135.75,
  "occurredAt": "2026-04-01T14:30:00Z",
  "description": "Grocery shopping - updated amount",
  "createdAtUtc": "2026-04-01T15:00:00Z"
}
```

### Response - Error

**Transaction Not Found (404 Not Found):**
```json
{
  "message": "Transaction not found"
}
```

---

## 5. Delete Transaction

**Remove a transaction**

### Request

```
DELETE /transactions/{id}
```

**Headers:**
```
Authorization: Bearer <access_token>
```

**Path Parameters:**
- `id` (GUID, required): Transaction ID

### Response - Success (204 No Content)

No response body.

### Response - Error

**Transaction Not Found (404 Not Found):**
```json
{
  "message": "Transaction not found"
}
```

---

## Transaction Types Reference

| Type     | Description | Example              |
|----------|-------------|----------------------|
| Entrada  | Income      | Salary, bonuses      |
| Saida    | Expense     | Shopping, utilities  |

---

## Important Notes

### Balance Recalculation
- When a transaction is created, the account's current balance is automatically updated
- When a transaction is updated, the balance is recalculated based on the difference
- When a transaction is deleted, the amount is added back to the account balance

### Account Balance Formula
```
Current Balance = Initial Balance + Sum(Entrada transactions) - Sum(Saida transactions)
```

### Audit Trail
- All transactions are automatically logged in the audit system
- Changes are tracked with user ID, timestamp, entity name, and before/after values
- Deleted transactions cannot be recovered

### Data Validation
- Amount must be positive (> 0)
- Categories and accounts must belong to the authenticated user
- Both category and account types must match the transaction type
- DateTime must be in ISO 8601 format (UTC)
- Description is optional but limited to 500 characters

---

## Implementation Notes

- Transactions are immutable once created (timestamps cannot be changed)
- Only the user who created the transaction can modify/delete it
- All transactions are private to the authenticated user
- Filtering by multiple criteria is supported (e.g., type AND category AND date range)
- Results are sorted by occurrence date (most recent first)
