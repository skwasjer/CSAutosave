// © 2015 skwas

using System.Diagnostics;
using ColossalFramework.Plugins;
using ColossalFramework.UI;

namespace skwas.CitiesSkylines
{
	/// <summary>
	/// Logs info to the ingame console. Note I use ConditionalAttribute to disable the logging functionality in Release. The DebugOutputPanel can overflow and cause the game to crash at the moment. In future I will implement file based logging for Release mode.
	/// </summary>
	class Log
	{
		[Conditional("DEBUG")]
		public static void Info(string text, params object[] args)
		{
			Write(PluginManager.MessageType.Message, text, args);
		}

		[Conditional("DEBUG")]
		public static void Warning(string text, params object[] args)
		{
			Write(PluginManager.MessageType.Warning, text, args);
		}

		[Conditional("DEBUG")]
		public static void Error(string text, params object[] args)
		{
			Write(PluginManager.MessageType.Error, text, args);
		}

		private static void Write(PluginManager.MessageType type, string text, params object[] args)
		{
			// Prepend caller.
			var frame = new StackFrame(2);
			var method = frame.GetMethod();

			// Note that the game crashes after writing too much log items. We fix it by clearing the log first for every log entry. Limitation is only 1 log item visible.
			UIView.library.Get<DebugOutputPanel>("DebugOutputPanel").OnClear();
			DebugOutputPanel.AddMessage(type, string.Format("{0}->{1}: {2}", method.DeclaringType.Name, method.Name, string.Format(text, args)));
		}
	}
}
