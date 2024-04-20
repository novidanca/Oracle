namespace Oracle.Core.DatabaseManagement;

public interface IDbPathService
{
	Task InitializeDb(string path);
	bool IsDbInitialized();

	string? GetPath();
	Task<string?> MakePath(string campaignName, bool useDefaultPath = true);
	Task<bool> SetPath(string? path);
	Task<string?> GetPathFromUser();
}