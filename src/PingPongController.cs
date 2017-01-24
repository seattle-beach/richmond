using Microsoft.AspNetCore.Mvc;

namespace Richmond
{
    public class PingPongController
    {
        private readonly IPingPongRepository _sessionRepository;

        public PingPongController(IPingPongRepository sessionRepository)
        {
            _sessionRepository = sessionRepository;
        }

        [HttpGet("pingPongLadder")]
        public IActionResult PingPongLadder() {
            var response = _sessionRepository.ParsePingPongWebsite(_sessionRepository.RequestPingPongWebsite());
            return new OkObjectResult(response);
        }
    }

}