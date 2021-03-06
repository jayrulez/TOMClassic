﻿using System.Collections.Generic;
using TaleofMonsters.Config;
using TaleofMonsters.Core;
using TaleofMonsters.DataType.Achieves;
using TaleofMonsters.DataType.Others;
using TaleofMonsters.DataType.User.Db;
using TaleofMonsters.MainItem;

namespace TaleofMonsters.DataType.User
{
    public class InfoCard
    {
        [FieldIndex(Index = 1)]
        public Dictionary<int, DbDeckCard> Cards = new Dictionary<int, DbDeckCard>();//模板id为key
        [FieldIndex(Index = 2)]
        public List<int> Newcards = new List<int>();
        [FieldIndex(Index = 3)] 
        public DbDeckData[] Decks;
        [FieldIndex(Index = 4)]
        public int DeckId; //上次出战的卡组

        public InfoCard()
        {
            Decks = new DbDeckData[GameConstants.PlayDeckCount];
            for (int i = 0; i < Decks.Length; i++)
            {
                Decks[i] = new DbDeckData(i + 1);
            }
        }

        internal int GetCardCountByType(CardTypes type)
        {
            if (type == CardTypes.Null)
            {
                return Cards.Count;
            }
            int count = 0;
            foreach (var cd in Cards.Values)
            {
                if (ConfigIdManager.GetCardType(cd.BaseId) == type)
                {
                    count++;
                }
            }
            return count;
        }

        public DbDeckData SelectedDeck
        {
            get
            {
                return Decks[DeckId];
            }
        }

        /// <summary>
        /// 添加卡牌，外层都会检查cid的合法性，所以里面不用在判断了
        /// </summary>
        /// <param name="cid">卡牌id</param>
        public DbDeckCard AddCard(int cid)
        {
            DbDeckCard card = new DbDeckCard(cid, 1, 0);
            var cardData = CardConfigManager.GetCardConfig(cid);
            if (GetCardCount(cid) >= 1) //每张卡其实只能有一份
            {
                var myCard = GetDeckCardById(cid);
                myCard.AddExp(1);//多余的卡转化为经验值
                return myCard;//每种卡牌只能拥有1张
            }

            Cards.Add(card.BaseId, card);
            Newcards.Add(card.BaseId);
            if (Newcards.Count > 10)
                Newcards.RemoveAt(0);

            AchieveBook.CheckByCheckType("card");
            MainTipManager.AddTip(string.Format("|获得卡片-|{0}|{1}", HSTypes.I2QualityColor(cardData.Quality), cardData.Name), "White");

            return card;
        }

        public void RemoveCardPiece(int id, bool returnResource)
        {
            DbDeckCard dc;
            if (Cards.TryGetValue(id, out dc))
            {
                if (dc.Exp == 0)
                {
                    return;
                }

                if (returnResource)
                {
                    var cardData = CardConfigManager.GetCardConfig(dc.BaseId);
                    MainTipManager.AddTip(string.Format("|分解卡片-|{0}|{1}", HSTypes.I2QualityColor(cardData.Quality), cardData.Name), "White");
                    int qual = CardConfigManager.GetCardConfig(dc.BaseId).Quality + 1;
                    UserProfile.Profile.InfoBag.AddResource(GameResourceType.Gem, GameResourceBook.OutGemCardDecompose(qual));
                }

                dc.Exp--;
            }
        }

        public int GetCardCount(int id)//只有0或1
        {
            if (Cards.ContainsKey(id))
            {
                return 1;
            }
            return 0;
        }

        public string[] GetDeckNames()
        {
            string[] names = new string[Decks.Length];
            for (int i = 0; i < Decks.Length; i++)
            {
                names[i] = Decks[i].Name;
            }
            return names;
        }

        public DbDeckCard GetDeckCardById(int id)
        {
            if (Cards.ContainsKey(id))
            {
                return Cards[id];
            }
            return new DbDeckCard();
        }

        public int GetCardExp(int cardId)
        {
            var card = GetDeckCardById(cardId);
            if (card.BaseId > 0)
            {
                return card.Exp;
            }
            return 0;
        }

        public void AddCardExp(int cardId, int expadd)
        {
            var card = GetDeckCardById(cardId);
            if (card.BaseId > 0)
            {
                card.AddExp(expadd);
            }
        }

        public bool CanLevelUp(int cardId)
        {
            var card = GetDeckCardById(cardId);
            if (card.BaseId > 0)
            {
                int expNeed = ExpTree.GetNextRequiredCard(card.Level);
                if (card.Exp < expNeed)
                {
                    return false;
                }
                return true;
            }
            return false;
        }

        public void CardLevelUp(int cardId)
        {       
            var card = GetDeckCardById(cardId);
            if (card.BaseId > 0)
            {
                int expNeed = ExpTree.GetNextRequiredCard(card.Level);
                if (card.Exp < expNeed)
                {
                    return;
                }
                card.Exp = (ushort)(card.Exp - expNeed);
                card.Level++;
            }
        }
    }
}
