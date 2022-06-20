using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Norsevar
{

    [CreateAssetMenu(fileName = "New Game Level", menuName = "Norsevar/Game/Level")]
    public class GameLevel : ScriptableObject
    {

        #region Serialized Fields

        [AssetSelector] [SerializeField] [BoxGroup("Level Info")]
        private GameScene loadingScene;

        [AssetSelector] [SerializeField] [BoxGroup("Level Info")]
        private List<GameScene> scenes;

        #endregion

        #region Public Methods

        public void LoadScenes()
        {
            SceneManager.LoadSceneAsync(loadingScene.Name);

            foreach (GameScene scene in scenes)
                SceneManager.LoadSceneAsync(scene.Name, LoadSceneMode.Additive);
        }

        public void UnLoadScenes()
        {
            foreach (GameScene scene in scenes)
                SceneManager.UnloadSceneAsync(scene.Name);
        }

        #endregion

    }

}
