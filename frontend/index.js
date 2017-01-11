"use strict";


var DB = {};

function boot() {
    var clock = new DB.clock(document.getElementById("clock"));
    clock.start();
}


DB.clock = class {
    constructor(root) {
        this._root = root;
    }

    start() {
        setTimeout(this.setTime.bind(this), 500);
    }

    setTime() {
        var today = new Date();
        var h = today.getHours();
        var m = today.getMinutes();
        var s = today.getSeconds();
        m = this._checkTime(m);
        s = this._checkTime(s);
        this._root.innerHTML = h + ":" + m + ":" + s;
        setTimeout(this.setTime.bind(this), 500);
    }

    _checkTime(i) {
        if (i < 10) {i = "0" + i};  // add zero in front of numbers < 10
        return i;
    }
}
