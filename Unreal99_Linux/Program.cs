using System.Diagnostics;
using System.Net;
using CommandLine;

namespace Unreal99_Linux;

class Program
{
    public class Options
    {
        [Option('c', "cd", Required = true, HelpText = "Path to the Unreal99 CD or extracted GOG install.")]
        public string CDPath { get; set; }
        [Option('d', "destination", Required = true, HelpText = "Path to the destination directory.")]
        public string Destination { get; set; }
        [Option('p', "playername", Required = true, HelpText = "Player Name to use in the game.")]
        public string PlayerName { get; set; }
        [Option('w' , "widescreen", Required = false, Default = true, HelpText = "Download and install the Widescreen Patch.")]
        public bool Widescreen { get; set; }
        [Option('r', "remove", Required = false, Default = false, HelpText = "Reset your config files.")]
        public bool Remove { get; set; }
    }
    static void Main(string[] args)
    {
        Parser.Default.ParseArguments<Options>(args).WithParsed<Options>(o =>
        {
            Program program = new Program();
            program.Installer(o.CDPath, o.Destination, o.PlayerName, o.Widescreen, o.Remove);
        });
    }
    public void Installer(string objCdPath, string objDestination, string playername, bool widescreenmod, bool remove)
    {
        // Make sure Destination exists
        if (!Directory.Exists(objDestination))
        {
            Directory.CreateDirectory(objDestination);
        }
        else
        {
            // Remove all files in the destination
            Directory.Delete(objDestination, true);
            Directory.CreateDirectory(objDestination);
        }
        
        //Download the Linux Patch
        Console.WriteLine("Downloading Linux Patch...");
        Console.Title = "Unreal99 Linux Installer - Downloading Linux Patch";
        var patchUrl = "https://github.com/OldUnreal/UnrealTournamentPatches/releases/download/v469d/OldUnreal-UTPatch469d-Linux-amd64.tar.bz2";
        var patchDestination = Path.Combine(objDestination, "UTPatch469d-Linux-amd64.tar.bz2");
        using (var client = new WebClient())
        {
            client.DownloadFile(patchUrl, patchDestination);
        }
        Console.WriteLine("Linux Patch Downloaded!");
        Console.WriteLine("Extracting Linux Patch...");
        Console.Title = "Unreal99 Linux Installer - Extracting Linux Patch";
        //Extract the Linux Patch to objDestination
        //tar -xvf patchDestination -C objDestination
        
        ProcessStartInfo startInfo = new ProcessStartInfo();
        startInfo.FileName = "tar";
        startInfo.Arguments = $"-xvf \"{patchDestination}\" -C \"{objDestination}\"";
        Process process = new Process();
        process.StartInfo = startInfo;
        process.Start();
        process.WaitForExit();
        // delete the patch file
        File.Delete(patchDestination);
        Console.WriteLine("Linux Patch Extracted!");
        
        //Copy the Music and Sounds directories from the CD to the destination
        Console.WriteLine("Copying Music Directory...");
        Console.Title = "Unreal99 Linux Installer - Copying Music Directory";
        var musicSource = Path.Combine(objCdPath, "Music");
        var musicDestination = Path.Combine(objDestination, "Music");
        
        //Ensure destination directories exist
        if (!Directory.Exists(musicDestination))
        {
            Directory.CreateDirectory(musicDestination);
        }
        
        foreach (var file in Directory.GetFiles(musicSource))
        {
            // Get Filename
            var fileName = Path.GetFileName(file);
            Console.WriteLine("Copying " + fileName);
            // Copy File
            File.Copy(file, Path.Combine(musicDestination, fileName), true);
        }
        Console.WriteLine("Music Directory Copied!");
        Console.WriteLine("Copying Sounds Directory...");
        Console.Title = "Unreal99 Linux Installer - Copying Sounds Directory";
        //Ensure destination directories exist
        var soundsSource = Path.Combine(objCdPath, "Sounds");
        var soundsDestination = Path.Combine(objDestination, "Sounds");
        
        //Ensure destination directories exist
        if (!Directory.Exists(soundsDestination))
        {
            Directory.CreateDirectory(soundsDestination);
        }
        
        foreach (var file in Directory.GetFiles(soundsSource))
        {
            // Get Filename
            var fileName = Path.GetFileName(file);
            Console.WriteLine("Copying " + fileName);
            // Copy File
            File.Copy(file, Path.Combine(soundsDestination, fileName), true);
        }
        Console.WriteLine("Sounds Directory Copied!");
        
        //Copy and then unpack the Maps directory from the CD to the destination
        Console.WriteLine("Copying Maps Directory...");
        Console.Title = "Unreal99 Linux Installer - Copying Maps Directory";
        var mapsSource = Path.Combine(objCdPath, "Maps");
        var mapsDestination = Path.Combine(objDestination, "Maps");
        
        //Ensure destination directories exist
        if (!Directory.Exists(mapsDestination))
        {
            Directory.CreateDirectory(mapsDestination);
        }
        
        foreach (var file in Directory.GetFiles(mapsSource))
        {
            // Get Filename
            var fileName = Path.GetFileName(file);
            Console.WriteLine("Copying " + fileName);
            // Copy File
            File.Copy(file, Path.Combine(mapsDestination, fileName), true);
        }
        Console.WriteLine("Maps Directory Copied!");
        Console.WriteLine("Unpacking Maps Directory...");
        Console.Title = "Unreal99 Linux Installer - Unpacking Maps Directory";
        //for i in <path-to-distribution-directory>/Maps/*.uz; do <path-to-game-directory>/System/ucc-bin decompress $i; done
        var uccBin = Path.Combine(objDestination, "System64", "ucc-bin");
        foreach (var file in Directory.GetFiles(mapsDestination, "*.uz"))
        {
            Console.WriteLine("Unpacking " + file);
            ProcessStartInfo startInfo2 = new ProcessStartInfo();
            startInfo2.FileName = uccBin;
            startInfo2.Arguments = $"decompress {file}";
            Process process2 = new Process();
            process2.StartInfo = startInfo2;
            process2.Start();
            process2.WaitForExit();
        }
        //Delete the .uz files
        foreach (var file in Directory.GetFiles(mapsDestination, "*.uz"))
        {
            File.Delete(file);
        }
        Console.WriteLine("Maps Directory Unpacked!");
        //Copy Textures directory from the CD to the destination, DO NOT OVERWRITE
        Console.WriteLine("Copying Textures Directory...");
        Console.Title = "Unreal99 Linux Installer - Copying Textures Directory";
        var texturesSource = Path.Combine(objCdPath, "Textures");
        var texturesDestination = Path.Combine(objDestination, "Textures");
        foreach (var file in Directory.GetFiles(texturesSource))
        {
            // Get Filename
            var fileName = Path.GetFileName(file);
            // Check if file exists in destination
            if (!File.Exists(Path.Combine(texturesDestination, fileName)))
            {
                Console.WriteLine("Copying " + fileName);
                // Copy File
                File.Copy(file, Path.Combine(texturesDestination, fileName));
            }
        }
        Console.WriteLine("Textures Directory Copied!");
        
        
        // Lets customize the UnrealTournament.ini file
        Console.WriteLine("Customizing UnrealTournament.ini...");
        Console.Title = "Unreal99 Linux Installer - Customizing UnrealTournament.ini";
        
        var iniFile = Path.Combine(objDestination, "System64", "Default.ini");

        List<string> UnrealTournamentIni = File.ReadAllLines(iniFile).ToList();
        
        //Loop through the lines of the file
        
        for (int i = 0; i < UnrealTournamentIni.Count; i++)
        {
            if(UnrealTournamentIni[i].StartsWith("FullscreenViewportX="))
            {
                UnrealTournamentIni[i] = "FullscreenViewportX=1920";
            }
            if(UnrealTournamentIni[i].StartsWith("FullscreenViewportY="))
            {
                UnrealTournamentIni[i] = "FullscreenViewportY=1080";
            }
            if(UnrealTournamentIni[i].StartsWith("Name="))
            {
                UnrealTournamentIni[i] = $"Name={playername}";
            }
            if(UnrealTournamentIni[i].StartsWith("GameRenderDevice="))
            {
                UnrealTournamentIni[i] = "GameRenderDevice=XOpenGLDrv.XOpenGLRenderDevice";
            }
            if(UnrealTournamentIni[i].StartsWith("Console="))
            {
                if (widescreenmod)
                {
                    UnrealTournamentIni[i] = "Console=foxWSFix99.foxUTConsole";  
                }
            }
        }
        // [foxWSFix99.foxUTConsole]
        // Desired43FOV=90.000000
        // bCorrectZoomFOV=True
        // bCorrectWeaponFOV=True
        // bCorrectMouseSensitivity=True
        
        if (widescreenmod)
        {
            UnrealTournamentIni.Add("[foxWSFix99.foxUTConsole]");
            UnrealTournamentIni.Add("Desired43FOV=130.000000");
            UnrealTournamentIni.Add("bCorrectZoomFOV=True");
            UnrealTournamentIni.Add("bCorrectWeaponFOV=True");
            UnrealTournamentIni.Add("bCorrectMouseSensitivity=True");
        }
        
        File.WriteAllLines(iniFile, UnrealTournamentIni);
        Console.WriteLine("UnrealTournament.ini Customized for 1080p and OpenGL!");
        
        // Lets customize the DefUser.ini file
        Console.WriteLine("Customizing User.ini...");
        var defUserFile = Path.Combine(objDestination, "System64", "DefUser.ini");
        List<string> DefUserIni = File.ReadAllLines(defUserFile).ToList();
        //Loop through the lines of the file
        for (int i = 0; i < DefUserIni.Count; i++)
        {
            if (DefUserIni[i].StartsWith("Name="))
            {
                
                DefUserIni[i] = $"Name={playername}";
            }
            if (DefUserIni[i].StartsWith("LeftMouse="))
            {
                DefUserIni[i] = "LeftMouse=Fire";
            }
            if (DefUserIni[i].StartsWith("RightMouse="))
            {
                DefUserIni[i] = "RightMouse=AltFire";
            }
            if (DefUserIni[i].StartsWith("W="))
            {
                DefUserIni[i] = "W=MoveForward";
            }
            if (DefUserIni[i].StartsWith("S="))
            {
                DefUserIni[i] = "S=MoveBackward";
            }
            if (DefUserIni[i].StartsWith("A="))
            {
                DefUserIni[i] = "A=StrafeLeft";
            }
            if (DefUserIni[i].StartsWith("D="))
            {
                DefUserIni[i] = "D=StrafeRight";
            }
            if (DefUserIni[i].StartsWith("Space="))
            {
                DefUserIni[i] = "Space=Jump";
            }
        }
        // Find Engine.PlayerPawn
        for (int i = 0; i < DefUserIni.Count; i++)
        {
            if (DefUserIni[i].StartsWith("[Engine.PlayerPawn]") && widescreenmod == false)
            {
                DefUserIni.Insert(i + 1, "DesiredFOV=130.000000");
                DefUserIni.Insert(i + 1, "DefaultFOV=130.000000");
            }
        }

        //https://github.com/alexstrout/foxWSFix-UT99/releases/download/v1.1.1/foxWSFix99-v1.1.1.7z
        // Widescreen Patch
        if (widescreenmod)
        {
            Console.WriteLine("Downloading Widescreen Patch...");
            Console.Title = "Unreal99 Linux Installer - Downloading Widescreen Patch";
            var widescreenPatchUrl =
                "https://github.com/alexstrout/foxWSFix-UT99/releases/download/v1.1.1/foxWSFix99-v1.1.1.7z";
            var widescreenPatchDestination = Path.Combine(objDestination, "foxWSFix99-v1.1.1.7z");
            using (var client = new WebClient())
            {
                client.DownloadFile(widescreenPatchUrl, widescreenPatchDestination);
            }

            Console.WriteLine("Widescreen Patch Downloaded!");
            Console.WriteLine("Extracting Widescreen Patch...");
            Console.Title = "Unreal99 Linux Installer - Extracting Widescreen Patch";
            //Extract the Widescreen Patch to objDestination
            //7z x widescreenPatchDestination
            ProcessStartInfo startInfo3 = new ProcessStartInfo();
            startInfo3.FileName = "7z";
            startInfo3.Arguments = $"x \"{widescreenPatchDestination}\"";
            startInfo3.WorkingDirectory = objDestination;
            Process process3 = new Process();
            process3.StartInfo = startInfo3;
            process3.Start();
            process3.WaitForExit();
            // remove Src and foxWSFix99-v1.1.1.7z and foxWSFix Readme.txt
            Directory.Delete(Path.Combine(objDestination, "Src"), true);
            File.Delete(widescreenPatchDestination);
            File.Delete(Path.Combine(objDestination, "foxWSFix Readme.txt"));
        }

        // Write the changes to the file
        File.WriteAllLines(defUserFile, DefUserIni);
        Console.WriteLine("User.ini Customized!");
        
        // Offer to remove the original config files already on the system
        // but only if they exist
        var homeDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        var files = Path.Combine(homeDirectory, ".utpg", "System");

        if (remove)
        {
            Directory.Delete(files, true);
        }
        else
        {
            if (File.Exists(Path.Combine(files, "UnrealTournament.ini")))
            {
                Console.WriteLine("Would you like to remove the original config files?");
                Console.WriteLine("This will remove any customizations you have made.");
                Console.WriteLine("Y/N");
                var response = Console.ReadLine();
                if (response.ToLower() == "y")
                {
                    Console.WriteLine("Removing original config files...");
                    Console.Title = "Unreal99 Linux Installer - Removing Original Config Files";
                    Directory.Delete(files, true);
                    Console.WriteLine("Original Config Files Removed!");
                }
            }
        }
        Console.WriteLine("Installation Complete!");
        Console.WriteLine("You can now run Unreal Tournament 99 on Linux!");
        Console.ReadKey();
    }
}