﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ControlPlus;
using NarlonLib.Control;
using TaleofMonsters.Controler.Loader;
using TaleofMonsters.DataType;
using TaleofMonsters.DataType.Cards;
using TaleofMonsters.DataType.Equips;
using TaleofMonsters.DataType.HeroSkills;
using TaleofMonsters.DataType.Items;
using TaleofMonsters.Forms.Items.Core;
using ConfigDatas;
using TaleofMonsters.Core;
using TaleofMonsters.Forms.Items.Regions;
using TaleofMonsters.Forms.Items.Regions.Decorators;
using TaleofMonsters.DataType.User;

namespace TaleofMonsters.Forms
{
    internal sealed partial class SelectJobForm : BasePanel
    {
        private NLSelectPanel selectPanel;
        private NLPageSelector nlPageSelector1;
        private int pageid;
        private int baseid;
        private List<int> jobIdList;
        private int selectJobId;
        private VirtualRegion virtualRegion;
        private ColorWordRegion jobDes;
        private ImageToolTip tooltip = MainItem.SystemToolTip.Instance;

        private List<PictureRegionCellType> cellTypeList = new List<PictureRegionCellType>();

        public SelectJobForm()
        {
            InitializeComponent();
            bitmapButtonClose.ImageNormal = PicLoader.Read("ButtonBitmap", "CloseButton1.JPG");

            bitmapButtonSelect.ImageNormal = PicLoader.Read("ButtonBitmap", "ButtonBack2.PNG");
            bitmapButtonSelect.Font = new Font("宋体", 8*1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
            bitmapButtonSelect.ForeColor = Color.White;
            bitmapButtonSelect.IconImage = TaleofMonsters.Core.HSIcons.GetIconsByEName("oth2");
            bitmapButtonSelect.IconSize = new Size(16, 16);
            bitmapButtonSelect.IconXY = new Point(8, 5);
            bitmapButtonSelect.TextOffX = 8;

            selectPanel = new NLSelectPanel(8, 38, 154, 400, this);
            selectPanel.ItemHeight = 35;
            selectPanel.SelectIndexChanged += selectPanel_SelectedIndexChanged;
            selectPanel.DrawCell += selectPanel_DrawCell;

            this.nlPageSelector1 = new NLPageSelector(this, 10, 321, 150);
            nlPageSelector1.PageChange += nlPageSelector1_PageChange;

            jobDes = new ColorWordRegion(180, 70, 320, "宋体", 10, Color.White);

            virtualRegion = new VirtualRegion(this);
            PictureRegion region = new PictureRegion(1, 178, 266, 48, 48, PictureRegionCellType.HeroSkill, 0);
            region.AddDecorator(new RegionBorderDecorator(Color.DodgerBlue));
            virtualRegion.AddRegion(region);

            virtualRegion.AddRegion(new PictureRegion(2, 238, 266, 48, 48, PictureRegionCellType.Card, 0));
            virtualRegion.AddRegion(new PictureRegion(3, 298, 266, 48, 48, PictureRegionCellType.Card, 0));
            virtualRegion.AddRegion(new PictureRegion(4, 358, 266, 48, 48, PictureRegionCellType.Card, 0));
            virtualRegion.RegionEntered += virtualRegion_RegionEntered;
            virtualRegion.RegionLeft += virtualRegion_RegionLeft;
        }

        public override void Init(int width, int height)
        {
            base.Init(width, height);
            jobIdList = new List<int>();
            foreach (var jobConfig in ConfigData.JobDict.Values)
            {
                if (jobConfig.IsSpecial)
                    continue;
                jobIdList.Add(jobConfig.Id);
            }
            nlPageSelector1.TotalPage = (jobIdList.Count + 7) / 8;
            RefreshInfo();
        }

        private void RefreshInfo()
        {
            selectPanel.ClearContent();
            for (int i = baseid; i < Math.Min(baseid + 8, jobIdList.Count); i++)
            {
                selectPanel.AddContent(jobIdList[i]);
            }
           
            selectPanel.SelectIndex = 0;
        }

        private void selectPanel_SelectedIndexChanged()
        {
            selectJobId = jobIdList[baseid + selectPanel.SelectIndex];
            JobConfig jobConfig = ConfigData.GetJobConfig(selectJobId);
            jobDes.UpdateText(jobConfig.Des);
            virtualRegion.SetRegionKey(1, jobConfig.SkillId);
            for (int i = 2; i < 5; i++)//把后面的物件都清除下
            {
                virtualRegion.SetRegionKey(i, 0);
            }
            cellTypeList.Clear();

            if (!UserProfile.Profile.InfoRecord.CheckFlag((uint)MemPlayerFlagTypes.SelectJob))
            {
#region 显示第一次选职业的奖励
                int imgIndex = 2;
                if (jobConfig.InitialCards != null)//初始卡牌
                {
                    foreach (var cardId in jobConfig.InitialCards)
                    {
                        if (cardId > 0)
                        {
                            virtualRegion.SetRegionType(imgIndex, PictureRegionCellType.Card);
                            virtualRegion.SetRegionKey(imgIndex++, cardId);
                            cellTypeList.Add(PictureRegionCellType.Card);
                        }
                    }
                }
                if (jobConfig.InitialEquip != null)//初始道具
                {
                    foreach (var eid in jobConfig.InitialEquip)
                    {
                        if (eid > 0)
                        {
                            virtualRegion.SetRegionType(imgIndex, PictureRegionCellType.Equip);
                            virtualRegion.SetRegionKey(imgIndex++, eid);
                            cellTypeList.Add(PictureRegionCellType.Equip);
                        }
                    }
                }
                if (jobConfig.InitialEquip != null)//初始道具
                {
                    foreach (var eid in jobConfig.InitialItem)
                    {
                        if (eid > 0)
                        {
                            virtualRegion.SetRegionType(imgIndex, PictureRegionCellType.Item);
                            virtualRegion.SetRegionKey(imgIndex++, eid);
                            cellTypeList.Add(PictureRegionCellType.Item);
                        }
                    }
                }
#endregion
            }

            bitmapButtonSelect.Visible = !jobConfig.InitialLocked ||
                                         UserProfile.Profile.InfoBasic.AvailJobList.Contains(selectJobId);
            Invalidate();
        }

        private void selectPanel_DrawCell(Graphics g, int info, int xOff, int yOff)
        {
            var jobConfig = ConfigData.GetJobConfig(info);
            var img = HSIcons.GetIconsByEName("job" + jobConfig.JobIndex);
            g.DrawImage(img, 14 + xOff, 4 + yOff, 28, 28);
            Font font = new Font("微软雅黑", 11.25F*1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
            g.DrawString(jobConfig.Name, font, Brushes.White, 58 + xOff, 6 + yOff);
            font.Dispose();

            if (jobConfig.InitialLocked && !UserProfile.Profile.InfoBasic.AvailJobList.Contains(info))
            {
                Brush b = new SolidBrush(Color.FromArgb(150, Color.Black));
                g.FillRectangle(b, xOff, yOff, 154, selectPanel.ItemHeight);

                var lockIcon = HSIcons.GetIconsByEName("oth4");
                g.DrawImage(lockIcon, 65 + xOff, 6 + yOff, 24, 24);
            }
        }

        private void virtualRegion_RegionEntered(int id, int x, int y, int key)
        {
            Image image = null;
            if (id == 1)
            {//1一定是heroskill
                image = HeroPowerBook.GetPreview(key);
            }
            else
            {
                if (key > 0)
                {
                    var cellType = cellTypeList[id - 2];
                    if (cellType == PictureRegionCellType.Card)
                    {
                        image = CardAssistant.GetCard(key).GetPreview(CardPreviewType.Normal, new int[] { });
                    }
                    else if (cellType == PictureRegionCellType.Item)
                    {
                        image = HItemBook.GetPreview(key);
                    }
                    else if (cellType == PictureRegionCellType.Equip)
                    {
                        Equip equip = new Equip(key);
                        image = equip.GetPreview();
                    }
                }
            }
            if (image != null)
            {
                tooltip.Show(image, this, x, y);    
            }
        }

        private void virtualRegion_RegionLeft()
        {
            tooltip.Hide(this);
        }

        private string GetStarText(int count)
        {
            return "★★★★★★★★★★".Substring(0, Math.Min(count, 9));
        }

        private void SelectJobForm_Paint(object sender, PaintEventArgs e)
        {
            BorderPainter.Draw(e.Graphics, "", Width, Height);

            Font font = new Font("黑体", 12*1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
            e.Graphics.DrawString("选择职业", font, Brushes.White, Width / 2 - 40, 8);
            font.Dispose();

            jobDes.Draw(e.Graphics);
            virtualRegion.Draw(e.Graphics);
            
            if (selectJobId > 0)
            {
                var jobConfig = ConfigData.GetJobConfig(selectJobId);
                Font font1 = new Font("宋体", 12 * 1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
                e.Graphics.DrawString(jobConfig.Name, font1, Brushes.Gold, 180, 45);
                font1.Dispose();

                Font fontDes = new Font("宋体", 10 * 1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
                e.Graphics.DrawString("领导", fontDes, Brushes.White, 180, 175);
                e.Graphics.DrawString(GetStarText(jobConfig.EnergyRate[0] / 8 + 1), fontDes, Brushes.Gold, 210, 175);
                e.Graphics.DrawString("力量", fontDes, Brushes.White, 180, 195);
                e.Graphics.DrawString(GetStarText(jobConfig.EnergyRate[1] / 8 + 1), fontDes, Brushes.Red, 210, 195);
                e.Graphics.DrawString("魔力", fontDes, Brushes.White, 180, 215);
                e.Graphics.DrawString(GetStarText(jobConfig.EnergyRate[2] / 8 + 1), fontDes, Brushes.Blue, 210, 215);
                fontDes.Dispose();
            }

            e.Graphics.DrawRectangle(Pens.DodgerBlue, 8, 39, 153, 279);
            e.Graphics.DrawRectangle(Pens.DodgerBlue, 170, 38, 330, 125);
            e.Graphics.DrawRectangle(Pens.DodgerBlue, 170, 168, 330, 86);
            e.Graphics.DrawRectangle(Pens.DodgerBlue, 170, 259, 330, 59);
        }

        private void nlPageSelector1_PageChange(int pg)
        {
            pageid = pg;
            baseid = pageid * 8;
            RefreshInfo();
        }

        private void bitmapButtonSelect_Click(object sender, EventArgs e)
        {
            var jobConfig = ConfigData.GetJobConfig(selectJobId);
            if (jobConfig.IsSpecial || jobConfig.InitialLocked && !UserProfile.Profile.InfoBasic.AvailJobList.Contains(selectJobId))
                return;

            if (UserProfile.InfoRecord.CheckFlag((uint)MemPlayerFlagTypes.SelectJob))
            {//转职
                UserProfile.InfoBasic.Job = selectJobId;
            }
            else
            {//第一次选职业
                UserProfile.InfoBasic.Job = selectJobId;
                UserProfile.InfoRecord.SetFlag((uint)MemPlayerFlagTypes.SelectJob);
                SendJobReward();
            }

            Close();
        }

        private void SendJobReward()
        {
            JobConfig jobConfig = ConfigData.GetJobConfig(selectJobId);
            Profile user = UserProfile.Profile;
            if (jobConfig.InitialCards != null) //初始卡牌
            {
                foreach (var cardId in jobConfig.InitialCards)
                {
                    if (cardId > 0)
                    {
                        user.InfoCard.AddCard(cardId);
                    }
                }
            }
            if (jobConfig.InitialEquip != null) //初始道具
            {
                foreach (var eid in jobConfig.InitialEquip)
                {
                    if (eid > 0)
                    {
                        user.InfoEquip.AddEquip(eid, 60);
                    }
                }
            }
            if (jobConfig.InitialEquip != null) //初始道具
            {
                foreach (var eid in jobConfig.InitialItem)
                {
                    if (eid > 0)
                    {
                        user.InfoBag.AddItem(eid, 1);
                    }
                }
            }
        }

        private void bitmapButtonClose_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}