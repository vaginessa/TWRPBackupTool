using static System.Console;
using static System.Convert;
using SharpAdbClient;
using System.Diagnostics;
namespace TWRPBackupTool;
public class Program
{
    static void DisplayMenu ()
    {
        WriteLine("\t[1] Reboot to recovery\n\t[2] Backup\n\t[3] Restore Backup\n\t[4] Reboot System\n\t[5] Check connectivity\n\t[6] Start Server\n\t[0] Quit\n");
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

                        break;
                    }
                case 6:
                   var result =server.StartServer(@"C:\Program Files (x86)\Android\android-sdk\platform-tools\adb.exe", false);
        
                   WriteLine("adb server: " + result.ToString());
                   Write("Next Command: ");
                   choice = ToInt32(ReadLine());                    
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

