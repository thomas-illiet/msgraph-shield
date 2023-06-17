using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GraphShield.Data.Configuration.Presets
{
    /// <summary>
    /// Provides extension methods for adding seed data presets to entity types.
    /// </summary>
    public static class PresetExtensions
    {
        /// <summary>
        /// Adds seed data to this entity type.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <typeparam name="TPreset">The type of the preset.</typeparam>
        /// <param name="builder">The entity type builder.</param>
        /// <returns>The data builder.</returns>
        public static DataBuilder<TEntity> HasData<TEntity, TPreset>(this EntityTypeBuilder<TEntity> builder)
            where TEntity : class
            where TPreset : IPresetFactory<TEntity>, new()
        {
            var presets = new TPreset();
            return builder.HasData(presets.Presets);
        }
    }

    /// <summary>
    /// Represents a factory preset to easily seed data into the database.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    public interface IPresetFactory<TEntity> where TEntity : class
    {
        /// <summary>
        /// Gets the collection of presets.
        /// </summary>
        List<TEntity> Presets { get; }

        /// <summary>
        /// Adds a preset to the collection.
        /// </summary>
        /// <param name="preset">The preset to add.</param>
        void AddPreset(TEntity preset);
    }

    /// <summary>
    /// Represents a factory preset to easily seed data into the database.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    public class PresetFactory<TEntity> : IPresetFactory<TEntity> where TEntity : class
    {
        private readonly List<TEntity> _presets = new List<TEntity>();

        /// <summary>
        /// Gets the collection of presets.
        /// </summary>
        public List<TEntity> Presets => _presets;

        /// <summary>
        /// Adds a preset to the collection.
        /// </summary>
        /// <param name="preset">The preset to add.</param>
        public void AddPreset(TEntity preset)
        {
            _presets.Add(preset);
        }
    }
}
