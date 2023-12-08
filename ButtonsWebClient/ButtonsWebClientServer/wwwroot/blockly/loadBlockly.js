var workspace;
    
var supportedEvents = new Set([
    Blockly.Events.BLOCK_CHANGE,
    Blockly.Events.BLOCK_CREATE,
    Blockly.Events.BLOCK_DELETE,
    Blockly.Events.BLOCK_MOVE,
]);

function updateCode(event) {
    if (workspace.isDragging()) return; // Don't update while changes are happening.
    if (!supportedEvents.has(event.type)) return;

    const code = javascript.javascriptGenerator.workspaceToCode(workspace);
    var element = document.getElementById('textarea');
    element.textContent = code;
}

function initialiseBlockly(definitionsJson,toolboxJson)
{
    let definitions = JSON.parse(definitionsJson);
    Blockly.defineBlocksWithJsonArray(definitions);

    let toolbox = JSON.parse(toolboxJson);
    workspace = Blockly.inject('blocklyDiv', { 'toolbox': toolbox });

    workspace.addChangeListener(updateCode);
}