using System;
using Nancy;
using Nancy.Bootstrapper;
using NzbDrone.Api.Frontend;

namespace NzbDrone.Api.Extensions.Pipelines
{
    public class CacheHeaderPipeline : IRegisterNancyPipeline
    {
        private readonly ICacheableSpecification _cacheableSpecification;

        public CacheHeaderPipeline(ICacheableSpecification cacheableSpecification)
        {
            _cacheableSpecification = cacheableSpecification;
        }

        public int Order { get { return 0; } }

        public void Register(IPipelines pipelines)
        {
            pipelines.AfterRequest.AddItemToStartOfPipeline((Action<NancyContext>) Handle);
        }

        private void Handle(NancyContext context)
        {
            if (_cacheableSpecification.IsCacheable(context))
            {
                context.Response.Headers.EnableCache();
            }
            else
            {
                context.Response.Headers.DisableCache();
            }
        }
    }
}