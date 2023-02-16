using System.Configuration;
using System.Data;
using BaltaDataAccess.Models;
using Dapper;
using Microsoft.Data.SqlClient;

Console.Clear();

string connectionString = ConfigurationManager
                        .ConnectionStrings["SqlServerConnectionString"].ConnectionString;

using (var connection = new SqlConnection(connectionString))
{
    // var commandSql = "DELETE FROM [Category] WHERE [Title] = 'Amazon AWS'";
    // connection.Execute(commandSql);
    // CreateCategory(connection);
    // CreateManyCategories(connection);
    // UpdateCategory(connection);
    // DeleteCategory(connection);
    // ListCategories(connection);
    // ExecuteProcedure(connection);
    // ExecuteReadProcedure(connection);
    // ExecuteScalar(connection);
    // ReadView(connection);
    // OneToOne(connection);
    // OneToMany(connection);
    // QueryMultiple(connection);
    // SelectIn(connection);
    // Like(connection, "front");
    Transaction(connection);
}

static void ListCategories(SqlConnection connection)
{
    // O Dapper faz o mapeamento das colunas e linhas conforme a Model.
    var categories = connection.Query<Category>("SELECT [Id], [Title] FROM [Category] ORDER By [Title]");
    foreach (var item in categories)
    {
        Console.WriteLine($"{item.Id} - {item.Title}");
    }
}

static void CreateCategory(SqlConnection connection)
{
    var category = new Category();
    category.Id = Guid.NewGuid();
    category.Title = "Amazon AWS";
    category.Url = "amazon";
    category.Summary = "AWS Cloud";
    category.Order = 8;
    category.Description = "Categoria destinada a serviços AWS";
    category.Featured = false;

    /*
        Por segurança, não faça concatenações diretamente na query,
        evitando ataques via SQL Injection.
        Utilize SQL Parameters.
    */
    var commandSql = @"
    INSERT INTO [Category] VALUES 
    (
        @Id,
        @Title,
        @Url,
        @Summary,
        @Order,
        @Description,
        @Featured
    )";

    var rows = connection.Execute(commandSql, new
    {
        category.Id,
        category.Title,
        category.Url,
        category.Summary,
        category.Order,
        category.Description,
        category.Featured
    });
    Console.WriteLine($"{rows} registros inseridos.");
}

static void CreateManyCategories(SqlConnection connection)
{
    var category = new Category();
    category.Id = Guid.NewGuid();
    category.Title = "Amazon AWS";
    category.Url = "amazon";
    category.Summary = "AWS Cloud";
    category.Order = 8;
    category.Description = "Categoria destinada a serviços AWS";
    category.Featured = false;

    var category2 = new Category();
    category2.Id = Guid.NewGuid();
    category2.Title = "Nova Amazon AWS";
    category2.Url = "nova-amazon";
    category2.Summary = "Nova AWS Cloud";
    category2.Order = 9;
    category2.Description = "Nova Categoria destinada a serviços AWS";
    category2.Featured = true;

    var commandSql = @"
    INSERT INTO [Category] VALUES 
    (
        @Id,
        @Title,
        @Url,
        @Summary,
        @Order,
        @Description,
        @Featured
    )";

    // Passa um array de itens
    var rows = connection.Execute(commandSql, new[]
    {
        new
        {
            category.Id,
            category.Title,
            category.Url,
            category.Summary,
            category.Order,
            category.Description,
            category.Featured
        },
        new
        {
            category2.Id,
            category2.Title,
            category2.Url,
            category2.Summary,
            category2.Order,
            category2.Description,
            category2.Featured
        }
    });
    Console.WriteLine($"{rows} registros inseridos.");
}

static void UpdateCategory(SqlConnection connection)
{
    var commandSql = "UPDATE [Category] SET [Title] = @title WHERE [Id] = @id";

    var rows = connection.Execute(commandSql, new
    {
        id = new Guid("6872c0dc-d6e6-4fcf-9bea-acd0b899df29"),
        title = "AI Integration"
    });
    Console.WriteLine($"{rows} registros atualizados.");
}

static void DeleteCategory(SqlConnection connection)
{
    var commandSql = "DELETE FROM [Category] WHERE [Id] = @id";

    var rows = connection.Execute(commandSql, new
    {
        id = new Guid("6872c0dc-d6e6-4fcf-9bea-acd0b899df29"),
        title = "AI Integration"
    });
    Console.WriteLine($"{rows} registros apagados.");
}

static void ExecuteProcedure(SqlConnection connection)
{
    var procedure = "[spDeleteStudent]";
    var pars = new { StudentId = "d7f255d0-c887-4f17-9285-13f3a559abc7" };

    var result = connection.Execute(procedure, pars, commandType: CommandType.StoredProcedure);
    Console.WriteLine($"{result} linha(s) afetada(s).");
}

static void ExecuteReadProcedure(SqlConnection connection)
{
    var procedure = "[spGetCoursesByCategory]";
    var pars = new { CategoryId = "b4c5af73-7e02-4ff7-951c-f69ee1729cac" };

    /*
        Lista do tipo IEnumerable dinâmico, utilizando o Query sem tipagem.
        O tipo dinamico não exibe as propriedades via Intellisense, pois o retorno
        vem em tempo de execução, e não em tempo de compilação.
    */
    var courses = connection.Query(procedure, pars, commandType: CommandType.StoredProcedure);

    foreach (var item in courses)
    {
        Console.WriteLine($"{item.Id} - {item.Title}");
    }
}

static void ExecuteScalar(SqlConnection connection)
{
    // Não é informado o Id da Categoria via aplicação.
    var category = new Category();
    category.Title = "Amazon AWS";
    category.Url = "amazon";
    category.Summary = "AWS Cloud";
    category.Order = 8;
    category.Description = "Categoria destinada a serviços AWS";
    category.Featured = false;

    /*
        Gera o Id somente no SQL SERVER com o INSERT.
        Caso o Id seja do tipo Identity Seed, pode ser passada a instrução 
        abaixo do insert SELECT SCOPE_IDENTITY(), que retorna o Id gerado.

        Ex: INSERT INTO [Category] VALUES 
            (
                NEWID(),
                @Title,
                @Url,
                @Summary,
                @Order,
                @Description,
                @Featured
            )
            SELECT SCOPE_IDENTITY()
    */

    // OUTPUT inserted | retorna o valor do campo inserido.
    var commandSql = @"
    INSERT INTO [Category]
    OUTPUT inserted.Id
    VALUES 
    (
        NEWID(),
        @Title,
        @Url,
        @Summary,
        @Order,
        @Description,
        @Featured
    )";

    // Retorna o Id inserido
    var id = connection.ExecuteScalar<Guid>(commandSql, new
    {
        category.Title,
        category.Url,
        category.Summary,
        category.Order,
        category.Description,
        category.Featured
    });
    Console.WriteLine($"A categoria inserida foi: {id}.");
}

static void ReadView(SqlConnection connection)
{
    var sql = "SELECT TOP 10 * FROM [vwCourses]";
    var courses = connection.Query(sql);

    foreach (var item in courses)
    {
        Console.WriteLine($"{item.Id} - {item.Tag} - {item.Title}");
    }
}

static void OneToOne(SqlConnection connection)
{
    var sql = @"
        SELECT * FROM
        [CareerItem]
        INNER JOIN [Course]
        ON [CareerItem].[CourseId] = [Course].[Id]";

    /*
        Mapeia os objetos do tipo CareerItem e Course.
        A função de mapeamento define que o objeto course será atribuído
        ao objeto careerItem como uma propriedade Course.

        splitOn indica em qual coluna a consulta deve ser "dividida"
        para mapear as informações para dois objetos diferentes.

        O resultado do método Query é uma coleção de objetos do tipo CareerItem.
    */
    var items = connection.Query<CareerItem, Course, CareerItem>(
        sql,
        (careerItem, course) =>
        {
            careerItem.Course = course;
            return careerItem;
        }, splitOn: "Id");

    foreach (var item in items)
    {
        Console.WriteLine($"Carreira: {item.Title} - Curso: {item.Course.Title}");
    }
}

static void OneToMany(SqlConnection connection)
{
    var sql = @"
        SELECT 
            [Career].[Id],
            [Career].[Title],
            [CareerItem].[CareerId],
            [CareerItem].[Title]
        FROM 
            [Career] 
        INNER JOIN 
            [CareerItem] ON [CareerItem].[CareerId] = [Career].[Id]
        ORDER BY
            [Career].[Title]";

    /*
        Mapeia os objetos do tipo Career e CareerItem.
        O método Query<> recebe uma carreira, popula com cada carreira 
        com items da carreira, resultando em uma coleção de objetos do tipo carreira.
    */

    var careers = new List<Career>();

    // Preenche a lista de carreiras, adicionando as carreiras.
    // Dentro de cada carreira, adiciona cada item relacionado a carreira.
    var items = connection.Query<Career, CareerItem, Career>(
        sql,
        (career, item) =>
        {
            // Busca na lista de carreiras por uma carreira com o mesmo Id da carreira atual.
            var existingCareer = careers.Where(x => x.Id == career.Id).FirstOrDefault();

            // Se não houver uma carreira com o mesmo Id na lista,
            if (existingCareer == null)
            {
                // a carreira atual é armazenada na variável existingCareer
                existingCareer = career;
                // e o item é adicionado à lista de itens da carreira.
                existingCareer.Items.Add(item);
                // Finalmente, a carreira é adicionada à lista de carreiras
                careers.Add(existingCareer);
            }
            else
            {
                // Caso a carreira tenha sido encontrada anteriormente,
                // apenas o item da carreira é adicionado a carreira existente.
                existingCareer.Items.Add(item);
            }
            return career;
        }, splitOn: "CareerId");

    // Percorre cada carreira da lista de carreiras.
    foreach (var career in careers)
    {
        Console.WriteLine("");
        Console.WriteLine($"Carreira: {career.Title}");

        // Percorrre cada item dentro de uma carreira.
        foreach (var item in career.Items)
        {
            Console.WriteLine($" - {item.Title}");
        }
    }
}

static void QueryMultiple(SqlConnection connection)
{
    /// <summary>
    /// Essa é a forma mais otimizada de fazer multiplas queries usando uma mesma conexão.
    /// </summary>
    /// <param name="query">Informe a query, separadas por ponto-e-virgula.</param>
    /// <returns>Retorna a execução de todas as queries.</returns>

    var query = "SELECT * FROM [Category]; SELECT * FROM [Course]";

    using (var multi = connection.QueryMultiple(query))
    {
        var categories = multi.Read<Category>();
        var courses = multi.Read<Course>();

        foreach (var item in categories)
        {
            Console.WriteLine($"Categoria: {item.Title}");
        }

        foreach (var item in courses)
        {
            Console.WriteLine($"Cursos: {item.Title}");
        }
    }
}

static void OneToOneSemOO(SqlConnection connection)
{
    // Nesse método temos apenas o tabelão, resultado do join.
    var sql = @"
        SELECT * FROM [CareerItem]
        INNER JOIN [Course]
        ON [CareerItem].[CourseId] = [Course].[Id]";

    var items = connection.Query(sql);

    foreach (var item in items)
    {
        Console.WriteLine($"{item.DurationInMinutes}");
    }
}

static void SelectIn(SqlConnection connection)
{
    var query = @"SELECT * FROM Career WHERE Id IN @Id";
    var param = new
    {
        Id = new[]{
            "01ae8a85-b4e8-4194-a0f1-1c6190af54cb",
            "4327ac7e-963b-4893-9f31-9a3b28a4e72b"
        }
    };

    var items = connection.Query<Career>(query, param);

    // Também poderia ser passado o objeto anonimo diretamente como parametro.
    // var items = connection.Query<Career>(query, new
    // {
    //     Id = new[]{
    //         "01ae8a85-b4e8-4194-a0f1-1c6190af54cb",
    //         "4327ac7e-963b-4893-9f31-9a3b28a4e72b"
    //     }
    // });

    foreach (var item in items)
    {
        Console.WriteLine($"Carreiras: {item.Title}");
    }
}

static void Like(SqlConnection connection, string term)
{
    var query = "SELECT * FROM [Course] WHERE Title LIKE @exp";
    var param = new
    {
        exp = $"%{term}%"
    };

    var items = connection.Query<Career>(query, param);
    foreach (var item in items)
    {
        Console.WriteLine($"Carreiras: {item.Title}");
    }
}

static void Transaction(SqlConnection connection)
{
    var category = new Category();
    category.Id = Guid.NewGuid();
    category.Title = "Nao salve, faça ROLLBACK!";
    category.Url = "amazon";
    category.Summary = "AWS Cloud";
    category.Order = 8;
    category.Description = "Categoria destinada a serviços AWS";
    category.Featured = false;

    /*
        Por segurança, não faça concatenações diretamente na query,
        evitando ataques via SQL Injection.
        Utilize SQL Parameters.
    */
    var commandSql = @"
    INSERT INTO [Category] VALUES 
    (
        @Id,
        @Title,
        @Url,
        @Summary,
        @Order,
        @Description,
        @Featured
    )";

    // A conexão é fechada pelo using, após concluir o transaction.
    connection.Open();
    using (var transaction = connection.BeginTransaction())
    {
        var rows = connection.Execute(commandSql, new
        {
            category.Id,
            category.Title,
            category.Url,
            category.Summary,
            category.Order,
            category.Description,
            category.Featured
        }, transaction);

        // É necessario informar o Commit para gravar a transação no banco.
        //transaction.Commit();
        transaction.Rollback();
        Console.WriteLine($"{rows} registros inseridos.");
    }
}

void exemploDeConsultaADONET()
{
    using (var connection = new SqlConnection(connectionString))
    {
        Console.Clear();
        Console.WriteLine("Conectado");

        // Utilizando o ADO.NET | Access Data Object
        connection.Open();
        using (var command = new SqlCommand())
        {
            command.Connection = connection;
            command.CommandType = System.Data.CommandType.Text;
            command.CommandText = "SELECT [Id], [Title] FROM [Category]";

            // O objeto SqlDataReader é utilizado pelo Dapper e outros frameworks,
            // sendo a forma mais rápida para acesso a dados.
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                Console.WriteLine($"{reader.GetGuid(0)} | {reader.GetString(1)}");
            }
        }
    }
}

void antigaFormaDeConexao()
{
    /*
        Metódo antigo para conectar/desconectar do banco.
        Essa maneira pode ser problematica, devido a necessidade
        gerenciar a abertura e fechamento da conexão e posterior
        liberação de memória com a destruição do objeto de conexão.

        A melhor forma é utilizar o using, que faz esse gerenciamento,
        tanto para conexões quanto para comandos.
    */

    // Cria o objeto para conexão.
    var connection = new SqlConnection();
    connection.Open();
    // Instruções SQL com a conexão aberta.
    // Faz Insert...
    // Faz update...
    connection.Close();

    // destroi o objeto da conexão, sendo necessario instanciar novamente.
    connection.Dispose();
}