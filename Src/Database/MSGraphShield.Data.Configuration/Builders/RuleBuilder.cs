using MSGraphShield.Data.Configuration.Extensions;
using MSGraphShield.Data.Configuration.Presets;
using MSGraphShield.Data.Model.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MSGraphShield.Data.Configuration.Builders
{
    public class RuleBuilder : IEntityTypeConfiguration<RuleEntity>
    {
        /// <summary>
        /// Configures the entity of type <typeparamref name="TEntity" />.
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity type.</param>
        public void Configure(EntityTypeBuilder<RuleEntity> builder)
        {
            builder.ToTable("rules");
            builder.HasData<RuleEntity, RulePreset>();

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("id");

            builder.Property(x => x.ProfileId)
                .HasColumnName("profile_id");

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(128)
                .HasColumnName("name");

            builder.Property(x => x.DisplayName)
                .IsRequired()
                .HasMaxLength(128)
                .HasColumnName("display_name");

            builder.Property(x => x.Method)
                .HasColumnName("method");

            builder.Property(x => x.Pattern)
                .HasMaxLength(256)
                .HasColumnName("pattern")
                .IsRequired(false);

            builder.Property(x => x.Type)
                .HasColumnName("type");

            var jsonOptions = new JsonSerializerOptions()
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            };

            builder.Property(x => x.Remote)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, jsonOptions),
                    v => JsonSerializer.Deserialize<RuleContent>(v, jsonOptions))
                .HasColumnName("remote")
                .IsRequired(false);

            builder.Property(x => x.Request)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, jsonOptions),
                    v => JsonSerializer.Deserialize<RuleContent>(v, jsonOptions))
                .HasColumnName("request")
                .IsRequired(false);

            builder.Property(x => x.Version)
                .HasColumnName("version");

            builder.Property(x => x.CreatedUtc)
                .HasCreationTrackable();

            builder.Property(x => x.UpdatedUtc)
                .HasModificationTrackable();

            builder.HasOne(x => x.Profile)
                .WithMany(x => x.Rules);
        }
    }
}
