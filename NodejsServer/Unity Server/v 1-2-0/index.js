//Unity Server index.js
//v.1.2.0 -- 1/24/2019 Add Supports for the record upload; Create Record and Update Record
'use strict';

const mdb_port = require('./lib/db-port.js').connect();
var io = require('socket.io')(8078);
 
console.log('server start');
 
io.on('connection', function (socket) {
    console.log('client connection');
    
    //触发客户端注册的自定义事件
    socket.emit('ClientListener', { hello: 'world' });
    
    //注册服务器的自定义事件
    socket.on('ServerListener', function (data, callback) {
        console.log('ServerListener email:' + data['email']);
        callback({ abc: 123 });
    });

    mdb_port.ConnectDB(()=>{
    	socket.on('PassAuth', (data, callback)=>{
        	console.log('PassAuth Event, username:' + data['Username'] + ' PasswordHash:' + data['PasswordHash']);
        	//callback({res: 'true'});
        	mdb_port.Find_User(data['Username'], (res)=>{
        		//console.log("Find result:"+res);
        		if(res == null){
        			callback({res: "false"});
        		}else if(res._password !== data['PasswordHash']){
        			callback({res: "false"});
        		}else{
        			callback({res: "true"});
        		}
        	});
    	});

    	socket.on('CreateUser', (data, callback)=>{
        	console.log('CreateUser Event, username:' + data['Username'] + ' PasswordHash:' + data['PasswordHash']);
        	mdb_port.Add_User(mdb_port, data['Username'], data['PasswordHash'], (resid)=>{
        		console.log('new id:'+resid);
        		callback({res: 'true', id: resid});
        	});
    	});

        socket.on('GetRecord', (data, callback)=>{
            console.log('GetRecord Event:'+data);
            mdb_port.Find_User(data['Username'], (res)=>{
                console.log('userID:'+res._id);
                mdb_port.Find_Record(mdb_port, res._id, (recordres)=>{
                    let tempheight = 0;
                    let tempdistance = 0;
                    let temptime = 0;

                    for (var i = recordres.length - 1; i >= 0; i--) {
                        tempheight += parseInt(recordres[i]._AVGHeight);
                        tempdistance += parseInt(recordres[i]._distance);
                        temptime += (recordres[i]._endtime - recordres[i]._starttime);
                    }
                    temptime = temptime/1000;
                    console.log('length:'+recordres.length);
                    console.log('tempheight:'+tempheight);
                    console.log('temptime:'+temptime);
                    let transdata = { logintimes: recordres.length, playingtime: temptime, avgdis: tempdistance/recordres.length, avgheight: tempheight/recordres.length };
                    callback(transdata);
                });
            });
        });

        socket.on('CreateRecord', (data, callback)=>{
            console.log('CreateRecord Event:' + data);
            mdb_port.Find_User(data['Username'], (res)=>{
                mdb_port.Add_Record(mdb_port, res._id, (id)=>{
                    let transdata = {_id:id};
                    console.log('CreateRecord Event:' + id);
                    callback(transdata);
                });
            });
        });

        socket.on('UploadRecord', (data)=>{
            mdb_port.Update_Record(data['recordid'], data['AVGHeight'], data['distance'], ()=>{
                console.log('UploadRecord Event:'+data['recordid']+' '+data['AVGHeight']+' '+data['distance']);
            });
        });
    });

    
    
    //断开连接会发送
    socket.on('disconnect', function () {
        console.log('client disconnected');
    });
    
});


