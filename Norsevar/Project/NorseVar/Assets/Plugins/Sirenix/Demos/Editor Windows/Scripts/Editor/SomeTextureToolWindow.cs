using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
namespace Sirenix.OdinInspector.Demos
{

    public class SomeTextureToolWindow : OdinEditorWindow
    {
        #region Serialized Fields

        [EnumToggleButtons]
        [BoxGroup( "Settings" )]
        public ScaleMode ScaleMode;

        [HorizontalGroup( 0.5f, PaddingRight = 5, LabelWidth = 70 )]
        public Texture[] Textures = new Texture[ 8 ];

        [ReadOnly]
        [HorizontalGroup]
        [InlineEditor( InlineEditorModes.LargePreview )]
        public Texture Preview;

        #endregion

        #region Properties

        [BoxGroup( "Settings" )]
        [FolderPath( RequireExistingPath = true )]
        public string OutputPath
        {
            // Use EditorPrefs to hold persisntent user-variables.
            get => EditorPrefs.GetString( "SomeTextureToolWindow.OutputPath" );
            set => EditorPrefs.SetString( "SomeTextureToolWindow.OutputPath", value );
        }

        #endregion

        #region Private Methods

        [MenuItem( "Tools/Odin Inspector/Demos/Odin Editor Window Demos/Some Texture Tool" )]
        private static void OpenWindow()
        {
            var window = GetWindow<SomeTextureToolWindow>();
            window.position = GUIHelper.GetEditorWindowRect().AlignCenter( 600, 600 );
            window.titleContent = new GUIContent( "Some Texture Tool Window" );
        }

        #endregion

        #region Public Methods

        [Button( ButtonSizes.Gigantic )] [GUIColor( 0, 1, 0 )]
        public void PerformSomeAction() { }

        #endregion
    }

}
#endif