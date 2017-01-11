describe("Index", function() {
  var clock;

  beforeEach(function() {
    this.root = document.createElement("div");
    this.subject = new DB.clock(this.root);
  });
  
  describe("startTime", function() {
    it("populates clock div periodically", function() {
      this.subject.setTime();
      var time = this.root.innerHTML;
      expect(time).toEqual(jasmine.stringMatching(/^[0-9][0-9]:[0-9][0-9]:[0-9][0-9]$/));

      setTimeout(function(){
        expect(time).not.toEqual(this.root.innerHTML);
      }, 1000);
      

    });
  });
});