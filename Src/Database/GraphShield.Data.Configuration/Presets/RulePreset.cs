using GraphShield.Data.Model.Entities;
using System;
using GraphShield.Data.Model.Enums;

namespace GraphShield.Data.Configuration.Presets
{
    public class RulePreset : PresetFactory<RuleEntity>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RulePreset"/> class.
        /// </summary>
        public RulePreset()
        {
            AddPreset(new RuleEntity()
            {
                Id = Guid.Parse("f62b000b-cf9b-441a-b400-ba11f05c473b"),
                ProfileId = Guid.Parse("ef0a06cd-dc53-40c7-aa8e-65c4b06f96a3"),
                Name = "test",
                DisplayName = "Test",
                Type = RuleType.User,
                Pattern = "/v1/users/*",
                Method = RuleMethod.Get,
                Version = "1.0",
                CreatedUtc = new DateTime(2022, 9, 30),
            });
        }
    }
}