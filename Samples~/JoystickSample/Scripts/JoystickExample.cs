// -----------------------------------------------------------------------------
//
// This example C# file can be used to quickly utilise usage of Joystick APIs
//
// -----------------------------------------------------------------------------

using System.IO;
using JoystickRemote;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;

public class JoystickExample : MonoBehaviour
{
    [SerializeField] private RequestContentConfig _config;
    [SerializeField] private Text _resultText;

    private void Start()
    {
        //Register fetching remote content complete event when toggle on the "Request Data at Start" option in Joystick editor window
        Joystick.OnAutoStartFetchContentCompleted += (isSucceed, result) =>
        {
            string formattedJson = FormatJson(result);

            _resultText.text = $"Auto start fetch: {isSucceed} \nResponse: \n{formattedJson}";
        };
    }

    public void OnFetchContentButtonClicked()
    {
        //Fetch the remote content and display the result in UI
        Joystick.FetchConfigContent(_config, (isSucceed, result) =>
        {
            string formattedJson = FormatJson(result);

            _resultText.text = isSucceed ? $"Succeed: \n{formattedJson}" : $"Failed: \n{formattedJson}";
        });
    }

    public void OnFetchCatalogButtonClicked()
    {
        //Fetch the catalog from remote and display the result in UI
        Joystick.FetchCatalogContent((isSucceed, result) =>
        {
            string formattedJson = FormatJson(result);

            _resultText.text = isSucceed ? $"Succeed: \n{formattedJson}" : $"Failed: \n{formattedJson}";
        });
    }

    /// <summary>
    /// Format Json string
    /// </summary>
    /// <param name="json"></param>
    /// <returns></returns>
    private string FormatJson(string json)
    {
        string formattedJson = string.Empty;
        JsonSerializer jsonSerializer = new JsonSerializer();
        TextReader textReader = new StringReader(json);
        JsonTextReader jsonTextReader = new JsonTextReader(textReader);
        object obj = jsonSerializer.Deserialize(jsonTextReader);
        
        if (obj != null)
        {
            StringWriter stringWriter = new StringWriter();
            
            JsonTextWriter jsonTextWriter = new JsonTextWriter(stringWriter)
            {
                Formatting = Formatting.Indented,
                Indentation = 4,
                IndentChar = ' '
            };
            
            jsonSerializer.Serialize(jsonTextWriter, obj);

            formattedJson = stringWriter.ToString();
        }

        return formattedJson;
    }
}