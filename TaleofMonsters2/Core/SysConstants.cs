namespace TaleofMonsters.Core
{
    static class SysConstants
    {
        public const int CardMaxLevel = 99;
        public const int CardSlotMaxCount = 10;
        public const int DeckCardCount = 30;//һ�����м���
        public const int CardLimit = 1;//ͬ�ֿ���ӵ�е�����

        public const int CardShopDura = 24*3600;
        public const int MergeWeaponDura = 6*3600;
        public const int ChangeCardDura = 24*3600;
        public const int NpcPieceDura = 6 * 3600;
        public const int QuestionCooldownDura = 15;

        public const float DrawManaTime = 1f;
        public const float DrawManaTimeFast = 0.5f;

        public const int PlayDeckCount = 9;
        public const int PlayFarmCount = 9;

        public const int NewDayAP = 100;

        public const int FightAPCost = 2;
        public const int MazeAPCost = 20;

        public const int RoundTime = 5000;//һ���غ϶���ms��һ���غϸ�һ�ſ�
        public const int BattleAttackRoundWait = 4; //Ӱ��ս�������Լ�һ�̵�ʱ��
        public const int BattleActionLimit = 200;//200���ɹ���
        public const float RoundRecoverAddon = 1.0f; //�غϵĻظ�����
        public const int RoundRecoverLimit = 10; //ÿ�غϵ�ħ���ָ���������
        public const int InitialAts = 30;//������ˣ��ֵ�hpҲҪ���ߣ�spell���˺����������;õ���
    }
}
