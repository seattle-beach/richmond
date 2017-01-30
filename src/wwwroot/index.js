"use strict";

var DB = {};
var timeSensitiveWidget = {};

function boot() {
    var clock = new DB.clock(document.getElementById("clock"));
    clock.updateTime();

    var root = document.getElementById("dynamic-content");
    timeSensitiveWidget = new DB.timeSensitiveWidget(new DB.foodTruckWidget(root), new DB.busScheduleWidget(root));
    timeSensitiveWidget.update();
};

function cssClassesForFoodtruckType(type) {
    return "foodtruck " + type.toLowerCase().replace(new RegExp(' ', 'g'), '-').replace(new RegExp('-?/-?', 'g'), ' ');
};

DB.timeSensitiveWidget = function(earlyWidget, lateWidget) {
    this._earlyWidget = earlyWidget;
    this._lateWidget = lateWidget;

    this.widget = earlyWidget;
};

DB.timeSensitiveWidget.prototype.update = function() {
    var hour = new Date().getHours();
    if (hour < 16) {
        this.widget = this._earlyWidget;
    } else if (hour ) {
        this.widget = this._lateWidget;
    }

    this.widget.update();

    setTimeout(this.update.bind(this), this.widget.updateInterval);
};

DB.busScheduleWidget = function(root) {
  this._root = root;
  this.updateInterval = 30 * 1000;
};

DB.busScheduleWidget.prototype.update = function() {
    $.ajax({type: "GET",
        url: "/buses",
        async: true,
        crossDomain: true,

        success: function(ret){
            try {
                var inner = "<div><h1 class='bus-stop-title'>S Jackson St & Occidental Ave</h1>";
                inner += "<h2 class='bus-description-title'>" + ret.buses[0].shortName + " - " + ret.buses[0].longName +"</h2>"
                inner += "<ul class='bus-list'>";
                ret.buses.forEach(function(bus) {
                    inner += "<li class=\"" + "\">";
                    inner += "<p class='bus-arrival " + bus.status + "'>";
                    inner += "arriving ";
                    if (bus.status != "scheduled")
                    {
                        inner += bus.status + " ";
                    }
                    inner += "in " + bus.eta + "m";
                    inner += "</p></li>";
                });
                inner += "</ul></div>";
                this._root.innerHTML = inner;
            } catch (e) {
                console.log("ERRRR: " + e);
            }

        }.bind(this),
        error: function(e) {
          console.log('ERROR: ' + e);
        }
    });
};

DB.foodTruckWidget = function(root) {
    this._root = root;
    this.updateInterval = 5 * 60 * 1000;
};

DB.foodTruckWidget.prototype.update = function() {
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
                    inner += "<li class=\"" + cssClassesForFoodtruckType(foodTruck.type) + "\"><h2>";
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
};

DB.clock = function(root) {
    this._root = root;
};

DB.clock.prototype.updateTime = function() {
    var today = new Date();
    var h = this._getHours(today);
    var m = today.getMinutes();
    m = this._checkTime(m);
    this._root.innerHTML = h + ":" + m;
    setTimeout(this.updateTime.bind(this), 1000);
};

DB.clock.prototype._getHours = function(today) {
    var h = today.getHours();
    if (h > 12) {h = h%12};
    return h;
}

DB.clock.prototype._checkTime = function(i) {
    if (i < 10) {i = "0" + i};  // add zero in front of numbers < 10
    return i;
};
