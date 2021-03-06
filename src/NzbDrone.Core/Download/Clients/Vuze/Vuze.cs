using System;
using FluentValidation.Results;
using NLog;
using NzbDrone.Common.Disk;
using NzbDrone.Common.Http;
using NzbDrone.Core.Configuration;
using NzbDrone.Core.Download.Clients.Transmission;
using NzbDrone.Core.MediaFiles.TorrentInfo;
using NzbDrone.Core.RemotePathMappings;

namespace NzbDrone.Core.Download.Clients.Vuze
{
    public class Vuze : TransmissionBase
    {
        private const int MINIMUM_SUPPORTED_PROTOCOL_VERSION = 14;

        public Vuze(ITransmissionProxy proxy,
                    ITorrentFileInfoReader torrentFileInfoReader,
                    IHttpClient httpClient,
                    IConfigService configService,
                    IDiskProvider diskProvider,
                    IRemotePathMappingService remotePathMappingService,
                    Logger logger)
            : base(proxy, torrentFileInfoReader, httpClient, configService, diskProvider, remotePathMappingService, logger)
        {
        }

        protected override OsPath GetOutputPath(OsPath outputPath, TransmissionTorrent torrent)
        {
            _logger.Debug("Vuze output directory: {0}", outputPath);

            return outputPath;
        }

        protected override ValidationFailure ValidateVersion()
        {
            var versionString = _proxy.GetProtocolVersion(Settings);

            _logger.Debug("Vuze protocol version information: {0}", versionString);

            int version;
            if (!int.TryParse(versionString, out version) || version < MINIMUM_SUPPORTED_PROTOCOL_VERSION)
            {
                {
                    return new ValidationFailure(string.Empty, "Protocol version not supported, use Vuze 5.0.0.0 or higher with Vuze Web Remote plugin.");
                }
            }

            return null;
        }

        public override string Name
        {
            get
            {
                return "Vuze";
            }
        }
    }
}