using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Server;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

// *    V1.0.1
// *    - Improve code quality

namespace LogWriterMeow
{
    internal class Plugin:Plugin<Config>
    {
        public override string Name => "LogWriterMeow";
        public override string Author =>  "MeowServer";
        public override PluginPriority Priority => PluginPriority.Highest;

        public override Version Version => new Version(1, 0, 1);

        public static Plugin Instance;

        public override void OnEnabled()
        {
            Instance = this;

            Exiled.Events.Handlers.Server.RoundStarted += Logger.WriteStartTime;
            Exiled.Events.Handlers.Server.RoundEnded += Logger.WriteEndTime;
            Exiled.Events.Handlers.Server.WaitingForPlayers += Logger.WriteWaitingForPlayerTime;
            Exiled.Events.Handlers.Server.RestartingRound += Logger.WriteRestartTime;

            Exiled.Events.Handlers.Player.Verified += Logger.WritePlayerJoinEvent;
            Exiled.Events.Handlers.Player.Left += Logger.WritePlayerLeftEvent;

            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            Instance = null;

            Logger.WriteLoggerDisabledEvent();

            Exiled.Events.Handlers.Server.RoundStarted -= Logger.WriteStartTime;
            Exiled.Events.Handlers.Server.RoundEnded -= Logger.WriteEndTime;
            Exiled.Events.Handlers.Server.WaitingForPlayers -= Logger.WriteWaitingForPlayerTime;
            Exiled.Events.Handlers.Server.RestartingRound -= Logger.WriteRestartTime;

            Exiled.Events.Handlers.Player.Verified -= Logger.WritePlayerJoinEvent;
            Exiled.Events.Handlers.Player.Left -= Logger.WritePlayerLeftEvent;

            base.OnDisabled();
        }
    }

    public enum LogType
    {
        Info,
        Error,
        Warning,

        Debug
    }

    public static class Logger
    {
        private static string LogFilePath => Path.Combine(Plugin.Instance.Config.LogFilePath, $"{Server.Port}.txt");

        private static void WriteLog(string log)
        {
            try
            {
                if (!File.Exists(LogFilePath))
                    File.WriteAllText(LogFilePath, "Log file created at " + DateTime.Now.ToString("f"));
                
                using(StreamWriter logWriter = new StreamWriter(LogFilePath, true)){
                    logWriter.WriteLine(log);
                    logWriter.Close();
                }

            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        internal static void WriteStartTime()
        {
            string log = "Round Started at " + System.DateTime.Now.ToString();
            WriteLog(log);
        }

        internal static void WriteEndTime(RoundEndedEventArgs ev)
        {
            string log = "Round Ended at " + System.DateTime.Now.ToString();
            WriteLog(log);
        }

        internal static void WriteRestartTime()
        {
            string log = "Round Restart at " + System.DateTime.Now.ToString();
            WriteLog(log);
        }

        internal static void WriteWaitingForPlayerTime()
        {
            string log = "Round Start Waiting For Player at " + System.DateTime.Now.ToString();
            WriteLog(log);
        }

        internal static void WritePlayerJoinEvent(VerifiedEventArgs ev)
        {
            string log = System.DateTime.Now.ToString(@"hh\:mm\:ss") + $" Player {ev.Player.Nickname} has joined the server from {ev.Player.IPAddress}, ID: {ev.Player.UserId}";
            WriteLog(log);
        }
        
        internal static void WritePlayerLeftEvent(LeftEventArgs ev)
        {
            string log = System.DateTime.Now.ToString(@"hh\:mm\:ss") + $" Player {ev.Player.Nickname} has left the server from {ev.Player.IPAddress}, ID: {ev.Player.UserId}";
            WriteLog(log);
        }

        internal static void WriteLoggerDisabledEvent()
        {
            string log = System.DateTime.Now.ToString(@"hh\:mm\:ss") + $" Logger was disabled";
            WriteLog(log);
        }

        public static void Info(string message)
        {
            if(Plugin.Instance.Config.PrintLogOntoConsole) 
                Log.Info(message);

            WriteLog(FormatHandler.FormatLog(LogType.Info, message));
        }

        public static void Info(string message, string source)
        {
            if (Plugin.Instance.Config.PrintLogOntoConsole) 
                Log.Info(message);

            WriteLog(FormatHandler.FormatLog(LogType.Info, message, source));
        }

        public static void Error(string message)
        {
            if (Plugin.Instance.Config.PrintLogOntoConsole) 
                Log.Error(message);

            WriteLog(FormatHandler.FormatLog(LogType.Error, message));
        }

        public static void Error(string message, string source)
        {
            if (Plugin.Instance.Config.PrintLogOntoConsole) 
                Log.Info(message);

            WriteLog(FormatHandler.FormatLog(LogType.Error, message, source));
        }

        public static void Warning(string message)
        {
            if (Plugin.Instance.Config.PrintLogOntoConsole) 
                Log.Warn(message);

            WriteLog(FormatHandler.FormatLog(LogType.Warning, message));
        }

        public static void Warning(string message, string source)
        {
            if (Plugin.Instance.Config.PrintLogOntoConsole) 
                Log.Info(message);

            WriteLog(FormatHandler.FormatLog(LogType.Warning, message, source));
        }

        public static void Debug(string message)
        {
            if (Plugin.Instance.Config.PrintLogOntoConsole)
                Log.Debug(message);

            WriteLog(FormatHandler.FormatLog(LogType.Debug, message));
        }

        public static void Debug(string message, string source)
        {
            if (Plugin.Instance.Config.PrintLogOntoConsole)
                Log.Debug(message);

            WriteLog(FormatHandler.FormatLog(LogType.Debug, message, source));
        }
    }

    internal static class FormatHandler
    {
        public static string FormatLog(LogType type, string rawMessage, string source = null)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(DateTime.Now.ToString(@"hh\:mm\:ss"));
            
            if(!string.IsNullOrEmpty(source))
                stringBuilder.Append("[").Append(source).Append("]");

            stringBuilder
                .Append("[")
                .Append(type.ToString())
                .Append("]")
                .Append(rawMessage);

            return stringBuilder.ToString();
        }
    }
}
