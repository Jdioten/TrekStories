using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Web;
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

            bool actual = FileUploadUtility.InvalidFileExtension(file);

            Assert.IsFalse(actual);
        }

        [TestMethod]
        public void CanValidateFileSize()
        {
            HttpPostedFileBase file = new TestPostedFileBase(7168000);

            bool actual = FileUploadUtility.InvalidFileSize(file);

            Assert.IsFalse(actual);
        }

        [TestMethod]
        public void CanInvalidateDocFile()
        {
            HttpPostedFileBase file = new TestPostedFileBase("test.doc");

            bool actual = FileUploadUtility.InvalidFileExtension(file);

            Assert.IsTrue(actual);
        }

        [TestMethod]
        public void CanInvalidateFileSize()
        {
            HttpPostedFileBase file = new TestPostedFileBase(7168001);

            bool actual = FileUploadUtility.InvalidFileSize(file);

            Assert.IsTrue(actual);
        }

        [TestMethod]
        public void CanGetFilenameWithTimestamp()
        {
            HttpPostedFileBase file = new TestPostedFileBase("test.jpg");
            string actual = FileUploadUtility.GetFilenameWithTimestamp(file.FileName);
            DateTime now = DateTime.Now;

            string expected = "test_" + FileUploadUtility.GetTimestamp(now) + ".jpg";

            Assert.AreEqual(expected, actual);
        }
    }


    public class TestPostedFileBase : HttpPostedFileBase
    {
        int contentLength;
        string fileName;
        Stream inputStream;

        public TestPostedFileBase(string fileName)
        {
            this.fileName = fileName;
        }

        public TestPostedFileBase(int contentLength)
        {
            this.contentLength = contentLength;
        }

        public TestPostedFileBase(string fileName, int contentLength, Stream stream)
        {
            this.fileName = fileName;
            this.contentLength = contentLength;
            this.inputStream = stream;
        }

        public override int ContentLength
        {
            get { return contentLength; }
        }

        public override string FileName
        {
            get { return fileName; }
        }

        public override Stream InputStream
        {
            get { return inputStream; }
        }
    }
}
