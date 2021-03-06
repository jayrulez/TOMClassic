﻿using TaleofMonsters.Controler.Battle.Data.MemCard;
using TaleofMonsters.DataType.Decks;

namespace TaleofMonsters.Controler.Battle.Data.Players
{
    internal class IdlePlayer : Player
    {
        public IdlePlayer(bool isLeft)
            :base(false, isLeft)
        {
            Level = 1;
            Job = ConfigDatas.JobConfig.Indexer.NewBie;

            DeckCard[] cds = DeckBook.GetDeckByName("test", Level);
            Cards = new ActiveCards(cds);
            InitBase();
        }
    }
}
