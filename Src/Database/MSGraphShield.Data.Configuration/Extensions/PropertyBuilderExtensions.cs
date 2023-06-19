using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MSGraphShield.Data.Configuration.Extensions
{
    /// <summary>
    /// Provides extension methods for configuring property builders.
    /// </summary>
    public static class PropertyBuilderExtensions
    {
        /// <summary>
        /// Configures a property to track its creation timestamp.
        /// </summary>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="propertyBuilder">The property builder.</param>
        public static PropertyBuilder<TProperty> HasCreationTrackable<TProperty>(this PropertyBuilder<TProperty> propertyBuilder)
        {
            propertyBuilder.HasColumnName("created_utc");
            propertyBuilder.IsRequired(true);
            return propertyBuilder;
        }

        /// <summary>
        /// Configures a property to track its modification timestamp.
        /// </summary>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="propertyBuilder">The property builder.</param>
        public static PropertyBuilder<TProperty> HasModificationTrackable<TProperty>(this PropertyBuilder<TProperty> propertyBuilder)
        {
            propertyBuilder.HasColumnName("updated_utc");
            propertyBuilder.IsRequired(false);
            return propertyBuilder;
        }
    }
}