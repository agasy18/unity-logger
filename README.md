# Unity Logger
### Log and assert everywhere in your code.

## Instalation
Download and put  `Logger.cs` file in root (Assets).

## Usage

use log everywhere even in expressions 
``` C#
int x = 5;
x.Log("my x");
if (5+6+y.Log("y")<35)
{
  //do something
}
```

use log to print single object or multiple objects
``` C#
"hello".Log();
myObj.Log("My object and some", x, y, d, f);
```

specify your log type by using LogType
``` C#
param.Log("Parameter is so big", obj1,obj2, ..., Logger.LogType.Warning);
```


use assert to for easy debuging
it will log and break or throw AssertionException on debug
``` C#
(x<5).Assert("Assert if x is not less then 5")

//assert if obj is null
SomeMethod(obj.Assert("obj mast be not null"));
```
