using Microsoft.Data.Sqlite;

namespace Dissectr.Models;

class Project
{
    public string Name { get; set; }
    public string ProjectFile { get; set; }
    public string VideoFile { get; set; }
    public TimeSpan Interval { get; set; }
    public List<Dimension> Dimensions { get; set; }

    public Project(string name, string projectFile, string videoFile, TimeSpan interval, List<Dimension> dimensions)
    {
        Name = name;
        ProjectFile = projectFile;
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

    private static async Task CreateEntriesTable(SqliteConnection connection)
    {
        var command = connection.CreateCommand();
        command.CommandText = @"
            CREATE TABLE IF NOT EXISTS IntervalEntries (
                start TEXT PRIMARY KEY NOT NULL,
                transcription TEXT
            );";
        await command.ExecuteNonQueryAsync();
    }

    private static async Task CreateEntryDimensionsTable(SqliteConnection connection)
    {
        var command = connection.CreateCommand();
        command.CommandText = @"
            CREATE TABLE IF NOT EXISTS EntryDimensions (
                start TEXT NOT NULL,
                dimensionId TEXT NOT NULL,
                optionId TEXT NOT NULL,
                FOREIGN KEY (start) REFERENCES IntervalEntries(start),
                FOREIGN KEY (dimensionId) REFERENCES dimensions(id),
                FOREIGN KEY (optionId) REFERENCES dimension_options(id),
                PRIMARY KEY (start, dimensionId)
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

    public static async Task<string> CreateAsync(Project project)
    {
        using var connection = CreateConnection(project.ProjectFile);
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
        return project.ProjectFile;
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
            Project project = new(name, path, videoFile, interval, new(dimensions));
            return project;
        }
        throw new ApplicationException("Project not found");
    }

    public async Task InitEntryTables(TimeSpan totalDuration)
    {
        var connectionString = new SqliteConnectionStringBuilder()
        {
            DataSource = ProjectFile,
            Mode = SqliteOpenMode.ReadWriteCreate,
        }.ToString();
        using var connection = new SqliteConnection(connectionString);
        await connection.OpenAsync();
        await CreateEntriesTable(connection);
        await CreateEntryDimensionsTable(connection);

        var totalDurationInTicks = totalDuration.Ticks;
        var intervalInTicks = Interval.Ticks;
        List<TimeSpan> intervalStarts = Enumerable.Range(0, (int)(totalDurationInTicks / intervalInTicks))
            .Select(x => TimeSpan.FromTicks(x * intervalInTicks))
            .ToList();

        using var transaction = await connection.BeginTransactionAsync();
        var command = connection.CreateCommand();
        command.CommandText = @"
            INSERT INTO IntervalEntries (start)
            VALUES ($start)
            ON CONFLICT (start) DO NOTHING;";
        var startParam = AddParameter(command, "$start");

        foreach (var start in intervalStarts)
        {
            startParam.Value = start;
            await command.ExecuteNonQueryAsync();
        }
        await transaction.CommitAsync();
    }

    private async Task<List<IntervalEntry.DimensionSelection>> GetSelections(SqliteConnection connection, TimeSpan intervalStart)
    {
        var command = connection.CreateCommand();
        command.CommandText = @"SELECT dimensionId, optionId FROM EntryDimensions WHERE start = $start;";
        var reader = await command.ExecuteReaderAsync();
        List<IntervalEntry.DimensionSelection> selections = Dimensions
            .Select(d => new IntervalEntry.DimensionSelection(d, null))
            .ToList();
        while (await reader.ReadAsync())
        {
            var dimensionId = reader.GetGuid(0);
            var optionId = reader.GetGuid(0);
            /* TODO: We should do something more robust here (First explodes) */
            var selection = selections.Where(s => s.Dimension.Id == dimensionId).First();

            var option = selection.Dimension.DimensionOptions.Where(o => o.Id == optionId).First();
            selection.Selection = option;
        }
        return selections;
    }

    public async Task<IntervalEntry> GetEntry(TimeSpan intervalStart)
    {
        var connectionString = new SqliteConnectionStringBuilder()
        {
            DataSource = ProjectFile,
            Mode = SqliteOpenMode.ReadWriteCreate,
        }.ToString();
        using var connection = new SqliteConnection(connectionString);
        await connection.OpenAsync();

        var command = connection.CreateCommand();
        command.CommandText = @"SELECT transcription FROM IntervalEntries WHERE start = $start;";
        AddParameter(command, "$start", intervalStart);
        var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            var transcription = reader.GetString(0);
            var selections = await GetSelections(connection, intervalStart);
            return new()
            {
                Start = intervalStart,
                Transcription = transcription,
                Dimensions = selections,
            };
        }
        else
        {
            throw new ApplicationException($"Unexpected error: Did not find entry for {intervalStart.ToString(@"hh\:mm\:ss")}.");
        }
    }
}
