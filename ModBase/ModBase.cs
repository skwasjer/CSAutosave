// © 2015 skwas

using System;
using System.IO;
using System.Linq;
using System.Reflection;
using ColossalFramework.Plugins;
using ColossalFramework.Steamworks;
using ICities;

namespace skwas.CitiesSkylines
{
	/// <summary>
	/// Base class for mod implementations. Takes mod info from the type and assembly (AssemblyInfo), and resolves into the actual plugin info.
	/// </summary>
	/// <remarks>
	/// Use of this base class is only working for compiled DLL projects. It is also absolutely important you version your DLL using [assembly: AssemblyVersion("1.0.*")], otherwise the plugin resolving code won't work properly and might return the wrong mod location.
	/// </remarks>
	public abstract class ModBase
		: IUserMod
	{
		private readonly Assembly _assembly;
		private PluginManager.PluginInfo _pluginInfo; 

		protected ModBase()
		{
			_assembly = GetType().Assembly;
		}

		#region Implementation of IUserMod

		/// <summary>
		/// Gets the mod name.
		/// </summary>
		/// <remarks>Uses the class name if AssemblyTitleAttribute is missing.</remarks>
		public virtual string Name
		{
			get
			{
				var attr = GetAssemblyAttribute<AssemblyTitleAttribute>();
				return string.Format("{0} v{1}", 
					attr == null ? GetType().Name : attr.Title,
					Version
				);
			}
		}

		/// <summary>
		/// Gets the in-game mod description (includes author name if available).
		/// </summary>
		/// <remarks>Explicitly implemented, the author name is only added to the game description.</remarks>
		string IUserMod.Description
		{
			get
			{
				return string.IsNullOrEmpty(Author)
					? Description
					: string.Format("{0}\nby {1}", Description, Author);
			}
		}

		#endregion

		/// <summary>
		/// Gets the mod description.
		/// </summary>
		public virtual string Description
		{
			get
			{
				var attr = GetAssemblyAttribute<AssemblyDescriptionAttribute>();
				return attr == null ? null : attr.Description;
			}
		}

		/// <summary>
		/// Gets the mod author.
		/// </summary>
		public virtual string Author
		{
			get
			{
				var attr = GetAssemblyAttribute<AssemblyCompanyAttribute>();
				return attr == null ? null : attr.Company;
			}
		}

		/// <summary>
		/// Gets the mod version.
		/// </summary>
		public virtual Version Version
		{
			get { return _assembly.GetName().Version; }
		}

		/// <summary>
		/// Gets the mod path.
		/// </summary>
		public virtual string ModPath { get { return PluginInfo == null ? null : PluginInfo.modPath; } }

		private string _modDllPath;
		/// <summary>
		/// Gets the mod dll path.
		/// </summary>
		public virtual string ModDllPath {
			get
			{
				var modPath = ModPath;	// Prefetch, forces load of plugin info.
				return _modDllPath ?? modPath;
			}
		}

		/// <summary>
		/// Gets the steam workshop id of the mod/plugin. In local installations, returns PublishedFileId.invalid.
		/// </summary>
		public virtual PublishedFileId SteamWorkshopId { get { return PluginInfo == null ? PublishedFileId.invalid : PluginInfo.publishedFileID; } }

		/// <summary>
		/// Gets the plugin info for this mod (and caches it in local variable).
		/// </summary>
		protected virtual PluginManager.PluginInfo PluginInfo
		{
			get { return _pluginInfo ?? (_pluginInfo = GetPluginInfo()); }
		}

		/// <summary>
		/// Helper to read assembly info.
		/// </summary>
		/// <typeparam name="T">The custom attribute to read from the assembly.</typeparam>
		/// <returns></returns>
		protected T GetAssemblyAttribute<T>()
		{
			var attributes = _assembly.GetCustomAttributes(typeof (T), false);
			if (attributes.Length == 0) return default(T);
			return (T)attributes[0];
		}

		/// <summary>
		/// Enumerates plugins to find our plugin definition. This can be used to also determine save location when using Steam workshop (in which case path is different).
		/// </summary>
		/// <returns></returns>
		private PluginManager.PluginInfo GetPluginInfo()
		{
			try
			{
				var currentAssemblyName = _assembly.GetName().FullName;

				foreach (var p in PluginManager.instance.GetPluginsInfo())
				{
					if (!p.isBuiltin)
					{
						// Enumerate all assemblies in modPath, until we find the one matching the current assembly.
						foreach (var assemblyName in Directory.GetFiles(p.modPath, "*.dll", SearchOption.TopDirectoryOnly)
							.Select(AssemblyName.GetAssemblyName)
							.Where(assemblyName => assemblyName.FullName.Equals(currentAssemblyName, StringComparison.OrdinalIgnoreCase)))
						{
							// This is our assembly!
							_modDllPath = new Uri(assemblyName.CodeBase).LocalPath;
							return p;
						}
					}
				}
				throw new FileNotFoundException(string.Format("The plugin assembly '{0}' could not be resolved.", currentAssemblyName));
			}
			catch (Exception ex)
			{
				Log.Error(ex.ToString());
				throw; // Note: the plugin will fail if it can't find the assembly.
			}
		}
	}
}
