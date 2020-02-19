﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using YamlDotNet.Serialization;

namespace NuixClient.Orchestration
{
    /// <summary>
    /// A process which searches a case with a particular search string and adds all items it finds to a production set.
    /// Will create a new production set if one with the given name does not already exist.
    /// </summary>
    internal class AddToProductionSetProcess : RubyScriptProcess
    {
        /// <summary>
        /// The name of this process
        /// </summary>
        public override string GetName() => $"Search and add to production set '{ProductionSetName}'";


        /// <summary>
        /// The production set to add results to. Will be created if it doesn't already exist
        /// </summary>
        [DataMember]
        [Required]
        [YamlMember(Order = 3)]
#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
        public string ProductionSetName { get; set; }


        /// <summary>
        /// The term to search for
        /// </summary>
        [DataMember]
        [Required]
        [YamlMember(Order = 4)]
        public string SearchTerm { get; set; }

        /// <summary>
        /// The path of the case to search
        /// </summary>
        [DataMember]
        [Required]
        [YamlMember(Order = 5)]
        public string CasePath { get; set; }

#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.


        internal override IEnumerable<string> GetArgumentErrors()
        {
            var (searchTermParseSuccess, searchTermParseError, searchTermParsed) = Search.SearchParser.TryParse(SearchTerm);

            if (!searchTermParseSuccess || searchTermParsed == null)
            {
                yield return $"Error parsing search term: {searchTermParseError}";
            }
        }

        internal override string ScriptName => "AddToProductionSet.rb";
        internal override IEnumerable<(string arg, string val)> GetArgumentValuePairs()
        {
            yield return ("-p", CasePath);
            yield return ("-s", SearchTerm);
            yield return ("-n", ProductionSetName);
        }
    }
}