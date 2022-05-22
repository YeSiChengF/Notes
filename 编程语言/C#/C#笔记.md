## 反射和特性



程序本身(类的定义和BCL中的类)这些也是数据。(BCL-Basic Class Lib基础类库)

程序及其类型的数据被称为**元数据**(metadata)，他们保存在程序集中。

元数据存储于**程序集**中。

元数据保存类的定义、方法、内容。

运行的程序查看其元数据或其他程序集元数据的行为，称为反射。

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

### 特性

 特性(attribute)是一种允许我们向程序的程序集增加元数据的语言结构。它是用于保存程序结构信息的某种特殊类型的类。

通过标签往类里多增加信息，增加描述信息

- 将应用了特性的程序结构叫做目标
-  设计用来获取和使用元数据的程序（对象浏览器）叫做特性的消费者 (VS读取特性信息，VS就是消费者)
- .NET预定了很多特性，我们也可以声明自定义特性  

###  Obsolete和Conditional特性 

#### Obsolete 标签

使用Obsolete标签标记方法已弃用，使用其他方法。

添加弃用标签后会报警报提醒

```c#
[Obsolete("方法已过时，请使用xx方法")]
public static void OpenInFolder(string folderPath)
{
	Application.OpenURL("file://" + folderPath);
}
//使用true时，方法不可使用
[Obsolete("方法已过时，请使用xx方法"),true]
public static void OpenInFolder(string folderPath)
{
	Application.OpenURL("file://" + folderPath);
}
```

#### Conditional标签

条件标签，需要与宏相对应

当条件标签里的字符串有相对应定义的宏时，方法会被调用。

当没有定义相对应宏时，方法不会被调用。

```c#
//宏定义
#define IsShowMessage
//当宏被定义时会调用，没有则不调用
[Conditional("IsShowMessage")]
public void ShowMessage()
{
}
```

###  调用者信息特性和DebuggerStepThrough特性 

#### Caller调用者特性

通过调用者特性查找哪里调用了，当给参数添加调用者特性时参数必须有初始值。当方法被调用时，系统会给对应参数赋值。

`[CallerLineNumber]`第几行调用了

`[CallerFilePath]`哪个文件被调用了，完整路径

`[CallerMemberName]`哪个方法调用的

```c#
using System.Runtime.CompilerServices;
public void ShowMessage(string message,[CallerLineNumber]int lineNumber=0,[CallerFilePath]="",[CallerMemberName]string memberName="")
{
}
```

#### DebuggerStepThrough

跳过调试模式，不会进入方法内部

```c#
[DebuggerStepThrough]
public void ShowMessage(){
}
```

### 自定义特性

自定义一个特性需要先定义一个特性类

特性类命名规范：`**Attribute`,必须以Attribute结尾

必须继承Attribute类

需要通过特性定义特性类的作用于哪里

```c#
[AttributeUsage(AttributeTargets.Class)]
internal sealed class InfomationAttribute:Attribute
{
    public string developer;
    public string version;
    public string description;
    public InfomationAttribute(string developer,string version,string description)
    {
        this.developer=developer;
        this.version=version;
        this.description=description;
    }
}
```
使用自定义特性

```c#
//本质上调用特性类的构造方法 
[InfomationAttribute("name","v1.1","个人信息")]
public class Personal(){}
```

#### Type判断是否定义某个特性

```c#
Type t=typeof(Personal);
t.isDefined(typeof(InfomationAttribute),false)
```

#### 得到所有Type上所有特性

```c#
object[] attributeArray=t.GetCustomAttributes(false);
```

## 线程、任务和同步

### 异步委托（后续不支持这种写法）

委托是创建线程的一种简单方法，可以异步调用它。

委托是方法的类型安全的引用。

当`Delegate类`异步调用时，Delegate类会创建一个执行任务的线程。

```c#
public delegate void TasksTestDelegate();
public void	TaskTest(){
    
}
public void Main()
{
    TasksTestDelegate t1=TaskTest;
    t1.BeginInvoke(null,null);
}

```

