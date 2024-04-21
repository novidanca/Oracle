#region using

using System.Text;
using CommunityToolkit.Maui.Storage;
using Microsoft.EntityFrameworkCore;
using Oracle.Data;

#endregion

namespace Oracle.Core.DatabaseManagement;

public class MauiDbPathService : IDbPathService
{
	public string? GetPath()
	{
		var path = Preferences.Get("DbPath", null);

		return File.Exists(path) ? path : null;
	}

	public async Task<string?> MakePath(string campaignName, bool useDefaultPath = true)
	{
		var fileName = $"{campaignName}.campaign";
		var getPath = !useDefaultPath && DeviceInfo.Current.Platform == DevicePlatform.WinUI;

		var dbPath = "";

		// Keep using in a block so we unlock the file after creating it
		using (var stream = new MemoryStream(Encoding.Default.GetBytes("")))
		{
			if (getPath)
			{
				//We're only calling this on windows, so this should be ok.
				var fileResult = await FileSaver.SaveAsync(FileSystem.AppDataDirectory, fileName, stream);
				dbPath = fileResult.FilePath;

				// If the user doesn't select a file, stop
				if (dbPath == null)
					return null;
			}
			else
			{
				dbPath = Path.Combine(FileSystem.AppDataDirectory, fileName);
			}

			await using var fileStream = new FileStream(dbPath, FileMode.Create, FileAccess.ReadWrite);
			await stream.CopyToAsync(fileStream);
		}

		Preferences.Set("DbInitialized", false);
		return dbPath;
	}

	public async Task<bool> SetPath(string? path)
	{
		if (path == null || Path.GetExtension(path) != ".campaign")
			return false;

		await InitializeDb(path);
		return true;
	}

	public async Task<string?> GetPathFromUser()
	{
		var customFileType = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
		{
			{
				DevicePlatform.iOS, new[] { "public.data" }
			}, // For iOS, 'public.data' is a generic type for any data file. Custom extensions are harder to filter directly.
			{
				DevicePlatform.Android, new[] { "*/*" }
			}, // Android doesn't allow direct filtering by custom file extensions in the file picker. Use "*/*" and then filter manually if necessary.
			{ DevicePlatform.WinUI, new[] { ".campaign" } }, // Directly specify the custom extension for Windows.
			{
				DevicePlatform.Tizen, new[] { "*/*" }
			}, // Similar to Android, Tizen does not allow filtering by custom extension.
			{
				DevicePlatform.macOS, new[] { "public.data" }
			} // macOS, like iOS, uses UTIs. 'public.data' covers any file type, as macOS file picker doesn't filter by custom extension.
		});

		var options = new PickOptions()
		{
			PickerTitle = "Please select a .campaign file",
			FileTypes = customFileType
		};

		var fileResult = await FilePicker.PickAsync(options);
		return fileResult?.FullPath;
	}

	public async Task InitializeDb(string path)
	{
		var context = new OracleDbContext(new DbContextOptions<OracleDbContext>(), path);
		await context.Database.MigrateAsync();
		Preferences.Set("DbInitialized", true);
		Preferences.Set("DbPath", path);
	}

	public bool IsDbInitialized()
	{
		return Preferences.Get("DbInitialized", false);
	}
}