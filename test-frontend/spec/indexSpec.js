describe("Index", function() {


  beforeEach(function() {
    this.root = document.createElement("div");
  });

  describe("updateTime", function() {
    beforeEach(function() {
      var baseTime = new Date(2013, 9, 23, 3, 2, 1);
      jasmine.clock().install().mockDate(baseTime);
      this.subject = new DB.clock();
    });

    afterEach(function() {
      jasmine.clock().uninstall();
    });

    it("populates clock div every half second", function() {
      this.subject.updateTime();
      expect(this.subject.el.innerHTML).toEqual("3:02");

      jasmine.clock().tick(1000 * 60);
      expect(this.subject.el.innerHTML).toEqual("3:03");
    });

    it("displays 12h time", function() {
      var baseTime = new Date(2013, 9, 23, 13, 2, 1);
      jasmine.clock().mockDate(baseTime);
      this.subject.updateTime();
      expect(this.subject.el.innerHTML).toEqual("1:02");
    });
  });

  describe("countdownTimeToStandup", function() {

    beforeEach(function() {
      var baseTime = new Date(2013, 9, 23, 9, 1, 0);
      jasmine.clock().install().mockDate(baseTime);
      standardAudio = {
        play: jasmine.createSpy('play'),
        currentTime: 0
      };
      specialAudio = {
        play: jasmine.createSpy('play'),
        currentTime: 0
      };
      this.subject = new DB.standUpCountdown(standardAudio, specialAudio);
    });

    afterEach(function() {
      jasmine.clock().uninstall();
    });

    it("countdown updates correctly", function() {
      this.subject.update();
      expect(this.subject.el.innerHTML).toContain("5:00");

      jasmine.clock().tick(1000);
      this.subject.update();
      expect(this.subject.el.innerHTML).toContain("4:59");
    });

    it("countdown clock is formatted following m:ss", function() {
      var baseTime = new Date(2013, 9, 23, 9, 5, 55);
      jasmine.clock().mockDate(baseTime);
      this.subject.update();
      expect(this.subject.el.innerHTML).toContain("0:05");
    });

    it("countdown clock plays a sound when the coundown is up", function() {
      var baseTime = new Date(2013, 9, 23, 9, 5, 59);
      jasmine.clock().mockDate(baseTime);
      this.subject.update();
      expect(this.subject.standardAudio.play).not.toHaveBeenCalled();

      jasmine.clock().tick(1000);
      this.subject.update();
      expect(this.subject.standardAudio.play).toHaveBeenCalled();
    });

    it("countdown clock plays THE FINAL COUNTDOWN when there are 15 seconds to go on a Friday", function() {
      var baseTime = new Date(2017, 2, 3, 9, 5, 44);
      jasmine.clock().mockDate(baseTime);
      this.subject.update();
      expect(this.subject.specialAudio.play).not.toHaveBeenCalled();

      jasmine.clock().tick(1000);
      this.subject.update();
      expect(this.subject.specialAudio.play).toHaveBeenCalled();
    });

    it("doesn't play THE FINAL COUNTDOWN on non Fridays", function() {
      var baseTime = new Date(2017, 2, 2, 9, 5, 45);
      jasmine.clock().mockDate(baseTime);
      this.subject.update();
      expect(this.subject.specialAudio.play).not.toHaveBeenCalled();
    });

    it("doesn't play the normal sound on Fridays", function() {
      var baseTime = new Date(2017, 2, 3, 9, 6, 0);
      jasmine.clock().mockDate(baseTime);
      this.subject.update();
      expect(this.subject.standardAudio.play).not.toHaveBeenCalled();
    });

    it("the countdown alert sound only plays once even if update is called twice when there is zero seconds left", function() {
      var baseTime = new Date(2013, 9, 23, 9, 6, 0);
      jasmine.clock().mockDate(baseTime);
      this.subject.update();
      expect(this.subject.standardAudio.play).toHaveBeenCalled();

      this.subject.standardAudio.play.calls.reset();
      jasmine.clock().tick(1);
      this.subject.standardAudio.currentTime = 1;
      this.subject.update();
      expect(this.subject.standardAudio.play).not.toHaveBeenCalled();
    });

  });

  describe("updatesSchedule", function() {

    beforeEach(function() {
      jasmine.clock().install().mockDate();
      jasmine.Ajax.install();
      this.subject = new DB.foodTruckWidget();
    });

    afterEach(function() {
      jasmine.Ajax.uninstall();
      jasmine.clock().uninstall();
    });

    it("populate the food trucks widget", function() {

      jasmine.Ajax.stubRequest('/foodtrucks').andReturn({
        "responseText": "{\"foodTrucks\":[{\"name\":\"Bread Circuses\",\"type\":\"Burgers / Hot Dogs\"}], \"date\":\"Monday, 16 January 2017\", \"dayOfWeek\": \"Monday\"}"
      });

      this.subject.update();

      expect(this.subject.el.innerHTML).toContain('Bread Circuses');
      expect(this.subject.el.innerHTML).toContain('Burgers / Hot Dogs');
      expect(this.subject.el.innerHTML).toContain('Monday');
      var foodTruckClasses = this.subject.el.getElementsByClassName('foodtruck')[0].className;
      expect(foodTruckClasses).toEqual('foodtruck burgers hot-dogs');
      expect(jasmine.Ajax.requests.mostRecent().url).toBe('/foodtrucks');
    });
  });

  describe("updatesBusSchedule", function() {
    beforeEach(function() {
      jasmine.clock().install().mockDate();
      jasmine.Ajax.install();
      this.subject = new DB.busScheduleWidget();
    });

    afterEach(function() {
      jasmine.Ajax.uninstall();
      jasmine.clock().uninstall();
    });

    it("parses data for the 99 bus", function() {
      jasmine.Ajax.stubRequest('/buses').andReturn({
        "responseText": "{\"buses\":[{\"shortName\":\"99\",\"longName\":\"Belltown Via 1st Ave\", \"eta\":-2, \"status\": \"scheduled\"}," +
        "{\"shortName\":\"99\",\"longName\":\"Belltown Via 1st Ave\", \"eta\":\"5\", \"status\": \"late\"}]}"
      });

      this.subject.update();

      expect(this.subject.el.innerHTML).toContain("99");
      expect(this.subject.el.innerHTML).toContain("-2");
      expect(this.subject.el.innerHTML).toContain("5");
    });
  });

  describe("widgetSwapper", function() {
    var element1, element2, widget1, widget2

    beforeEach(function() {
      element1 = document.createElement("span");
      element2 = document.createElement("p");
      widget1 = {
        update: jasmine.createSpy('widget1.update'),
        el: element1,
        updateInterval: 5000
      };
      widget2 = {
        update: jasmine.createSpy('widget2.update'),
        el: element2,
        updateInterval: 5000
      };

      jasmine.clock().install().mockDate();
      this.subject = new DB.widgetSwapper();
    });

    afterEach(function() {
      jasmine.clock().uninstall();
    });

    it("without any widgets the widget swapper is blank", function() {
      expect(this.subject.el.tagName).toEqual("DIV");
      expect(this.subject.el.innerHTML).toEqual("");
    });

    it("with only one widget the widget swapper displays that widget", function() {
      this.subject.addWidget(widget1);
      this.subject.update();
      expect(this.subject.el.firstChild).toBe(element1);
    });

    it("swaps widgets based on start time", function() {
      this.subject.addWidget(widget1, 10, 5);
      this.subject.addWidget(widget2, 14, 30);

      var baseTime = new Date(2013, 9, 23, 10, 5, 0);
      jasmine.clock().mockDate(baseTime);
      this.subject.update();
      expect(this.subject.el.firstChild.tagName).toEqual("SPAN");

      jasmine.clock().tick(5 * 60 * 60 * 1000);
      this.subject.update();
      expect(this.subject.el.firstChild.tagName).toEqual("P");
    })

    it("swaps based on the minutes when the hours are equal", function() {
      this.subject.addWidget(widget1, 10, 5);
      this.subject.addWidget(widget2, 10, 30);

      var baseTime = new Date(2013, 9, 23, 10, 5, 0);
      jasmine.clock().mockDate(baseTime);
      this.subject.update();
      expect(this.subject.el.firstChild.tagName).toEqual("SPAN");

      jasmine.clock().tick(25 * 60 * 1000);
      this.subject.update();
      expect(this.subject.el.firstChild.tagName).toEqual("P");
    })

    it("swaps based on the seconds when the minutes and hours are equal", function() {
      this.subject.addWidget(widget1, 10, 30);
      this.subject.addWidget(widget2, 10, 30, 1);

      var baseTime = new Date(2013, 9, 23, 10, 30, 0);
      jasmine.clock().mockDate(baseTime);
      this.subject.update();
      expect(this.subject.el.firstChild.tagName).toEqual("SPAN");

      jasmine.clock().tick(1000);
      this.subject.update();
      expect(this.subject.el.firstChild.tagName).toEqual("P");
    })

    it("swaps widgets based on start time regardless of the order they were added in", function() {
      this.subject.addWidget(widget1, 14, 30);
      this.subject.addWidget(widget2, 10, 5);

      var baseTime = new Date(2013, 9, 23, 10, 5, 0);
      jasmine.clock().mockDate(baseTime);
      this.subject.update();
      expect(this.subject.el.firstChild.tagName).toEqual("P");

      jasmine.clock().tick(5 * 60 * 60 * 1000);
      this.subject.update();
      expect(this.subject.el.firstChild.tagName).toEqual("SPAN");
    })

    it("uses the most recently added widget if the start time is the same as another", function() {
      this.subject.addWidget(widget1, 14, 30);
      this.subject.addWidget(widget2, 14, 30);

      var baseTime = new Date(2013, 9, 23, 14, 30, 0);
      jasmine.clock().mockDate(baseTime);
      this.subject.update();
      expect(this.subject.el.firstChild.tagName).toEqual("P");
    })

    it("uses the lastest scheduled widget if the time is before any widgets start time", function() {
      this.subject.addWidget(widget1, 10, 5);
      this.subject.addWidget(widget2, 14, 30);

      var baseTime = new Date(2013, 9, 23, 9, 30, 0);
      jasmine.clock().mockDate(baseTime);
      this.subject.update();
      expect(this.subject.el.firstChild.tagName).toEqual("P");
    })

    it("triggers updates on the currently active widget at the specified time interval", function() {
      var baseTime = new Date(2013, 9, 23, 9, 8, 0);
      jasmine.clock().mockDate(baseTime);

      this.subject.addWidget(widget1, 0, 0);
      this.subject.update();
      expect(widget1.update).toHaveBeenCalled();
      widget1.update.calls.reset();

      jasmine.clock().tick(widget1.updateInterval);
      expect(widget1.update).toHaveBeenCalled();
    })

    it("stops updating a widget after it is no longer active", function(){
      var baseTime = new Date(2013, 9, 23, 0, 0, 0);
      jasmine.clock().mockDate(baseTime);

      this.subject.addWidget(widget1, 0, 0);
      this.subject.addWidget(widget2, 2, 0);
      this.subject.update();
      expect(widget1.update).toHaveBeenCalled();

      jasmine.clock().tick(2 * 60 * 60 * 1000);
      this.subject.update();
      widget1.update.calls.reset();

      jasmine.clock().tick(widget1.updateInterval);
      expect(widget1.update).not.toHaveBeenCalled();
    })

    it("throws an error if you try to add a widget without an el property of type object", function() {
      expect(
        function() {
          var badWidget = {
            update: jasmine.createSpy('widget.update'),
            updateInterval: 5000
          };
          this.subject.addWidget(badWidget);
        }.bind(this)).toThrowError("Widget must have an 'el' property of type object");
    })

    it("throws an error if you try to add a widget without an updateInterval property of type number", function() {
      expect(
        function() {
          var badWidget = {
            el: element1,
            update: jasmine.createSpy('widget.update')
          };
          this.subject.addWidget(badWidget);
        }.bind(this)).toThrowError("Widget must have an 'updateInterval' property of type number");
    })

    it("throws an error if you try to add a widget without an update function", function() {
      expect(
        function() {
          var badWidget = {
            el: element1,
            updateInterval: 5000
          };
          this.subject.addWidget(badWidget);
        }.bind(this)).toThrowError("Widget must have an 'update' function");
    })
  });

});
