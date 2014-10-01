using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;
using IronRuby;
using Nebulas;


namespace Nebulas
{
    namespace Scripting
    {
        public class RubyCore
        {
            protected ScriptRuntime mRuntime;
            protected ScriptEngine mEngine;
            protected ScriptScope mScope;
            protected Dictionary<Int32, CompiledCode> mBinaries;
            protected List<String> mPatches;
            public const String TargetVarName = "this";
            public const String BinaryDir = "compiled";
            //public String mPath;
            //CompiledCode mContextPatch;


            public void Initialize()
            {
                //mPath = "./scripts";
                mRuntime = Ruby.CreateRuntime();
                mEngine = mRuntime.GetEngine("ruby");
                
                mScope = mEngine.CreateScope();
                //mContextPatch = null;
                mPatches = new List<String>();
                mBinaries = new Dictionary<Int32, CompiledCode>();
            }

            public void DumpCurrentScope()
            {
                Console.WriteLine("Scope Variables");
                foreach(string name in mScope.GetVariableNames())
                {
                    Console.WriteLine(name);
                }
            }
            
            public CompiledCode Compile(string source)
            {
                Int32 hash = source.GetHashCode();
                CompiledCode binary;
                if (mBinaries.TryGetValue(hash, out binary))
                {
                    return binary;
                }
                else
                {
                    ScriptSource result;
                    if ((source.Length > 3) && (source.Substring(source.Length - 3) == ".rb"))
                    {
                        source = File.ReadAllText(Config.GetScriptPath(source));
                    }
                    foreach(String patch in mPatches)
                    {
                        source = patch + source;
                    }
                    result =  mEngine.CreateScriptSourceFromString(source, SourceCodeKind.Statements);
                    
                    binary = result.Compile();
                    mBinaries.Add(hash, binary);
                    return binary;
                }
            }

            protected CompiledCode ComplilePatch(Nebulas.Events.Event evt)
            {
                CompiledCode bin = Compile(UnrollProperties(evt));
                return bin;
            }
            
            public void Exec(string script)
            {
                Compile(script).Execute(mScope);
            }

            public void ExecString(string scriptContents)
            {
                try
                {
                    Compile(scriptContents).Execute(mScope);
                }
                catch(SyntaxErrorException e)
                {
                    ExceptionOperations eo;
                    eo = mEngine.GetService<ExceptionOperations>();
                    string error = eo.FormatException(e);

                    string msg = "Syntax error in \"{0}\"";
                    Console.WriteLine(msg + ":\n" + error);
                }
            }

            public void ExecFile(string scriptFile)
            {
                Compile(scriptFile).Execute(mScope);
            }

            public void Run(string script)
            {
                mEngine.Execute(script, mScope);
            }

            public void SetContext(Nebulas.Events.Event evt)
            {
                mScope = mEngine.CreateScope();
                SetVariable("event", evt.mProperties);
            }

            public Boolean ExtractContext(Nebulas.Events.Event evt)
            {
                dynamic thisvar = GetVariable("event");
                if (thisvar != null)
                {
                    evt.UpdateProperties((Dictionary<string, string>)thisvar);
                    return true;
                }
                else
                {
                    return false;
                }
            }

            protected String UnrollProperties(Nebulas.Events.Event evt)
            {
                String values = TargetVarName + " = {}";
                foreach (KeyValuePair<string, string> kvp in evt.mProperties)
                {
                    values += "\n" + TargetVarName + "." + kvp.Key + " = " + kvp.Value;
                }
                return values;
            }

            public Int32? GetAsInteger(string key)
            {
                //dynamic val;
                if (mScope.ContainsVariable(key))
                {
                    return mScope.GetVariable<Int32>(key);
                }
                else
                {
                    return null;
                }
              
            }

            public String GetLocal(string key)
            {
                dynamic value;
                try
                {
                    value = mEngine.Execute(key, mScope);
                }
                catch(System.MissingMethodException)
                {
                    return null;
                }
                return (String)value;
            }
            
            public void SetLocal(string key, string value)
            {
                mEngine.Execute(key + " = \"" + value + "\"", mScope);
            }

            public void SetVariable(string key, dynamic val)
            {
                mScope.SetVariable(key, val);
            }

            public void ExportDictionary(string key, Dictionary<String, String> dict)
            {
                //Build patch for dictionary
                String patch = key + "= { ";
                foreach(KeyValuePair<String, String> kvp in dict)
                {
                    patch += "\"" + kvp.Key + "\" => \"" + kvp.Value + "\",";
                }
                patch += "}\n";
                mPatches.Add(patch);
                mEngine.Execute(patch, mScope);
            }

            public void ApplyPatches()
            {
                foreach(String patch in mPatches)
                {
                    mEngine.Execute(patch, mScope);
                }
            }

            public void ClearPatches()
            {
                mPatches.Clear();
            }

            public Dictionary<String, String> ImportDictionary(string key)
            {
                Dictionary<String, String> import = new Dictionary<String,String>();
                dynamic hash = null;
                hash = mEngine.Execute(key, mScope);
                if(hash != null)
                {
                    foreach(KeyValuePair<Object, Object> kvp in hash)
                    {
                        import.Add(kvp.Key.ToString(), kvp.Value.ToString());
                    }
                }
                
                return import;
            }

            public dynamic GetVariable(string key)
            {
                return mScope.GetVariable(key);
            }

            public Int32? GetGlobalInteger(string key)
            {
                dynamic val;
                if (mEngine.Runtime.Globals.TryGetVariable(key, out val))
                {
                    return (Int32)val;
                }
                else
                {
                    return null;
                }
            }

            public String GetAsString(string key)
            {
                dynamic val;
                if (!mScope.TryGetVariable(key, out val))
                {
                    return null;
                }
                else
                {
                    return (String)val;
                }
                
            }

            public String GetGlobalString(string key)
            {
                dynamic val;
                if (mEngine.Runtime.Globals.TryGetVariable(key, out val))
                {
                    return (String)val;
                }
                else
                {
                    return null;
                }
            }

            public Double? GetAsDouble(string key)
            {
                dynamic val;
                if (!mScope.TryGetVariable(key, out val))
                {
                    return null;
                }
                else
                {
                    return (Double)mScope.GetVariable(key);
                }
                
            }

            public Double? GetGlobalDouble(string key)
            {
                dynamic val;
                if(mEngine.Runtime.Globals.TryGetVariable(key, out val))
                {
                    return (Double)val;
                }
                else
                {
                    return null;
                }
            }

            public String SanityTest(String before, String after)
            {
                String code = "self.msg = '" + after + "'.to_clr_string";
                ScriptSource script = mEngine.CreateScriptSourceFromString(code, SourceCodeKind.Statements);
                SetVariable("msg", before);
                script.Execute(mScope);
                String result = GetVariable("msg");
                return result;
            }
            
        }
    }
}
