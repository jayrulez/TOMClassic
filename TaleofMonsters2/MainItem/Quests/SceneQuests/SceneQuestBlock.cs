﻿using System.Collections.Generic;

namespace TaleofMonsters.MainItem.Quests.SceneQuests
{
    public class SceneQuestBlock
    {
        protected int eventId;

        protected int level;

        public List<SceneQuestBlock> Children { get; private set; }

        public int Line { get; private set; } //行号

        public string Prefix { get; protected set; } //前缀
        public string Script { get; protected set; } //文本

        public int Depth { get; private set; } //tab数量

        public bool Disabled { get; set; }

        public SceneQuestBlock(int eid, int lv, string s, int depth, int line)
        {
            eventId = eid;
            level = lv;
            Script = s;
            Depth = depth;
            Line = line;
            Children = new List<SceneQuestBlock>();
        }

        public override string ToString()
        {
            return Script;
        }
    }
}