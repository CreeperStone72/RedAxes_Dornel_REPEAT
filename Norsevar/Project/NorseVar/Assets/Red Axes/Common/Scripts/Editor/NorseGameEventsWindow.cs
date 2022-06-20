using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Norsevar.VFX;
using Sirenix.OdinInspector.Demos.RPGEditor;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Norsevar.Editor
{
    public class NorseGameEventsWindow : OdinMenuEditorWindow
    {
        #region Private Fields

        private NorseGameEventBindingsSO _norseGameEventBindings;

        #endregion

        #region Properties

        private static string EnumFilePath => Application.dataPath + "/Red Axes/Common/Generated/GeneratedEventEnum.cs";

        private static string RelativeEnumFilePath => "Assets/Red Axes/Common/Generated/GeneratedEventEnum.cs";

        private NorseGameEventBindingsSO NorseGameEventBindings
        {
            get
            {
                if (_norseGameEventBindings == null)
                    _norseGameEventBindings = Resources.Load<NorseGameEventBindingsSO>("Data/NorseGameEventBindings");

                return _norseGameEventBindings;
            }
        }

        #endregion

        #region Private Methods

        private void GenerateEnums()
        {
            string basePath = Application.dataPath + "/Red Axes/Resources/Data/Events";

            //GET all files
            string[] files = Directory.GetFiles(basePath, "*.asset", SearchOption.AllDirectories);

            EnumData enumData = new();

            int index = 0;

            foreach (string file in files)
            {
                string fileSubPath = Path.GetRelativePath(basePath, file);

                fileSubPath = fileSubPath.Replace('\\', '/').Replace(".asset", "");

                string eventName = Path.GetFileNameWithoutExtension(file).RemoveWhitespaces();

                enumData.filePaths.Add(fileSubPath);

                if (fileSubPath.Contains('/'))
                {
                    eventName = fileSubPath.Replace('/', '_').RemoveWhitespaces();
                }

                enumData.entries.Add(eventName);
                enumData.identifiers.Add(Random.Range(int.MinValue, int.MaxValue));
            }

            //If an Enum File Exists
            if (File.Exists(EnumFilePath))
            {
                //Fetch existing data
                string enumFileText = File.ReadAllText(EnumFilePath);
                //Seperate into header, body and footer
                string header = String.Empty, body = String.Empty, footer = String.Empty;

                SeperateEnumFile(enumFileText, out header, out body, out footer);

                //Sanitize file
                string bodyClean = RemoveAllEscapeCharactersAndWhiteSpaces(body);

                //Get enums in file
                List<string> existingData = BodyToEnums(bodyClean);

                //Get diffs
                List<string> removedEnums = existingData.Except(enumData.entries).ToList();
                List<string> addedEnums = enumData.entries.Except(existingData).ToList();

                List<int> removedIdentifiers = GetIdentifiersFromEnumNames(body, removedEnums);
                List<int> keptIndices = new List<int>();

                foreach (var addedEnum in addedEnums)
                {
                    keptIndices.Add(enumData.entries.IndexOf(addedEnum));
                }

                for (int i = enumData.entries.Count - 1; i >= 0; i--)
                {
                    if (!keptIndices.Contains(i))
                    {
                        enumData.entries.RemoveAt(i);
                        enumData.identifiers.RemoveAt(i);
                        enumData.filePaths.RemoveAt(i);
                    }
                }

                //Apply changes to string
                var newBody = ApplyEnumChangesToBody(body, enumData, removedEnums, true);

                string newEnumFile = header + newBody + footer;

                File.WriteAllText(EnumFilePath, newEnumFile);

                ApplyChangesToEventBindings(enumData, removedIdentifiers);
            }
            else
            {
                //File doesn't exist yet
                string header = "namespace Norsevar {\npublic enum ENorseGameEvent\n{";
                string footer = "\n}\n}";

                string body = "";

                body = ApplyEnumChangesToBody(body, enumData, null);

                string newEnumFile = header + body + footer;

                File.WriteAllText(EnumFilePath, newEnumFile);

                NorseGameEventBindings.Clear();
                ApplyChangesToEventBindings(enumData, null);
            }

            EditorUtility.SetDirty(NorseGameEventBindings);
            AssetDatabase.Refresh();
        }

        private List<int> GetIdentifiersFromEnumNames(string body, List<string> removedEnums)
        {
            List<int> res = new List<int>();

            foreach (var removedEnum in removedEnums)
            {
                res.Add(GetIdentifierFromEnumName(body, removedEnum));
            }

            return res;
        }

        private int GetIdentifierFromEnumName(string body, string removedEnum)
        {
            int start = body.IndexOf(" = ", body.IndexOf(removedEnum)) + 3;
            int end = body.IndexOf(',', start);
            string val = body.Substring(start, end - start);
            return Int32.Parse(val);
        }

        private void ApplyChangesToEventBindings(EnumData enumData, List<int> removedEntries)
        {
            if (removedEntries != null)
                foreach (var removedEntry in removedEntries)
                    NorseGameEventBindings.Remove(removedEntry);


            string basePath = Application.dataPath + "/Red Axes/Resources/Data/Events";

            for (int i = 0; i < enumData.entries.Count; i++)
            {
                NorseGameEvent gameEvent =
                    Resources.Load<NorseGameEvent>("Data/Events/" + enumData.filePaths[i]);
                NorseGameEventBindings.Add(enumData.identifiers[i], gameEvent);
            }
        }

        private List<string> BodyToEnums(string body)
        {
            List<string> enums = body.Split(",").ToList();
            enums.RemoveAt(enums.Count - 1);
            var enumsSanitized = enums.Select(s => s.Substring(0, s.IndexOf('=')));
            return enumsSanitized.ToList();
        }

        private void SeperateEnumFile(string input, out string header, out string body, out string footer)
        {
            //We skip until we find the start of the Enum
            int enumStart = input.IndexOf("ENorseGameEvent", StringComparison.Ordinal);
            //From there, we skip until first Bracket
            enumStart = input.IndexOf('{', enumStart) + 1;

            //Header
            header = input.Substring(0, enumStart);

            //End of enums
            int enumEnd = input.IndexOf('}', enumStart);
            int enumLength = enumEnd - enumStart;

            //Body
            body = input.Substring(enumStart, enumLength);

            //Footer
            footer = input.Substring(enumEnd);
        }

        private string ApplyEnumChangesToBody(string body, EnumData addedEntries, List<string> removedEntries, bool addToExisting = false)
        {
            var res = body;
            //Remove
            if (removedEntries != null)
            {
                foreach (var entry in removedEntries)
                {
                    int start = res.IndexOf(entry);
                    int end = res.IndexOf(',', start);
                    //-2 for \n\t and +3 for the \n\t + ','
                    res = res.Remove(start - 2, end - start + 3);
                }
            }

            //Add
            for (int i = 0; i < addedEntries.entries.Count; i++)
            {
                if (addToExisting && i == 0)
                {
                    res = res.Substring(0, res.Length - 1);
                }
                res = res + "\n\t" + addedEntries.entries[i] + " = " + addedEntries.identifiers[i];
                res = res + ",";
            }

            if (addedEntries.entries.Count > 0 && addToExisting) res = res + '\n';

            return res;
        }

        private string RemoveAllEscapeCharactersAndWhiteSpaces(string s)
        {
            string res = string.Empty;

            foreach (char c in s)
            {
                if (c.IsWhitespace() || c == '\n' || c == '\t' || c == '\r') continue;
                res = res + c;
            }

            return res;
        }

        [MenuItem("Norsevar/Events")]
        private static void OpenWindow()
        {
            GetWindow<NorseGameEventsWindow>().Show();
        }

        #endregion

        #region Protected Methods

        protected override OdinMenuTree BuildMenuTree()
        {
            OdinMenuTree tree = new(false)
            {
                Config =
                {
                    DrawSearchToolbar = true
                }
            };

            tree.AddAllAssetsAtPath("Events", "Assets/Red Axes/Resources/Data/Events", typeof(NorseGameEvent), true);

            return tree;
        }

        protected override void OnBeginDrawEditors()
        {
            if (MenuTree == null) ForceMenuTreeRebuild();

            OdinMenuItem selected = MenuTree.Selection.FirstOrDefault();

            int toolbarHeight = MenuTree.Config.SearchToolbarHeight;

            SirenixEditorGUI.BeginHorizontalToolbar(toolbarHeight);
            {
                string path = "";

                if (selected != null)
                {
                    path = "Assets/Red Axes/Resources/Data/" + Path.GetDirectoryName(selected.GetFullPath());
                    if (path == string.Empty) path = "Assets/Red Axes/Resources/Data/Events";
                    GUILayout.Label(selected.Name);
                }

                if (SirenixEditorGUI.ToolbarButton(new GUIContent("Create Event")))
                    ScriptableObjectCreator.ShowDialog<NorseGameEvent>(path, TrySelectMenuItemWithObject);

                if (SirenixEditorGUI.ToolbarButton(new GUIContent("Generate Enums")))
                    GenerateEnums();
            }

            SirenixEditorGUI.EndHorizontalToolbar();
        }

        protected override void OnEndDrawEditors()
        {
            OdinMenuTreeSelection selected = MenuTree?.Selection;

            SirenixEditorGUI.BeginHorizontalToolbar();
            {
                if (SirenixEditorGUI.ToolbarButton(new GUIContent("Delete Generated Enums")))
                {
                    AssetDatabase.DeleteAsset(RelativeEnumFilePath);
                    NorseGameEventBindings.Clear();
                    AssetDatabase.SaveAssets();
                }

                if (SirenixEditorGUI.ToolbarButton(new GUIContent("Delete Selected Event")))
                {
                    if (selected is {SelectedValue: ScriptableObject so})
                        AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(so));
                    AssetDatabase.SaveAssets();
                }
            }

            SirenixEditorGUI.EndHorizontalToolbar();
        }

        #endregion

        private class EnumData
        {
            public EnumData()
            {
                entries = new List<string>();
                identifiers = new List<int>();
                filePaths = new List<string>();
            }

            public readonly List<string> filePaths;
            public readonly List<string> entries;
            public readonly List<int> identifiers;
        }
    }
}