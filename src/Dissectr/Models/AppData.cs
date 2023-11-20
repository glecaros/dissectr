//using SQLite;

using System.Collections.Generic;
using System.Threading.Tasks;

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

    public Task<List<ProjectReference>> GetRecentProjectsAsync()
    {

        return Task.FromResult(new List<ProjectReference>());
        //SQLiteAsyncConnection connection = await EnsureInit();
        //recentProjects = await connection.Table<ProjectReference>().ToListAsync();
    }

    public Task SaveRecentProjectsAsync(IEnumerable<ProjectReference> recentProjects)
    {
        return Task.CompletedTask;
        //SQLiteAsyncConnection connection = await EnsureInit();
        //var toInsert = recentProjects.OrderBy(p => p.LastOpened).Take(5);
        //await connection.Table<ProjectReference>().DeleteAsync();
        //await connection.InsertAllAsync(recentProjects);
    }
}
