## 反射和特性



程序本身(类的定义和BCL中的类)这些也是数据。(BCL-Basic Class Lib基础类库)

程序及其类型的数据被称为**元数据**(metadata)，他们保存在程序集中。

元数据存储于**程序集**中。

元数据保存类的定义、方法、内容。

运行的程序查看其元数据或其他程序集元数据的行为，称为反射。

### 特性

 特性(attribute)是一种允许我们向程序的程序集增加元数据的语言结构。它是用于保存程 序结构信息的某种特殊类型的类。

 

### Type类型

Type类位于`System.Reflection`命名空间下

Type类用来包含类型的特性，使用这个类的对象能让 我们获取程序使用的类型的信息。  

Type是**抽象类**，因此**不能利用它去实例化对象**。关于Type的重要事项如下： 

- 对于程序中用到的**每一个类型**，**CLR都会创建一个包含这个类型信息的Type类型的 对象**。 

- 程序中用到的每一个类型都会关联到独立的Type类的对象。 

- **不管创建的类型有多少个示例，只有一个Type对象会关联到所有这些实例。** 

```
System.Type类部分成员
成员 成员类型 描述
Name 属性 返回类型的名字
Namespace 属性 返回包含类型声明的命名空间
Assembly 属性 返回声明类型的程序集。
GetFields 方法 返回类型的字段列表
GetProperties 方法 返回类型的属性列表
GetMethods 方法 返回类型的方法列表
```

#### 获取Type对象

```c#
Type t=myClass.GetType();
Type t=typeof(ClassName);
```

#### 获取里面的字段

通过反射无法获得类的私有成员，只能在类内部访问

```c#
FieldInfo[] fi=t.GetFields();
```

###  Assembly类 

 Assembly 位于`System.Reflection`命名空间下。Assembly允许访问程序集的元数据，也可以加载和执行程序集。

#### 加载程序集

```c#
//根据程序集的名字加载程序集，它会在本地目录和全局程序集缓存目录查找符合名字的程序集。
Assembly assembly1 = Assembly.Load("SomeAssembly");
//这里的参数是程序集的完整路径名，它不会在其他位置搜索。
Assembly assembly2 = Assembly.LoadFrom(@"c:\xx\xx\xx\SomeAssembly.dll");
```

####  Assembly对象的使用 

#####  遍历程序集中定义的类型 

```c#
Type[] types = theAssembly.GetTypes();
foreach(Type definedType in types)
{
//
}
```

#####  遍历程序集中定义的所有特性 

```c#
Attribute[] definedAttributes =
Attribute.GetCustomAttributes(someAssembly)
```

