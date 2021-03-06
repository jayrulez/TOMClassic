﻿using System;
using System.Drawing;
using ConfigDatas;
using NarlonLib.Control;
using TaleofMonsters.Controler.Loader;
using TaleofMonsters.Core;
using TaleofMonsters.DataType;
using TaleofMonsters.DataType.Blesses;
using TaleofMonsters.DataType.Others;
using TaleofMonsters.DataType.User;
using TaleofMonsters.Forms.Items.Regions;
using TaleofMonsters.MainItem.Blesses;

namespace TaleofMonsters.Forms.Items
{
    internal class BlessItem
    {
        private int index;
        private int blessId;
        private bool show;
        private ImageToolTip tooltip = MainItem.SystemToolTip.Instance;
        private VirtualRegion virtualRegion;

        private int x, y, width, height;
        private BasePanel parent;
        private BitmapButton bitmapButtonBuy;

        public BlessItem(BasePanel prt, int x, int y, int width, int height)
        {
            parent = prt;
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
            this.bitmapButtonBuy = new BitmapButton();
            bitmapButtonBuy.Location = new Point(x + 125, y + 53);
            bitmapButtonBuy.Size = new Size(50, 24);
            this.bitmapButtonBuy.Click += new System.EventHandler(this.pictureBoxBuy_Click);
            this.bitmapButtonBuy.ImageNormal = PicLoader.Read("ButtonBitmap", "ButtonBack2.PNG");
            bitmapButtonBuy.Font = new Font("宋体", 8 * 1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
            bitmapButtonBuy.ForeColor = Color.White;
            bitmapButtonBuy.IconImage = TaleofMonsters.Core.HSIcons.GetIconsByEName("hatt2");
            bitmapButtonBuy.IconSize = new Size(16, 16);
            bitmapButtonBuy.IconXY = new Point(4, 4);
            bitmapButtonBuy.TextOffX = 8;
            this.bitmapButtonBuy.Text = @"锻造";
            parent.Controls.Add(bitmapButtonBuy);
        }

        public void Init(int idx)
        {
            index = idx;

            virtualRegion = new VirtualRegion(parent);
            virtualRegion.AddRegion(new PictureRegion(1, x + 3, y + 3, 76, 75, PictureRegionCellType.Bless, 0));
            virtualRegion.RegionEntered += new VirtualRegion.VRegionEnteredEventHandler(virtualRegion_RegionEntered);
            virtualRegion.RegionLeft += new VirtualRegion.VRegionLeftEventHandler(virtualRegion_RegionLeft);
        }

        public void RefreshData(int eid)
        {
            blessId = eid;
            if (eid > 0)
            {
                bitmapButtonBuy.Visible = true;
                virtualRegion.SetRegionKey(1, eid);
                show = true;
            }
            else
            {
                virtualRegion.SetRegionKey(1, 0);
                bitmapButtonBuy.Visible = false;
                show = false;
            }

            var config = ConfigData.GetBlessConfig(eid);
            if (config.Type == 1)
            {
                bitmapButtonBuy.IconImage = TaleofMonsters.Core.HSIcons.GetIconsByEName("oth9");
                this.bitmapButtonBuy.Text = @"购买";
            }
            else
            {
                bitmapButtonBuy.IconImage = TaleofMonsters.Core.HSIcons.GetIconsByEName("hatt8");
                this.bitmapButtonBuy.Text = @"移除";
            }

            parent.Invalidate(new Rectangle(x, y, width, height));
        }

        private void virtualRegion_RegionEntered(int info, int mx, int my, int key)
        {
            if (blessId > 0)
            {
                Image image = BlessBook.GetPreview(key);
                tooltip.Show(image, parent, mx, my);
            }
        }

        private void virtualRegion_RegionLeft()
        {
            tooltip.Hide(parent);
        }

        private void pictureBoxBuy_Click(object sender, EventArgs e)
        {
            var config = ConfigData.GetBlessConfig(blessId);
            uint cost = 0;
            if (config.Type == 1) //买入
                cost = GameResourceBook.OutMercuryBlessBuy(config.Level);
            else
                cost = GameResourceBook.OutMercuryBlessBuy(config.Level);

            if (!UserProfile.InfoBag.HasResource(GameResourceType.Mercury, cost))
            {
                parent.AddFlowCenter("资源不足", "Red");
                return;
            }

            UserProfile.InfoBag.SubResource(GameResourceType.Mercury, cost);
            if (config.Type == 1) //买入
            {
                BlessManager.AddBless(blessId, GameConstants.QuestBlessTime);
                UserProfile.InfoWorld.BlessShopItems.Remove(blessId);
                parent.AddFlowCenter("祝福成功", "Lime");
            }
            else
            {
                BlessManager.RemoveBless(blessId);
                parent.AddFlowCenter("移除成功", "Lime");
            }

            (parent as BlessForm).RefreshPage();
            parent.Invalidate();
        }

        public void Draw(Graphics g)
        {
            SolidBrush sb = new SolidBrush(Color.FromArgb(20, 20, 20));
            g.FillRectangle(sb, x + 2, y + 2, width - 4, height - 4);
            sb.Dispose();
            g.DrawRectangle(Pens.Teal, x + 2, y + 2, width - 4, height - 4);

            if (show)
            {
                var blessConfig = ConfigData.GetBlessConfig(blessId);

                virtualRegion.Draw(g);

                var cost = GameResourceBook.OutMercuryBlessBuy(blessConfig.Level);
                Font ft = new Font("宋体", 9 * 1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
                Brush b = new SolidBrush(Color.FromName(HSTypes.I2QualityColor(blessConfig.Level)));
                g.DrawString(blessConfig.Name, ft, b, x + 90, y + 10);
                b.Dispose();
                g.DrawString(string.Format("{0}", cost), ft, Brushes.White, x + 90 + 20, y + 32);
                ft.Dispose();

                g.DrawImage(HSIcons.GetIconsByEName("res4"), x + 90, y + 32 - 3, 18, 18);
            }
        }
    }
}
