"use strict";

var DB = {};

function boot() {
    var clock = new DB.clock(document.getElementById("clock"));
    clock.start();
}

DB.clock = function(root) {
    this._root = root;
};

DB.clock.prototype.start = function() {
    setTimeout(this.setTime.bind(this), 500);
};

DB.clock.prototype.setTime = function() {
    var today = new Date();
    var h = today.getHours();
    var m = today.getMinutes();
    var s = today.getSeconds();
    m = this._checkTime(m);
    s = this._checkTime(s);
    this._root.innerHTML = h + ":" + m + ":" + s;
    setTimeout(this.setTime.bind(this), 500);
};

DB.clock.prototype._checkTime = function(i) {
    if (i < 10) {i = "0" + i};  // add zero in front of numbers < 10
    return i;
};
