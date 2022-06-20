using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace Norsevar
{

    [CreateAssetMenu(fileName = "New Game Layout", menuName = "Norsevar/Game/Layout")]
    public class GameLayout : ScriptableObject
    {

        #region Private Fields

        private int _currentRoom;

        #endregion

        #region Serialized Fields

        [SerializeField] [BoxGroup("Level Info")]
        private GameScene menu;
        [SerializeField] [BoxGroup("Level Info")]
        private GameScene death;
        [SerializeField] [BoxGroup("Level Info")]
        private GameScene loading;
        [SerializeField] [BoxGroup("Level Info")]
        private GameScene[] levels;
        [SerializeField] [BoxGroup("Level Info")]
        private GameScene player;
        [SerializeField] [BoxGroup("Level Info")]
        private GameScene roomSelection;
        [FormerlySerializedAs("levels")]
        [SerializeField] [BoxGroup("Level Info")]
        private List<GameScene> rooms;

        #endregion

        #region Private Methods

        private static void LoadScene(GameScene gameScene)
        {
            SceneManager.LoadSceneAsync(gameScene.Name);
        }

        private static void LoadSceneAdditive(GameScene gameScene)
        {
            SceneManager.LoadSceneAsync(gameScene.Name, LoadSceneMode.Additive);
        }

        private static void UnloadScene(GameScene gameScene)
        {
            SceneManager.UnloadSceneAsync(gameScene.Name);
        }

        #endregion

        #region Public Methods

        public void ClearedLevel()
        {
            UnloadScene(rooms[_currentRoom]);
            LoadSceneAdditive(roomSelection);
        }

        public string GetCurrentLoadingSceneName()
        {
            return rooms[_currentRoom].Name;
        }

        public string GetLoadingSceneName()
        {
            return loading.Name;
        }

        public string GetRoomSelectionSceneName()
        {
            return roomSelection.Name;
        }

        public void LoadDeath()
        {
            LoadScene(death);
        }

        public void LoadMenu()
        {
            Analytics.WriteToFile();
            LoadScene(menu);
        }

        public void LoadNextRoom(int type)
        {
            _currentRoom = type;
            UnloadScene(roomSelection);
            LoadSceneAdditive(rooms[_currentRoom]);
        }

        public void LoadNextLevel()
        {
            UnloadScene(levels[0]);
            LoadSceneAdditive(levels[1]);
            LoadSceneAdditive(roomSelection);
        }

        public void NewGame()
        {
            Time.timeScale = 0;

            LoadScene(loading);
            LoadSceneAdditive(levels[0]);
            LoadSceneAdditive(player);

            _currentRoom = 0;
        }

        #endregion

        [Serializable]
        public enum LifecycleStage
        {
            PreAlpha,
            Alpha,
            Beta,
            ReleaseCandidate,
            StableRelease,
            EndOfLife
        }

        public static void Quit()
        {
            Analytics.WriteToFile();
            Application.Quit();
        }
    }

}
