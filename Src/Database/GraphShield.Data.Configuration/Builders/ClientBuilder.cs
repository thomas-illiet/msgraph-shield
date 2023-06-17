using GraphShield.Data.Configuration.Extensions;
using GraphShield.Data.Model.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace GraphShield.Data.Configuration.Builders
{
    public class ClientBuilder : IEntityTypeConfiguration<ClientEntity>
    {
        /// <summary>
        /// Configures the entity of type <typeparamref name="TEntity" />.
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity type.</param>
        public void Configure(EntityTypeBuilder<ClientEntity> builder)
        {
            builder.ToTable("clients");

            builder.HasKey(x => x.Id);

            builder.HasIndex(x => x.RemoteId)
                .IsUnique();

            builder.Property(x => x.Id)
                .ValueGeneratedOnAdd();

            builder.Property(x => x.DisplayName)
                .IsRequired();

            builder.Property(x => x.CredentialId);

            builder.Property(x => x.LastSeenUtc)
                .HasDefaultValue(DateTimeOffset.MinValue)
                .IsRequired();

            builder.Property(x => x.CreatedUtc)
                .HasCreationTrackable();

            builder.Property(x => x.UpdatedUtc)
                .HasModificationTrackable();

            builder.HasOne(x => x.Credential)
                .WithMany(x => x.Clients)
                .HasForeignKey(x => x.CredentialId)
                .OnDelete(DeleteBehavior.SetNull)
                .IsRequired(false);
        }
    }
}