using Microsoft.Data.Sqlite;

namespace Dissectr.Models;

class Project
{
    public string Name { get; set; }
    public string VideoFile { get; set; }
    public TimeSpan Interval { get; set; }
    public List<Dimension> Dimensions { get; set; }

    public Project(string name, string videoFile, TimeSpan interval, List<Dimension> dimensions)
    {
        Name = name;
        VideoFile = videoFile;
        Interval = interval;
        Dimensions = dimensions;
    }

    private static SqliteConnection CreateConnection(string path)
    {
        var connectionString = new SqliteConnectionStringBuilder
        {
            DataSource = path,
            Mode = SqliteOpenMode.ReadWriteCreate,
        }.ToString();
        return new SqliteConnection(connectionString);
    }


    private static async Task CreateProjectTable(SqliteConnection connection)
    {
        var command = connection.CreateCommand();
        command.CommandText = @"
            CREATE TABLE IF NOT EXISTS projects (
                id INT PRIMARY KEY NOT NULL,
                name TEXT,
                videoFile TEXT,
                interval TEXT
            );";
        await command.ExecuteNonQueryAsync();
    }

    private static async Task CreateDimensionTable(SqliteConnection connection)
    {
        var command = connection.CreateCommand();
        command.CommandText = @"
            CREATE TABLE IF NOT EXISTS dimensions (
                id TEXT PRIMARY KEY NOT NULL,
                `order` INTEGER NOT NULL,
                name TEXT,
                optional INTEGER
            );";
        await command.ExecuteNonQueryAsync();
    }

    private static async Task CreateDimensionOptionsTable(SqliteConnection connection)
    {
        var command = connection.CreateCommand();
        command.CommandText = @"
            CREATE TABLE IF NOT EXISTS dimension_options (
                id TEXT PRIMARY KEY NOT NULL,
                dimensionId TEXT,
                code INTEGER,
                name TEXT,
                FOREIGN KEY(dimensionId) REFERENCES dimensions(id)
            );";
        await command.ExecuteNonQueryAsync();
    }

    private static SqliteParameter AddParameter(SqliteCommand command, string parameterName, object? value = null)
    {
        var parameter = command.CreateParameter();
        parameter.ParameterName = parameterName;
        if (value is not null)
        {
            parameter.Value = value;
        }
        command.Parameters.Add(parameter);
        return parameter;
    }

    private static async Task InsertDimensions(SqliteConnection connection, IEnumerable<Dimension> dimensions)
    {
        var command = connection.CreateCommand();
        command.CommandText = @"
            INSERT INTO dimensions (id, `order`, name, optional)
            VALUES ($id, $order, $name, $optional);";
        var idParam = AddParameter(command, "$id");
        var orderParam = AddParameter(command, "$order");
        var nameParam = AddParameter(command, "$name");
        var optionalParam = AddParameter(command, "$optional");
        foreach (var dimension in dimensions)
        {
            idParam.Value = dimension.Id;
            orderParam.Value = dimension.Order;
            nameParam.Value = dimension.Name;
            optionalParam.Value = dimension.Optional;
            await command.ExecuteNonQueryAsync();

            await InsertDimensionOptions(connection, dimension.Id, dimension.DimensionOptions);
        }
    }

    private static async Task InsertDimensionOptions(SqliteConnection connection, Guid dimensionId, IEnumerable<DimensionOption> dimensionOptions)
    {
        var command = connection.CreateCommand();
        command.CommandText = @"
            INSERT INTO dimension_options (id, dimensionId, code, name)
            VALUES ($id, $dimensionId, $code, $name);";
        var idParam = AddParameter(command, "$id");
        AddParameter(command, "$dimensionId", dimensionId);
        var codeParam = AddParameter(command, "$code");
        var nameParam = AddParameter(command, "$name");
        foreach ( var dimensionOption in dimensionOptions)
        {
            idParam.Value = dimensionOption.Id;
            codeParam.Value = dimensionOption.Code;
            nameParam.Value = dimensionOption.Name;
            await command.ExecuteNonQueryAsync();
        }
    }

    public static async Task<string> CreateAsync(string path, Project project)
    {
        var fullProjectPath = Path.ChangeExtension(Path.Combine(path, project.Name), ".dissectr");
        using var connection = CreateConnection(fullProjectPath);
        await connection.OpenAsync();
        using var transaction = connection.BeginTransaction();

        await CreateProjectTable(connection);
        await CreateDimensionTable(connection);
        await CreateDimensionOptionsTable(connection);

        var command = connection.CreateCommand();
        command.CommandText = @"
            INSERT INTO projects (id, name, videoFile, interval)
            VALUES ($id, $name, $videoFile, $interval)
        ";
        AddParameter(command, "$id", 1);
        AddParameter(command, "$name", project.Name);
        AddParameter(command, "$videoFile", project.VideoFile);
        AddParameter(command, "$interval", project.Interval);
        await command.ExecuteNonQueryAsync();

        await InsertDimensions(connection, project.Dimensions);

        await transaction.CommitAsync();
        await connection.CloseAsync();
        return fullProjectPath;
    }

    private static async Task<List<DimensionOption>> LoadOptions(SqliteConnection connection, Guid dimensionId)
    {
        var command = connection.CreateCommand();
        command.CommandText = "SELECT id, code, name FROM dimension_options WHERE dimensionId = $dimensionId;";
        AddParameter(command, "$dimensionId", dimensionId);
        using var reader = await command.ExecuteReaderAsync();
        List<DimensionOption> options = new();
        while (await reader.ReadAsync())
        {
            var id = reader.GetGuid(0);
            var code = reader.GetInt32(1);
            var name = reader.GetString(2);
            options.Add(new(id, code, name));
        }
        return options;
    }

    private static async Task<List<Dimension>> LoadDimensions(SqliteConnection connection)
    {
        var command = connection.CreateCommand();
        command.CommandText = @"SELECT id, `order`, name, optional FROM dimensions;";

        using var reader = await command.ExecuteReaderAsync();
        List<Dimension> dimensions = new();
        while (await reader.ReadAsync())
        {
            var id = reader.GetGuid(0);
            var order = reader.GetInt32(1);
            var name = reader.GetString(2);
            var optional = reader.GetBoolean(3);
            var options = await LoadOptions(connection, id);
            dimensions.Add(new()
            {
                Id = id,
                Order = order,
                Name = name,
                Optional = optional,
                DimensionOptions = new(options),
            });
        }
        return dimensions;
    }

    public static async Task<Project> LoadAsync(string path)
    {
        var connectionString = new SqliteConnectionStringBuilder()
        {
            DataSource = path,
            Mode = SqliteOpenMode.ReadWriteCreate,
        }.ToString();
        using var connection = new SqliteConnection(connectionString);
        await connection.OpenAsync();
        var command = connection.CreateCommand();
        command.CommandText = "SELECT name, videoFile, interval FROM projects LIMIT 1";
        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            var name = reader.GetString(0);
            var videoFile = reader.GetString(1);
            var interval = reader.GetTimeSpan(2);
            var dimensions = await LoadDimensions(connection);
            Project project = new(name, videoFile, interval, new(dimensions));
            return project;
        }
        throw new ApplicationException("Project not found");
    }
}
