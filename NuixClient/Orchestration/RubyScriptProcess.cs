﻿using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NuixClient.Orchestration
{
    internal abstract class RubyScriptProcess1 : Process
    {
        //TODO make a config property
        private const string NuixExeConsolePath = @"C:\Program Files\Nuix\Nuix 7.8\nuix_console.exe";
        //TODO make a config property
        private const bool UseDongle = true;
        /// <summary>
        /// Checks if the current set of arguments is valid
        /// </summary>
        /// <returns></returns>
        internal abstract IEnumerable<string> GetArgumentErrors();
        
        internal abstract string ScriptName { get; }

        internal abstract IEnumerable<(string arg, string val)> GetArgumentValuePairs();

        public override async IAsyncEnumerable<ResultLine> Execute()
        {
            var argumentErrors = GetArgumentErrors().ToList();

            if (argumentErrors.Any())
            {
                foreach (var ae in argumentErrors)
                    yield return new ResultLine(false, ae);
                yield break;
            }

            var currentDirectory = Directory.GetCurrentDirectory();
            var scriptPath = Path.Combine(currentDirectory, "..", "nuixclient", "NuixClient", "Scripts", ScriptName);
            
            var args = new List<string>();

            foreach (var (key, value) in GetArgumentValuePairs())
            {
                args.Add(key);
                args.Add(value);
            }

            var result = ScriptRunner. RunScript(NuixExeConsolePath, scriptPath, UseDongle, args);

            await foreach (var line in result)
                yield return line;
        }

        public override bool Equals(object? obj)
        {
            return obj is RubyScriptProcess1 rsp && ScriptName == rsp.ScriptName &&
                   GetArgumentValuePairs().SequenceEqual(rsp.GetArgumentValuePairs());
        }

        public override int GetHashCode()
        {
            var t = 2;

            unchecked
            {
                t += 3 * GetType().GetHashCode();

                // ReSharper disable once LoopCanBeConvertedToQuery - possible overflow exception
                foreach (var argumentValuePair in GetArgumentValuePairs()) 
                    t += argumentValuePair.GetHashCode();
            }
            

            return t;
        }
    }
}