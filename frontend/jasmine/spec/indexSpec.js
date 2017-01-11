describe("Index", function() {
  var clock, time;

  beforeEach(function() {
    this.root = document.createElement("div");
    this.subject = new DB.clock(this.root);
  });
  
  describe("startTime", function() {
    it("populates clock div periodically", function(done) {
      this.subject.setTime();
      time = "" + this.root.innerHTML;
      expect(time).toEqual(jasmine.stringMatching(/^[0-9][0-9]:[0-9][0-9]:[0-9][0-9]$/));

      setTimeout(function(){
        expect(time).not.toEqual("" + this.root.innerHTML);
        done();
      }.bind(this), 1500);
    });
  });
});