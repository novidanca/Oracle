#region using

using Microsoft.EntityFrameworkCore;
using Oracle.Data.Models;

#endregion

namespace Oracle.App.Components.Pages.Players;

public partial class PlayersPage : OracleBasePage
{
	private List<Player> Players { get; set; } = [];

	protected override async Task Refresh()
	{
		Players = await Db.Players.Include(x => x.Characters).ToListAsync();
	}
}