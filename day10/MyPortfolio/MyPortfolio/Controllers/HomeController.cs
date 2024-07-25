using Microsoft.AspNetCore.Mvc;
using MyPortfolio.Data;
using MyPortfolio.Helper;
using MyPortfolio.Models;
using System.Diagnostics;
using System.Security.Cryptography; //��ȣȭ

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

        // ȸ�� �α������� ���� �߰�
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // �α���
        [HttpPost]
        public IActionResult Login(User user)
        {
            // userEmail, password md5�� ��ȣȭ
            // DB�� �ִ� ���� ��
            var mdHash = MD5.Create();
            user.Password = Common.GetMd5Hash(mdHash, user.Password);   // �α��� â�� �Է��� ��й�ȣ ��ȣȭ

            var result = _context.User.FirstOrDefault(u => u.UserEmail == user.UserEmail && u.Password == user.Password);
            if (result == null) // �α����� ���̵� ��й�ȣ�� ���ٴ� ��
            {
                return View(user);
            }
            else
            {
                // �α��� ����ó��
                HttpContext.Session.SetInt32("USER_LOGIN_KEY", result.Id);
                HttpContext.Session.SetString("USER_NAME", result.UserName);
                HttpContext.Session.SetString("USER_EMAIL", result.UserEmail);

                return RedirectToAction("Index", "Home"); // �α��� �Ϸ�
            }
        }

        // �α׾ƿ�
        [HttpGet]
        public IActionResult Logout(User user)
        {

            // �α��� ����ó��
            HttpContext.Session.Remove("USER_LOGIN_KEY");
            HttpContext.Session.Remove("USER_NAME");
            HttpContext.Session.Remove("USER_EMAIL");

            return RedirectToAction("Index", "Home"); // �α��� �Ϸ�
        }

        // ȸ�� ��� �������� ������
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // ȸ�� ��� ������ ������ DB�� ��������
        [HttpPost]
        public async Task<IActionResult> Register(User user)
        {
            // ��й�ȣ�� ��ġ���� ������ �Ϻη� ���������� �߻�
            if (user.Password != user.PasswordCheck)
            {
                ModelState.AddModelError("PasswordCheck", "��й�ȣ�� ��ġ���� �ʽ��ϴ�.");
            }

            if (ModelState.IsValid)
            {
                // ��� ���� ��Ȯ�� ������
                user.RedDate = DateTime.Now; // ȸ�� ��ϳ�¥ ���÷� ����

                var mdHash = MD5.Create();
                user.Password = Common.GetMd5Hash(mdHash, user.Password);
                user.PasswordCheck = null;

                _context.Add(user); // INSERT
                await _context.SaveChangesAsync(); // COMMIT
                return RedirectToAction("Login");
            }

            return View(user);
        }

        public IActionResult Project()
        {
            return View();
        }
    }
}