using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Cai_San_Thu_Vien.Entity;
using Cai_San_Thu_Vien.Models;
using System.Diagnostics;

namespace Cai_San_Thu_Vien.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MinecraftController : Controller
    {
        MinecraftContext _DBContext;

        ResponseAPI re;
        public MinecraftController(MinecraftContext DBContext)
        {
            _DBContext = DBContext;
            re = new ResponseAPI();
        }
        [HttpGet]
        public async Task<IActionResult> GetAllAccount()
        {
            try
            {
                var data = await _DBContext.Accounts.ToListAsync();

                re.message = "Lấy dữ liệu thành công";
                re.success = true;
                re.data = data;

                return Ok(re); //code 200
            }
            catch (Exception ex)
            {
                re.message = "Lấy dữ liệu không thành công";
                re.success = false;
                re.data = ex.Message;

                return BadRequest(re); //code 400
            }
        }
        [HttpPost("CreateAcc")] //Phương thức ghi dữ liệu
        public async Task<IActionResult> CreateAccount(string email, string password, string charname)
        {
            try
            {
                Account account = new Account();
                account.Email = email;
                account.Password = password;
                account.CharName = charname;

                _DBContext.Add(account);
                await _DBContext.SaveChangesAsync();

                re.message = "Tạo tài khoản thành công";
                re.success = true;
                re.data = account;

                return Ok(re); //code 200
            }
            catch (Exception ex)
            {
                re.message = "Tạo tài khoản thất bại";
                re.success = false;
                re.data = ex.Message;

                return BadRequest(re); //code 400
            }
        }
        [HttpPut("UpdateAcc")] //Phương thức cập nhật dữ liệu
        public async Task<IActionResult> UpdateAccount(Account acc)
        {
            try
            {
                var getAcc = await _DBContext.Accounts.FirstOrDefaultAsync(x => x.UId == acc.UId);
                if (getAcc != null) 
                {
                    getAcc.Email = acc.Email;
                    getAcc.Password = acc.Password;
                    getAcc.CharName = acc.CharName;

                    await _DBContext.SaveChangesAsync();

                    re.message = "Cập nhật tài khoản thành công";
                    re.success = true;
                    re.data = getAcc;
                    return Ok(re); //code 200
                }
                else
                {
                    re.message = "Không tìm thấy tài khoản có UID tương ứng";
                    re.success = false;
                    re.data = "Lỗi";
                    return Ok(re); //code 200
                }    
            }
            catch (Exception ex)
            {
                re.message = "Cập nhật tài khoản thất bại";
                re.success = false;
                re.data = ex.Message;

                return BadRequest(re); //code 400
            }
        }
        [HttpPatch("UpdateEmail")] //Phương thức cập nhật dữ liệu, cập nhật 1 phần
        public async Task<IActionResult> UpdateEmail(int uid, string email)
        {
            try
            {
                var getAcc = await _DBContext.Accounts.FirstOrDefaultAsync(x => x.UId == uid);
                if (getAcc != null)
                {
                    getAcc.Email = email;
                    await _DBContext.SaveChangesAsync();
                    re.message = "Cập nhật email tài khoản thành công";
                    re.success = true;
                    re.data = getAcc;
                    return Ok(re); //code 200
                }
                else
                {
                    re.message = "Không tìm thấy tài khoản";
                    re.success = false;
                    re.data = "Lỗi";
                    return Ok(re); //code 200
                }    
            }
            catch (Exception ex)
            {
                re.message = "Cập nhật email tài khoản thất bại";
                re.success = false;
                re.data = ex.Message;

                return BadRequest(re); //code 400
            }
        }
        [HttpDelete("DeleteAcc")] //Phương thức xóa dữ liệu
        public async Task<IActionResult> DeleteAcc(int uid)
        {
            try
            {
                var getAcc = await _DBContext.Accounts.FirstOrDefaultAsync(x => x.UId == uid);
                if (getAcc != null)
                {
                    _DBContext.Accounts.Remove(getAcc);
                    await _DBContext.SaveChangesAsync();
                    re.message = "Xóa tài khoản thành công";
                    re.success = true;
                    re.data = getAcc;
                    return Ok(re); //code 200
                }
                else
                {
                    re.message = "Không tìm thấy tài khoản cần xóa";
                    re.success = false;
                    re.data = "Lỗi";
                    return Ok(re); //code 200
                }
            }
            catch (Exception ex)
            {
                re.message = "Xóa tài khoản thất bại";
                re.success = false;
                re.data = ex.Message;

                return BadRequest(re); //code 400
            }
        }
        [HttpGet("C1")] //Lấy thông tin tất cả các loại tài nguyên trong game
        public async Task<IActionResult> GetAllResource()
        {
            try
            {
                var data = await _DBContext.Resources.ToListAsync();

                re.message = "Lấy danh sách tài nguyên thành công";
                re.success = true;
                re.data = data;

                return Ok(re); //code 200
            }
            catch (Exception ex)
            {
                re.message = "Lấy danh sách tài nguyên thất bại";
                re.success = false;
                re.data = ex.Message;

                return BadRequest(re); //code 400
            }
        }
        [HttpGet("C2")] //Lấy thông tin tất cả người chơi theo từng chế độ chơi
        public async Task<IActionResult> GetPlayersByMode(string modeName)
        {
            try
            {
                // Tìm chế độ chơi theo tên
                var mode = await _DBContext.Modes.FirstOrDefaultAsync(x => x.MName == modeName);
                
                if (mode == null)
                {
                    re.message = "Không tìm thấy chế độ chơi: " + modeName;
                    re.success = false;
                    re.data = "Lỗi";
                    return Ok(re); //code 200
                }

                // Lấy tất cả người chơi có chế độ chơi này, kèm thông tin Mode và Account
                var data = await _DBContext.Plays
                    .Where(x => x.MId == mode.MId)
                    .Include(x => x.MIdNavigation) // Load thông tin Mode
                    .Include(x => x.UIdNavigation) // Load thông tin Account
                    .ToListAsync();

                re.message = "Lấy danh sách người chơi theo chế độ chơi thành công";
                re.success = true;
                re.data = data;

                return Ok(re); //code 200
            }
            catch (Exception ex)
            {
                re.message = "Lấy danh sách người chơi theo chế độ chơi thất bại";
                re.success = false;
                re.data = ex.Message;

                return BadRequest(re); //code 400
            }
        }
        [HttpGet("C3")] //Lấy tất cả các vũ khí có giá trị trên 100 điểm kinh nghiệm
        public async Task<IActionResult> GetWeaponsByExp(int minExp = 100)
        {
            try
            {
                // Lấy tất cả vũ khí (iKind = 1) có Quest liên quan với exp > minExp
                var data = await _DBContext.Items
                    .Where(x => x.IKind == 1 && x.Quests.Any(q => q.Exp > minExp))
                    .Select(x => new
                    {
                        x.IId,
                        x.IName,
                        x.IImg,
                        x.IPrice,
                        x.IKind,
                        MaxQuestExp = x.Quests.Max(q => q.Exp ?? 0) // Lấy exp cao nhất của quest liên quan
                    })
                    .ToListAsync();

                re.message = "Lấy danh sách vũ khí có giá trị trên " + minExp + " điểm kinh nghiệm thành công";
                re.success = true;
                re.data = data;

                return Ok(re); //code 200
            }
            catch (Exception ex)
            {
                re.message = "Lấy danh sách vũ khí thất bại";
                re.success = false;
                re.data = ex.Message;

                return BadRequest(re); //code 400
            }
        }
        [HttpGet("C4")] //Lấy thông tin các item mà người chơi có thể mua với số điểm kinh nghiệm tích lũy hiện tại
        public async Task<IActionResult> GetAffordableItems(int pId)
        {
            try
            {
                // Tìm người chơi theo pId
                var play = await _DBContext.Plays.FirstOrDefaultAsync(x => x.PId == pId);
                
                if (play == null)
                {
                    re.message = "Không tìm thấy người chơi có ID: " + pId;
                    re.success = false;
                    re.data = "Lỗi";
                    return Ok(re); //code 200
                }

                // Lấy exp hiện tại của người chơi (mặc định 0 nếu null)
                int playerExp = play.Exp ?? 0;

                // Lấy tất cả item có giá <= exp của người chơi
                var data = await _DBContext.Items
                    .Where(x => x.IPrice <= playerExp)
                    .Select(x => new
                    {
                        x.IId,
                        x.IName,
                        x.IImg,
                        x.IPrice,
                        x.IKind,
                        RemainingExp = playerExp - (x.IPrice ?? 0) // Số exp còn lại sau khi mua
                    })
                    .OrderBy(x => x.IPrice) // Sắp xếp theo giá tăng dần
                    .ToListAsync();

                re.message = "Lấy danh sách item có thể mua thành công. Exp hiện tại: " + playerExp;
                re.success = true;
                re.data = new
                {
                    PlayerExp = playerExp,
                    Items = data
                };

                return Ok(re); //code 200
            }
            catch (Exception ex)
            {
                re.message = "Lấy danh sách item có thể mua thất bại";
                re.success = false;
                re.data = ex.Message;

                return BadRequest(re); //code 400
            }
        }
        [HttpGet("C5")] //Lấy thông tin các item có tên chứa từ 'kim cương' và có giá trị dưới 500 điểm kinh nghiệm
        public async Task<IActionResult> GetItemsByKeywordAndPrice(string keyword = "kim cương", int maxPrice = 500)
        {
            try
            {
                // Lấy tất cả item có tên chứa keyword (không phân biệt hoa thường) và giá < maxPrice
                // Dùng ToLower() để so sánh không phân biệt hoa thường (EF Core có thể translate sang SQL)
                var data = await _DBContext.Items
                    .Where(x => x.IName.ToLower().Contains(keyword.ToLower()) 
                            && (x.IPrice ?? 0) < maxPrice)
                    .Select(x => new
                    {
                        x.IId,
                        x.IName,
                        x.IImg,
                        x.IPrice,
                        x.IKind
                    })
                    .OrderBy(x => x.IPrice) // Sắp xếp theo giá tăng dần
                    .ToListAsync();

                // Kiểm tra nếu không tìm thấy item nào
                if (data.Count == 0)
                {
                    re.message = "Không có item nào có tên '" + keyword + "' với giá < " + maxPrice;
                    re.success = false;
                    re.data = "Không tìm thấy";
                    return Ok(re); //code 200
                }

                re.message = "Lấy danh sách item có tên chứa '" + keyword + "' và giá < " + maxPrice + " thành công";
                re.success = true;
                re.data = data;

                return Ok(re); //code 200
            }
            catch (Exception ex)
            {
                re.message = "Lấy danh sách item thất bại";
                re.success = false;
                re.data = ex.Message;

                return BadRequest(re); //code 400
            }
        }
        [HttpGet("C6")] //Lấy thông tin tất cả các giao dịch mua item và phương tiện của một người chơi cụ thể, sắp xếp theo thứ tự thời gian
        public async Task<IActionResult> GetPlayerTransactions(int pId)
        {
            try
            {
                // Kiểm tra người chơi có tồn tại không
                var play = await _DBContext.Plays.FirstOrDefaultAsync(x => x.PId == pId);
                if (play == null)
                {
                    re.message = "Không tìm thấy người chơi có pID: " + pId;
                    re.success = false;
                    re.data = "Lỗi";
                    return Ok(re); //code 200
                }

                // Lấy giao dịch mua item (Inventory) - dùng Play.time làm thời gian
                var inventoryTransactions = await _DBContext.Inventories
                    .Where(x => x.PId == pId)
                    .Select(x => new
                    {
                        TransactionId = x.InId,
                        TransactionType = "Mua Item",
                        Time = x.PIdNavigation.Time,
                        ItemId = x.IId,
                        ItemName = x.IIdNavigation.IName,
                        ItemImg = x.IIdNavigation.IImg,
                        ItemPrice = x.IIdNavigation.IPrice,
                        Quantity = x.Quantity ?? 0,
                        RecipeName = (string?)null,
                        RecipeId = (int?)null
                    })
                    .ToListAsync();

                // Lấy giao dịch craft item (Craft) - dùng Craft.time
                var craftTransactions = await _DBContext.Crafts
                    .Where(x => x.PId == pId && x.Time != null)
                    .Select(x => new
                    {
                        TransactionId = x.CId,
                        TransactionType = "Craft Item",
                        Time = x.Time ?? x.PIdNavigation.Time,
                        ItemId = x.Rc.IIdNavigation.IId,
                        ItemName = x.Rc.IIdNavigation.IName,
                        ItemImg = x.Rc.IIdNavigation.IImg,
                        ItemPrice = x.Rc.IIdNavigation.IPrice,
                        Quantity = 1,
                        RecipeName = (string?)x.Rc.RcName,
                        RecipeId = (int?)x.RcId
                    })
                    .ToListAsync();

                // Gộp và sắp xếp theo thời gian
                var allTransactions = inventoryTransactions
                    .Concat(craftTransactions)
                    .OrderBy(x => x.Time)
                    .ToList();

                if (allTransactions.Count() == 0)
                {
                    re.message = "Người chơi có pID " + pId + " chưa có giao dịch nào";
                    re.success = false;
                    re.data = "Không tìm thấy";
                    return Ok(re); //code 200
                }

                re.message = "Lấy danh sách giao dịch của người chơi thành công";
                re.success = true;
                re.data = allTransactions;

                return Ok(re); //code 200
            }
            catch (Exception ex)
            {
                re.message = "Lấy danh sách giao dịch thất bại";
                re.success = false;
                re.data = ex.Message;

                return BadRequest(re); //code 400
            }
        }


        [HttpPatch("C8")] //Cập nhật mật khẩu của người chơi
        public async Task<IActionResult> UpdatePassword(int uid, string newPassword)
        {
            try
            {
                // Tìm tài khoản theo uid (mã người chơi)
                var account = await _DBContext.Accounts.FirstOrDefaultAsync(x => x.UId == uid);
                if (account == null)
                {
                    re.message = "Không tìm thấy tài khoản có UID: " + uid;
                    re.success = false;
                    re.data = "Không tìm thấy";
                    return Ok(re); //code 200
                }

                // Cập nhật mật khẩu mới
                account.Password = newPassword;
                await _DBContext.SaveChangesAsync();

                re.message = "Cập nhật mật khẩu thành công";
                re.success = true;
                re.data = new
                {
                    account.UId,
                    account.Email,
                    account.CharName
                };

                return Ok(re); //code 200
            }
            catch (Exception ex)
            {
                re.message = "Cập nhật mật khẩu thất bại";
                re.success = false;
                re.data = ex.Message;

                return BadRequest(re); //code 400
            }
        }

        [HttpGet("C9")] //Lấy danh sách các item được mua nhiều nhất
        public async Task<IActionResult> GetTopPurchasedItems()
        {
            try
            {
                // Nhóm theo item, tính tổng số lượng mua, sắp xếp giảm dần theo tổng số lượng
                var data = await _DBContext.Inventories
                    .GroupBy(x => new { x.IId, x.IIdNavigation.IName, x.IIdNavigation.IImg, x.IIdNavigation.IPrice, x.IIdNavigation.IKind })
                    .Select(g => new
                    {
                        ItemId = g.Key.IId,
                        ItemName = g.Key.IName,
                        ItemImg = g.Key.IImg,
                        ItemPrice = g.Key.IPrice,
                        ItemKind = g.Key.IKind,
                        TotalQuantity = g.Sum(x => x.Quantity ?? 0)
                    })
                    .OrderByDescending(x => x.TotalQuantity)
                    .ToListAsync();

                if (data.Count == 0)
                {
                    re.message = "Chưa có giao dịch mua item nào";
                    re.success = false;
                    re.data = "Không tìm thấy";
                    return Ok(re); //code 200
                }

                re.message = "Lấy danh sách các item được mua nhiều nhất thành công";
                re.success = true;
                re.data = data;

                return Ok(re); //code 200
            }
            catch (Exception ex)
            {
                re.message = "Lấy danh sách các item được mua nhiều nhất thất bại";
                re.success = false;
                re.data = ex.Message;

                return BadRequest(re); //code 400
            }
        }

        [HttpGet("C10")] //Lấy danh sách tất cả người chơi và số lần họ đã mua hàng
        public async Task<IActionResult> GetPlayersPurchaseCount()
        {
            try
            {
                // Nhóm theo người chơi (Account), đếm số lần mua (số dòng Inventory) và tổng số lượng mua
                var data = await _DBContext.Inventories
                    .GroupBy(x => new
                    {
                        x.PIdNavigation.UId,
                        x.PIdNavigation.UIdNavigation.Email,
                        x.PIdNavigation.UIdNavigation.CharName
                    })
                    .Select(g => new
                    {
                        PlayerId = g.Key.UId,
                        Email = g.Key.Email,
                        CharName = g.Key.CharName,
                        PurchaseCount = g.Count(),                // Số lần mua (số giao dịch)
                        TotalQuantity = g.Sum(x => x.Quantity ?? 0) // Tổng số item đã mua
                    })
                    .OrderByDescending(x => x.PurchaseCount)
                    .ToListAsync();

                if (data.Count == 0)
                {
                    re.message = "Chưa có giao dịch mua hàng nào";
                    re.success = false;
                    re.data = "Không tìm thấy";
                    return Ok(re); //code 200
                }

                re.message = "Lấy danh sách tất cả người chơi và số lần họ đã mua hàng thành công";
                re.success = true;
                re.data = data;

                return Ok(re); //code 200
            }
            catch (Exception ex)
            {
                re.message = "Lấy danh sách tất cả người chơi và số lần họ đã mua hàng thất bại";
                re.success = false;
                re.data = ex.Message;

                return BadRequest(re); //code 400
            }
        }

        //Tải lên file
        [HttpPost("upload")]
        public async Task<IActionResult> UploadFiles([FromForm] List<IFormFile> files)
        {
            string savepath = "D:/";
            ResponseAPI re = new ResponseAPI(); //xem lớp ResponseAPI tại phần 4 - REST API
            try
            {
                //nếu không có file được đưa vào
                if (files == null || files.Count == 0)
                {
                    re.message = "Không có file nào được chọn";
                    re.success = false;
                    re.data = "Không có file";
                    return BadRequest(re);
                }

                ////Sử dụng thư mục Project/wwwroot/uploads để lưu files
                //var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), ""); //Thay chỗ này nếu muốn lưu ở thư mục khác
                ////Tạo thư mục nếu chưa có
                //Directory.CreateDirectory(uploadPath);


                var uploadPath = "D:/";

                //Khai báo danh sách string để lấy ra tên của các file đã upload thành công
                var uploadedfiles = new List<string>();
                Debug.WriteLine("========");

                //Duyệt qua các files được truyền vào
                foreach (var file in files)
                {
                    //Tạo đường dẫn lưu file gồm đường dẫn tới thư mục upload và tên file
                    var filePath = Path.Combine(uploadPath, file.FileName);
                    Debug.WriteLine(filePath);
                    //Sử dụng Lớp FileStream để mở 1 file với chế độ tạo mới theo đường dẫn ở trên
                    using (FileStream fs = new FileStream(filePath, FileMode.Create))
                    {
                        //file được copy và chờ cho đến khi tạo xong file bằng FileStream
                        await file.CopyToAsync(fs);
                    }
                    //Thêm tên file vào trong danh sách các file đã upload thành công
                    uploadedfiles.Add(file.FileName);
                }

                re.message = "upload thành công";
                re.success = true;
                re.data = uploadedfiles;

                return Ok(re);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
