using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// 有关程序集的常规信息通过以下
// 特性集控制。更改这些特性值可修改
// 与程序集关联的信息。
#if OEM
[assembly: AssemblyTitle("柜台管理端")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("期货柜台开发组")]
[assembly: AssemblyProduct("柜台管理端")]
[assembly: AssemblyCopyright("Copyright © 开发组 2014")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

#else
[assembly: AssemblyTitle("资管系统管理端")]
[assembly: AssemblyDescription("资管系统管理端")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("天财邦网络科技(上海)有限公司")]
[assembly: AssemblyProduct("资管系统管理端")]
[assembly: AssemblyCopyright("Copyright © 天财邦  2014")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]


#endif

// 将 ComVisible 设置为 false 使此程序集中的类型
// 对 COM 组件不可见。如果需要从 COM 访问此程序集中的类型，
// 则将该类型上的 ComVisible 特性设置为 true。
[assembly: ComVisible(false)]

// 如果此项目向 COM 公开，则下列 GUID 用于类型库的 ID
[assembly: Guid("b4154337-7eb9-4ef1-9973-df5710c1fb4a")]

// 程序集的版本信息由下面四个值组成:
//
//      主版本
//      次版本 
//      内部版本号
//      修订号
//
// 可以指定所有这些值，也可以使用“内部版本号”和“修订号”的默认值，
// 方法是按如下所示使用“*”:
// [assembly: AssemblyVersion("1.0.*")]
#if OEM
[assembly: AssemblyVersion("0.7.3.0")]//程序集版本
[assembly: AssemblyFileVersion("0.7.3.0")]//文件版本
[assembly: AssemblyInformationalVersion("0.7.0.0")]//产品版本
#else
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]
#endif