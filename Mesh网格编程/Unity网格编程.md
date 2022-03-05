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

 ![img](https://catlikecoding.com/unity/tutorials/rounded-cube/04-inner-cube.png) 

通过法向量求出圆角立方体，法向量为目标点到中心点的向量，内部长方形到外部长方形的向量。我们还需要了解圆角的半径是多少。

![1646491529596](C:\Users\ASUS\AppData\Roaming\Typora\typora-user-images\1646491529596.png)

x小于圆角半径时判断圆角法向量`x<r`或者`x>r&&x<xSize-r`

![1646492283958](C:\Users\ASUS\AppData\Roaming\Typora\typora-user-images\1646492283958.png)

左边的中心点都为p0 右边的目标点都为p1，中间的中心点都为他本身(x相同)

![1646494516855](C:\Users\ASUS\AppData\Roaming\Typora\typora-user-images\1646494516855.png)

具体思路：根据`圆角半径Rounded`来求出假想的内置长方形，如长方体上的任一顶点`vector`判断其`x、y、z`点的范围是否在`vextex.x < roundness`或`vextex.x > xSize - roundness`(以x轴举例)来得出内置长方形中的`中心点(inner)(向外发射向量的点)`，`法向量(中心点向目标点的防方向没有大小)`则通过`目标点(为判断的顶点vector)`-`中心点`得到，在根据`法向量`和`中心点(inner)`得到新的点即为`中心点inner`+`向量`得到的新的顶点。再根据长方体的面创建方法创建。