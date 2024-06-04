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

namespace LogWritterMeow
{
    public class LoggerMeow:Plugin<Config>
    {
        public override string Name => base.Name;
        public override string Author => base.Author;
        public override PluginPriority Priority => PluginPriority.Higher;

        public static LoggerMeow instance;

        public override void OnEnabled()
        {
            instance = this;

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
            instance = null;

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

    public static class Logger
    {
        private static readonly string logFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "EXILED\\Logs", "ServerLog-" + Server.Port.ToString() + ".txt");

        private static void writeLog(string log)
        {
            try
            {
                if (!File.Exists(logFilePath))
                {
                    using (StreamWriter createFile = File.CreateText(logFilePath))
                    {
                        createFile.WriteLine("ServerLog file created at: " + DateTime.Now);
                        createFile.Close();
                    }
                }
                
                using(StreamWriter logWriter = new StreamWriter(logFilePath, true)){
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
            string log = "Round Started at "+System.DateTime.Now.ToString();
            writeLog(log);
        }

        internal static void WriteEndTime(RoundEndedEventArgs ev)
        {
            string log = "Round Ended at " + System.DateTime.Now.ToString();
            writeLog(log);
        }

        internal static void WriteRestartTime()
        {
            string log = "Round Restart at " + System.DateTime.Now.ToString();
            writeLog(log);
        }

        internal static void WriteWaitingForPlayerTime()
        {
            string log = "Round Start Waiting For Player at " + System.DateTime.Now.ToString();
            writeLog(log);
        }

        internal static void WritePlayerJoinEvent(VerifiedEventArgs ev)
        {
            string log = System.DateTime.Now.ToString(@"hh\:mm\:ss") + $" Player {ev.Player.Nickname} has joined the server from {ev.Player.IPAddress}, ID: {ev.Player.UserId}";
            writeLog(log);
        }
        
        internal static void WritePlayerLeftEvent(LeftEventArgs ev)
        {
            string log = System.DateTime.Now.ToString(@"hh\:mm\:ss") + $" Player {ev.Player.Nickname} has left the server from {ev.Player.IPAddress}, ID: {ev.Player.UserId}";
            writeLog(log);
        }

        internal static void WriteLoggerDisabledEvent()
        {
            string log = System.DateTime.Now.ToString(@"hh\:mm\:ss") + $" Logger was disabled";
            writeLog(log);
        }

        public static void Info(string message)
        {
            if(LoggerMeow.instance.Config.PrintLogOntoConsole) Log.Info(message);

            writeLog(FormatHanlder.Info(message));
        }

        public static void Info(string message, string source)
        {
            if (LoggerMeow.instance.Config.PrintLogOntoConsole) Log.Info(message);

            writeLog(FormatHanlder.Info(message, source));
        }

        public static void Error(string message)
        {
            if (LoggerMeow.instance.Config.PrintLogOntoConsole) Log.Error(message);

            writeLog(FormatHanlder.Error(message));
        }

        public static void Error(string message, string source)
        {
            if (LoggerMeow.instance.Config.PrintLogOntoConsole) Log.Info(message);

            writeLog(FormatHanlder.Error(message, source));
        }

        public static void Warning(string message)
        {
            if (LoggerMeow.instance.Config.PrintLogOntoConsole) Log.Warn(message);

            writeLog(FormatHanlder.Warning(message));
        }

        public static void Warning(string message, string source)
        {
            if (LoggerMeow.instance.Config.PrintLogOntoConsole) Log.Info(message);

            writeLog(FormatHanlder.Warning(message, source));
        }

        public static void Debug(string message)
        {
            if (LoggerMeow.instance.Config.PrintLogOntoConsole) Log.Debug(message);

            writeLog(FormatHanlder.Debug(message));
        }

        public static void Debug(string message, string source)
        {
            if (LoggerMeow.instance.Config.PrintLogOntoConsole) Log.Debug(message);

            writeLog(FormatHanlder.Debug(message, source));
        }
    }

    public static class FormatHanlder
    {
        public static string Info(string rawMessage)
        {
            string message = string.Empty;
            message += System.DateTime.Now.ToString(@"hh\:mm\:ss");
            message += "[INFO]";
            message += rawMessage;

            return message;
        }

        public static string Info(string rawMessage, string source)
        {
            string message = string.Empty;
            message += System.DateTime.Now.ToString(@"hh\:mm\:ss");
            message += "[INFO]";
            message += "[" + source + "]";
            message += rawMessage;

            return message;
        }

        public static string Error(string rawMessage)
        {
            string message = string.Empty;
            message += System.DateTime.Now.ToString(@"hh\:mm\:ss");
            message += "[ERROR]";
            message += rawMessage;

            return message;

        }

        public static string Error(string rawMessage, string source)
        {
            string message = string.Empty;
            message += System.DateTime.Now.ToString(@"hh\:mm\:ss");
            message += "[ERROR]";
            message += "[" + source + "]";
            message += rawMessage;

            return message;
        }

        public static string Warning(string rawMessage)
        {
            string message = string.Empty;
            message += System.DateTime.Now.ToString(@"hh\:mm\:ss");
            message += "[WARNING]";
            message += rawMessage;

            return message;

        }

        public static string Warning(string rawMessage, string source)
        {
            string message = string.Empty;
            message += System.DateTime.Now.ToString(@"hh\:mm\:ss");
            message += "[WARNING]";
            message += "[" + source + "]";
            message += rawMessage;

            return message;

        }

        public static string Debug(string rawMessage)
        {
            string message = string.Empty;
            message += System.DateTime.Now.ToString(@"hh\:mm\:ss");
            message += "[DEBUG]";
            message += rawMessage;

            return message;
        }

        public static string Debug(string rawMessage, string source)
        {
            string message = string.Empty;
            message += System.DateTime.Now.ToString(@"hh\:mm\:ss");
            message += "[DEBUG]";
            message += "[" + source + "]";
            message += rawMessage;

            return message;
        }
    }
}
