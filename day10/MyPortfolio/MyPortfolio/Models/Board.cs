using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MyPortfolio.Models
{
    public class Board
    {
        [Key]
        public int Id { get; set; } // PK
        [Required(ErrorMessage = "이름은 필수입니다")]
        [MaxLength(50)]     // NAVRCHAR(50) 사이즈 지정하려면
        [DisplayName("이름")]
        public string Name { get; set; } // 작성자명

        [Required(ErrorMessage = "아이디는 필수입니다")]
        [MaxLength(20)]
        [DisplayName("아이디")]
        public string UserId { get; set; }  // 작성자 아이디
        
        [Required(ErrorMessage = "제목은 필수입니다")]
        [MaxLength(250)]
        [DisplayName("제목")]
        public string Title { get; set; }   // 게시글 제목

        [Required(ErrorMessage = "내용은 필수입니다")]
        [DisplayName("내용")]
        public string Contents { get; set; }    // 게시글 내용
        
        [DisplayName("조회수")]
        public int? Hit { get; set; }    // 읽은 횟수
        
        [DisplayName("작성일")]
        public DateTime? RegDate { get; set; } = DateTime.Now;  // 게시글 작성일자
        
        [DisplayName("수정일")]
        public DateTime? ModDate { get; set; }  // 게시글 최종 수정일자

        // RelationShip 부모User -> 자식Board
        public virtual User User { get; set; }
    }
}
