using System;
using System.Collections;
using Norsevar.MusicAndSFX;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace Norsevar.Dialogue_System
{
    public class DialogueVoiceLines : MonoBehaviour {
        [SerializeField] private string level1;
        [SerializeField] private string tagF1;
        [SerializeField] private string tagF2;
        [SerializeField] private string tagF3;
        [SerializeField] private Transform playerTransform;
        [SerializeField] private UnityEvent setAmbiance;
        [SerializeField] private UnityEvent setHubMusic;

        #region Private Methods

        private void PlayDialogue(ENorseGameEvent id) { StartCoroutine(PlaySound(id)); }

        private void PlayRandomVoiceLine(ENorseGameEvent[] voiceLines, float none = 0f)
        {
            var random = Random.Range(0f, 1f);

            if (random < none) return;

            var normalizedId = (random - none) / (1f - none);
            var lineId = Mathf.FloorToInt(normalizedId * voiceLines.Length);

            PlayVoiceLine(voiceLines[lineId]);
        }

        private IEnumerator PlaySound(ENorseGameEvent id) {
            NorseGame.Instance.RaiseEvent(id, transform.position);
            yield return null;
        }

        private void PlayVoiceLine(ENorseGameEvent id) { StartCoroutine(PlaySound(id)); }

        #region Resurrection dialogues
        
        #region Erik

        private IEnumerator Erik_Resurrection_01() {
            PlayDialogue(ENorseGameEvent.Dialogues_Erik_Resurrection_01_WhatThe);
            yield return new WaitForSeconds(6.4f);
        }

        private IEnumerator Erik_Resurrection_02() {
            PlayDialogue(ENorseGameEvent.Dialogues_Erik_Resurrection_02_Loki);
            yield return new WaitForSeconds(6.78f);
        }

        private IEnumerator Erik_Resurrection_03() {
            PlayDialogue(ENorseGameEvent.Dialogues_Erik_Resurrection_03_Breath);
            yield return new WaitForSeconds(7.024f);
        }

        private IEnumerator Erik_Resurrection_04() {
            PlayDialogue(ENorseGameEvent.Dialogues_Erik_Resurrection_04_Thanks);
            yield return new WaitForSeconds(5.294f);
        }

        private IEnumerator Erik_Resurrection_05() {
            PlayDialogue(ENorseGameEvent.Dialogues_Erik_Resurrection_05_Valhalla);
            yield return new WaitForSeconds(5.015f);
        }
            
        #endregion
            
        #region Heimdall

        private IEnumerator Heimdall_Resurrection_01() {
            PlayDialogue(ENorseGameEvent.Dialogues_Heimdall_Resurrection_01_PrayTell);
            yield return new WaitForSeconds(2.742f);
        }

        private IEnumerator Heimdall_Resurrection_02() {
            PlayDialogue(ENorseGameEvent.Dialogues_Heimdall_Resurrection_02_IamHeimdall);
            yield return new WaitForSeconds(7.915f);
        }

        private IEnumerator Heimdall_Resurrection_03() {
            PlayDialogue(ENorseGameEvent.Dialogues_Heimdall_Resurrection_03_GodOfMischief);
            yield return new WaitForSeconds(8.306f);
        }

        private IEnumerator Heimdall_Resurrection_04() {
            PlayDialogue(ENorseGameEvent.Dialogues_Heimdall_Resurrection_04_TapestryOfTime);
            yield return new WaitForSeconds(5.773f);
        }

        private IEnumerator Heimdall_Resurrection_05() {
            PlayDialogue(ENorseGameEvent.Dialogues_Heimdall_Resurrection_05_LokiWontLeave);
            yield return new WaitForSeconds(7.758f);
        }
        
        #endregion

        #endregion

        private IEnumerator ResurrectionDialogue() {
            setAmbiance.Invoke();
            yield return StartCoroutine(Heimdall_Resurrection_01());
            yield return new WaitForSeconds(0.25f);
            yield return StartCoroutine(Erik_Resurrection_01());
            yield return new WaitForSeconds(0.25f);
            yield return StartCoroutine(Heimdall_Resurrection_02());
            yield return new WaitForSeconds(0.25f);
            yield return StartCoroutine(Erik_Resurrection_02());
            yield return new WaitForSeconds(0.25f);
            yield return StartCoroutine(Heimdall_Resurrection_03());
            yield return new WaitForSeconds(0.25f);
            yield return StartCoroutine(Erik_Resurrection_03());
            yield return new WaitForSeconds(0.25f);
            yield return StartCoroutine(Heimdall_Resurrection_04());
            yield return new WaitForSeconds(0.25f);
            yield return StartCoroutine(Erik_Resurrection_04());
            yield return new WaitForSeconds(0.25f);
            yield return StartCoroutine(Heimdall_Resurrection_05());
            yield return new WaitForSeconds(0.25f);
            yield return StartCoroutine(Erik_Resurrection_05());
            setHubMusic.Invoke();
        }
        
        #endregion

        #region Public Methods

        public void Resurrection() { StartCoroutine(ResurrectionDialogue()); }

        public void OnDeathVoiceLine()
        {
            var deathVoiceLines = new[]
            {
                ENorseGameEvent.Dialogues_Erik_Death_AintDoneYet,
                ENorseGameEvent.Dialogues_Erik_Death_NotLikeThis,
                ENorseGameEvent.Dialogues_Erik_Death_Odin,
                ENorseGameEvent.Dialogues_Erik_Death_AllYouGot
            };

            PlayRandomVoiceLine(deathVoiceLines);
        }

        public void OnEnterCombatVoiceLine()
        {
            var enterCombatVoiceLines = new[]
            {
                ENorseGameEvent.Dialogues_Erik_EnteringCombat_BringItOn,
                ENorseGameEvent.Dialogues_Erik_EnteringCombat_LetsDance,
                ENorseGameEvent.Dialogues_Erik_EnteringCombat_Odin,
                ENorseGameEvent.Dialogues_Erik_EnteringCombat_Tyr,
                ENorseGameEvent.Dialogues_Erik_EnteringCombat_Wulfing
            };

            if (SceneManager.GetActiveScene().name == level1) {
                GameObject f1 = GameObject.FindWithTag(tagF1), f2 = GameObject.FindWithTag(tagF2), f3 = GameObject.FindWithTag(tagF3);
                BoxCollider c1, c2, c3;

                if (f1 == null || f2 == null || f3 == null) return;
                
                if ((c3 = f3.GetComponent<BoxCollider>()) != null) {
                    if (c3.bounds.Contains(playerTransform.position)) { 
                        PlayVoiceLine(ENorseGameEvent.Dialogues_Erik_Level1_05_Village);
                        return;
                    }
                }
                
                if ((c2 = f2.GetComponent<BoxCollider>()) != null) {
                    if (c2.bounds.Contains(playerTransform.position)) {
                        PlayVoiceLine(ENorseGameEvent.Dialogues_Erik_Level1_03_WhyDidItHaveToBeSnakes);
                        return;
                    }
                }

                if ((c1 = f1.GetComponent<BoxCollider>()) != null) {
                    if (c1.bounds.Contains(playerTransform.position)) { 
                        PlayVoiceLine(ENorseGameEvent.Dialogues_Erik_Level1_01_WhatThe);
                        return;
                    }
                }
            }
            
            PlayRandomVoiceLine(enterCombatVoiceLines);
        }

        public void OnExitCombatVoiceLine()
        {
            var exitCombatVoiceLines = new[]
            {
                ENorseGameEvent.Dialogues_Erik_ExitingCombat_Allfather,
                ENorseGameEvent.Dialogues_Erik_ExitingCombat_AllYouGot,
                ENorseGameEvent.Dialogues_Erik_ExitingCombat_NoTimeToDie
            };

            if (SceneManager.GetActiveScene().name == level1) {
                GameObject f1 = GameObject.FindWithTag(tagF1), f2 = GameObject.FindWithTag(tagF2), f3 = GameObject.FindWithTag(tagF3);
                BoxCollider c1, c2, c3;

                if (f1 == null || f2 == null || f3 == null) return;
                
                if ((c1 = f1.GetComponent<BoxCollider>()) != null) {
                    if (c1.bounds.Contains(playerTransform.position)) { 
                        PlayVoiceLine(ENorseGameEvent.Dialogues_Erik_Level1_02_ThatsOdd);
                        return;
                    }
                }
                
                if ((c2 = f2.GetComponent<BoxCollider>()) != null) {
                    if (c2.bounds.Contains(playerTransform.position)) { 
                        PlayVoiceLine(ENorseGameEvent.Dialogues_Erik_Level1_04_NoSense);
                        return;
                    }
                }
                
                if ((c3 = f3.GetComponent<BoxCollider>()) != null) {
                    if (c3.bounds.Contains(playerTransform.position)) { 
                        PlayVoiceLine(ENorseGameEvent.Dialogues_Erik_Level1_06_WalkInThePark);
                        return;
                    }
                }
            }
            
            PlayRandomVoiceLine(exitCombatVoiceLines);
        }

        public void OnRespawnVoiceLine()
        {
            var respawnVoiceLines = new[]
            {
                ENorseGameEvent.Dialogues_Loki_PersistentOne,
                ENorseGameEvent.Dialogues_Loki_WontSaveYou,
                ENorseGameEvent.Dialogues_Loki_TryAgain,
                ENorseGameEvent.Dialogues_Loki_Laugh,
                ENorseGameEvent.Dialogues_Loki_DefinitionOfInsanity
            };

            PlayRandomVoiceLine(respawnVoiceLines);
        }

        #endregion

    }
}
