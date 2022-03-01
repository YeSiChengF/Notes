# Unity网格编程

所有的图形都是由点和面(三角面)构成

三角面渲染效率最高

顶点顺时针顺序的三角面，法线朝上。![1644332408309](C:\Users\ASUS\AppData\Roaming\Typora\typora-user-images\1644332408309.png)

相同位置顶点可以进行优化

### Mesh的属性

- vertex顶点
- normal每个顶点法线(大小和顶点相对应)
- UV纹理坐标
- Triangles三角形序列

Triangles三角形序列  通过int[]存储，与Vertex顶点的数值[]相对应,存储的为Vertex[]内的下标。三角形序列的意义在于辨别哪三个点构成了三角形(有可能共用相同顶点)

mesh绘制时的坐标是根据脚本挂载物体的坐标来计算的，当前物体的局部坐标。

mesh绘制时的坐标都是自身坐标系下的局部坐标

在Unity中的OpenGL和UE中的DirectX显示规则UV坐标系不同，切线的第四维度为区分正反。切线更多应用于shader部分

### 圆角立方体实现思路

