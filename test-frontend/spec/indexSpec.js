describe("Index", function() {


  beforeEach(function() {
    this.root = document.createElement("div");
  });

  describe("updateTime", function() {
    beforeEach(function() {
      var baseTime = new Date(2013, 9, 23, 3, 2, 1);
      jasmine.clock().install().mockDate(baseTime);
      this.subject = new DB.clock(this.root);
    });

    afterEach(function() {
      jasmine.clock().uninstall();
    });

    it("populates clock div every half second", function() {
      this.subject.updateTime();
      expect(this.root.innerHTML).toEqual("3:02");

      jasmine.clock().tick(1000 * 60);
      expect(this.root.innerHTML).toEqual("3:03");
    });

    it("displays 12h time", function() {
      var baseTime = new Date(2013, 9, 23, 13, 2, 1);
      jasmine.clock().mockDate(baseTime);
      this.subject.updateTime();
      expect(this.root.innerHTML).toEqual("1:02");
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

      jasmine.Ajax.requests.reset();
      expect(jasmine.Ajax.requests.count()).toEqual(0);
      this.subject.update();
      expect(jasmine.Ajax.requests.count()).toEqual(1);
      expect(jasmine.Ajax.requests.mostRecent().url).toBe('/foodtrucks');
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

  describe("updatesBusWidget", function() {
    beforeEach(function() {
      jasmine.clock().install().mockDate();
      jasmine.Ajax.install();
      this.subject = new DB.busScheduleWidget(this.root);
    });

    afterEach(function() {
      jasmine.Ajax.uninstall();
      jasmine.clock().uninstall();
    });
  });
});
