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
        public static SystemInfo GetSystemInfo()
        {
            // Create system info struct
            SystemInfo systemInfo = new SystemInfo();
            systemInfo.Init();
            ManagementObjectCollection classObjects;

            // Check VideoControllers
            ManagementClass videoController = new ManagementClass("Win32_VideoController");
            classObjects = videoController.GetInstances();

            foreach (ManagementObject classObject in
                classObjects)
            {
                VideoControllerInfo infoStruct = new VideoControllerInfo
                {
                    AdapterRAM = (classObject.GetPropertyValue("AdapterRAM") ?? new object()).ToString(),
                    ConfigManagerErrorCode = (uint?)classObject.GetPropertyValue("ConfigManagerErrorCode"),
                    CurrentHorizontalResolution = (uint?)classObject.GetPropertyValue("CurrentHorizontalResolution"),
                    CurrentVertialResolution = (uint?)classObject.GetPropertyValue("CurrentVerticalResolution"),
                    Description = (classObject.GetPropertyValue("Description") ?? new object()).ToString(),
                    DriverVersion = (classObject.GetPropertyValue("DriverVersion") ?? new object()).ToString(),
                    InstalledDisplayDrivers = (classObject.GetPropertyValue("InstalledDisplayDrivers") ?? new object()).ToString(),
                    MaxMemorySupported = (uint?)classObject.GetPropertyValue("MaxMemorySupported"),
                    Name = (classObject.GetPropertyValue("Name") ?? new object()).ToString() 
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
                    ConfigManagerErrorCode = (uint?)classObject.GetPropertyValue("ConfigManagerErrorCode"),
                    Description = (classObject.GetPropertyValue("Description") ?? new object()).ToString(),
                    Layout = (classObject.GetPropertyValue("Layout") ?? new object()).ToString(),
                    Name = (classObject.GetPropertyValue("Name") ?? new object()).ToString()
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
                    ConfigManagerErrorCode = (uint?)classObject.GetPropertyValue("ConfigManagerErrorCode"),
                    Description = (classObject.GetPropertyValue("Description") ?? new object()).ToString(),
                    Handedness = (classObject.GetPropertyValue("Handedness") ?? new object()).ToString(),
                    HardwareType = (classObject.GetPropertyValue("HardwareType") ?? new object()).ToString(),
                    Name = (classObject.GetPropertyValue("Name") ?? new object()).ToString(),
                    NumberOfButtons = (classObject.GetPropertyValue("NumberOfButtons") ?? new object()).ToString()
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
                    ConfigManagerErrorCode = (uint?)classObject.GetPropertyValue("ConfigManagerErrorCode"),
                    Description = (classObject.GetPropertyValue("Description") ?? new object()).ToString(),
                    MediaType = (classObject.GetPropertyValue("MediaType") ?? new object()).ToString(),
                    Name = (classObject.GetPropertyValue("Name") ?? new object()).ToString(),
                    Size = (ulong?)classObject.GetPropertyValue("Size")
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
                    ConfigManagerErrorCode = (uint?)classObject.GetPropertyValue("ConfigManagerErrorCode"),
                    Description = (classObject.GetPropertyValue("Description") ?? new object()).ToString(),
                    Manufacturer = (classObject.GetPropertyValue("Manufacturer") ?? new object()).ToString(),
                    Name = (classObject.GetPropertyValue("Name") ?? new object()).ToString(),
                    ProductName = (classObject.GetPropertyValue("ProductName") ?? new object()).ToString()
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
                    Description = (classObject.GetPropertyValue("Description") ?? new object()).ToString(),
                    Manufacturer = (classObject.GetPropertyValue("Manufacturer") ?? new object()).ToString(),
                    Model = (classObject.GetPropertyValue("Model") ?? new object()).ToString(),
                    Name = (classObject.GetPropertyValue("Name") ?? new object()).ToString()
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
                    Architecture = (classObject.GetPropertyValue("Architecture") ?? new object()).ToString(),
                    ConfigManagerErrorCode = (uint?)classObject.GetPropertyValue("ConfigManagerErrorCode"),
                    Description = (classObject.GetPropertyValue("Description") ?? new object()).ToString(),
                    Family = (classObject.GetPropertyValue("Family") ?? new object()).ToString(),
                    Manufacturer = (classObject.GetPropertyValue("Manufacturer") ?? new object()).ToString(),
                    MaxClockSpeed = (classObject.GetPropertyValue("MaxClockSpeed") ?? new object()).ToString(),
                    Name = (classObject.GetPropertyValue("Name") ?? new object()).ToString(),
                    ProcessorType = (classObject.GetPropertyValue("ProcessorType") ?? new object()).ToString()
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
                    BuildNumber = (classObject.GetPropertyValue("BuildNumber") ?? new object()).ToString(),
                    Description = (classObject.GetPropertyValue("Description") ?? new object()).ToString(),
                    Name = (classObject.GetPropertyValue("Name") ?? new object()).ToString(),
                    OSArchitecture = (classObject.GetPropertyValue("OSArchitecture") ?? new object()).ToString(),
                    Primary = (classObject.GetPropertyValue("Primary") ?? new object()).ToString(),
                    TotalVisibleMemorySize = (classObject.GetPropertyValue("TotalVisibleMemorySize") ?? new object()).ToString(),
                    Version = (classObject.GetPropertyValue("Version") ?? new object()).ToString()
                };
                systemInfo.OperatingSystems.Add(infoStruct);
            }

            return systemInfo;
        }

#elif PLATFORM_LINUX
        public static SystemInfo GetSystemInfo()
        {
            SystemInfo systemInfo = new SystemInfo();
            systemInfo.Init();
            return systemInfo;
        }

#else
        public static void GetSystemInfo()
        {
        }
#endif
    }
}
