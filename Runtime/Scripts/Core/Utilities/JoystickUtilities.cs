using System.Collections.Generic;
using System.Text;
using JoystickRemote.Core.Data;
using UnityEngine;

namespace JoystickRemote.Core
{
    public static class JoystickUtilities
    {
        public static EnvironmentsDataDefinition GetEnvironmentsDataDefinition()
        {
            return Resources.Load<EnvironmentsDataDefinition>("EnvironmentsDataDefinition");
        }

        public static JoystickGeneralDefinition GetJoystickGeneralDefinition()
        {
            return Resources.Load<JoystickGeneralDefinition>("JoystickGeneralDefinition");
        }

        public static string GetConfigContentAPIUrl(List<ContentRequestSettings> configDataList)
        {
            StringBuilder stringBuilder = new StringBuilder();

            for (int i = 0; i < configDataList.Count; i++)
            {
                stringBuilder.Append("\"");
                stringBuilder.Append(configDataList[i].contentId);
                stringBuilder.Append("\"");

                if (i < configDataList.Count - 1)
                {
                    stringBuilder.Append(",");
                }
            }

            var generalConfig = GetJoystickGeneralDefinition();
            bool shouldSerialized = generalConfig.IsSerializedResponseEnabled;

            string responseTypeParam = "&responseType=serialized";
            string appendParam = shouldSerialized ? responseTypeParam : string.Empty;

            return $"https://api.getjoystick.com/api/v1/combine/?c=[{stringBuilder}]&dynamic=true{appendParam}";
        }

        public static string GetCatalogAPIUrl()
        {
            return $"https://api.getjoystick.com/api/v1/env/catalog";
        }
    }
}