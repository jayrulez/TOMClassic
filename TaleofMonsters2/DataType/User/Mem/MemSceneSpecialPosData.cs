﻿using TaleofMonsters.Core;

namespace TaleofMonsters.DataType.User.Mem
{
    public class MemSceneSpecialPosData
    {
        [FieldIndex(Index = 4)] public int Id;
        [FieldIndex(Index = 1)] public string Type;
        [FieldIndex(Index = 2)] public string Info;
        [FieldIndex(Index = 3)] public bool Disabled;
        [FieldIndex(Index = 5)] public int RandomSeed;
    }
}