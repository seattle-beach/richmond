"use strict";

var DB = {};
var foodTruckWidget = {};

function boot() {
    var clock = new DB.clock(document.getElementById("clock"));
    clock.updateTime();
    foodTruckWidget = new DB.foodTruckWidget(document.getElementById("foodTrucks"));
    foodTruckWidget.updateSchedule();
}


DB.foodTruckWidget = function(root) {
    this._root = root;
};

DB.foodTruckWidget.prototype.updateSchedule = function()
{
    $.ajax({type: "GET",
        url: "/foodtrucks", 
        async: true,
        crossDomain: true,

        success: function(ret){
            try {
                var inner = "<h1 class='foodtrucks-title'>";
                inner += ret.dayOfWeek;
                inner += "'s Food Trucks</h1>";
                inner += "<ul>";
                ret.foodTrucks.forEach(function(foodTruck) {
                    inner += "<li class=\"foodtruck " + foodTruck.type.toLowerCase().replace(' ', '-') + "\"><h2>";
                    inner += foodTruck.name;
                    inner += "</h2><p class='foodtruck-content'>";
                    inner += foodTruck.type;
                    inner += "</p></li>";
                });
                inner += "</ul>";
                this._root.innerHTML = inner;
            } catch (e) {
                console.log("ERRRR: " + e);
            }

            }.bind(this),
            error: function(e) {
                console.log('ERROR: ' + e);
            }
    });

    setTimeout(this.updateSchedule.bind(this), 5 * 60 * 1000);
};


DB.clock = function(root) {
    this._root = root;
};

DB.clock.prototype.updateTime = function() {
    var today = new Date();
    var h = today.getHours();
    var m = today.getMinutes();
    var s = today.getSeconds();
    m = this._checkTime(m);
    s = this._checkTime(s);
    this._root.innerHTML = h + ":" + m + ":" + s;
    setTimeout(this.updateTime.bind(this), 500);
};

DB.clock.prototype._checkTime = function(i) {
    if (i < 10) {i = "0" + i};  // add zero in front of numbers < 10
    return i;
};