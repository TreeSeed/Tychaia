// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
// Platform specific usings
#if PLATFORM_WINDOWS
using System.Management;
#endif

namespace CrashReport
{
    public static class CollectSystemInfo
    {

#if PLATFORM_WINDOWS
        public static void GetSystemInfo()
        {
            // Create system info struct
            SystemInfo systemInfo = new SystemInfo();
            ManagementObjectCollection classObjects;

            // Check VideoControllers
            ManagementClass videoController = new ManagementClass("Win32_VideoController");
            classObjects = videoController.GetInstances();

            foreach (ManagementObject classObject in
                classObjects)
            {
                VideoControllerInfo infoStruct = new VideoControllerInfo
                {
                    AcceleratorCapabilities = classObject.GetPropertyValue("AcceleratorCapabilities").ToString(),
                    AdapterRAM = classObject.GetPropertyValue("AdapterRAM").ToString(),
                    ConfigManagerErrorCode = classObject.GetPropertyValue("ConfigManagerErrorCode").ToString(),
                    CurrentHorizontalResolution = classObject.GetPropertyValue("CurrentHorizontalResolution").ToString(),
                    CurrentVertialResolution = classObject.GetPropertyValue("CurrentVerticalResolution").ToString(),
                    Description = classObject.GetPropertyValue("Description").ToString(),
                    DriverVersion = classObject.GetPropertyValue("DriverVersion").ToString(),
                    InstalledDisplayDrivers = classObject.GetPropertyValue("InstalledDisplayDrivers").ToString(),
                    MaxMemorySupported = classObject.GetPropertyValue("MaxMemorySupported").ToString(),
                    Name = classObject.GetPropertyValue("Name").ToString() 
                };
                systemInfo.VideoControllers.Add(infoStruct);
            }

            // Check Keyboards
            ManagementClass keyboard = new ManagementClass("Win32_Keyboard");
            classObjects = keyboard.GetInstances();

            foreach (ManagementObject classObject in
                classObjects)
            {
                KeyboardInfo infoStruct = new KeyboardInfo
                {
                    ConfigManagerErrorCode = classObject.GetPropertyValue("ConfigManagerErrorCode").ToString(),
                    Description = classObject.GetPropertyValue("Description").ToString(),
                    Layout = classObject.GetPropertyValue("Layout").ToString(),
                    Name = classObject.GetPropertyValue("Name").ToString()
                };
                systemInfo.Keyboards.Add(infoStruct);
            }

            // Check Pointing Devices
            ManagementClass pointingDevice = new ManagementClass("Win32_PointingDevice");
            classObjects = pointingDevice.GetInstances();

            foreach (ManagementObject classObject in
                classObjects)
            {
                PointingDeviceInfo infoStruct = new PointingDeviceInfo
                {
                    ConfigManagerErrorCode = classObject.GetPropertyValue("ConfigManagerErrorCode").ToString(),
                    Description = classObject.GetPropertyValue("Description").ToString(),
                    Handedness = classObject.GetPropertyValue("Handedness").ToString(),
                    HardwareType = classObject.GetPropertyValue("HardwareType").ToString(),
                    Name = classObject.GetPropertyValue("Name").ToString(),
                    NumberOfButtons = classObject.GetPropertyValue("NumberOfButtons").ToString(),
                    PointingType = classObject.GetPropertyValue("PointingType").ToString()
                };
                systemInfo.PointingDevices.Add(infoStruct);
            }

            // Check Disk Drives
            ManagementClass diskDrive = new ManagementClass("Win32_DiskDrive");
            classObjects = diskDrive.GetInstances();

            foreach (ManagementObject classObject in
                classObjects)
            {
                DiskDriveInfo infoStruct = new DiskDriveInfo
                {
                    ConfigManagerErrorCode = classObject.GetPropertyValue("ConfigManagerErrorCode").ToString(),
                    Description = classObject.GetPropertyValue("Description").ToString(),
                    MediaType = classObject.GetPropertyValue("MediaType").ToString(),
                    Name = classObject.GetPropertyValue("Name").ToString()
                };
                systemInfo.DiskDrives.Add(infoStruct);
            }

            // Check Network Adapters
            ManagementClass networkAdapter = new ManagementClass("Win32_NetworkAdapter");
            classObjects = networkAdapter.GetInstances();

            foreach (ManagementObject classObject in
                classObjects)
            {
                NetworkAdapterInfo infoStruct = new NetworkAdapterInfo
                {
                    AdapterTypeID = classObject.GetPropertyValue("AdapterTypeID").ToString(),
                    ConfigManagerErrorCode = classObject.GetPropertyValue("ConfigManagerErrorCode").ToString(),
                    Description = classObject.GetPropertyValue("Description").ToString(),
                    Manufacturer = classObject.GetPropertyValue("Manufacturer").ToString(),
                    Name = classObject.GetPropertyValue("Name").ToString(),
                    NetConnectionID = classObject.GetPropertyValue("NetConnectionID").ToString(),
                    NetConnectionStatus = classObject.GetPropertyValue("NetConnectionStatus").ToString(),
                    NetEnabled = classObject.GetPropertyValue("NetEnabled").ToString(),
                    ProductName = classObject.GetPropertyValue("ProductName").ToString()
                };
                systemInfo.NetworkAdapters.Add(infoStruct);
            }

            // Check Physical Memory
            ManagementClass physicalMemory = new ManagementClass("Win32_PhysicalMemory");
            classObjects = physicalMemory.GetInstances();

            foreach (ManagementObject classObject in
                classObjects)
            {
                PhysicalMemoryInfo infoStruct = new PhysicalMemoryInfo
                {
                    Description = classObject.GetPropertyValue("Description").ToString(),
                    Manufacturer = classObject.GetPropertyValue("Manufacturer").ToString(),
                    Model = classObject.GetPropertyValue("Model").ToString(),
                    Name = classObject.GetPropertyValue("Name").ToString()
                };
                systemInfo.PhysicalMemory.Add(infoStruct);
            }

            // Check Proceessors
            ManagementClass processor = new ManagementClass("Win32_Processor");
            classObjects = processor.GetInstances();

            foreach (ManagementObject classObject in
                classObjects)
            {
                ProcessorInfo infoStruct = new ProcessorInfo
                {
                    Architecture = classObject.GetPropertyValue("Architecture").ToString(),
                    ConfigManagerErrorCode = classObject.GetPropertyValue("ConfigManagerErrorCode").ToString(),
                    Description = classObject.GetPropertyValue("Description").ToString(),
                    Family = classObject.GetPropertyValue("Family").ToString(),
                    Manufacturer = classObject.GetPropertyValue("Manufacturer").ToString(),
                    MaxClockSpeed = classObject.GetPropertyValue("MaxClockSpeed").ToString(),
                    Name = classObject.GetPropertyValue("Name").ToString(),
                    ProcessorType = classObject.GetPropertyValue("ProcessorType").ToString()
                };
                systemInfo.Processors.Add(infoStruct);
            }

            // Check Operating Systems
            ManagementClass operatingSystem = new ManagementClass("Win32_OperatingSystem");
            classObjects = operatingSystem.GetInstances();

            foreach (ManagementObject classObject in
                classObjects)
            {
                OperatingSystemInfo infoStruct = new OperatingSystemInfo
                {
                    BuildNumber = classObject.GetPropertyValue("BuildNumber").ToString(),
                    Description = classObject.GetPropertyValue("Description").ToString(),
                    Name = classObject.GetPropertyValue("Name").ToString(),
                    OperatingSystemSKU = classObject.GetPropertyValue("OperatingSystemSKU").ToString(),
                    OSArchitecture = classObject.GetPropertyValue("OSArchitecture").ToString(),
                    Primary = classObject.GetPropertyValue("Primary").ToString(),
                    TotalVisibleMemorySize = classObject.GetPropertyValue("TotalVisibleMemorySize").ToString(),
                    Version = classObject.GetPropertyValue("Version").ToString()
                };
                systemInfo.OperatingSystems.Add(infoStruct);
            }
        }

#elif PLATFORM_LINUX
        public static void GetSystemInfo()
        {
        }

#else
        public static void GetSystemInfo()
        {
            
        }
#endif
    }
}
