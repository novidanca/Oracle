#region using

using Oracle.Core.ServiceManagement;
using Oracle.Data;

#endregion

namespace Oracle.Logic.Services;

public class ServiceBase(OracleDbContext db) : ITransientService
{
	protected readonly OracleDbContext Db = db;
}