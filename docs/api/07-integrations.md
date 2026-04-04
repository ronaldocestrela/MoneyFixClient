# Integrations Endpoints

Base URL: `http://localhost:5000/api/integrations`

**Authentication:** Required (Bearer token)

## 1. Import Transactions from CSV

**Upload a CSV file to bulk import transactions into the system**

### Request

```
POST /integrations/import
```

**Headers:**
```
Authorization: Bearer <access_token>
Content-Type: multipart/form-data
```

**Body Parameters:**
- `file` (file, required): CSV file with transaction data
- `accountId` (GUID, required): Target account for imported transactions
- `skipErrors` (boolean, optional): If true, import valid rows and skip invalid ones; if false, reject entire import on any error (default: false)

### CSV Format

The CSV file must have the following columns in order:

```
Date,Amount,Type,Description,CategoryName
2026-04-01,125.50,Saida,Grocery shopping,Groceries
2026-04-02,2500.00,Entrada,Monthly salary,Salary
2026-04-03,50.00,Saida,Gas station,Transportation
```

**Column Specifications:**

| Column | Type | Required | Format | Notes |
|--------|------|----------|--------|-------|
| Date | DateTime | Yes | `YYYY-MM-DD` | Must be valid date |
| Amount | Decimal | Yes | `123.45` | Must be > 0, max 2 decimals |
| Type | String | Yes | `Entrada` or `Saida` | Case-sensitive |
| Description | String | No | Any text | Max 500 characters |
| CategoryName | String | Yes | Category name | Must match existing category for the user |

### Response - Success (200 OK)

```json
{
  "imported": 2,
  "skipped": 1,
  "errors": [
    {
      "rowNumber": 3,
      "error": "Category 'InvalidCategory' not found"
    }
  ],
  "summary": "Successfully imported 2 transactions. 1 transaction was skipped due to errors.",
  "details": {
    "importedCount": 2,
    "skippedCount": 1,
    "errorCount": 1,
    "duplicateDetected": 0
  }
}
```

**Response Fields:**
- `imported`: Number of successfully imported transactions
- `skipped`: Number of rows that were skipped due to errors or duplicates
- `errors`: Array of validation errors per row
  - `rowNumber`: CSV row number (1-indexed, header is row 1)
  - `error`: Human-readable error message
- `summary`: Overall import summary message
- `details`: Breakdown of import results

### Response - Error

**Missing Required Parameter (400 Bad Request):**
```json
{
  "errors": [
    "File is required",
    "Account is required"
  ]
}
```

**Invalid Account (404 Not Found):**
```json
{
  "message": "Selected account does not belong to you or was not found"
}
```

**Invalid CSV Format (400 Bad Request):**
```json
{
  "errors": [
    "CSV header is invalid. Expected: Date,Amount,Type,Description,CategoryName"
  ]
}
```

**All Rows Have Errors - skipErrors=false (400 Bad Request):**
```json
{
  "message": "All rows failed validation. No transactions were imported.",
  "errors": [
    {
      "rowNumber": 2,
      "error": "Category 'NonExistent' not found"
    },
    {
      "rowNumber": 3,
      "error": "Type 'Invalid' is not valid. Must be 'Entrada' or 'Saida'"
    }
  ]
}
```

---

## 2. Export Transactions

**Download transactions in CSV or JSON format with optional filtering**

### Request

```
GET /integrations/export
```

**Headers:**
```
Authorization: Bearer <access_token>
```

**Query Parameters:**
- `format` (string, required): Export format - `csv` or `json`
- `from` (datetime, optional): Start date for export (default: all data)
- `to` (datetime, optional): End date for export (default: all data)
- `accountId` (GUID, optional): Filter by account (default: all accounts)
- `type` (string, optional): Filter by type - `Entrada` or `Saida` (default: all types)

### Response - Success (200 OK)

**CSV Format (format=csv):**

Response headers:
```
Content-Type: text/csv
Content-Disposition: attachment; filename="transactions_2026-04-01.csv"
```

Response body:
```
Date,Amount,Type,Description,CategoryName,AccountName
2026-04-01,125.50,Saida,Grocery shopping,Groceries,Main Checking
2026-04-02,2500.00,Entrada,Monthly salary,Salary,Main Checking
2026-04-03,50.00,Saida,Gas station,Transportation,Main Checking
2026-04-04,350.00,Saida,Utility bill,Utilities,Main Checking
2026-04-05,5000.00,Entrada,Bonus,Salary,Savings Account
```

**JSON Format (format=json):**

Response headers:
```
Content-Type: application/json
Content-Disposition: attachment; filename="transactions_2026-04-01.json"
```

Response body:
```json
[
  {
    "date": "2026-04-01T00:00:00Z",
    "amount": 125.50,
    "type": "Saida",
    "description": "Grocery shopping",
    "categoryName": "Groceries",
    "accountName": "Main Checking"
  },
  {
    "date": "2026-04-02T00:00:00Z",
    "amount": 2500.00,
    "type": "Entrada",
    "description": "Monthly salary",
    "categoryName": "Salary",
    "accountName": "Main Checking"
  },
  {
    "date": "2026-04-03T00:00:00Z",
    "amount": 50.00,
    "type": "Saida",
    "description": "Gas station",
    "categoryName": "Transportation",
    "accountName": "Main Checking"
  }
]
```

### Response - Error

**Invalid Format (400 Bad Request):**
```json
{
  "message": "Format must be 'csv' or 'json'"
}
```

**Invalid Date Range (400 Bad Request):**
```json
{
  "message": "From date must be before To date"
}
```

**No Transactions Match Filter (200 OK with empty data):**

CSV:
```
Date,Amount,Type,Description,CategoryName,AccountName
```

JSON:
```json
[]
```

---

## Duplicate Detection

The system automatically detects duplicate transactions using SHA-256 hashing. A transaction is considered a duplicate if:
- It has the same transaction **date**
- It has the same **amount**
- It belongs to the same **category**
- It belongs to the same **account**
- It has the same **description**

When a duplicate is detected during import, the transaction is **skipped** and reported in the error list.

### Duplicate Example

If you try to import the same transaction twice:

```
Date,Amount,Type,Description,CategoryName
2026-04-01,125.50,Saida,Grocery shopping,Groceries
```

**First import:** Transaction is imported successfully
**Second import:** Transaction is skipped as duplicate

---

## Important Notes

### CSV Import Rules
- File must be UTF-8 encoded
- Header row is required (Date,Amount,Type,Description,CategoryName)
- Empty rows are ignored
- Whitespace is trimmed from all fields
- Categories must already exist in the system
- Accounts must belong to the authenticated user

### Export Options
- Both CSV and JSON contain identical data, just different formats
- Exports include the user's account and category names (not just IDs)
- Filename includes export date for easy identification
- Large exports may take longer to generate

### Date Handling
- Import dates must be in `YYYY-MM-DD` format (no time component)
- Export dates represent the occurrence date of the transaction
- All dates are treated as UTC
- Future dates (in CSV format) are allowed

### CSV File Best Practices
- Use Excel, Google Sheets, or similar to prepare CSV
- Ensure Amount field uses `.` (dot) as decimal separator
- Do not include extra columns or rows
- Validate categories exist before importing
- Use `skipErrors=true` to import partial datasets

### Idempotency
- Importing the same file multiple times will skip duplicates
- Safe to retry imports without worrying about double-entries
- Use date and description variation to import similar transactions

### Performance
- Large files (1000+ rows) may take several seconds to process
- Exports are generated on-demand
- Date range filtering reduces export size

### Security
- All imports/exports are isolated to the authenticated user
- Other users' data is never included in exports
- Import operations are logged in the audit system
- Exported files do not contain sensitive user information

---

## Implementation Notes

- CSV parsing is done line-by-line for memory efficiency
- Duplicate detection uses SHA-256 hash of (date, amount, categoryId, accountId, description)
- Amount validation: must be > 0 and have max 2 decimal places
- Type must exactly match `Entrada` or `Saida` (case-sensitive)
- Category lookup is case-insensitive but returns the stored category name
- All amounts are in the system's default currency
- Import timestamp is recorded automatically
- Each imported transaction generates an audit log entry marked as `Create`
