# SpriteRenderer优化

## 优化步骤

1. 设置一个统一的网格，使用MeshRenderer替换SpriteRenderer
2. 将小图打成图集，记录padding和uv(在图集中的位置)
3. 使用DrawMeshInstancedIndirect渲染Mesh，传入缩放值和偏移值
4. 在顶点着色器中进行缩放、偏移、uv值的设置

- [ ] SpriteRender 替换成MeshRender
  - [ ] 静态图片 工具统一生成mesh
  - [ ] 可能还需要一个mesh 的管理器
- [ ] 帧动画原本使用SpriteRender的也改为MeshRender
  - [ ] 因为SpriteRender在Simple模式下只需要设置Sprite，底层会自动根据sprite修改GameObject尺寸。导致帧动画播放时会一直修改顶点信息。
  - [ ] 所以需要修改成MeshRender并且只使用同一份mesh，在播放帧动画的适合通过修改uv的缩放+offest来使uv显示正确。
  - [ ] 使用`Graphics.DrawMeshInstancedIndirect`渲染mesh，用gpu instancing

## SpriteRenderer

使用精灵图的`Mesh`进行uv的渲染

- 如果是alpha通道分离的图集，那么无法通过sprite图构建出mesh
- 如果是带alpha通道的图集，那么会在赋值时底层通过sprite图的透明信息构建出mesh
  - 如果是帧动画播放，每一帧都会渲染不同的mesh造成消耗。

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

相比于`SpriteRenderer`需要多持有一个mesh。但是在片元着色时相比于`SpriteRenderer`消耗更小，因为减少了Overdraw的行程，`MeshRender`使用的`Mesh`可以更贴合uv的大小，从而避免过多半透像素的渲染。

### SpriteRenderer使用GPUInstance优化

在播放大量帧动画时，一些地方也可以使用MeshRenderer，使用相同的`Mesh`设置帧动画uv。比如播放一堆士兵进行战斗的画面时，每个soldier动画组件都是同一个`mesh`，播放的帧动画打包在同一个图集中，保证动画材质的相同，可以使用`GPU Instancing`来进行渲染上的优化。

#### 注意事项

##### TextureImportSetting

###### MeshType

Sprite图集  MeshType需要为FullRect，这样使用可以使用通用的平面网格，不需要变更uv坐标，缺点就是透明图会增加OverrDraw

如果大量静态图，那么可以把MeshType设置为Tight，然后再用工具导出Sprite图的网格，这样应该是最友好的方式。

如果是动态图类似于帧动画这种，还是FullRect使用平面网格进行设置最为方便。因为如果通过工具来匹配所有图集内精灵图都适用的网格，再设置uv坐标再把数据记录起来，还需要对数据进行一个统一的管理反而麻烦许多。

###### Pixels Per Unit

一个unity单位将显示多少个像素该问题。

比如设置为100，那么128 x 128的纹理 / 100 那么就需要1.28个unity单位才能显示完。

当需要计算出Sprite的Mesh数据时，需要除以 Pixels Per Unit才能得到Unity单位。Mesh都是Unity单位。

### 网格优化

网格内存 由 Positon、Normal、Tangent、Color、uv1234 组成。

#### Vertex Compression

**每个通道使用 32bit 浮点数保存，如果勾选Vertex Compression则使用16 bit 浮点数保存。**Color 比较特殊，会始终以 4byte 存储。

默认设置的情况下，Position 和 uv1 是不开启的，因为前者需要用于存顶点坐标，后者用于存光照贴图（尺寸往往很大）uv，这两者往往需要更高精度，可结合项目要求决定要不要压缩。

**该选项生效要求网格的前提还有网格不能是动态合批的、不能开启Read/Write、不能开启 Mesh Compression**，此外实际测试发现对蒙皮网格的切线、法线不起作用，但是其他通道有作用，可能是因为这些通道需要参与蒙皮过程。

```c#
//一般2d游戏设置position和uv就可以了，因为也不需要法线和切线
var layout = new[]
{
	new VertexAttributeDescriptor(VertexAttribute.Position, VertexAttributeFormat.Float16,2),
	new VertexAttributeDescriptor(VertexAttribute.TexCoord0,VertexAttributeFormat.Float16,2),
};
var vts = mesh.vertices;
mesh.SetVertexBufferParams(vts.Length,layout);
```

## 测试结果