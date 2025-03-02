namespace DoAnLTW.Models.Repositories
{
    public class EAccount : IAccount
    {
        private readonly ApplicationDbContext _context;

        public EAccount(ApplicationDbContext context) // Đổi tên constructor
        {
            _context = context;
        }

        public Account GetAccountByUsername(string username)
        {
            return _context.Accounts.FirstOrDefault(a => a.username == username);
        }

        public void CreateAccount(Account account)
        {
            _context.Accounts.Add(account);
            _context.SaveChanges();
        }

        public bool ValidateLogin(string username, string password)
        {
            var account = _context.Accounts.FirstOrDefault(a => a.username == username);
            return account != null && account.password == password;
        }

        public List<Account> GetAllAccounts()
        {
            return _context.Accounts.ToList();
        }
    }
}
