using Microsoft.AspNetCore.Mvc;
using MyPortfolio.Data;

namespace MyPortfolio.Controllers
{
    [Route("api/[controller]")] // api/BoardRest로 URL을 만들어줌
    [ApiController]
    public class BoardRestController : ControllerBase
    {
        private readonly AppDbContext _context;     // 이걸 추가해야 DB 사용가능

        public BoardRestController(AppDbContext context)
        {
            _context = context;
        }

        //[HttpGet]       // 화면 요청할 때 사용되는 메서드
        //[HttpPost]      // 값을 저장할 때 사용되는 메서드
        //[HttpPut]       // 값을 수정할 때
        //[HttpPatch]     // 값중에서 한가지만 수정할 때
        [HttpDelete("{id}")]    // 값을 삭제할 때
        public async Task<IActionResult> DeleteBoard(int id)
        {
            var board = await _context.Board.FindAsync(id); // select * .. where id = id
            if (board == null)
            {
                return NotFound();  // 404 Error
            }

            _context.Board.Remove(board);   // ASP.NET 상 메모리에 들어있는 DB객체 데이터 삭제
            await _context.SaveChangesAsync();  // DB가 삭제되고 Commit

            return NoContent(); // 아무 데이터도 없음을 리턴.
        }
    }
}
