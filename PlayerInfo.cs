using System.ComponentModel.DataAnnotations;

namespace LearningServer01
{
    public class PlayerInfo
    {
        // 고유 식별자(PK ,Primary Key) 설정
        [Key]
        public string NickName { get; set; }
        public int Level { get; set; }
        public int Gold { get; set; }

        // DB 기본 테이블은 List 못담은다고 하니 일단 주석 처리 
        // public List<string> Items { get; set; }
    }
}
