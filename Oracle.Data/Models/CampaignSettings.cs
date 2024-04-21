#region using

using System.ComponentModel.DataAnnotations;

#endregion

namespace Oracle.Data.Models;

public class CampaignSettings
{
    [Key] public int Id { get; set; }
    public string CampaignName { get; set; }
}