using System;
using FluentValidation;
using NzbDrone.Core.Annotations;
using NzbDrone.Core.ThingiProvider;
using NzbDrone.Core.Validation;

namespace NzbDrone.Core.Notifications.Plex
{
    public class PlexClientSettingsValidator : AbstractValidator<PlexClientSettings>
    {
        public PlexClientSettingsValidator()
        {
            RuleFor(c => c.Host).ValidHost();
            RuleFor(c => c.Port).GreaterThan(0);
        }
    }

    public class PlexClientSettings : IProviderConfig
    {
        private static readonly PlexClientSettingsValidator Validator = new PlexClientSettingsValidator();

        public PlexClientSettings()
        {
            Port = 3000;
        }

        [FieldDefinition(0, Label = "Host")]
        public string Host { get; set; }

        [FieldDefinition(1, Label = "Port")]
        public int Port { get; set; }

        [FieldDefinition(2, Label = "Username")]
        public string Username { get; set; }

        [FieldDefinition(3, Label = "Password")]
        public string Password { get; set; }

        public bool IsValid
        {
            get
            {
                return !string.IsNullOrWhiteSpace(Host);
            }
        }

        public NzbDroneValidationResult Validate()
        {
            return new NzbDroneValidationResult(Validator.Validate(this));
        }
    }
}
