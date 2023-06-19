using MSGraphShield.Data.Configuration.Extensions;
using MSGraphShield.Data.Configuration.Presets;
using MSGraphShield.Data.Model.Entities;
using MSGraphShield.Data.Model.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MSGraphShield.Data.Configuration.Builders
{
    public class ProfileBuilder : IEntityTypeConfiguration<ProfileEntity>
    {
        /// <summary>
        /// Configures the entity of type <typeparamref name="TEntity" />.
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity type.</param>
        public void Configure(EntityTypeBuilder<ProfileEntity> builder)
        {
            builder.ToTable("profiles");
            builder.HasData<ProfileEntity, ProfilePreset>();

            builder.HasKey(x => x.Id);

            builder.HasIndex(x => x.DisplayName)
                .IsUnique();

            builder.Property(x => x.Id)
                .ValueGeneratedOnAdd();

            builder.Property(x => x.DisplayName)
                .IsRequired()
                .HasMaxLength(128);

            builder.Property(x => x.Description)
                .HasMaxLength(256);

            builder.Property(x => x.Audit)
                .HasDefaultValue(AuditMode.None);

            builder.Property(x => x.IsProtected)
                .HasDefaultValue(false);

            builder.Property(x => x.CreatedUtc)
                .HasCreationTrackable();

            builder.Property(x => x.UpdatedUtc)
                .HasModificationTrackable();

            builder.HasOne(x => x.Credential)
                .WithMany(x => x.Profiles)
                .HasForeignKey(x => x.CredentialId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasMany(x => x.Clients)
                .WithMany(x => x.Profiles)
                .UsingEntity<ClientProfileEntity>(
                    x => x.HasOne(x => x.Client).WithMany().HasForeignKey(x => x.ClientId),
                    x => x.HasOne(x => x.Profile).WithMany().HasForeignKey(x => x.ProfileId));
        }
    }
}
