#region using

using System.Reflection;

#endregion

namespace Oracle.Core.ServiceManagement;

public static class ServiceCollectionExtensions
{
	public static void AddAutoRegisteredServices(this IServiceCollection services)
	{
		var assemblies = GetAssemblies();

		foreach (var assembly in assemblies)
		foreach (var type in assembly.GetTypes())
			if (typeof(ITransientService).IsAssignableFrom(type) && type is { IsInterface: false, IsAbstract: false })
				services.AddTransient(type);
	}

	private static List<Assembly> GetAssemblies()
	{
		var names = new List<string>()
		{
			"Oracle.App",
			"Oracle.Logic",
			"Oracle.Data",
			"Oracle.Core"
		};

		return names
			.Select(Assembly.Load)
			.ToList();
	}
}