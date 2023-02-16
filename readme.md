# Dapper - acesso a dados .NET

Aprendizado e revisão de conceitos.

Todos os exemplos com base no excelente curso do [Balta](https://github.com/balta-io/2806).

Dapper - Extensão do SqlClient com diversas melhorias como mapeamento
e encapsulamento das linhas e colunas do banco de dados para objetos.

Requisitos e ferramentas:

1. Ambiente Docker e SQL Server
2. Azure Data Studio
3. Visual Studio Code
4. Pacote Microsoft.Data.SqlClient - dotnet add package Microsoft.Data.SqlClient --version 2.1.0
5. Pacote Dapper - dotnet add package dapper --version 2.0.90
6. Pacote ConfigurationManager - dotnet add package System.Configuration.ConfigurationManager
7. Renomeie o arquivo **app.config.fake** para **app.config**
8. Faça um restore do arquivo **balta.bacpac** para utilizar a base de testes

Boas práticas:

1. Crie uma pasta Models, para armazenar as entidades.
2. Tenha uma classe para representar cada tabela do banco de dados.
3. Cada propriedade do Objeto deve possuir o mesmo nome das colunas da tabela,
permitindo o mapeamento pelo Dapper.
