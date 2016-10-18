namespace TaleofMonsters.Core
{
    static class GameConstants
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

        public const int BattleInitialCardCount = 3; //ս����ʼʱ�Ŀ�����
        public const int RoundTime = 8000;//һ���غ϶���ms��һ���غϸ�һ�ſ�
        public const float RoundRecoverAddon = 1.5f; //�غϵĻظ�����
        public const int RoundRecoverDoubleRound = 10; //�ڼ����غϿ�ʼ�ָ��ӱ�
        public const int RoundRecoverAllRound = 4; //ÿ���ٻغϻظ�һ������������
        public const int RoundAts = 30;//������ˣ��ֵ�hpҲҪ���ߣ�spell���˺����������;õ���
        public const int LimitAts = 600; //�������ֵ�ͻ���й�����LimitAts/RoundAts/5=�������ʱ��

        public const int MaxMeleeAtkRange = 20; //��ս��������
        public const float SideKickFactor = 0.6f; //֧Ԯ��ϵ��
        public const int DefaultHitRate = 85;//Ĭ�ϵ�����
        public const int CrtToRate = 6;//ÿ��crtʵ�ʵı�����
        public const float DefaultCrtDamage = 1.5f;//Ĭ�ϱ���ʱ���˺�����
        public const int DefToRate = 6;//ÿ��defʵ�ʵķ�����
        public const int MagToRate = 10;//ÿ��magʵ�ʵ��˺���
        public const int SpdToRate = 5;//ÿ�㹥��ʵ�ʵ�������
        public const int HitToRate = 5;//ÿ������ʵ�ʵ�������

        public const int SkillTileId = 55610001;
        public const int NewbieGift = 22031001;
        public const int NewbieGiftElement = 22018001;
    }
}
