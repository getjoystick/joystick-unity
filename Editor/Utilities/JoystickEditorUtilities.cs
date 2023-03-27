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
            
        }
    }
}