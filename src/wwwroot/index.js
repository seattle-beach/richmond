"use strict";

var DB = {};
var timeSensitiveWidget = {};
var mainContent = {};

function boot() {
    var root = document.getElementById("root");

    mainContent = new DB.mainContentDecider(new DB.standUpCountdown(root), new DB.regularContent(root));
    mainContent.update();
}

DB.standUpCountdown = function(root) {
  this._root = root;
};

DB.standUpCountdown.prototype.update = function() {
    var seconds = new Date().getSeconds();
    var minutes = new Date().getMinutes();
    var secondsRemaining = 60 - seconds;
    var minutesRemaining = 5 - minutes;
    if (secondsRemaining == 60) {
        minutesRemaining = 6 - minutes;
        secondsRemaining = 0;
    }
    secondsRemaining = new DB.clock().formatTime(secondsRemaining);

    this._root.innerHTML = "<div id='countdown'><div>"+ minutesRemaining + ":" + secondsRemaining + "</div></div>";
}

DB.regularContent = function(root) {
  this._root = root;
};

DB.regularContent.prototype.update = function(){
    var inner = "<div class='flex-container'><div id='dynamic-content'></div></div>";
    this._root.innerHTML = inner;

    var clock = new DB.clock();
    clock.updateTime();
    this._root.querySelector(".flex-container").appendChild(clock.el);

    var dynamicContent = document.getElementById("dynamic-content");
    timeSensitiveWidget = new DB.timeSensitiveWidget(new DB.foodTruckWidget(dynamicContent), new DB.busScheduleWidget(dynamicContent));
    timeSensitiveWidget.update();
};

function cssClassesForFoodtruckType(type) {
    return "foodtruck " + type.toLowerCase().replace(new RegExp(' ', 'g'), '-').replace(new RegExp('-?/-?', 'g'), ' ');
};

DB.mainContentDecider = function(standUpCountdown, regularContent) {
    this._standUpCountdown = standUpCountdown;
    this._regularContent = regularContent;
    this.widget = this._regularContent;
    this.widget.update();
    this._isCountdown = false;
};

DB.mainContentDecider.prototype.update = function() {
    var hour = new Date().getHours();
    var minutes = new Date().getMinutes();
    var isCountdown = false;
    var widget = this._regularContent;
    if (hour == 9 && (minutes < 6 && minutes > 0)) {
        isCountdown = true;
        widget = this._standUpCountdown;
    }

    if (isCountdown != this._isCountdown) {
        this.widget = widget;
        this._isCountdown = isCountdown;
        this.widget.update();
    }
    if (this._isCountdown) {
        this.widget.update();
    }

    setTimeout(this.update.bind(this), 500);
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

DB.widgetSwapper = function(){
    this.el = document.createElement("div");
    this.widgets = [];
}

DB.widgetSwapper.prototype.addWidget = function(widget, hours, minutes) {
    this.widgets.push({widget: widget, startTime: hours * 60 + minutes});
    this.widgets.sort(function(a, b) {
        return a.startTime - b.startTime;
    });
};

DB.widgetSwapper.prototype.update = function() {
    var currentHour = new Date().getHours();
    var currentMinute = new Date().getMinutes();
    var currentTimeInMinutes = (currentHour * 60) + currentMinute;
    var newWidget = this.widgets[this.widgets.length - 1].widget;
    for(var i=0; i < this.widgets.length; i++) {
        var widget = this.widgets[i];
        if (widget.startTime <= currentTimeInMinutes) {
            newWidget = widget.widget;
        }
    }

    if (this.el.firstChild){
        this.el.removeChild(this.el.firstChild);
    }

    this.el.appendChild(newWidget.el);
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

DB.clock = function() {
    this.el = document.createElement("div");
    this.el.id = "clock"
};

DB.clock.prototype.updateTime = function() {
    var today = new Date();
    var h = this._getHours(today);
    var m = today.getMinutes();
    m = this.formatTime(m);
    this.el.innerHTML = h + ":" + m;
    setTimeout(this.updateTime.bind(this), 1000);
};

DB.clock.prototype._getHours = function(today) {
    var h = today.getHours();
    if (h > 12) {h = h%12};
    return h;
}

DB.clock.prototype.formatTime = function(i) {
    if (i < 10) {i = "0" + i};  // add zero in front of numbers < 10
    return i;
};
