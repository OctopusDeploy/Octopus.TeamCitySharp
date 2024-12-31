using System;
using System.Net;
using NUnit.Framework;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using TeamCitySharp.DomainEntities;
using TeamCitySharp.Fields;
using TeamCitySharp.Connection;

namespace TeamCitySharp.IntegrationTests
{
    [TestFixture]
    public class when_interacting_to_get_vcs_details
    {
        private ITeamCityClient m_client;
        private readonly string m_server;
        private readonly bool m_useSsl;
        private readonly string m_username;
        private readonly string m_password;
        private readonly string m_goodProjectId;

        public when_interacting_to_get_vcs_details()
        {
            m_server = Configuration.GetAppSetting("Server");
            bool.TryParse(Configuration.GetAppSetting("UseSsl"), out m_useSsl);
            m_username = Configuration.GetAppSetting("Username");
            m_password = Configuration.GetAppSetting("Password");
            m_goodProjectId = Configuration.GetAppSetting("GoodProjectId");
        }

        [SetUp]
        public void SetUp()
        {
            m_client = new TeamCityClient(m_server, m_useSsl);
            m_client.Connect(m_username, m_password);
        }

        [Test]
        public void it_returns_exception_when_no_host_specified()
        {
            Assert.Throws<ArgumentNullException>(() => new TeamCityClient(null));
        }


        [Test]
        public void it_returns_exception_when_no_connection_formed()
        {
            var client = new TeamCityClient(m_server, m_useSsl);
            Assert.Throws<ArgumentException>(() => client.VcsRoots.All());
        }

        [Test]
        public void it_returns_all_vcs_roots()
        {
            List<VcsRoot> vcsRoots = m_client.VcsRoots.All();

            Assert.That(vcsRoots.Any(), "No VCS Roots were found for the installation");
        }

        [TestCase("TestDrive_ReactApp_HttpsGithubComHypnosphiTeamcityReactDemoRefsHeadsMaster")]
        public void it_returns_vcs_details_when_passing_vcs_root_id(string vcsRootId)
        {
            VcsRoot rootDetails = m_client.VcsRoots.ById(vcsRootId);

            Assert.That(rootDetails != null, "Cannot find the specific VCSRoot");
        }

        [Test]
        public void it_returns_correct_next_builds_with_filter()
        {
            var client = new TeamCityClient(m_server, m_useSsl);
            client.ConnectAsGuest();

            VcsRootField vcsRootField = VcsRootField.WithFields(id: true, href: true, lastChecked: true, name: true,
                status: true, vcsName: true, version: true);
            VcsRootsField vcsRootsField = VcsRootsField.WithFields(vcsRootField);
            var vcsRoots = client.VcsRoots.GetFields(vcsRootsField.ToString()).All();

            Assert.That(vcsRoots != null);
        }

        [Test, Ignore("Test user doesn't have the rights to create new VcsRoot on tested instance.")]
        public void it_create_new_vsc()
        {
            var project = m_client.Projects.ById(m_goodProjectId);

            VcsRoot vcsroot = new VcsRoot();
            vcsroot.Id = project.Id + "_vcsroot1_01";
            vcsroot.Name = project.Name + "_vcsroot1";
            vcsroot.VcsName = "jetbrains.git";
            vcsroot.Project = new Project();
            vcsroot.Project.Id = project.Id;

            Properties properties = new Properties();

            properties.Add("agentCleanFilesPolicy", "IGNORED_ONLY");
            vcsroot.Properties = properties;

            var vcsroot2 = m_client.VcsRoots.CreateVcsRoot(vcsroot, project.Id);

            m_client.VcsRoots.SetVcsRootValue(vcsroot2, VcsRootValue.Name, "TestChangeName");

            m_client.VcsRoots.SetConfigurationProperties(vcsroot2, "agentCleanFilesPolicy", "ALL_UNTRACKED");
            m_client.VcsRoots.SetConfigurationProperties(vcsroot2, "tt", "tt2");
            m_client.VcsRoots.DeleteProperties(vcsroot2, "tt");
            m_client.VcsRoots.DeleteVcsRoot(vcsroot2);
        }

        [Test]
        public void it_throws_exception_create_new_vsc_forbidden()
        {
            var project = m_client.Projects.ById(m_goodProjectId);

            VcsRoot vcsroot = new VcsRoot();
            vcsroot.Id = project.Id + "_vcsroot1_01";
            vcsroot.Name = project.Name + "_vcsroot1";
            vcsroot.VcsName = "jetbrains.git";
            vcsroot.Project = new Project();
            vcsroot.Project.Id = project.Id;

            Properties properties = new Properties();

            properties.Add("agentCleanFilesPolicy", "IGNORED_ONLY");
            vcsroot.Properties = properties;

            try
            {
                m_client.VcsRoots.CreateVcsRoot(vcsroot, project.Id);
            }
            catch (HttpException e)
            {
                Assert.That(e.ResponseStatusCode == HttpStatusCode.Forbidden);
            }
        }
    }
}
