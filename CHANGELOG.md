# Changelog

Todas as mudanças notáveis deste projeto serão documentadas neste arquivo.

O formato é baseado em [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
e este projeto adere ao [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.1.0] - 2025-08-30

### Adicionado
- Funcionalidade completa de listagem de categorias
- Endpoint GET /api/categories implementado
- Interface visual para visualização de categorias
- Sistema de cards responsivo para categorias
- Exibição de ID da categoria (primeiros 8 caracteres)

### Modificado
- Modelo Category atualizado para corresponder exatamente à resposta da API
- Removida propriedade CreatedAt do modelo (não retornada pela API)
- Interface atualizada para mostrar informações corretas das categorias

### Corrigido
- Sincronização entre modelo de dados e resposta da API
- Exibição correta das informações de categoria na interface

## [1.0.0] - 2025-08-30

### Adicionado
- Sistema de autenticação JWT
- Login/Logout de usuários
- Dashboard com overview financeiro
- Proteção de rotas com autorização
- Layout responsivo com Bootstrap 5
- Gerenciamento de estado de autenticação
- Interceptação automática de requisições HTTP com token
- Sistema de criação de categorias
- Endpoint POST /api/categories/add-category implementado
- Modal para criação de categorias com validação
- Feedback visual com sistema de alerts
- Navegação inteligente baseado no estado de autenticação

### Funcionalidades Principais
- **Autenticação**: Sistema completo de login/logout com JWT
- **Dashboard**: Área protegida com estatísticas financeiras
- **Categorias**: Criação e listagem de categorias de transações
- **Layout**: Interface responsiva e moderna
- **Segurança**: Proteção de rotas e gerenciamento seguro de tokens

### Tecnologias
- Blazor WebAssembly (.NET 9)
- Bootstrap 5 + FontAwesome
- Blazored.LocalStorage para persistência
- Arquitetura limpa com separação de responsabilidades

---

## Tipos de Mudanças
- `Added` para novas funcionalidades
- `Changed` para mudanças em funcionalidades existentes
- `Deprecated` para funcionalidades que serão removidas
- `Removed` para funcionalidades removidas
- `Fixed` para correções de bugs
- `Security` para correções de vulnerabilidades
