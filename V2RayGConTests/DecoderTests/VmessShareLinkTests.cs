﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace V2RayGCon.Test.DecoderTests
{
    [TestClass]
    public class VmessShareLinkTests
    {
        List<Tuple<Models.Datas.Vmess, string>> GenVmessTestData()
        {
            var data = new List<Tuple<Models.Datas.Vmess, string>>();

            var vmessStandard = new Models.Datas.Vmess
            {
                v = "2",
                add = "1.2.3.4",
                port = "1234",
                id = Guid.NewGuid().ToString(),
                ps = "standard",
            };

            var linkStandard = vmessStandard.ToVmessLink();

            var vmessNonStandard = new Models.Datas.Vmess
            {
                v = "2",
                add = "1.2.3.4",
                port = "1234",
                id = Guid.NewGuid().ToString(),
                ps = "non-standard",
            };

            var linkNonStandard = vmessNonStandard.ToVmessLink().Replace(@"=", @"");

            data.Add(new Tuple<Models.Datas.Vmess, string>(vmessStandard, linkStandard));
            data.Add(new Tuple<Models.Datas.Vmess, string>(vmessNonStandard, linkNonStandard));
            return data;
        }

        [TestMethod]
        public void VmessDecoderTest()
        {
            var testData = GenVmessTestData();
            var testLinks = testData.Select(e => e.Item2).ToList();
            var subText = string.Join(Environment.NewLine, testLinks);

            var cache = Services.Cache.Instance;
            var vmessDecoder = new Services.ShareLinkComponents.VmessDecoder(cache);

            var vmessLinks = vmessDecoder.ExtractLinksFromText(subText);
            foreach (var vmessLink in vmessLinks)
            {
                Assert.IsTrue(testLinks.Contains(vmessLink));
            }
        }

        [TestMethod]
        public void ExtractVmessLinkTest()
        {
            var testData = GenVmessTestData();
            var vmessLinks = testData.Select(e => e.Item2).ToList();
            var subText = string.Join(Environment.NewLine, vmessLinks);

            var extracted = Misc.Utils.ExtractLinks(
                subText,
                VgcApis.Models.Datas.Enums.LinkTypes.vmess
            );

            Assert.AreEqual(testData.Count, extracted.Count);
            foreach (var link in extracted)
            {
                Assert.IsTrue(vmessLinks.Contains(link));
            }
        }

        [TestMethod]
        public void DecodeVmessTest()
        {
            var testData = GenVmessTestData();

            foreach (var data in testData)
            {
                var vmess = Misc.Utils.VmessLink2Vmess(data.Item2);
                Assert.IsTrue(vmess.Equals(data.Item1));
            }
        }

        [TestMethod]
        public void DecodeVmessFailTest()
        {
            var errorVmess = new Models.Datas.Vmess
            {
                id = "1234", // invalid GUID
                port = "1234",
                add = "1.2.3.4",
            };

            var vmessLink = errorVmess.ToVmessLink();
            var vmessDecode = Misc.Utils.VmessLink2Vmess(vmessLink);
            Assert.IsNull(vmessDecode);
        }
    }
}
