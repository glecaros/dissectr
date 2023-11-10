using Microsoft.Data.Sqlite;

namespace Dissectr.Models;

internal class ScopeGuard : IAsyncDisposable
{
    private Func<Task> _action;

    internal ScopeGuard(Func<Task> action)
    {
        _action = action;
    }

    public async ValueTask DisposeAsync()
    {
        await _action().ConfigureAwait(false);
    }
}

class Project
{
    private struct ProjectRecord
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string VideoFile { get; set; }

        public TimeSpan Interval { get; set; }
    }

    private struct DimensionRecord
    {
        public Guid Id { get; set; }

        public int Order { get; set; }

        public string Name { get; set; }

        public bool Optional { get; set; }
    }

    private struct DimensionOptionRecord
    {
        public Guid Id { get; set; }

        public Guid DimensionId { get; set; }

        public int Code { get; set; }

        public string Name { get; set; }
    }

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

    private static ProjectRecord ToRecord(Project project)
    {
        return new ProjectRecord
        {
            Id = 1,
            Name = project.Name,
            VideoFile = project.VideoFile,
            Interval = project.Interval,
        };
    }

    private static DimensionRecord ToRecord(Dimension dimension)
    {
        return new DimensionRecord
        {
            Id = dimension.Id,
            Order = dimension.Order,
            Name = dimension.Name,
            Optional = dimension.Optional,
        };
    }

    private static DimensionOptionRecord ToRecord(Guid dimensionId, DimensionOption dimensionOption)
    {
        return new DimensionOptionRecord
        {
            Id = dimensionOption.Id,
            DimensionId = dimensionId,
            Name = dimensionOption.Name,
            Code = dimensionOption.Code,
        };
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
                order INTEGER NOT NULL,
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
                dimension_id TEXT,
                code INTEGER,
                name TEXT,
                FOREIGN KEY(dimension_id) REFERENCES dimensions(id)
            );";
        await command.ExecuteNonQueryAsync();
    }

    private static void AddParameter(SqliteCommand command, string parameterName, object value)
    {
        var parameter = command.CreateParameter();
        parameter.ParameterName = parameterName;
        parameter.Value = value;
        command.Parameters.Add(parameter);
    }

    public static async Task<string> CreateAsync(string path, Project project)
    {
        var fullProjectPath = Path.ChangeExtension(Path.Combine(path, project.Name), ".dissectr");
        using var connection = CreateConnection(fullProjectPath);

        using var transaction = connection.BeginTransaction();

        await CreateProjectTable(connection);
        await CreateDimensionTable(connection);
        await CreateDimensionOptionsTable(connection);

        var command = connection.CreateCommand();
        command.CommandText = @"
            INSERT INTO project (id, name, videoFile, interval)
            VALUES ($id, $name, $videoFile, $interval)
        ";
        AddParameter(command, "$id", 1);
        AddParameter(command, "$name", project.Name);
        AddParameter(command, "$videoFile", project.VideoFile);
        AddParameter(command, "$interval", project.Interval);

        await transaction.CommitAsync();


        return "lala";
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
        command.CommandText = "SELECT Name, VideoFile, Interval FROM ProjectRecord LIMIT 1";
        using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            var name = reader.GetString(0);
            var videoFile = reader.GetString(1);
            var interval = reader.GetTimeSpan(2);
            Project project = new(name, videoFile, interval, new());
            return project;
        }
        throw new ApplicationException("Project not found");
        //SQLiteAsyncConnection connection = new(path, Flags);//, SQLite.SQLiteOpenFlags.ReadWrite | SQLite.SQLiteOpenFlags.FullMutex);
        //await using var _ = new ScopeGuard(connection.CloseAsync);
        //var projectRecord = await connection.GetAsync<ProjectRecord>(1);
        //var projectRecords = connection.Query<ProjectRecord>("SELECT * FROM ProjectRecord LIMIT 1");
        //var projectRecord = projectRecords.First();


        //var dimensionRecords = await connection.Table<DimensionRecord>()
        //                                       .ToListAsync();
        //foreach (var dimensionRecord in dimensionRecords)
        //{
        //    var optionRecords = await connection.Table<DimensionOptionRecord>()
        //                                        .Where(o => o.DimensionId == dimensionRecord.Id)
        //                                        .ToListAsync();
        //    Dimension dimension = new()
        //    {
        //        Id = dimensionRecord.Id,
        //        Name = dimensionRecord.Name,
        //        Order = dimensionRecord.Order,
        //        Optional = dimensionRecord.Optional,
        //        DimensionOptions = new(optionRecords.Select(r => new DimensionOption(r.Id, r.Code, r.Name))),
        //    };
        //}
    }
}
