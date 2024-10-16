
//namespace Models.InnmelderModels
//{

//    public class UserCase
//    {
//        public int CaseId { get; set; }
//        public required string Subject { get; set; }
//        public required string Status { get; set; }
//        public required string GeoLocation { get; set; }
//    }


//    public class CaseService
//    {
//        private readonly ApplicationDbContext _context;

//        public CaseService(ApplicationDbContext context)
//        {
//            _context = context;
//        }


//        //Retrieves list of user cases from the database based on the username. If there is an error, an empty list is returned.
//        public List<UserCase> GetUserCasesForUser(string username)
//        {
//            if (string.IsNullOrEmpty(username))
//            {
//                throw new ArgumentNullException(nameof(username), "Username is null or empty.");
//            }
//            try
//            {
//                // Query the database for the user's cases.
//                // _context.Cases represents collection of cases in the database.
//                // Where() filters and includes only cases that match the username.
//                // Select() creates a new UserCase object for each case that is found. Filling details from the database.
//                // ToList() converts the query result to a list.
//                return _context.Cases
//                    .Where(c => c.Username == username)
//                    .Select(c => new UserCase
//                    {
//                        CaseId = c.Id,
//                        Subject = c.Description,
//                        Status = c.Status,
//                        GeoLocation = $"/Home/MapView?id={c.GeoLocationId}"
//                    })
//                    .ToList();
//            }
//            catch (Exception e)
//            {
//                // Log the error to the console
//                Console.WriteLine($"ERROR: An error occurred while fetching user cases for username: {username}");
//                Console.WriteLine($"Exception: {e.Message}");

//                // Adds the error message to a log file named log.txt, including the current date and time. This way, you can check the log file later to see what errors happened.
//                File.AppendAllText("log.txt", $"{DateTime.Now}: ERROR - {e.Message}\n");

//                // Return an empty list in case of an error
//                return new List<UserCase>();
//            }
//        }
//    }

//    public class ApplicationDbContext : DbContext
//    {
//        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
//            : base(options)
//        {
//        }

//        public DbSet<Case> Cases { get; set; }
//    }
//}


