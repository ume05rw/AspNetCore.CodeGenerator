Custom code generator that works with dotnet aspnet-codegenerator
===

I built it by referring to this: [prafullbhosale/CustomScaffolder](https://github.com/prafullbhosale/CustomScaffolder).


### Steps to build. 
1. Run `dotnet restore` in the `XbScaffolder\` directory.
2. Run `dotnet pack -o ..\packages /p:PackageVersion=0.0.1-dev-1` in the `XbScaffolder\` directory.
3. Run `dotnet restore` in the `App` directory.
4. Add NuGet-Packages `XbScaffolder-0.0.1-dev-1.nupkg` to `App` project.
5. Run `dotnet aspnet-codegenerator -p . xb -m Shop -dc Db -name ShopsController` directory.
  
  
-> These files will be added to App-project...  

* App\Controller\ShopContorller.cs
* App\Models\Stores\Shop.cs
* App\Views\Shops\Index.cshtml
* App\Views\Shops\Details.cshtml
* App\Views\Shops\Edit.cshtml

### Requirements
1. dotnet cli

### Sample executions


```
C:\src\App>dotnet aspnet-codegenerator -p . xb -m Shop -dc Db -name ShopsController
Command Line: -p . xb -m Shop -dc Db -name ShopsController
.NET Core 向け Microsoft (R) Build Engine バージョン 15.4.8.50001
Copyright (C) Microsoft Corporation.All rights reserved.

Building project ...
.NET Core 向け Microsoft (R) Build Engine バージョン 15.4.8.50001
Copyright (C) Microsoft Corporation.All rights reserved.

  App -> C:\src\App\bin\Debug\netcoreapp1.1\App.dll

ビルドに成功しました。
    0 個の警告
    0 エラー

経過時間 00:00:03.38
Command Line: --no-dispatch --port-number 4699 -p . xb -m Shop -dc Db -name ShopsController --dispatcher-version 1.0.1-rtm
Finding the generator 'xb'...
Running the generator 'xb'...
version: 1.0.0.7
Model name: Shop
DbContext name: Db
Controller name: ShopsController
Attempting to compile the application in memory
Attempting to figure out the EntityFramework metadata for the model and DbContext: Shop
Added Controller : \.\Controllers\ShopsController.cs
Added View : \Views\Shops\Edit.cshtml
Added View : \Views\Shops\Details.cshtml
Added View : \Views\Shops\Index.cshtml
Added Model : \.\Models\Stores\Shops.cs
RunTime 00:00:11.67

C:\src\App>
```

### Links
Original:  
[https://github.com/prafullbhosale/CustomScaffolder](https://github.com/prafullbhosale/CustomScaffolder)  

aspnet/Scaffolding(ver 1.1.3):  
[https://github.com/aspnet/Scaffolding/tree/1.1.3](https://github.com/aspnet/Scaffolding/tree/1.1.3)
