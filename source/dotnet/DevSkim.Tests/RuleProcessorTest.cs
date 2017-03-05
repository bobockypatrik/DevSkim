﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Security.DevSkim;
using System.Diagnostics.CodeAnalysis;

namespace DevSkim.Tests
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class RuleProcessorTest
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void IsMatch_FalseTest()
        {
            Ruleset ruleset = Ruleset.FromDirectory(@"rules\valid", null);
            RuleProcessor processor = new RuleProcessor(ruleset);            
            string testString = "this is a test string";

            // Normal functionality test
            Match match = processor.IsMatch(testString, 0, "csharp");
            Assert.IsFalse(match.Success, "Match.Success should be false");

            // Non existent langugage
            match = processor.IsMatch(testString, 0, "");
            Assert.IsFalse(match.Success, "Match.Success should be false, when no language is passed");

            // Index out of range
            match = processor.IsMatch(testString, testString.Length + 1, "csharp");
            Assert.IsFalse(match.Success, "Match.Success should be false, when invalid index is passed");
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void IsMatch_InvalidInputTest()
        {
            RuleProcessor processor = new RuleProcessor();

            // Langugage is null
            Match match = processor.IsMatch(null, 0, "");
            Assert.IsFalse(match.Success, "Match.Success should be false");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void IsMatch_InvalidLanguageTest()
        {
            RuleProcessor processor = new RuleProcessor();
            string testString = "this is a test string";

            // Langugage is null
            Match match = processor.IsMatch(testString, 0, null);
            Assert.IsFalse(match.Success, "Match.Success should be false");
        }
        
        [TestMethod]
        public void RuleInfoTest()
        {
            Ruleset ruleset = Ruleset.FromDirectory(@"rules\valid", null);
            RuleProcessor processor = new RuleProcessor(ruleset);
            string testString = "strcpy(dest,src);";
            
            Match match = processor.IsMatch(testString, 0, "cpp");
            Assert.IsTrue(match.Success, "strcpy should be flagged");

            Rule r = match.Rule;
            Assert.IsTrue(r.Description.Contains("strcpy"), "Invalid decription");
            Assert.IsTrue(r.Source.Contains("dangerous_api.json"), "Invalid file");
            Assert.IsTrue(r.Name.Contains("strcpy"), "Invalid name");
            Assert.IsTrue(r.Replecement.Contains("strcpy_s"), "Invalid replacement");
            Assert.IsTrue(r.RuleInfo.Contains(r.Id), "Invalid ruleinfo");
        }
    }
}
