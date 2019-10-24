//db-port v 1.0.0 -- 1/6/2019
//db-port v 1.1.0 -- 1/10/2019 change the Find_User input;
//db-port v 2.0.0 -- 1/15/2019 add some services to support 'Record'; Change some function ports;
//db-port v 2.1.0 -- 1/24/2019 change function:Add_Record, add some other factors;(----it is a wrong try)
'use strict';
const EventEmitter = require('events').EventEmitter;
var MongoClient = require('mongodb').MongoClient;
var url = "mongodb://localhost:27017/";
var mdbo;
var mdb;


class db_port extends EventEmitter{

	constructor(){
		super();
	}

	ConnectDB(callback){
		MongoClient.connect(url, { useNewUrlParser: true },function(err, db) {
        if (err) throw err;
        mdbo = db.db("Flight-DB");
        mdb = db;
        console.log('DB Connected');
        callback();
      });
	}

//***************After this are codes about User Table***************//
  Add_User(self, username, password, callback){
    self.Get_Next_ID('users', (id)=>{
      var myobj = {_id: id, _name: username, _password: password };
      mdbo.collection("users").insertOne(myobj, function(err, res) {
        if (err) throw err;
        callback(id);
      });
    });
  }

  Find_User(name, callback){
    var myquery = {_name: name};
    mdbo.collection('users').find(myquery).toArray(function(err, result) {
        if (err) throw err;
        callback(result[0]);
    });
  }

  Update_User(id, newpassword, callback){
    var myquery = { _id: parseInt(id) };
    var newvalues = { $set: {_password: newpassword} };
    mdbo.collection('users').updateOne(myquery, newvalues, (err, res)=>{
      if(err) throw err;
      console.log('1 document updated.');
      callback();
    });
  }

  Delete_User(id, callback){//{user_id:...}
    var myquery = { _id: parseInt(id) };
    mdbo.collection("users").deleteOne(myquery, function(err, obj) {
      if (err) throw err;
      callback();
    });
  }
//*******************************************************************//
//***************After this are codes about Record Table*************//
  Add_Record(self, playerID, callback){
    self.Get_Next_ID('records', (id)=>{
      var myobj = {_id: id, _playerID: playerID.toString(), _starttime: Date.now(), _endtime: Date.now(), _AVGHeight: 0, _distance: 0};
      mdbo.collection('records').insertOne(myobj, function(err, res) {
        if (err) throw err;
        callback(id);
      });
    });
  }

  Find_Record(self, playerID, callback){
    var myquery = {_playerID: playerID.toString()};
      mdbo.collection('records').find(myquery).toArray(function(err, result){
        if (err) throw err;
        callback(result);
      });
  }

  Update_Record(RecordID, AVGHeight, Distance, callback){
    var myquery = { _id: parseInt(RecordID) };
    var newvalues = { $set: {_endtime: Date.now(), _AVGHeight: AVGHeight, _distance: Distance} };
    mdbo.collection('records').updateOne(myquery, newvalues, (err, res)=>{
      if(err) throw err;
      console.log('1 document updated.');
      callback();
    });
  }

  Delete_Record(recordID, callback){
    var myquery = { _id: parseInt(recordID) };
    mdbo.collection('records').deleteOne(myquery, function(err, obj){
        if (err) throw err;
        callback();
      });
  }
  //*******************************************************************//
  //***************After this are General Support Functions************//
  Get_Next_ID(tablename, callback){
    mdbo.collection(tablename).find({}, { projection: { _id: 1 } }).toArray(function(err, result) {
        if (err) throw err;
        console.log(result.length);
        if(result.length === 0){
          callback(0);
        }else{
          let id = result[result.length-1]._id;
          callback(id+1);
        }
    });
  }

  Look_DB(callback){
    mdbo.listCollections().toArray(function(err, collInfos) {
    callback(collInfos);
    });
  }

  Look_Table(tablename, callback){
        mdbo.collection(tablename).find({}).toArray(function(err, result) {
          if (err) throw err;
          callback(result);
        });
  }

  Close(){
    mdb.close();
  }
  //*******************************************************************//

	static connect(){
	    return new db_port();
	}
}

module.exports = db_port;