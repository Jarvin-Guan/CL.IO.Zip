# CL.IO.Zip
CL.IO.Zip is a Zip library written in C# for the .NET platform. it provide process delegate.

## PackDirectory
###Statement:(class:ZipHandler)
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
