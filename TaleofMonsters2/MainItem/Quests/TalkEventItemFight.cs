﻿using System.Drawing;
using TaleofMonsters.Core;
using TaleofMonsters.DataType.Peoples;
using TaleofMonsters.MainItem.Quests.SceneQuests;

namespace TaleofMonsters.MainItem.Quests
{
    internal class TalkEventItemFight : TalkEventItem
    {
        private bool isEndFight = false;

        public TalkEventItemFight(int evtId, int level, Rectangle r, SceneQuestEvent e)
            : base(evtId, level, r, e)
        {
            int enemyId = config.EnemyId;
            HsActionCallback winCallback = () => { result = evt.ChooseTarget(1); isEndFight = true; };
            HsActionCallback failCallback = () => { result = evt.ChooseTarget(0); isEndFight = true; };
            PeopleBook.Fight(enemyId, "oneline", -1, level, PeopleFightReason.SceneQuest, winCallback, failCallback, failCallback);
        }

        public override void OnFrame(int tick)
        {
            if (isEndFight)
            {
                RunningState = TalkEventState.Finish;
                isEndFight = false;
            }
        }
    }
}
