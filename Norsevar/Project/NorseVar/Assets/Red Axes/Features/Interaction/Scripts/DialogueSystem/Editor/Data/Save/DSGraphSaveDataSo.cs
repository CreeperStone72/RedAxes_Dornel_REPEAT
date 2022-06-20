using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering;

namespace Norsevar.Interaction.DialogueSystem.Editor
{

    public class DSGraphSaveDataSo : SerializedScriptableObject
    {

        #region Serialized Fields

        [SerializeField] private string fileName;

        [SerializeField] private List<DSGroupSaveData> groups;

        [SerializeField] private List<DSNodeSaveData> nodes;

        [SerializeField] private List<string> oldGroupNames;

        [SerializeField] private List<string> oldNodeNames;

        [SerializeField] private SerializedDictionary<string, List<string>> oldGroupedNodeNames;

        #endregion

        #region Properties

        public string FileName => fileName;

        public List<DSGroupSaveData> Groups => groups;

        public List<DSNodeSaveData> Nodes => nodes;

        public List<string> OldGroupNames
        {
            get => oldGroupNames;
            set => oldGroupNames = value;
        }

        public List<string> OldNodeNames
        {
            get => oldNodeNames;
            set => oldNodeNames = value;
        }

        public SerializedDictionary<string, List<string>> OldGroupedNodeNames
        {
            get => oldGroupedNodeNames;
            set => oldGroupedNodeNames = value;
        }

        #endregion

        #region Public Methods

        public void Initialize(string pFileName)
        {
            fileName = pFileName;

            groups = new List<DSGroupSaveData>();
            nodes = new List<DSNodeSaveData>();

        }

        #endregion

    }

}
