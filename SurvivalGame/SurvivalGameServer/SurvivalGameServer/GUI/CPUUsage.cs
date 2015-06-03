/****************************************/
/* From: Writing High-Performance .NET Code */
/* Writer: Ben Watson */
/* Link: http://www.philosophicalgeek.com/2009/01/03/determine-cpu-usage-of-current-process-c-and-c/ */
/****************************************/

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using ComTypes = System.Runtime.InteropServices.ComTypes;

namespace Mentula.SurvivalGameServer.GUI
{
    internal class CPUUsage
    {
        private bool IsFirstRun { get { return lastRun == DateTime.MinValue; } }
        private bool EnoughTimePassed 
        {
            get
            {
                const int minElapMS = 250;
                TimeSpan diff = DateTime.Now - lastRun;
                return diff.TotalMilliseconds > minElapMS;
            }
        }

        private ComTypes.FILETIME prevSysKernel, prevSysUser;
        private TimeSpan prevProcTotal;

        private Int16 cpuUsage;
        private DateTime lastRun;
        private long runCount;

        public CPUUsage()
        {
            prevSysUser.dwHighDateTime = prevSysUser.dwLowDateTime = 0;
            prevSysKernel.dwHighDateTime = prevSysKernel.dwLowDateTime = 0;

            prevProcTotal = TimeSpan.MinValue;

            cpuUsage = 0;
            lastRun = DateTime.MinValue;
            runCount = 0;
        }

        public short GetUsage()
        {
            short cpuCopy = cpuUsage;

            if (Interlocked.Increment(ref runCount) == 1)
            {
                if (!EnoughTimePassed)
                {
                    Interlocked.Decrement(ref runCount);
                    return cpuCopy;
                }

                ComTypes.FILETIME sysIdle, sysKernel, sysUser;
                TimeSpan procTime;

                Process process = Process.GetCurrentProcess();
                procTime = process.TotalProcessorTime;

                if (!GetSystemTimes(out sysIdle, out sysKernel, out sysUser))
                {
                    Interlocked.Decrement(ref runCount);
                    return cpuCopy;
                }

                if (!IsFirstRun)
                {
                    UInt64 sysKernelDiff = SubtractTimes(sysKernel, prevSysKernel);
                    UInt64 sysUserDiff = SubtractTimes(sysUser, prevSysUser);

                    UInt64 sysTotal = sysKernelDiff + sysUserDiff;
                    Int64 procTotal = procTime.Ticks - prevProcTotal.Ticks;

                    if (sysTotal > 0) cpuUsage = (short)((100f * procTotal) / sysTotal);
                }

                prevProcTotal = procTime;
                prevSysKernel = sysKernel;
                prevSysUser = sysUser;

                lastRun = DateTime.Now;

                cpuCopy = cpuUsage;
            }
            Interlocked.Decrement(ref runCount);

            return cpuCopy;
        }

        private UInt64 SubtractTimes(ComTypes.FILETIME a, ComTypes.FILETIME b)
        {
            UInt64 aInt = ((UInt64)(a.dwHighDateTime << 32)) | (UInt64)a.dwLowDateTime;
            UInt64 bInt = ((UInt64)(b.dwHighDateTime << 32)) | (UInt64)b.dwLowDateTime;

            return aInt - bInt;
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool GetSystemTimes(out ComTypes.FILETIME lpIdleTime, out ComTypes.FILETIME lpKernelTime, out ComTypes.FILETIME lpUserTime);
    }
}
