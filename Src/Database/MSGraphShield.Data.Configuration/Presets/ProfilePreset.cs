using MSGraphShield.Data.Model.Entities;
using System;
using MSGraphShield.Data.Model.Enums;

namespace MSGraphShield.Data.Configuration.Presets
{
    public class ProfilePreset : PresetFactory<ProfileEntity>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProfilePreset"/> class.
        /// </summary>
        public ProfilePreset()
        {
            AddPreset(new ProfileEntity()
            {
                Id = Guid.Parse("fbb98721-a111-4596-b6ae-c3d8319ad1f4"),
                DisplayName = "Read all groups",
                Description = "Can read all azure groups",
                Audit = AuditMode.None,
                IsProtected = true,
                IsActive = true,
                CreatedUtc = new DateTime(2022, 9, 30)
            });

            AddPreset(new ProfileEntity()
            {
                Id = Guid.Parse("ef0a06cd-dc53-40c7-aa8e-65c4b06f96a3"),
                DisplayName = "Read all users",
                Description = "Can read all azure users",
                Audit = AuditMode.None,
                IsProtected = true,
                IsActive = true,
                CreatedUtc = new DateTime(2022, 9, 30)
            });
        }
    }
}