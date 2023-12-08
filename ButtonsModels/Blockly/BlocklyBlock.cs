using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ButtonsModels.Blockly
{
    public class BlocklyBlock
    {
        [JsonPropertyName("kind")]
        public string Kind { get; set; } = "block";

        [JsonPropertyName("type")]
        public string Type { get; set; }


        public static class Predefined
        {
            public static class Controls
            {
                public static BlocklyBlock IF = new BlocklyBlock { Kind = "block", Type = "controls_if" };
                public static BlocklyBlock REPEAT = new BlocklyBlock { Kind = "block", Type = "controls_repeat_ext" };
            }

            public static class Logic
            {
                public static BlocklyBlock COMPARE = new BlocklyBlock { Kind = "block", Type = "logic_compare" };
            }

            public static class Math
            {
                public static BlocklyBlock NUMBER = new BlocklyBlock { Kind = "block", Type = "math_number" };
                public static BlocklyBlock ARITHMETIC = new BlocklyBlock { Kind = "block", Type = "math_arithmetic" };
            }

            public static class Text
            {
                public static BlocklyBlock TEXT = new BlocklyBlock { Kind = "block", Type = "text" };
                public static BlocklyBlock TEXTPRINT = new BlocklyBlock { Kind = "block", Type = "text_print" };
            }
        }
    }
}
