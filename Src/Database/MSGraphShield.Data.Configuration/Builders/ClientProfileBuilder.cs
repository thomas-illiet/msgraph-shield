using MSGraphShield.Data.Model.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MSGraphShield.Data.Configuration.Builders
{
    public class ClientProfileBuilder : IEntityTypeConfiguration<ClientProfileEntity>
    {
        /// <summary>
        /// Configures the entity of type <typeparamref name="TEntity" />.
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity type.</param>
        public void Configure(EntityTypeBuilder<ClientProfileEntity> builder)
        {
            builder.ToTable("client_profiles");

            builder.HasKey(x => new { x.ProfileId, x.ClientId });

            builder.Property(x => x.ProfileId)
                .IsRequired()
                .HasColumnName("profile_id");

            builder.Property(x => x.ClientId)
                .IsRequired()
                .HasColumnName("application_id");
        }
    }
}