## Introduction

The Joystick Unity package provides an easy-to-use interface to fetch remote config content from the Joystick server. This package enables you to fetch content and catalog data from the server and utilize it in your Unity game.

## Installation
*Requires Unity 2019.2+*

To use the Joystick Unity package, you will need to have Unity installed on your computer. Once you have Unity installed, you can install the Joystick Unity package through the Unity Package Manager.
To use the Joystick package, you will need to have Unity installed on your computer. Once you have Unity installed, you can install the Joystick package through the Unity Package Manager.

Open your Unity project.
1. Open the Package Manager window by selecting Window > Package Manager from the Unity Editor menu.
1. Click on the "+" button in the top left corner of the Package Manager window.
1. Select "Add package from git URL".
1. Enter the following URL into the text field: https://github.com/getjoystick/joystick-unity.git
1. Press the "Add" button to add the package to your project.
1. Once the package is installed, you will be able to start using it in your project.

### Install via UPM (using Git URL)
1. Navigate to your project's Packages folder and open the manifest.json file.
2. Add this line below the `"dependencies": {` line
```json
"com.getjoystick.joystick": "https://github.com/getjoystick/joystick-unity.git",
```
3. UPM should now install Joystick-Unity and it's dependencies.

Don't want to use git? Just download and unzip the repository into the Packages folder.

### Install via OpenUPM

The package is available on the [openupm registry](https://openupm.com). It's recommended to install it via [openupm-cli](https://github.com/openupm/openupm-cli).

```
openupm add com.getjoystick.joystick
```

## Getting Started

To use the Joystick Unity package, you need to follow the below steps:

1. Import the Joystick Unity package into your Unity project.
1. After importing the package, you can access the Joystick class by using the following namespace: JoystickRemote.
1. The Joystick class contains the following static methods:
1. FetchConfigContent(RequestContentConfig definition, Action<bool, string> callback, bool getFreshContent = false)
1. FetchConfigContent(List<ContentConfigData> configList, Action<bool, string> callback, ExtendedRequestData extendedRequestData = null, bool getFreshContent = false)
1. FetchCatalogContent(Action<bool, string> callback, bool getFreshContent = false)
1. To fetch content, call the FetchConfigContent method, passing in the RequestContentConfig or List of ContentConfigData and the callback method. You can also set a boolean value to force updating the content when requesting.
1. To fetch the catalog content, call the FetchCatalogContent method, passing in the callback method. You can also set a boolean value to force updating the content when requesting.

Example Usage:

Fetching Config Content using RequestContentConfig

```C#
using JoystickRemote;
using UnityEngine;

 
public class FetchContent : MonoBehaviour
{
    public RequestContentConfig configDefinition;
    private void Start()
    {
        Joystick.FetchConfigContent(configDefinition, OnContentFetchComplete);
    }

    private void OnContentFetchComplete(bool success, string response)
    {
        if (success)
        {
            Debug.Log("Content Fetch Successful: " + response);
        }
        else
        {
            Debug.LogError("Content Fetch Failed: " + response);
        }
    }
}
```
Fetching Config Content using List of ContentConfigData

```C#
using JoystickRemote;
using UnityEngine;

public class FetchContent : MonoBehaviour
{
    public List<ContentConfigData> configList;
    private void Start()
    {
        Joystick.FetchConfigContent(configList, OnContentFetchComplete);
    }

    private void OnContentFetchComplete(bool success, string response)
    {
        if (success)
        {
            Debug.Log("Content Fetch Successful: " + response);
        }
        else
        {
            Debug.LogError("Content Fetch Failed: " + response);
        }
    }
}
```

Fetching Catalog Content
```C#
using JoystickRemote;
using UnityEngine;

public class FetchContent : MonoBehaviour
{
    private void Start()
    {
        Joystick.FetchCatalogContent(OnCatalogFetchComplete);
    }

    private void OnCatalogFetchComplete(bool success, string response)
    {
        if (success)
        {
            Debug.Log("Catalog Fetch Successful: " + response);
        }
        else
        {
            Debug.LogError("Catalog Fetch Failed: " + response);
        }
    }
}
```

Note: To use the Joystick Unity package, you need to have a Joystick account and have configured the remote content on the Joystick server. Also, make sure to check the "Request Data at Start" option in the Joystick editor window to trigger the OnAutoStartFetchContentCompleted event when the data fetching is complete.
