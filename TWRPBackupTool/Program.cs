using Console = Colorful.Console;
using static System.Convert;
using SharpAdbClient;
using System.Diagnostics;
using System.Net;
using System.Text;
using Colorful;
using System.Drawing;

namespace TWRPBackupTool;
public class Program
{
    
    static void DisplayMenu()
    {
        
        Console.WriteLine("\t[1] Reboot to recovery\n\t[2] Backup (disconnect other devices!!)\n\t[3] Restore Backup\n\t[4] Reboot System\n\t[5] Check connectivity\n\t[6] Start Server\n\t[0] Quit\n");
    }
    static void Welcome()
    {
        Console.WriteAscii("TWRP Backup Tool", Color.FromArgb(131, 184, 214));
        Console.WriteLine("\t\t\t\t\t\t\t\tBy Wisdom Jere, github: @izzyjere", Color.Green);
    }
    public static async Task Main()
    {
        var choice = 0;
        var server = new AdbServer();
        var client = new AdbClient();
        var devices = client.GetDevices();
        Welcome();


        do
        {
            DisplayMenu();
            Console.Write("Reply: ");
            choice = ToInt32(Console.ReadLine());
            switch (choice)
            {
                case 1:
                    Console.WriteLine("=====================================================================");
                    Console.WriteLine("List of Devices attached");
                    foreach (var device in devices)
                    {
                        Console.WriteLine($"  {devices.IndexOf(device) + 1} \t{device.Name}   \t{device.Model}\t    {device.State}\t  {device.Serial}\n\n");
                    }
                    Console.WriteLine("_____________________________________________________________________\n");
                    Console.Write("Choose Device: ");
                    var exist = devices.FirstOrDefault(d => devices.IndexOf(d) + 1 == ToInt32(Console.ReadLine()));
                    if (exist == null)
                    {
                        Console.WriteLine("Invalid entry.");
                    }
                    client.Reboot("recovery", exist);
                    Console.WriteLine("Done... \n");
                    break;
                case 2:
                    {
                        //check if device is in recovery
                        if (!devices.Any())
                        {
                            Console.WriteLine("No devices detected..");
                            break;
                        }
                        var dev = devices.First();
                        if (dev.State != DeviceState.Recovery)
                        {
                            Console.WriteLine("Device is not in recovery mode, trying reboot..");
                            if (dev.State == DeviceState.Offline && dev.State == DeviceState.Unknown)
                            {
                                Console.WriteLine("Device is offline or unknown state");
                                break;
                            }
                            else if (dev.State == DeviceState.Unauthorized)
                            {
                                Console.WriteLine("Please enable usb debugging..");
                                break;
                            }
                            else
                            {
                                client.Reboot("recovery", dev);
                                Console.WriteLine("Press any key to continue");
                                Console.ReadLine();
                            }
                        }
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
                        var output = new StringBuilder();
                        cmd.OutputDataReceived += (sender, args) => output.AppendLine(args.Data);
                        cmd.BeginOutputReadLine();
                        Console.WriteLine("Check your device and Select partitions to backup. Don't close this window until backup is finished.");
                        ShowSpinner();
                        cmd.WaitForExit();
                        string stdOut = output.ToString();
                        Console.WriteLine(stdOut);
                        break;
                    }
                case 3:
                    {
                        Console.Write("Enter backup name (e.g) backup.ab");
                        var filename = Console.ReadLine();
                        //check if device is in recovery
                        if (!devices.Any())
                        {
                            Console.WriteLine("No devices detected..");
                            break;
                        }
                        var dev = devices.First();

                        if (dev.State == DeviceState.Offline && dev.State == DeviceState.Unknown)
                        {
                            Console.WriteLine("Device is offline or unknown state");
                            break;
                        }
                        else if (dev.State == DeviceState.Unauthorized)
                        {
                            Console.WriteLine("Please enable usb debugging..");
                            break;
                        }
                        var receivedMessage = new ConsoleOutputReceiver();
                        var command = @$"adb restore {filename}";
                        var startInfo = new ProcessStartInfo
                        {
                            FileName = "cmd.exe",
                            Arguments = "/K " + command,
                            RedirectStandardOutput = true,
                            CreateNoWindow = true,
                            WindowStyle = ProcessWindowStyle.Hidden
                        };
                        using var cmd = Process.Start(startInfo);
                        var output = new StringBuilder();
                        cmd.OutputDataReceived += (sender, args) => output.AppendLine(args.Data);
                        cmd.BeginOutputReadLine();
                        Console.WriteLine("Restore started don't close  this window until your phone reboots..");
                        cmd.WaitForExit();
                        string stdOut = output.ToString();
                        Console.WriteLine(stdOut);
                        break;
                    }
                case 6:
                    var result = server.StartServer(@"C:\Program Files (x86)\Android\android-sdk\platform-tools\adb.exe", false);

                    Console.WriteLine("adb server: " + result.ToString());
                    break;
                case 5:
                    Console.WriteLine("=====================================================================");
                    Console.WriteLine("List of Devices attached");
                    foreach (var device in devices)
                    {
                        Console.WriteLine($"  {devices.IndexOf(device) + 1} \t{device.Name}   \t{device.Model}\t    {device.State}\t  {device.Serial}\n\n");
                    }
                    Console.WriteLine("_____________________________________________________________________\n");
                    break;
                case 4:
                    Console.WriteLine("=====================================================================");
                    Console.WriteLine("List of Devices attached");
                    foreach (var device in devices)
                    {
                        Console.WriteLine($"  {devices.IndexOf(device) + 1} \t{device.Name}   \t{device.Model}\t    {device.State}\t  {device.Serial}\n\n");
                    }
                    Console.WriteLine("_____________________________________________________________________\n");
                    Console.Write("Choose Device: ");
                    var exist1 = devices.FirstOrDefault(d => devices.IndexOf(d) + 1 == ToInt32(Console.ReadLine()));
                    if (exist1 == null)
                    {
                        Console.WriteLine("Invalid entry.");
                    }
                    client.Reboot("system", exist1);
                    Console.WriteLine("Done... \n");
                    break;
                default:
                    Console.WriteLine("Invalid Option");
                    break;
                case 0:
                    Console.Clear();
                    Console.WriteAscii("Good Bye", Color.FromArgb(131, 184, 214));
                    break;
            }
        } while (choice != 0);
    }
    static async void ShowSpinner()
    {
        var counter = 0;
        for (int i = 0; ; i++)
        {
            switch (counter % 4)
            {
                case 0: Console.Write("/"); await Task.Delay(100); break;
                case 1: Console.Write("-"); await Task.Delay(100); break;
                case 2: Console.Write("\\"); await Task.Delay(100); break;
                case 3: Console.Write("|"); await Task.Delay(100); break;
            }
            Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
            counter++;
           
        }
    }
}

