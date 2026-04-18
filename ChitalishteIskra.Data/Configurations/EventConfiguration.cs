using ChitalishteIskra.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChitalishteIskra.Data.Configurations
{
    public class EventConfiguration : IEntityTypeConfiguration<Event>
    {
        public void Configure(EntityTypeBuilder<Event> builder)
        {
            builder.Property(e => e.Name)
    .HasMaxLength(200)
    .IsRequired();

            builder.Property(e => e.Location)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(e => e.ImageUrl)
                .HasColumnType("nvarchar(max)");
        }
    }
}