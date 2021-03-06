﻿using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ConfigDatas;
using TaleofMonsters.Controler.Loader;
using TaleofMonsters.Core;
using TaleofMonsters.DataType.Quests;
using TaleofMonsters.DataType.Scenes;
using TaleofMonsters.DataType.User;
using TaleofMonsters.Forms.Items.Core;
using TaleofMonsters.MainItem.Quests;
using TaleofMonsters.MainItem.Quests.SceneQuests;
using TaleofMonsters.MainItem.Scenes;

namespace TaleofMonsters.Forms
{
    internal sealed partial class NpcTalkForm : BasePanel
    {
        private int tar = -1;
        private bool showImage;
        private SceneQuestBlock interactBlock;

        private ColorWordRegion colorWord;//问题区域
        private SceneQuestConfig config;
        private List<SceneQuestBlock> answerList; //回答区
        private TalkEventItem evtItem; //事件交互区

        public int EventId { get; set; }
        private int eventLevel;

        public NpcTalkForm()
        {
            InitializeComponent();
            NeedBlackForm = true;
            colorWord = new ColorWordRegion(160, 38, Width-170, "宋体", 14, Color.White);
        }

        public override void Init(int width, int height)
        {
            base.Init(width, height);
            showImage = true;
            config = ConfigData.GetSceneQuestConfig(EventId);
            if (config.Level > 0)
                eventLevel = config.Level;
            else
                eventLevel = ConfigData.GetSceneConfig(UserProfile.InfoBasic.MapId).Level;
            interactBlock = SceneQuestBook.GetQuestData(EventId, eventLevel, config.Script);
            answerList = new List<SceneQuestBlock>();
            SetupQuestItem();
        }

        public override void OnFrame(int tick, float timePass)
        {
            base.OnFrame(tick, timePass);

            if (evtItem != null && evtItem.RunningState == TalkEventItem.TalkEventState.Running)
            {
                evtItem.OnFrame(tick);
                if (evtItem.RunningState == TalkEventItem.TalkEventState.Finish)
                {
                    interactBlock = evtItem.GetResult();
                    if (interactBlock is SceneQuestSay)
                    {
                        SetupQuestItem();
                    }
                    else if (interactBlock == null)
                    {
                        answerList.Clear();
                        var block = new SceneQuestBlock(EventId, eventLevel, "结束", 999, 999);
                        answerList.Add(block);
                    }
                }
                Invalidate();
            }
        }

        private void NpcTalkForm_MouseClick(object sender, MouseEventArgs e)
        {
            if (tar != -1)
            {
                if (interactBlock == null)//一般是最后一选了
                {
                    Close();
                    Scene.Instance.OnEventFinish();
                    return;
                }


                if (evtItem != null && evtItem.RunningState == TalkEventItem.TalkEventState.Running)
                {
                    //事件过程中无视点击
                    return;
                }

                interactBlock = answerList[tar]; //对话换页
                evtItem = null;
                if (interactBlock.Children.Count == 1 && interactBlock.Children[0] is SceneQuestSay)
                {
                    interactBlock = interactBlock.Children[0];
                    SetupQuestItem();
                }
                else if (interactBlock.Children.Count == 1 && interactBlock.Children[0] is SceneQuestEvent)
                {
                    evtItem = TalkEventItem.CreateEventItem(EventId, eventLevel, this, new Rectangle(10, Height - 10 - 5 * 20 - 160, Width - 20, 160), interactBlock.Children[0] as SceneQuestEvent);
                }

                if (evtItem != null && evtItem.AutoClose())
                {
                    Close();
                    Scene.Instance.OnEventFinish();
                }
                this.Invalidate();
            }
        }

        private void SetupQuestItem()
        {
            colorWord.UpdateText(interactBlock.Script);
            answerList.Clear();
            foreach (var sceneQuestBlock in interactBlock.Children)
            {
                if (sceneQuestBlock.Disabled)
                    continue;
                answerList.Add(sceneQuestBlock);
            }
        }

        private void NpcTalkForm_Paint(object sender, PaintEventArgs e)
        {
            BorderPainter.Draw(e.Graphics, "", Width, Height);

            if (showImage)
            {
                Font font2 = new Font("黑体", 12 * 1.33f, FontStyle.Bold, GraphicsUnit.Pixel);
                e.Graphics.DrawString(string.Format("{0}(Lv{1})",config.Name, eventLevel), font2, Brushes.White, Width / 2 - 40, 8);
                font2.Dispose();

                e.Graphics.DrawImage(SceneBook.GetSceneQuestImage(config.Id), 15, 40, 140, 140);
                Image border = PicLoader.Read("Border", "questb1.PNG"); //边框
                e.Graphics.DrawImage(border, 15, 40, 140, 140);
                border.Dispose();

                if (evtItem != null)
                {
                    evtItem.Draw(e.Graphics);
                }

                colorWord.Draw(e.Graphics);

                if (answerList != null && (evtItem == null || evtItem.RunningState != TalkEventItem.TalkEventState.Running))
                {
                    Font font = new Font("宋体", 11 * 1.33f, FontStyle.Regular, GraphicsUnit.Pixel);
                    int id = 0;
                    foreach (var word in answerList)
                    {
                        if (id == tar)
                        {
                            e.Graphics.FillRectangle(Brushes.DarkBlue, 10, id * 20 + Height - 10 - answerList.Count * 20, Width - 20, 20);
                        }

                        int textOff = 20;
                        if (!string.IsNullOrEmpty(word.Prefix))
                        {
                            string icon = "";
                            if (word.Prefix.StartsWith("quest"))
                                icon = "npc5";
                            if (word.Prefix.StartsWith("rival"))
                                icon = "tsk1";
                            if (icon != "")
                            {
                                e.Graphics.DrawImage(HSIcons.GetIconsByEName(icon), textOff, id * 20 + Height - 10 - answerList.Count * 20 + 2, 18, 18);
                                textOff += 20;
                            }
                        }
                        e.Graphics.DrawString(word.Script, font, Brushes.Wheat, textOff, id * 20 + Height - 10 - answerList.Count * 20 + 2);

                        id++;
                    }
                    font.Dispose();
                }
            }
        }

        private void NpcTalkForm_MouseMove(object sender, MouseEventArgs e)
        {
            if (evtItem == null || evtItem.RunningState != TalkEventItem.TalkEventState.Running) //evtItem运行期间无法选择
            {
                if (e.Y > Height - 10 - answerList.Count * 20 && e.Y < Height - 10)
                {
                    int val = (e.Y - (Height - 10) + answerList.Count * 20) / 20;
                    if (val != tar)
                    {
                        tar = val;
                        Invalidate();
                        return;
                    }
                }
                else
                {
                    tar = -1;
                }
            }
        }
    }
}