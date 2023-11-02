# SpriteRenderer优化

- [ ] SpriteRender 替换成MeshRender
  - [ ] 静态图片 工具统一生成mesh
  - [ ] 可能还需要一个mesh 的管理器
- [ ] 帧动画原本使用SpriteRender的也改为MeshRender
  - [ ] 因为SpriteRender在Simple模式下只需要设置Sprite，底层会自动根据sprite修改GameObject尺寸。导致帧动画播放时会一直修改顶点信息。
  - [ ] 所以需要修改成MeshRender并且只使用同一份mesh，在播放帧动画的适合通过修改uv的缩放+offest来使uv显示正确。
  - [ ] 使用`Graphics.DrawMeshInstancedIndirect`渲染mesh，用gpu instancing

## SpriteRenderer

使用精灵图的`Mesh`进行uv的渲染

通过Z轴(相对相机的位置)的顺序来渲染精灵图。

可以使用精灵遮罩

可以进行九宫格切片

Draw Mode

- Simple
  - 不能使用九宫格
  - `SpriteRenderer`会跟随sprite内容做自适应大小。
- Sliced
  - 可以使用九宫格，通过九宫格缩放uv尺寸
  - 不随sprite内容自适应。
- Tiled
  - 使用九宫格的中间部分，在尺寸变化时进行平铺。

## MeshRenderer

一般和`Mesh Fiter`搭配，`Mesh Fiter`负责展示具体的`Mesh`。`MeshRenderer`负责决定怎么渲染这个`Mesh`。

相比于`SpriteRenderer`基本上顶点数量更多，因为必须持有一个mesh。但是在片元着色时相比于`SpriteRenderer`消耗更小，因为减少了Overdraw的行程，`MeshRender`使用的`Mesh`可以更贴合uv的大小，从而避免过多半透像素的渲染。

### 优化

在播放大量帧动画时，一些地方也可以使用MeshRenderer，使用相同的`Mesh`设置帧动画uv。比如播放一堆士兵进行战斗的画面时，每个soldier动画组件都是同一个`mesh`，播放的帧动画打包在同一个图集中，保证动画材质的相同，可以使用`GPU Instancing`来进行渲染上的优化。

