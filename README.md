# Production Order System

Sistema completo para gerenciamento de ordens de produção, composto por uma API RESTful em .NET e um frontend React com TypeScript.

## 📋 Sobre o Projeto

Sistema completo para gerenciamento do ciclo de vida das ordens de produção, desde o planejamento até a finalização, com interface moderna e responsiva.

## 🏗️ Arquitetura do Sistema

### Backend (API .NET)

- **.NET 8** - Framework principal
- **Entity Framework Core** - ORM para acesso a dados
- **SQL Server** - Banco de dados
- **ASP.NET Core** - Framework web
- **Swagger/OpenAPI** - Documentação da API

### Frontend (React + TypeScript)

- **React 18** - Biblioteca UI
- **TypeScript** - Tipagem estática
- **Material-UI (MUI)** - Componentes UI
- **Axios** - Cliente HTTP
- **Vite** - Build tool e dev server

## 📦 Estrutura do Projeto

### Backend

```
ProductionOrderApi/
├── Controllers/          # Controladores da API
├── Models/              # Modelos de dados e DTOs
├── Services/            # Lógica de negócio
├── Repositories/        # Acesso a dados
├── Enums/              # Enumerações do sistema
└── Data/               # Contexto do banco de dados
```

### Frontend

```
src/
├── components/          # Componentes React
│   ├── ProductionOrderForm.tsx
│   ├── ProductionOrderEditForm.tsx
│   ├── ProductionOrderList.tsx
│   └── ProgressBar.tsx
├── services/           # Serviços de API
│   └── api.ts
├── types/              # Definições TypeScript
│   └── productionOrder.ts
├── App.tsx             # Componente principal
└── main.tsx           # Ponto de entrada
```

## 🗃️ Modelos Principais

### ProductionOrder

- Gerencia ordens de produção
- Status: Planejada, EmProducao, Finalizada
- Controla quantidades planejadas vs produzidas

### ProductionLog

- Registra a produção em tempo real
- Associa produção a recursos
- Atualiza automaticamente as quantidades produzidas

### Product

- Catálogo de produtos
- Associação com ordens de produção

### Resource

- Recursos de produção (máquinas, equipamentos)
- Status: Disponivel, EmUso, Parado

## 🎯 Funcionalidades do Frontend

### Listagem de Ordens

- ✅ Tabela paginada com busca
- ✅ Filtro por número da ordem
- ✅ Controle de itens por página (5-1000)
- ✅ Barra de progresso visual
- ✅ Atualização em tempo real

### Gestão de Ordens

- ✅ Criação de novas ordens
- ✅ Edição de ordens existentes
- ✅ Atualização de status com um clique
- ✅ Validação de formulários
- ✅ Feedback visual durante operações

### Interface do Usuário

- ✅ Design responsivo com Material-UI
- ✅ Snackbars para feedback
- ✅ Loading states
- ✅ Ícones intuitivos para ações
- ✅ Progress bars visuais

## 🔌 Endpoints da API

### Ordens de Produção (`/api/orders`)

- `GET /api/orders` - Lista todas as ordens
- `GET /api/orders/{id}` - Obtém ordem por ID
- `POST /api/orders` - Cria nova ordem
- `PATCH /api/orders/{id}` - Atualiza ordem existente
- `GET /api/orders/status/{status}` - Filtra por status
- `GET /api/orders/status/possible` - Lista status possíveis

### Logs de Produção (`/api/productionlogs`)

- `GET /api/productionlogs` - Lista todos os logs
- `GET /api/productionlogs/{id}` - Obtém log por ID
- `POST /api/productionlogs` - Cria novo log
- `GET /api/productionlogs/order/{orderId}` - Logs por ordem

### Produtos (`/api/products`)

- `GET /api/products` - Lista todos os produtos

## ⚙️ Configuração e Execução

### Pré-requisitos

- .NET 8 SDK
- SQL Server
- Node.js 20.19.4
- npm

### Configuração do Backend

1. Configure a connection string no `appsettings.json`:

```json
"ConnectionStrings": {
    "DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=ProductionOrderDb;Trusted_Connection=true;MultipleActiveResultSets=true;TrustServerCertificate=true"
  }
```

2. Execute as migrações e seed do banco:

```bash
dotnet ef migrations add InitialCreate

dotnet ef database update
```

3. Execute a API:

```bash
dotnet run
# API disponível em: https://localhost:5079
```

### Configuração do Frontend

1. Instale as dependências:

```bash
npm install
```

2. Execute em modo desenvolvimento:

```bash
npm run dev
# Frontend disponível em: http://localhost:3000
```

3. Build para produção:

```bash
npm run build
```

4. Execute em modo de produção:

```bash
npm run start
# Frontend disponível em: http://localhost:4173
```

### Configuração de URLs

O frontend está configurado para se comunicar com a API em `http://localhost:5079`. Para alterar, modifique a constante `API_BASE_URL` em `src/services/api.ts`.

## 📊 Funcionalidades

### Gestão de Ordens

- Criação e atualização de ordens de produção
- Controle de status (Planejada → EmProducao → Finalizada)
- Validação de datas e quantidades
- Impedimento de duplicação de números de ordem

### Logs de Produção

- Registro em tempo real da produção
- Associação com recursos
- Atualização automática de quantidades produzidas
- Validação para não exceder quantidade planejada

### Interface do Usuário

- Busca e paginação de ordens
- Barra de progresso visual
- Atualização de status com um clique
- Formulários com validação
- Feedback visual para todas as ações

## 🛡️ Tratamento de Erros

### Backend

- `400 Bad Request` - Dados inválidos
- `404 Not Found` - Recurso não encontrado
- `409 Conflict` - Conflito de dados
- `503 Service Unavailable` - Erros internos

### Frontend

- Snackbars para feedback de sucesso/erro
- Loading states durante operações
- Tratamento de erros de rede
- Validação de formulários em tempo real

## 🔄 Status da Ordem de Produção

| Status       | Descrição                     | Ação no Frontend    |
| ------------ | ----------------------------- | ------------------- |
| `Planejada`  | Ordem criada mas não iniciada | ▶️ Iniciar Produção |
| `EmProducao` | Ordem em andamento            | ✅ Finalizar Ordem  |
| `Finalizada` | Ordem concluída               | 🔄 Reabrir Ordem    |

## 📝 Exemplos de Uso

### Criar Ordem de Produção (Frontend)

```typescript
// Formulário automático com validação
// Número: ORD-12345
// Produto: PROD-001 - Produto Exemplo
// Quantidade: 1000
// Status: Planejada
// Data: 2024-01-15
```

### Atualizar Status (Frontend)

```typescript
// Clique no botão de ação na lista
// Transições automáticas:
// Planejada → Em Produção → Finalizada
```

### Buscar e Filtrar

```typescript
// Campo de busca por número da ordem
// Paginação com controles
// Filtro por quantidade de itens por página
```

## 🎨 Componentes do Frontend

### ProductionOrderList

- Tabela principal com todas as ordens
- Busca, paginação e filtros
- Ações de status e edição

### ProductionOrderForm

- Modal para criação de ordens
- Validação e seleção de produtos
- Formatação automática de números

### ProductionOrderEditForm

- Modal para edição de ordens
- Carregamento de dados existentes
- Atualização em tempo real

### ProgressBar

- Barra visual de progresso
- Cores baseadas no percentual
- Exibição de porcentagem

## 🔧 Desenvolvimento

### Estrutura de Tipos TypeScript

```typescript
interface ProductionOrder {
  id: number;
  orderNumber: string;
  productCode: string;
  quantityPlanned: number;
  quantityProduced: number;
  status: number;
  startDate: string;
  endDate?: string;
}
```

### Serviços de API

```typescript
// Serviços organizados por entidade
productionOrderService.getOrders();
productionOrderService.updateOrderStatus();
productService.getProducts();
```

## 🚀 Scripts Disponíveis

### Frontend

- `npm run dev` - Servidor de desenvolvimento
- `npm run build` - Build para produção
- `npm run start` - Servidor de produção
- `npm run lint` - Análise de código
- `npm run preview` - Preview do build

### Backend

- `dotnet ef migrations add InitialCreate` - Cria migração
- `dotnet ef database update` - Cria banco de dados
- `dotnet run` - Execução normal da API e realiza seed
- `dotnet build` - Build do projeto

## 🎯 Próximos Passos

### Melhorias Planejadas

- [ ] Implementar autenticação e autorização
- [ ] Adicionar relatórios de produção
- [ ] Implementar notificações
- [ ] Dashboard administrativo
- [ ] Exportação de relatórios
- [ ] Gráficos e métricas

### Funcionalidades Futuras

- [ ] Upload de arquivos
- [ ] Integração com ERP
- [ ] Mobile app
- [ ] API de relatórios
- [ ] Cache e performance

---

**Desenvolvido por** - Hugo de Souza Caramez

- **API**: https://localhost:5079
- **Frontend**: http://localhost:3000 ou via build http://localhost:4173
- **Documentação**: https://localhost:5079/swagger
