using System;
using System.Collections.Generic;
using ConfigDatas;
using NarlonLib.Math;
using TaleofMonsters.Controler.Battle.Data.MemMonster;
using TaleofMonsters.Controler.Battle.Tool;
using TaleofMonsters.DataType;

namespace TaleofMonsters.Controler.Battle.Data
{
    internal class MonsterAuro:IMonsterAuro
    {
        private LiveMonster self;
        private int buffId; //buff id
        private int level; //buff�ȼ�

        private int range = -1; //��Χ
        private string target;
        private int targetMonsterId; //�ض���id
        private int starMin = 0; //��С�Ǽ�Ӱ��
        private int starMax = 10; //����Ǽ�Ӱ��

        private List<CardTypeSub> raceList = new List<CardTypeSub>();
        private List<CardElements> attrList = new List<CardElements>();

        public MonsterAuro(LiveMonster mon, int buff, int lv, string tar)
        {
            self = mon;
            buffId = buff;
            level = lv;
            target = tar;
        }

        public IMonsterAuro AddRace(string race)
        {
            raceList.Add((CardTypeSub)Enum.Parse(typeof(CardTypeSub), race));
            return this;
        }

        public IMonsterAuro AddAttr(string attr)
        {
            attrList.Add((CardElements)Enum.Parse(typeof(CardElements), attr));
            return this;
        }

        public IMonsterAuro SetRange(int rg)
        {
            range = rg;
            return this;
        }

        public IMonsterAuro SetMid(int mid)
        {
            targetMonsterId = mid;
            return this;
        }

        public IMonsterAuro SetStar(int min, int max)
        {
            starMin = min;
            starMax = max;
            return this;
        }

        #region IMonsterAuro ��Ա

        public void CheckAuroState()
        {
            int size = BattleManager.Instance.MemMap.CardSize;
            foreach (LiveMonster mon in BattleManager.Instance.MonsterQueue.Enumerator)
            {
                if (mon.IsGhost || mon.Id == self.Id)
                    continue;
                if (target[0] != 'A' && ((BattleTargetManager.IsSpellEnemyMonster(target[0]) && self.IsLeft != mon.IsLeft) || (BattleTargetManager.IsSpellFriendMonster(target[0]) && self.IsLeft == mon.IsLeft)))
                    continue;
                if (targetMonsterId != 0 && mon.Avatar.Id != targetMonsterId)
                    continue;
                if (mon.Star > starMax || mon.Star < starMin)
                    continue;
                if (raceList.Count > 0 && !raceList.Contains((CardTypeSub)mon.Avatar.MonsterConfig.Type))
                    continue;
                if (attrList.Count > 0 && !attrList.Contains((CardElements)mon.Avatar.MonsterConfig.Attr))
                    continue;

                int truedis = MathTool.GetDistance(self.Position, mon.Position);
                if (range != -1 && range*size/10 <= truedis)
                    continue;

                mon.AddBuff(buffId, level, 0.05);
            }
        }

        #endregion
    }
}
