# Mesh网格编程

## 简介

在Unity中显示一个3D模型，需要两个组件。

- MeshFilter(对要显示的网格的引用)
- MeshRender(配置网格渲染的方式)：使用哪种材质、是否接收阴影和进行反射。

给网格添加细节的快速方式是提供一个UV贴图，向顶点添加2D纹理坐标实现，通过设置UV坐标调整贴图在网格上的位置。

## 创建一个面片

### 1.创建网格顶点

设置顶点的参数

```c#
//创建圆形的高度和宽度
public int xSize=10, ySize=5;
//记录顶点坐标 
public Vector3[] vertices;
```

给网格顶点定位，否则都在同一个位置渲染

```c#
private void Generate () {
	vertices = new Vector3[(xSize + 1) * (ySize + 1)];
	for (int i = 0, y = 0; y <= ySize; y++) {
		for (int x = 0; x <= xSize; x++, i++) {
			vertices[i] = new Vector3(x, y);
		}
	}
}
```

使用Gizmos将顶点可视化，显示于Scene窗口

```c#
private void OnDrawGizmos()
{
    //判断顶点是否被渲染过
	if (vertices == null)
	{
		return;
	}
    //设置Gizmos颜色
    Gizmos.color = Color.red;

    for (int i = 0; i < vertices.Length; i++)
    {
        //DrawSphere(渲染的顶点坐标,渲染的半径)
        //将当前顶点渲染出来，transform.TransformPoint渲染gameobject的局部坐标，可以跟随gameobject移动
        Gizmos.DrawSphere(transform.TransformPoint(vertices[i]),0.1f);
    }
}

```

使用协程让渲染顺序可视化

```c#
private void Awake () {
	StartCoroutine(Generate());
}

private IEnumerator Generate () {
	WaitForSeconds wait = new WaitForSeconds(0.05f);
	vertices = new Vector3[(xSize + 1) * (ySize + 1)];
	for (int i = 0, y = 0; y <= ySize; y++) {
		for (int x = 0; x <= xSize; x++, i++) {
			vertices[i] = new Vector3(x, y);
			yield return wait;
		}
	}
}
```



### 2.创建网格

通过 Gizmos  知道顶点是正确的后，需要处理实际网格。一旦处理了顶点，就需要将组件内保存的顶点引用赋值给MeshFilter。

```c#
private Mesh mesh;
private IEnumerator Generate () {
		WaitForSeconds wait = new WaitForSeconds(0.05f);
		
		GetComponent<MeshFilter>().mesh = mesh = new Mesh();
		mesh.name = "Procedural Grid";

		vertices = new Vector3[(xSize + 1) * (ySize + 1)];
		…
		mesh.vertices = vertices;
	}
```

还需要对网格进行持久化保存，使其在编辑模式下也能保存MeshFilter组件的引用。

//TODO

#### 绘制一个三角形

```c#
private IEnumerator Generate () {
		…

		int[] triangles = new int[3];
		triangles[0] = 0;
		triangles[1] = 1;
		triangles[2] = 2;
		mesh.triangles = triangles;
}
```

#### 绘制立方体

概念上一个立方体是由六个2D面组成，这些面通过旋转和定位来构成3D图形。

##### 创建立方体顶点

添加立方体顶点前，需要先知道有多少顶点数量，一个面的顶点数量为`(x+1)*(y+1)`，则一个立方体的顶点数量为`2(((#x+1)*(#y+1))*((#x+1)*(#z+1))*((#z+1)*(#y+1)))`。这样计算后八个角的顶点会各会多出三个重复的顶点，每条边上的顶点也都会多增加一个重复顶点。

 ![img](https://catlikecoding.com/unity/tutorials/rounded-cube/02-vertex-overlap.png) 

所以我们需要将八个角、十二条边(每个方向四条)、面内部的顶点进行单独计算。

```c#
private void GenerateCube()
{
	//八个角顶点
	int cornerVertices = 8;
	//各条边顶点
    //边上的点为x+1(包含两段),则不包含两端为x-1
    //(X-1)*4+(Y-1)*4+(Z-1)*4
	int edgeVertices = (xSize + ySize + zSize - 3) * 4;
	//内部面顶点
    //去掉两端的顶点相乘
	int faceVertices = (
		(xSize - 1) * (ySize - 1) +
		(xSize - 1) * (zSize - 1) +
		(ySize - 1) * (zSize - 1)) * 2;
	_vertices = new Vector3[cornerVertices + edgeVertices + faceVertices];
}
```

生成四个面的点

```c#
private IEnumerator GenerateVerTex()
{
    int index = 0;
    for (int y = 0; y < ySize + 1; y++)
    {
        for (int x = 0; x < xSize + 1; x++, index++)
        {
            _vertices[index] = new Vector3(x, y, 0);
            yield return new WaitForSeconds(0.2f);
        }
        //z为0的点已经生成过
        for (int z = 1; z < zSize + 1; z++, index++)
        {
            _vertices[index] = new Vector3(xSize, y, z);
            yield return new WaitForSeconds(0.2f);
        }
        //xSize这个点已经生成过了
        for (int x = xSize - 1; x >= 0; x--, index++)
        {
            _vertices[index] = new Vector3(x, y, zSize);
            yield return new WaitForSeconds(0.2f);
        }
        //不能到0 已经生成过
        for (int z = zSize - 1; z > 0; z--, index++)
        {
            _vertices[index] = new Vector3(0, y, z);
            yield return new WaitForSeconds(0.2f);
        }
    }
}
```

上下两侧面内顶点
```c#
//底面 内部顶点
for (int z = 1; z < zSize; z++)
{
    for (int x = 1; x < xSize; x++, index++)
    {
        _vertices[index] = new Vector3(x, 0, z);
        yield return new WaitForSeconds(0.1f);
    }
}
//顶面 内部顶点
for (int z = 1; z < zSize; z++)
{
    for (int x = 1; x < xSize; x++, index++)
    {
        _vertices[index] = new Vector3(x, ySize, z);
        yield return new WaitForSeconds(0.1f);
    }
}
```

##### 创建立方体三角形序列

 ![img](https://catlikecoding.com/unity/tutorials/rounded-cube/03-quad.png)
提供创建一个面的方法，其他的五个面可以通过一样的方法创建
``` c#
private int SetQuad(int[] triangles, int i, int v00, int v10, int v01, int v11)
{
    triangles[i] = v00;
    triangles[i + 1] = triangles[i + 4] = v01;
    triangles[i + 2] = triangles[i + 3] = v10;
    triangles[i + 5] = v11;
    //提供给后面的面的索引值
    return i + 6;
}
```
三角形序列数量与顶点数量不同，不需要考虑顶点重复的问题

首先取一个面中四边形的数量`(x*y)*2`后为一个面中三角形的数量,每个面各有两个面需要`(x*y)*2*2`，最后乘以三角形的顶点数`(x*y)*2*2*3`，最终求出来的是三角形序列的数量不要搞混。

 ``` c#
private void CreateTriangles () {
	int quads = (xSize * ySize + xSize * zSize + ySize * zSize) * 2;
	int[] triangles = new int[quads * 6];
	mesh.triangles = triangles;
}
 ```

 获得立方体一圈的顶点数量，首先四边形有四个顶点，一条边内除顶点的其他点数量为`x-1`，z一圈的点则为`4+(x-1+y-1)*2`，化简后为`2x+2z`。

###### 生成长方形的第一个面片

```c#
//一圈的顶点数量
int circleVerticeCount = 2 * xSize + 2 * zSize;
SetQuad(_triangles, 0, 0, 1,0+ circleVerticeCount,1+ circleVerticeCount);
```
###### 生成一圈的长方形
``` c#
for (int v = 0, t = 0; v < circleVerticeCount; v++, t += 6)
{
	SetQuad(_triangles, t, v, v + 1, v + circleVerticeCount, v + 1 + circleVerticeCount, circleVerticeCount);
}
```
但是这样处理后的一圈的最后两个顶点V10、v11会向上一个高度单位,因为我们的顶点不是重复的。我们需要对最后v10和V11进行特殊处理
``` c#
private int SetQuad(int[] triangles, int i, int v00, int v10, int v01, int v11, int circleVerticeCount)
{
    //取余操作保证最后一个点不会超过范围保证在同一水平线生成
    v10 = ((v00 / circleVerticeCount) * circleVerticeCount) + (v10 % circleVerticeCount);
    v11 = ((v01 / circleVerticeCount) * circleVerticeCount) + (v11 % circleVerticeCount);
    triangles[i] = v00;
    triangles[i + 1] = triangles[i + 4] = v01;
    triangles[i + 2] = triangles[i + 3] = v10;
    triangles[i + 5] = v11;
    //提供给后面的面的索引值
    return i + 6;
}
```