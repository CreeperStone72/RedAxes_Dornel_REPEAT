using System;
using UnityEditor;

#if UNITY_EDITOR
namespace Sirenix.OdinInspector.Demos
{

    [HideLabel]
    [Serializable]
    public class SomeData
    {
        #region Serialized Fields

        [MultiLineProperty] [Title( "Basic Odin Menu Editor Window",
            "Inherit from OdinMenuEditorWindow, and build your menu tree" )]
        public string Test1 =
            "This value is persistent cross reloads, but will reset once you restart Unity or close the window.";

        #endregion

        #region Properties

        [MultiLineProperty] [ShowInInspector]
        private string Test3
        {
            get => EditorPrefs.GetString( "OdinDemo.PersistentString",
                "This value is persistent forever, even cross Unity projects. But it's not saved together " +
                "with your project. That's where ScriptableObejcts and OdinEditorWindows come in handy." );
            set => EditorPrefs.SetString( "OdinDemo.PersistentString", value );
        }

        #endregion

        [MultiLineProperty] [ShowInInspector] [NonSerialized]
        public string Test2 =
            "This value is not persistent cross reloads, and will reset once you hit play or recompile.";
    }

}
#endif