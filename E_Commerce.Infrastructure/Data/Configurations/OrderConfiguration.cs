using E_Commerce.Domain.Entities.Orders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace E_Commerce.Infrastructure.Data.Configurations
{
    internal class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.Property(x => x.SubTotal).HasColumnType("decimal(10,2)");
            builder.Property(x => x.BuyerEmail).IsRequired().HasMaxLength(250);
            builder.Property(x => x.Status).HasConversion<string>().HasMaxLength(50);
            builder.OwnsOne(x => x.ShippingAddress);
        }
    }
}
