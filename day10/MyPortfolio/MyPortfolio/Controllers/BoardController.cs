using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyPortfolio.Data;
using MyPortfolio.Models;

namespace MyPortfolio.Controllers
{
    public class BoardController : Controller
    {
        private readonly AppDbContext _context;

        public BoardController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Board
        // FromSql()로 작업시는 비동기 async, await, Task<> 를 걷어내야함
        // AppDbContext(DB핸들링객체) 안의 Board DBSet객체에다가
        // 들어있는 데이터를 리스트로 가져와서
        // 화면으로 보낸 다음에 출력하라
        // Views/Board/Index.cshtml을 화면에 뿌려라
        // return View(await _context.Board.ToListAsync());
        public IActionResult Index(int page = 1, string search = "")
        {
            var totalCount = _context.Board.FromSqlRaw<Board>($"SELECT * FROM Board WHERE Title LIKE '%{search}%'").Count(); // 총 글갯수
            var countList = 10; // 페이지별 게시글 수
            var totalPage = totalCount / countList; // 총 페이지 수

            if (totalCount % countList > 0) totalPage++; // 12 % 10 = 2 > 0 --> 한페이지가 더 필요
            if (totalPage < page) page = totalPage; // 현재 페이지 번호가 전체 페이지 수보다 크면 축소

            var countPage = 10; // 밑에 페이지번호 수
            var startPage = ((page - 1) / countList) * countPage + 1;   // 1~10페이지, 11~20페이지 식으로
            var endPage = startPage + countPage - 1;                    // 1페이지부터 시작하면 10페이지가 마지막
            if (totalPage < endPage) endPage = totalPage; // 2페이지까지 밖에 없으면 endPage 10 -> 2 로 변경

            // 쿼리로 넘길 값
            var startCount = ((page - 1) * countPage) + 1;  // 1, 11, 21, 순으로
            var endCount = startCount + (countPage - 1);      // 10, 20, 30... 순으로

            // ViewData, ViewBag 변수... (뷰에서 사용할 변수)
            ViewBag.StartPage = startPage;
            ViewBag.EndPage = endPage;
            ViewBag.Page = page;
            ViewBag.TotalPage = totalPage;
            ViewBag.TotalCount = totalCount;    // 전체 글 갯수
            ViewBag.Search = search;    // 검색결과

            // 쿼리는 나중에 수정필요!!!
            var list = _context.Board.FromSqlRaw<Board>(@$"
                SELECT *
                  FROM (
	                SELECT ROW_NUMBER() OVER (ORDER BY b.Id DESC) AS rowNum
                          ,b.Id
                          ,b.UserId
                          ,b.UserName AS UserName1
                          ,b.Title
                          ,b.Contents
                          ,b.Hit
                          ,b.RegDate
                          ,b.ModDate
	                      ,u.UserName
                      FROM Board AS b
                      LEFT JOIN [User] AS u
                        ON b.UserId = u.Id
                     WHERE b.Title LIKE '%{search}%'
		          ) AS base
                  WHERE base.rowNum BETWEEN {startCount} AND {endCount}
            ").ToList();

            return View(list);
        }

        // 게시글 상세 읽기
        // GET: Board/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var board = await _context.Board
                .Include(u => u.User!)  // NUll로 관계가 형성된 부모/자식의 객체 값도 같이 포함시켜서 보여달라
                .FirstOrDefaultAsync(m => m.Id == id);

            if (board == null)
            {
                return NotFound();
            }

            board.Hit = board.Hit == null ? 1 : board.Hit + 1;// 게시글 조회수 +1
            _context.Update(board); // 객체에 내용 반영
            await _context.SaveChangesAsync();  // 실제 DB에 반영

            // 사용자 객체 가져옴

            return View(board); // 게시글 하나를 뷰로 던져줘
        }

        // GET: Board/Create
        // 링크를 클릭해서 화면 전환
        [HttpGet]
        public IActionResult Create()
        {
            if(HttpContext.Session.GetInt32("USER_LOGIN_KEY") == null)
            {
                // 로그인을 안했으니 로그인 창으로 가라
                return RedirectToAction("Login");
            }

            ViewData["USER_NAME"] = HttpContext.Session.GetString("USER_NAME");
            // Views/Board/Create.cshtml 화면을 출력하라
            return View();
        }

        // POST: Board/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Contents,Hit,RegDate,ModeDate")] Board board)
        {
            // 아무값도 입력하지 않으면 ModelState.IsValid는 false
            if (ModelState.IsValid)
            {
                // 사용자 객체 가져옴
                User currUser = await _context.User.FirstOrDefaultAsync(u => u.UserEmail == HttpContext.Session.GetString("USER_EMAIL"));

                if (currUser == null)
                {
                    return RedirectToAction("Index");
                }

                board.User = currUser;  // 현재 로그인한 사용자를 할당
                board.RegDate = DateTime.Now;

                _context.Add(board); // DB 객체에 저장
                // Insert to Commit 데이터베이스 커밋
                await _context.SaveChangesAsync();
                // 게시판 목록화면으로 돌아감
                return RedirectToAction(nameof(Index));
            }
            return View(board);
        }

        // GET: Board/Edit/5
        public async Task<IActionResult> Edit(int? id) // 수정
        {
            if (HttpContext.Session.GetInt32("USER_LOGIN_KEY") == null)
            {
                // 로그인을 안했으니 로그인 창으로 가라
                return RedirectToAction("Login");
            }

            ViewData["USER_NAME"] = HttpContext.Session.GetString("USER_NAME");

            if (id == null)
            {
                return NotFound();
            }

            var board = await _context.Board.FindAsync(id);
            if (board == null)
            {
                return NotFound();
            }
            return View(board);
        }

        // POST: Board/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Contents,Hit,RegDate,ModeDate")] Board board)
        {
            if (id != board.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // 수정날짜 추가!
                    board.ModDate = DateTime.Now; // 현재 수정하는 날짜시간을 입력
                    _context.Update(board);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BoardExists(board.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(board);
        }

        // GET: Board/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var board = await _context.Board
                .FirstOrDefaultAsync(m => m.Id == id);
            if (board == null)
            {
                return NotFound();
            }

            return View(board);
        }

        // POST: Board/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var board = await _context.Board.FindAsync(id);
            if (board != null)
            {
                _context.Board.Remove(board); // 객체 삭제
            }

            // DB Delete 후에 Commit
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BoardExists(int id)
        {
            return _context.Board.Any(e => e.Id == id);
        }
    }
}
