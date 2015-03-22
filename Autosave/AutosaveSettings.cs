// © 2015 skwas

namespace skwas.CitiesSkylines
{
	/// <summary>
	/// AutosaveSettings, overrides ModSettings for strong typed (and with default values) settings.
	/// </summary>
	public class AutosaveSettings
		: ModSettings
	{
		public const int DefaultInterval = 15;

		public AutosaveSettings(string dllPath)
			: base(dllPath)
		{ }

		public int Interval
		{
			get
			{
				int interval;
				return int.TryParse(this["interval_minutes"], out interval) ? interval : DefaultInterval;
			}
		}
	}
}
