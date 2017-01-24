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

    it("populate the food trucks widget every five minutes", function() {

      jasmine.Ajax.stubRequest('/foodtrucks').andReturn({
        "responseText": "{\"foodTrucks\":[{\"name\":\"Bread Circuses\",\"type\":\"Burgers / Hot Dogs\"}], \"date\":\"Monday, 16 January 2017\", \"dayOfWeek\": \"Monday\"}"
      });

      this.subject.updateSchedule();

      expect(this.root.innerHTML).toContain('Bread Circuses');
      expect(this.root.innerHTML).toContain('Burgers / Hot Dogs');
      expect(this.root.innerHTML).toContain('Monday');
      var foodTruckClasses = this.root.getElementsByClassName('foodtruck')[0].className;
      expect(foodTruckClasses).toEqual('foodtruck burgers hot-dogs');
      expect(jasmine.Ajax.requests.mostRecent().url).toBe('/foodtrucks');

      jasmine.Ajax.requests.reset();
      expect(jasmine.Ajax.requests.count()).toEqual(0);
      jasmine.clock().tick(5* 60 * 1000 + 1);
      expect(jasmine.Ajax.requests.count()).toEqual(1);
      expect(jasmine.Ajax.requests.mostRecent().url).toBe('/foodtrucks');
    });
  });
});
