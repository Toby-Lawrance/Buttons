javascript.javascriptGenerator.forBlock['useralert'] = function(block, generator) {
  var value_msg = generator.valueToCode(block, 'msg', javascript.Order.ATOMIC);
    // TODO: Assemble javascript into code variable.
    
    var code = `${value_msg} + " Called with ID: " + CallUserAPI()`;
  // TODO: Change ORDER_NONE to the correct strength.
  return [code, Blockly.JavaScript.ORDER_ATOMIC];
};