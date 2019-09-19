// C# example.
using UnityEditor;
using System.Diagnostics;

public class AllPlatforms
{
    public static void Build()
    {
        // Get filename.
        string path = EditorUtility.SaveFolderPanel("Choose build output folder", "", "");

        Windows.BuildGame(path + "/Windows");
        Linux.BuildGame(path + "/Linux");
        Mac.BuildGame(path + "/Mac");

        // Copy a file from the project folder to the build folder, alongside the built game.
        //FileUtil.CopyFileOrDirectory("Assets/Templates/Readme.txt", path + "Readme.txt");

        // Run the game (Process class from System.Diagnostics).
        //Process proc = new Process();
        //proc.StartInfo.FileName = path + "/Screeps3D.exe";
        //proc.Start();
    }
}