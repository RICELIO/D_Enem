using Nop.Core.Domain.Topics;

namespace Nop.Data.Mapping.Topics
{
    public class TopicMap : NopEntityTypeConfiguration<Topic>
    {
        public TopicMap()
        {
            this.ToTable("Topic");
            this.HasKey(t => t.Id);
            this.Property(p => p.CourseId).IsRequired();
            this.Property(p => p.LessonId).IsRequired();

            this.HasRequired(t => t.Course)
                .WithMany(t=>t.Topics)
                .HasForeignKey(c => c.CourseId);

            this.HasRequired(t => t.Lesson)
                .WithMany(t => t.Topics)
                .HasForeignKey(c => c.LessonId);
        }
    }
}
