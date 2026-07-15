using E_Commerce.Domain.Entities.Payments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_Commerce.Infrastructure.Data.Configurations
{
    internal class ProcessedWebhookEventConfiguration : IEntityTypeConfiguration<ProcessedWebhookEvent>
    {
        public void Configure(EntityTypeBuilder<ProcessedWebhookEvent> builder)
        {
            builder.Property(x => x.StripeEventId)
                .HasMaxLength(100)
                .IsRequired();


            builder.HasIndex(x => x.StripeEventId)
                .IsUnique();
        }
    }
}
