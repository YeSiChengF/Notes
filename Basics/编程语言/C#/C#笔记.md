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

### 多线程

#### 异步委托（后续不支持这种写法）

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

#### 创建线程

`Thread thread = new Thread(Test);`创建线程必须传递参数，可以通过传递方法进行构建。

线程的调用通过`thread.Start()`函数

#### 线程的调用

##### 传递方法

```c#
static void Test()
{
    Console.WriteLine("Started");
    Thread.Sleep(1000);
    Console.WriteLine("Completed");
}
static void Main(string[] args)
{
    Thread thread = new Thread(Test);
    thread.Start();
    Console.WriteLine("Main completed");
}
```

线程的构建可以传递无参和有一个参的构造函数

`ParameterizedThreadStart`有参和`ThreadStart`无参

通过`Start(obj)`方法传递参数

```c#
static void Download(Object o)
{
    Console.WriteLine("Started");
    Thread.Sleep(1000);
    Console.WriteLine("Completed");
}
static void Main(string[] args)
{
    Thread thread = new Thread(Download);
    thread.Start("abb");
    Console.WriteLine("Main completed");
}
```

##### 自定义类传递数据

通过自定义类，在类里创建方法。创建线程时使用对象内的方法构建。

###### 自定义类

```c#
internal class DownloadTool
{
    public string URL
    {
        get;
        private set;
    }
    public string Message
    {
        get;
        private set;
    }
    public DownloadTool(string url, string message)
    {
        URL = url;
        Message = message;
    }
    public void Download()
    {
        Console.WriteLine("从" + URL + "中下载" + Message);
    }
}
```

```c#
static void Main(string[] args)
{
    DownloadTool downloadTool = new DownloadTool("http:xxxx/xxxx/xx.com", "ssdd");
    Thread thread = new Thread(downloadTool.Download);
    thread.Start();
}
```

#### 后台线程和前台线程

**后台线程是服务于前台线程的。前台线程关闭后，后台线程也没有存在的必要。(会自动关闭)**

 只有一个前台线程在运行，应用程序的进程就在运行，如果多个前台线程在运行，但是Main 方法结束了，应用程序的进程仍然是运行的，直到所有的前台线程完成其任务为止。 

 在默认情况下，用Thread类创建的线程是前台线程。线程池中的线程总是后台线程。 

**在用Thread类创建线程的时候，可以设置IsBackground属性，表示它是一个前台线程还是一 个后台线程。**

```c#
static void Main(string[] args)
{
    DownloadTool downloadTool = new DownloadTool("http:xxxx/xxxx/xx.com", "ssdd");
    //设置为前台线程(默认)
    Thread thread = new Thread(downloadTool.Download){ IsBackground = false};
    thread.Start();
}
```

 

```c#
static void Main(string[] args)
{
    Console.WriteLine("dadada");
    //前台线程结束后，后台线程都会自动关闭不执行
    DownloadTool downloadTool = new DownloadTool("http:xxxx/xxxx/xx.com", "ssdd");
    //设置为后台线程
    Thread thread = new Thread(downloadTool.Download){ IsBackground = true};
    thread.Start();
}
```

#### 线程优先级

每个线程分配到的`时间片段`可能不太相同，可以通过优先级控制。优先级比较高的，会更优先调度。

线程有操作系统调度，**一个CPU同一时间只能做一件事情（运行一个线程中的计算任务）**，当有很多线程需要CPU去执行的时候，线程调度器会根据线程的优先级去判断先去执行哪一个线程，**如果优先级相同的话，就使用一个循环调度规则，逐个执行每个线程。**

##### 设置优先级

在Thead类中，可以设置`Priority`属性，以影响线程的基本优先级,`Priority`属性是一个`ThreadPriority`枚举定义的一个值。定义的级别有`Highest`,`AboveNormal`,`Normal`,`BelowNormal` 和 `Lowest`。

#### 控制线程

##### 线程的状态

当我们获取线程状态时，线程拥有两个状态`Running`和`Unstarted`

当我们通过调用`Thread`对象的`Start`方法，可以创建线程，但是调用了`Start`方法之后，新线程不是马上进入`Running`状态，而是出于`Unstarted`状态。

只有当操作系统的线程调度器选择了要运行的线程，这个线程的状态才会修改为`Running`状态。我们使用`Thread.Sleep()`方法可以让当前线程休眠进入`WaitSleepJoin`状态。

##### Join方法

如果需要等待线程的结束，可以调用`Thread`对象的 `Join`方法。
把`Thread`加入进来并停止当前的线程，并将当前线程设置为`WaitSleepJoin`状态，直到Join进来的线程完成为止。才会继续执行当前线程。

#### 线程池

**创建线程需要时间。线程池减少了创建线程的时间。**

**线程池启动的线程默认都是后台线程,不能给入池的线程设置优先级或名称**

**入池的线程只能用与时间较短的任务，如果线程要一直运行就要用Thread类创建一个线程。**

`ThreadPool`类管理线程。这个类会在需要时增减池中线程的线程数,直到达到最大的线程数。 池中的最大线程数是可配置的。 

在双核 CPU中,默认设置为1023个工作线程和 1000个 I/O线程。

也可以指定在创建线程池时应立即启动的最小线程数,以及线程池中可用的最大线程数。 
如果有更多的作业要处理,线程池中线程的个数也到了极限,最新的作业就要排队,且必须等待线程完成其任务。

使用`ThreadPool.QueueUserWorkItem(WaitCallback)`调用线程池内线程。`WaitCallback`带有一个参数的方法。

```c#
static void Main()
{
    int nWorkerThreads;
    int nCompletionPortThreads;
    ThreadPool.GetMaxThreads(out nWorkerThreads, out nCompletionPortThreads);
    Console.WriteLine("Max worker threads : " + nWorkerThreads + " I/O completion threads
:"+nCompletionPortThreads );
for (int i = 0; i < 5; i++)
{
    //使用线程池内线程
    ThreadPool.QueueUserWorkItem(JobForAThread);
}
Thread.Sleep(3000);
}
static void JobForAThread(object state)
{
    for (int i = 0; i < 3; i++)
    {
        Console.WriteLine("Loop " + i + " ,running in pooled thread
        "+Thread.CurrentThread.ManagedThreadId);
        Thread.Sleep(50);
    }
}
```

### 任务

 在.NET4 新的命名空间`System.Threading.Tasks`包含了类抽象出了线程功能，**在后台使用的 `ThreadPool`进行管理的(后台线程)**。 

 任务表示应完成某个单元的工作。这个工作可以在单独的线程中运 行，也可以以同步方式启动一个任务。 

 任务也是异步编程中的一种实现方式。 、

#### 启动任务

```c#
//启动任务的两种方式
TaskFactory tf = new TaskFactory();
Task t1 = tf.StartNew(TaskMethod);
Task t3 = new Task(TaskMethod);
t3.Start();
```

#### 连续任务

 如果一个任务t1的执行是依赖于另一个任务t2的，那么就需要在这个任务t2执行完毕后才开 始执行t1。这个时候我们可以使用连续任务。  

```c#
static void DoFirst(){
	Console.WriteLine("do in task : "+Task.CurrentId);
	Thread.Sleep(3000);
}
static void DoSecond(Task t){
	Console.WriteLine("task "+t.Id+" finished.");
	Console.WriteLine("this task id is "+Task.CurrentId);
	Thread.Sleep(3000);
}
Task t1 = new Task(DoFirst);
//t1执行完后执行t2
Task t2 = t1.ContinueWith(DoSecond);
Task t3 = t1.ContinueWith(DoSecond);
//t2执行完后执行t4
Task t4 = t2.ContinueWith(DoSecond);
Task t5 = t1.ContinueWith(DoError,TaskContinuationOptions.OnlyOnFaulted);
```

#### 任务层级结构

在一个任务中嵌套一个任务，相当于新任务是当前任务的子任务。两个任务异步执行，如果父任务执行完但子任务还没执行完，父任务的状态会设置为` WaitingForChildrenToComplete `，只有当子任务也执行完了，父任务的状态才会变成` RunToCompletion `

### 资源访问冲突问题

多线程访问同一个资源时,会产生逻辑冲突或程序奔溃。

多个线程读写同一个资源时，没有增加保护机制导致的数据不安全、不可靠。

```c#
public class StateObject{
	private int state = 5;
	public void ChangeState(int loop){
		if(state==5){
			state++;//6
			Console.WriteLine("State==5:"+state==5+" Loop:"+loop);//false
		}
		state = 5;
	}
}
static void Main(){
	var state = new StateObject();
	for(int i=0;i<20;i++){
        //导致当前线程访问到的资源可能已经被后面线程修改的情况。使资源数据已经成为了脏数据。
		new Task(RaceCondition,state).Start();
	}
    //只是保证后台线程能够执行完后再结束主线程
	Thread.Sleep(10000);
}

```

#### 使用锁

**锁可以是任意类型的对象，一般为`Object`类型对象**

**只有一个线程会拿到锁，拿不到锁的线程会在这里等待。**如果等待锁的线程数量很多，则会发生“抢锁”的情况，谁先抢到锁谁就先执行。

**使用锁的优缺点，优点使数据安全，缺点拖慢线程的执行速度。**

```c#
private Object _lock=new Object();
private int state=5;
private void ChangeState()
{
    lock(_lock){
        if(state==5)
        {
            state++;
            Console.WriteLine("State:"+state);
		}
        state=5;
    }
}
```

#### 死锁问题

 **线程在等待一个永远取不到的锁，被称为死锁。**死锁问题导致等待这个锁的线程全部进入无限制的等待。

当一个锁管理两个资源时，如果两个资源是被线程独立调用（没有线程同时用两个资源的情况）就会造成性能浪费，因为只要用到这个两个资源的线程都会面临等待锁的情况。

```c#
public class SampleThread
{
    private StateObject s1;
    private StateObject s2;
    public SampleThread(StateObject s1, StateObject s2)
    {
        this.s1 = s1;
        this.s2 = s2;
    }
    public void Deadlock1()
    {
        int i = 0;
        while (true)
        {
            //先取s1锁再取s2锁
            lock (s1)
            {
                lock (s2)
                {
                    s1.ChangeState(i);
                    s2.ChangeState(i);
                    i++;
                    Console.WriteLine("Running i : " + i);
                }
            }
        }
    }
    public void Deadlock2()
    {
        int i = 0;
        while (true)
        {
            //先取s2锁再取s1锁
            lock (s2)
            {
                lock (s1)
                {
                    s1.ChangeState(i);
                    s2.ChangeState(i);
                    i++;
                    Console.WriteLine("Running i : " + i);
                }
            }
        }
    }
}
var state1 = new StateObject();
var state2 = new StateObject();
//当线程需要的锁被其他线程取走了，就造成了死锁问题。
//这个线程取了s1锁，拿不到s2锁
new Task(new SampleTask(s1, s2).DeadLock1).Start();
//这个线程取了s2锁，拿不到s1锁
new Task(new SampleTask(s1, s2).DeadLock2).Start();
```

##### 解决死锁

 在编程的开始设计阶段，设计锁定顺序 

必须先拿到A锁，然后才能拿B锁。不能产生不同顺序的情况。