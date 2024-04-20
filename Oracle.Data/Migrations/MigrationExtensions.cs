namespace Oracle.Data.Migrations;

public static class MigrationExtensions
{
	// Replace single quotes in the input to avoid SQL injection issues or errors
	public static string EscapeSqlValue(string value)
	{
		return $"'{value.Replace("'", "''")}'";
	}

	public static string NullableIntToSql(int? value)
	{
		// Convert nullable integer to SQL compatible format
		return value.HasValue ? value.ToString() : "NULL";
	}
}