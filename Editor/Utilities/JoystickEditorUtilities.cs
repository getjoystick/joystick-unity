using System.IO;
using System.Linq;
using JoystickRemoteConfig.Core.Data;
using UnityEditor;
using UnityEngine;

namespace JoystickRemoteConfig
{
    public static class JoystickEditorUtilities
    {
        private const string JoystickPackagePath = "Packages/com.getjoystick.joystick";

        public static string GetPackageDisplayName()
        {
            var packageInfo = UnityEditor.PackageManager.PackageInfo.FindForAssetPath(JoystickPackagePath);
           
            string displayName = packageInfo.displayName;
            return displayName;
        }

        public static string GetPackageVersion()
        {
           var packageInfo = UnityEditor.PackageManager.PackageInfo.FindForAssetPath(JoystickPackagePath);
           
           string version = packageInfo.version;
           return version;
        }
        
        public static void UpdateEnvironment(EnvironmentData[] environments)
        {
            string[] environmentArray = { };
            environmentArray = environments.Aggregate(environmentArray,
                (current, environment) => current.Append(environment.Name).ToArray());

            UpdateEnvironmentTypeClass(environmentArray);
        }

        private static void UpdateEnvironmentTypeClass(string[] environments)
        {
            var enumName = "EnvironmentType";
            var enumEntries = environments;
            
            var className = "EnvironmentType";
            var guids = AssetDatabase.FindAssets(className);
            if (guids.Length > 0)
            {
                // Get the first asset path from the list
                string assetPath = AssetDatabase.GUIDToAssetPath(guids[0]);
            
                // Check if the asset is a C# script file
                if (assetPath.EndsWith(".cs"))
                {
                    var filePathAndName = assetPath;
 
                    using (var streamWriter = new StreamWriter( filePathAndName ) )
                    {
                        streamWriter.WriteLine( "namespace JoystickRemoteConfig.Core.Data");
                        streamWriter.WriteLine( "{" );
                        streamWriter.WriteLine( "   public enum " + enumName );
                        streamWriter.WriteLine( "   {" );
                
                        foreach (var t in enumEntries)
                        {
                            streamWriter.WriteLine( "\t" + t + "," );
                        }
                
                        streamWriter.WriteLine( "   }" );
                        streamWriter.WriteLine( "}" );
                    }
                    AssetDatabase.Refresh();
                }
            }
            else
            {
                Debug.LogError("Could not find " + className + " class file");
            }
        }
    }
}