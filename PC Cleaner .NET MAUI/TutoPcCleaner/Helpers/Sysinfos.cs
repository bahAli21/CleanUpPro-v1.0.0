using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NvAPIWrapper;
using NvAPIWrapper.GPU;


namespace TutoPcCleaner.Helpers
{
    public class Sysinfos
    {
        /// <summary>
        /// Get Windows version
        /// </summary>
        /// <returns>Windows version string</returns>
        public string GetWinVer()
        {
            try
            {
                return Environment.OSVersion.ToString();
            }
            catch {
                return "Windows";
            }
        }

        /// <summary>
        /// Get computer hardware infos (CPU, GPU)
        /// </summary>
        /// <returns>string</returns>
        public string GetHardwareInfos()
        {
            StringBuilder sb = new StringBuilder();

            // CPU
            RegistryKey processor_name = Registry.LocalMachine.OpenSubKey(@"Hardware\Description\System\CentralProcessor\0", RegistryKeyPermissionCheck.ReadSubTree);

            if (processor_name != null)
            {
                sb.AppendLine($"{processor_name.GetValue("ProcessorNameString")}");
            }

            // GPU
            try
            {
                NVIDIA.Initialize();
                sb.AppendLine($"{PhysicalGPU.GetPhysicalGPUs()[0]}");
            }
            catch (Exception ex) { }

            return sb.ToString();
        }
    }
}
