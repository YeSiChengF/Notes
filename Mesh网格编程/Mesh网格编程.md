# Mesh网格编程

## 简介

在Unity中显示一个3D模型，需要两个组件。

- MeshFilter(对要显示的网格的引用)
- MeshRender(配置网格渲染的方式)：使用哪种材质、是否接收阴影和进行反射。

给网格添加细节的快速方式是提供一个UV贴图，向顶点添加2D纹理坐标实现，通过设置UV坐标调整贴图在网格上的位置。

## 创建一个面片

### 1.创建网格顶点

设置圆点的参数

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