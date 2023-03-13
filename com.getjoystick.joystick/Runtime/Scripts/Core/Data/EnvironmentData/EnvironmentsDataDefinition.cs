using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace JoystickRemote.Core.Data
{
    public class EnvironmentsDataDefinition : ScriptableObject
    {
        public EnvironmentData[] environments;
        [HideInInspector] public EnvironmentType environmentType;
        
        private bool _updateEnvironment;

        public void UpdateEnvironment()
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
                        streamWriter.WriteLine( "namespace JoystickRemote.Core.Data");
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