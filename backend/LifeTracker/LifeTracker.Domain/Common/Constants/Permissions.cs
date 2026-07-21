namespace LifeTracker.Domain.Common.Constants;

public static class Permissions
{
    public const string ClaimType = "permission";

    public static class Todo
    {
        public const string Read = "Todo.Read";
        public const string Create = "Todo.Create";
        public const string Update = "Todo.Update";
        public const string Delete = "Todo.Delete";
    }

    public static class Category
    {
        public const string Create = "Category.Create";
        public const string Update = "Category.Update";
        public const string Read = "Category.Read";
        public const string Delete = "Category.Delete";
    }

    public static class User
    {
        public const string Create = "User.Create";
        public const string Read = "User.Read";
        public const string Update = "User.Update";
        public const string Delete = "User.Delete";
    }

    public static class Admin
    {
        public const string Users = "Admin.Users";
        public const string Roles = "Admin.Roles";
    }

    public static IReadOnlyCollection<string> All { get; } =
    [
        Todo.Read,
        Todo.Create,
        Todo.Update,
        Todo.Delete,
        Category.Read,
        Category.Create,
        Category.Update,
        Category.Delete,
        User.Create,
        User.Read,
        User.Update,
        User.Delete,
        Admin.Users,
        Admin.Roles
    ];
}
