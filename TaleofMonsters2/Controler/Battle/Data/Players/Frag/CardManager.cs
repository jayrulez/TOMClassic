﻿using System;
using System.Collections.Generic;
using ConfigDatas;
using NarlonLib.Math;
using TaleofMonsters.Config;
using TaleofMonsters.Controler.Battle.Data.MemCard;
using TaleofMonsters.Core;
using TaleofMonsters.DataType;

namespace TaleofMonsters.Controler.Battle.Data.Players.Frag
{
    internal class CardManager
    {
        private Player self;
        private ActiveCard[] cards = new ActiveCard[GameConstants.CardSlotMaxCount];

        public float HeroSkillCd { get; set; }

        public CardManager(Player p)
        {
            self = p;
            for (int i = 0; i < GameConstants.CardSlotMaxCount; i++)
                cards[i] = ActiveCards.NoneCard;
        }

        public void GetNextCard()
        {
            if (GetCardNumber() < GameConstants.CardSlotMaxCount)
            {
                ActiveCard next = self.Cards.GetNextCard();
                if (next != ActiveCards.NoneCard)
                {
                    AddCard(next);
                }
                else
                {
                    self.OnGetCardFail(true); //卡组抽完有惩罚
                }
            }
            else
            {
                self.OnGetCardFail(false);//手牌满了有惩罚
            }
        }
        private void SetCard(int id, ActiveCard card)
        {
            int count = GetCardNumber();
            if (id < count)
            {
                cards[id] = card;
            }
            if (self.CardsDesk != null)
                self.CardsDesk.UpdateSlot(cards);
        }

        public void AddCard(ActiveCard card)
        {
            var spikeManager = self.SpikeManager;
            card.MpCostChange = spikeManager.MpCost;
            card.LpCostChange = spikeManager.LpCost;
            card.PpCostChange = spikeManager.PpCost;
            card.Lp2Mp = spikeManager.HasSpike("lp2mp");
            card.Combo = self.Combo;
            int count = GetCardNumber();
            if (count < GameConstants.CardSlotMaxCount)
            {
                cards[count] = card;
            }
            if (spikeManager.HasSpike("copycard") && count < GameConstants.CardSlotMaxCount-1)
            {
                cards[count+1] = card.GetCopy();
            }
            if (self.CardsDesk != null)
                self.CardsDesk.UpdateSlot(cards);
        }

        public void UpdateCardCost()
        {
            var spikeManager = self.SpikeManager;
            foreach (ActiveCard activeCard in cards)
            {
                activeCard.LpCostChange = spikeManager.LpCost;
                activeCard.MpCostChange = spikeManager.MpCost;
                activeCard.PpCostChange = spikeManager.PpCost;
                activeCard.Lp2Mp = spikeManager.HasSpike("lp2mp");
            }
            if (self.CardsDesk != null)
                self.CardsDesk.UpdateSlot(cards);
        }

        public void UpdateCardCombo()
        {
            var isCombo = self.Combo;
            foreach (ActiveCard activeCard in cards)
            {
                activeCard.Combo = isCombo;
            }
            if (self.CardsDesk != null)
                self.CardsDesk.UpdateSlot(cards);
        }

        public ActiveCard GetDeckCardAt(int index)
        {
            if (index > GameConstants.CardSlotMaxCount || index <= 0)
                return ActiveCards.NoneCard;

            return cards[index - 1];
        }

        public void DeleteCardAt(int index)
        {
            if (index <= 0)
            {//使用英雄技能卡
                return;
            }

            cards[index - 1] = ActiveCards.NoneCard;
            for (int i = 0; i < GameConstants.CardSlotMaxCount - 1; i++)
            {
                if (cards[i].CardId == 0 && cards[i + 1].CardId > 0)
                {
                    ActiveCard tempCard = cards[i];
                    cards[i] = cards[i + 1];
                    cards[i + 1] = tempCard;
                }
            }
            if (self.CardsDesk != null)
            {
                self.CardsDesk.UpdateSlot(cards);
            }
        }

        /// <summary>
        /// 把第几张牌替换成下一张卡片,初始化使用
        /// </summary>
        /// <param name="index">偏移</param>
        public void RedrawCardAt(int index)
        {
            var newCard = self.Cards.ReplaceCard(cards[index - 1]);
            if (newCard == ActiveCards.NoneCard)
            {
                return;
            }
            cards[index - 1] = newCard;
            if (self.CardsDesk != null)
            {
                self.CardsDesk.UpdateSlot(cards);
            }
        }

        public void DeleteRandomCardFor(IPlayer p, int levelChange)
        {
            if (GetCardNumber() > 0)
            {
                int id = MathTool.GetRandom(GetCardNumber());
                ActiveCard card = cards[id];
                DeleteCardAt(id + 1);

                card.ChangeLevel((byte)(card.Level + levelChange));
                Player player = p as Player;
                if (player != null)
                {
                    player.CardManager.AddCard(card);
                }
            }
        }

        public void CopyRandomCardFor(IPlayer p, int levelChange)
        {
            if (GetCardNumber() > 0)
            {
                int id = MathTool.GetRandom(GetCardNumber());
                ActiveCard card = cards[id].GetCopy();

                card.ChangeLevel((byte)(card.Level + levelChange));
                Player player = p as Player;
                if (player != null)
                {
                    player.CardManager.AddCard(card);
                }
            }
        }

        public void AddCard(int cardId, int level, int modify)
        {
            ActiveCard card = new ActiveCard(cardId, (byte)level, 0);
            card.CostModify = modify;
            AddCard(card);
        }

        public void CopyRandomNCard(int n, int spellid)
        {
            List<int> indexs = new List<int>();
            for (int i = 0; i < GameConstants.CardSlotMaxCount; i++)
            {
                if (cards[i].CardId != 0 && (cards[i].CardId != spellid || cards[i].CardType != CardTypes.Spell))
                    indexs.Add(i);
            }
            ListTool.RandomShuffle(indexs);
            for (int i = 0; i < Math.Min(n, indexs.Count); i++)
            {
                AddCard(cards[indexs[i]].GetCopy());
            }
        }

        public void DeleteAllCard()
        {
            for (int i = 0; i < GameConstants.CardSlotMaxCount; i++)
            {
                cards[i] = ActiveCards.NoneCard;
            }
            if (self.CardsDesk != null)
            {
                self.CardsDesk.UpdateSlot(cards);
            }
        }

        public int GetCardNumber()
        {
            int count = 0;
            for (int i = 0; i < GameConstants.CardSlotMaxCount; i++)
            {
                if (cards[i].CardId != 0)
                    count++;
            }
            return count;
        }

        public void ConvertCard(int count, int cardId, int levelChange)
        {
            int num = GetCardNumber();
            int id = MathTool.GetRandom(num);
            for (int i = 0; i < count; i++)
            {
                if (num <= i) continue;

                var oldLevel = cards[id].Level;
                SetCard((id + i) % num, new ActiveCard(cardId, (byte)Math.Max(1, oldLevel + levelChange), 0));
            }
        }

        public void CardLevelUp(int n, int type)
        {
            foreach (ActiveCard activeCard in cards)
            {
                if (type != 0 && ConfigIdManager.GetCardType(activeCard.CardId) != (CardTypes)type)
                {
                    continue;
                }

                activeCard.Level = (byte)(activeCard.Level + n);
                if (activeCard.Level < 1)
                {
                    activeCard.Level = 1;
                }
                else if (activeCard.Level > GameConstants.CardMaxLevel)
                {
                    activeCard.Level = GameConstants.CardMaxLevel;
                }
            }

            if (self.CardsDesk != null)
            {
                self.CardsDesk.UpdateSlot(cards);
            }
        }
    }
}
