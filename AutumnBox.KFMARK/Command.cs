using System;
using System.IO;
using System.Net;
using AutumnBox.Basic.Calling;
using AutumnBox.Basic.Device;
using AutumnBox.OpenFramework.Open;

namespace AutumnBox.KFMARK
{
    public class Command
    {
        private const string KFURL = "https://atmb.sm9.top/AutumnBox/拓展模块/MonoLogueChi/KFMARK/static/KFMARK.Beta.Android.apk";
        private const string KFDURL = "https://atmb.sm9.top/AutumnBox/拓展模块/MonoLogueChi/KFMARK/static/daemon";

        private readonly CommandExecutor _executer = new CommandExecutor();

        /// <summary>
        /// 连接设备信息
        /// </summary>
        /// <param name="device"></param>
        /// <returns></returns>
        public string DeviceInfo(IDevice device)
        {
            return _executer.Adb(device, "devices").Output;
        }

        /// <summary>
        /// 是否安装快否
        /// </summary>
        /// <param name="devices"></param>
        /// <returns></returns>
        public bool IsInstallKF(IDevice devices)
        {
            var KFPath = _executer.AdbShell(devices, "pm path com.af.benchaf").Output;
            if (string.IsNullOrWhiteSpace(KFPath)) return false;
            else return true;
        }

        /// <summary>
        /// 卸载快否
        /// </summary>
        /// <param name="device"></param>
        /// <returns></returns>
        public int UnInstallKF(IDevice device)
        {
            return _executer.AdbAsync(device, "uninstall com.af.benchaf").Result.ExitCode;
        }

        /// <summary>
        /// 安装快否
        /// </summary>
        /// <param name="device"></param>
        /// <param name="tmp"></param>
        /// <returns></returns>
        public int InstallKF(IDevice device, ITemporaryFloder tmp)
        {
            return _executer.Adb(device, $"install {Path.Combine(tmp.Path, "com.af.benchaf.apk")}").ExitCode;
        }

        /// <summary>
        /// 激活快否
        /// </summary>
        /// <param name="device"></param>
        /// <param name="tmp"></param>
        /// <returns></returns>
        public int ActivationKF(IDevice device,ITemporaryFloder tmp)
        {
            var a = _executer.Adb(device, $"push {Path.Combine(tmp.Path, "daemon")} /data/local/tmp").ExitCode;
            var b = _executer.AdbShell(device, "chmod 777 /data/local/tmp/daemon").ExitCode;
            var c = _executer.AdbShell(device, "\"./data/local/tmp/daemon &\"").ExitCode;
            return a + b + c;
        }


        /// <summary>
        /// 下载快否
        /// </summary>
        /// <returns></returns>
        public bool GetKF(ITemporaryFloder tmp, bool YNGetKF)
        {
            using (WebClient webClient = new WebClient())
            {
                try
                {
                    if (YNGetKF)
                    {
                        webClient.DownloadFile(
                            new Uri(KFURL),
                            Path.Combine(tmp.Path, "com.af.benchaf.apk"));
                    }
                    webClient.DownloadFile(new Uri(KFDURL), Path.Combine(tmp.Path, "daemon"));
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }
    }
}
