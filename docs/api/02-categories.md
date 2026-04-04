# Categories Endpoints

Base URL: `http://localhost:5000/api/categories`

**Authentication:** Required (Bearer token)

## 1. Create Category

**Create a new spending category**

### Request

```
POST /categories
```

**Headers:**
```
Authorization: Bearer <access_token>
Content-Type: application/json
```

**Body:**
```json
{
  "name": "Groceries",
  "type": "Saida",
  "color": "#FF0000"
}
```

**Parameters:**
- `name` (string, required): Category name (max 120 characters)
- `type` (string, required): Category type - `Entrada` (income) or `Saida` (expense)
- `color` (string, required): Hex color code for UI representation (e.g., `#FF0000`)

### Response - Success (201 Created)

```json
{
  "id": "550e8400-e29b-41d4-a716-446655440000",
  "name": "Groceries",
  "type": "Saida",
  "color": "#FF0000",
  "createdAtUtc": "2026-04-01T10:30:00Z"
}
```

### Response - Error

**Duplicate Category (409 Conflict):**
```json
{
  "message": "A category with this name and type already exists for your account."
}
```

**Invalid Input (400 Bad Request):**
```json
{
  "errors": [
    "Category name is required",
    "Invalid category type"
  ]
}
```

---

## 2. Get All Categories

**Retrieve all categories for current user**

### Request

```
GET /categories
```

**Headers:**
```
Authorization: Bearer <access_token>
```

**Query Parameters:**
- `type` (string, optional): Filter by type - `Entrada` or `Saida`

### Response - Success (200 OK)

```json
[
  {
    "id": "550e8400-e29b-41d4-a716-446655440000",
    "name": "Groceries",
    "type": "Saida",
    "color": "#FF0000",
    "createdAtUtc": "2026-04-01T10:30:00Z"
  },
  {
    "id": "660f9511-f40c-52e5-b827-557766551111",
    "name": "Salary",
    "type": "Entrada",
    "color": "#00FF00",
    "createdAtUtc": "2026-04-01T09:00:00Z"
  }
]
```

---

## 3. Get Category by ID

**Retrieve a specific category**

### Request

```
GET /categories/{id}
```

**Headers:**
```
Authorization: Bearer <access_token>
```

**Path Parameters:**
- `id` (GUID, required): Category ID

### Response - Success (200 OK)

```json
{
  "id": "550e8400-e29b-41d4-a716-446655440000",
  "name": "Groceries",
  "type": "Saida",
  "color": "#FF0000",
  "createdAtUtc": "2026-04-01T10:30:00Z"
}
```

### Response - Error

**Not Found (404 Not Found):**
```json
{
  "message": "Category not found"
}
```

---

## 4. Update Category

**Update an existing category**

### Request

```
PUT /categories/{id}
```

**Headers:**
```
Authorization: Bearer <access_token>
Content-Type: application/json
```

**Path Parameters:**
- `id` (GUID, required): Category ID

**Body:**
```json
{
  "name": "Supermarket",
  "type": "Saida",
  "color": "#FF5500"
}
```

### Response - Success (200 OK)

```json
{
  "id": "550e8400-e29b-41d4-a716-446655440000",
  "name": "Supermarket",
  "type": "Saida",
  "color": "#FF5500",
  "createdAtUtc": "2026-04-01T10:30:00Z"
}
```

### Response - Error

**Category Not Found (404 Not Found):**
```json
{
  "message": "Category not found"
}
```

**Unauthorized - Different User (403 Forbidden):**
```json
{
  "message": "Access denied"
}
```

---

## 5. Delete Category

**Delete a category**

### Request

```
DELETE /categories/{id}
```

**Headers:**
```
Authorization: Bearer <access_token>
```

**Path Parameters:**
- `id` (GUID, required): Category ID

### Response - Success (204 No Content)

No response body.

### Response - Error

**Category Not Found (404 Not Found):**
```json
{
  "message": "Category not found"
}
```

**Category in Use (400 Bad Request):**
```json
{
  "message": "Cannot delete a category with associated transactions"
}
```

---

## Category Types Reference

| Type     | Description  | Use Case          |
|----------|--------------|-------------------|
| Entrada  | Income       | Salary, Bonuses   |
| Saida    | Expense      | Groceries, Rent   |

---

## Color Format

Colors must be provided as hexadecimal color codes:
- Format: `#RRGGBB`
- Examples: `#FF0000` (red), `#00FF00` (green), `#0000FF` (blue)

---

## Implementation Notes

- Category names must be unique per type and per user
- Categories can only be deleted if they have no associated transactions
- All categories are private to the authenticated user
- Deleted categories cannot be recovered
