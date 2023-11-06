# Graphics

## Texture

### Blit(source,dest,mat)

使用着色器拷贝原纹理到RenderTexture

### CopyTexture(sre,dst)

复制纹理

可对区域拷贝，无法改变分辨率

## Mesh

使用Graphics绘制的Mesh无需创建GameObject，因此没有相关挂载Mesh Renderer的组件。

### DrawMeshInstanced()

- 一帧内最多绘制1023个网格，
- 实例化失败会抛出错误，不会影响性能

#### 缺点

- 实例化移动的时候需要执行大量的循环语句，导致性能下降
-  没有Unity自带的视锥体剔除、LOD以及遮罩剔除等优化，会导致面数增加需要额外的处理。

### DrawMeshInstancedIndirect()

与`DrawMeshInstanced`类似，区别在于`DrawMeshInstancedIndirect`参数来自于`ComputeBuffer`或`GraphicsBuffer`。

需要先把参数传递给ComputeBuffer，ComputeBuffer传递给Shader。也可以传递给ComputeShader通过计算后Draw到渲染管线中。

