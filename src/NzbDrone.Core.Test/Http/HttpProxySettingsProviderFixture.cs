using NzbDrone.Core.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using FluentAssertions;
using NzbDrone.Test.Common;
using NzbDrone.Common.Http.Proxy;
using NzbDrone.Common.Http;

namespace NzbDrone.Core.Test.Http
{
    [TestFixture]
    public class HttpProxySettingsProviderFixture : TestBase<HttpProxySettingsProvider>
    {
        private HttpProxySettings GetProxySettings()
        {
            return new HttpProxySettings(ProxyType.Socks5, "localhost", 8080, "*.httpbin.org,google.com", true, null, null);
        }

        [Test]
        public void should_bypass_proxy()
        {
            var settings = GetProxySettings();

            Subject.ShouldProxyBeBypassed(settings, new HttpUri("http://eu.httpbin.org/get")).Should().BeTrue();
            Subject.ShouldProxyBeBypassed(settings, new HttpUri("http://google.com/get")).Should().BeTrue();
            Subject.ShouldProxyBeBypassed(settings, new HttpUri("http://localhost:8654/get")).Should().BeTrue();
        }

        [Test]
        public void should_not_bypass_proxy()
        {
            var settings = GetProxySettings();

            Subject.ShouldProxyBeBypassed(settings, new HttpUri("http://bing.com/get")).Should().BeFalse();
        }
    }
}
