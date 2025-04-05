using MobileDevMcpServer.Helpers;
using ModelContextProtocol.Server;
using System.ComponentModel;

namespace MobileDevMcpServer
{
    [McpServerToolType]
    public class AndroidShellTool
    {
        /// <summary>
        /// Runs a shell command on the specified device.
        /// </summary>
        /// <param name="deviceSerial">The serial number of the device.</param>
        /// <param name="command">The shell command to execute.</param>
        /// <returns>A formatted string containing the command output.</returns>
        [McpServerTool(Name = "android_shell_command")]
        [Description("Runs a shell command on the specified device.")]
        public string ShellCommand(string deviceSerial, string command)
        {
            if (string.IsNullOrEmpty(deviceSerial))
            {
                return $"Error: Device not found.";
            }

            try
            {
                string result = Process.ExecuteCommand($"adb -s {deviceSerial} shell {command}");

                // Check for security validation failure
                if (result.StartsWith("Error: Command rejected", StringComparison.OrdinalIgnoreCase))
                {
                    return result;
                }

                // Format the output
                string output = $"# Command Output from {deviceSerial}\n\n";
                output += $"```\n{result}\n```";

                return output;
            }
            catch (Exception ex)
            {
                return $"Error executing shell command: {ex.Message}";
            }
        }
    }
}