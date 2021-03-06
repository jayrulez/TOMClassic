﻿using System;
using System.Collections.Generic;
using ConfigDatas;
using TaleofMonsters.Config;
using TaleofMonsters.Controler.Battle.Data.MemCard;
using TaleofMonsters.Controler.Battle.Tool;
using TaleofMonsters.DataType.Cards.Monsters;
using TaleofMonsters.DataType.Cards.Spells;
using TaleofMonsters.DataType.Cards.Weapons;
using TaleofMonsters.DataType.Decks;
using TaleofMonsters.DataType.Drops;
using TaleofMonsters.DataType.User;
using TaleofMonsters.Forms;
using TaleofMonsters.MainItem;
using TaleofMonsters.MainItem.Scenes;

namespace TaleofMonsters.DataType.Items
{
    internal static class Consumer
    {
        public static bool UseItemsById(int id, int useMethod)
        {
            HItemConfig itemConfig = ConfigData.GetHItemConfig(id);
            if (itemConfig.Id == ConfigData.NoneHItem.Id)
                return false;

            ItemConsumerConfig consumerConfig = ConfigData.GetItemConsumerConfig(id);
            if (useMethod == HItemTypes.Common)
            {
                if (itemConfig.SubType == HItemTypes.Gift)
                    return UseGift(id);
                if (itemConfig.SubType == HItemTypes.Item)
                    return UseItem(consumerConfig);
                if (itemConfig.SubType == HItemTypes.RandomCard)
                    return UseRandomCard(consumerConfig);
                if (itemConfig.SubType == HItemTypes.DropItem)
                    return UseDropItem(consumerConfig);
            }
            else if (useMethod == HItemTypes.Fight)
            {
                if (itemConfig.SubType == HItemTypes.Fight)
                    return UseFightItem(consumerConfig);
            }
            else if (useMethod == HItemTypes.Seed)
            {
                if (itemConfig.SubType == HItemTypes.Seed)
                    return UseSeedItem(consumerConfig);
            }

            return false;
        }

        private static bool UseItem(ItemConsumerConfig itemConfig)
        {
            if (itemConfig.ResourceId > 0)
            {
                UserProfile.InfoBag.AddResource((GameResourceType)(itemConfig.ResourceId - 1), (uint)itemConfig.ResourceCount);
            }
            if (itemConfig.GainExp > 0)
            {
                UserProfile.InfoBasic.AddExp(itemConfig.GainExp);
            }
            if (itemConfig.GainFood > 0)
            {
                UserProfile.InfoBasic.AddFood((uint)itemConfig.GainFood);
            }
            if (itemConfig.GainHealth > 0)
            {
                UserProfile.InfoBasic.AddHealth((uint)itemConfig.GainHealth);
            }
            if (itemConfig.GainMental > 0)
            {
                UserProfile.InfoBasic.AddMental((uint)itemConfig.GainMental);
            }
            if (itemConfig.MoveAdd > 0)
            {
                UserProfile.InfoBasic.SetSceneMove(itemConfig.MoveAdd, itemConfig.MoveRound);
            }
            if (!String.IsNullOrEmpty(itemConfig.Instruction))
            {
                CheckInstruction(itemConfig.Instruction);
            }
            return true;
        }

        private static bool UseFightItem(ItemConsumerConfig itemConfig)
        {
            var player = BattleManager.Instance.PlayerManager.LeftPlayer;
            if (itemConfig.GainLp > 0)
            {
                player.AddLp(itemConfig.GainLp);
            }
            if (itemConfig.GainPp > 0)
            {
                player.AddPp(itemConfig.GainPp);
            }
            if (itemConfig.GainMp > 0)
            {
                player.AddMp(itemConfig.GainMp);
            }

            if (itemConfig.DirectDamage > 0)
            {
                player.DirectDamage += itemConfig.DirectDamage;
            }

            if (itemConfig.FightRandomCardType > 0)
            {
                int cardId = CardConfigManager.GetRandomTypeCard(itemConfig.FightRandomCardType);
                var card = new ActiveCard(new DeckCard(cardId, 1, 0));
                player.CardManager.AddCard(card);
            }
            if (!string.IsNullOrEmpty(itemConfig.HolyWord))
            {
                player.AddHolyWord(itemConfig.HolyWord);
            }

            if (itemConfig.AttrAddAfterSummon != null && itemConfig.AttrAddAfterSummon.Length>0)
            {
                player.AddMonsterAddon(itemConfig.AttrAddAfterSummon);
            }
            if (itemConfig.AddTowerHp > 0)
            {
                player.AddTowerHp(itemConfig.AddTowerHp);
            }
            return true;
        }

        private static bool UseSeedItem(ItemConsumerConfig itemConfig)
        {
            return UserProfile.Profile.InfoFarm.UseSeed(itemConfig.FarmItemId, itemConfig.FarmTime);
        }
        
        private static bool UseRandomCard(ItemConsumerConfig itemConfig)
        {
            var form = PanelManager.FindPanel(typeof(CardBagForm));
            if (form != null)//如果打开着开包面板，退出
            {
                return false;
            }
            form = new CardBagForm();
            ((CardBagForm)form).SetEffect(itemConfig.Id);
            PanelManager.DealPanel(form);

            return true;
        }
        public static bool UseDropItem(ItemConsumerConfig itemConfig)
        {
            if (itemConfig.DropId > 0)
            {
                var itemList = DropBook.GetDropItemList(itemConfig.DropId);
                var countList = new List<int>();
                foreach (var itemId in itemList)
                {
                    var isEquip = ConfigIdManager.IsEquip(itemId);
                    if (isEquip)
                    {
                        UserProfile.InfoEquip.AddEquip(itemId, 60);
                    }
                    else
                    {
                        UserProfile.InfoBag.AddItem(itemId, 1);
                    }
                    countList.Add(1);
                }
                var form = new ItemPackageForm();
                ((ItemPackageForm)form).SetItem(itemList.ToArray(), countList.ToArray());
                PanelManager.DealPanel(form);
            }

            return true;
        }

        private static bool UseGift(int id)
        {
            var items = ConfigData.GetItemGiftConfig(id).Items;
            List<int> itemList = new List<int>();
            List<int> countList = new List<int>();
            for (int i = 0; i < items.Count; i++)
            {
                var item = items[i];
                UserProfile.InfoBag.AddItem(item.Id, item.Value);
                itemList.Add(item.Id);
                countList.Add(item.Value);
            }
            var form = new ItemPackageForm();
            ((ItemPackageForm)form).SetItem(itemList.ToArray(), countList.ToArray());
            PanelManager.DealPanel(form);
            return true;
        }

        private static void CheckInstruction(string ins)
        {
            switch (ins)
            {
                case "detectall": Scene.Instance.DetectAll(); break;
            }
        }
    }
}
