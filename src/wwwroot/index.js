"use strict";

var DB = {};
var mainContent = {};

function boot() {
    var widgetSwapper = new DB.widgetSwapper();
    var standardAudio = new Audio('/audio/bell.mp3');
    var specialAudio = new Audio('/audio/final_countdown.wav');

    widgetSwapper.addWidget(new DB.standUpCountdown(standardAudio, specialAudio), 9, 1);
    widgetSwapper.addWidget(new DB.regularContent(), 9, 6, 1);
    widgetSwapper.update();

    setInterval( function() {widgetSwapper.update()}, 500);

    document.querySelector("#root").appendChild(widgetSwapper.el);
}

DB.standUpCountdown = function(standardAudio, specialAudio) {
    this.el = document.createElement("div");
    this.el.id = "countdown";
    this.updateInterval = 500;
    this.standardAudio = standardAudio;
    this.specialAudio = specialAudio;
};

DB.standUpCountdown.prototype.update = function() {
    var now = new Date();
    var secondsRemaining = 60 - now.getSeconds();
    var minutesRemaining = 5 - now.getMinutes();
    if (secondsRemaining == 60) {
        minutesRemaining = 6 - now.getMinutes();
        secondsRemaining = 0;
    }
    secondsRemaining = new DB.clock().formatTime(secondsRemaining);

    this.el.innerHTML = "<div>"+ minutesRemaining + ":" + secondsRemaining + "</div>";

    if (minutesRemaining != 0) {
        return;
    }
    if (new DB.clock().isFriday()) {
        var isPlaying = !this.specialAudio.paused && !this.specialAudio.ended && 0 < this.specialAudio.currentTime;
        if (secondsRemaining <= 15 && !isPlaying) {
            this.specialAudio.play();
        }
    }
    else {
        var isPlaying = !this.standardAudio.paused && !this.standardAudio.ended && 0 < this.standardAudio.currentTime;
        if (secondsRemaining == 0 && !isPlaying) {
            this.standardAudio.play();
        }
    }
}

DB.regularContent = function() {
    this.el = document.createElement("div");

    var inner = "<div class='flex-container'><div id='dynamic-content'></div><div id='static-content'></div></div>";

    this.el.innerHTML = inner;
    this._clock = new DB.clock();
    this.el.querySelector("#static-content").appendChild(this._clock.el);

    this._coffee = new DB.coffeeWidget();
    this.el.querySelector("#static-content").appendChild(this._coffee.el);
    //widgetSwapper.addWidget(new DB.coffeeWidget(), 0, 0);

    this._widgetSwapper = new DB.widgetSwapper();
    this._widgetSwapper.addWidget(new DB.foodTruckWidget(), 0, 0);
    this._widgetSwapper.addWidget(new DB.busScheduleWidget(), 14, 0);
    this.el.querySelector("#dynamic-content").appendChild(this._widgetSwapper.el);

    this.updateInterval = 30 * 1000;
};

DB.regularContent.prototype.update = function(){
    this._clock.updateTime();
    this._coffee.update();
    this._widgetSwapper.update();
};

function cssClassesForFoodtruckType(type) {
    return "foodtruck " + type.toLowerCase().replace(new RegExp(' ', 'g'), '-').replace(new RegExp('-?/-?', 'g'), ' ');
};

function cssClassesForCoffeeLevel(status, level) {
    var cssClass = "coffee-" + status.toLowerCase();
    if (status.toLowerCase() == "available")
    {
        if (level <= 33) {
            cssClass = "coffee-low";
        } else if (level <= 66) {
            cssClass = "coffee-medium";
        } else {
            cssClass = "coffee-high";
        }
    }

    return cssClass;
};

DB.widgetSwapper = function(){
    this.el = document.createElement("div");
    this.widgets = [];
    this.widgetUpdater;
}

DB.widgetSwapper.prototype.addWidget = function(widget, hours, minutes, seconds) {
    if (typeof seconds === 'undefined') {
        seconds = 0;
    }
    if (typeof widget.el !== 'object'){
        throw new Error("Widget must have an 'el' property of type object");
    }
    if (typeof widget.updateInterval !== 'number'){
        throw new Error("Widget must have an 'updateInterval' property of type number");
    }
    if (typeof widget.update !== 'function'){
        throw new Error("Widget must have an 'update' function");
    }
    this.widgets.push({widget: widget, startTime: (hours * 60 * 60) + (minutes * 60) + seconds});
    this.widgets.sort(function(a, b) {
        return a.startTime - b.startTime;
    });
};

DB.widgetSwapper.prototype.update = function() {
    var currentHour = new Date().getHours();
    var currentMinute = new Date().getMinutes();
    var currentSeconds = new Date().getSeconds();
    var currentTimeInSeconds = (currentHour * 60 * 60) + (currentMinute * 60) + currentSeconds;
    var newWidget = this.widgets[this.widgets.length - 1].widget;
    for(var i=0; i < this.widgets.length; i++) {
        var widget = this.widgets[i];
        if (widget.startTime <= currentTimeInSeconds) {
            newWidget = widget.widget;
        }
    }

    if (this.previousWidget == newWidget)
    {
        return;
    }
    this.previousWidget = newWidget;

    if (this.el.firstChild)
    {
        this._stopUpdating();
        this.el.removeChild(this.el.firstChild);
    }

    this._startUpdating(newWidget);
    this.el.appendChild(newWidget.el);
};

DB.widgetSwapper.prototype._startUpdating = function(widget) {
    widget.update();
    this.widgetUpdater = setInterval( function() {widget.update()}, widget.updateInterval);
}

DB.widgetSwapper.prototype._stopUpdating = function() {
    clearInterval(this.widgetUpdater);
}

DB.coffeeWidget = function() {
    this.el = document.createElement("div");
    this.el.id = "coffee";
    this.updateInterval = 30 * 1000;
};

DB.coffeeWidget.prototype.update = function() {
    $.ajax({type: "GET",
        url: "/coffee",
        async: true,
        crossDomain: true,

        success: function(ret)
        {
            try {
                var inner = "<h1 class='coffee-title'>Coffee Pots</h1>";
                inner += "<ul>";
                ret.coffees.forEach(function(coffee) {
                    inner += "<li>";
                    inner += "<span class='" + cssClassesForCoffeeLevel(coffee.status, coffee.level) + "'>";
                    inner += " <strong>"+ coffee.devId + "</strong>";
                    if (coffee.level) {
                        inner += "(" + coffee.level + "%)<br />";
                    }
                    for (var i = 0; i < Math.round(coffee.cups); i++) {
                        inner += "<i class='fa fa-coffee' aria-hidden='true'></i>";
                    }
                    inner += "</span></li>";
                });
                inner += "</ul>";
                this.el.innerHTML = inner;
            } catch (e) {
                console.log("ERRRR: " + e);
            }
        }.bind(this),
        error: function(e) {
        console.log('ERROR: ' + e);
        }
    });
};

DB.busScheduleWidget = function() {
    this.el = document.createElement("div");
    this.updateInterval = 30 * 1000;
};

DB.busScheduleWidget.prototype.update = function() {
    $.ajax({type: "GET",
        url: "/buses",
        async: true,
        crossDomain: true,

        success: function(ret){
            try {
                var inner = "<div><h1 class='bus-stop-title'>International District Station</h1>";
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
                this.el.innerHTML = inner;
            } catch (e) {
                console.log("ERRRR: " + e);
            }

        }.bind(this),
        error: function(e) {
          console.log('ERROR: ' + e);
        }
    });
};

DB.foodTruckWidget = function() {
    this.el = document.createElement("div");
    this.el.class = "foodtruck";
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
                this.el.innerHTML = inner;
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
    this.el.id = "clock";
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

DB.clock.prototype.isFriday = function() {
    var today = new Date();
    return today.getDay() == 5;
};
