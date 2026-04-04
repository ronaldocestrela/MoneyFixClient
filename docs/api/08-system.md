# System Endpoints

Base URL: `http://localhost:5000/api/system`

**Authentication:** Not required (public endpoints)

## 1. Health Check

**Check the current status and health of the API service**

### Request

```
GET /system/health
```

**Headers:**
```
No authentication required
```

### Response - Success (200 OK)

```json
{
  "status": "ok",
  "service": "MoneyFix.Host.WebApi",
  "utcNow": "2026-04-01T15:30:45Z"
}
```

**Response Fields:**
- `status`: Service status (`ok` = running normally)
- `service`: Service name identifier
- `utcNow`: Current server time in UTC (ISO 8601 format)

---

## 2. Get Version Information

**Retrieve API version and architecture details**

### Request

```
GET /system/version
```

**Headers:**
```
No authentication required
```

### Response - Success (200 OK)

```json
{
  "framework": ".NET 10",
  "architecture": "Modular Monolith",
  "version": "0.1.0"
}
```

**Response Fields:**
- `framework`: Runtime framework being used
- `architecture`: Application architecture pattern
- `version`: Current API version

---

## Important Notes

### Health Check
- This endpoint is useful for monitoring and load balancing
- Response time indicates API responsiveness
- Always returns 200 if service is operational
- Server time (`utcNow`) can be used to verify clock synchronization

### Version Information
- Version follows semantic versioning (MAJOR.MINOR.PATCH)
- Architecture describes the internal organization pattern
- Framework version indicates .NET version in use
- Configuration is read from `appsettings.json`

### Monitoring
- Implement periodic health checks (e.g., every 60 seconds)
- Use in Kubernetes/Docker health probes
- Monitor response times for performance degradation
- System endpoints are available even during maintene restrictions

### No Authentication Required
- These endpoints are intentionally public
- Useful for uptime monitoring services
- Can be accessed without JWT token
