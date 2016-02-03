﻿using System;
using System.Drawing;
using System.Windows.Forms;
using ControlPlus;
using NarlonLib.Core;
using TaleofMonsters.Controler.Loader;
using TaleofMonsters.Core;
using TaleofMonsters.DataType;
using TaleofMonsters.DataType.Shops;
using TaleofMonsters.DataType.User;
using TaleofMonsters.Forms.Items;
using TaleofMonsters.Forms.Items.Core;
using TaleofMonsters.Forms.Items.Regions;
using TaleofMonsters.Forms.Items.Regions.Decorators;

namespace TaleofMonsters.Forms
{
    internal sealed partial class CardShopViewForm : BasePanel
    {
        private CardShopItem[] itemControls;
        private NLPageSelector nlPageSelector1;
        private int page;
        private int shelf;
        private CardProduct[] products;
        private string timeText;

        private VirtualRegion virtualRegion;

        public CardShopViewForm()
        {
            InitializeComponent();
            this.bitmapButtonClose.ImageNormal = PicLoader.Read("ButtonBitmap", "CloseButton1.JPG");
            this.bitmapButtonRefresh.ImageNormal = PicLoader.Read("ButtonBitmap", "LearnButton.JPG");
            this.nlPageSelector1 = new NLPageSelector(this, 371, 438, 150);
            nlPageSelector1.PageChange += nlPageSelector1_PageChange;

            virtualRegion = new VirtualRegion(this);
            for (int i = 0; i < 3; i++)
            {
                SubVirtualRegion subRegion = new ButtonRegion(i + 1, 16 + 45 * i, 40, 42, 23, i + 1, "ShopTag.JPG", "ShopTagOn.JPG");
                subRegion.AddDecorator(new RegionTextDecorator(subRegion, 8, 7, 9, Color.White, false));
                virtualRegion.AddRegion(subRegion);
            }
            virtualRegion.SetRegionDecorator(1, 0, "怪物");
            virtualRegion.SetRegionDecorator(2, 0, "武器");
            virtualRegion.SetRegionDecorator(3, 0, "魔法");
            virtualRegion.RegionClicked += new VirtualRegion.VRegionClickEventHandler(virtualRegion_RegionClick);
        }

        internal override void Init(int width, int height)
        {
            base.Init(width, height);
            itemControls = new CardShopItem[18];
            for (int i = 0; i < 18; i++)
            {
                itemControls[i] = new CardShopItem(this, 12 + (i % 6) * 85, 62 + (i / 6) * 125, 85, 125);
                itemControls[i].Init();
            }
            virtualRegion_RegionClick(1, MouseButtons.Left);

            SoundManager.PlayBGM("TOM003.MP3");
            IsChangeBgm = true;
            OnFrame(0);
        }

        private void RefreshInfo()
        {
            for (int i = 0; i < 18; i++)
            {
                itemControls[i].RefreshData((page * 18 + i < products.Length) ? products[page * 18 + i] : new CardProduct());
            }
            Invalidate();
        }

        private void ChangeShop(int type)
        {
            page = 0;
            shelf = type;
            products = UserProfile.InfoWorld.GetCardProductsByType((CardTypes)shelf);
            nlPageSelector1.TotalPage = (products.Length - 1)/18 + 1;
            Array.Sort(products, new CompareByMark());
            RefreshInfo();
        }

        public void ChangeShop()
        {
            ChangeShop(shelf);
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void bitmapButtonRefresh_Click(object sender, EventArgs e)
        {
            if (MessageBoxEx2.Show("是否花10钻石立刻刷新卡片?") == DialogResult.OK)
            {
                if (UserProfile.InfoBag.PayDiamond(10))
                {
                    UserProfile.InfoRecord.SetRecordById((int)MemPlayerRecordTypes.LastCardShopTime, 0);
                    ChangeShop(shelf);
                    AddFlowCenter("卡片商店数据刷新", "Lime");
                }
            }
        }

        private void virtualRegion_RegionClick(int info, MouseButtons button)
        {
            if (button == MouseButtons.Left)
            {
                for (int i = 0; i < 3; i++)
                {
                    virtualRegion.SetRegionState(i + 1, RegionState.Free);
                }

                virtualRegion.SetRegionState(info, RegionState.Blacken);
                shelf = info;
                ChangeShop(shelf);
                Invalidate(new Rectangle(16, 40, 45 * 3, 23));
            }
        }

        delegate void ChangeShopCallback(int shelf);
        internal override void OnFrame(int tick)
        {
            base.OnFrame(tick);
            foreach (CardShopItem cardShopItem in itemControls)
            {
                cardShopItem.OnFrame();
            }
            if ((tick % 6) == 0)
            {
                TimeSpan span = TimeTool.UnixTimeToDateTime(UserProfile.InfoRecord.GetRecordById((int)MemPlayerRecordTypes.LastCardShopTime) + SysConstants.CardShopDura) - DateTime.Now;
                if (span.TotalSeconds > 0)
                {
                    timeText = string.Format("更新剩余 {0}:{1:00}:{2:00}", span.Hours, span.Minutes, span.Seconds);
                    Invalidate(new Rectangle(18, 447, 150, 20));
                }
                else
                {
                    BeginInvoke(new ChangeShopCallback(ChangeShop), shelf);
                }
            }
        }

        private void CardShopViewForm_Paint(object sender, PaintEventArgs e)
        {
            BorderPainter.Draw(e.Graphics, "", Width, Height);

            Font font = new Font("黑体", 12*1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
            e.Graphics.DrawString("卡片商店", font, Brushes.White, Width / 2 - 40, 8);
            font.Dispose();

            virtualRegion.Draw(e.Graphics);
            foreach (CardShopItem ctl in itemControls)
            {
                ctl.Draw(e.Graphics);
            }
            font = new Font("宋体", 9*1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
            e.Graphics.DrawString(timeText, font, Brushes.YellowGreen, 18, 447);
            font.Dispose();
        }

        private void nlPageSelector1_PageChange(int pg)
        {
            page = pg;
            RefreshInfo();
        }
    }
}