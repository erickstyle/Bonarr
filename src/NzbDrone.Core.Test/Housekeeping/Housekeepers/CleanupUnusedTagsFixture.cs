using FizzWare.NBuilder;
using FluentAssertions;
using NUnit.Framework;
using NzbDrone.Core.Download.Pending;
using NzbDrone.Core.Housekeeping.Housekeepers;
using NzbDrone.Core.Parser.Model;
using NzbDrone.Core.Test.Framework;
using NzbDrone.Core.Tv;
using NzbDrone.Core.Tags;
using NzbDrone.Core.Restrictions;

namespace NzbDrone.Core.Test.Housekeeping.Housekeepers
{
    [TestFixture]
    public class CleanupUnusedTagsFixture : DbTest<CleanupUnusedTags, Tag>
    {
        [Test]
        public void should_delete_unused_tags()
        {
            var tags = Builder<Tag>.CreateListOfSize(2).BuildList();

            Db.InsertMany(tags);
            Subject.Clean();
            AllStoredModels.Should().BeEmpty();
        }

        [Test]
        public void should_not_delete_used_tags()
        {
            var tags = Builder<Tag>.CreateListOfSize(2).BuildList();
            Db.InsertMany(tags);

            var restrictions = Builder<Restriction>.CreateListOfSize(2)
                .All()
                .With(v => v.Tags.Add(tags[0].Id))
                .BuildList();
            Db.InsertMany(restrictions);

            Subject.Clean();
            AllStoredModels.Should().HaveCount(1);
        }
    }
}
