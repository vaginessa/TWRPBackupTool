using static System.Console;
namespace TWRPBackupTool;
public class Program
{
    static void DisplayMenu ()
    {
        WriteLine("\t[1] Reboot to recovery\n\t[2] Backup\n\t[3] Restore Backup\n\t[4] Reboot System\n\t[Q] Quit\n");
    }
    public static async Task Main()
    {
        DisplayMenu();
    }
}

