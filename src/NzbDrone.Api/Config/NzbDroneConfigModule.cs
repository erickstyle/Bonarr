using System.Linq;
using System.Reflection;
using NzbDrone.Api.REST;
using NzbDrone.Core.Configuration;

namespace NzbDrone.Api.Config
{
    public abstract class NzbDroneConfigModule<TResource> : NzbDroneRestModule<TResource> where TResource : RestResource, new()
    {
        private readonly IConfigService _configService;

        protected NzbDroneConfigModule(IConfigService configService)
            : this(new TResource().ResourceName.Replace("config", ""), configService)
        {
        }

        protected NzbDroneConfigModule(string resource, IConfigService configService) :
            base("config/" + resource.Trim('/'))
        {
            _configService = configService;

            GetResourceSingle = GetConfig;
            GetResourceById = GetConfig;
            UpdateResource = SaveConfig;
        }

        private TResource GetConfig()
        {
            var resource = ToResource(_configService);
            resource.Id = 1;

            return resource;
        }

        protected abstract TResource ToResource(IConfigService model);

        private TResource GetConfig(int id)
        {
            return GetConfig();
        }

        private void SaveConfig(TResource resource)
        {
            var dictionary = resource.GetType()
                                     .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                                     .ToDictionary(prop => prop.Name, prop => prop.GetValue(resource, null));

            _configService.SaveConfigDictionary(dictionary);
        }
    }
}
