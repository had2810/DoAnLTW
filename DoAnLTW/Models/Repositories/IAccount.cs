namespace DoAnLTW.Models.Repositories
{
    public interface IAccount
    {
        public interface IAccount
        {
            Account GetAccountByUsername(string username); // Lấy thông tin tài khoản theo Username
            void CreateAccount(Account account); // Thêm tài khoản mới
            bool ValidateLogin(string username, string password); // Kiểm tra đăng nhập
            List<Account> GetAllAccounts(); // Lấy danh sách tất cả tài khoản
        }
    }
}
