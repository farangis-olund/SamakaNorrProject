using Infrastructure.Contexts;
using Infrastructure.Entities;
using Infrastructure.Models;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


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
            var userEntity = await _userManager.GetUserAsync(User);

            var viewModel = new SearchViewModel
            {
                SearchModel = new RideSearchModel(),
                CurrentUser = userEntity!.FirstName + " " + userEntity.LastName   // 👈 Pass full name
            };

            if (statusMessage != null)
                ViewData["StatusMessage"] = statusMessage;

            var result = await _searchService.GetAllRequestsAsync();
            
            if (result.StatusCode == Infrastructure.Models.StatusCode.Ok)
            {
                var requests = (List<SearchRequestEntity>)result.ContentResult!;
                var today = DateTime.Today;
                var futureRequests = requests
                     .Where(r => r.DepartureTime.Date >= today)
                    .OrderBy(r => r.DepartureTime)
                    .ToList();

                foreach (var req in futureRequests)
                {
                    var reqUser = await _userManager.FindByIdAsync(req.UserId);
                    var drivingInfo = await _openRouteService.GetDrivingInfoAsync(req.Origin, req.Destination);

                    viewModel.Requests.Add(new SearchRequestModel
                    {
                        Id = req.Id,
                        Origin = req.Origin,
                        Destination = req.Destination,
                        DepartureTime = req.DepartureTime,
                        UserName = reqUser!.FirstName + " " + reqUser.LastName,
                        UserId = reqUser.Id,
                        UserImgUrl = reqUser.ProfileImgUrl,
                        SeatsRequired = req.SeatsRequired,
                        Notes = req.Notes,
                        DistanceKm = drivingInfo?.DistanceKm ?? 0,
                        Duration = drivingInfo?.Duration ?? TimeSpan.Zero,
                        EstimatedArrival = req.DepartureTime + (drivingInfo?.Duration ?? TimeSpan.Zero),

                        Messages = req.Messages.Select(m => new SearchMessageModel
                        {
                            Sender = _context.Users
                                .Where(u => u.Email == m.SenderId)
                                .Select(u => u.FirstName + " " + u.LastName)
                                .FirstOrDefault() ?? m.SenderId,
                            Text = m.MessageContent,
                            Timestamp = m.Timestamp,
                            IsRead = m.IsRead
                        }).OrderBy(m => m.Timestamp).ToList() 
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
                    var today = DateTime.Today;

                    var futureRequests = requests
                        .Where(r => r.DepartureTime.Date >= today)
                        .OrderBy(r => r.DepartureTime)
                        .ToList();

                    foreach (var req in futureRequests)
                    {
                        var reqUser = await _userManager.FindByIdAsync(req.UserId);
                        var drivingInfo = await _openRouteService.GetDrivingInfoAsync(req.Origin, req.Destination);

                        viewModel.Requests.Add(new SearchRequestModel
                        {
                            Id = req.Id,
                            Origin = req.Origin,
                            Destination = req.Destination,
                            DepartureTime = req.DepartureTime,
                            UserName = reqUser!.FirstName + " " + reqUser.LastName,
                            UserId = reqUser.Id,
                            UserImgUrl = reqUser.ProfileImgUrl,
                            SeatsRequired = req.SeatsRequired,
                            Notes = req.Notes,
                            DistanceKm = drivingInfo?.DistanceKm ?? 0,
                            Duration = drivingInfo?.Duration ?? TimeSpan.Zero,
                            EstimatedArrival = req.DepartureTime + (drivingInfo?.Duration ?? TimeSpan.Zero),

                            Messages = req.Messages.Select(m => new SearchMessageModel
                            {
                                Sender = _context.Users
                                    .Where(u => u.Email == m.SenderId)
                                    .Select(u => u.FirstName + " " + u.LastName)
                                    .FirstOrDefault() ?? m.SenderId,
                                Text = m.MessageContent,
                                Timestamp = m.Timestamp,
                                IsRead = m.IsRead
                            }).OrderBy(m => m.Timestamp).ToList()
                        });
                    }

                    return View("Index", viewModel);
                }

                ViewData["StatusMessage"] = "warning|No matching requests found.";
                return View("Index", viewModel);
            }

            ViewData["StatusMessage"] = "danger|Invalid search.";
            return View("Index", viewModel);
        }


        // ✅ GET: Edit request
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var userEntity = await _userManager.GetUserAsync(User);
            var request = await _context.SearchRequests.FindAsync(id);

            if (request == null || request.UserId != userEntity.Id)
                return Unauthorized();

            var model = new SearchRequestModel
            {
                Id = request.Id,
                Origin = request.Origin,
                Destination = request.Destination,
                DepartureTime = request.DepartureTime,
                SeatsRequired = request.SeatsRequired,
                Notes = request.Notes
            };

            return View(model);
        }

        // ✅ POST: Edit request
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(SearchRequestModel model)
        {
            
            var request = await _context.SearchRequests.FindAsync(model.Id);


            if (ModelState.IsValid)
            {
                request.Origin = model.Origin;
                request.Destination = model.Destination;
                request.DepartureTime = (DateTime) model.DepartureTime;
                request.SeatsRequired = model.SeatsRequired;
                request.Notes = model.Notes;

                _context.Update(request);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index", new { statusMessage = "success|Din förfrågan har uppdaterats!" });
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var request = await _context.SearchRequests
                .Include(r => r.Messages) // load related messages
                .FirstOrDefaultAsync(r => r.Id == id);

            if (request == null)
            {
                return RedirectToAction("Index", new { statusMessage = "danger|Förfrågan hittades inte." });
            }

            // ✅ Remove related messages
            if (request.Messages != null && request.Messages.Any())
            {
                _context.SearchMessages.RemoveRange(request.Messages);
            }

            _context.SearchRequests.Remove(request);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", new { statusMessage = "success|Din förfrågan har tagits bort!" });
        }

    }

}



