# CL.IO.Zip
CL.IO.Zip 是一个基于SharpZipLib的一个压缩和解压的类库，提供给用户在.net环境下使用（VB.NET,C#..等等）当前最新版本为：V1.1.0 

做任何操作之前，请使用单例模式，获取ZipHandler对象。

  <code>ZipHandler handler = ZipHandler.GetInstance();</code>


##1.压缩

### 1.1压缩整个文件夹，并且得到压缩进度。
#### Method：PackDirectory (class:ZipHandler)
  <code>public static void PackFileDirectory(string strDirectory, string zipedFile, PackProcessChange changedDG)</code>

### Use:
  <code>var fromDic="The directory path you want to zip";</code>
  
  <code>var toZip="Target zipFile Path";</code>
  
  <code>ZipHandler.PageFileDirectory(fromDic, toZip, (num) => { Debug.WriteLine("pack num:" + num); }); 
  </code>
  
  
## UnPackZip
###Statement:(class:ZipHandler)
  <code>public static void UnpackFiles(string zipFilePath, string unZipFile, UnPackProcessChange changedDG)</code>

###Use:
  <code>var fromZip="The zipFile path";</code>
  <code>var toDic="Target directory Path";</code>
  <code>ZipHandler.UnpackFiles(fromZip, toDic, (num) => { Debug.WriteLine("unpack num:" + num); });</code>
