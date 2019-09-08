    /// <summary>
    /// Dynamically create a class with a specific number of properties
    /// </summary>
    public class DynamicRowFactory
    {
        public const string CLASS_NAME = "DynamicRow";
        public static List<object> CreateDynamicRows(int rows, int cols)
        {
            using (FileStream fout = new FileStream(CLASS_NAME + ".cs", FileMode.OpenOrCreate))
            using (StreamWriter writer = new StreamWriter(fout, Encoding.UTF8))
            {
                writer.WriteLine("using System;");
                writer.WriteLine("using System.ComponentModel;");
                writer.WriteLine("namespace TestField {");
                writer.WriteLine("public class " + CLASS_NAME + " : INotifyPropertyChanged {");
                writer.WriteLine("public event PropertyChangedEventHandler PropertyChanged;");
                for(int i = 0; i < cols; ++i)
                {
                    writer.WriteLine("public double Item" + i + " {");
                    writer.WriteLine("get { return DateTime.Now.Ticks; }");
                    writer.WriteLine("}");
                }
                writer.WriteLine("public void InvokePropertyChanged(params int[] indices) {");
                writer.WriteLine("if(PropertyChanged == null) return;");
                writer.WriteLine("foreach(int i in indices) PropertyChanged.Invoke(this, new PropertyChangedEventArgs(\"Item{i}\"));");
                writer.WriteLine("}");
                writer.WriteLine("}");
                writer.WriteLine("}");
            }

            CSharpCodeProvider provider = new CSharpCodeProvider();
            CompilerParameters cp = new CompilerParameters()
            {
                GenerateExecutable = false,
                GenerateInMemory = false,
                IncludeDebugInformation = true,
                OutputAssembly = CLASS_NAME + ".dll"
            };
            cp.ReferencedAssemblies.Add("System.dll");
            cp.ReferencedAssemblies.Add("System.ObjectModel.dll");
            CompilerResults result = provider.CompileAssemblyFromFile(cp, CLASS_NAME + ".cs");
            if (result.Errors.Count > 0)
            {
                foreach (CompilerError ce in result.Errors)
                {
                    Console.WriteLine("  {0}", ce.ToString());
                    Console.WriteLine();
                }
            }

            List<object> ret = new List<object>(rows);
            Assembly assembly = Assembly.LoadFrom(CLASS_NAME + ".dll");
            for(int i = 0; i < rows; ++i)
            {
                ret.Add(assembly.CreateInstance("TestField.DynamicRow"));
            }
            return ret;
        }
    }
