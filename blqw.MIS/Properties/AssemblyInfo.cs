using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Resources;

// 有关程序集的一般信息由以下
// 控制。更改这些特性值可修改
// 与程序集关联的信息。
[assembly: AssemblyTitle("blqw.MIS")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("blqw.MIS")]
[assembly: AssemblyCopyright("版权所有(C)  2017")]
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
// 可以指定所有值，也可以使用以下所示的 "*" 预置版本号和修订号
// 方法是按如下所示使用“*”: :
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion(blqw.MIS.Version.String)]
[assembly: AssemblyFileVersion(blqw.MIS.Version.String)]
[assembly: AssemblyInformationalVersion(blqw.MIS.Version.Full)]


// ReSharper disable once CheckNamespace
namespace blqw.MIS
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public static class Version
    {
        private const string PRE_VERSION = "0.1.0";
        private const string BUG_FIX = "0";
        private const string PREVIEW = "";

        public const string Full = PRE_VERSION + "." + BUG_FIX + PREVIEW;

        public const string String = PRE_VERSION + "." + BUG_FIX;


        public static int Major => GetVersion().Major;
        public static int Minor => GetVersion().Minor;
        public static int Build => GetVersion().Build;
        public static int Revision => GetVersion().Revision;

        private static System.Version _version;

        public static System.Version GetVersion() => _version ?? (_version = new System.Version(String));


    }
}