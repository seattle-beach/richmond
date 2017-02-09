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
      this.subject = new DB.standUpCountdown(this.root);
    });

    afterEach(function() {
      jasmine.clock().uninstall();
    });

    it("countdown updates correctly", function() {
      this.subject.update();
      expect(this.root.innerHTML).toContain("5:00");

      jasmine.clock().tick(1000);
      this.subject.update();
      expect(this.root.innerHTML).toContain("4:59");
    });

    it("countdown clock is formatted following m:ss", function() {
      var baseTime = new Date(2013, 9, 23, 9, 5, 55);
      jasmine.clock().mockDate(baseTime);
      this.subject.update();
      expect(this.root.innerHTML).toContain("0:05");
    });

  });

  describe("updatesSchedule", function() {

    beforeEach(function() {
      jasmine.clock().install().mockDate();
      jasmine.Ajax.install();
      this.subject = new DB.foodTruckWidget(this.root);
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

      expect(this.root.innerHTML).toContain('Bread Circuses');
      expect(this.root.innerHTML).toContain('Burgers / Hot Dogs');
      expect(this.root.innerHTML).toContain('Monday');
      var foodTruckClasses = this.root.getElementsByClassName('foodtruck')[0].className;
      expect(foodTruckClasses).toEqual('foodtruck burgers hot-dogs');
      expect(jasmine.Ajax.requests.mostRecent().url).toBe('/foodtrucks');
    });
  });

  describe("mainContentChanges", function() {
    beforeEach(function() {
      jasmine.clock().install()

      this.standUpCountdown = jasmine.createSpyObj("standUpCountdown", ["update"]);
      this.regularContent = jasmine.createSpyObj("regularContent", ["update"]);
      this.subject = new DB.mainContentDecider(this.standUpCountdown, this.regularContent);
    });

    afterEach(function() {
      jasmine.clock().uninstall();
    });

    it("swaps the standup content in for the regular content at 9:01 am", function() {
      var baseTime = new Date(2013, 9, 23, 9, 0, 59);
      jasmine.clock().mockDate(baseTime);

      this.subject.update();
      expect(this.subject.widget).toEqual(this.regularContent);
      jasmine.clock().tick(1000);
      expect(this.subject.widget).toEqual(this.standUpCountdown);
    });

    it("swaps the regular content in for the standup content at 9:06 am", function() {
      var baseTime = new Date(2013, 9, 23, 9, 5, 59);
      jasmine.clock().mockDate(baseTime);

      this.subject.update();
      expect(this.subject.widget).toEqual(this.standUpCountdown);
      jasmine.clock().tick(1000);
      expect(this.subject.widget).toEqual(this.regularContent);
    });

    it("the mainContentDecider triggers the countdown clock to update during the countdown", function() {
      var baseTime = new Date(2013, 9, 23, 9, 1, 0);
      jasmine.clock().mockDate(baseTime);
      this.subject.update();
      expect(this.standUpCountdown.update).toHaveBeenCalled();
      this.standUpCountdown.update.calls.reset();

      jasmine.clock().tick(1000);
      expect(this.standUpCountdown.update).toHaveBeenCalled();
    });

    it("the mainContentDecider does not triggers the countdown clock to update once the countdown is over", function() {
      var baseTime = new Date(2013, 9, 23, 9, 6, 0);
      jasmine.clock().mockDate(baseTime);
      this.subject.update();
      expect(this.standUpCountdown.update).not.toHaveBeenCalled();
    });

    it("after swapping in the regular content for the standup countdown, " +
      "the mainContentDecider does not trigger updates of the regular content", function() {
      var baseTime = new Date(2013, 9, 23, 9, 8, 0);
      jasmine.clock().mockDate(baseTime);
      this.subject.update();
      expect(this.regularContent.update).toHaveBeenCalled();
      this.regularContent.update.calls.reset();

      jasmine.clock().tick(5000);
      expect(this.regularContent.update).not.toHaveBeenCalled();
    });
  });

  describe("dynamicWidgetChanges", function() {
    beforeEach(function() {
      jasmine.clock().install()

      this.earlyWidget = jasmine.createSpyObj("earlyWidget", ["update", "updateInterval"]);
      this.lateWidget = jasmine.createSpyObj("lateWidget", ["update", "updateInterval"]);
    });

    afterEach(function() {
      jasmine.clock().uninstall();
    });

    it("swaps the bus widget in for the food truck widget at 4pm", function() {
      var baseTime = new Date(2013, 9, 23, 15, 59, 0);
      jasmine.clock().mockDate(baseTime);

      var interval = 60 * 1000;
      this.earlyWidget.updateInterval = interval;
      this.subject = new DB.timeSensitiveWidget(this.earlyWidget, this.lateWidget);

      this.subject.update();
      expect(this.subject.widget).toEqual(this.earlyWidget);
      jasmine.clock().tick(interval);
      expect(this.subject.widget).toEqual(this.lateWidget);
    });

    it("updates only on the inner widget's schedule", function() {
      var baseTime = new Date(2013, 9, 23, 15, 0, 0);
      jasmine.clock().mockDate(baseTime);

      var interval = 5 * 60 * 1000;
      var truckMock = jasmine.createSpyObj("truckMock", ["update", "updateInterval"]);
      truckMock.updateInterval = interval;
      this.subject = new DB.timeSensitiveWidget(truckMock, this.lateWidget);

      this.subject.update();
      expect(truckMock.update.calls.count()).toEqual(1);
      jasmine.clock().tick(interval - 1);
      expect(truckMock.update.calls.count()).toEqual(1);
      jasmine.clock().tick(1);
      expect(truckMock.update.calls.count()).toEqual(2);
    });
  });

  describe("updatesBusSchedule", function() {
    beforeEach(function() {
      jasmine.clock().install().mockDate();
      jasmine.Ajax.install();
      this.subject = new DB.busScheduleWidget(this.root);
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

      expect(this.root.innerHTML).toContain("99");
      expect(this.root.innerHTML).toContain("-2");
      expect(this.root.innerHTML).toContain("5");
    });
  });

  describe("widgetSwapper", function() {
    var element1, element2

    beforeEach(function() {
      element1 = document.createElement("span");
      element2 = document.createElement("p");
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
      var element = document.createElement("span");
      this.subject.addWidget({el:element});
      this.subject.update();
      expect(this.subject.el.firstChild).toBe(element);

    });

    it("swaps widgets based on start time", function() {
      this.subject.addWidget({el:element1}, 10, 5);
      this.subject.addWidget({el:element2}, 14, 30);

      var baseTime = new Date(2013, 9, 23, 10, 5, 0);
      jasmine.clock().mockDate(baseTime);
      this.subject.update();
      expect(this.subject.el.firstChild.tagName).toEqual("SPAN");

      jasmine.clock().tick(5 * 60 * 60 * 1000);
      this.subject.update();
      expect(this.subject.el.firstChild.tagName).toEqual("P");
    })

    it("swaps based on the minutes when the hours are equal", function() {
      this.subject.addWidget({el:element1}, 10, 5);
      this.subject.addWidget({el:element2}, 10, 30);

      var baseTime = new Date(2013, 9, 23, 10, 5, 0);
      jasmine.clock().mockDate(baseTime);
      this.subject.update();
      expect(this.subject.el.firstChild.tagName).toEqual("SPAN");

      jasmine.clock().tick(25 * 60 * 1000);
      this.subject.update();
      expect(this.subject.el.firstChild.tagName).toEqual("P");
    })

    it("swaps widgets based on start time regardless of the order they were added in", function() {
      this.subject.addWidget({el:element1}, 14, 30);
      this.subject.addWidget({el:element2}, 10, 5);

      var baseTime = new Date(2013, 9, 23, 10, 5, 0);
      jasmine.clock().mockDate(baseTime);
      this.subject.update();
      expect(this.subject.el.firstChild.tagName).toEqual("P");

      jasmine.clock().tick(5 * 60 * 60 * 1000);
      this.subject.update();
      expect(this.subject.el.firstChild.tagName).toEqual("SPAN");
    })

    it("uses the most recently added widget if the start time is the same as another", function() {
      this.subject.addWidget({el:element1}, 14, 30);
      this.subject.addWidget({el:element2}, 14, 30);

      var baseTime = new Date(2013, 9, 23, 14, 30, 0);
      jasmine.clock().mockDate(baseTime);
      this.subject.update();
      expect(this.subject.el.firstChild.tagName).toEqual("P");
    })

    it("uses the lastest scheduled widget if the time is before any widgets start time", function() {
      this.subject.addWidget({el:element1}, 10, 5);
      this.subject.addWidget({el:element2}, 14, 30);

      var baseTime = new Date(2013, 9, 23, 9, 30, 0);
      jasmine.clock().mockDate(baseTime);
      this.subject.update();
      expect(this.subject.el.firstChild.tagName).toEqual("P");
    })
  });

});
