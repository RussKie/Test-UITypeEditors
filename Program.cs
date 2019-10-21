using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace NetFxApp
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());

            return;


            var allTypes = AppDomain.CurrentDomain.GetAssemblies().SelectMany(asm => asm.DefinedTypes).ToList();
            var map = new Dictionary<string, HashSet<string>>();

            void Report(string editorTypeName, Type type, PropertyInfo prop)
            {
                string editorTypeFullName = GetEditorTypeName(editorTypeName);
                string editorTypeAssemblyFullName = GetEditorTypeAssemblyName(editorTypeName);
                if (!map.TryGetValue(editorTypeFullName, out var list))
                {
                    map.Add(editorTypeFullName, list = new HashSet<string>());
                }

                string subject = $"{editorTypeAssemblyFullName}\t{type.FullName}\t{prop?.Name}\t{type.Assembly.FullName}";
                list.Add(subject);
            }

            string GetEditorTypeName(string editorTypeName)
            {
                if (editorTypeName.Length < 1)
                {
                    return string.Empty;
                }

                return editorTypeName.Substring(0, editorTypeName.IndexOf(','));
            }

            string GetEditorTypeAssemblyName(string editorTypeName)
            {
                if (editorTypeName.Length < 1)
                {
                    return string.Empty;
                }

                return editorTypeName.Substring(editorTypeName.IndexOf(',') + 1);
            }

            Console.WriteLine("Starting");

            foreach (var type in allTypes)
            {
                foreach (EditorAttribute editorAttr in type.GetCustomAttributes(typeof(EditorAttribute), false))
                {
                    //Console.WriteLine($"{type.Name} --> {editorAttr.EditorTypeName}");

                    if (editorAttr != null) // && Type.GetType(editorAttr.EditorTypeName) == null)
                    {
                        Report(editorAttr.EditorTypeName, type, null);
                    }
                }

                foreach (var prop in type.DeclaredProperties)
                {
                    foreach (EditorAttribute editorAttr in prop.GetCustomAttributes(typeof(EditorAttribute), false))
                    {
                        //Console.WriteLine($"\t{prop.Name} --> {editorAttr.EditorTypeName}");

                        if (editorAttr != null) // && Type.GetType(editorAttr.EditorTypeName) == null)
                        {
                            Report(editorAttr.EditorTypeName, type, prop);
                        }
                    }
                }
            }

            Console.WriteLine("\r\nConsolidated");

            var sb = new StringBuilder();
            foreach (var pair in map)
            {
                //Console.WriteLine($"{pair.Key}");
                foreach (var x in pair.Value)
                {
                    sb.AppendLine($"{pair.Key}\t{x}");
                }
            }

            File.WriteAllText(@"C:\Users\igveliko\Desktop\uitypeeditors.txt", sb.ToString());

            //Console.ReadKey();
        }
    }
}
