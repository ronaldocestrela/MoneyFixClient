# Tenancy (Removido)

O modulo multi-tenant foi **completamente removido** da API MoneyFix.

## Motivo

O MoneyFix opera como sistema de gestao financeira pessoal, com isolamento por usuario (`UserId`). A camada de multi-tenancy era legado de uma arquitetura anterior e adicionava complexidade desnecessaria:

- `TenantMiddleware` bloqueava rotas que nao tinham header/claim de tenant
- Novos usuarios registrados sem tenant nao conseguiam acessar endpoints de Finance, Reporting, Audit e Integrations
- O campo `TenantId` em `ApplicationUser` e `RefreshToken` era sempre `Guid.Empty`

## O que foi removido

- `TenantMiddleware` e todo o pipeline de resolucao de tenant
- `TenancyController` (endpoint `POST /api/tenancy/onboard` que retornava 410 Gone)
- Entidade `Tenant` e tabela `Tenants` no banco de dados
- Coluna `TenantId` das tabelas `ApplicationUsers` e `RefreshTokens`
- Interfaces `ICurrentTenantService`, `ICurrentTenantAccessor`, `IMultiTenantEntity`
- Classes base `TenantEntity` e `AggregateRoot` (dependentes de tenant)
- Indice composto `IX_ApplicationUsers_TenantId_Email` (substituido por `IX_ApplicationUsers_Email`)

## Modelo atual

- Cada usuario se registra via `POST /api/identity/register`
- Isolamento de dados e feito por `UserId` em todas as queries
- Nenhuma rota exige header `X-Tenant-Id` ou claim de tenant
- O JWT contem apenas `sub` (user id) e `email`

## Migracao

A migration `RemoveTenancyLegacy` executa automaticamente no startup e realiza:
1. Drop da tabela `Tenants`
2. Drop da coluna `TenantId` de `ApplicationUsers` e `RefreshTokens`
3. Substituicao do indice unico de `(TenantId, Email)` para apenas `(Email)`
