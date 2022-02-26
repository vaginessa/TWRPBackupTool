using static System.Console;
using static System.Convert;
using System.Diagnostics;
namespace TWRPBackupTool;
public class Program
{
    static void DisplayMenu ()
    {
        WriteLine("\t[1] Reboot to recovery\n\t[2] Backup\n\t[3] Restore Backup\n\t[4] Reboot System\n\t[0] Quit\n");
    }
    static Task RebootToRecovery()
    {
        var command = "/K adb reboot recovery";
        var startInfo = new ProcessStartInfo
        {
            FileName = "cmd.exe",
            Arguments = "/K " + command,
            UseShellExecute = true
        };
        Process.Start(startInfo);
        return Task.CompletedTask;
    }
    public static async Task Main()
    {
        var choice = 0;
        do
        {
           DisplayMenu();
           Write("Reply: ");
           choice = ToInt32(ReadLine());
           switch(choice)
           {
                case 1:
                    await RebootToRecovery();
                    Write("Next Command: ");
                    choice = ToInt32(ReadLine());
                    break;
                
           }
        } while (choice!=0);
    }
}

