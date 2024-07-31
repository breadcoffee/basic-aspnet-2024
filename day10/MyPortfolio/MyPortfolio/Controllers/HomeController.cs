using Microsoft.AspNetCore.Mvc;
using MyPortfolio.Data;
using MyPortfolio.Helper;
using MyPortfolio.Models;
using System.Diagnostics;
using System.Security.Cryptography; //암호화

namespace MyPortfolio.Controllers
{
    public class HomeController : Controller
    {
        //private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context;

        //public HomeController(ILogger<HomeController> logger, AppDbContext context)
        //{
        //    _logger = logger;
        //    _context = context;
        //}

        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        // 회원 로그인으로 새로 추가
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // 로그인
        [HttpPost]
        public IActionResult Login(User user)
        {
            // userEmail, password md5로 암호화
            // DB에 있는 값과 비교
            var mdHash = MD5.Create();
            user.Password = Common.GetMd5Hash(mdHash, user.Password);   // 로그인 창에 입력한 비밀번호 암호화

            var result = _context.User.FirstOrDefault(u => u.UserEmail == user.UserEmail && u.Password == user.Password);
            if (result == null) // 로그인할 아이디나 비밀번호가 없다는 거
            {
                return View(user);
            }
            else
            {
                // 로그인 세션처리
                HttpContext.Session.SetInt32("USER_LOGIN_KEY", result.Id);
                HttpContext.Session.SetString("USER_NAME", result.UserName);
                HttpContext.Session.SetString("USER_EMAIL", result.UserEmail);

                return RedirectToAction("Index", "Home"); // 로그인 완료
            }
        }

        // 로그아웃
        [HttpGet]
        public IActionResult Logout(User user)
        {

            // 로그인 세션처리
            HttpContext.Session.Remove("USER_LOGIN_KEY");
            HttpContext.Session.Remove("USER_NAME");
            HttpContext.Session.Remove("USER_EMAIL");

            return RedirectToAction("Index", "Home"); // 로그인 완료
        }

        // 회원 등록 페이지를 열어줘
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // 회원 등록 페이지 내용을 DB에 저장해줘
        [HttpPost]
        public async Task<IActionResult> Register(User user)
        {
            // 비밀번호가 일치하지 않을때 일부러 검증에러를 발생
            if (user.Password != user.PasswordCheck)
            {
                ModelState.AddModelError("PasswordCheck", "비밀번호가 일치하지 않습니다.");
            }

            if (ModelState.IsValid)
            {
                // 모든 값이 정확히 들어왔음
                user.RedDate = DateTime.Now; // 회원 등록날짜 오늘로 지정

                var mdHash = MD5.Create();
                user.Password = Common.GetMd5Hash(mdHash, user.Password);
                user.PasswordCheck = null;

                _context.Add(user); // INSERT
                await _context.SaveChangesAsync(); // COMMIT
                return RedirectToAction("Login");
            }

            return View(user);
        }

        [HttpGet]
        public IActionResult Project()
        {
            // DB Project 테이블 내용을 리스트로 받아서 뷰로 전달
            var list = _context.Project.ToList();

            foreach (var item in list)
            {
                var finalPath = "/uploads/" + item.FilePath;
                item.FilePath = finalPath;
            }
            return View(list);
        }
    }
}
