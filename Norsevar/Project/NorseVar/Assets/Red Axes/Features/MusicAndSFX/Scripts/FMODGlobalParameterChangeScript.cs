using FMODUnity;
using UnityEngine;

namespace Norsevar.MusicAndSFX
{
    public class FMODGlobalParameterChangeScript : MonoBehaviour
    {

        #region Constants and Statics

        private static int _enemyCount;
        private static float _maxHealth, _currentHealth, _currentIntensity, _musicIndex;

        #endregion

        #region Unity Methods

        private void Start()
        {
            _currentHealth = _maxHealth;
            _currentIntensity = _musicIndex = _enemyCount  = 0;
        }

        private void Update()
        {
            RuntimeManager.StudioSystem.setParameterByName("ShopOrUpgrade", _musicIndex);
            RuntimeManager.StudioSystem.setParameterByName("EnemyCount", _enemyCount);
            RuntimeManager.StudioSystem.setParameterByName("Intensity", _currentIntensity);
            RuntimeManager.StudioSystem.setParameterByName("Health", GetHealthPercent());
        }
        #endregion

        #region Private Methods

        private static float GetHealthPercent()
        {
            return _currentHealth / _maxHealth * 100 % 100;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Changes FMOD global index of the shop/upgrade dual track to be the right music
        /// Alongside switching the music to the shop/upgrade music
        /// </summary>
        public static void ActivateRewardMusic(MusicEnums track)
        {
            switch (track)
            {
                case MusicEnums.Shop:
                    _musicIndex = 0f;
                    FMODEventEmittersTogglerScript.ChangeMusic(MusicEnums.Shop);
                    break;
                case MusicEnums.Upgrade:
                    _musicIndex = 2f;
                    FMODEventEmittersTogglerScript.ChangeMusic(MusicEnums.Shop);
                    break;
                case MusicEnums.NeitherSOrH:
                    _musicIndex = 1f;
                    FMODEventEmittersTogglerScript.ChangeMusic(MusicEnums.Ambiance);
                    break;
            }
        }

        /// <summary>
        ///     Takes in the volume as a percentage of 100, eg. 75.
        ///     Will change so player health is 75% of max health
        /// </summary>
        public static void ChangeHealthByPercentage(float healthPercent)
        {
            _currentHealth = _maxHealth * (healthPercent / 100) % 100;
        }

        public static void AddEnemy()
        {
            if(_enemyCount <= 0)
                FMODEventEmittersTogglerScript.ChangeMusic(MusicEnums.Combat);
            if(_enemyCount < 15)
                _enemyCount++;
        }

        public static void RemoveEnemy()
        {
            if(_enemyCount > 1)
                _enemyCount--;
            else
                ClearEnemies();
        }

        /// <summary>
        /// Ups the enemy count FMOD parameter to 15, as it is the highest intensity
        /// also turns on boss music
        /// </summary>
        public static void BossEncounter()
        {
            _enemyCount = 15;
            FMODEventEmittersTogglerScript.ChangeMusic(MusicEnums.Boss);
        }

        /// <summary>
        /// Resets the enemy count FMOD parameter to 0 & sets ambience to play
        /// Should be called when defeating a mini boss / reg boss, or leaving an area
        /// </summary>
        public static void ClearEnemies()
        {
            _enemyCount = 0;

            if (FMODEventEmittersTogglerScript.IsInHub())
                FMODEventEmittersTogglerScript.ChangeMusic(MusicEnums.Hub);
            else
                FMODEventEmittersTogglerScript.ChangeMusic(MusicEnums.Ambiance);
        }

        /// <summary>
        /// Changes the current health that controls the global FMOD parameter by adding to it
        /// </summary>
        /// <param name="healthRegen"></param>
        public static void Heal(float healthRegen)
        {
            _currentHealth += healthRegen;
            if (_currentHealth > _maxHealth)
                _currentHealth = _maxHealth;
        }

        /// <summary>
        /// Can update the current health, which then uses the max health value to get a % with controls the FMOD
        /// parameter for health
        /// </summary>
        /// <param name="health"></param>
        public static void SetCurrentHealth(float health)
        {
            _currentHealth = health;

            if (_currentHealth > _maxHealth)
                _currentHealth = _maxHealth;
            else if (_currentHealth < 0)
                _currentHealth = 0;

            _currentIntensity = -1 * (_currentHealth - _maxHealth) / 100;
        }

        /// <summary>
        /// Updates the maximum health the player can have for use with calculations of % values for FMOD parameters
        /// </summary>
        /// <param name="health"></param>
        public static void SetMaxHealth(float health)
        {
            _maxHealth = health;
        }

        #endregion

    }
}
