using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ToDoApp.Domains.Entities
{
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

        /* optimistic locking
         * cho phép thực hiện khi bản ghi có row version có trùng với row version hiện tại trên db ko
         * trùng thì cho xoá, ko thì ko cho xoá
         */

        /* pessimistic locking
         * khi 1 luồng đang thực thi trên db thì nó sẽ khoá lại và ko cho các luồng khác thực thi
         */

        //[Timestamp] 
        //public byte[] RowVersion { get; set; }

        [ConcurrencyCheck] //kiểm tra xem có ai thay đổi dữ liệu ko
        public decimal Balance { get; set; }

        public string Address1 { get; set; }

        //public string Address2 { get; set; }

        //[DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public int Age { get; set; }

        [ForeignKey("School")]
        public int SId { get; set; }
        public virtual School School { get; set; }

        public virtual ICollection<CourseStudent> CourseStudents { get; set; }
        public ICollection<StudentExam> StudentExams { get; set; }
    }
}
