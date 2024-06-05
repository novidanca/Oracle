namespace Oracle.App.Components.Shared.Timeline.Models;

public class TimelineDayStyleSpec(int dayWidthPixels, int dayHeightPixels, int marginPixels)
{
	public int DayWidthPixels { get; set; } = dayWidthPixels;
	public int DayHeightPixels { get; set; } = dayHeightPixels;
	public int MarginPixels { get; set; } = marginPixels;

	public static int BorderPixels => 2;

	public int InnerSpanEndWidthPixels => DayWidthPixels - MarginPixels - BorderPixels;
	public int InnerSpanHeightPixels => DayHeightPixels - BorderPixels * 2;

	public int GetTimelineDayPixelWidth(int numDays = 1)
	{
		var numMarginPixels = numDays <= 1 ? 0 : MarginPixels * (numDays - 1);
		var numPixels = DayWidthPixels * numDays;
		numPixels += numMarginPixels;
		return numPixels;
	}

	public int GetSpanInnerPixelWidth(int numDays)
	{
		var spanPixels = GetTimelineDayPixelWidth(numDays);
		return spanPixels - BorderPixels * 2;
	}

	public TimelineDayStyleSpec() : this(36, 36, 1)
	{
	}
}