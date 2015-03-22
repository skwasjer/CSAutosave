// © 2015 skwas

using System.Configuration;

namespace skwas.CitiesSkylines
{
	/// <summary>
	/// Simple settings class which uses the standard .NET App.config for storing settings (under AppSettings). Can (and should) be extended for strong typed settings.
	/// </summary>
	public class ModSettings
	{
		private readonly Configuration _config;

		/// <summary>
		/// Initializes a new instance of ModSettings using specified DLL-path.
		/// </summary>
		/// <param name="dllPath">The path to the DLL of your plugin (without .config extension).</param>
		public ModSettings(string dllPath)
		{
			_config = ConfigurationManager.OpenExeConfiguration(dllPath);
		}

		public string this[string key]
		{
			get { return Contains(key) ? _config.AppSettings.Settings[key].Value : null; }
		}

		public bool Contains(string key)
		{
			return _config.AppSettings.Settings[key] != null;
		}
	}
}
