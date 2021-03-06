using System;
using System.IO;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using NzbDrone.Common.EnvironmentInfo;
using NzbDrone.Common.Extensions;
using NzbDrone.Test.Common;
using NzbDrone.Test.Common.Categories;

namespace NzbDrone.Common.Test
{
    [TestFixture]
    public class PathExtensionFixture : TestBase
    {
        private string _parent = @"C:\Test".AsOsAgnostic();

        private IAppFolderInfo GetIAppDirectoryInfo()
        {
            var fakeEnvironment = new Mock<IAppFolderInfo>();

            fakeEnvironment.SetupGet(c => c.AppDataFolder).Returns(@"C:\NzbDrone\".AsOsAgnostic());

            fakeEnvironment.SetupGet(c => c.TempFolder).Returns(@"C:\Temp\".AsOsAgnostic());

            return fakeEnvironment.Object;
        }

        [TestCase(@"c:\test\", @"c:\test")]
        [TestCase(@"c:\\test\\", @"c:\test")]
        [TestCase(@"C:\\Test\\", @"C:\Test")]
        [TestCase(@"C:\\Test\\Test\", @"C:\Test\Test")]
        [TestCase(@"\\Testserver\Test\", @"\\Testserver\Test")]
        [TestCase(@"\\Testserver\\Test\", @"\\Testserver\Test")]
        [TestCase(@"\\Testserver\Test\file.ext", @"\\Testserver\Test\file.ext")]
        [TestCase(@"\\Testserver\Test\file.ext\\", @"\\Testserver\Test\file.ext")]
        [TestCase(@"\\Testserver\Test\file.ext   \\", @"\\Testserver\Test\file.ext")]
        public void Clean_Path_Windows(string dirty, string clean)
        {
            WindowsOnly();

            var result = dirty.CleanFilePath();
            result.Should().Be(clean);
        }

        [TestCase(@"/test/", @"/test")]
        [TestCase(@"//test/", @"/test")]
        [TestCase(@"//test//", @"/test")]
        [TestCase(@"//test// ", @"/test")]
        [TestCase(@"//test//other// ", @"/test/other")]
        [TestCase(@"//test//other//file.ext ", @"/test/other/file.ext")]
        [TestCase(@"//CAPITAL//lower// ", @"/CAPITAL/lower")]
        public void Clean_Path_Linux(string dirty, string clean)
        {
            MonoOnly();

            var result = dirty.CleanFilePath();
            result.Should().Be(clean);
        }

        [TestCase(@"C:\", @"C:\")]
        [TestCase(@"C:\\", @"C:\")]
        [TestCase(@"C:\Test", @"C:\Test\\")]
        [TestCase(@"C:\\\\\Test", @"C:\Test\\")]
        [TestCase(@"C:\Test\\\\", @"C:\Test\\")]
        [TestCase(@"\\Server\pool", @"\\Server\pool")]
        [TestCase(@"\\Server\pool\", @"\\Server\pool")]
        [TestCase(@"\\Server\pool", @"\\Server\pool\")]
        [TestCase(@"\\Server\pool\", @"\\Server\pool\")]
        [TestCase(@"\\smallcheese\DRIVE_G\TV-C\Simspsons", @"\\smallcheese\DRIVE_G\TV-C\Simspsons")]
        public void paths_should_be_equal(string first, string second)
        {
            first.AsOsAgnostic().PathEquals(second.AsOsAgnostic()).Should().BeTrue();
        }

        [TestCase(@"c:\", @"C:\")]
        public void should_be_equal_windows_only(string first, string second)
        {
            WindowsOnly();
            first.PathEquals(second.AsOsAgnostic()).Should().BeTrue();
        }

        [TestCase(@"C:\Test", @"C:\Test2\")]
        [TestCase(@"C:\Test\Test", @"C:\TestTest\")]
        public void paths_should_not_be_equal(string first, string second)
        {
            first.AsOsAgnostic().PathEquals(second.AsOsAgnostic()).Should().BeFalse();
        }
        
        [Test]
        public void should_return_false_when_not_a_child()
        {
            var path = @"C:\Another Folder".AsOsAgnostic();

            _parent.IsParentPath(path).Should().BeFalse();
        }

        [Test]
        public void should_return_true_when_folder_is_parent_of_another_folder()
        {
            var path = @"C:\Test\TV".AsOsAgnostic();

            _parent.IsParentPath(path).Should().BeTrue();
        }

        [Test]
        public void should_return_true_when_folder_is_parent_of_a_file()
        {
            var path = @"C:\Test\30.Rock.S01E01.Pilot.avi".AsOsAgnostic();

            _parent.IsParentPath(path).Should().BeTrue();
        }
        [TestCase(@"C:\Test\", @"C:\Test\mydir")]
        [TestCase(@"C:\Test\", @"C:\Test\mydir\")]
        [TestCase(@"C:\Test", @"C:\Test\30.Rock.S01E01.Pilot.avi")]
        public void path_should_be_parent(string parentPath, string childPath)
        {
            parentPath.AsOsAgnostic().IsParentPath(childPath.AsOsAgnostic()).Should().BeTrue();
        }

        [TestCase(@"C:\Test2\", @"C:\Test")]
        [TestCase(@"C:\Test\Test\", @"C:\Test\")]
        [TestCase(@"C:\Test\", @"C:\Test")]
        [TestCase(@"C:\Test\", @"C:\Test\")]
        public void path_should_not_be_parent(string parentPath, string childPath)
        {
            parentPath.AsOsAgnostic().IsParentPath(childPath.AsOsAgnostic()).Should().BeFalse();
        }

        [TestCase(@"C:\test\", @"C:\Test\mydir")]
        [TestCase(@"C:\test", @"C:\Test\mydir\")]
        public void path_should_be_parent_on_windows_only(string parentPath, string childPath)
        {
            var expectedResult = OsInfo.IsWindows;

            parentPath.IsParentPath(childPath).Should().Be(expectedResult);
        }

        [TestCase(@"C:\Test\mydir", @"C:\Test")]
        [TestCase(@"C:\Test\", @"C:")]
        [TestCase(@"C:\", null)]
        public void path_should_return_parent(string path, string parentPath)
        {
            path.GetParentPath().Should().Be(parentPath);
        }

        [Test]
        public void path_should_return_parent_for_oversized_path()
        {
            var path       = @"/media/2e168617-f2ae-43fb-b88c-3663af1c8eea/downloads/sabnzbd/nzbdrone/Some.Real.Big.Thing/With.Alot.Of.Nested.Directories/Some.Real.Big.Thing/With.Alot.Of.Nested.Directories/Some.Real.Big.Thing/With.Alot.Of.Nested.Directories/Some.Real.Big.Thing/With.Alot.Of.Nested.Directories/Some.Real.Big.Thing/With.Alot.Of.Nested.Directories";
            var parentPath = @"/media/2e168617-f2ae-43fb-b88c-3663af1c8eea/downloads/sabnzbd/nzbdrone/Some.Real.Big.Thing/With.Alot.Of.Nested.Directories/Some.Real.Big.Thing/With.Alot.Of.Nested.Directories/Some.Real.Big.Thing/With.Alot.Of.Nested.Directories/Some.Real.Big.Thing/With.Alot.Of.Nested.Directories/Some.Real.Big.Thing";

            path.GetParentPath().Should().Be(parentPath);
        }

        [Test]
        [Ignore("Parent, not Grandparent")]
        public void should_not_be_parent_when_it_is_grandparent()
        {
            var path = Path.Combine(_parent, "parent", "child");

            _parent.IsParentPath(path).Should().BeFalse();
        }

        [Test]
        public void normalize_path_exception_empty()
        {
            Assert.Throws<ArgumentException>(() => "".CleanFilePath());
            ExceptionVerification.ExpectedWarns(1);
        }

        [Test]
        public void normalize_path_exception_null()
        {
            string nullPath = null;
            Assert.Throws<ArgumentException>(() => nullPath.CleanFilePath());
            ExceptionVerification.ExpectedWarns(1);
        }

        [Test]
        public void get_actual_casing_for_none_existing_file_return_partially_fixed_result()
        {
            WindowsOnly();
           "C:\\WINDOWS\\invalidfile.exe".GetActualCasing().Should().Be("C:\\Windows\\invalidfile.exe");
        }


        [Test]
        public void get_actual_casing_for_none_existing_folder_return_partially_fixed_result()
        {
            WindowsOnly();
            "C:\\WINDOWS\\SYSTEM32\\FAKEFOLDER\\invalidfile.exe".GetActualCasing().Should().Be("C:\\Windows\\System32\\FAKEFOLDER\\invalidfile.exe");
        }

        [Test]
        public void get_actual_casing_should_return_actual_casing_for_local_file_in_windows()
        {
            WindowsOnly();
            var path = Environment.ExpandEnvironmentVariables("%SystemRoot%\\System32");
            path.ToUpper().GetActualCasing().Should().Be(path);
            path.ToLower().GetActualCasing().Should().Be(path);
        }


        [Test]
        public void get_actual_casing_should_return_actual_casing_for_local_dir_in_windows()
        {
            WindowsOnly();
            var path = Directory.GetCurrentDirectory().Replace("c:\\","C:\\").Replace("system32", "System32");

            path.ToUpper().GetActualCasing().Should().Be(path);
            path.ToLower().GetActualCasing().Should().Be(path);
        }

        [Test]
        public void get_actual_casing_should_return_original_value_in_linux()
        {
            MonoOnly();
            var path = Directory.GetCurrentDirectory();
            path.GetActualCasing().Should().Be(path);
            path.GetActualCasing().Should().Be(path);
        }

        [Test]
        [Explicit]
        [ManualTest]
        public void get_actual_casing_should_return_original_casing_for_shares()
        {
            var path = @"\\server\Pool\Apps";
            path.GetActualCasing().Should().Be(path);
        }

        [Test]
        public void AppDataDirectory_path_test()
        {
            GetIAppDirectoryInfo().GetAppDataPath().Should().BeEquivalentTo(@"C:\NzbDrone\".AsOsAgnostic());
        }

        [Test]
        public void Config_path_test()
        {
            GetIAppDirectoryInfo().GetConfigPath().Should().BeEquivalentTo(@"C:\NzbDrone\Config.xml".AsOsAgnostic());
        }

        [Test]
        public void Sandbox()
        {
            GetIAppDirectoryInfo().GetUpdateSandboxFolder().Should().BeEquivalentTo(@"C:\Temp\nzbdrone_update\".AsOsAgnostic());
        }

        [Test]
        public void GetUpdatePackageFolder()
        {
            GetIAppDirectoryInfo().GetUpdatePackageFolder().Should().BeEquivalentTo(@"C:\Temp\nzbdrone_update\NzbDrone\".AsOsAgnostic());
        }

        [Test]
        public void GetUpdateClientFolder()
        {
            GetIAppDirectoryInfo().GetUpdateClientFolder().Should().BeEquivalentTo(@"C:\Temp\nzbdrone_update\NzbDrone\NzbDrone.Update\".AsOsAgnostic());
        }

        [Test]
        public void GetUpdateClientExePath()
        {
            GetIAppDirectoryInfo().GetUpdateClientExePath().Should().BeEquivalentTo(@"C:\Temp\nzbdrone_update\NzbDrone.Update.exe".AsOsAgnostic());
        }

        [Test]
        public void GetUpdateLogFolder()
        {
            GetIAppDirectoryInfo().GetUpdateLogFolder().Should().BeEquivalentTo(@"C:\NzbDrone\UpdateLogs\".AsOsAgnostic());
        }

        [Test]
        public void GetAncestorFolders_should_return_all_ancestors_in_path_Windows()
        {
            WindowsOnly();
            var path = @"C:\Test\TV\Series Title";
            var result = path.GetAncestorFolders();

            result.Count.Should().Be(4);
            result[0].Should().Be(@"C:\");
            result[1].Should().Be(@"Test");
            result[2].Should().Be(@"TV");
            result[3].Should().Be(@"Series Title");
        }

        [Test]
        public void GetAncestorFolders_should_return_all_ancestors_in_path_Linux()
        {
            MonoOnly();
            var path = @"/Test/TV/Series Title";
            var result = path.GetAncestorFolders();

            result.Count.Should().Be(4);
            result[0].Should().Be(@"/");
            result[1].Should().Be(@"Test");
            result[2].Should().Be(@"TV");
            result[3].Should().Be(@"Series Title");
        }
    }
}
