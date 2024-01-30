<div align="center">
  <h1>Api - Hotel 🏨 </h1>
</div>
---

### Documentação da API 📖

| Método      | Endpoint                 | Descrição       | Requer Autenticação |
| ----------- | ------------------------ | ----------------- | -------------------- |
| POST    | /booking | Cadastra nova reserva | Client          |
| GET   | /booking/id         | Retorna uma reserva | Client     |
| GET    | /city             | Retorna as cidades | Não |
| POST | /city           | Cadastra nova cidade  | Não |
| PUT   | /city           | Atualiza uma cidade | Não |
| GET  | /geo/status | Retorna status da api externa  | Não |
| GET  | /geo/address  | Retorna dados de geolocalização de api externa | Não |
| GET  | /hotel           | Retorna hotéis cadastrados   | Não |
| POST   | /hotel         | Cadastra novo hotel | Admin     |
| POST   | /login         | Valida os dados de login e retorna um token | Não   |
| GET   | /room/id         | Retorna um quarto | Não    |
| POST  | /room         | Cadastra novo quarto |  Admin |
| DELETE | /room/id     | Deleta um quarto |  Admin |
| GET   | /user   |  Retorna usuários   | Admin|
| POST   |  /user  |   Cadastra novos usuários |  Não    |

---
## Descrição

Projeto backend de uma Api de Hotéis construída com Asp.net, banco de dados SQL Server, Entity Framework e JWT Token.
Além das operações no banco de dados, utilizei uma api externa para implementar uma funcionalidade que permite que as pessoas usuárias possam buscar os hotéis mais próximos baseando-se em um endereço.

Explorei recursos do .NET como Logger(ILogger), Cache(IMemory) e Filtros(IActionFilter).

## Testes
Os testes serão adicionados em um novo projeto XUnit.


## 💻 Tecnologias usadas

  * .NET 
  * SQL SERVER
  * Docker
  * JWT


## 🐋 Rodando o banco de dados com Docker-compose
Para subir o banco utilizando docker, no diretório da aplicação execute o comando:

`docker-compose up -d --build`
