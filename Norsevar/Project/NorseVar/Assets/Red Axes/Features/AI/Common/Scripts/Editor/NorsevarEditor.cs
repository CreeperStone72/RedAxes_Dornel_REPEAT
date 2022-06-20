using System.Linq;
using Sirenix.OdinInspector.Demos.RPGEditor;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace Norsevar.AI.Editor
{

    public class NorsevarEditor : OdinMenuEditorWindow
    {

        #region Private Methods

        [MenuItem("Norsevar/Enemies")]
        private static void OpenWindow()
        {
            GetWindow<NorsevarEditor>().Show();
        }

        #endregion

        #region Protected Methods

        protected override OdinMenuTree BuildMenuTree()
        {
            OdinMenuTree tree = new(false)
            {
                DefaultMenuStyle =
                {
                    IconSize = 28.00f
                },
                Config =
                {
                    DrawSearchToolbar = true
                }
            };

            tree.AddAllAssetsAtPath("AI/Enemies", "Assets/Red Axes/", typeof(Enemy), true, true);
            tree.AddAllAssetsAtPath("AI/Spawners", "Assets/Red Axes/", typeof(Spawner.Spawner), true, true);
            tree.AddAllAssetsAtPath("Level/Scenes", "Assets/Red Axes/", typeof(GameScene), true, true);
            tree.AddAllAssetsAtPath("Level/Layout", "Assets/Red Axes/", typeof(GameLayout), true, true);
            tree.AddAllAssetsAtPath("Level/Level", "Assets/Red Axes/", typeof(GameLevel), true, true);

            tree.EnumerateTree().AddIcons<Enemy>(pX => pX.Icon);

            return tree;
        }

        protected override void OnBeginDrawEditors()
        {
            OdinMenuItem selected = MenuTree.Selection.FirstOrDefault();
            int toolbarHeight = MenuTree.Config.SearchToolbarHeight;

            SirenixEditorGUI.BeginHorizontalToolbar(toolbarHeight);
            {
                if (selected != null) GUILayout.Label(selected.Name);

                if (SirenixEditorGUI.ToolbarButton(new GUIContent("Create Enemy")))
                {
                    ScriptableObjectCreator.ShowDialog<Enemy>(
                        "Assets/Red Axes/Features/AI/Common/Data/Enemies",
                        pObj =>
                        {
                            pObj.Name = pObj.name;
                            TrySelectMenuItemWithObject(pObj);
                        });
                }

                if (SirenixEditorGUI.ToolbarButton(new GUIContent("Create Attack Type")))
                {
                    ScriptableObjectCreator.ShowDialog<AttackType>(
                        "Assets/Red Axes/Features/AI/Common/Data/AttackType",
                        TrySelectMenuItemWithObject);
                }
                if (SirenixEditorGUI.ToolbarButton(new GUIContent("Create States")))
                {
                    ScriptableObjectCreator.ShowDialog<AIStates>(
                        "Assets/Red Axes/Features/AI/Common/Data/States",
                        TrySelectMenuItemWithObject);
                }

                if (SirenixEditorGUI.ToolbarButton(new GUIContent("Create Spawner")))
                {
                    ScriptableObjectCreator.ShowDialog<Spawner.Spawner>(
                        "Assets/Red Axes/Features/AI/Common/Data/Spawners",
                        pObj =>
                        {
                            pObj.Name = pObj.name;
                            TrySelectMenuItemWithObject(pObj);
                        });
                }
            }
            SirenixEditorGUI.EndHorizontalToolbar();
        }

        protected override void OnEndDrawEditors()
        {
            OdinMenuTreeSelection selection = MenuTree?.Selection;

            SirenixEditorGUI.BeginHorizontalToolbar();
            {
                GUILayout.FlexibleSpace();

                if (SirenixEditorGUI.ToolbarButton("Delete Current"))
                {
                    if (selection != null)
                    {
                        ScriptableObject asset = selection.SelectedValue as ScriptableObject;
                        string assetPath = AssetDatabase.GetAssetPath(asset);
                        AssetDatabase.DeleteAsset(assetPath);
                    }

                    AssetDatabase.SaveAssets();
                }
            }
            SirenixEditorGUI.EndHorizontalToolbar();
        }

        #endregion

    }

}
