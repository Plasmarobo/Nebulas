using System;
using System.Collections.Generic;
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
            //protected Dictionary<Int32, ScriptSource> mCache;
            public const String TargetVarName = "this";
            public const String BinaryDir = "compiled";
            CompiledCode mContextPatch;


            public void Initialize()
            {
                mRuntime = Ruby.CreateRuntime();
                mEngine = mRuntime.GetEngine("ruby");
                
                mScope = mEngine.CreateScope();
                mContextPatch = null;
                mBinaries = new Dictionary<Int32, CompiledCode>();
                //mCache = new Dictionary<Int32,ScriptSource>();
            }

            public void DumpCurrentScope()
            {
                Console.WriteLine("Scope Variables");
                foreach(string name in mScope.GetVariableNames())
                {
                    Console.WriteLine(name);
                }
            }
            
            // Iron Ruby doesn't support comp
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
                    //Engage Compiler
                    ScriptSource result; 
                    if ((source.Length > 3) && (source.Substring(source.Length-3) == ".rb"))
                    {
                        result = mEngine.CreateScriptSourceFromFile(source, Encoding.UTF8, SourceCodeKind.File);
                    }
                    else
                    {
                        result =  mEngine.CreateScriptSourceFromString(source, SourceCodeKind.Statements);
                    }
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
            /*
            public ScriptSource Compile(String source)
            {
                Int32 hash = source.GetHashCode();
                ScriptSource src;
                if(mCache.TryGetValue(hash, out src))
                {
                    return src;
                }
                else
                {
                    //Pesudocompilation
                    ScriptSource result;
                    if ((source.Length > 3) && (source.Substring(source.Length - 3) == ".rb"))
                    {
                        result = mEngine.CreateScriptSourceFromFile(source, Encoding.UTF8, SourceCodeKind.File);
                    }
                    else
                    {
                        result = mEngine.CreateScriptSourceFromString(source, SourceCodeKind.Statements);
                    }
                    mCache.Add(hash, result);
                    return result;
                }
            }

            protected ScriptSource CompilePatch(Nebulas.Events.Event evt)
            {
                return Compile(UnrollProperties(evt));
            }*/

            public void Exec(string script)
            {
                if (mContextPatch != null)
                {
                    mContextPatch.Execute(mScope);
                }
                Compile(script).Execute(mScope);
            }

            public void ExecString(string scriptContents)
            {
                try
                {
                    if (mContextPatch != null)
                    {
                        mContextPatch.Execute(mScope);
                    }
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
                if (mContextPatch != null)
                {
                    mContextPatch.Execute(mScope);
                }
                
                Compile(scriptFile).Execute(mScope);
            }

            public void SetContext(Nebulas.Events.Event evt)
            {
                mScope = mEngine.CreateScope();
                SetVariable("this", evt.mProperties);
            }

            public Boolean ExtractContext(Nebulas.Events.Event evt)
            {
                mContextPatch = null;
                dynamic thisvar = GetVariable("this");
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

            public void SetVariable(string key, dynamic val)
            {
                mScope.SetVariable(key, val);
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
                /*ScriptScope scope = mEngine.CreateScope();
                String code = "puts self.msg\nself.msg = '" + after + ".to_clr_string'\n";
                ScriptSource script = mEngine.CreateScriptSourceFromString(code, SourceCodeKind.Statements);
                SetVariable("msg", before);
                script.Execute(scope);
                String result = GetVariable("msg");
                return result;
                 */
                //String before = "hello world";
                //String after = "changed";

                //ScriptRuntime mRuntime = Ruby.CreateRuntime();
                //ScriptEngine mEngine = mRuntime.GetEngine("ruby");
                ScriptScope scope = mEngine.CreateScope();
                String code = "self.msg = '" + after + "'.to_clr_string";
                ScriptSource script = mEngine.CreateScriptSourceFromString(code, SourceCodeKind.Statements);
                SetVariable("msg", before);
                script.Execute(mScope);
                String result = scope.GetVariable("msg");
                return result;
            }
            
        }
    }
}
