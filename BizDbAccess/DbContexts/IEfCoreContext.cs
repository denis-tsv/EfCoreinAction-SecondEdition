using System;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Infrastructure.Interfaces.DbContexts
{
    public interface IEfCoreContext
    {
        DbSet<Book> Books { get; }
        DbSet<Author> Authors { get; }
        DbSet<PriceOffer> PriceOffers { get; }
        DbSet<Tag> Tags { get; }
        DbSet<Order> Orders { get; }

        DbSet<T> Set<T>() where T : class;

        int SaveChanges();
        IDbContextTransaction BeginTransaction();
        ImmutableList<ValidationResult> SaveChangesWithValidation();
        Task<ImmutableList<ValidationResult>> SaveChangesWithValidationAsync();
    }
}