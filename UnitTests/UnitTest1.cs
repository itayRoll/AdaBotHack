﻿namespace UnitTests
{
    using System.Collections.Generic;
    using DataProvider;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            // Arrange
            Query query = new Query(9, LevelType.Beginner.ToString(), DomainType.Game.ToString(), MediumType.Workshop.ToString());
            FileProvider provider = new FileProvider();

            // Act
            List<Result> res = provider.GetResults(query);

            // Assert result
            Assert.AreEqual(8, res.Count);
        }

        [TestMethod]
        public void TestMethod2()
        {
            // Arrange
            Query query = new Query(9, LevelType.Intermediate.ToString(), DomainType.Web.ToString(), MediumType.Course.ToString());
            FileProvider provider = new FileProvider();

            // Act
            List<Result> res = provider.GetResults(query);

            Assert.AreEqual(2, res.Count);
        }
    }
}
