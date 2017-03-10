using System.Data.Entity;

namespace CcvSignIn.db
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext() : base("DatabaseContext")
        {
        }

        public DbSet<Child>  Children { get; set; }
        public DbSet<SignIn> SignIns  { get; set; }
    }

    public class Child
    {
	    public int    id              { get; set; }
	    public string first_name      { get; set; }
        public string last_name       { get; set; }
        public int    date_of_birth   { get; set; }
        public int    update_required { get; set; }
        public int    medical_flag    { get; set; }
    }

    public class SignIn
    {
        public int    id           { get; set; }
        public string first_name   { get; set; }
        public string last_name    { get; set; }
        public string room         { get; set; }
        public int    sign_in_time { get; set; }
        public string label        { get; set; }
        public int    newcomer     { get; set; }
        public int    child_id     { get; set; }
        public int    remote_id    { get; set; }
    }

}
