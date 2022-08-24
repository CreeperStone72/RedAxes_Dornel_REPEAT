using System;
using Norsevar.MusicAndSFX;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace Norsevar._REPEAT.Scripts {
    [CreateAssetMenu(fileName = "Run variables", menuName = "Norsevar/Game/Run variables", order = 0)]
    public class RunVariables : ScriptableObject {
        public string currentScene;
        public int deathCount;

        public void OnRespawn(Scene newScene, Action setHubMusic, UnityEvent onFirstRespawn, UnityEvent onSubsequentRespawn) {
            currentScene = newScene.name;
            deathCount++;
            
            if (deathCount == 1) {
                setHubMusic();
                onFirstRespawn.Invoke();
            }
            else onSubsequentRespawn.Invoke();
        }

        private void OnValidate() { if (deathCount < 0) deathCount = 0; }
    }
}