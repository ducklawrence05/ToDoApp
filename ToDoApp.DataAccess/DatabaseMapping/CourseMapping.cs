using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ToDoApp.Domains.Entities;

namespace ToDoApp.Infrastructures.DatabaseMapping
{
    public class CourseMapping : IEntityTypeConfiguration<Course>
    {
        public void Configure(EntityTypeBuilder<Course> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name).HasMaxLength(255);

            builder.Property(x => x.StartDate).HasDefaultValueSql("GETDATE()");

            builder.ToTable("Courses");
        }
    }
}
