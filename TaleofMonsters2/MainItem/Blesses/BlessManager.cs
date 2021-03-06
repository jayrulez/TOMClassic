﻿using System;
using System.Collections.Generic;
using System.Drawing;
using ConfigDatas;
using NarlonLib.Math;
using TaleofMonsters.DataType.Blesses;
using TaleofMonsters.DataType.User;

namespace TaleofMonsters.MainItem.Blesses
{
    internal static class BlessManager
    {
        internal delegate void BlessUpdateMethod();

        private static BlessConfig cache;

        public static BlessUpdateMethod Update = null;


        public static void OnChangeMap()
        {
            RebuildCache();
        }

        public static void AddBless(int id, int time)
        {
            if (UserProfile.InfoWorld.Blesses.Count >= 10) //最大10个bless
                return;
            UserProfile.InfoWorld.Blesses[id] = time;
            if (Update != null)
            {
                Update();
            }
            RebuildCache();
        }

        public static void RemoveBless(int id)
        {
            UserProfile.InfoWorld.Blesses.Remove(id);
            if (Update != null)
            {
                Update();
            }
            RebuildCache();
        }

        public static List<int> GetNegtiveBless()
        {
            List<int> datas = new List<int>();
            foreach (var bless in UserProfile.InfoWorld.Blesses)
            {
                var blessConfig = ConfigData.GetBlessConfig(bless.Key);
                if (blessConfig.Type == 2)
                {
                    datas.Add(blessConfig.Id);
                }
            }
            return datas;
        }

        private static void RebuildCache()
        {
            cache = new BlessConfig();
            foreach (var key in UserProfile.InfoWorld.Blesses.Keys)
            {
                var config = ConfigData.GetBlessConfig(key);
                cache.MoveFoodChange += config.MoveFoodChange;
                cache.PunishFoodMulti += config.PunishFoodMulti;
                cache.PunishGoldMulti += config.PunishGoldMulti;
                cache.PunishHealthMulti += config.PunishHealthMulti;
                cache.PunishMentalMulti += config.PunishMentalMulti;
                cache.RewardExpMulti += config.RewardExpMulti;
                cache.RewardFoodMulti += config.RewardFoodMulti;
                cache.RewardGoldMulti += config.RewardGoldMulti;
                cache.RewardHealthMulti += config.RewardHealthMulti;
                cache.RewardMentalMulti += config.RewardMentalMulti;
                cache.RollWinAddGold += config.RollWinAddGold;
                cache.RollFailSubHealth += config.RollFailSubHealth;
                cache.FightLevelChange += config.FightLevelChange;
                cache.FightWinAddExp += config.FightWinAddExp;
                cache.FightWinAddHealth += config.FightWinAddHealth;
                cache.FightFailSubHealth += config.FightFailSubHealth;
                cache.FightFailSubMental += config.FightFailSubMental;
            }
        }

        public static void OnMove()
        {
            Dictionary<int, int> replace = new Dictionary<int, int>();
            int count = UserProfile.InfoWorld.Blesses.Count;
            foreach (var bless in UserProfile.InfoWorld.Blesses)
            {
                if (bless.Value > 1)
                    replace[bless.Key] = bless.Value-1;
            }
            UserProfile.InfoWorld.Blesses = replace;
            if (count != UserProfile.InfoWorld.Blesses.Count)
            {
                if (Update != null)
                {
                    Update();
                }
            }
        }

        public static int SceneMove
        {
            get { return cache.MoveFoodChange; }
        }

        public static int PunishFoodMulti
        {
            get { return cache.PunishFoodMulti; }
        }
        public static int PunishGoldMulti
        {
            get { return cache.PunishGoldMulti; }
        }
        public static int PunishHealthMulti
        {
            get { return cache.PunishHealthMulti; }
        }
        public static int PunishMentalMulti
        {
            get { return cache.PunishMentalMulti; }
        }
        public static int RewardExpMulti
        {
            get { return cache.RewardExpMulti; }
        }
        public static int RewardFoodMulti
        {
            get { return cache.RewardFoodMulti; }
        }
        public static int RewardGoldMulti
        {
            get { return cache.RewardGoldMulti; }
        }
        public static int RewardHealthMulti
        {
            get { return cache.RewardHealthMulti; }
        }
        public static int RewardMentalMulti
        {
            get { return cache.RewardMentalMulti; }
        }
        public static int RollWinAddGold
        {
            get { return cache.RollWinAddGold; }
        }
        public static int RollFailSubHealth
        {
            get { return cache.RollFailSubHealth; }
        }
        public static int FightLevelChange
        {
            get { return cache.FightLevelChange; }
        }

        public static int FightWinAddExp
        {
            get { return cache.FightWinAddExp; }
        }
        public static int FightWinAddHealth
        {
            get { return cache.FightWinAddHealth; }
        }
        public static int FightFailSubHealth
        {
            get { return cache.FightFailSubHealth; }
        }
        public static int FightFailSubMental
        {
            get { return cache.FightFailSubMental; }
        }

    }
}
