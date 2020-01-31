using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Xsolla.Core;
using Xsolla.Store;

namespace Tests
{
    public class TestAPI_GetGroups : BaseTestApiScript
    {
        const int GROUPS_COUNT = 3;
        const string GROUP_WITH_NAME_ALL = "All";
        const string GROUP_WITH_NAME_INNER = "Inner";

        protected override void Request()
        {
            StoreAPI.GetListOfItemGroups(XsollaSettings.StoreProjectId, SuccessRequest, FailedRequest);
        }

        private void FailedRequest(Error error)
        {
            Assert.Fail(error.errorMessage);
            Complete();
        }

        private void SuccessRequest(Groups groups)
        {
            try {
                CheckResult(groups);
            } catch (Exception e) {
                throw e;
            } finally {
                Complete();
            }
        }

        private void CheckResult(Groups groups)
        {
            CheckCatalogSize(groups);
            CheckGroups(groups);
            CheckInnerGroup(groups);
        }

        private void CheckCatalogSize(Groups groups)
        {
            int catalogSize = groups.groups.Count();
            bool condition = catalogSize == GROUPS_COUNT;
            Assert.True(condition,
                "Catalog size must be = " + GROUPS_COUNT +
                " but we have = " + catalogSize
            );
        }

        private void CheckGroups(Groups groups)
		{
            foreach (Group group in groups.groups.ToList()) {
                if (!CheckItem(group, out string message)) {
                    Assert.Fail(message);
                }
            }
        }

        private void CheckInnerGroup(Groups groups)
        {
            Group group = GetGroupByName(groups.groups.ToList(), GROUP_WITH_NAME_ALL);
            CheckChildrensOfGroup(group);
			group = GetGroupByName(group.children, GROUP_WITH_NAME_INNER);
            if (!CheckItem(group, out string message)) {
                Assert.Fail(message);
            }
        }

        private Group GetGroupByName(List<Group> groups, string name)
		{
            IEnumerable<Group> matchingGroups = groups.Where(g => g.name.Equals(name));
            Assert.True(matchingGroups.Count() > 0, "We have not groups with name = " + name);
            return matchingGroups.First();
        }

        private void CheckChildrensOfGroup(Group group)
        {
            Assert.True(group.children.Count > 0, GetMessageFor(group, "without childrens!"));
        }

        private bool CheckItem(Group group, out string message)
        {
            message = "no message";

            Assert.False(string.IsNullOrEmpty(group.name), "Group `name` is null or empty!");
            Assert.False(string.IsNullOrEmpty(group.external_id), GetMessageFor(group, "`external_id` is null or empty!"));

            return true;
        }

		private string GetMessageFor(Group group, string innerMessage)
		{
            return "Group '" + group.name + "' have a error: " + innerMessage;
		}
    }
}
