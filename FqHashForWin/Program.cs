using System;
using System.Globalization;
using System.IO;
using System.Reflection;

namespace FqHashForWin
{
    // This is for creating single EXE that depends on many DLL files in C# using visual studio
    public class Program
    {
        /// <summary>
        /// 使用静态构造函数来绑定解析程序集失败事件
        /// </summary>
        static Program()
        {
            AppDomain.CurrentDomain.AssemblyResolve += OnResolveAssembly;
        }

        [STAThreadAttribute]
        public static void Main()
        {
            App.Main(); //启动WPF项目
        }

        //解析程序集失败，会加载对应的程序集
        private static Assembly OnResolveAssembly(object sender, ResolveEventArgs args)
        {
            Assembly executingAssembly = Assembly.GetExecutingAssembly();
            AssemblyName assemblyName = new AssemblyName(args.Name);

            var path = assemblyName.Name + ".dll";
            //判断程序集的区域性
            if (!assemblyName.CultureInfo.Equals(CultureInfo.InvariantCulture))
            {
                path = string.Format(@"{0}\{1}", assemblyName.CultureInfo, path);
            }

            using (Stream stream = executingAssembly.GetManifestResourceStream(path))
            {
                if (stream == null) return null;

                var assemblyRawBytes = new byte[stream.Length];
                stream.Read(assemblyRawBytes, 0, assemblyRawBytes.Length);
                return Assembly.Load(assemblyRawBytes);
            }
        }
    }
}
