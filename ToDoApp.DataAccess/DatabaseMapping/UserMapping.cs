using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ToDoApp.Constant;
using ToDoApp.DataAccess.Entities;

namespace ToDoApp.Infrastructures.DatabaseMapping
{
    public class UserMapping : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.UserName).IsRequired();
            builder.HasIndex(x => x.UserName).IsUnique();

            builder.Property(x => x.EmailAddress).IsRequired();
            builder.HasIndex(x => x.EmailAddress).IsUnique();

            builder.Property(x => x.Password).IsRequired();
            builder.HasIndex(x => x.Password).IsUnique();

            builder.Property(x => x.Role)
                .IsRequired()
                .HasConversion(
                    codeToDatabase => (int)codeToDatabase,
                    databaseToCode => (Role)databaseToCode
                );
        }
    }
}
