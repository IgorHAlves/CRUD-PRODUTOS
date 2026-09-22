# CRUD Produtos API 🚀

Esta é uma API REST desenvolvida em **.NET 10** para o gerenciamento de produtos e usuários. O projeto utiliza **PostgreSQL** como banco de dados e implementa segurança via **JWT (JSON Web Token)** com permissões baseadas em perfis.

## 🛠 Tecnologias e Ferramentas

* **Linguagem:** C# (.NET 10)
* **Banco de Dados:** PostgreSQL 17 (via Docker, porta `5434`)
* **ORM:** Entity Framework Core 10
* **Autenticação:** JWT Bearer com Roles (Admin/Padrao)
* **Testes:** xUnit, Moq e Shouldly
* **Documentação:** Swagger (OpenAPI)

## 📌 Funcionalidades

### Autenticação e Usuários
* **Registro:** Auto-cadastro público. O perfil **não** é aceito do cliente: todo usuário criado por este endpoint nasce como `Padrao`.
* **Promoção de perfil:** `PUT /api/auth/usuarios/{id}/role`, restrito a administradores.
* **Segurança:** Senhas armazenadas com hash BCrypt, mínimo de 8 caracteres.
* **Login:** Autenticação que gera um token JWT, com validade configurável em `Jwt:ExpireMinutes`.

### Produtos
* **CRUD Completo:** Criar, Visualizar, Editar e Deletar produtos.
* **Listagem Paginada:** Endpoint para listar produtos com suporte a paginação.
* **Busca Avançada:** Filtro de produtos por nome diretamente no banco de dados.
* **Proteção de Rotas:** Endpoints de escrita (Criar/Editar/Deletar) restritos a usuários com a Role `Admin`.

## ⚙️ Como Rodar o Projeto

### 1. Subir o banco (PostgreSQL)

O projeto usa um container dedicado na porta **5434**, para não conflitar com
outras instâncias de Postgres que você já tenha na `5432`:

```bash
docker run -d --name crud-produtos-postgres \
  -e POSTGRES_USER=produtos \
  -e POSTGRES_PASSWORD=SUA_SENHA \
  -e POSTGRES_DB=produtosdb \
  -p 5434:5432 \
  --restart unless-stopped \
  postgres:17
```

Comandos úteis:

```bash
docker ps --filter name=crud-produtos-postgres   # conferir se está no ar
docker stop crud-produtos-postgres               # parar
docker start crud-produtos-postgres              # subir de novo
docker rm -f crud-produtos-postgres              # remover (apaga os dados)
```

### 2. Configurar os segredos

A string de conexão e a chave JWT **não ficam versionadas**. Configure-as com o
gerenciador de segredos do .NET (em produção, use variáveis de ambiente):

```bash
dotnet user-secrets set "ConnectionStrings:DefaultConnection" \
  "Host=localhost;Port=5434;Database=produtosdb;Username=produtos;Password=SUA_SENHA" \
  --project CRUD.PRODUTOS.API

# A chave precisa ter no mínimo 32 caracteres
dotnet user-secrets set "Jwt:Key" "$(openssl rand -base64 48)" --project CRUD.PRODUTOS.API
```

A aplicação valida essa configuração na inicialização e **não sobe** com a chave
ausente ou curta — não existe chave padrão embutida no código.

### 3. Rodar as Migrations (criação das tabelas)

Na raiz da solution, com o container no ar:

```bash
# caso ainda não tenha a ferramenta do EF Core instalada
dotnet tool install --global dotnet-ef

dotnet ef database update --project CRUD.PRODUTOS.DATA --startup-project CRUD.PRODUTOS.API
```

### 4. Executar a Aplicação

```bash
dotnet run --project CRUD.PRODUTOS.API
```

O Swagger fica disponível em `/swagger` quando o ambiente é `Development`.

## 🧪 Testes Unitários

O projeto possui testes utilizando xUnit, Moq e Shouldly, cobrindo os serviços de
autenticação e produtos (isolados do banco, com dublês de repositório), os
repositórios (EF Core InMemory) e a validação dos DTOs.

Para rodar os testes:

```bash
dotnet test
```

## 🔑 Utilizando a Autenticação

Para testar as rotas protegidas da API, siga os passos abaixo:

1. **Registrar Usuário**: Utilize o endpoint `POST /api/auth/registrar`.
   * **Exemplo de Payload**:
     ```json
     {
       "login": "admin",
       "senha": "senhaSegura1"
     }
     ```
   * O campo `role` é ignorado/rejeitado: o usuário é criado como `Padrao`.
     Para ter um administrador, promova o usuário com
     `PUT /api/auth/usuarios/{id}/role` (exige um token de `Admin`). No
     primeiro setup, como ainda não existe nenhum administrador, promova pelo
     banco:

     ```bash
     docker exec crud-produtos-postgres psql -U produtos -d produtosdb \
       -c "UPDATE \"Usuarios\" SET \"Role\"='Admin' WHERE \"Login\"='admin';"
     ```
2. **Obter Token**: Realize o login no endpoint `POST /api/auth/login` com as credenciais criadas para receber o seu **Token JWT** e a data de expiração.

3. **Configurar o Swagger**:
   * Clique no botão **Authorize** (ícone do cadeado verde) localizado no topo da página do Swagger.
   * No campo **Value**, cole apenas o código do token (sem o prefixo `Bearer`).
   * Clique em **Authorize** e depois em **Close**.


4. **Acessar Rotas**: As rotas protegidas pelo atributo `[Authorize(Roles = "Admin")]` estarão liberadas enquanto o token for válido.

> **Nota**: Se você tentar acessar um recurso de administrador com um usuário de role `Padrao`, a API retornará um erro **403 Forbidden**.
    Clique no botão Authorize (ícone do cadeado) no topo do Swagger.

## Swagger da API

<img width="1498" height="923" alt="image" src="https://github.com/user-attachments/assets/5d762f6b-56d4-4e30-973d-7142452b5470" />
