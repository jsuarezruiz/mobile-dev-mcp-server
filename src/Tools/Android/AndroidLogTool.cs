﻿using MobileDevMcpServer.Helpers;
using MobileDevMcpServer.Models;
using ModelContextProtocol.Server;
using System.ComponentModel;

namespace MobileDevMcpServer
{
    [McpServerToolType]
    public class AndroidLogTool
    {
        /// <summary>
        /// Retrieves the system logs from a connected Android device using the logcat tool.
        /// </summary>
        /// <returns>
        /// <param name="deviceSerial">The serial number of the target Android device. This is required if multiple devices are connected.</param>
        /// <param name="maxLines">The maximum number of log lines to retrieve (default: 1000).</param>
        /// A string containing the log output captured from the device.
        /// </returns>
        [McpServerTool("android_device_logcat")]
        [Description("Retrieves the system logs from the connected Android device using logcat.")]
        public string GetDeviceLogcat(string deviceSerial, int maxLines = 1000)
        {
            try
            {
                if (!Adb.CheckAdbInstalled())
                {
                    throw new Exception("ADB is not installed or not in PATH. Please install ADB and ensure it is in your PATH.");
                }

                string adbCommand = $"adb -s {deviceSerial} logcat -d";

                // Line limit
                if (maxLines > 0)
                {
                    adbCommand += $" -t {maxLines}";
                }

                // Execute the adb logcat command to capture logs from the device
                string result = Process.ExecuteCommand(adbCommand);

                return result;
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }

        /// <summary>
        /// Retrieves system logs from the connected Android device using the logcat tool.
        /// This method filters log entries based on the specified log level (e.g., Verbose, Debug, Info, etc.)
        /// and allows you to limit the number of lines retrieved.
        /// <param name="deviceSerial">
        /// The serial number of the target Android device. This is required if multiple devices are connected.
        /// </param>
        /// </summary>
        /// <param name="logLevel">
        /// The log level to filter by (Verbose, Debug, Info, Warning, Error, Fatal).
        /// Only log entries matching the specified level will be included in the output.
        /// </param>
        /// <param name="maxLines">
        /// The maximum number of log lines to retrieve (default: 1000).
        /// Helps limit the amount of log data for easier analysis.
        /// </param>
        /// <returns>
        /// A string containing filtered log entries based on the specified log level.
        /// If no matching logs are found, an empty string is returned.
        /// </returns>
        [McpServerTool("android_device_logcat_log_level")]
        [Description("Retrieves the system logs from the connected Android device using logcat by Log Level.")]
        public string GetDeviceLogcatByLogLevel(string deviceSerial, LogCatLogLevel logLevel, int maxLines = 1000)
        {
            var logs = GetDeviceLogcat(deviceSerial, maxLines);

            // Convert log level to the corresponding letter (e.g., V, D, I, W, E, F)
            string logLevelLetter = logLevel switch
            {
                LogCatLogLevel.Verbose => "V",
                LogCatLogLevel.Debug => "D",
                LogCatLogLevel.Info => "I",
                LogCatLogLevel.Warning => "W",
                LogCatLogLevel.Error => "E",
                LogCatLogLevel.Fatal => "F",
                _ => throw new ArgumentException("Invalid log level specified.")
            };

            // Filter logs to include only the specified log level
            var filteredLogs = logs.Split('\n')
                .Where(line => line.Contains($"/{logLevelLetter} "))
                .ToList();

            // Combine filtered lines into a single string
            return string.Join("\n", filteredLogs);
        }
    }
}