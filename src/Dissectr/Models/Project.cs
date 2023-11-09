using SQLite;

namespace Dissectr.Models;

class ScopeGuard<T>: IDisposable
{
    private Action<T> action;
    private T value;
}

class Project
{
    private struct ProjectRecord
    {
        [PrimaryKey]
        public int Id { get; set; }

        public string Name { get; set; }

        public string VideoFile { get; set; }

        public TimeSpan Interval { get; set; }
    }

    private struct DimensionRecord
    {
        [PrimaryKey]
        public Guid Id { get; set; }

        public int Order { get; set; }

        public string Name { get; set; }

        public bool Optional { get; set; }
    }

    private struct DimensionOptionRecord
    {
        [PrimaryKey]
        public Guid Id { get; set; }

        [Indexed]
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

    public static async Task<string> CreateAsync(string path, Project project)
    {
        var fullProjectPath = Path.ChangeExtension(Path.Combine(path, project.Name), ".dissectr");
        SQLiteAsyncConnection connection = new(fullProjectPath, SQLite.SQLiteOpenFlags.Create | SQLite.SQLiteOpenFlags.ReadWrite | SQLite.SQLiteOpenFlags.FullMutex);
        await connection.CreateTableAsync<ProjectRecord>();
        await connection.CreateTableAsync<DimensionRecord>();
        await connection.CreateTableAsync<DimensionOptionRecord>();
        await connection.InsertAsync(ToRecord(project));
        int order = 0;
        foreach (var dimension in project.Dimensions)
        {
            dimension.Order = ++order;
            var dimensionRecord = ToRecord(dimension);
            await connection.InsertAsync(dimensionRecord);
            var optionRecords = dimension.DimensionOptions.Select(o => ToRecord(dimensionRecord.Id, o));
            await connection.InsertAllAsync(optionRecords);
        }

        return fullProjectPath;
    }

    public static async Task<Project> LoadAsync(string path)
    {
        SQLiteAsyncConnection connection = new(path, SQLite.SQLiteOpenFlags.Create | SQLite.SQLiteOpenFlags.ReadWrite | SQLite.SQLiteOpenFlags.FullMutex);
        var projectRecords= await connection.Table<ProjectRecord>().Take(1).ToListAsync();
        var projectRecord = projectRecords.First();

        Project project = new(projectRecord.Name, projectRecord.VideoFile, projectRecord.Interval, new());

        var dimensionRecords = await connection.Table<DimensionRecord>()
                                               .ToListAsync();
        foreach (var dimensionRecord in dimensionRecords)
        {
            var optionRecords = await connection.Table<DimensionOptionRecord>()
                                                .Where(o => o.DimensionId == dimensionRecord.Id)
                                                .ToListAsync();
            Dimension dimension = new()
            {
                Id = dimensionRecord.Id,
                Name = dimensionRecord.Name,
                Order = dimensionRecord.Order,
                Optional = dimensionRecord.Optional,
                DimensionOptions = new(optionRecords.Select(r => new DimensionOption(r.Id, r.Code, r.Name))),
            };
        }
        return project;
    }
}
