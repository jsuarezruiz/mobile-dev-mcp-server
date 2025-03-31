﻿using MobileDevMcpServer.Helpers;
using ModelContextProtocol.Server;
using System.ComponentModel;

namespace MobileDevMcpServer
{
    [McpServerToolType]
    public class IosScreenshotTool
    {
        /// <summary>
        /// Captures a screenshot from the specified iOS device and returns it as a byte array.
        /// </summary>
        /// <param name="deviceId">
        /// The unique identifier (UDID) of the iOS device from which the screenshot is to be taken.
        /// This identifies the specific device to target for the screenshot operation.
        /// </param>
        /// <returns>
        /// A byte array representing the screenshot image data.
        /// Returns <c>null</c> if the screenshot operation fails.
        /// </returns>
        /// <remarks>
        /// This method interacts with the specified device to capture its current screen content.
        /// Ensure that the device with the given <paramref name="deviceId"/> is connected, 
        /// accessible, and properly configured for the operation.
        /// </remarks>
        [McpServerTool("ios_screenshot")]
        [Description("Captures a screenshot from the specified iOS device and returns it as a byte array.")]
        public static byte[]? TakeScreenshot(string deviceId)
        {
            try
            {
                Logger.LogInfo($"Initializing shell command execution for device: {deviceId}");
                
                if (!Idb.CheckIdbInstalled())
                {
                    Logger.LogError("Idb is not installed or not in PATH. Please install Idb and ensure it is in your PATH.");
                    throw new Exception("Idb is not installed or not in PATH. Please install Idb and ensure it is in your PATH.");
                }

                if (string.IsNullOrEmpty(deviceId))
                {
                    Logger.LogError($"Device not found: {deviceId}");
                    throw new Exception($"Error: Device {deviceId} not found.");
                }

                // Define a temporary file path to save the pulled screenshot locally
                string localTempFilePath = Path.GetTempFileName();

                // Take the screenshot on the device
                Logger.LogInfo("Taking screenshot...");
                Process.ExecuteCommand($"idb screenshot --udid {deviceId} \"{localTempFilePath}\"");

                // Read the screenshot image data into a byte array
                byte[] imageData = File.ReadAllBytes(localTempFilePath);

                // Delete the temporary file after reading
                File.Delete(localTempFilePath);

                Logger.LogInfo("Screenshot captured and image data retrieved successfully.");
                return imageData;
            }
            catch(Exception ex)
            {
                Logger.LogException("Error taking screenshot", ex);

                return null;
            }
        }
    }
}