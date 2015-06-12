using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JarvinZip;
using System.IO;
using System.Diagnostics;

namespace Test
{
    [TestClass]
    public class ZipTest
    {
        public ZipTest()
        {
            this.DebugPath = Path.GetDirectoryName(typeof(ZipTest).Assembly.Location);
            this.FileName =  @"Zipfile.zip";
            this.DicName =  @"ZipDIC";
            this.tempPath = this.DebugPath+@"\tmp";
        }

        ZipHandler handler = ZipHandler.GetInstance();
        private string DebugPath { set; get; }

        private string FileName { set; get; }
        private string DicName { set; get; }
        private string tempPath { set; get; }
        [TestMethod]
        public void PackFileTest ()
        {
            string fromDic = this.DebugPath + @"\" + this.DicName;
            string toZip = this.tempPath + @"\" + this.FileName;
            //ZipHandler.PackFile(fromDic, toZip, (num) => { Debug.WriteLine("压缩进度:" + num); });
            Assert.IsTrue(File.Exists(toZip));
            File.Delete(toZip);
        }
        [TestMethod]
        public void PackDirectoryTest()
        {
            string fromDic = this.DebugPath+ @"\"+ this.DicName;
            string toZip = this.tempPath + @"\" + this.FileName;
            handler.PackFileDirectory(fromDic, toZip, (num) => { Debug.WriteLine("压缩进度:" + num); }); 
            Assert.IsTrue(File.Exists(toZip));
            File.Delete(toZip);
        }

        [TestMethod]
        public void UnPackTest()
        {
            string fromZip = this.DebugPath + @"\" + this.FileName;
            string toDic = this.tempPath + @"\" + this.DicName;
            handler.IsKeepPath = false;
            handler.UnpackFiles(fromZip, toDic, (num) => { Debug.WriteLine("解压进度:" + num); });
            Assert.IsTrue(Directory.Exists(toDic));
            Directory.Delete(toDic,true);
        }
        [TestCleanup]
        public void Clean()
        {

        }
    }
}