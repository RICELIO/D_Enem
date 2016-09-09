using Nop.Core.Domain.Catalog;

namespace Nop.Data.Mapping.Catalog
{
    public partial class CourseMap : NopEntityTypeConfiguration<Course>
    {
        public CourseMap()
        {
            this.ToTable("Course");
            this.HasKey(c => c.Id);
            this.Property(c => c.Name).IsRequired().HasMaxLength(400);
            this.Property(c => c.Description).HasMaxLength(400);
        }
    }
}