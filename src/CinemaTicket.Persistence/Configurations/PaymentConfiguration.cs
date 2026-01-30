namespace CinemaTicket.Persistence.Configurations
{
    using CinemaTicket.Domain.Entities;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Amount)
                   .HasPrecision(18, 2);

            builder.Property(p => p.Currency)
                   .HasMaxLength(10);

            builder.Property(p => p.StripePaymentIntentId)
                   .HasMaxLength(200);

            builder.Property(p => p.Status)
                   .HasConversion<string>()  
                   .HasMaxLength(50);

            builder.Property(p => p.Status)
                   .IsRequired()
                   .HasConversion<string>()
                   .HasMaxLength(50);


        }
    }
}
