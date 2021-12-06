# C++基础入门

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
    cout<<"a="<<a<<endl;
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

