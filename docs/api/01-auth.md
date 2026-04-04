# Authentication & Identity Endpoints

Base URL: `http://localhost:5000/api/identity`

## 1. Login

**Authenticate user with email and password**

### Request

```
POST /identity/login
```

**Headers:**
```
Content-Type: application/json
```

**Body:**
```json
{
  "email": "user@example.com",
  "password": "Password123!"
}
```

### Response - Success (200 OK)

```json
{
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "550e8400-e29b-41d4-a716-446655440000",
  "expiresIn": 3600,
  "tokenType": "Bearer"
}
```

### Response - Error

**Invalid Credentials (401 Unauthorized):**
```json
{
  "message": "Invalid credentials."
}
```

---

## 2. Register

**Create a new user account**

### Request

```
POST /identity/register
```

**Headers:**
```
Content-Type: application/json
```

**Body:**
```json
{
  "email": "newuser@example.com",
  "password": "Password123!",
  "fullName": "John Doe"
}
```

**Password Requirements:**
- Minimum 8 characters
- At least one uppercase letter
- At least one lowercase letter
- At least one digit
- At least one special character

### Response - Success (200 OK)

```json
{
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "550e8400-e29b-41d4-a716-446655440000",
  "expiresIn": 3600,
  "tokenType": "Bearer"
}
```

### Response - Error

**User Already Exists (409 Conflict):**
```json
{
  "message": "User with this email already exists."
}
```

**Invalid Data (400 Bad Request):**
```json
{
  "errors": [
    "Password does not meet complexity requirements",
    "Email format is invalid"
  ]
}
```

---

## 3. Refresh Token

**Get a new access token using a refresh token**

### Request

```
POST /identity/refresh
```

**Headers:**
```
Content-Type: application/json
```

**Body:**
```json
{
  "refreshToken": "550e8400-e29b-41d4-a716-446655440000"
}
```

### Response - Success (200 OK)

```json
{
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "660f9511-f40c-52e5-b827-557766551111",
  "expiresIn": 3600,
  "tokenType": "Bearer"
}
```

**Note:** The refresh token is rotated on each refresh for security.

### Response - Error

**Invalid or Expired Refresh Token (401 Unauthorized):**
```json
{
  "message": "Invalid or expired refresh token."
}
```

---

## 4. Forgot Password

**Request a password reset token**

### Request

```
POST /identity/forgot-password
```

**Headers:**
```
Content-Type: application/json
```

**Body:**
```json
{
  "email": "user@example.com"
}
```

### Response - Success (200 OK)

```json
{
  "message": "If the account exists and is active, a password reset token was generated.",
  "resetToken": "AQAAAIAARiAA..."
}
```

**Note:** For MVP, the reset token is returned directly. In production, this would be sent via email.

---

## 5. Reset Password

**Reset user password with a reset token**

### Request

```
POST /identity/reset-password
```

**Headers:**
```
Content-Type: application/json
```

**Body:**
```json
{
  "email": "user@example.com",
  "token": "AQAAAIAARiAA...",
  "newPassword": "NewPassword123!"
}
```

### Response - Success (204 No Content)

No response body.

### Response - Error

**Invalid Reset Request (400 Bad Request):**
```json
{
  "message": "Invalid password reset request."
}
```

**Invalid Password (400 Bad Request):**
```json
{
  "errors": [
    "Password does not meet complexity requirements"
  ]
}
```

---

## 6. Logout

**Revoke all refresh tokens and logout user**

### Request

```
POST /identity/logout
```

**Headers:**
```
Authorization: Bearer <access_token>
Content-Type: application/json
```

### Response - Success (204 No Content)

No response body.

### Response - Error

**Unauthorized (401 Unauthorized):**
```json
{
  "message": "Unauthorized"
}
```

---

## 7. Get Current User Profile

**Retrieve authenticated user information**

### Request

```
GET /identity/me
```

**Headers:**
```
Authorization: Bearer <access_token>
```

### Response - Success (200 OK)

```json
{
  "id": "550e8400-e29b-41d4-a716-446655440000",
  "email": "user@example.com",
  "fullName": "John Doe",
  "createdAtUtc": "2026-01-15T08:30:00Z",
  "lastLoginUtc": "2026-04-01T10:30:00Z",
  "isActive": true
}
```

### Response - Error

**Unauthorized (401 Unauthorized):**
```json
{
  "message": "Unauthorized"
}
```

**User Not Found (404 Not Found):**
```json
{
  "message": "Not Found"
}
```

---

## JWT Token Structure

The access token is a JWT with the following claims:

```json
{
  "sub": "550e8400-e29b-41d4-a716-446655440000",
  "email": "user@example.com",
  "name": "John Doe",
  "iat": 1712058600,
  "exp": 1712062200,
  "iss": "moneyfix-api",
  "aud": "moneyfix-client"
}
```

**Token Expiration:** 1 hour (3600 seconds)

---

## Implementation Notes

- Keep the `refreshToken` secure (e.g., HttpOnly cookie)
- Refresh tokens automatically rotate for enhanced security
- Store `accessToken` in memory or secure storage
- All requests to protected endpoints must include the `Authorization` header with the access token
