using System;
using System.IO;
using JoystickRemoteConfig.Core;
using JoystickRemoteConfig.Core.Data;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using Object = UnityEngine.Object;

namespace JoystickRemoteConfig.Editor
{
    public class JoystickEditorSetupWindow : EditorWindow
    {
        private TabbedMenuController _tabbedMenuController;
        private JoystickGeneralDefinition _joystickGeneralDefinition;
        
        private GroupBox _environmentsSetupGroupBox;

        private HelpBox _updateEnvironmentsHelpBox;

        [MenuItem("Joystick/Setup Window")]
        public static void ShowWindow()
        {
            JoystickEditorSetupWindow wnd = GetWindow<JoystickEditorSetupWindow>();
            wnd.titleContent = new GUIContent("Joystick Setup Window");
        }

        public void CreateGUI()
        {
            _joystickGeneralDefinition = GetJoystickGeneralDefinition();

            VisualElement root = rootVisualElement;
            var rootPath = GetJoystickGeneralDefinitionPath();
            // Import UXML
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(rootPath + "/JoystickEditorSetupWindow.uxml");
            TemplateContainer treeAsset = visualTree.CloneTree();
            rootVisualElement.Add(treeAsset);

            var styleSheet =
                AssetDatabase.LoadAssetAtPath<StyleSheet>(rootPath + "/JoystickEditorSetupWindow.uss");
            root.styleSheets.Add(styleSheet);

            _tabbedMenuController = new(root);

            _tabbedMenuController.RegisterTabCallbacks();
            
            _environmentsSetupGroupBox = rootVisualElement.Q<GroupBox>("EnvironmentSetup");
            _updateEnvironmentsHelpBox = new HelpBox();
            
            CreateEnvironmentConfigView();
            SetupRequestDataAtStartView();
            SetupGenericConfigView();
            SetupInformationView();
        }

        private void CreateEnvironmentConfigView()
        {
            SetupDropdownMenuWithEnvironment();
            
            var environmentsConfigDefinition = GetEnvironmentDefinition();

            var serializedObject = new SerializedObject(environmentsConfigDefinition);
            var serializedProperty = serializedObject.GetIterator();
            serializedProperty.Next(true);

            while (serializedProperty.NextVisible(false))
            {
                if (serializedProperty.name is "m_Script" or "EnvironmentType")
                    continue;

                var propertyField = new PropertyField(serializedProperty)
                {
                    style =
                    {
                        marginLeft = 10
                    }
                };

                propertyField.Bind(serializedObject);
                _environmentsSetupGroupBox.Add(propertyField);
            }

            var updateEnvironmentButton = rootVisualElement.Q<Button>("UpdateEnvironmentButton");

            updateEnvironmentButton.clickable.clicked += () =>
            {
                SetupDropdownMenuWithEnvironment();
                EditorUtility.SetDirty(environmentsConfigDefinition);
            };
            
        }
        
        private void SetupDropdownMenuWithEnvironment()
        {
            var environmentsConfigDefinition = GetEnvironmentDefinition();
            var environments = environmentsConfigDefinition.environments;
            var environmentsDropdown = rootVisualElement.Q<DropdownField>("DropdownEnvironmentType");
            
            environmentsDropdown.value = environmentsConfigDefinition.selectedEnvironment.Name;
            //check if environments names are unique
            for (int i = 0; i < environments.Length; i++)
            {
                for (int j = 0; j < environments.Length; j++)
                {
                    if (i == j) continue;
                    
                    if (environments[i].Name == environments[j].Name)
                    {
                        ShowHelpBox(_updateEnvironmentsHelpBox, rootVisualElement.Q<VisualElement>("UpdateEnvironmentHelpBoxView"), "Environment names must be unique, check environment name: " + environments[i].Name + " for duplicates.", HelpBoxMessageType.Error);
                        JoystickLogger.LogError("Environment names must be unique, check environment name: " + environments[i].Name + " for duplicates.");
                        return;
                    }
                }
            }
            
            _updateEnvironmentsHelpBox.RemoveFromHierarchy();

            environmentsDropdown.choices.Clear();
            foreach (var environmentData in environments)
            {
                environmentsDropdown.choices.Add(environmentData.Name);
            }

            environmentsDropdown.RegisterValueChangedCallback(evt =>
            {
                foreach (var environmentData in environments)
                {
                    if (environmentData.Name != evt.newValue.ToString()) continue;
                    
                    environmentsConfigDefinition.selectedEnvironment = environmentData;
                    break;
                }
                
                EditorUtility.SetDirty(environmentsConfigDefinition);
            });
        }

        private void ShowHelpBox(HelpBox helpBox, VisualElement visualElement, string text, HelpBoxMessageType helpBoxMessageType)
        {
            helpBox.messageType = helpBoxMessageType;
            helpBox.text = text;

            visualElement.Add(helpBox);
        }

        private void SetupRequestDataAtStartView()
        {
            var requestDataAtStartField = rootVisualElement.Q<Toggle>("ToggleRequestDataAtStart");
            var requestContentAtStartView = rootVisualElement.Q<GroupBox>("RequestContentAtStartView");

            var globalExtendedRequestData = GetGlobalExtendedRequestDefinition();
            RequestContentGlobalExtendedRequestData(globalExtendedRequestData);
            
            requestContentAtStartView.visible = _joystickGeneralDefinition.IsRequestContentAtStartEnabled;

            requestDataAtStartField.value = _joystickGeneralDefinition.IsRequestContentAtStartEnabled;
            requestDataAtStartField.RegisterValueChangedCallback(evt =>
            {
                requestContentAtStartView.visible = evt.newValue;
                _joystickGeneralDefinition.IsRequestContentAtStartEnabled = evt.newValue;
            });
            
            var propertyField = rootVisualElement.Q<PropertyField>("RequestContentConfig");
            propertyField.label = "Request Content Config";

            var targetObjectField = new ObjectField("Request Content Config:")
            {
                objectType = typeof(RequestContentDefinition),
                allowSceneObjects = false,
                value = _joystickGeneralDefinition.RequestContentDefinitionAtStart
            };

            targetObjectField.RegisterValueChangedCallback(evt =>
            {
                _joystickGeneralDefinition.RequestContentDefinitionAtStart = (RequestContentDefinition)evt.newValue;

                //RequestContentConfigInfo(evt.newValue);
                EditorUtility.SetDirty(_joystickGeneralDefinition);
            });
            
            propertyField.Add(targetObjectField);
            //RequestContentConfigInfo(targetObjectField.value);
        }
        

        private void RequestContentGlobalExtendedRequestData(Object targetObjectField)
        {
            var extendedRequestConfigView = rootVisualElement.Q<GroupBox>("GlobalExtendedRequestDataView");
            if (targetObjectField == null)
            {
                extendedRequestConfigView.Clear();
            }
            else
            {
                SerializedObject serializedObject = new(targetObjectField);
                var serializedProperty = serializedObject.GetIterator();
                serializedProperty.Next(true);

                while (serializedProperty.NextVisible(false))
                {
                    if (serializedProperty.name is "m_Script" or "GlobalExtendedRequestDefinition")
                        continue;

                    var requestConfigPropertyField = new PropertyField(serializedProperty)
                    {
                        style =
                        {
                            marginLeft = 10
                        }
                    };

                    requestConfigPropertyField.Bind(serializedObject);

                    extendedRequestConfigView.Add(requestConfigPropertyField);
                }
            }
        }

        private void SetupGenericConfigView()
        {
            var toggleSerializedResponseField = rootVisualElement.Q<Toggle>("ToggleSerializedResponse");

            toggleSerializedResponseField.value = _joystickGeneralDefinition.IsSerializedResponseEnabled;
            toggleSerializedResponseField.RegisterValueChangedCallback(evt =>
            {
                _joystickGeneralDefinition.IsSerializedResponseEnabled = evt.newValue;
                EditorUtility.SetDirty(_joystickGeneralDefinition);
            });
            
            var toggleDebugModeField = rootVisualElement.Q<Toggle>("ToggleDebugMode");
            
            toggleDebugModeField.value = _joystickGeneralDefinition.IsDebugModeEnabled;
            toggleDebugModeField.RegisterValueChangedCallback(evt =>
            {
                _joystickGeneralDefinition.IsDebugModeEnabled = evt.newValue;
                EditorUtility.SetDirty(_joystickGeneralDefinition);
            });
        }

        private void SetupInformationView()
        {
            var websiteButton = rootVisualElement.Q<Button>("WebsiteButton");
            websiteButton.clickable.clicked += () => { Application.OpenURL("https://www.getjoystick.com/"); };

            var documentationButton = rootVisualElement.Q<Button>("DocumentationButton");
            documentationButton.clickable.clicked += () => { Application.OpenURL("https://docs.getjoystick.com/"); };

            var checkForUpdatesButton = rootVisualElement.Q<Button>("CheckForUpdatesButton");
            checkForUpdatesButton.clickable.clicked += () => { Debug.Log("ToDo: Check for updates"); };
            
            var labelVersion = rootVisualElement.Q<Label>("LabelVersion");
            labelVersion.text = "Version: " + JoystickEditorUtilities.GetPackageVersion();
        }
        
        private GlobalExtendedRequestDefinition GetGlobalExtendedRequestDefinition()
        {
            var guids = AssetDatabase.FindAssets("t:GlobalExtendedRequestDefinition");

            if (guids.Length <= 0)
            {
                var globalExtendedRequestDefinition = CreateInstance<GlobalExtendedRequestDefinition>();
                var configDirectoryPath = TryCreateJoystickDefinitionsResourcesFolder();

                AssetDatabase.CreateAsset(globalExtendedRequestDefinition,
                    $"{configDirectoryPath}/GlobalExtendedRequestDefinition.asset");
                return globalExtendedRequestDefinition;
            }

            var path = AssetDatabase.GUIDToAssetPath(guids[0]);
            return AssetDatabase.LoadAssetAtPath<GlobalExtendedRequestDefinition>(path);
        }

        private EnvironmentsDataDefinition GetEnvironmentDefinition()
        {
            var guids = AssetDatabase.FindAssets("t:EnvironmentsDataDefinition");

            if (guids.Length <= 0)
            {
                var environmentsConfigDefinition = CreateInstance<EnvironmentsDataDefinition>();
                var configDirectoryPath = TryCreateJoystickDefinitionsResourcesFolder();

                AssetDatabase.CreateAsset(environmentsConfigDefinition,
                    $"{configDirectoryPath}/EnvironmentsDataDefinition.asset");
                return environmentsConfigDefinition;
            }

            var path = AssetDatabase.GUIDToAssetPath(guids[0]);
            return AssetDatabase.LoadAssetAtPath<EnvironmentsDataDefinition>(path);
        }

        private JoystickGeneralDefinition GetJoystickGeneralDefinition()
        {
            var guids = AssetDatabase.FindAssets("t:JoystickGeneralDefinition");

            if (guids.Length <= 0)
            {
                var joystickGeneralConfig = CreateInstance<JoystickGeneralDefinition>();
                var configDirectoryPath = TryCreateJoystickDefinitionsResourcesFolder();
                AssetDatabase.CreateAsset(joystickGeneralConfig, $"{configDirectoryPath}/JoystickGeneralDefinition.asset");
                return joystickGeneralConfig;
            }

            var path = AssetDatabase.GUIDToAssetPath(guids[0]);
            return AssetDatabase.LoadAssetAtPath<JoystickGeneralDefinition>(path);
        }
        
        private string GetJoystickGeneralDefinitionPath()
        {
            var guids = AssetDatabase.FindAssets("JoystickEditorSetupWindow");
            string assetPath = null;

            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);

                if (!path.EndsWith("JoystickEditorSetupWindow.cs")) continue;
                assetPath = path.Replace("/JoystickEditorSetupWindow.cs", "");
                break;
            }

            return assetPath;
        }

        private string TryCreateJoystickDefinitionsResourcesFolder()
        {
            var configDirectoryPath = $"Assets/{JoystickEditorUtilities.GetPackageDisplayName()}/Joystick Definitions/Resources";

            if (!Directory.Exists(configDirectoryPath))
            {
                Directory.CreateDirectory(configDirectoryPath);
            }

            return configDirectoryPath;
        }
    }
}