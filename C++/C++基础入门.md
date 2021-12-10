# C++基础入门

### 学习路线

计算机组成原理

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