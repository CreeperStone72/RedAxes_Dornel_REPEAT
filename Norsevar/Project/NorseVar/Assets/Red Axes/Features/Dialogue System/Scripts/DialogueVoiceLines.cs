using System.Collections;
using UnityEngine;

namespace Norsevar.Dialogue_System
{
    public class DialogueVoiceLines : MonoBehaviour
    {

        #region Private Methods

        private void PlayDialogue(ENorseGameEvent id)
        {
            StartCoroutine(PlaySound(id));
        }

        private void PlayRandomVoiceLine(ENorseGameEvent[] voiceLines, float none = 0f)
        {
            var random = Random.Range(0f, 1f);

            if (random < none) return;

            var normalizedId = (random - none) / (1f - none);
            var lineId = Mathf.FloorToInt(normalizedId * voiceLines.Length);

            PlayVoiceLine(voiceLines[lineId]);
        }

        private IEnumerator PlaySound(ENorseGameEvent id)
        {
            NorseGame.Instance.RaiseEvent(id, transform.position);
            yield return null;
        }

        private void PlayVoiceLine(ENorseGameEvent id)
        {
            StartCoroutine(PlaySound(id));
        }

        #endregion

        #region Public Methods

        public void Erik_Resurrection_01()
        {
            PlayDialogue(ENorseGameEvent.Dialogues_Erik_Resurrection_01_WhatThe);
        }

        public void Erik_Resurrection_02()
        {
            PlayDialogue(ENorseGameEvent.Dialogues_Erik_Resurrection_02_Loki);
        }

        public void Erik_Resurrection_03()
        {
            PlayDialogue(ENorseGameEvent.Dialogues_Erik_Resurrection_03_Breath);
        }

        public void Erik_Resurrection_04()
        {
            PlayDialogue(ENorseGameEvent.Dialogues_Erik_Resurrection_04_Thanks);
        }

        public void Erik_Resurrection_05()
        {
            PlayDialogue(ENorseGameEvent.Dialogues_Erik_Resurrection_05_Valhalla);
        }

        public void Heimdall_IamHeimdall()
        {
            PlayDialogue(ENorseGameEvent.Dialogues_Heimdall_Resurrection_02_IamHeimdall);
        }

        public void Heimdall_LokiWontLeave()
        {
            PlayDialogue(ENorseGameEvent.Dialogues_Heimdall_Resurrection_05_LokiWontLeave);
        }

        public void Heimdall_PrayTell()
        {
            PlayDialogue(ENorseGameEvent.Dialogues_Heimdall_Resurrection_01_PrayTell);
        }

        public void Heimdall_TapestryOfTime()
        {
            PlayDialogue(ENorseGameEvent.Dialogues_Heimdall_Resurrection_04_TapestryOfTime);
        }

        public void Heimdall_TheGodOfMischief()
        {
            PlayDialogue(ENorseGameEvent.Dialogues_Heimdall_Resurrection_03_GodOfMischief);
        }

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
