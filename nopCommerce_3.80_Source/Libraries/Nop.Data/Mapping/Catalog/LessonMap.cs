using Nop.Core.Domain.Catalog;

namespace Nop.Data.Mapping.Catalog
{
    public partial class LessonMap : NopEntityTypeConfiguration<Lesson>
    {
        public LessonMap()
        {
            this.ToTable("Lesson");
            this.HasKey(c => c.Id);
            this.Property(c => c.Title).IsRequired().HasMaxLength(400);
            this.Property(c => c.Description).HasMaxLength(400);
        }
    }
}