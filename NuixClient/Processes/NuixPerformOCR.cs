﻿using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using YamlDotNet.Serialization;

namespace Reductech.EDR.Connectors.Nuix.processes
{
    /// <summary>
    /// Performs optical character recognition on files which need it in a NUIX case.
    /// </summary>
    public sealed class NuixPerformOCR : RubyScriptProcess
    {
        /// <inheritdoc />
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override string GetName() => "RunOCR";

        /// <summary>
        /// The path to the case
        /// </summary>
        [Required]
        [DataMember]
        [YamlMember(Order = 3)]
#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
        public string CasePath { get; set; }


        /// <summary>
        /// The name of the OCR profile to use. If not provided, the default profile will be used
        /// </summary>
        [DataMember]
        [YamlMember(Order = 3)]
        public string? OCRProfileName { get; set; }

        /// <inheritdoc />
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override IEnumerable<string> GetArgumentErrors()
        {
            yield break;      
        }
        internal override string ScriptName => "RunOCR.rb";
        internal override IEnumerable<(string arg, string val)> GetArgumentValuePairs()
        {
            yield return ("-p", CasePath);
            if(OCRProfileName != null)
                yield return ("-o", OCRProfileName);
        }
    }
}