public class SeedData
{
    public static void Seed()
    {
        var context = new UserContext();

        // Add students
        Users Clem = new Users
        {
            LastName = "Carrere",
            FirstName = "Clémence",
            UserName = "carrereclem",
            Password = "Clem22",
            Admin = true,
        };
        Users François = new Users
        {
            LastName = "Robillard",
            FirstName = "François",
            UserName = "robifrance",
            Password = "Franfran",
            Admin = false,
        };
        Users Faustine = new Users
        {
            LastName = "Picavet",
            FirstName = "Faustine",
            UserName = "faufaulesuppot",
            Password = "Faustine",
            Admin = false,
        };
        context.Users.AddRange(Clem, François, Faustine);

        context.SaveChanges();
    }
}
