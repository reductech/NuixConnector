﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace NuixClient.Orchestration
{
    /// <summary>
    /// A process which searches a case with a particular search string and tags all files it finds
    /// </summary>
    public class SearchAndTag : IProcess
    {
        /// <summary>
        /// The name of this process
        /// </summary>
        public string Name => $"Search and Tag with '{Tag}'"; 

        /// <summary>
        /// Execute this process
        /// </summary>
        /// <returns></returns>
        public IAsyncEnumerable<ResultLine> Execute()
        {
            var r = OutsideScripting.SearchAndTag(CasePath, SearchTerm, Tag);

            return r;
        }

        /// <summary>
        /// The tag to assign to found results
        /// </summary>
        [DataMember]
        [Required]
#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
        public string Tag { get; set; }


        /// <summary>
        /// The term to search for
        /// </summary>
        [DataMember]
        [Required]
        public string SearchTerm { get; set; }

        /// <summary>
        /// The path of the case to search
        /// </summary>
        [DataMember]
        [Required]
        public string CasePath { get; set; }

        /// <summary>
        /// Conditions under which this process will execute
        /// </summary>
        public IReadOnlyCollection<ICondition> Conditions { get; }
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
    }
}