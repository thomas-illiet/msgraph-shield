using GraphShield.Data.Configuration.Extensions;
using GraphShield.Data.Model.Entities;
using GraphShield.Data.Model.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GraphShield.Data.Configuration.Builders
{
    public class CredentialBuilder : IEntityTypeConfiguration<CredentialEntity>
    {
        /// <summary>
        /// Configures the entity of type <typeparamref name="TEntity" />.
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity type.</param>
        public void Configure(EntityTypeBuilder<CredentialEntity> builder)
        {
            builder.ToTable("credentials");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .ValueGeneratedOnAdd();

            builder.Property(x => x.DisplayName)
                .IsRequired()
                .HasMaxLength(128);

            builder.Property(x => x.Description)
                .HasMaxLength(256);

            builder.Property(x => x.TenantId)
                .IsRequired();

            builder.Property(x => x.ClientId)
                .IsRequired();

            builder.Property(x => x.SecretType)
                .HasDefaultValue(CredentialType.Token);

            builder.Property(x => x.Secret)
                .IsRequired();

            builder.Property(x => x.ExpireUtc);

            builder.Property(x => x.CreatedUtc)
                .HasCreationTrackable();

            builder.Property(x => x.UpdatedUtc)
                .HasModificationTrackable();
        }
    }
}