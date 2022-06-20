using System.IO;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Norsevar.Interaction.DialogueSystem.Editor
{

    public class DSEditorWindow : EditorWindow
    {

        #region Constants and Statics

        private const string DEFAULT_FILE_NAME = "DialogueFileName";
        private static TextField _fileNameTextField;

        #endregion

        #region Private Fields

        private Button _saveButton;
        private Button _miniMapButton;
        private DSGraphView _graphView;

        #endregion

        #region Unity Methods

        private void CreateGUI()
        {
            AddGraphView();
            AddToolBar();
            AddStyles();
        }

        #endregion

        #region Private Methods

        private void AddGraphView()
        {
            _graphView = new DSGraphView(this);

            _graphView.StretchToParentSize();

            rootVisualElement.Add(_graphView);
        }

        private void AddStyles()
        {
            rootVisualElement.AddStyleSheets("Dialogue System/DSVariables.uss");
        }

        private void AddToolBar()
        {
            Toolbar toolbar = new();

            _fileNameTextField = DSElementUtility.CreateTextField(
                DEFAULT_FILE_NAME,
                "File Name:",
                evt =>
                {
                    _fileNameTextField.value = evt.newValue.RemoveWhitespaces().RemoveSpecialCharacters();
                });

            _saveButton = DSElementUtility.CreateButton("Save", Save);

            Button loadButton = DSElementUtility.CreateButton("Load", Load);
            Button clearButton = DSElementUtility.CreateButton("Clear", Clear);
            Button resetButton = DSElementUtility.CreateButton("Reset", ResetGraph);
            _miniMapButton = DSElementUtility.CreateButton("MiniMap", ToggleMiniMap);

            toolbar.Add(_fileNameTextField);
            toolbar.Add(_saveButton);
            toolbar.Add(loadButton);
            toolbar.Add(clearButton);
            toolbar.Add(resetButton);
            toolbar.Add(_miniMapButton);

            toolbar.AddStyleSheets("Dialogue System/DSToolbarStyles.uss");

            rootVisualElement.Add(toolbar);
        }

        private void Clear()
        {
            _graphView.ClearGraph();
        }

        private void Load()
        {
            string filepath = EditorUtility.OpenFilePanel("Dialogue Graphs", $"{DSIOUtility.EditorFolderPath}/Graphs", "asset");
            if (string.IsNullOrEmpty(filepath)) return;

            Clear();
            DSIOUtility.Initialize(_graphView, Path.GetFileNameWithoutExtension(filepath));
            DSIOUtility.Load();
        }

        private void ResetGraph()
        {
            Clear();
            UpdateFileName(DEFAULT_FILE_NAME);
        }

        private void Save()
        {
            if (string.IsNullOrEmpty(_fileNameTextField.value))
            {
                EditorUtility.DisplayDialog("Invalid File Name", "File name is null or empty.", "OK");
                return;
            }
            DSIOUtility.Initialize(_graphView, _fileNameTextField.value);
            DSIOUtility.Save();
        }

        private void ToggleMiniMap()
        {
            _graphView.ToggleMiniMap();
            _miniMapButton.ToggleInClassList("ds-toolbar_button_selected");
        }

        #endregion

        #region Public Methods

        public void DisableSaving()
        {
            _saveButton.SetEnabled(false);
        }

        public void EnableSaving()
        {
            _saveButton.SetEnabled(true);
        }

        [MenuItem("Norsevar/Dialogue")]
        public static void Open()
        {
            GetWindow<DSEditorWindow>("Dialogue Graph");
        }

        public static void UpdateFileName(string newFileName)
        {
            _fileNameTextField.value = newFileName;
        }

        #endregion

    }

}
