# Changelog

Todas as mudanças notáveis deste projeto serão documentadas neste arquivo.

O formato é baseado em [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
e este projeto adere ao [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.2.2] - 2025-08-31

### Adicionado
- **Dashboard**: Cores personalizadas das categorias nas transações
  - CategoryColor adicionado ao modelo Transaction
  - Badges coloridos substituem indicadores genéricos de tipo
  - Exibição do nome real da categoria em vez de "Despesa/Receita"
  - Integração visual completa com sistema de categorias

### Melhorado
- **UX**: Dashboard mais informativo e visualmente atraente
  - Cores consistentes entre sistema de categorias e dashboard
  - Loading states mantidos para melhor experiência
  - Design mais moderno com badges coloridos dinamicamente

### Técnico
- Transaction.CategoryColor: nova propriedade para cor da categoria
- Dashboard.razor: badges com style dinâmico baseado na cor da categoria
- Integração com TransactionService.GetLastTransactionsAsync() mantida

## [1.2.1] - 2025-08-31

### Corrigido
- **CRÍTICO**: API de atualização de categorias corrigida
  - Implementado UpdateCategoryRequest com campos obrigatórios (id, userId, name, color)
  - CategoryService.UpdateCategoryAsync agora envia body completo conforme especificação da API
  - Preservação correta de id e userId durante edição
- **UI**: Bootstrap JavaScript adicionado ao index.html
  - Dropdowns agora funcionam corretamente
  - Componentes Bootstrap interativos habilitados
- **UX**: Modal de edição de categorias totalmente funcional
  - IDs únicos para dropdowns previnem conflitos
  - Botões corrigidos para type="button"
  - Logs de debug adicionados para troubleshooting

### Técnico
- API Request Body agora segue especificação:
  ```json
  {
    "id": "uuid",
    "userId": "uuid", 
    "name": "string",
    "color": "string"
  }
  ```
- Bootstrap 5 JavaScript bundle carregado corretamente
- Tratamento de erros HTTP específicos (400/401/403/404)

## [1.2.0] - 2025-08-30

### Adicionado
- Sistema completo de gerenciamento de transações (CRUD)
- Endpoints de transações implementados:
  - POST /api/transactions - Criar transação
  - PUT /api/transactions/{id} - Atualizar transação
  - DELETE /api/transactions/{id} - Excluir transação
  - GET /api/transactions - Listar transações
- Página Transactions.razor com interface completa
- Modais para criação/edição de transações
- Modal de confirmação para exclusão
- Sistema de validação de formulários
- Suporte para tipos de transação (Despesa/Receita)
- Integração com sistema de categorias
- TransactionService registrado no DI container
- Modelos TransactionRequest e Transaction
- Design responsivo com estilos customizados
- Feedback visual com alerts e loading states
- Tratamento detalhado de códigos de erro HTTP

### Funcionalidades de Transações
- Criar transações com descrição, valor, data, tipo e categoria
- Editar transações existentes com validação de propriedade
- Excluir transações com confirmação de segurança
- Visualizar lista de transações ordenada por data
- Diferenciação visual entre receitas e despesas
- Carregamento assíncrono de dados

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
