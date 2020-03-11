﻿using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Reductech.EDR.Connectors.Nuix.Search;
using Reductech.EDR.Utilities.Processes;
using YamlDotNet.Serialization;

namespace Reductech.EDR.Connectors.Nuix.processes
{
    /// <summary>
    /// A process that removes particular items from a Nuix production set
    /// </summary>
    public sealed class NuixRemoveFromProductionSet : RubyScriptProcess
    {
        /// <inheritdoc />
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override string GetName() => "Remove items from Production Set";

        /// <summary>
        /// The production set to remove results from.
        /// </summary>
        [DataMember]
        [Required]
        [YamlMember(Order = 3)]
#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
        public string ProductionSetName { get; set; }


        /// <summary>
        /// The search term to use for choosing which items to remove.
        /// </summary>
        [DataMember]
        [YamlMember(Order = 4)]
        [DefaultValueExplanation("All items will be removed.")]
        [ExampleValue("Tag:sushi")]
        public string? SearchTerm { get; set; }

        /// <summary>
        /// The path of the case to search
        /// </summary>
        [DataMember]
        [Required]
        [YamlMember(Order = 5)]
        public string CasePath { get; set; }

        /// <inheritdoc />
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override IEnumerable<string> GetArgumentErrors()
        {
            if (SearchTerm != null)
            {
                var (searchTermParseSuccess, searchTermParseError, searchTermParsed) = SearchParser.TryParse(SearchTerm);

                if (!searchTermParseSuccess || searchTermParsed == null)
                {
                    yield return $"Error parsing search term: {searchTermParseError}";
                }
            }
        }
        

        internal override string ScriptName => "RemoveFromProductionSet.rb";
        internal override IEnumerable<(string arg, string val)> GetArgumentValuePairs()
        {
            yield return ("-p", CasePath);
            if(SearchTerm != null)
                yield return ("-s", SearchTerm);
            yield return ("-n", ProductionSetName);
        }
    }
}