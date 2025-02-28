using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ToDoApp.Domains.Entities
{
    [Table("Students")]
    public class Student
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)] //tắt tự tăng id
        public int Id { get; set; }

        [MaxLength(255)] //dùng đc cho cả string và các loại khác (ví dụ: byte[])
        public string? FirstName { get; set; } //? : có thể null

        [Column("Surname")]
        [StringLength(255)] //chỉ cho string
        public string? LastName { get; set; }

        public DateTime DateOfBirth { get; set; }

        //[MaxLength(2000)]
        //public byte[] Image { get; set; }

        public string Address1 { get; set; }

        //public string Address2 { get; set; }

        [NotMapped]
        public int Age { get => DateTime.Now.Year - DateOfBirth.Year; }

        [ForeignKey("School")]
        public int SId { get; set; }
        public School School { get; set; }
    }
}
