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

### 立方体实现思路

根据x和z的长度生成一个底部一圈的顶点，再使用y的长度循环生成四个面的顶点。再分别求出顶部和底部的面，注意顶点生成时不应该重复，否则可能产生计算的失误。顶部和底部点数量分别为`xSize-2*zSize-2`，其区别仅在于y轴上的差异。在生成网格时，也是需要先生成四周的网格，最后生成顶部和底部网格。在生成顶部和底部网格时，需要注意在生成顶点时，顶部和底部顶点的生成顺序，以免计算失误。

在生成网格面时可以使用通用方法来生成一个正方形的网格(为两个三角形组成)

```c#
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



### 圆角立方体实现思路

 ![img](https://catlikecoding.com/unity/tutorials/rounded-cube/04-inner-cube.png) 

通过法向量求出圆角立方体，法向量为目标点到中心点的向量，内部长方形到外部长方形的向量。我们还需要了解圆角的半径是多少。

![1646491529596](C:\Users\ASUS\AppData\Roaming\Typora\typora-user-images\1646491529596.png)

x小于圆角半径时判断圆角法向量`x<r`或者`x>r&&x<xSize-r`

![1646492283958](C:\Users\ASUS\AppData\Roaming\Typora\typora-user-images\1646492283958.png)

左边的中心点都为p0 右边的目标点都为p1，中间的中心点都为他本身(x相同)

![1646494516855](C:\Users\ASUS\AppData\Roaming\Typora\typora-user-images\1646494516855.png)

具体思路：根据`圆角半径Rounded`来求出假想的内置长方形，如长方体上的任一顶点`vector`判断其`x、y、z`点的范围是否在`vextex.x < roundness`或`vextex.x > xSize - roundness`(以x轴举例)来得出内置长方形中的`中心点(inner)(向外发射向量的点)`，`法向量(中心点向目标点的防方向没有大小)`则通过`目标点(为判断的顶点vector)`-`中心点`得到，在根据`法向量`和`中心点(inner)`得到新的点即为`中心点inner`+`向量`得到的新的顶点。再根据长方体的面创建方法创建。

### 球体实现思路

与圆角立方体的实现思路类似，将`xSize、ySize、zSzie`统一为`直径DiameterSize`，计算得出一个正方形(每条边和每个面都相等)。再根据 `中心点`在球体中即为圆心，根据`半径roundness`求出圆心方向向量(法向量)为`new Vector(roundness,roundness,roundness)`。根据`法向量`重新计算圆形的点并生成图形。

### 图形设置大小

在前面的思路中只通过了Size来设置x、y、z的位置，如要设置大小则需要加入大小单位`UnitDistace`每个网格的大小单位。再设置顶点坐标时需要`坐标*UnitDistace`得到新的坐标。只要在使用坐标时都需要引入使用`UnitDistace`。
可以使用统一方法进行设置
``` c#
private Vector3 SetVertexPos(int x, int y, int z)
{
    //return new Vector3(x * UnitDistace, y * UnitDistace, z * UnitDistace);
    return new Vector3(x, y, z) * UnitDistace;
}
```
### 变形动画思路

当点击物体时获得到点击的顶点，再根据范围(权重)来修改顶点的位置。变形的本质就是顶点不变修改顶点的位置。主要取决于变形动画自不自然，取决于变形公式。

需要保存两个数组 一个保存原本的顶点位置数值(所有)，还有一个保存变形后的顶点数值(所有)(如果只保存修改的点可能无法知道哪些点是修改后的点)，在恢复时使用原数据恢复。

#### 力的作用方式

![1646559315745](C:\Users\ASUS\AppData\Roaming\Typora\typora-user-images\1646559315745.png)

当一个力按压时周围点跟随变化，方向向量则为p点向各个点的方向向量。方向不同，受到的具体力也不同。受力不同变形的速度也不同。
在计算`施力点`和`受力点`的方向向量时，由于射线检测`施力点`可能在和`受力点`在同一平面，所以最好将`施力点`向偏移一段距离来方便计算。
**计算力衰减公式：`原始的力/距离²=新的力`，距离越小力越大，距离越大力越小**!
[1646564277834](C:\Users\ASUS\AppData\Roaming\Typora\typora-user-images\1646564277834.png)
但是如果将距离为0带入公式后，力也会为0，因为距离为分母不能为0。
于是有了新的公式：`原始的力/(距离²+1)=新的力`，当距离为0时，力为最大值。
力的加速度公式：`F/M=a`，速度公式则为`v=a*△t(时间)`，最终速度为`F/M*△t`，在这里先不需要计算`重量m`，即速度公式为`F*△t`

#### 反作用力

在静止时会有一个反向的力，大小相等方向相反。在没有施加反向力的情况下会一直凹陷下去。

![1646579704105](C:\Users\ASUS\AppData\Roaming\Typora\typora-user-images\1646579704105.png)

反作用力可以视为想回到`原始坐标点`的向量，`中心点`为当前改变的坐标，`目标点`为原始坐标点。
反作用力需要衰减，否则将会一直执行反作用力。

