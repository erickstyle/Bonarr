using FluentValidation;
using NzbDrone.Core.Annotations;
using NzbDrone.Core.ThingiProvider;
using NzbDrone.Core.Validation;

namespace NzbDrone.Core.Extras.Metadata.Consumers.Wdtv
{
    public class WdtvSettingsValidator : AbstractValidator<WdtvMetadataSettings>
    {
        public WdtvSettingsValidator()
        {
        }
    }

    public class WdtvMetadataSettings : IProviderConfig
    {
        private static readonly WdtvSettingsValidator Validator = new WdtvSettingsValidator();

        public WdtvMetadataSettings()
        {
            EpisodeMetadata = true;
            SeriesImages = true;
            SeasonImages = true;
            EpisodeImages = true;
        }

        [FieldDefinition(0, Label = "Episode Metadata", Type = FieldType.Checkbox)]
        public bool EpisodeMetadata { get; set; }

        [FieldDefinition(1, Label = "Series Images", Type = FieldType.Checkbox)]
        public bool SeriesImages { get; set; }

        [FieldDefinition(2, Label = "Season Images", Type = FieldType.Checkbox)]
        public bool SeasonImages { get; set; }

        [FieldDefinition(3, Label = "Episode Images", Type = FieldType.Checkbox)]
        public bool EpisodeImages { get; set; }
        
        public bool IsValid
        {
            get
            {
                return true;
            }
        }

        public NzbDroneValidationResult Validate()
        {
            return new NzbDroneValidationResult(Validator.Validate(this));
        }
    }
}
