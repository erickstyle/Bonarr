using System.Collections.Generic;
using NzbDrone.Core.Indexers;
using NzbDrone.Core.Parser.Model;
using NzbDrone.Core.ThingiProvider;

namespace NzbDrone.Core.Download
{
    public interface IDownloadClient : IProvider
    {
        DownloadProtocol Protocol { get; }

        string Download(RemoteEpisode remoteEpisode);
        IEnumerable<DownloadClientItem> GetItems();
        void RemoveItem(string downloadId, bool deleteData);
        DownloadClientStatus GetStatus();
    }
}
