# C++大纲

## 注意点

- 值类型也是可以把地址作为函数参数，**使用指针传递地址**，或**使用引用传递地址**
- 当类没有new时只在栈中存放里内存地址，并没有开辟空间，所以都是在栈中，所以如果在方法内只声明了类当方法结束时会被释放，会触发构造和析构函数。
- 只要使用了new关键字则都是在堆中开辟内存
- C++编译器会给每个空对象也分配一个字节空间，为了区分空对象占内存的位置信息。每个对象都有独立的内存空间
- 在32位操作系统下，我们普遍用的gcc编译器和vs编译器都是默认**按照4个大小进行内存对齐**的也就是说结构体或类中的内存分配默**认是按照4个字节的倍数进行分配**的。当不足4的倍数时会向上补齐
- 在C#中静态成员必须在静态类中才能定义，而c++在普通类中也可以定义静态成员，`int 类名::静态变量名=值`，静态成员和静态函数为每个对象所拥有
- 一般把类声明写在头文件中，将类成员函数写在c文件中
- c++中类里的函数可以在类里声明，在类外进行实现(编写)，格式`类名::函数名(){ 实现的内容 }`。在大项目中进行分开封装便于查看。
- 如果同一块内存重复释放会导致程序崩溃，两个指针都指向同一个块内存地址的情况下。利用深拷贝解决浅拷贝带来的问题

## 学习路线

计算机组成原理

https://www.bilibili.com/video/BV1EW411u7th?from=search&seid=11558880190937398744&spm_id_from=333.337.0.0

深入理解计算机系统

计算机体系结构量化分析

## 1.C++初识

### 简介

C++是C语言的超集，合法的C程序都是合法的C++程序

通常使用 .cpp .cp .c后缀名

### 启动框架

```c++
#include<iostream>
using namespace std;

int main(){
	int a=10;
    cout<<"a="<<a<<endl;//endl代表这行结束
    system("pause");
    return 0;
}
```

使用"\n"代替endl

```c++
#include<iostream>
int main(){
    int a=10;
    std::cout<<"a="<<a<<"\n";
    return 0;
}
```

### 三字符组

以两个？开头

现在默认不自动替换，需要 设置编译器命令行选项/Zc:trigraphs 

| 三字符组 | 替换 |
| :------- | :--- |
| ??=      | #    |
| ??/      | \    |
| ??'      | ^    |
| ??(      | [    |
| ??)      | ]    |
| ??!      | \|   |
| ??<      | {    |
| ??>      | }    |
| ??-      | ~    |

### 常量

#### 定义常量的两种方式

#define 宏常量	**通常在文件上方定义**，表示一个常量

const关键字

```c++
#define acc 888
int main(){
    const int abb=5;
    cout<<"acc="<<acc<<endl;//报错，宏常量不可修改
}
```

## 2.数据类型

### 整型

short 2个字节(-32768-32767)	int 4个字节	long 4个字节	long long(长长整形)8个字节

short<int<=long<=long long

### sizeof关键字

统计数据类型所占用内存大小

可以传入变量或者数据类型进行判断

```c++
cout<<sizeof(int)<<endl;
short num1=10;
cout<<num1<<endl;
```

### 类型的最大最小值

```c++
namespace std;
int main(){
	cout << "\t最大值：" << (numeric_limits<long>::max)();  
	cout << "\t最小正数：" << (numeric_limits<long>::min)() << endl; 	cout << "\t最小值(为负数或0)"<<std::numeric_limits<T>::lowest()<<endl;
}
```

### typedef声明

可以对一个已有类型取别名

```c++
typedef int feet;
feet num=10;
```

### 实型(浮点型)

float 4字节 7位有效数字	double 8字节 15~16位有效数字

```c++
float f1=3.14;//不加f 默认识别为double类型，再转换赋值给float
float f2=3.14f;
float f3=3e2;//3*10^2
float f4=3e-2;//3*0.1^2;
```

### 字符型

char 在c和c++中只占1个字节

字符变量不是对字符本身进行存储，而是将**对应的ASCII码**进行存储

```c++
char ch='a';
//输出对应的ASCII码
cout<<(int)ch<<endl;
```

### 字符串型

需要使用头文件

```c++
#include <string> //头文件(类似类库)
```

### 布尔型

非0的数值都可以代表true,0代表false

```c++
bool flag=true;
cout<<flag<<endl;//1
bool flag=false;
cout<<flag<<endl;//0
```

### 数据的输入

关键字 cin

```c++
bool isHave;
cin>>isHave;
```

## 3.算数运算符

### 除法/	 

除数和取模数不能为0，否则为非法操作

### 取模%

两个小数不可以做取模运算

只有整型变量可以进行取模运算

## 4.程序流程结构

### break

退出当前循环

### continue

跳过本次循环

### goto

无条件跳转代码,跳转至标记点

`语法：goto 标记;`

```c++
int main(){
	goto FLAG;
    cout<<"!"<<endl;
    
    FLAG:
    cout<<"@"<<endl;
    
    return 0;
}
```

## 5.数组

### 一维数组

`int arr1[] = { 1,3,4,5 };`

**必须进行初始化操作**

名称用途：

1. 统计整个数组在内存中的长度 	`sizeof(arr)`
2. 获取数组在内存中的首地址(16进制) `cout<<arr<<endl`

查看数组元素地址 `cout<<&arr[0]<<endl`

数组名是常量已经指定了内存地址，不可以进行赋值操作

不能将一个数组直接赋值给另一个数组

### 二维数组

C++会自动划分元素

第四个元素自动划分为第二行中`int arr[2][3]={1,2,3,4,5,6}`

自动推算出行数`int arr[][3]={1,2,3,4,5,6}`  

## 6.函数

### 函数的分文件编写

1.创建后缀名为.h的头文件

2.创建后缀名为.cpp的源文件

3.在头文件中写函数的声明

4.在源文件中写函数的定义

## 7.指针

### 指针的概念

指针的作用：`可以通过指针间接访问内存`

内存编号从0开始，用十六进制数字表示

可以利用指针变量保存地址

**指针实际上记录的就是地址**

**在函数中，值类型需要按地址传递都是以指针形式**

指针类型在32位系统占用4个字节

1.定义指针

`数据类型*指针变量名`指针变量名一般用 p(pointer)

让指针记录变量地址 `&`

```c++
int a=10;
//定义指针 数据类型*指针变量名
int * p;
//让指针记录变量a的地址
p=&a;
//修改指针指向地址的值
*p=100;
```

2.使用指针

可以通过解引用的方式来找到指针指向的内存 `*`

可以修改或读取指针访问的内存

### 空指针和野指针

#### 空指针

空指针：`指针变量指向内存中编号为0的空间`

用途：初始化指针变量

注意：空指针指向的内存是不可以访问的

```c++
int * p = null;
//空指针不能复制会报错
*p=100;
```

#### 野指针

指针随便指向的一个内存地址，可能没有访问过会报错

在程序中需要避免出现野指针

```c++
int * p =(int)0x1100
```

### const修饰指针

#### 常量指针

指针的指向可以修改，但是指针指向的值不可以改

指针指向的内存地址可以不同，但是内存地址里的值需要相同

```c++
const int * p= &a;
```

#### 指针常量

指针指向的内存地址不能修改，地址内的值可以修改

```c++
int * const p = &a;
```

const修饰指针和常量

```c++
//地址和值都不可以改
const int * const p= &a;
```

### 指针和数组

利用指针访问数组的元素

```c++
int arr[10]={1,2,3,4,5,6,7,8,9,10};
int * p =arr;//数组首地址(数组第一个元素的地址)
cout << *p << endl;
//16进制++
//获取数组第二个元素
p++;
cout << *p << endl;
```

### 指针和函数

通过指针可以将值类型按引用类型方式传递

```c++
int a=10;
int b=20;
void swap(int * p1,int * p2){
    * p1=20;
    * p2=10;
}
swap(&a,&b);
```

### 指针、数组、函数

```c++
int arr[10]={4,3,6,9,1,2,10,8,7,5};
//数组长度
//数组所占用内存大小/单个元素占用内存大小=数组长度
int len=sizeof(arr)/sizeof(arr[0]);
```

## 8.结构体

### 结构体数组

将自定义的结构体放入数组中方便维护

`struct 结构体名 数组名[元素个数] = {{},{},.....}`

### 结构体指针

利用操作符`->`可以通过结构体指针访问结构体属性

```c++
struct student{
    string name;
    int age;
    int score;
};
```

```c++
struct student s={"张三",18,100};
student * p= &s;
```

### 结构体函数

**将函数中的形参改为指针可以减少内存空间,而且不会赋值新的副本**

如果一个结构体有很多数据，那么使用函数传递结构体时需要拷贝的字节数量增加。而使用指针只需要拷贝4个字节(32位系统)

### 结构体嵌套结构体

```c++
struct student{
    string name;
    int age;
    int score;
};
struct School{
	int peopleCout;
    student stu[];
};
```

### 结构体中的const使用

当结构体作为函数的形参传递时，使用的是引用传递，那么为了防止在函数内结构体里数据的修改可以使用 常量指针进行限制

```c++
void printStudents(const student *s){
    
}
```

## 9.核心编程

### 1.内存分区模型

- 代码区：存放函数体的二进制代码，由操作系统管理

- 全局区：存放全局变量和静态变量以及常量

- 栈区：由编译器自动分配释放，存放函数的参数值，局部变量等

- 堆区：由程序员分配和释放，若程序员不释放，程序结束时由操作系统回收

#### 内存四区意义：
不同区域存放的数据，赋予不同的生命周期。

### 程序执行时

编译后生成exe文件，**未执行该程序前**分为两个区域：代码区、全局区

#### 代码区

存放CPU执行的机器指令

代码区是**共享**的，频繁被执行的程序在内存里只需要一份代码

代码区是**只读**的，防止程序意外修改了指令

#### 全局区

全局变量、常量、静态变量

程序结束后由操作系统释放
##### 在全局区内
全局变量、静态变量static关键字、常量(字符串常量、const修饰的全局变量(全局常量))
##### 不在全局区内

局部变量、const修饰的局部变量(局部常量)

#### 栈区

不要返回局部变量的地址，栈区开辟的变量会在函数结束释放

#### 堆区

使用new开辟空间

### new操作符

堆区开辟的数据，手动释放利用操作符`delete`

释放数组时需要加[] `delete[] arr;`

```c++
int * func(){
    //new返回的是 该数据类型的指针
	int *p=new int(10);
    return p;
}
```

### 引用

作用：给变量起别名(给指针起别名)

语法：`数据类型 &别名 = 原名`

```c++
int a=10;
//此时a和b同时使用一个内存地址
//a和b的指针是相同的
int &b=a;
```

#### 注意事项

1.引用必须初始化(必须赋值)

2.初始化后不能更改

#### 方法中的引用传递

值类型可以按引用传递

```c++
int mySwap(int &a,int &b){
	//只有值修改，地址不变
    int temp=a;
	a=b;
	b=temp;
}
mySwap(a,b);
```
##### 1.不要返回局部变量的引用
```c++
int& mySwap(){
    //局部变量
	int a=10;
    return a;
}
int &ref=mySwap();
cout<<ref<<endl;//第一次结果正确，编译器做了保留
cout<<ref<<endl;//结果错误，a的内存释放
```
##### 2.函数的调用可以作为左值

如果函数的返回值是引用，这个函数调用可以作为左值

```c++
int& test(){
	static int a=10;
    return a;
}
int &ref=test();
test()=1000;
//ref=1000
```

#### 引用的本质

**引用的本质在c++内部实现就是一个指针常量**

#### 常量引用

常量引用主要修饰形参，防止误操作

在函数形参列表中，可以加入`const修饰形参`，防止形参改变实参

```c++
//等价于 int temp=10;const int& ref=temp;
//常量引用可以直接复制
const int & ref=10;
```

```c++
void showValue(const int &val){
    //防止在方法内修改了val
}
```

## 10.函数提升

### 占位参数

语法：`返回值类型 函数名 (数据类型){}`

写个数据类型占位

```c++
void func(int a,int){
}
//占位参数可传可不传
func(10,10);
func(20);
```
### 函数重载注意事项

#### 1.引用作为重载的条件

```c++
void func(int &a){
	cout<<a<<endl;
}
void func(const int &a){
	cout<<a<<endl;
}
func(a);//调用第一个方法,默认调用引用传递
func(10);//调用第二个方法，默认值传递
```

#### 2.函数重载碰到默认值

出现二义性，只能尽量避免

```c++
void func(int a,int b=10){
	cout<<a<<endl;
}
void func(int a){
	cout<<a<<endl;
}
func(19);//会报错起冲突不知道调用哪个
```

## 11.类与对象

### 封装

#### 封装的意义

- 将属性和行为作为一个整体，表现生活中的事物
- 将属性和行为加以权限控制

在设计类的时候，属性和行为写在一起，表现事物

语法：`class 类名{ 访问权限 : 属性 /行为 };`

```c++
//c++中的类
class Circle{
    public:
    	int m_r;
    	double calculateZC(){
            return 2*PI*m_r;
		}
};
```

#### struct和class的区别

##### 默认访问权限不同

struct默认权限为公共

class默认权限为私有

### 继承

### 多态

### 对象初始化和清理

#### 构造函数负责初始化

创建对象时为成员属性赋值

语法：`类名(){}`

#### 析构函数负责做清理

在销毁前系统自动调用，执行一些清理工作

语法：`~类名(){}`

**析构函数不可以有参数**

#### 构造函数的分类及调用

两种分类方式：

- ​	按参数分为：有参构造和无参构造

- ​	按类型分为：普通构造和拷贝构造

**拷贝构造函数**

```c++
Personal(const Personal &p){
    age=p.age;
}
```

三种调用方式：

- ​	括号法

- ​	显示法

- ​	隐式转换法

```c++
//括号法
Personal p();
//显示法;
Personal p=Personal();
//匿名对象 当前行执行结束后，系统会立即回收
//如果用拷贝构造匿名对象会报错
Personal();
//隐式转换法
Personal p=10;//等价于Personal p=Personal(10);
```

#### 构造函数调用规则

一个类默认添加三个函数

1.默认构造函数

2.默认析构函数

3.默认拷贝构造函数,对属性进行值拷贝

#### 深拷贝与浅拷贝

浅拷贝：简单的赋值拷贝操作，如果是引用类型则指针地址相同

深拷贝：在堆区重新申请空间，进行拷贝操作，不同内存地址

```c++
class Personal {
	int age;
	int *height;
public:
	Personal(int a,int b) {
		age = a;//浅拷贝
		height = new int(b);//深拷贝
	}
	~Personal() {
		if (height != NULL) {
			delete height;//手动释放
			height = NULL;
		}
	}
};
```
#### 初始化列表

格式：`类名():属性名(值1),属性名(值2){}`

```c++
class Personal{
	int age;
    int height;
    Personal(int a,int h):age(a),height(h){}
}
```
#### 类外构造函数

``` c++
class Building{
    Building();
}
//一般类内声明构造函数，类外实现。封装在不同文件内
Building::Building(){
}
```


#### 静态成员函数

静态函数特征：

- 所有对象共享一个函数
- 静态函数只能访问静态成员变量

```c++
//通过对象访问
Person p;
p.func();
//通过类名访问
Personal::func();
```

### 对象模型和this指针

#### 成员变量和成员函数分开存储空对象

空对象占用1个字节，C++编译器会给每个空对象也分配一个字节空间，为了区分空对象占内存的位置

当`sizeof(对象);`时，只会显示成员变量的大小或空对象的大小，会进行内存对齐。而静态成员 和成员函数是分开存储的

#### this指针

**this指针指向被调用的成员函数所属的对象**

this指针是指针常量 指针的指向是不可以修改的

那个对象调用this，this就指向谁

this指针隐含在每一个非静态成员函数内

this指针不需要定义，直接使用即可

返回对象本身时，使用`return *this`

```c++
class Person{
	int age;
	Person(int age){
        this->age=age;
	}
}
```

#### 空指针访问成员函数

空指针访问的成员函数不能带有成员变量

```c++
class Person {
public:
	void ShowClassName() {
		cout << "this is Person class" << endl;
	}
    int age;
    void ShowClassAge(){
        //预防空指针报错
        if(this==NULL)
        {
            return;
        }
        cout<<age<<endl;
    }
};
int main() {
	Person *p = NULL;
	p->ShowClassName();
}
```

#### const修饰成员函数

常函数：

- 成员函数后加const后我们成为这个函数为**常函数**
- 常函数内不可以修改成员属性
- 成员属性声明时加关键字`mutable`后，在常函数中依然可以修改

```c++
void func() const{ //等价于 const 类名* const this
    //方法内不能修改成员属性
}
```

##### mutable关键字

```c++
class Person {
public:
	mutable int age;
	void ShowClassName() const
	{
        //在常函数中依旧可以修改
		age = 123;
	}
};
```

常对象：

- 声明对象前加const称改对象为常对象(不能修改成员属性 除非使用mutable关键字)
- 常对象只能调用常函数

`const 类名 变量名;`

### 友元

友元的目的是让一个函数或者类访问另一个私有成员

关键字：`friend`

友元的三种实现

- 全局函数做友元
- 类做友元
- 成员函数做友元

``` c++
class Building{
	//友元函数 可以访问该类的私有成员
	friend void Func(Building building);
    //友元类
    friend class Goods;
    //友元成员函数
    friend void Goods::visit();
private:
	string m_BedRoom;
}
```
### 运算符重载

#### +运算符重载

方法名：`operator运算符(){}`

##### 1.通过成员函数重载+号

```c++
class Person{
public:
    Person operator+(Person &p){
		Person temp;
        temp.m_A=this->m_A+p.m_A;
        return temp;
    }
    int m_A;
}
```

##### 2.全局函数重载+号

```c++
Person operator+(Person &p1,Person &p2){
		Person temp;
        temp.m_A=p1.m_A+p2.m_A;
        return temp;
}
```

#### 左移运算符重载

作用：`可以输出自定义数据类型`

 不会利用成员函数重载<<运算符，无法实现cout在左侧

```c++
ostream operator<<(ostream &cout,Person &p){
	cout<<"m_A="<<p.m_A<<"m_B="<<p.m_B;
    return cout;
}
cout<<p<<endl;
```

#### 递增运算符重载

##### 前置++运算符

```c++
//成员函数重载
MyInteger& operator++(){
	m_Num++;
    return *this;
}
```

##### 后置++运算符

通过占位参数区分前置后置，使用占位参数的是后置++

```c++
//成员函数重载
//使用占位参数区分后置++运算符
MyInteger operator++(int){
    //先记录当时结果
	MyInteger temp=*this;
    m_Num++;
    //返回旧值，由于c++的后置++不能链式使用，此处只是显示数值作用
    return temp;
}
```

#### =运算符重载
一般用于处理=需要进行深拷贝时进行的重载

```c++
//成员函数重载
MyInteger& operator=(Person &p){
	//先判断是否有属性在堆区，如果有需要先释放干净，然后再深拷贝
    if(m_Age!=NULL){
        delete m_Age;
        m_Age=NULL;
	}
    //深拷贝
    m_Age= new int(*p.m_Age);
    return *this;
}
```

#### 关系运算符重载

```c++
bool operator==(Person &p){
	if(this->m_Name==p._Name&&this->m_Age==p.m_Age){
        return true;
    }
    return false;
}
```
#### 函数调用运算符重载-仿函数

- 函数调用运算符()也可以重载
- 由于重载后使用的方法非常像函数的调用，因此称为仿函数
- 仿函数没有固定写法，非常灵活

仿函数就是重写()
``` C++
class MyPrint{
    void operator()(string test){
        cout<<test<<endl;
    }
}
MyPrint myPrint;
myPrint("hello world");
```

仿函数非常灵活没有固定的写法，可以任意扩展

```c++
//匿名对象函数
MyPrint()("hello");
```

