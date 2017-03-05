﻿using System;
using System.Collections.Generic;
using System.Windows.Forms;
using ConfigDatas;
using TaleofMonsters.DataType.User;
using TaleofMonsters.Forms;
using TaleofMonsters.Forms.Items.Core;
using TaleofMonsters.Forms.MagicBook;

namespace TaleofMonsters.MainItem 
{
    internal static class SystemMenuManager
    {
        private static List<ToolBarItemData> menuItems;
        private static List<ToolBarItemData> activeItems;
        private static List<RiverFlow> flows;

        public static bool IsHotkeyEnabled { get; set; }

        public static int MenuTar { get; private set; }

        static SystemMenuManager()
        {
            IsHotkeyEnabled = true;
            MenuTar = -1;
        }

        public static void Load(int width, int height)
        {
            flows = new List<RiverFlow>();
            flows.Add(new RiverFlow(width-138, height-55, 50, 50, 4, IconDirections.RightToLeft));
       //     flows.Add(new RiverFlow(10, 50, 50, 50, 5, IconDirections.LeftToRight)); bless system conflict
            flows.Add(new RiverFlow(width-54, 200, 50, 50, 5, IconDirections.UpToDown));

            menuItems = new List<ToolBarItemData>();
            foreach (MainIconConfig mainIconConfig in ConfigData.MainIconDict.Values)
            {
                menuItems.Add(new ToolBarItemData(mainIconConfig.Id, width, height));
            }
        }

        private static void Reload()
        {
            foreach (RiverFlow riverFlow in flows)
            {
                riverFlow.Reset();
            }

            activeItems = new List<ToolBarItemData>();
            foreach (ToolBarItemData toolBarItemData in menuItems)
            {
                int itemFlow = toolBarItemData.MainIconConfig.Flow;
                if (!toolBarItemData.Enable || itemFlow == -1 || UserProfile.InfoBasic.Level < toolBarItemData.MainIconConfig.Level)
                {
                    continue;
                }

                if (itemFlow > 0)
                {
                    System.Drawing.Point newPoint = flows[itemFlow - 1].GetNextPosition();
                    toolBarItemData.SetSize(newPoint.X, newPoint.Y, flows[itemFlow - 1].Width, flows[itemFlow - 1].Height);
                }
               
                activeItems.Add(toolBarItemData);
            }
        }

        public static bool UpdateToolbar(int mouseX, int mouseY)
        {
            foreach (ToolBarItemData item in activeItems)
            {
                if (item.InRegion(mouseX, mouseY))
                {
                    if (item.Id != MenuTar)
                    {
                        MenuTar = item.Id;
                        return true;
                    }
                    return false;
                }
            }
            if (MenuTar != -1)
            {
                MenuTar = -1;
                return true;
            }
            return false;
        }

        public static void ResetIconState()
        {
            foreach (ToolBarItemData toolBarItemData in menuItems)
            {
                if (toolBarItemData.Id >= 1000)
                {
                    toolBarItemData.Enable = false;
                }
            }
            var funcStr = ConfigData.GetSceneConfig(UserProfile.InfoBasic.MapId).Func;
            if (funcStr != null)
            {
                string[] funcs = funcStr.Split(';');
                foreach (string func in funcs)
                {
                    if (func != "")
                        SetIconEnable((SystemMenuIds) Enum.Parse(typeof (SystemMenuIds), func), true);
                }
            }

            Reload();
        }

        private static void SetIconEnable(SystemMenuIds id, bool enable)
        {
            foreach (ToolBarItemData toolBarItemData in menuItems)
            {
                if ((SystemMenuIds)toolBarItemData.Id == id)
                {
                    toolBarItemData.Enable = enable;
                    return;
                }
            }
        }

        public static void UpdateAll(Control parent)
         {
             foreach (ToolBarItemData item in activeItems)
             {
                 item.Update(parent);
             }
         }

        public static void DrawAll(System.Drawing.Graphics g)
        {
            foreach (ToolBarItemData item in activeItems)
            {
                item.Draw(g, MenuTar);
            }
        }

        public static void CheckItemClick(SystemMenuIds id)
        {
            foreach (ToolBarItemData toolBarItemData in activeItems)
            {
                if ((SystemMenuIds)toolBarItemData.Id == id && toolBarItemData.InCD)
                {
                    return;
                }
            }

            switch (id)
            {
                case SystemMenuIds.SystemMenu:
                    MainForm.Instance.DealPanel(new SystemMenu());
                    break;
                case SystemMenuIds.ItemForm:
                    MainForm.Instance.DealPanel(new ItemForm());
                    break;
                case SystemMenuIds.AchieveViewForm:
                    MainForm.Instance.DealPanel(new AchieveViewForm());
                    break;
                case SystemMenuIds.PeopleViewForm:
                    MainForm.Instance.DealPanel(new PeopleViewForm());
                    break;
                case SystemMenuIds.DeckViewForm:
                    MainForm.Instance.DealPanel(new DeckViewForm());
                    break;
                case SystemMenuIds.GameShopViewForm:
                    MainForm.Instance.DealPanel(new GameShopViewForm());
                    break;
                case SystemMenuIds.TaskForm:
                    MainForm.Instance.DealPanel(new QuestForm());
                    break;
                case SystemMenuIds.EquipmentForm:
                    MainForm.Instance.DealPanel(new EquipmentForm());
                    break;
                case SystemMenuIds.HeroSkillAttrForm:
                    MainForm.Instance.DealPanel(new HeroSkillAttrForm());
                    break;
                case SystemMenuIds.EquipComposeForm:
                    MainForm.Instance.DealPanel(new EquipComposeForm());
                    break;
                case SystemMenuIds.MagicBookViewForm:
                    MainForm.Instance.DealPanel(new MagicBookViewForm());
                    break;
                case SystemMenuIds.WorldMapViewForm:
                    MainForm.Instance.DealPanel(new WorldMapViewForm());
                    break;
                case SystemMenuIds.ConnectForm:
                    MainForm.Instance.DealPanel(new ConnectForm());
                    break;
                case SystemMenuIds.CommandForm:
                    CommandForm cmf = new CommandForm();
                    cmf.ShowDialog();
                    break;
                case SystemMenuIds.CardShopViewForm:
                    MainForm.Instance.DealPanel(new CardShopViewForm());
                    break;
                case SystemMenuIds.TournamentViewForm:
                    MainForm.Instance.DealPanel(new TournamentViewForm());
                    break;
                case SystemMenuIds.TreasureWheelForm:
                    MainForm.Instance.DealPanel(new TreasureWheelForm());
                    break;
                case SystemMenuIds.ExpBottle:
                    MainForm.Instance.DealPanel(new ExpBottleForm());
                    //if (MessageBoxEx2.Show("是否花5钻石将时间回到3小时前?") == DialogResult.OK)
                    //{
                    //    if (UserProfile.Profile.PayDiamond(5))
                    //    {
                    //        UserProfile.Profile.time.AddSeconds(-3 * 3600);
                    //    }
                    //}
                    break;
                case SystemMenuIds.QuestionForm:
                    MainForm.Instance.DealPanel(new QuestionForm());
                    break;
                case SystemMenuIds.MinigameForm:
                    MainForm.Instance.DealPanel(new MinigameForm());
                    break;
            }
        }

        public static void CheckHotKey(Keys key)
        {
            if (!IsHotkeyEnabled)
            {
                return;
            }

            switch (key)
            {
                case Keys.Escape:
                    if (!MainForm.Instance.CloseLastPanel())//如果已经没有面板了，才出
                    {
                        CheckItemClick(SystemMenuIds.SystemMenu);
                    }
                    break;
                case Keys.C:
                    CheckItemClick(SystemMenuIds.EquipmentForm);
                    break;
                case Keys.I:
                    CheckItemClick(SystemMenuIds.ItemForm);
                    break;
                case Keys.D:
                    CheckItemClick(SystemMenuIds.DeckViewForm);
                    break;
                case Keys.F:
                    CheckItemClick(SystemMenuIds.PeopleViewForm);
                    break;
                case Keys.T:
                    CheckItemClick(SystemMenuIds.TaskForm);
                    break;
                case Keys.V:
                    CheckItemClick(SystemMenuIds.GameShopViewForm);
                    break;
                case Keys.M:
                    CheckItemClick(SystemMenuIds.WorldMapViewForm);
                    break;
                case Keys.A:
                    CheckItemClick(SystemMenuIds.AchieveViewForm);
                    break;
                case Keys.G:
#if DEBUG
                    CheckItemClick(SystemMenuIds.CommandForm);
#endif
                    break;
            }
        }
    }

    internal enum SystemMenuIds
    {
        SystemMenu = 1,
        GameShopViewForm = 2,
        AchieveViewForm = 3,
        TaskForm = 6,
        PeopleViewForm = 7,
        DeckViewForm = 8,
        HeroSkillAttrForm = 9,
        EquipComposeForm = 10,
        ItemForm = 11,
        EquipmentForm = 12,
        CommandForm = 31,
        MagicBookViewForm = 32,
        WorldMapViewForm = 33,
        ConnectForm = 34,
      //  MergeWeaponForm = 35,
        CardShopViewForm = 36,
        TournamentViewForm = 37,
       // AddDayForm = 38,
        TreasureWheelForm = 39,
        ExpBottle = 40,
        QuestionForm = 41,
        MinigameForm = 42,
        GameUpToNumber = 1100,
        GameIconsCatch = 1101,
        GameBattleRobot = 1102,
        GameThreeBody = 1103,
        GameSeven = 1104,
        GameRussiaBlock = 1105,
        GameLink = 1106,
    }
}
