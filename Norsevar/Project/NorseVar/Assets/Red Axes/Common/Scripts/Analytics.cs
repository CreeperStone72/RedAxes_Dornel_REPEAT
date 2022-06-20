using System;
using System.Globalization;
using System.IO;
using UnityEngine;

namespace Norsevar
{
    public static class Analytics
    {

        #region Constants and Statics

        private static int _roomsCleared;
        private static int _enemiesKilled;
        private static int _deaths;
        private static int _upgradeCount;
        private static int _poisonedCount;
        private static int _upgradeRoomsEncountered;
        private static int _merchantRoomEncountered;
        private static int _groundSlamsUsed;
        private static int _dashesUsed;
        private static int _rounds;
        private static int _fightRoomsEncountered;
        private static int _chargeAttack;
        private static int _attack;
        private static int _crowsFound;
        private static float _startTime;
        private static float _fightTime;
        private static int _critAmount;
        private static float _maxDamage;
        private static int _moneyCollected;

        private static float _avgFPS;

        #endregion

        #region Private Methods

        private static string CombineMessage(string s, int value)
        {
            return $"{s}: {value.ToString()}";
        }

        private static string CombineMessage(string s, float value)
        {
            return $"{s}: {value.ToString(CultureInfo.CurrentCulture)}";
        }

        #endregion

        #region Public Methods

        public static void AddAttack()
        {
            _attack++;
        }

        public static void AddChargeAttack()
        {
            _chargeAttack++;
        }

        public static void AddCrit()
        {
            _critAmount++;
        }

        public static void AddCrowFound()
        {
            _crowsFound++;
        }

        public static void AddDash()
        {
            _dashesUsed++;
        }

        public static void AddEnemyKilled()
        {
            _enemiesKilled++;
        }

        public static void AddFightRoom()
        {
            _fightRoomsEncountered++;
        }

        public static void AddGroundSlam()
        {
            _groundSlamsUsed++;
        }

        public static void AddMerchantRoom()
        {
            _merchantRoomEncountered++;
        }

        public static void AddMoney(int amount)
        {
            _moneyCollected += amount;
        }

        public static void AddPlayerKilled()
        {
            _deaths++;
        }

        public static void AddPoisoned()
        {
            _poisonedCount++;
        }

        public static void AddRoomCleared()
        {
            _roomsCleared++;
        }

        public static void AddRoundPlayed()
        {
            _rounds++;
        }

        public static void AddToFightTime()
        {
            float time = Time.time - _startTime;
            _fightTime += time;
        }

        public static void AddUpgradeCollected()
        {
            _upgradeCount++;
        }

        public static void AddUpgradeRoom()
        {
            _upgradeRoomsEncountered++;
        }

        public static void InitStartTime()
        {
            _startTime = Time.time;
        }

        public static void UpdateMaxDamage(float value)
        {
            if (value > _maxDamage)
                _maxDamage = value;
        }

        public static void WriteToFile()
        {
            string path = Path.Combine(Application.dataPath, "..", "data.txt");

            // Debug.Log(path);

            StreamWriter writer = new(path, false);

            DateTime dateTime = DateTime.UtcNow;

            writer.WriteLine(dateTime.ToString(CultureInfo.CurrentCulture));
            writer.WriteLine("Norsevar");
            writer.WriteLine();
            writer.WriteLine("----------Rooms Encountered----------");
            writer.WriteLine(CombineMessage("Fight rooms encountered", _fightRoomsEncountered));
            writer.WriteLine(CombineMessage("Merchant rooms encountered", _merchantRoomEncountered));
            writer.WriteLine(CombineMessage("Upgrade rooms encountered", _upgradeRoomsEncountered));
            writer.WriteLine(CombineMessage("Rooms cleared", _roomsCleared));
            writer.WriteLine();
            writer.WriteLine("---------------Attacks---------------");
            writer.WriteLine(CombineMessage("Ground slams used", _groundSlamsUsed));
            writer.WriteLine(CombineMessage("Dashes used", _dashesUsed));
            writer.WriteLine(CombineMessage("Enemies killed", _enemiesKilled));
            writer.WriteLine(CombineMessage("Charge attack used", _chargeAttack));
            writer.WriteLine(CombineMessage("Attack used", _attack));
            writer.WriteLine(CombineMessage("Crit made", _critAmount));
            writer.WriteLine(CombineMessage("Max damage made", _maxDamage));
            writer.WriteLine();
            writer.WriteLine("---------------Player----------------");
            writer.WriteLine(CombineMessage("Death count", _deaths));
            writer.WriteLine(CombineMessage("Poisoned count", _poisonedCount));
            writer.WriteLine(CombineMessage("Upgrades collected", _upgradeCount));
            writer.WriteLine(CombineMessage("Money collected", _moneyCollected));
            writer.WriteLine();
            writer.WriteLine("----------------Game----------------");
            writer.WriteLine(CombineMessage("Rounds played", _rounds));
            float fightTime = _fightTime / _roomsCleared;
            writer.WriteLine(CombineMessage("Avg time per fight", fightTime));
            writer.WriteLine();
            writer.WriteLine("----------------Extra---------------");
            writer.WriteLine(CombineMessage("Crows hit", _crowsFound));

            writer.Close();
        }

        #endregion

    }
}
