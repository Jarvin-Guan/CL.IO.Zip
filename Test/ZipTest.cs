using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CL.IO.Zip;
using System.IO;
using System.Linq;
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
        public void PackDirectoryTest()
        {
            string fromDic = this.DebugPath + this.DicName;
            string toZip = this.tempPath + this.FileName;
            if(File.Exists(toZip))
            {
                File.Delete(toZip);
            }
            double percent = 0;
            handler.PackDirectory(fromDic, toZip, (num) => { Debug.WriteLine("压缩进度:" + num); percent = num; });
            Assert.IsTrue(File.Exists(toZip));
            Assert.IsTrue(percent == 100);
            File.Delete(toZip);
        }
        [TestMethod]
        public void AddFileTest ()
        {
            string zipPath = this.tempPath +  this.FileName;
            if (!File.Exists(zipPath))
            {
                File.Copy(this.FileName, zipPath);
            }
            string filePath = this.DebugPath + this.FileName;
            int count =handler.GetZipFileCount(zipPath);
            handler.AddFile(filePath, zipPath, @"123\" + Path.GetFileName(filePath));
            Assert.IsTrue(count+1==handler.GetZipFileCount(zipPath));
            File.Delete(zipPath);
        }
        [TestMethod]
        public void AddDicTest()
        {
            string zipPath = this.tempPath + this.FileName;
            if (!File.Exists(zipPath))
            {
                File.Copy(this.FileName, zipPath);
            }
            string dicPath = this.DebugPath + this.DicName;
            handler.IsKeepPath = false;
            int count = handler.GetZipFileCount(zipPath);
            handler.AddDirectory(dicPath, zipPath, Path.GetFileName(dicPath),
                (num) => { Debug.WriteLine("压缩进度:" + num); });
            Assert.IsTrue(count + Directory.GetFiles(dicPath,"*",SearchOption.AllDirectories).ToList().Count 
                == handler.GetZipFileCount(zipPath));
            File.Delete(zipPath);
        }
        

        [TestMethod]
        public void UnPackTest()
        {
            string fromZip = this.DebugPath + this.FileName;
            string toDic = this.tempPath + this.DicName;
            handler.IsKeepPath = true;
            handler.UnpackAll(fromZip, toDic, (num) => { Debug.WriteLine("解压进度:" + num); });
            Assert.IsTrue(Directory.GetFiles(toDic, "*", SearchOption.AllDirectories).ToList().Count
                == handler.GetZipFileCount(fromZip));
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