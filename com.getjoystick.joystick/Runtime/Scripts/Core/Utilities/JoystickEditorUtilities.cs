namespace JoystickRemote.Core
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
    }
}