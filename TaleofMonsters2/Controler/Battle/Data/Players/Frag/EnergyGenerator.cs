﻿using System;
using System.Collections.Generic;
using ConfigDatas;
using NarlonLib.Math;
using TaleofMonsters.Core;

namespace TaleofMonsters.Controler.Battle.Data.Players.Frag
{
    internal class EnergyGenerator
    {
        public PlayerManaTypes NextAimMana { get { return manaList[0]; } }

        public List<PlayerManaTypes> QueuedMana { get { return manaList.GetRange(1,4); } }

        private List<PlayerManaTypes> manaList = new List<PlayerManaTypes>();

        public int RateLp { get; private set; }
        public int RatePp { get; private set; }
        public int RateMp { get; private set; }

        public int LimitLp { get; set; } //极限值
        public int LimitPp { get; set; }
        public int LimitMp { get; set; }

        /// <summary>
        /// 仅仅npc使用，玩家不用
        /// </summary>
        public void SetRateNpc(PeopleConfig peopleConfig)
        {
            JobConfig jobConfig = ConfigData.GetJobConfig(peopleConfig.Job);
            RateLp = jobConfig.EnergyRate[0];
            RatePp = jobConfig.EnergyRate[1];
            RateMp = jobConfig.EnergyRate[2];
            LimitLp = jobConfig.EnergyLimit[0];
            LimitPp = jobConfig.EnergyLimit[1];
            LimitMp = jobConfig.EnergyLimit[2];
        }
        
        public void SetRate(int[] rateChange, int jobId)
        {
            JobConfig jobConfig = ConfigData.GetJobConfig(jobId);
            RateLp = jobConfig.EnergyRate[0] + rateChange[0];
            RatePp = jobConfig.EnergyRate[1] + rateChange[1];
            RateMp = jobConfig.EnergyRate[2] + rateChange[2];
            LimitLp = jobConfig.EnergyLimit[0];
            LimitPp = jobConfig.EnergyLimit[1];
            LimitMp = jobConfig.EnergyLimit[2];

            int total = RateLp + RatePp + RateMp;
            if (total > 0 && total != 100)
            {
                RateLp = Math.Max(0, RateLp/total*100);
                RatePp = Math.Max(0,RatePp / total * 100);
                RateMp = 100 - RateLp - RatePp;
            }
        }

        public void Next(int round)
        {
            while (manaList.Count < 5)
            {
                if (round > 0 && (round % GameConstants.RoundRecoverAllRound) == 0)
                {
                    manaList.Add(PlayerManaTypes.All);
                    continue;
                }

                var roll = MathTool.GetRandom(100);
                if (roll < RateMp)
                {
                    manaList.Add(PlayerManaTypes.Mp);
                    continue;
                }
                if (roll < RateMp + RatePp)
                {
                    manaList.Add(PlayerManaTypes.Pp);
                    continue;
                }
                manaList.Add(PlayerManaTypes.Lp);
            }
        }

        public void UseMana()
        {
            manaList.RemoveAt(0);
        }
    }
}
