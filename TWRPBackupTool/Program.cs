using static System.Console;
using static System.Convert;
using SharpAdbClient;
using System.Diagnostics;
using System.Net;

namespace TWRPBackupTool;
public class Program
{
    
    static void DisplayMenu ()
    {
        WriteLine("\t[1] Reboot to recovery\n\t[2] Backup (disconnect other devices!!)\n\t[3] Restore Backup\n\t[4] Reboot System\n\t[5] Check connectivity\n\t[6] Start Server\n\t[0] Quit\n");
    }
    
    public static async Task Main()
    {
        var choice = 0;
        var server = new AdbServer();
        var client = new AdbClient();
        var devices = client.GetDevices();
       
       

        do
        {
           DisplayMenu();
           Write("Reply: ");
           choice = ToInt32(ReadLine());
           switch(choice)
           {
                case 1:
                    WriteLine("=====================================================================");
                    WriteLine("List of Devices attached");
                    foreach (var device in devices)
                    {
                        WriteLine($"  {devices.IndexOf(device) + 1} \t{device.Name}   \t{device.Model}\t    {device.State}\t  {device.Serial}\n\n");
                    }
                    WriteLine("_____________________________________________________________________\n");
                    Write("Choose Device: ");
                    var exist = devices.FirstOrDefault(d => devices.IndexOf(d) + 1 == ToInt32(ReadLine()));
                    if (exist == null)
                    {
                        WriteLine("Invalid entry.");
                    }
                    client.Reboot("recovery", exist);
                    WriteLine("Done... \n");
                    break;
                case 2:
                    {
                        //check if device is in recovery
                        var dev = devices.First();
                        if(dev.State !=DeviceState.Recovery)
                        {
                            WriteLine("Device is not in recovery mode, trying reboot..");
                            if (dev.State == DeviceState.Offline && dev.State == DeviceState.Unknown)
                            {
                                WriteLine("Device is offline or unknown state");
                                break;
                            }
                            else if(dev.State==DeviceState.Unauthorized)
                            {
                                WriteLine("Please enable usb debugging..");
                                break;
                            }
                            else
                            {
                                client.Reboot("recovery", dev);
                                WriteLine("Press any key to continue");
                                ReadLine();
                            }
                        }
                        var receivedMessage = new ConsoleOutputReceiver();
                        var command = @"adb backup --twrp";
                        var startInfo = new ProcessStartInfo
                        {
                            FileName = "cmd.exe",
                            Arguments = "/K " + command,
                            RedirectStandardOutput = true,
                            CreateNoWindow = true,
                            WindowStyle = ProcessWindowStyle.Hidden
                        };
                        using var cmd = Process.Start(startInfo);
                        string output = cmd.StandardOutput.ReadToEnd(); 
                        WriteLine(output);
                        cmd.WaitForExit();
                        
                        break;
                    }
                case 6:
                   var result =server.StartServer(@"C:\Program Files (x86)\Android\android-sdk\platform-tools\adb.exe", false);
        
                   WriteLine("adb server: " + result.ToString());                                   
                   break;
                case 5:                  
                    WriteLine("=====================================================================");
                    WriteLine("List of Devices attached");
                    foreach (var device in devices)
                    {
                        WriteLine($"  {devices.IndexOf(device)+1} \t{device.Name}   \t{device.Model}\t    {device.State}\t  {device.Serial}\n\n");
                    }
                    WriteLine("_____________________________________________________________________\n");
                    break;
                case 4:
                    WriteLine("=====================================================================");
                    WriteLine("List of Devices attached");
                    foreach (var device in devices)
                    {
                        WriteLine($"  {devices.IndexOf(device) + 1} \t{device.Name}   \t{device.Model}\t    {device.State}\t  {device.Serial}\n\n");
                    }
                    WriteLine("_____________________________________________________________________\n");
                    Write("Choose Device: ");
                    var exist1 = devices.FirstOrDefault(d => devices.IndexOf(d) + 1 == ToInt32(ReadLine()));
                    if(exist1==null)
                    {
                        WriteLine("Invalid entry.");
                    }
                    client.Reboot("system", exist1);
                    WriteLine("Done... \n");
                    break;
            }
        } while (choice!=0);
    }
}

