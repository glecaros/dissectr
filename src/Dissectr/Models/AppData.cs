using SQLite;

namespace Dissectr.Models;

public class AppData
{
    private SQLiteAsyncConnection? connection;
    private async Task<SQLiteAsyncConnection> EnsureInit()
    {
        if (connection is null)
        {
            connection = new(Path.Combine(FileSystem.AppDataDirectory, "appdata.db"));
            await connection.CreateTableAsync<ProjectReference>();
        }
        return connection;
    }

    private List<ProjectReference>? recentProjects;
    public List<ProjectReference> RecentProjects
    {
        get
        {
            if (recentProjects is null)
            {
                throw new InvalidOperationException();
            }
            return recentProjects;
        }
    }

    public async Task LoadAsync()
    {
        SQLiteAsyncConnection connection = await EnsureInit();
        recentProjects = await connection.Table<ProjectReference>().ToListAsync();
    }

    public async Task SaveAsync()
    {
        SQLiteAsyncConnection connection = await EnsureInit();
        if (recentProjects is not null)
        {
            recentProjects = recentProjects.OrderBy(p => p.LastOpened).Take(5).ToList();
            await connection.Table<ProjectReference>().DeleteAsync();
            await connection.InsertAllAsync(recentProjects);
        }
    }
}
