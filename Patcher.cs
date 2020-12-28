using System;
using BepInEx;
using Mono.Cecil;
using System.IO;
using Mono.Cecil.Cil;
using BepInEx.Logging;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Dragons.PublicDragon
{
    public static class Patcher
    {
        public static void Patch(AssemblyDefinition assembly)
        {
            ModuleDefinition module = assembly.MainModule;
            TypeDefinition IgnoresAccessChecksToAttribute = new TypeDefinition("System.Runtime.CompilerServices", "IgnoresAccessChecksToAttribute", TypeAttributes.Public | TypeAttributes.BeforeFieldInit, module.ImportReference(typeof(Attribute)));
            IgnoresAccessChecksToAttribute.CustomAttributes.Add(new CustomAttribute(module.ImportReference(typeof(AttributeUsageAttribute).GetConstructor(new Type[] { typeof(AttributeTargets) } )), new byte[] { 1, 0, 1, 0, 0, 0, 1, 0, 84, 2, 13, 65, 108, 108, 111, 119, 77, 117, 108, 116, 105, 112, 108, 101, 1 }));
            FieldDefinition backingField = new FieldDefinition("<AssemblyName>k__BackingField", FieldAttributes.Private | FieldAttributes.InitOnly, module.TypeSystem.String);
            IgnoresAccessChecksToAttribute.Fields.Add(backingField);
            backingField.CustomAttributes.Add(new CustomAttribute(module.ImportReference(typeof(CompilerGeneratedAttribute).GetConstructor(new Type[0]))));
            MethodDefinition ctor = new MethodDefinition(".ctor", MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName, module.TypeSystem.Void);
            ctor.Parameters.Add(new ParameterDefinition("assemblyName", ParameterAttributes.None, module.TypeSystem.String));
            ILProcessor il = ctor.Body.GetILProcessor();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Call, module.ImportReference(typeof(Attribute).GetConstructor(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance, null, new Type[0], null)));
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldarg_1);
            il.Emit(OpCodes.Stfld, backingField);
            il.Emit(OpCodes.Ret);
            IgnoresAccessChecksToAttribute.Methods.Add(ctor);
            MethodDefinition get_AssemblyName = new MethodDefinition("get_AssemblyName", MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName, module.TypeSystem.String);
            il = get_AssemblyName.Body.GetILProcessor();
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Ldfld, backingField);
            il.Emit(OpCodes.Ret);
            get_AssemblyName.CustomAttributes.Add(new CustomAttribute(module.ImportReference(typeof(CompilerGeneratedAttribute).GetConstructor(new Type[0]))));
            IgnoresAccessChecksToAttribute.Methods.Add(get_AssemblyName);
            PropertyDefinition AssemblyName = new PropertyDefinition("AssemblyName", PropertyAttributes.None, module.TypeSystem.String);
            AssemblyName.GetMethod = get_AssemblyName;
            IgnoresAccessChecksToAttribute.Properties.Add(AssemblyName);
            module.Types.Add(IgnoresAccessChecksToAttribute);
            
            module.CustomAttributes.Add(new CustomAttribute(module.ImportReference(typeof(System.Security.UnverifiableCodeAttribute).GetConstructor(new Type[0]))));
            
            SecurityDeclaration dec = new SecurityDeclaration(SecurityAction.RequestMinimum);
            SecurityAttribute attr = new SecurityAttribute(module.ImportReference(typeof(System.Security.Permissions.SecurityPermissionAttribute)));
            attr.Properties.Add(new CustomAttributeNamedArgument("SkipVerification", new CustomAttributeArgument(module.TypeSystem.Boolean, true)));
            dec.SecurityAttributes.Add(attr);
            assembly.SecurityDeclarations.Add(dec);
            
            assembly.CustomAttributes.Add(new CustomAttribute(ctor, new byte[] { 1, 0, 15, 65, 115, 115, 101, 109, 98, 108, 121, 45, 67, 83, 104, 97, 114, 112, 0, 0 }));
        }
        
        public static IEnumerable<string> FindTargetDLLs()
        {
            List<string> result = new List<string>();
            foreach (string dll in Directory.GetFiles(Path.Combine(Paths.BepInExRootPath, "plugins"), "*.dll"))
            {
                string dllName = Path.GetFileName(dll);
                result.Add(dllName);
            }
            return result;
        }
        
        public static IEnumerable<string> TargetDLLs
        {
            get
            {
                return FindTargetDLLs();
            }
        }
        
        public static ManualLogSource logger = Logger.CreateLogSource("Dragons.PublicDragon");
        public static bool initialized;
        
        public static string updateURL = "http://beestuff.pythonanywhere.com/audb/api/mods/0/19";
        public static int version = 0;
        public static string keyE = "AQAB";
        public static string keyN = "yu7XMmICrzuavyZRGWoknFIbJX4N4zh3mFPOyfzmQkil2axVIyWx5ogCdQ3OTdSZ0xpQ3yiZ7zqbguLu+UWZMfLOBKQZOs52A9OyzeYm7iMALmcLWo6OdndcMc1Uc4ZdVtK1CRoPeUVUhdBfk2xwjx+CvZUlQZ26N1MZVV0nq54IOEJzC9qQnVNgeeHxO1lRUTdg5ZyYb7I2BhHfpDWyTvUp6d5m6+HPKoalC4OZSfmIjRAi5UVDXNRWn05zeT+3BJ2GbKttwvoEa6zrkVuFfOOe9eOAWO3thXmq9vJLeF36xCYbUJMkGR2M5kDySfvoC7pzbzyZ204rXYpxxXyWPP5CaaZFP93iprZXlSO3XfIWwws+R1QHB6bv5chKxTZmy/Imo4M3kNLo5B2NR/ZPWbJqjew3ytj0A+2j/RVwV9CIwPlN4P50uwFm+Mr0OF2GZ6vU0s/WM7rE78+8Wwbgcw6rTReKhVezkCCtOdPkBIOYv3qmLK2S71NPN2ulhMHD9oj4t0uidgz8pNGtmygHAm45m2zeJOhs5Q/YDsTv5P7xD19yfVcn5uHpSzRIJwH5/DU1+aiSAIRMpwhF4XTUw73+pBujdghZdbdqe2CL1juw7XCa+XfJNtsUYrg+jPaCEUsbMuNxdFbvS0Jleiu3C8KPNKDQaZ7QQMnEJXeusdU=";
    }
}