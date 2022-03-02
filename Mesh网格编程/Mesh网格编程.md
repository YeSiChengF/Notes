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

添加立方体顶点前，需要先知道有多少顶点数量，一个面的顶点数量为`(x+1)*(y+1)`，则一个立方体的顶点数量为`2(((#x+1)*(#y+1))*((#x+1)*(#z+1))*((#z+1)*(#y+1)))`。这样计算后八个角的顶点会各会多出三个重复的顶点，每条边上的顶点也都会多增加一个重复顶点。

 ![img](https://catlikecoding.com/unity/tutorials/rounded-cube/02-vertex-overlap.png) 

所以我们需要将八个角、十二条边(每个方向四条)、面内部的顶点进行单独计算。

```c#
private void GenerateCube()
{
	//八个角顶点
	int cornerVertices = 8;
	//各条边顶点
	int edgeVertices = (xSize + ySize + zSize - 3) * 4;
	//内部面顶点
	int faceVertices = (
		(xSize - 1) * (ySize - 1) +
		(xSize - 1) * (zSize - 1) +
		(ySize - 1) * (zSize - 1)) * 2;
	_vertices = new Vector3[cornerVertices + edgeVertices + faceVertices];
}
```

