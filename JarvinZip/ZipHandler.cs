using ICSharpCode.SharpZipLib.Checksums;
using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace JarvinZip
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

        public delegate void PackProcessChange(double percent);
        public delegate void UnPackProcessChange(double percent);
        private  Dictionary<string, ProcessItem> ProcessItems = new Dictionary<string, ProcessItem>();

        private bool _isKeepPath = true;
        /// <summary>
        /// 压缩、解压的过程中是否保持路径
        /// </summary>
        public bool IsKeepPath { set { _isKeepPath = value; } get { return _isKeepPath; } }

        #region 压缩
        /// <summary>
        /// 压缩单个文件
        /// </summary>
        /// <param name="fileToZip">要进行压缩的文件名</param>
        /// <param name="zipedFile">压缩后生成的压缩文件名</param>
        public  void PackFile(string fileToZip, string zipedFile)
        {
            //如果文件没有找到，则报错
            if (!File.Exists(fileToZip))
            {
                throw new System.IO.FileNotFoundException("指定要压缩的文件: " + fileToZip + " 不存在!");
            }

            using (FileStream fs = File.OpenRead(fileToZip))
            {
                byte[] buffer = new byte[fs.Length];
                fs.Read(buffer, 0, buffer.Length);
                fs.Close();

                using (FileStream ZipFile = File.Create(zipedFile))
                {
                    using (ZipOutputStream ZipStream = new ZipOutputStream(ZipFile))
                    {
                        string fileName = fileToZip.Substring(fileToZip.LastIndexOf("\\") + 1);
                        ZipEntry ZipEntry = new ZipEntry(fileName);
                        ZipStream.PutNextEntry(ZipEntry);
                        ZipStream.SetLevel(5);
                        ZipStream.Write(buffer, 0, buffer.Length);
                        ZipStream.Finish();
                        ZipStream.Close();
                    }
                }
            }
        }

        /// <summary>
        /// pack directory
        /// </summary>
        /// <param name="strDirectory">The directory path you want to zip</param>
        /// <param name="zipedFile">Target zipFile Path</param>
        /// <param name="changedDG">report process delegate</param>

        public  void PackFileDirectory(string strDirectory, string zipedFile, PackProcessChange changedDG)
        {
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
            string processItemKey,string zipPath, PackProcessChange changedDG)
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
        /// <param name="unZipFile">to Directory</param>
        /// <param name="changedDG">report process delegate</param>
        public  void UnpackFiles(string zipFilePath, string unZipFile, UnPackProcessChange changedDG)
        {
            //总需要压缩的文件数量
            double totalCount = GetZipFileSystemCount(zipFilePath);
            string key = System.Guid.NewGuid().ToString(); //Guid Key
            ProcessItems.Add(key, new ProcessItem(totalCount));

            #region unzip
            if (unZipFile[unZipFile.Length - 1] != Path.DirectorySeparatorChar)
            {
                unZipFile = unZipFile + Path.DirectorySeparatorChar;
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
                        if (!Directory.Exists(unZipFile + directoryName))
                        {
                            Directory.CreateDirectory(unZipFile + directoryName);
                        }
                    }
                    if (!string.IsNullOrEmpty(fileName))
                    {
                        if (zipEntry.CompressedSize == 0)
                            break;
                        if (zipEntry.IsDirectory)
                        {
                            directoryName = Path.GetDirectoryName(unZipFile + zipEntry.Name);
                        }
                        string zipFileName = !IsKeepPath ? Path.GetFileName(zipEntry.Name) : zipEntry.Name;
                        using (FileStream stream = File.Create(unZipFile + zipFileName))
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
                            changedDG(AddOneAndReport(key));
                        }
                    }
                }
            }
            #endregion

            ProcessItems.Remove(key);
        }
        #endregion

        #region 工具
        private  double AddOneAndReport(string processItemKey)
        {
            return Math.Round(((++ProcessItems[processItemKey].HadHandleCount)
                                / ProcessItems[processItemKey].NeedHandleCount) * 100, 2);
        }
        /// <summary>
        /// 获取压缩文件中含有的文件系统
        /// </summary>
        /// <param name="zipPath"></param>
        /// <returns></returns>
        public  int GetZipFileSystemCount(string zipPath)
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
