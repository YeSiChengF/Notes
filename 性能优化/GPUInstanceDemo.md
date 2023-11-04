# GPUInstanceDemo

## DrawMeshInstanced()

- 一帧内最多绘制1023个网格，
- 实例化失败会抛出错误，不会影响性能
- 缺点：实例化移动的时候需要执行大量的循环语句，导致性能下降

## DrawMeshInstancedIndirect()

## ComputeShader

图形API中的一种可编程着色器，独立于渲染管线之外，但是可以对GPU资源（存放在显存中）进行读取和写入操作。

通过GPU进行多线程的计算，并且可以将`ComputeShader`的计算结果作为渲染管线的输入。

文件扩展名为`.compute`

### kernel

一个CS中至少要有一个kernel才能被唤醒  

\`#pragma kernel xxxx`

### numthreads

定义一个线程组（Thread Group）中可以被执行的线程（Thread）总数量。

`[numthreads(8,8,1)]` 线程的数量=tX * tY * tZ

**每个核函数前面我们都需要定义numthreads**，否则编译会报错。

每个线程组都有一个各自的共享内存，不能访问别的组对应的共享内存。

先用`numthreads`定义好核函数对应线程组里的线程数量，再用Dispatch定义用多少线程组(gX\*gY\*gZ)来处理这个核函数。

| 参数                | 值类型 | 含义                                                         | 计算公式                                                     |
| ------------------- | ------ | ------------------------------------------------------------ | ------------------------------------------------------------ |
| SV_GroupID          | int3   | 当前线程所在的线程组的ID，取值范围为(0,0,0)到(gX-1,gY-1,gZ-1)。 | 无                                                           |
| SV_GroupThreadID    | int3   | 当前线程在所在线程组内的ID，取值范围为(0,0,0)到(tX-1,tY-1,tZ-1)。 | 无                                                           |
| SV_DispatchThreadID | int3   | 当前线程在所有线程组中的所有线程里的ID，取值范围为(0,0,0)到(gX*tX-1, gY*tY-1, gZ*tZ-1)。 | 假设该线程的SV_GroupID=(a, b, c)，SV_GroupThreadID=(i, j, k) 那么SV_DispatchThreadID=(a*tX+i, b*tY+j, c*tZ+k) |
| SV_GroupIndex       | int    | 当前线程在所在线程组内的下标，取值范围为0到tX*tY*tZ-1。      | 假设该线程的SV_GroupThreadID=(i, j, k) 那么SV_GroupIndex=k*tX*tY+j*tX+i |

### c#部分

以往的`Vertex shader`和` Fragment shade`r我们都是给它关联到`Material`上来使用的，但是CS不一样，它是**由c#来驱动**的。

### ComputeBuffer

用来把CPU中自定义的Struct数据读写到显存中。在C#中创建并填充它，然后传递给`ComputeShader`或者其他的Shader中。

#### 定义

```c#
ComputeBuffer buffer = new ComputeBuffer(int count,int stride)
    
//使用完Release掉
buffer.Release();
```

count代表buffer中元素的数量，而stride指每个元素占用的字节。

**ComputeBuffer中的stride大小必须和RWStructuredBuffer中的每个元素大小一致。**

#### 填充

```c#
//参数为自定义的struct数组
buffer.SetData(T[]);
```

#### 传递到CS

```c#
public void SetBuffer(int kernelIndex, string name, ComputeBuffer buffer)
```

#### ComputeBufferType

| Default           | ComputeBuffer的默认类型，对应HLSL shader中的StructuredBuffer或RWStructuredBuffer，常用于自定义Struct的Buffer传递。 |
| ----------------- | ------------------------------------------------------------ |
| Raw               | Byte Address Buffer，把里面的内容（byte）做偏移，可用于寻址。它对应HLSL shader中的ByteAddressBuffer或RWByteAddressBuffer，用于着色器访问的底层DX11格式为无类型的R32。 |
| Append            | Append and Consume Buffer，允许我们像处理Stack一样处理Buffer，例如动态添加和删除元素。它对应HLSL shader中的AppendStructuredBuffer或ConsumeStructuredBuffer。 |
| Counter           | 用作计数器，可以为RWStructuredBuffer添加一个计数器，然后在ComputeShader中使用IncrementCounter或DecrementCounter方法来增加或减少计数器的值。由于Metal和Vulkan平台没有原生的计数器，因此我们需要一个额外的小buffer用来做计数器。 |
| Constant          | constant buffer (uniform buffer)，该buffer可以被当做Shader.SetConstantBuffer和Material.SetConstantBuffer中的参数。如果想要绑定一个structured buffer那么还需要添加ComputeBufferType.Structured，但是在有些平台（例如DX11）不支持一个buffer即是constant又是structured的。 |
| Structured        | 如果没有使用其他的ComputeBufferType那么等价于Default。       |
| IndirectArguments | 被用作 Graphics.DrawProceduralIndirect，ComputeShader.DispatchIndirect或Graphics.DrawMeshInstancedIndirect这些方法的参数。buffer大小至少要12字节，DX11底层UAV为R32_UINT，SRV为无类型的R32。 |

### UAV（Unordered Access view）

在渲染Shader中使用的资源称为SRV（Shader resource view），比如Texture2D是只读的。但是在ComputeShader中，往往需要写入操作，因此SRV不能满足需求。

UAV允许多个线程临时的无序读/写操作，不会产生内存冲突。

RWTexture、RWStructuredBuffer都属于UAV数据类型，**支持在读取的同时写入**。只能在FragmentShader和ComputeShader中使用。

如果RenderTexture不设置`enableRandomWrite`，当传递一个renderTexture给RWTexture时就会报错。

`the texture wasn't created with the UAV usage flag set!`

### groupshared

每个线程组都有一块属于自己的内存空间，通过`groupshared`关键字定义的变量会被存放在线程组内的共享内存中

```
groupshared float4 vec;
```

共享内存支持的最大大小为32kb，单个线程最多支持对共享内存进行256byte的写入操作。

```
Texture2D input;
groupshared float4 cache[256];

[numthreads(256, 1, 1)]
void CS(int3 groupThreadID : SV_GroupThreadID, int3 dispatchThreadID : SV_DispatchThreadID)
{
    cache[groupThreadID.x] = input[dispatchThreadID.xy];

    GroupMemoryBarrierWithGroupSync();

    float4 left = cache[groupThreadID.x - 1];
    float4 right = cache[groupThreadID.x + 1];
    ......
}
```

#### GroupMemoryBarrierWithGroupSync

GroupMemoryBarrierWithGroupSync 同步组内线程，会阻塞线程组中所有线程的执行，直到所有共享内存的访问完成并且线程组中的所有线程都执行到此调用。

### Shader.PropertyToID

在CS中定义的变量依旧可以通过 Shader.PropertyToID("name") 的方式来获得唯一id。

在频繁利用`ComputeShader.SetBuffer`对相同变量赋值的时候，将id进行缓存避免GC。

```c#
int grassMatrixBufferId;
void Start() {
    grassMatrixBufferId = Shader.PropertyToID("grassMatrixBuffer");
}
void Update() {
    compute.SetBuffer(kernel, grassMatrixBufferId, grassMatrixBuffer);
    
    // dont use it
    //compute.SetBuffer(kernel, "grassMatrixBuffer", grassMatrixBuffer);
}
```

## 视锥体剔除

View Frustum Culling

### 为什么需要视锥剔除？

如果不对物体做剔除操作，那么视锥体范围内看不到的物体也会通过CPU提交DrawCall传递到GPU中，并参与到顶点着色器的计算，如果有几何着色器一样会参与计算。