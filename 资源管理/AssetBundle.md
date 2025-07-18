# AssetBundle

## 一个AssetBundle有什么？

- 一个序列化文件，标识符、是否压缩和清单(manifest)数据。
  - manifest数据是由对象的名称为key的查找表(lookup table)，每个条目都提供了一个字节的索引来标识对象在AssetBundle数据段的位置
- 资源文件，二进制数据块，支持异步加载
  - 所有资源都被序列化，如果选择压缩则将序列化数据压缩。

## AssetBundle的压缩方式

### LZMA

基于文件流

包体小，解压时间长

因为使用流的方式，所以使用时必须加载整包，解压后会被重新压缩成LZ4

### LZ4

基于块(Chunk)的算法，读取文件时只需要解压文件所在的块数据

加载速度和不加载速度差不多，优势在于在内存中的占用变小了

会比LZMA的包体大25%，可以使用其他压缩手段进行二次压缩，包体也可以被压得很小。

### 差异更新

可以通过包体的差异更新，在热更新的时候通过算法比对出新增内容再进行更新。因为是基于Chunk的算法，所以比对出来的差异文件会比较小，因为数据相对连续。

差异更新一般分为两种

- 基于源文件(安装包)
  - 成功率高，算法简单
  - 但在移动端保存所有源文件，在更新文件整合后再编译不现实。
- 基于二进制
  - 也是最常用的差异比对方式，也是大多数PC游戏的做法。算是很成熟的技术了
  - 可以和AB包配合，打出更小的热更包

#### Bsdiff ( 基于二进制的比对算法 )

二进制比对生成差异化文件

比对两个版本文件的二进制数据，然后生成一个patch文件，再将这个patch打到旧文件就可以生成新文件。也就是游戏中的更新补丁包。

缺点：需要考虑二进制效率，生成的差异化文件可能会比小包情况大一些。

##### AssetBundle压缩算法不同导致的差异化文件差异

**在`LZ4`和`LZMA`两种压缩方式中，生成的差异化文件大小也是不同的。因为前面提到的`AssetBundle`的压缩算法都是先序列化再压缩。这就导致了`LZMA`的压缩算法可能导致一个`Asset`的数据分散开导致差异化文件过大，而`LZ4`压缩则是按块压缩数据相对比较集中。所以`LZ4`生成的差异化文件都比`LZMA`小。**

`Uncompressed`(不压缩)的方式则差异不大

#####  差异化文件(补丁)合并

在合并差异化文件过程中，至少会进行一次文件复制，所以考虑到合并造成的IO消耗，所以单个包的大小还是需要限制一下。

在patch过程中，可能会造成界面卡顿，所以最好在其他线程中处理。

### 补丁文件的版本规划

保持上个版本的全部文件，然后和新版本的文件做一次diff操作，然后把diff和新版本文件上传CDN。

如果有玩家多个版本没有更新，那么就下载多个版本的diff进行多次合并。如果跨越的版本数量过多，也可以中间插几个大版本的补丁包来减少补丁数量。

## AssetBundle依赖项

当`Asset`被打进`AssetBundle包`时，其他`AssetBundle包`依赖这个`Asset`时会变成依赖这个`AssetBundle包`。

当`Asset`没有打进`AssetBundle包`，在构建`AssetBundle`时如果有依赖于这个`Asset`则会将它打到一起。如果有多个`AssetBundle`依赖于这个`Asset`就会被打入多份，造成冗余。

如果`AssetBundle`内有依赖对象，在使用时需要先加载依赖的`AssetBundle`，Unity不会自动加载依赖项。需要自己根据`AssetBundleManifest`中加载依赖项。

**当精灵图被依赖时，对应的精灵图集也会被打到AssetBundle中，容易造成图集冗余**

## AssetBundle之间的重复信息

公共的资源最好打入一个`AssetBundle包`中，否则公共资源可能会被构建在多个`AssetBundle包`中。从而造成资源冗余，安装包的大小变大，运行时的内存占用量变大。

**一个公共材质如果被打入到AssetBundle中会被视为副本，Unity将同一个材质的每个副本都视为独特材质。如果材质被打入到多个AssetBundle中，每个生成的AssetBundle都会包含此材质(包括其着色器和引用纹理)。Unity将同一个材质的每个副本都视为独特材质，所以还会影响到批处理。**

所以在构建时`材质`和`引用的资源`尽量打在同一个AssetBundle中。还可以仅标记材质，因为纹理依赖关系也会被包含在AssetBundle中。

## AssetBundle的加载

### AssetBundle.LoadFromMemoryAsync

```c#
AssetBundleCreateRequest createRequest = AssetBundle.LoadFromMemoryAsync(File.ReadAllBytes(path));
```

按字节数组加载AB包。LZMA会在加载时解压，LZ4则会以压缩状态加载。

### AssetBundle.LoadFromFile

```c#
var myLoadedAssetBundle = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, "myassetBundle"));
```

从本地存储中加载AB包。未压缩或LZ4直接从磁盘加载AB包，LZMA则先解压再加载到内存中。

### 从AssetBundle中加载资源

```c#
//加载指定资源
T objectFromBundle = bundleObject.LoadAsset<T>(assetName);
//加载全部资源
Unity.Object[] objectArray = loadedAssetBundle.LoadAllAssets();
//加载依赖清单文件
AssetBundleManifest manifest = assetBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
```

## AssetBundle的卸载

### AssetBundle.Unload(true)

强制卸载掉所有AssetBundle中加载的Asset，包括AssetBundle的映射结构，和从AssetBundle创建出来的所有资源。释放AssetBundle实例。

**会导致正在使用的资源丢失**，需要一套自己的机制(引用计数)来关注是否有正在使用的资源。

### AssetBundle.Unload(false)

AssetBundle内的序列化数据会被释放，正在使用的资源还保持完好。等于断开了AssetBundle和实例的联系。再次使用这个AB包时，则会重新实例化一个新的，旧的就引用不到了。就造成了重复资源的冗余。

### Resources.UnloadUnusedAssets

卸载掉没有使用的Assets，作用范围是整个系统。

作用方式类似于GC，会遍历所有资源进行引用查询，会阻塞线程。

### 小结

`AssetBundle.Unload(false)`更适用于一次性使用的资源，卸载后，在特定时机触发`Resources.UnloadUnusedAssets`就能卸载干净了。

`AssetBundle.Unload(true)`使用时最好添加引用计数作为保护。或在应用程序生命周期中具有明确定义的卸载瞬态 AssetBundle 的时间点，例如在关卡之间或在加载屏幕期间。

## AssetBundle变体

在Android 生态中有些设备不支持ETC2的纹理只支持ETC1的纹理，那么可以构建AssetBundle变体。使用ETC1支持的资源格式来构建AssetBundle变体，构建足够多的变体来支持Android 生态中不支持ETC2的设备。