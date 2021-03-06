using System.Collections.Generic;
using System.IO;
using NzbDrone.Common.Extensions;

namespace NzbDrone.Mono.Disk
{
    public static class FindDriveType
    {
        private static readonly Dictionary<string, DriveType> DriveTypeMap = new Dictionary<string, DriveType>
                                                                                  {
                                                                                      { "afpfs", DriveType.Network },
                                                                                      { "zfs", DriveType.Fixed }
                                                                                  };

        public static DriveType Find(string driveFormat)
        {
            if (driveFormat.IsNullOrWhiteSpace())
            {
                return DriveType.Unknown;
            }

            return DriveTypeMap.GetValueOrDefault(driveFormat);
        }
    }
}
