using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CL.IO.ZIP;
using System.IO;
using System.Diagnostics;

namespace Test
{
    [TestClass]
    public class ZipTest
    {
        public ZipTest()
        {
            this.DebugPath = Path.GetDirectoryName(typeof(ZipTest).Assembly.Location)+"\\";
            this.FileName =  @"Zipfile.zip";
            this.DicName =  @"ZipDIC";
            this.tempPath = this.DebugPath+@"tmp\";
        }

        ZipHandler handler = ZipHandler.GetInstance();
        private string DebugPath { set; get; }

        private string FileName { set; get; }
        private string DicName { set; get; }
        private string tempPath { set; get; }
        [TestMethod]
        public void AddFileTest ()
        {
            string targetZipPath = this.tempPath +  this.FileName;
            if (!File.Exists(targetZipPath))
            {
                File.Copy(this.FileName, targetZipPath);
            }
            string filePath = this.DebugPath + this.FileName;
            
            handler.AddFile(filePath, targetZipPath, @"123\" + Path.GetFileName(filePath));
            File.Delete(targetZipPath);
        }
        [TestMethod]
        public void AddDicTest()
        {
            string targetZipPath = this.tempPath + this.FileName;
            if (!File.Exists(targetZipPath))
            {
                File.Copy(this.FileName, targetZipPath);
            }
            string dicPath = this.DebugPath + this.DicName;
            handler.IsKeepPath = false;
            handler.AddDirectory(dicPath, targetZipPath, Path.GetFileName(dicPath),
                (num) => { Debug.WriteLine("压缩进度:" + num); });
            File.Delete(targetZipPath);
        }
        [TestMethod]
        public void PackDirectoryTest()
        {
            string fromDic = this.DebugPath+  this.DicName;
            string toZip = this.tempPath + this.FileName;
            handler.PackFileDirectory(fromDic, toZip, (num) => { Debug.WriteLine("压缩进度:" + num); }); 
            Assert.IsTrue(File.Exists(toZip));
            File.Delete(toZip);
        }

        [TestMethod]
        public void UnPackTest()
        {
            string fromZip = this.DebugPath + this.FileName;
            string toDic = this.tempPath + this.DicName;
            handler.IsKeepPath = true;
            handler.UnpackAll(fromZip, toDic, (num) => { Debug.WriteLine("解压进度:" + num); });
            Assert.IsTrue(Directory.Exists(toDic));
            Directory.Delete(toDic,true);
        }
        [TestMethod]
        public void UnPackFileTest()
        {
            string fromZip = this.DebugPath + this.FileName;
            string toDic = this.tempPath + this.DicName;
            handler.IsKeepPath = false;
            handler.UnpackFile(fromZip, toDic, @"models/db.js");
            Assert.IsTrue(Directory.Exists(toDic));
            Directory.Delete(toDic, true);
        }
        [TestMethod]
        public void UnPackDicTest()
        {
            string fromZip = this.DebugPath + this.FileName;
            string toDic = this.tempPath + this.DicName;
            handler.IsKeepPath = false;
            handler.UnpackDirectory(fromZip, toDic, @"node_modules");
            Assert.IsTrue(Directory.Exists(toDic));
            Directory.Delete(toDic, true);
        }

        [TestCleanup]
        public void Clean()
        {

        }
    }
}