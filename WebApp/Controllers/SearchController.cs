using Infrastructure.Contexts;
using Infrastructure.Entities;
using Infrastructure.Models;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;


namespace WebApp.Controllers
{
    [Authorize]
    public class SearchController : Controller
    {
        private readonly DataContext _context;
        private readonly UserManager<UserEntity> _userManager;
        private readonly OpenRouteService _openRouteService;
        private readonly SearchService _searchService;

        public SearchController(DataContext context, UserManager<UserEntity> userManager, OpenRouteService openRouteService, SearchService searchService)
        {
            _context = context;
            _userManager = userManager;
            _openRouteService = openRouteService;
            _searchService = searchService;
        }

        // ✅ GET: Show all ride requests
        [HttpGet]
        [Route("/search")]
        public async Task<IActionResult> Index(string? statusMessage)
        {
            var viewModel = new SearchViewModel
            {
                SearchModel = new RideSearchModel()
            };

            if (statusMessage != null)
                ViewData["StatusMessage"] = statusMessage;

            var result = await _searchService.GetAllRequestsAsync();
            if (result.StatusCode == Infrastructure.Models.StatusCode.Ok)
            {
                var requests = (List<SearchRequestEntity>)result.ContentResult!;
                var futureRequests = requests
                    .Where(r => r.DepartureTime >= DateTime.Now)
                    .OrderBy(r => r.DepartureTime)
                    .ToList();

                foreach (var req in futureRequests)
                {
                    var userEntity = await _userManager.FindByIdAsync(req.UserId);
                    var drivingInfo = await _openRouteService.GetDrivingInfoAsync(req.Origin, req.Destination);

                    viewModel.Requests.Add(new SearchRequestModel
                    {
                        Id = req.Id,
                        Origin = req.Origin,
                        Destination = req.Destination,
                        DepartureTime = req.DepartureTime,
                        UserName = userEntity!.FirstName + " " + userEntity.LastName,
                        UserImgUrl = userEntity.ProfileImgUrl,
                        SeatsRequired = req.SeatsRequired,
                        Notes = req.Notes,
                        DistanceKm = drivingInfo?.DistanceKm ?? 0,
                        Duration = drivingInfo?.Duration ?? TimeSpan.Zero,
                        EstimatedArrival = req.DepartureTime + (drivingInfo?.Duration ?? TimeSpan.Zero),

                        // 🔹 Map entity messages into DTOs
                        Messages = req.Messages.Select(m => new SearchMessageModel
                        {
                            Sender = _context.Users
                            .Where(u => u.Email == m.SenderId)
                            .Select(u => u.FirstName + " " + u.LastName)
                            .FirstOrDefault() ?? m.SenderId,
                            Text = m.MessageContent,
                            Timestamp = m.Timestamp,
                            IsRead = m.IsRead
                        }).OrderByDescending(m => m.Timestamp).ToList()
                    });

                }
            }

            return View(viewModel);
        }

        // ✅ POST: Add new request
        [HttpPost]
        [Route("/search")]
     
        public async Task<IActionResult> Create(SearchViewModel viewModel)
        {
            var userEntity = await _userManager.GetUserAsync(User);

            ModelState.Remove("SearchModel.Origin");
            ModelState.Remove("SearchModel.Destination");
            ModelState.Remove("SearchModel.DepartureTime");

            if (ModelState.IsValid && viewModel.NewRequest != null)
            {
                var result = await _searchService.AddRequestAsync(viewModel.NewRequest, userEntity.Email!);
                if (result.StatusCode == Infrastructure.Models.StatusCode.Ok)
                {
                    return RedirectToAction("Index", new { statusMessage = "success|Din förfrågan har postats!" });
                }
            }

            ViewData["StatusMessage"] = "danger|Felaktiga uppgifter.";
            return View("Index", viewModel);
        }

        // ✅ POST: Search requests
        [HttpPost]
        [Route("/search/find")]
        public async Task<IActionResult> Search(SearchViewModel viewModel)
        {
            if (viewModel.SearchModel != null)
            {
                var result = await _searchService.SearchRequestsAsync(viewModel.SearchModel);
                if (result.StatusCode == Infrastructure.Models.StatusCode.Ok)
                {
                    var requests = (List<SearchRequestEntity>)result.ContentResult!;
                    viewModel.Requests = requests
                        .Where(r => r.DepartureTime >= DateTime.Now)
                        .Select(r => new SearchRequestModel
                        {
                            Id = r.Id,
                            Origin = r.Origin,
                            Destination = r.Destination,
                            DepartureTime = r.DepartureTime,
                            SeatsRequired = r.SeatsRequired,
                            Notes = r.Notes
                        }).ToList();

                    return View("Index", viewModel);
                }

                ViewData["StatusMessage"] = "warning|No matching requests found.";
                return View("Index", viewModel);
            }

            ViewData["StatusMessage"] = "danger|Invalid search.";
            return View("Index", viewModel);
        }
    }
}
