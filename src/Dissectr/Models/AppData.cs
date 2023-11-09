//using SQLite;

namespace Dissectr.Models;

public class AppData
{
    //privakte SQLiteAsyncConnection? connection;
    //private async Task<SQLiteAsyncConnection> EnsureInit()
    //{
    //    if (connection is null)
    //    {
    //        connection = new(Path.Combine(FileSystem.AppDataDirectory, "appdata.db"));
    //        await connection.CreateTableAsync<ProjectReference>();
    //    }
    //    return connection;
    //}

    public async Task<List<ProjectReference>> GetRecentProjectsAsync()
    {
        //SQLiteAsyncConnection connection = await EnsureInit();
        //recentProjects = await connection.Table<ProjectReference>().ToListAsync();
        return new List<ProjectReference>
        {
            new ProjectReference{ Id = Guid.NewGuid(), LastOpened = DateTimeOffset.Now, Name = "Test1", Path = "C:\\Users\\james\\Videos\\test1.mp4" },
            new ProjectReference{ Id = Guid.NewGuid(), LastOpened = DateTimeOffset.Now, Name = "Test2", Path = "C:\\Users\\james\\Videos\\test2.mp4" },
            new ProjectReference{ Id = Guid.NewGuid(), LastOpened = DateTimeOffset.Now, Name = "Test3", Path = "C:\\Users\\james\\Videos\\test3.mp4" },
            new ProjectReference{ Id = Guid.NewGuid(), LastOpened = DateTimeOffset.Now, Name = "Test4", Path = "C:\\Users\\james\\Videos\\test4.mp4" },
        };
    }

    public async Task SaveRecentProjectsAsync(IEnumerable<ProjectReference> recentProjects)
    {
        //SQLiteAsyncConnection connection = await EnsureInit();
        //var toInsert = recentProjects.OrderBy(p => p.LastOpened).Take(5);
        //await connection.Table<ProjectReference>().DeleteAsync();
        //await connection.InsertAllAsync(recentProjects);
    }
}
