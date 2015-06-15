using ICSharpCode.SharpZipLib.Checksums;
using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace CL.IO.Zip
{
    public class ZipHandler
    {
        #region Single
        public static ZipHandler GetInstance()
        {
            return SingleHelper.GetEmperor();
        }
        private class SingleHelper
        {
            private static ZipHandler emperor = new ZipHandler();
            public static ZipHandler GetEmperor()
            {
                return emperor;
            }
        }
        #endregion

        public delegate void ProcessChange(double percent);
        private  Dictionary<string, ProcessItem> ProcessItems = new Dictionary<string, ProcessItem>();

        private bool _isKeepPath = true;
        /// <summary>
        /// 压缩、解压的过程中是否保持路径
        /// </summary>
        public bool IsKeepPath { set { _isKeepPath = value; } get { return _isKeepPath; } }

        #region 压缩
        /// <summary>
        /// 添加单个文件到压缩包中
        /// </summary>
        /// <param name="filePath">要压缩的文件</param>
        /// <param name="zipPath">目标压缩包路径</param>
        /// <param name="filePathInZip">在压缩包中文件的路径</param>
        public  void AddFile(string filePath, string zipPath,string filePathInZip)
        {
            using (ZipFile zip = new ZipFile(zipPath))
            {
                zip.BeginUpdate();
                zip.Add(filePath, filePathInZip);
                zip.CommitUpdate();
            }
        }
        
        /// <summary>
        /// 压缩文件夹到指定zip包中
        /// </summary>
        /// <param name="dicPath">需要压缩的文件夹</param>
        /// <param name="zipPath">zip包路径</param>
        /// <param name="dicPathInZip">压缩以后文件夹在zip包中的路径</param>
        public void AddDirectory(string dicPath,string zipPath,string dicPathInZip,ProcessChange changedDG)
        {
            dicPathInZip = dicPathInZip.EndsWith(Path.DirectorySeparatorChar.ToString()) ? dicPathInZip : dicPathInZip + Path.DirectorySeparatorChar;
            var files = Directory.GetFiles(dicPath, "*", SearchOption.AllDirectories);
            double totalCount = files.Count();
            string key = System.Guid.NewGuid().ToString(); //Guid Key
            ProcessItems.Add(key, new ProcessItem(totalCount));

            if(!Directory.Exists(dicPath))
            {
                throw new ArgumentException("文件夹路径不存在");
            }

            foreach (var file in files)
            {
                string filePathInZip=IsKeepPath?dicPathInZip+file.Remove(0,dicPath.Count()):
                    dicPathInZip+Path.GetFileName(file);
                AddFile(file, zipPath, filePathInZip);
                changedDG(AddOneAndReport(key));
            }
            ProcessItems.Remove(key); 
        }

        /// <summary>
        /// pack directory
        /// </summary>
        /// <param name="strDirectory">The directory path you want to zip</param>
        /// <param name="zipedFile">Target zipFile Path</param>
        /// <param name="changedDG">report process delegate</param>

        public void PackDirectory(string strDirectory, string zipedFile, ProcessChange changedDG)
        {
            if (!Directory.Exists(strDirectory))
            {
                throw new ArgumentException("需要压缩的文件夹不存在");
            }
            using (System.IO.FileStream ZipFile = System.IO.File.Create(zipedFile))
            {
                using (ZipOutputStream s = new ZipOutputStream(ZipFile))
                {
                    //总需要压缩的文件数量
                    double totalCount = Directory.GetFileSystemEntries(strDirectory, "*", SearchOption.AllDirectories).Count();
                    string key = System.Guid.NewGuid().ToString(); //Guid Key
                    ProcessItems.Add(key, new ProcessItem(totalCount));
                    PackSetp(strDirectory, s, "", key, zipedFile, changedDG);
                    ProcessItems.Remove(key);
                }
            }

        }

        /// <summary>
        /// 递归遍历目录
        /// </summary>
        private  void PackSetp(string strDirectory, ZipOutputStream s, string parentPath,
            string processItemKey, string zipPath, ProcessChange changedDG)
        {
            if (strDirectory[strDirectory.Length - 1] != Path.DirectorySeparatorChar)
            {
                strDirectory += Path.DirectorySeparatorChar;
            }
            Crc32 crc = new Crc32();

            string[] filenames = Directory.GetFileSystemEntries(strDirectory);

            foreach (string file in filenames)// 遍历所有的文件和目录
            {
                if (Directory.Exists(file))// 先当作目录处理如果存在这个目录就递归Copy该目录下面的文件
                {
                    string pPath = parentPath;
                    pPath += file.Substring(file.LastIndexOf(Path.DirectorySeparatorChar) + 1);
                    pPath += Path.DirectorySeparatorChar;
                    PackSetp(file, s, pPath, processItemKey,zipPath, changedDG);
                    if (changedDG != null)
                    {
                        changedDG(AddOneAndReport(processItemKey));
                    }
                }

                else // 否则直接压缩文件
                {
                    //打开压缩文件
                    using (FileStream fs = File.OpenRead(file))
                    {

                        byte[] buffer = new byte[fs.Length];
                        fs.Read(buffer, 0, buffer.Length);

                        string fileName = parentPath + file.Substring(file.LastIndexOf("\\") + 1);
                        if(!this.IsKeepPath&&fileName.Contains("\\"))
                        {
                            fileName=Path.GetFileName(fileName);
                            if (ProcessItems[processItemKey].HandledFiles.Count(p=>p==fileName)>0)
                            {
                                changedDG(AddOneAndReport(processItemKey));
                                continue;
                            }
                        }

                        ZipEntry entry = new ZipEntry(fileName);

                        entry.DateTime = DateTime.Now;
                        entry.Size = fs.Length;

                        fs.Close();

                        crc.Reset();
                        crc.Update(buffer);

                        entry.Crc = crc.Value;
                        s.PutNextEntry(entry);

                        s.Write(buffer, 0, buffer.Length);
                        if (changedDG != null)
                        {
                            changedDG(AddOneAndReport(processItemKey));
                        };
                        ProcessItems[processItemKey].HandledFiles.Add(fileName);
                    }
                }
            }
        }
        #endregion

        #region 解压
        /// <summary>
        /// unPack zipFile
        /// </summary>
        /// <param name="zipFilePath">the path of zipFile</param>
        /// <param name="unzipPath">to Directory</param>
        /// <param name="changedDG">report process delegate</param>
        public  void UnpackAll(string zipFilePath, string unzipPath, ProcessChange changedDG)
        {
            //总需要压缩的文件数量
            double totalCount = GetZipFileCount(zipFilePath);
            string key = System.Guid.NewGuid().ToString(); //Guid Key
            ProcessItems.Add(key, new ProcessItem(totalCount));
            UnpackFiles(zipFilePath, unzipPath, changedDG, "*", key);
            ProcessItems.Remove(key);
        }
        
        public void UnpackFile(string zipFilePath,string unzipPath,string filePathInZip)
        {
            UnpackFiles(zipFilePath, unzipPath,null,filePathInZip);
        }

        public void UnpackDirectory(string zipFilePath, string unzipPath, string DicPathInZip)
        {
            UnpackFiles(zipFilePath, unzipPath, null, DicPathInZip); 
        }

        /// <summary>
        /// 解压基类
        /// </summary>
        /// <param name="zipFilePath">压缩包文件路径</param>
        /// <param name="unzipPath">解压到的文件路径</param>
        /// <param name="changedDG">进度反馈委托</param>
        /// <param name="directoryName">解压指定的文件或文件夹，默认为空（所有）</param>
        private void UnpackFiles(string zipFilePath, string unzipPath,
            ProcessChange changedDG = null, string directName = "*", string processKey = "")
        {
            #region unzip
            if (unzipPath[unzipPath.Length - 1] != Path.DirectorySeparatorChar)
            {
                unzipPath = unzipPath + Path.DirectorySeparatorChar;
            }
            if(!Directory.Exists(unzipPath))
            {
                Directory.CreateDirectory(unzipPath);
            }
            using (ZipInputStream zipStream = new ZipInputStream(File.OpenRead(zipFilePath)))
            {
                ZipEntry zipEntry = null;
                while ((zipEntry = zipStream.GetNextEntry()) != null)
                {
                    zipEntry.CompressedSize = 9;
                    string directoryName = Path.GetDirectoryName(zipEntry.Name);
                    string fileName = Path.GetFileName(zipEntry.Name);
                    if (!string.IsNullOrEmpty(directoryName))
                    {
                        if (!Directory.Exists(unzipPath + directoryName))
                        {
                            if (directName != "*" && !Path.HasExtension(directName) && !directoryName.StartsWith(directName))
                            { continue; }
                            if(IsKeepPath)
                            {
                                Directory.CreateDirectory(unzipPath + directoryName);
                            }
                        }
                    }
                    if (!string.IsNullOrEmpty(fileName))
                    {
                        if (zipEntry.CompressedSize == 0)
                            break;
                        if (zipEntry.IsDirectory)
                        {
                            directoryName = Path.GetDirectoryName(unzipPath + zipEntry.Name);
                        }
                        string zipFileName = !IsKeepPath ? Path.GetFileName(zipEntry.Name) : zipEntry.Name;
                        if (directName != "*" && !Path.HasExtension(directName) && !zipEntry.Name.StartsWith(directName))
                        { continue; }
                        if (Path.HasExtension(directName) && directName == zipEntry.Name)
                        {
                            HandleFile(unzipPath + zipFileName, zipStream, processKey, changedDG); 
                            return;
                        }
                        else if (!string.IsNullOrEmpty(directName)&&
                            !Path.HasExtension(directName))
                        {
                            HandleFile(unzipPath + zipFileName, zipStream, processKey, changedDG);
                        }
                    }
                }
            }
            #endregion
        }

        #endregion

        #region 工具
        private void HandleFile(string filePath, ZipInputStream zipStream, string processKey, ProcessChange changedDG)
        {
            using (FileStream stream = File.Create(filePath))
            {
                byte[] buffer = new byte[2048];
                while (true)
                {
                    int size = zipStream.Read(buffer, 0, buffer.Length);
                    if (size > 0)
                    {
                        stream.Write(buffer, 0, size);
                    }
                    else
                    {
                        break;
                    }
                }
                if (changedDG != null)
                    changedDG(AddOneAndReport(processKey));
            }
        }
        private  double AddOneAndReport(string processItemKey)
        {
            return Math.Round(((++ProcessItems[processItemKey].HadHandleCount)
                                / ProcessItems[processItemKey].NeedHandleCount) * 100, 2);
        }
        /// <summary>
        /// 获取压缩文件中含有的文件数量
        /// </summary>
        /// <param name="zipPath"></param>
        /// <returns></returns>
        public  int GetZipFileCount(string zipPath)
        {
            int result = 0;
            try
            {
                using (ZipInputStream inputStream = new ZipInputStream(File.OpenRead(zipPath)))
                {
                    ZipEntry currentEntry = null;
                    //inputStream流自动变成对于该Entry的内容
                    while ((currentEntry = inputStream.GetNextEntry()) != null)
                    {
                        foreach (Match match in Regex.Matches(currentEntry.Name, @"[^/]+$"))
                        {
                            result++;
                        }
                    }
                }
            }
            catch
            {

            }
            return result;
        }

        /// <summary>
        /// 检查文件是否已存在Zip中
        /// </summary>
        /// <param name="zipPath">zip文件路径</param>
        /// <param name="filename">文件名</param>
        /// <returns></returns>
        public bool IsExistFileInZip(string zipPath,string filename)
        {
            try
            {
                using (ZipInputStream inputStream = new ZipInputStream(File.OpenRead(zipPath)))
                {
                    ZipEntry currentEntry = null;
                    //inputStream流自动变成对于该Entry的内容
                    while ((currentEntry = inputStream.GetNextEntry()) != null)
                    {
                       if(currentEntry.Name==filename)
                       {
                           return true;
                       }
                    }
                }
            }
            catch(Exception e)
            {
                throw e; 
            }
            return false;
        }
        #endregion
    }
}
