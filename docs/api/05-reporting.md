# Reporting Endpoints

Base URL: `http://localhost:5000/api/reporting`

**Authentication:** Required (Bearer token)

## 1. Get Monthly Summary

**Retrieve monthly financial summary with totals for income and expenses**

### Request

```
GET /reporting/monthly-summary
```

**Headers:**
```
Authorization: Bearer <access_token>
```

**Query Parameters:**
- `year` (integer, optional): Year for the summary (default: current year)
- `month` (integer, optional): Month for the summary 1-12 (default: current month)

### Response - Success (200 OK)

```json
{
  "month": 4,
  "year": 2026,
  "periodDescription": "April 2026",
  "totalIncome": 5500.00,
  "totalExpense": 1850.75,
  "netBalance": 3649.25,
  "transactionCount": 12,
  "incomeTransactionCount": 2,
  "expenseTransactionCount": 10
}
```

**Response Fields:**
- `month`: Month number (1-12)
- `year`: Year
- `periodDescription`: Human-readable period
- `totalIncome`: Sum of all `Entrada` transactions
- `totalExpense`: Sum of all `Saida` transactions
- `netBalance`: Income - Expenses
- `transactionCount`: Total number of transactions
- `incomeTransactionCount`: Count of `Entrada` transactions
- `expenseTransactionCount`: Count of `Saida` transactions

---

## 2. Get Expenses by Category

**Analyze expenses grouped by category for a time period**

### Request

```
GET /reporting/expenses-by-category
```

**Headers:**
```
Authorization: Bearer <access_token>
```

**Query Parameters:**
- `from` (datetime, optional): Start date (default: 30 days ago)
- `to` (datetime, optional): End date (default: today)

### Response - Success (200 OK)

```json
[
  {
    "categoryId": "660f9511-f40c-52e5-b827-557766551111",
    "categoryName": "Groceries",
    "totalAmount": 450.50,
    "transactionCount": 8,
    "percentage": 24.35,
    "averageTransaction": 56.31
  },
  {
    "categoryId": "770g0622-g51d-63f6-c938-668877662222",
    "categoryName": "Utilities",
    "totalAmount": 350.00,
    "transactionCount": 3,
    "percentage": 18.92,
    "averageTransaction": 116.67
  },
  {
    "categoryId": "880h1733-h62e-74g7-d049-779988773333",
    "categoryName": "Entertainment",
    "totalAmount": 280.75,
    "transactionCount": 5,
    "percentage": 15.18,
    "averageTransaction": 56.15
  },
  {
    "categoryId": "990i2844-i73f-85h8-e150-880099884444",
    "categoryName": "Transportation",
    "totalAmount": 370.00,
    "transactionCount": 6,
    "percentage": 20.00,
    "averageTransaction": 61.67
  }
]
```

**Response Fields:**
- `categoryId`: Category unique identifier
- `categoryName`: Category display name
- `totalAmount`: Sum of expenses in this category
- `transactionCount`: Number of transactions
- `percentage`: Percentage of total expenses
- `averageTransaction`: Average transaction amount

---

## 3. Get Balance Evolution

**Track account balance changes over time**

### Request

```
GET /reporting/balance-evolution
```

**Headers:**
```
Authorization: Bearer <access_token>
```

**Query Parameters:**
- `from` (datetime, optional): Start date (default: 90 days ago)
- `to` (datetime, optional): End date (default: today)
- `accountId` (GUID, optional): Filter by specific account (default: all accounts)
- `interval` (string, optional): Time grouping - `daily`, `weekly`, `monthly` (default: `daily`)

### Response - Success (200 OK)

```json
[
  {
    "date": "2026-03-01T00:00:00Z",
    "balance": 5000.00,
    "incomeToDate": 5000.00,
    "expenseToDate": 0.00
  },
  {
    "date": "2026-03-15T00:00:00Z",
    "balance": 4750.50,
    "incomeToDate": 5000.00,
    "expenseToDate": 249.50
  },
  {
    "date": "2026-04-01T00:00:00Z",
    "balance": 3649.25,
    "incomeToDate": 10500.00,
    "expenseToDate": 1850.75
  },
  {
    "date": "2026-04-15T00:00:00Z",
    "balance": 3850.00,
    "incomeToDate": 10500.00,
    "expenseToDate": 1650.00
  }
]
```

**Response Fields:**
- `date`: Date of the balance snapshot
- `balance`: Account balance at this date (after all transactions)
- `incomeToDate`: Cumulative income up to this date
- `expenseToDate`: Cumulative expenses up to this date

---

## 4. Get Dashboard Summary

**Retrieve comprehensive financial overview for dashboard display**

### Request

```
GET /reporting/dashboard
```

**Headers:**
```
Authorization: Bearer <access_token>
```

**Query Parameters:**
- `from` (datetime, optional): Start date for analysis (default: 30 days ago)
- `to` (datetime, optional): End date for analysis (default: today)

### Response - Success (200 OK)

```json
{
  "summary": {
    "totalBalance": 8500.00,
    "totalIncome": 8000.00,
    "totalExpense": 2100.00,
    "netChange": 5900.00,
    "transactionCount": 35
  },
  "byAccount": [
    {
      "accountId": "550e8400-e29b-41d4-a716-446655440000",
      "accountName": "Main Checking",
      "accountType": "ContaCorrente",
      "balance": 5000.00,
      "transactions": 20
    },
    {
      "accountId": "660f9511-f40c-52e5-b827-557766551111",
      "accountName": "Savings",
      "accountType": "ContaPoupanca",
      "balance": 3500.00,
      "transactions": 15
    }
  ],
  "topCategories": [
    {
      "categoryId": "660f9511-f40c-52e5-b827-557766551111",
      "categoryName": "Groceries",
      "totalAmount": 450.50,
      "percentage": 21.45
    },
    {
      "categoryId": "770g0622-g51d-63f6-c938-668877662222",
      "categoryName": "Utilities",
      "totalAmount": 350.00,
      "percentage": 16.67
    },
    {
      "categoryId": "880h1733-h62e-74g7-d049-779988773333",
      "categoryName": "Entertainment",
      "totalAmount": 280.75,
      "percentage": 13.37
    }
  ],
  "monthlyTrend": [
    {
      "month": 2,
      "year": 2026,
      "income": 2500.00,
      "expense": 800.00,
      "net": 1700.00
    },
    {
      "month": 3,
      "year": 2026,
      "income": 2800.00,
      "expense": 900.00,
      "net": 1900.00
    },
    {
      "month": 4,
      "year": 2026,
      "income": 2700.00,
      "expense": 400.00,
      "net": 2300.00
    }
  ]
}
```

**Response Structure:**

**Summary:**
- `totalBalance`: Total across all accounts
- `totalIncome`: Total income in period
- `totalExpense`: Total expenses in period
- `netChange`: Income - Expenses
- `transactionCount`: Total transactions count

**By Account:**
- `accountId`: Account unique identifier
- `accountName`: Account display name
- `accountType`: Account type (ContaCorrente, ContaPoupanca, Carteira)
- `balance`: Current balance
- `transactions`: Transaction count in period

**Top Categories:**
- `categoryId`: Category identifier
- `categoryName`: Category name
- `totalAmount`: Total expenses in category
- `percentage`: Percentage of total expenses

**Monthly Trend:**
- `month`: Month number (1-12)
- `year`: Year
- `income`: Total income for month
- `expense`: Total expenses for month
- `net`: Net balance change

---

## Important Notes

### Date Range Defaults
- **Monthly Summary**: If no month/year specified, defaults to current month/year
- **Expenses by Category**: Defaults to last 30 days if not specified
- **Balance Evolution**: Defaults to last 90 days if not specified
- **Dashboard**: Defaults to last 30 days if not specified

### Calculations
- **Net Balance**: Income - Expenses
- **Percentage**: Category amount / Total expenses * 100
- **Average Transaction**: Total amount / Number of transactions

### Data Accuracy
- All calculations are performed on transactions that occurred within the specified date range
- Future transactions (with `occurredAt` in the future) are included
- Deleted transactions are excluded from reporting
- All amounts are in the system's default currency

### Performance
- Dashboard summary query may include all periods for trend calculation
- Large date ranges with multiple accounts may impact response time
- Results are sorted by date (oldest to newest) and by category/account name

### Authorization
- Users only see their own data
- Date ranges respect all user transactions regardless of account ownership
- No cross-user data is leaked in responses

---

## Implementation Notes

- All reporting calculations are done via database queries for optimal performance
- Decimal precision is 2 places (currency standard)
- Timestamps are in UTC format (ISO 8601)
- Missing transactions for a date are treated as zero change
- Percentage values are rounded to 2 decimal places
- Categories with zero transactions are excluded from category reports
