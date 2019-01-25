using AutumnBox.Basic.Device;
using AutumnBox.OpenFramework.Extension;
using AutumnBox.OpenFramework.Extension.LeafExtension;
using AutumnBox.OpenFramework.Open;

namespace AutumnBox.KFMARK
{
    [ExtName("一键激活快否", "en-us:一键去除Wi-FI x和!号模块-暂定")]
    [ExtDesc("可以一键激活快否")]
    [ExtAuth("MonoLogueChi")]
    [ExtVersion(0, minor: 0, build: 1)]
    [ExtRequiredDeviceStates((DeviceState)2)]   //开机状态使用
    [ExtMinApi(value: 8)]
    [ExtTargetApi(value: 8)]
    [ExtIcon(@"Resources.icon.png")]
    public class KFMARK : LeafExtensionBase
    {
        private readonly Command _command = new Command();

        public void Main(ILeafUI ui, IUx ux, IDevice device, ILogger logger, ITemporaryFloder tmp)
        {
            using (ui)
            {
                ui.Title = "一键激活快否";
                ui.Icon = this.GetIconBytes();
                ui.Show();

                //检测设备连接状况
                ui.Tip = "正在检测设备连接状态";
                ui.Progress = 10;
                ui.WriteLine(_command.DeviceInfo(device).Replace("\r\n\r\n", ""));

                //检测是否安装快否
                ui.Tip = "正在检测快否是否安装";
                ui.Progress = 20;
                bool YNDownKF = false;
                var IsInstallKF = _command.IsInstallKF(device);
                ui.Progress = 30;
                if (IsInstallKF)
                {
                    ui.WriteLine("已安装快否");
                    var YNGetKF = ux.DoYN("已安装快否，但建议安装我们适配的版本用于激活", "马上安装", "不，坚持使用我自己的版本");
                    if (YNGetKF)
                    {
                        var UnInstallKF = _command.UnInstallKF(device);
                        if (UnInstallKF == 0)
                        {
                            ui.WriteLine("卸载快否成功，请等待下载完成");
                            YNDownKF = true;
                        }
                        else
                        {
                            ui.WriteLine("卸载失败，请手动卸载");
                            ui.Finish("卸载失败");
                        }

                    }
                }
                else
                {
                    ui.WriteLine("未安装快否\r\n正在下载快否...");
                    YNDownKF = true;
                }
                ui.Progress = 40;

                //下载快否
                if (_command.GetKF(tmp, YNDownKF))
                {
                    ui.WriteLine("快否下载完成");
                }
                else
                {
                    ui.WriteLine("快否下载出错");
                    ui.Finish("下载出错");
                }

                ui.Progress = 70;

                //安装快否
                if (YNDownKF)
                {
                    ui.WriteLine("正在安装快否，请保持您的手机处于未锁屏状态");
                    if (_command.InstallKF(device, tmp) == 0)
                    {
                        ui.WriteLine("安装成功");
                    }
                    else
                    {
                        ui.WriteLine("安装失败");
                        ui.Finish("安装快否失败");
                    }
                }

                ui.Progress = 80;

                //检查激活结果
                if (_command.ActivationKF(device,tmp) == 0)
                {
                    ui.WriteLine("激活成功，请在手机上测试");
                }
                else
                {
                    ui.WriteLine("激活失败");
                    ui.Finish("激活失败");
                }

                ui.Finish();
            }
        }
    }
}
