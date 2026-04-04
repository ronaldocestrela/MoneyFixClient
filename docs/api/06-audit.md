# Audit Endpoints

Base URL: `http://localhost:5000/api/audit`

**Authentication:** Required (Bearer token)

## 1. Get Audit Logs

**Retrieve a complete audit trail of all changes made to your financial data**

### Request

```
GET /audit
```

**Headers:**
```
Authorization: Bearer <access_token>
```

**Query Parameters:**
- `from` (datetime, optional): Start date for audit logs
- `to` (datetime, optional): End date for audit logs
- `entityName` (string, optional): Filter by entity type - `Transaction`, `Account`, `Category`, `RefreshToken`
- `actionType` (string, optional): Filter by action - `Create`, `Update`, `Delete`
- `userId` (GUID, optional): Filter by user (if user has permission to view other users' audits)
- `pageNumber` (integer, optional): Pagination page (default: 1)
- `pageSize` (integer, optional): Results per page (default: 50, max: 500)

### Response - Success (200 OK)

```json
[
  {
    "id": "990i2844-i73f-85h8-e150-880099884444",
    "userId": "aa0j3955-j84g-96i9-f261-991100995555",
    "entityName": "Transaction",
    "entityId": "770g0622-g51d-63f6-c938-668877662222",
    "actionType": "Create",
    "description": "Created transaction",
    "changes": [
      {
        "fieldName": "Amount",
        "oldValue": null,
        "newValue": "125.50"
      },
      {
        "fieldName": "Type",
        "oldValue": null,
        "newValue": "Saida"
      },
      {
        "fieldName": "Description",
        "oldValue": null,
        "newValue": "Grocery shopping"
      },
      {
        "fieldName": "OccurredAt",
        "oldValue": null,
        "newValue": "2026-04-01T14:30:00Z"
      }
    ],
    "createdAtUtc": "2026-04-01T15:00:00Z"
  },
  {
    "id": "bb1k4066-k95h-07j0-g372-002211006666",
    "userId": "aa0j3955-j84g-96i9-f261-991100995555",
    "entityName": "Transaction",
    "entityId": "770g0622-g51d-63f6-c938-668877662222",
    "actionType": "Update",
    "description": "Updated transaction",
    "changes": [
      {
        "fieldName": "Amount",
        "oldValue": "125.50",
        "newValue": "135.75"
      },
      {
        "fieldName": "Description",
        "oldValue": "Grocery shopping",
        "newValue": "Grocery shopping - updated amount"
      }
    ],
    "createdAtUtc": "2026-04-02T10:30:00Z"
  },
  {
    "id": "cc2l5177-l06i-18k1-h483-113322117777",
    "userId": "aa0j3955-j84g-96i9-f261-991100995555",
    "entityName": "Account",
    "entityId": "550e8400-e29b-41d4-a716-446655440000",
    "actionType": "Create",
    "description": "Created account",
    "changes": [
      {
        "fieldName": "Name",
        "oldValue": null,
        "newValue": "Main Checking"
      },
      {
        "fieldName": "Type",
        "oldValue": null,
        "newValue": "ContaCorrente"
      },
      {
        "fieldName": "InitialBalance",
        "oldValue": null,
        "newValue": "5000.00"
      }
    ],
    "createdAtUtc": "2026-03-15T09:00:00Z"
  },
  {
    "id": "dd3m6288-m17j-29l2-i594-224433228888",
    "userId": "aa0j3955-j84g-96i9-f261-991100995555",
    "entityName": "Transaction",
    "entityId": "770g0622-g51d-63f6-c938-668877662222",
    "actionType": "Delete",
    "description": "Deleted transaction",
    "changes": [
      {
        "fieldName": "Amount",
        "oldValue": "135.75",
        "newValue": null
      },
      {
        "fieldName": "Type",
        "oldValue": "Saida",
        "newValue": null
      },
      {
        "fieldName": "Description",
        "oldValue": "Grocery shopping - updated amount",
        "newValue": null
      }
    ],
    "createdAtUtc": "2026-04-03T16:45:00Z"
  }
]
```

**Response Fields:**
- `id`: Audit log entry unique identifier
- `userId`: ID of the user who made the change
- `entityName`: Type of entity changed (Transaction, Account, Category, RefreshToken)
- `entityId`: ID of the entity that was modified
- `actionType`: Type of action (Create, Update, Delete)
- `description`: Human-readable description of the change
- `changes`: Array of field-level changes
  - `fieldName`: Name of the field that changed
  - `oldValue`: Previous value (null for Create)
  - `newValue`: New value (null for Delete)
- `createdAtUtc`: When the change occurred

### Response - Error

**Invalid EntityName Filter (400 Bad Request):**
```json
{
  "message": "Invalid entity name. Allowed values: Transaction, Account, Category, RefreshToken"
}
```

**Invalid ActionType Filter (400 Bad Request):**
```json
{
  "message": "Invalid action type. Allowed values: Create, Update, Delete"
}
```

**Invalid Date Range (400 Bad Request):**
```json
{
  "message": "From date must be before To date"
}
```

---

## Entity Names Reference

| Entity Name    | Description              | Tracked Fields                                      |
|----------------|--------------------------|-----------------------------------------------------|
| Transaction    | Financial transaction    | Amount, Type, Description, OccurredAt, CategoryId, AccountId |
| Account        | Financial account        | Name, Type, InitialBalance, CurrentBalance          |
| Category       | Expense category         | Name, Type, Color                                   |
| RefreshToken   | Authentication token     | ExpiresAt, RevokedAt                                |

---

## Action Types Reference

| Action Type | Description |
|-------------|-------------|
| Create      | New entity was created |
| Update      | Existing entity was modified |
| Delete      | Entity was removed |

---

## Important Notes

### What Gets Audited
- All `Create`, `Update`, and `Delete` operations on Transactions, Accounts, Categories
- All authentication token refresh events (RefreshToken entity)
- Field-level changes are recorded with before/after values
- System-generated fields (IDs, timestamps) are typically not tracked

### What's NOT Audited
- Read operations (GET requests)
- Failed validation attempts
- Authentication failures
- User login/logout events (RefreshToken changes are tracked separately)

### Data Isolation
- Users only see audit logs for their own changes
- No cross-user audit data is visible
- Even with `userId` parameter, filtering is restricted to the authenticated user's data

### Audit Log Retention
- Audit logs are retained indefinitely
- Deleted entities' audit history remains (entity deletion is logged)
- Cannot manually delete or modify audit logs

### Filtering Behavior
- Multiple filters work as AND conditions
- `from` and `to` dates filter by `createdAtUtc`
- Entity name and action type are case-insensitive
- Date range defaults to all available logs if not specified

### Performance Considerations
- Large date ranges may impact response time
- Pagination is recommended for large result sets
- Default page size is 50 entries per page (max 500)
- Sort order: most recent first

### Change Tracking
- When a transaction amount changes, the old and new amounts are both recorded
- When a category is deleted but transactions reference it, the transaction audit logs preserve the category ID
- For decimal values, precision is maintained to 2 decimal places
- For boolean values, shown as "true" or "false"
- For GUID values, shown as hyphenated string format

---

## Implementation Notes

- All audit operations include the authenticated user's context
- Timestamps are in UTC (ISO 8601 format)
- Audit logs are created automatically during SaveChanges() hook
- User information is extracted from claims at the time of change
- If user information is unavailable, audit log is created with Guid.Empty as userId
- Changes array will be empty if only system fields were modified
- Sensitive data (like password hashes) is never included in audit logs
