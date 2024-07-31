using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MyPortfolio.Models
{
    public class Project
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength (100)]
        [DisplayName("프로젝트명")]
        public string ProjectName { get; set; }

        [Required]
        [MaxLength(800)]
        [DisplayName("프로젝트설명")]
        public string Description { get; set; }

        // 300X500 or 600 이미지
        [FileExtensions(Extensions = ".jpg,.png,.jpeg", ErrorMessage = "이미지 파일을 선택하세요.")]
        public string? FilePath { get; set; }
    }

    // 파일 업로드를 위한 중간단계 모델
    public class TempProject
    {
        public int Id { get; set; }

        [Required]
        [DisplayName("프로젝트명")]
        public string ProjectName { get; set; }


        [Required]
        [DisplayName("프로젝트설명")]
        public string Description { get; set; }

        [DisplayName("프로젝트이미지")]
        public IFormFile? ProjectImage { get; set; }

        public string? FilePath { get; set; }
    }
}
