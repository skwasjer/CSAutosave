// © 2015 skwas

using System;
using System.Timers;
using ColossalFramework;
using ICities;

namespace skwas.CitiesSkylines
{
	/// <summary>
	/// Autosave mod by skwas. Note I wrote this completely on my own. I know there are other Autosave mods out there that do the same or perhaps even better. However, this mod was a test bed for ModBase and ModSettings base implementations, and I didn't have any other idea so I just said lets write my own for testing purposes. I personally believe the base implementation for setting up a mod/plugin came out pretty well. Compared to others, I chose to use .NET configuration files for storing settings, instead of a custom implementation. Also, I used some neat tricks to resolve the actual running library (which is hidden by Mono/dynamic compile). I in the end started looking at implementing Steam cloud support during save but I will do that later once I verify this mod works fine via the Mods workshop, because I never uploaded a mod before there so I need something out there to test and receive feedback on. 
	/// </summary>
	/// <remarks>If you wish to use my base classes or code, please drop me a line first.</remarks>
	public class Autosave
		: ModBase, ISerializableDataExtension
	{
		private IManagers _managers;
		private AutosaveSettings _settings;

		#region Implementation of ISerializableDataExtension

		private Timer _timer;

		void ISerializableDataExtension.OnCreated(ISerializableData serializedData)
		{
			_managers = serializedData.managers;

			_settings = new AutosaveSettings(ModDllPath);

			RestartTimer();

			Log.Info("{0} running with interval {1}min...", Name, _settings.Interval);
		}

		void ISerializableDataExtension.OnReleased()
		{
			DestroyTimer();
		}

		void ISerializableDataExtension.OnLoadData()
		{
			RestartTimer();
		}

		void ISerializableDataExtension.OnSaveData()
		{
			RestartTimer();
		}

		void RestartTimer()
		{
			DestroyTimer();
			CreateTimer();

			_timer.Start();
		}

		void CreateTimer()
		{
			if (_timer != null) return;

			var intervalMillisecs = _settings.Interval * 60000; // Turn (minute) interval into milliseconds.

			_timer = new Timer(intervalMillisecs) { AutoReset = false };
			_timer.Elapsed += OnAutosave;
		}

		void DestroyTimer()
		{
			if (_timer == null) return;

			_timer.Stop();
			_timer.Elapsed -= OnAutosave;
			_timer.Dispose();
			_timer = null;
		}

		private void OnAutosave(object sender, ElapsedEventArgs e)
		{
			// Wrap in catch just in case. We don't want to trash the game.
			try
			{
				var simManager = Singleton<SimulationManager>.instance;

				// Ignore autosave if game is paused.
				if (simManager.ForcedSimulationPaused) return;

				var cityName = simManager.m_metaData.m_CityName ?? "Unnamed city";
				var saveName = "[autosave] " + cityName;
				Log.Info("Autosaving '{0}'...", saveName);

				// TODO: support cloud saves?
				_managers.serializableData.SaveGame(saveName);
			}
			catch (Exception ex)
			{
				Log.Error(ex.ToString());
			}
			finally
			{
				// Restart the timer.
				_timer.Start();
			}
		}

		#endregion
	}
}
