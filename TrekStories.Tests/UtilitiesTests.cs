using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Web;
using System.IO;
using Moq;
using TrekStories.Utilities;

namespace TrekStories.Tests
{
    [TestClass]
    public class UtilitiesTests
    {
        [TestMethod]
        public void CanValidateJPGFile()
        {
            HttpPostedFileBase file = new TestPostedFileBase("test.jpg");

            bool actual = FileUploadUtility.ValidFileExtension(file);

            Assert.IsTrue(actual);
        }

        [TestMethod]
        public void CanValidateFileSize()
        {
            HttpPostedFileBase file = new TestPostedFileBase(7168000);

            bool actual = FileUploadUtility.ValidFileSize(file);

            Assert.IsTrue(actual);
        }

        [TestMethod]
        public void CanInvalidateDocFile()
        {
            HttpPostedFileBase file = new TestPostedFileBase("test.doc");

            bool actual = FileUploadUtility.ValidFileExtension(file);

            Assert.IsFalse(actual);
        }

        [TestMethod]
        public void CanInvalidateFileSize()
        {
            HttpPostedFileBase file = new TestPostedFileBase(7168001);

            bool actual = FileUploadUtility.ValidFileSize(file);

            Assert.IsFalse(actual);
        }
    }


    class TestPostedFileBase : HttpPostedFileBase
    {
        int contentLength;
        string fileName;

        public TestPostedFileBase(string fileName)
        {
            this.fileName = fileName;
        }

        public TestPostedFileBase(int contentLength)
        {
            this.contentLength = contentLength;
        }

        public override int ContentLength
        {
            get { return contentLength; }
        }

        public override string FileName
        {
            get { return fileName; }
        }
    }
}
