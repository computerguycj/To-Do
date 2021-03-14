using Microsoft.VisualStudio.TestTools.UnitTesting;
using To_Do.Controllers;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using To_Do.Data;
using Microsoft.EntityFrameworkCore.Internal;
using System.Linq;

namespace To_Do.Controllers.Tests
{
    [TestClass]
    public class ListControllerTests
    {
        [TestInitialize]
        public void Initialize()
        {
            using (var context = new ListItemTestContext())
            {
                context.Database.EnsureDeleted();
                DbInitializer.Initialize(context);
            }
        }

        [TestMethod]
        public void CanGetListItems()
        {
            using (var context = new ListItemTestContext())
            {
                Assert.IsTrue(context.ListItems.AnyAsync().Result);
            }
        }

        [TestMethod]
        public void CanAddListItems()
        {
            using (var context = new ListItemTestContext())
            {
                int count = context.ListItems.CountAsync().Result;
                context.ListItems.Add(new Models.ListItem { Text = "Test", Completed = true });
                context.SaveChanges();
                Assert.AreEqual(context.ListItems.CountAsync().Result, count + 1);
            }
        }

        [TestMethod]
        public void CanEditListItems()
        {
            using (var context = new ListItemTestContext())
            {
                var firstItem = context.ListItems.OrderBy(i => i.Id).FirstAsync().Result;
                firstItem.Text = "foo";
                context.SaveChanges();
                Assert.IsTrue(context.ListItems.OrderBy(i => i.Id).FirstAsync().Result.Text == "foo");
            }
        }
    }
}