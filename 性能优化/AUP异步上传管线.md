# AUP异步上传管线

Async Upload Pipeline

[优化加载性能：了解异步上传管线AUP (qq.com)](https://mp.weixin.qq.com/s?__biz=MzkyMTM5Mjg3NQ==&mid=2247535763&idx=1&sn=dda4a1f7fc62e0a23aeef3f48af9d9b8&source=41#wechat_redirect)

#### 同步与异步

在同步上传管线中，Unity必须在单个帧中同时加载纹理或网格的元数据、纹理的每个Texel或网格的每个顶点数据。而在异步上传管线中，Unity则在单个帧中**仅加载元数据**，并在后续帧中将二进制数据流式传输到GPU。

##### 同步上传管线

在项目构建时，Unity会将同步加载的网格或纹理的标头数据和二进制数据都写入**同一.res文件**（res即Resource）。在运行时，当程序同步加载纹理或网格时，Unity将该纹理或网格的标头数据和二进制数据从.res文件（磁盘中）加载到内存（RAM）中。当所有数据都位于内存中时，Unity随后将二进制数据上传到GPU（Draw Call前）。**加载和上传操作都发生在主线程上的单个帧中**。

##### 异步上传管线

在项目构建时，Unity会将标头数据写入到一个.res文件，而将二进制数据写入到另一个.resS文件（S应该指Streaming）。在运行时，当程序异步加载纹理或网格时，Unity将标头数据从.res文件（磁盘中）加载到内存（RAM）中。当标头数据位于内存中时，Unity随后使用**固定大小的环形缓冲区**（一块可配置大小的缓冲区）将二进制数据从.resS文件（磁盘中）流式传输到GPU。**Unity使用多个线程通过几帧流式传输二进制数据。**

注意：使用AUP时，AB包必须是LZ4压缩。

在构建过程中，纹理或网格对象会写入序列化文件，大型二进制数据的纹理或顶点数据会写入附带的.resS文件，这样的配置应用于玩家数据和资源包。

AUP可以加载纹理和网格，但可读写纹理、可读写网格和压缩网格都不适用于AUP。

AUP对每个指令会执行以下过程：

1. 等待环形缓冲区中所需内存可用。
2. 从源.resS文件中读取数据到分配的内存。
3. 执行后期处理过程，例如：纹理解压、网格碰撞生成、每个平台的修复等。
4. 以时间切片的方式在渲染线程进行上传。
5. 释放环形缓冲区内存。

#### 参数

##### 时间片

QualitySettings.asyncUploadTimeSlice

设定渲染线程中每帧上传纹理和网格数据所用的时间总量，以毫秒为单位。

当异步加载操作进行时，该系统会执行二个该参数大小的时间切片，该参数的默认值为2毫秒。

如果该值太小，可能会在纹理/网格的GPU上传时遇到瓶颈。而该值太大的话，会造成帧率陡降。

如果上传时间设定的太长，那么留给渲染的时间就会变少。

##### 缓冲区大小

QualitySettings.asyncUploadBufferSize

该参数设定环形缓冲区的大小，以MB为单位。当上传时间切片在每帧发生时，要确保在环形缓冲区有足够的数据利用整个时间切片。

如果环形缓冲区太小，上传时间切片会被缩短。

当纹理大小超过缓冲区大小时，会先消耗完缓冲区剩余大小，再重新分配至纹理所需大小，待上传完成后缓冲区再调整至设置大小。

##### QualitySettings.asyncUploadPersistentBuffer

它决定在完成所有待定读取工作时，是否释放上传时使用的环形缓冲区。

分配和释放该缓冲区经常会产生内存碎片，因此通常将其保留为默认值True。如果需要在未加载时回收内存，可以将该值设为False。

#### 建议

- 选择不会导致掉帧的最大QualitySettings.asyncUploadTimeSlice。
- 在加载界面时，可以临时提高QualitySettings.asyncUploadTimeSlice。
- 使用性能分析器来检查时间切片的利用率。时间切片在性能分析器中会显示为AsyncUploadManager.AsyncResourceUpload。如果时间切片没有完全利用的话，就提高QualitySettings.asyncUploadBufferSize。
- 使用更大的QualitySettings.asyncUploadBufferSize会提高加载速度，所以如果内存足够的话，请将其从16MB提高至32MB。
- 将QualitySettings.asyncUploadPersistentBuffer保留为true，除非有理由在未加载时减少运行时内存的使用。