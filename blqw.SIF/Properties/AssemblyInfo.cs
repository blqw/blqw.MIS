using System.Resources;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// 有关程序集的一般信息由以下
// 控制。更改这些特性值可修改
// 与程序集关联的信息。
[assembly: AssemblyTitle("blqw.SIF")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("blqw.SIF")]
[assembly: AssemblyCopyright("版权所有(C)  2016")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: NeutralResourcesLanguage("zh-Hans")]

// 程序集的版本信息由下列四个值组成: 
//
//      主版本
//      次版本
//      生成号
//      修订号
//
//可以指定所有这些值，也可以使用“生成号”和“修订号”的默认值，
// 方法是按如下所示使用“*”: :
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion(VersionString.VERSION + ".*")]
[assembly: AssemblyFileVersion(VersionString.VERSION + VersionString.BUG_FIX)]
[assembly: AssemblyInformationalVersion(VersionString.VERSION + VersionString.BUG_FIX + (VersionString.BETA ? "-beta" : ""))]

internal static class VersionString
{
    public const string VERSION = "0.0.1";
    public const string BUG_FIX = ".0";
    public const bool BETA = true;
}
