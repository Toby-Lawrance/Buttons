using ButtonsModels.Blockly;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;

namespace ButtonsGameServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlocklyController : ControllerBase
    {
        [HttpGet("toolbox")]
        public async Task<BlocklyToolbox> GetToolBox()
        {
            var box = new BlocklyToolbox();
            box.Kind = "flyoutToolbox";
            box.Blocks = new[]
            {
                BlocklyBlock.Predefined.Controls.IF,
                BlocklyBlock.Predefined.Controls.REPEAT,
                BlocklyBlock.Predefined.Text.TEXT,
                BlocklyBlock.Predefined.Text.TEXTPRINT,
                new BlocklyBlock {Type="useralert"}
            };

            return box;
        }

        [HttpGet("definitions")]
        public async Task<string> GetDefinitions()
        {
            var files = Directory.EnumerateFiles(@"Blockly\");
            var totalContents = new List<string>();
            foreach (var file in files) {
                var contents = await System.IO.File.ReadAllTextAsync(file);
                totalContents.Add(contents);
            }
            return $"{string.Join(',', totalContents)}";
        }
    }
}
