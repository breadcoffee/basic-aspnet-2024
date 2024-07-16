using System.ComponentModel.DataAnnotations;

namespace basicDbHandling.Models
{
    /// <summary>
    /// DB에 테이블로 만들어지는 엔티티 클래스
    /// </summary>
    public class Category
    {
        [Key] // PK
        public int Id { get; set; }

        [Required] // Not NULL
        public string Name { get; set; }
        public int DisplayOrder { get; set; }
        public DateTime PostDate { get; set; } = DateTime.Now; // 기본적으로 생성되는 현재날짜
    }
}
