using System;
using System.Collections.Generic;
using TaleofMonsters.DataType.Cards;
using TaleofMonsters.DataType.Decks;
using ConfigDatas;
using TaleofMonsters.DataType;

namespace TaleofMonsters.Controler.Battle.Data.MemCard
{
    internal class ActiveCard
    {
        public int Id { get; private set; } //Ψһ��id
        public DeckCard Card { get; private set; }

        public int Mp {
            get
            {
                var lpCost = Card.Lp == 0 ? 0 : Math.Max(0, Card.Lp + LpCostChange);
                if (Lp2Mp && lpCost > 0)
                {
                    return lpCost;
                }
                return Card.Mp==0?0: Math.Max(0, Card.Mp + MpCostChange);
            }
        }
        public int Lp
        {
            get {
                if (Lp2Mp)
                {
                    return 0;
                }
                return Card.Lp == 0 ? 0 : Math.Max(0,Card.Lp + LpCostChange); }
        }
        public int Pp
        {
            get { return Card.Pp == 0 ? 0 : Math.Max(0, Card.Pp + PpCostChange); }
        }

        public IEnumerable<PlayerManaTypes> CostList
        {
            get
            {
                List<PlayerManaTypes> l = new List<PlayerManaTypes>();
                for (int i = 0; i < Lp; i++)
                {
                    l.Add(PlayerManaTypes.LeaderShip);
                }
                for (int i = 0; i < Mp; i++)
                {
                    l.Add(PlayerManaTypes.Mana);
                }
                for (int i = 0; i < Pp; i++)
                {
                    l.Add(PlayerManaTypes.Power);
                }
                return l;
            }
        }
        public int MpCostChange { get; set; }//���Ա������޸�
        public int LpCostChange { get; set; }//���Ա������޸�
        public int PpCostChange { get; set; }//���Ա������޸�
        public bool Lp2Mp { get; set; } //���Ա������޸�
        public byte Level { get; set; }

        public ActiveCard()
        {
            Card = new DeckCard(0,0,0);
        }

        public ActiveCard(DeckCard card)
        {
            this.Card = card;
            Id = World.WorldInfoManager.GetCardFakeId();
            Level = card.Level;
        }

        public ActiveCard(int baseid, byte level, ushort exp)
        {
            Id = World.WorldInfoManager.GetCardFakeId();
            Card = new DeckCard(baseid, level, exp);
            Level = Card.Level;
        }
        

        public int CardId //��Ƭ���õ�id
        {
            get { return Card.BaseId; }
        }

        public CardTypes CardType
        {
            get
            {
                return CardAssistant.GetCardType(Card.BaseId);
            }
        }
        

        public ActiveCard GetCopy()
        {
            DeckCard dc = new DeckCard(Card.BaseId, Card.Level, Card.Exp);
            var ac = new ActiveCard(dc);
            ac.MpCostChange = MpCostChange;
            ac.LpCostChange = LpCostChange;
            ac.PpCostChange = PpCostChange;
            ac.Lp2Mp = Lp2Mp;
            return ac;
        }

        public void ChangeLevel(byte level)
        {
            Level = level;
            Card.Level = level;
        }

        public static bool operator ==(ActiveCard rec1, ActiveCard rec2)
        {
            return Equals(rec1, rec2);
        }

        public static bool operator !=(ActiveCard rec1, ActiveCard rec2)
        {
            return !Equals(rec1, rec2);
        }

        public override int GetHashCode()
        {
            return Card.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (GetType() != obj.GetType())
                return false;
            ActiveCard rec = (ActiveCard) obj;
            if (rec.Card == Card)
            {
                return true;
            }
            return false;
        }

        public override string ToString()
        {
            return string.Format("id={0} cid={1}", Id, CardId);
        }
    }   
}
