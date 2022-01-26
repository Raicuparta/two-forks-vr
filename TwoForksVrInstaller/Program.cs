using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using AssetsTools.NET;
using AssetsTools.NET.Extra;

namespace TwoForksVRInstaller
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            try
            {
                var installerPath = Process.GetCurrentProcess().MainModule.FileName;
                var installerFileName = Path.GetFileName(installerPath);

                if (args.Length == 0)
                {
                    Console.WriteLine("Do not run this executable directly.");
                    Console.WriteLine($"You have to drag Firewatch.exe and drop it on top of {installerFileName}.");
                    Console.WriteLine($"Make sure to follow the instructions provided with the mod.");
                    return;
                }

                var gameExePath = args[0];

                if (!gameExePath.Contains("Firewatch.exe"))
                {
                    Console.WriteLine($"You have to drag Firewatch.exe and drop it on top of {installerFileName}.");
                    Console.WriteLine($"The file I got was {gameExePath}");
                    Console.WriteLine($"Make sure to follow the instructions provided with the mod.");
                    return;
                }

                var gamePath = Path.GetDirectoryName(gameExePath);
                var gameName = Path.GetFileNameWithoutExtension(gameExePath);
                var dataPath = Path.Combine(gamePath, $"{gameName}_Data/");
                var gameManagersPath = Path.Combine(dataPath, $"globalgamemanagers");
                var gameManagersBackupPath = CreateGameManagersBackup(gameManagersPath);
                var patcherPath = Path.GetDirectoryName(installerPath);
                var classDataPath = Path.Combine(patcherPath, "classdata.tpk");

                CopyModFiles(patcherPath, gamePath);
                PatchVR(gameManagersBackupPath, gameManagersPath, classDataPath);

                Console.WriteLine($"");
                Console.WriteLine("Installed successfully, probably.");
            }
            finally
            {
                Console.WriteLine($"");
                Console.WriteLine("Press any key to close this console.");
                Console.ReadKey();
            }
        }

        private static void CopyFilesRecursively(string sourcePath, string targetPath)
        {
            //Now Create all of the directories
            foreach (string dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
            {
                Directory.CreateDirectory(dirPath.Replace(sourcePath, targetPath));
            }

            //Copy all the files & Replaces any files with the same name
            foreach (string newPath in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories))
            {
                File.Copy(newPath, newPath.Replace(sourcePath, targetPath), true);
            }
        }

        private static void CopyModFiles(string patcherPath, string gamePath)
        {
            Console.WriteLine("Copying mod files...");

            var modFilesDirectoryPath = Path.Combine(patcherPath, "ModFiles");

            CopyFilesRecursively(modFilesDirectoryPath, gamePath);
        }

        private static string CreateGameManagersBackup(string gameManagersPath)
        {
            Console.WriteLine($"Backing up '{gameManagersPath}'...");
            var backupPath = gameManagersPath + ".bak";
            if (File.Exists(backupPath))
            {
                Console.WriteLine($"Backup already exists.");
                return backupPath;
            }
            File.Copy(gameManagersPath, backupPath);
            Console.WriteLine($"Created backup in '{backupPath}'");
            return backupPath;
        }

        private static void PatchVR(string gameManagersBackupPath, string gameManagersPath, string classDataPath)
        {
            Console.WriteLine("Patching globalgamemanagers...");
            Console.WriteLine($"Using classData file from path '{classDataPath}'");

            AssetsManager am = new AssetsManager();
            am.LoadClassPackage(classDataPath);
            AssetsFileInstance ggm = am.LoadAssetsFile(gameManagersBackupPath, false);
            AssetsFile ggmFile = ggm.file;
            AssetsFileTable ggmTable = ggm.table;
            am.LoadClassDatabaseFromPackage(ggmFile.typeTree.unityVersion);

            List<AssetsReplacer> replacers = new List<AssetsReplacer>();

            AssetFileInfoEx buildSettings = ggmTable.GetAssetInfo(11);
            AssetTypeValueField buildSettingsBase = am.GetATI(ggmFile, buildSettings).GetBaseField();
            AssetTypeValueField enabledVRDevices = buildSettingsBase.Get("enabledVRDevices").Get("Array");
            AssetTypeTemplateField stringTemplate = enabledVRDevices.templateField.children[1];
            AssetTypeValueField[] vrDevicesList = new AssetTypeValueField[] { StringField("OpenVR", stringTemplate) };
            enabledVRDevices.SetChildrenList(vrDevicesList);

            replacers.Add(new AssetsReplacerFromMemory(0, buildSettings.index, (int)buildSettings.curFileType, 0xffff, buildSettingsBase.WriteToByteArray()));

            using (AssetsFileWriter writer = new AssetsFileWriter(File.OpenWrite(gameManagersPath)))
            {
                ggmFile.Write(writer, 0, replacers, 0);
            }
        }

        private static AssetTypeValueField StringField(string str, AssetTypeTemplateField template)
        {
            return new AssetTypeValueField()
            {
                children = null,
                childrenCount = 0,
                templateField = template,
                value = new AssetTypeValue(EnumValueTypes.ValueType_String, str)
            };
        }
    }
}
