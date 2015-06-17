# CL.IO.Zip
	
	
####&nbsp;&nbsp;&nbsp;&nbsp;CL.IO.Zip 是一个基于SharpZipLib的一个压缩和解压的类库，提供给用户在.net环境下使用（VB.NET,C#..等等）当前最新版本为：V1.1.0 
百度网盘下载地址：http://pan.baidu.com/s/1dDnQuml#path=%252F

<br>&nbsp;&nbsp;&nbsp;&nbsp;做任何操作之前，请使用单例模式，获取ZipHandler对象。

&nbsp;&nbsp;&nbsp;&nbsp;<code>ZipHandler handler = ZipHandler.GetInstance();</code>


##1.压缩

### &nbsp;&nbsp;&nbsp;&nbsp;1.1压缩文件夹，并获取压缩进度。
#### Method：PackDirectory 
   &nbsp;&nbsp;&nbsp;&nbsp;<code>public void PackDirectory(string strDirectory, string zipedFile, ProcessChange changedDG)</code>

##### Demo
  &nbsp;&nbsp;&nbsp;&nbsp;<code>var fromDic="E:\\ZipTest";\\\需要压缩的文件夹路径</code>
  
  &nbsp;&nbsp;&nbsp;&nbsp;<code>var toZip="E:\\ZipFile.zip";\\\生成压缩包的目标路径</code>
  
  &nbsp;&nbsp;&nbsp;&nbsp;<code>handler.PackDirectory(fromDic, toZip, (num) => { Debug.WriteLine("pack num:" + num); });\\\\num为百分比，最大为100，可在此处写处理逻辑
  </code>
  
  
### &nbsp;&nbsp;&nbsp;&nbsp;1.2添加文件到zip文件中。
#### Method：AddFile
   &nbsp;&nbsp;&nbsp;&nbsp;<code>public  void AddFile(string filePath, string zipPath,string filePathInZip)</code>

##### Demo
  &nbsp;&nbsp;&nbsp;&nbsp;<code>var filePath="E:\\ReadyToAdd.txt";\\\\需要添加到压缩包的文件路径</code>
  
  &nbsp;&nbsp;&nbsp;&nbsp;<code>var zipPath="E:\\ZipFile.zip";\\\压缩包文件路径</code>
  
  &nbsp;&nbsp;&nbsp;&nbsp;<code>handler.AddFile(filePath, zipPath, @"123\" + Path.GetFileName(filePath));\\\需要添加到压缩包的文件路径
  </code>

### &nbsp;&nbsp;&nbsp;&nbsp;1.3添加文件夹到zip文件中(此函数支持是否保存文件路径的格式。)
#### Method：AddDirectory 
   &nbsp;&nbsp;&nbsp;&nbsp;<code>public void AddDirectory(string dicPath,string zipPath,string dicPathInZip,ProcessChange changedDG)</code>

##### Demo
  &nbsp;&nbsp;&nbsp;&nbsp;<code>var dicPath="E:\\ReadyToAddDic";\\\\需要添加到压缩包的文件夹路径</code>
  
  &nbsp;&nbsp;&nbsp;&nbsp;<code>var zipPath="E:\\ZipFile.zip";\\\压缩包文件路径</code>
  
  &nbsp;&nbsp;&nbsp;&nbsp;<code>var dicPathInZip="ReadyToAddDic";\\\需要压缩到压缩包内的相对路径，当前值指的是根目录的ReadyToAddDic</code>
  
  &nbsp;&nbsp;&nbsp;&nbsp;<code>handler.IsKeepPath=true;\\\保存原路径</code>
  
  &nbsp;&nbsp;&nbsp;&nbsp;<code>handler.AddDirectory(dicPath, zipPath, dicPathInZip,(num) => { Debug.WriteLine("压缩进度:" + num); });
  </code>
  
  
##2.解压

### &nbsp;&nbsp;&nbsp;&nbsp;2.1对压缩包进行解压。
#### Method：UnpackAll
   &nbsp;&nbsp;&nbsp;&nbsp;<code> public  void UnpackAll(string zipFilePath, string unzipPath, ProcessChange changedDG)</code>

##### Demo
  &nbsp;&nbsp;&nbsp;&nbsp;<code>var fromZip="E:\\ZipTest.zip";\\\需要解压的压缩文件路径</code>
  
  &nbsp;&nbsp;&nbsp;&nbsp;<code>var toDic="E:\\ZipFile";\\\解压到的文件夹路径</code>
  
  &nbsp;&nbsp;&nbsp;&nbsp;<code>handler.UnpackAll(fromZip, toDic, (num) => { Debug.WriteLine("解压进度:" + num); });  </code>
  
  
### &nbsp;&nbsp;&nbsp;&nbsp;2.2解压压缩包内的指定文件。
#### Method：UnpackFile
   &nbsp;&nbsp;&nbsp;&nbsp;<code>public void UnpackFile(string zipFilePath,string unzipPath,string filePathInZip)</code>

##### Demo
  &nbsp;&nbsp;&nbsp;&nbsp;<code>var fromZip="E:\\ZipFile.zip";</code>
  
  &nbsp;&nbsp;&nbsp;&nbsp;<code>var toDic="E:\\UnZipTest";</code>
  
  &nbsp;&nbsp;&nbsp;&nbsp;<code>handler.UnpackFile(fromZip, toDic, @"models/db.js");
  </code>

### &nbsp;&nbsp;&nbsp;&nbsp;2.3解压压缩包内的指定文件夹。
#### Method：UnpackDirectory
   &nbsp;&nbsp;&nbsp;&nbsp;<code>public void UnpackDirectory(string zipFilePath, string unzipPath, string DicPathInZip)</code>

##### Demo
  &nbsp;&nbsp;&nbsp;&nbsp;<code>var fromZip="E:\\ZipFile.zip";\\\压缩包文件路径</code>

  &nbsp;&nbsp;&nbsp;&nbsp;<code>var toDic="E:\\UnZipTest";</code>
  
  &nbsp;&nbsp;&nbsp;&nbsp;<code>var dicPathInZip="node_modules";
  </code>
  
  &nbsp;&nbsp;&nbsp;&nbsp;<code>handler.IsKeepPath=true;\\\保存原路径</code>
  
  &nbsp;&nbsp;&nbsp;&nbsp;<code>handler.UnpackDirectory(fromZip, toDic, dicPathInZip);</code>
