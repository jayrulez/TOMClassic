﻿using System.Drawing;
using TaleofMonsters.Controler.Battle.Data;
using TaleofMonsters.Controler.Battle.Data.MemMap;
using TaleofMonsters.Controler.Battle.Data.MemMonster;
using TaleofMonsters.Controler.Battle.DataTent;
using TaleofMonsters.Core;
using TaleofMonsters.DataType;

namespace TaleofMonsters.Controler.Battle.Tool
{
    internal class BattleManager
    {
        static BattleManager instance;
        public static BattleManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new BattleManager();
                }
                return instance;
            }
        }

        public EffectQueue EffectQueue;
        public FlowWordQueue FlowWordQueue;
        public MonsterQueue MonsterQueue;
        public BattleInfo BattleInfo;
        public PlayerManager PlayerManager;
        public MissileQueue MissileQueue;
        public MemRowColumnMap MemMap;

        public bool IsNight;
        public int RoundMark;//目前一个roundmark代表0.05s
        public float Round;//当前的回合数，超过固定值就可以抽牌

        public BattleManager()
        {
            Init();
        }

        public void Init()
        {
            EffectQueue = new EffectQueue();
            FlowWordQueue = new FlowWordQueue();
            MonsterQueue = new MonsterQueue();
            BattleInfo = new BattleInfo();
            PlayerManager = new PlayerManager();
            MissileQueue = new MissileQueue();
            IsNight = false;
        }

        public void Next()
        {
            RoundMark++;

            FlowWordQueue.Next();
            EffectQueue.Next();
            MissileQueue.Next();

            if (RoundMark % 4 == 0)
            {
                MonsterQueue.NextAction(); //1回合
            }

            if (RoundMark % 4 == 0) //200ms
            {
                float pastTime = (float)200 / GameConstants.RoundTime;
                PlayerManager.Update(false, pastTime, BattleInfo.Round);

                Round += pastTime;
                if (Round >= 1)
                {
                    Round = 0;
                    PlayerManager.CheckRoundCard(); //1回合
                }
            }
            BattleInfo.Round = RoundMark * 50 / GameConstants.RoundTime + 1;//50ms
            if (RoundMark % 10 == 0)
            {
                AIStrategy.AIProc(PlayerManager.RightPlayer);
            }
        }

        public void Draw(Graphics g, MagicRegion magicRegion, int mouseX, int mouseY, bool isMouseIn)
        {
            MemMap.Draw(g);

            if (magicRegion.Type != RegionTypes.None && isMouseIn)
                magicRegion.Draw(g, RoundMark, mouseX, mouseY);
            for (int i = 0; i < MonsterQueue.Count; i++)
            {
                LiveMonster monster = MonsterQueue[i];
                Color color = Color.White;
                if (isMouseIn)
                    color = magicRegion.GetColor(monster, mouseX, mouseY);
                monster.DrawOnBattle(g, color);
            }

            for (int i = 0; i < MissileQueue.Count; i++)
                MissileQueue[i].Draw(g);

            for (int i = 0; i < EffectQueue.Count; i++)
                EffectQueue[i].Draw(g);
            for (int i = 0; i < FlowWordQueue.Count; i++)
                FlowWordQueue[i].Draw(g);

        }
    }
}