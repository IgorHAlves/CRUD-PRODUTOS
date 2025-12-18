# CRUD Produtos API 🚀

Esta é uma API REST desenvolvida em **.NET 8** para o gerenciamento de produtos e usuários. O projeto utiliza **PostgreSQL** como banco de dados e implementa segurança via **JWT (JSON Web Token)** com permissões baseadas em perfis.

## 🛠 Tecnologias e Ferramentas

* **Linguagem:** C# (.NET 8)
* **Banco de Dados:** PostgreSQL
* **ORM:** Entity Framework Core
* **Autenticação:** JWT Bearer com Roles (Admin/Padrao)
* **Testes:** xUnit, Moq e Shouldly
* **Documentação:** Swagger (OpenAPI)

## 📌 Funcionalidades

### Autenticação e Usuários
* **Registro:** Cadastro de novos usuários com definição de Role (Admin/Padrao).
* **Segurança:** Senhas armazenadas com criptografia (Hash).
* **Login:** Autenticação que gera um token JWT válido por 2 horas.

### Produtos
* **CRUD Completo:** Criar, Visualizar, Editar e Deletar produtos.
* **Listagem Paginada:** Endpoint para listar produtos com suporte a paginação.
* **Busca Avançada:** Filtro de produtos por nome diretamente no banco de dados.
* **Proteção de Rotas:** Endpoints de escrita (Criar/Editar/Deletar) restritos a usuários com a Role `Admin`.

## ⚙️ Como Rodar o Projeto

### 1. Configurar o Banco de Dados (PostgreSQL)
Certifique-se de ter o PostgreSQL rodando. No arquivo `appsettings.json` do projeto **API**, ajuste a string de conexão:

```json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Port=5432;Database=produtosdb;Username=SEU_USUARIO;Password=SUA_SENHA"
}
```
### 2. Rodar as Migrations (Criação das Tabelas)
Abra o terminal no projeto de data (CRUD.PRODUTOS.DATA) e execute os comandos abaixo para criar a estrutura do banco automaticamente:

### Caso não tenha a ferramenta do EF Core instalada globalmente:
dotnet tool install --global dotnet-ef

### Comando para aplicar as migrations ao banco de dados:
dotnet ef database update --project CRUD.PRODUTOS.DATA --startup-project CRUD.PRODUTOS.API

### 3. Executar a Aplicação
dotnet run --project CRUD.PRODUTOS.API

## 🧪 Testes Unitários

O projeto possui testes utilizando xUnit, Moq e Shouldly, cobrindo os serviços de autenticação, produtos e repositórios.

Para rodar os testes:
dotnet test

## 🔑 Utilizando a Autenticação

Para testar as rotas protegidas da API, siga os passos abaixo:

1. **Registrar Usuário**: Utilize o endpoint `POST /api/auth/registrar`.
   * **Exemplo de Payload**:
     ```json
     { 
       "login": "admin", 
       "senha": "123", 
       "role": "Admin" 
     }
     ```
2. **Obter Token**: Realize o login no endpoint `POST /api/auth/login` com as credenciais criadas para receber o seu **Token JWT**.

3. **Configurar o Swagger**:
   * Clique no botão **Authorize** (ícone do cadeado verde) localizado no topo da página do Swagger.
   * No campo **Value**, cole apenas o código do token (sem o prefixo `Bearer`).
   * Clique em **Authorize** e depois em **Close**.


4. **Acessar Rotas**: As rotas protegidas pelo atributo `[Authorize(Roles = "Admin")]` estarão liberadas enquanto o token for válido.

> **Nota**: Se você tentar acessar um recurso de administrador com um usuário de role `Padrao`, a API retornará um erro **403 Forbidden**.
    Clique no botão Authorize (ícone do cadeado) no topo do Swagger.

## Swagger da API

<img width="1498" height="923" alt="image" src="https://github.com/user-attachments/assets/5d762f6b-56d4-4e30-973d-7142452b5470" />
