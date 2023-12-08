using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ButtonsModels.Blockly
{
    public class BlocklyToolbox
    {
        [JsonPropertyName("kind")]
        public string Kind { get; set; } = "flyoutToolbox";

        [JsonPropertyName("contents")]
        public IEnumerable<BlocklyBlock> Blocks { get; set; } = new List<BlocklyBlock>();
    }
}
