﻿using System.Drawing;
using ConfigDatas;
using TaleofMonsters.Controler.Battle.Data.MemFlow;
using TaleofMonsters.Controler.Battle.Data.MemMonster;
using TaleofMonsters.Controler.Battle.Tool;
using TaleofMonsters.DataType;
using TaleofMonsters.DataType.Cards.Weapons;

namespace TaleofMonsters.Controler.Battle.Data.MemWeapon
{
    internal class TrueWeapon : IBattleWeapon
    {
        private LiveMonster self;
        private Weapon avatar;

        public int CardId
        {
            get { return avatar.Id; }
        }
        public int Life { get; private set; }
        public int Level { get; private set; }
        public string Arrow { get { return avatar.WeaponConfig.Arrow; } }
        public int Range { get { return avatar.WeaponConfig.Range; } }
        public int Mov { get { return avatar.WeaponConfig.Mov; } }
        
        public TrueWeapon(LiveMonster lm, int level, Weapon wpn)
        {
            self = lm;
            avatar = wpn;
            Level = level;
            Life = wpn.Dura;
        }

        public void OnHit()
        {
            if (avatar.WeaponConfig.Type == (int)CardTypeSub.Weapon || avatar.WeaponConfig.Type == (int)CardTypeSub.Scroll)
            {
                SubWeaponLife();
            }
        }
        
        public void OnHited()
        {
            if (avatar.WeaponConfig.Type == (int)CardTypeSub.Armor)
            {
                SubWeaponLife();
            }
        }

        public void OnRound()
        {
            if (avatar.WeaponConfig.Type == (int)CardTypeSub.Ring)
            {
                SubWeaponLife();
            }
        }

        private void SubWeaponLife()
        {
            Life--;
            if (Life == 0)
            {
                self.BreakWeapon();
                BattleManager.Instance.FlowWordQueue.Add(new FlowWord("装备破损", self.Position, -2, "Cyan", 26, 0, 0, -2, 15), false);
            }
            else
            {
                BattleManager.Instance.FlowWordQueue.Add(new FlowWord("耐久-1", self.Position, -2, "Cyan", 26, 0, 0, -2, 15), false);
            }
        }

        public CardTypeSub Type
        {
            get { return (CardTypeSub)avatar.WeaponConfig.Type; }
        }

        public Image GetImage(int width, int height)
        {
            return WeaponBook.GetWeaponImage(avatar.Id, width, height);  
        }

        public IBattleWeapon GetCopy()
        {
            TrueWeapon newWeapon = new TrueWeapon(self, Level, avatar);
            newWeapon.Life = Life;
            return newWeapon;
        }

        public void CheckWeaponEffect(LiveMonster src, int symbol)
        {
            WeaponConfig weaponConfig = ConfigData.GetWeaponConfig(CardId);

            src.Atk += avatar.Atk * symbol;
            //src.MaxHp += avatar.Hp * symbol;
            if (avatar.PArmor > 0)
            {
                src.HpBar.AddPArmor(avatar.PArmor * symbol);
            }
            if (avatar.MArmor > 0)
            {
                src.HpBar.AddMArmor(avatar.MArmor * symbol);
            }
            src.Def += avatar.Def * symbol;
            src.Mag += avatar.Mag * symbol;
            src.Hit += avatar.Hit * symbol;
            src.Dhit += avatar.Dhit * symbol;
            src.Crt += avatar.Crt * symbol;
            src.Spd += avatar.Spd * symbol;
            src.Luk += avatar.Luk * symbol;

            if (weaponConfig.Type == (int)CardTypeSub.Scroll)
            {
                if (symbol == 1)
                {
                    src.AttackType = weaponConfig.Attr;
                }
                else
                {
                    src.AttackType = (int)CardElements.None;
                }

            }
            if (weaponConfig.SkillId != 0)
            {
                if (symbol == 1)
                {
                    src.SkillManager.AddSkill(weaponConfig.SkillId, Level, weaponConfig.Percent, SkillSourceTypes.Weapon);
                }
                else
                {
                    src.SkillManager.RemoveSkill(weaponConfig.SkillId);
                }
            }
        }

        public string Des
        {
            get
            {
                return string.Format("{0}Lv{3}({1}/{2})", avatar.WeaponConfig.Name,
                    Life, avatar.Dura, Level);
            }
        }
    }
}