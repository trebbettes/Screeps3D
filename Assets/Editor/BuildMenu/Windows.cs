// C# example.
using UnityEditor;
using System.Diagnostics;

public class Windows
{
    public static void BuildGame(string path = null)
    {
        if (string.IsNullOrEmpty(path))
        {
            // Get filename.
            path = EditorUtility.SaveFolderPanel("Choose Location of Built Game", "", "");
        }

        string[] levels = new string[] {
            "Assets/Scenes/Login.unity",
            "Assets/Scenes/Game.unity",
        };

        // Build player.
        BuildPipeline.BuildPlayer(levels, path + "/Screeps3D.exe", BuildTarget.StandaloneWindows, 
            BuildOptions.Development 
        );

        // Copy a file from the project folder to the build folder, alongside the built game.
        //FileUtil.CopyFileOrDirectory("Assets/Templates/Readme.txt", path + "Readme.txt");

        // Run the game (Process class from System.Diagnostics).
        //Process proc = new Process();
        //proc.StartInfo.FileName = path + "/Screeps3D.exe";
        //proc.Start();
    }
}