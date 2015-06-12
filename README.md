# JarvinZipLib
JarvinZipLib is a Zip library written in C# for the .NET platform. it provide process delegate.

# PackDirectory
Statement:
  /// <summary>
  /// pack directory
  /// </summary>
  /// <param name="strDirectory">The directory path you want to zip</param>
  /// <param name="zipedFile">Target zipFile Path</param>
  /// <param name="changedDG">report process delegate</param>
  public static void PackFileDirectory(string strDirectory, string zipedFile, PackProcessChange changedDG)

Use:
  var fromDic="The directory path you want to zip";
  var toZip="Target zipFile Path";
  ZipHandler.PageFileDirectory(fromDic, toZip, (num) => { Debug.WriteLine("pack num:" + num); }); 
  
  
# UnPackZip
Statement:
  /// <summary>
  /// unPack zipFile
  /// </summary>
  /// <param name="zipFilePath">the path of zipFile</param>
  /// <param name="unZipFile">to Directory</param>
  /// <param name="changedDG">report process delegate</param>
  public static void UnpackFiles(string zipFilePath, string unZipFile, UnPackProcessChange changedDG)

Use:
  var fromZip="The zipFile path";
  var toDic="Target directory Path";
  ZipHandler.UnpackFiles(fromZip, toDic, (num) => { Debug.WriteLine("unpack num:" + num); });
