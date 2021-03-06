﻿using System.Collections.Generic;
using ConfigDatas;
using TaleofMonsters.Controler.Battle.Data.MemMonster;
using TaleofMonsters.Controler.Battle.Data.Players;
using TaleofMonsters.Controler.Battle.Tool;
using TaleofMonsters.DataType;
using TaleofMonsters.DataType.Others;

namespace TaleofMonsters.Controler.Battle.DataTent
{
    internal class MonsterQueue
    {
        private List<LiveMonster> monsters = new List<LiveMonster>();
        private List<LiveMonster> toAdd = new List<LiveMonster>();
        public int LeftCount { get; private set; }
        public int RightCount { get; private set; }

        public List<LiveMonster> Enumerator
        {
            get { return monsters; }
        }

        public LiveMonster this[int index]
        {
            get { return monsters[index]; }
            set { monsters[index] = value; }
        }

        public int Count
        {
            get { return monsters.Count; }
        }


        public void Add(LiveMonster mon)
        {
            mon.OwnerPlayer.State.CheckMonsterEvent(true, mon);
            monsters.Add(mon);
            BattleLocationManager.UpdateCellOwner(mon.Position.X, mon.Position.Y, mon.Id);
            mon.OnInit();
            if (mon.IsLeft)
            {
                LeftCount++;
            }
            else
            {
                RightCount++;
            }
        }

        public void AddDelay(LiveMonster mon)
        {
            toAdd.Add(mon);
        }

        private void Remove(LiveMonster mon)
        {
            mon.OwnerPlayer.State.CheckMonsterEvent(false, mon);
            if (BattleManager.Instance.MemMap.GetMouseCell(mon.Position.X, mon.Position.Y).Owner == -mon.Id)
                BattleLocationManager.ClearCellOwner(mon.Position.X, mon.Position.Y);
            monsters.Remove(mon);                                
        }

        private void Remove(int monid)
        {
            foreach (LiveMonster monster in monsters)
            {
                if (monster.Id == monid)
                {
                    Remove(monster);
                    return;
                }
            }
        }

        public void RemoveDirect(int monid)
        {
            Remove(monid);
        }

        public LiveMonster GetMonsterByUniqueId(int id)
        {
            for (int i = 0; i < monsters.Count; i++)
            {
                if (monsters[i].Id == System.Math.Abs(id))
                {
                    return monsters[i];
                }
            }
            return null;
        }

        public LiveMonster GetKingTower(bool isLeft)
        {
            for (int i = 0; i < monsters.Count; i++)
            {
                var lm = monsters[i];
                if (lm.IsLeft == isLeft && lm.Type == (int)CardTypeSub.Hero)
                {
                    return lm;
                }
            }
            return null;
        }

        public void NextAction(float pastRound)
        {
            LeftCount = 0;
            RightCount = 0;
            List<int> removeMids = new List<int>();            
            foreach (var mon in monsters)
            {
                var rival = mon.Rival as Player;
                if (rival.DirectDamage > 0)
                {
                    HitDamage damage = new HitDamage(rival.DirectDamage, rival.DirectDamage, 0, DamageTypes.Magic);
                    mon.HpBar.OnDamage(damage);
                }
                if (!mon.IsAlive)
                {
                    if (!mon.IsGhost)
                    {
                        mon.OnDie();
                    }
                    else
                    {
                        if (mon.OwnerPlayer.SpikeManager.HasSpike("grave") || (mon.Rival as Player).SpikeManager.HasSpike("grave"))
                            mon.GhostTime+=0.005f;
                        else
                            mon.GhostTime+=0.01f;
                        if (mon.GhostTime>=1)
                        {
                            removeMids.Add(mon.Id);
                        }
                    }
                }
                else
                {
                    if (!mon.IsLeft) RightCount = RightCount + 1;
                    else LeftCount = LeftCount + 1;
                }
            }
            BattleManager.Instance.PlayerManager.LeftPlayer.DirectDamage = 0;//伤害清除
            BattleManager.Instance.PlayerManager.RightPlayer.DirectDamage = 0;

            foreach (int mid in removeMids)
            {
                Remove(mid);
            }


            foreach (var roundMonster in monsters)
            {
                if (roundMonster.IsGhost) 
                    continue;

                int tile = BattleManager.Instance.MemMap.GetMouseCell(roundMonster.Position.X, roundMonster.Position.Y).Tile;
                var match = TileBook.IsTileMatch(tile,roundMonster.Avatar.MonsterConfig.Attr);
                roundMonster.Next(pastRound, match);
            }

            foreach (var lm in toAdd)//添加延时怪
            {
                Add(lm);
            }
            toAdd.Clear();
        }

        public void OnPlayerUseCard(IPlayer caster, int cardType, int lv)
        {
            foreach (var mon in monsters)
            {
                mon.SkillManager.CheckUseCard(caster, cardType, lv);
            }
        }

        public void Clear()
        {
            monsters.Clear();
        }
    }
}
