//net-port.js v 0.0.0 -- 1/9/2019
'use strict';
const EventEmitter = require('events').EventEmitter;

class Net_Port extends EventEmitter{
	constructor(){
		super();
	}
	static connect(){
		return new Net_Port();
	}
}

module.exports = Net_Port;