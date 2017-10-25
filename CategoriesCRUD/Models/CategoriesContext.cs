namespace CategoriesCRUD
{
    using System.Data.Entity;
    using CategoriesCRUD.Models;

    public partial class CategoriesContext : DbContext
    {
        public CategoriesContext()
            : base("name=CategoriesContext")
        {
        }

        public virtual DbSet<Category> Category { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>()
                .HasMany(e => e.Childs)
                .WithOptional(e => e.Parent)
                .HasForeignKey(e => e.ParentId)
                .WillCascadeOnDelete(true);
        }
    }
}
