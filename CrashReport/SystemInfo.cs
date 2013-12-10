// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CrashReport
{
    public struct SystemInfo
    {
        // List of elements that we collect
        public List<VideoControllerInfo> VideoControllers;
        public List<KeyboardInfo> Keyboards;
        public List<PointingDeviceInfo> PointingDevices;
        public List<DiskDriveInfo> DiskDrives;
        public List<NetworkAdapterInfo> NetworkAdapters;
        public List<PhysicalMemoryInfo> PhysicalMemory;
        public List<ProcessorInfo> Processors;
        public List<OperatingSystemInfo> OperatingSystems;

        public void Init()
        {
            this.VideoControllers = new List<VideoControllerInfo>();
            this.Keyboards = new List<KeyboardInfo>();
            this.PointingDevices = new List<PointingDeviceInfo>();
            this.DiskDrives = new List<DiskDriveInfo>();
            this.NetworkAdapters = new List<NetworkAdapterInfo>();
            this.PhysicalMemory = new List<PhysicalMemoryInfo>();
            this.Processors = new List<ProcessorInfo>();
            this.OperatingSystems = new List<OperatingSystemInfo>();
        }

        // Formatted output
        public override string ToString()
        {
            string output = "```lang=none, lines=15" + "\n";
            
            foreach (var v in this.Processors.Select((result, id) => new { ID = id, Result = result }))
            {
                output += "Processor " + v.ID.ToString() + "\n===========\n";
                output += v.Result.ToString() + "\n";
            }

            foreach (var v in this.OperatingSystems.Select((result, id) => new { ID = id, Result = result }))
            {
                output += "Operating System " + v.ID.ToString() + "\n==================\n";
                output += v.Result.ToString() + "\n";
            }

            foreach (var v in this.VideoControllers.Select((result, id) => new { ID = id, Result = result }))
            {
                output += "Video Controller " + v.ID.ToString() + "\n==================\n";
                output += v.Result.ToString() + "\n";
            }

            foreach (var v in this.PhysicalMemory.Select((result, id) => new { ID = id, Result = result }))
            {
                output += "Physical Memory " + v.ID.ToString() + "\n=================\n";
                output += v.Result.ToString() + "\n";
            }

            foreach (var v in this.NetworkAdapters.Select((result, id) => new { ID = id, Result = result }))
            {
                output += "Network Adapter " + v.ID.ToString() + "\n=================\n";
                output += v.Result.ToString() + "\n";
            }

            foreach (var v in this.Keyboards.Select((result, id) => new { ID = id, Result = result }))
            {
                output += "Keyboard " + v.ID.ToString() + "\n==========\n";
                output += v.Result.ToString() + "\n";
            }

            foreach (var v in this.PointingDevices.Select((result, id) => new { ID = id, Result = result }))
            {
                output += "Pointing Device " + v.ID.ToString() + "\n=================\n";
                output += v.Result.ToString() + "\n";
            }

            foreach (var v in this.DiskDrives.Select((result, id) => new { ID = id, Result = result }))
            {
                output += "Disk Drive " + v.ID.ToString() + "\n============\n";
                output += v.Result.ToString() + "\n";
            }

            output += "```";

            return output;
        }
    }
}
