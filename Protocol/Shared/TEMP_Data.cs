namespace JNetwork
{
    /// <summary>
    /// 임시 전투 결과 관련 데이터 하드코딩 (추후 테이블 서비스 연동 시 교체 예정)
    /// </summary>
    public static class TEMP_Data
    {
        // 승리
        public const int WinGold = 200;
        public const int WinWood = 50;
        public const int WinFood = 50;
        public const int WinBounty = 100; // 승리 시 바운티 증가량

        // 패배
        public const int LoseGold = 30;
        public const int LoseWood = 0;
        public const int LoseFood = 0;
        public const int LoseBounty = -20; // 패배 시 바운티 감소량 (또는 증가량)

        // 무승부
        public const int DrawGold = 80;
        public const int DrawWood = 20;
        public const int DrawFood = 20;
        public const int DrawBounty = 10; // 무승부 시 바운티 증가량

        /// <summary>
        /// 파괴한 적 건물 1개당 추가 골드 보상
        /// </summary>
        public const int GoldPerDestroyedEntity = 20;

        // 수리 관련 임시 비용 (건물 1개당)
        public const int RepairCostGold = 0;
        public const int RepairCostWood = 10;
        public const int RepairCostFood = 0;

        /// <summary>
        /// 전투 매칭 시 차감되는 골드
        /// </summary>
        public const int MatchEntryGold = 10;
    }
}
