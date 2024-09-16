using Homeverse.Domain.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;
using System.Transactions;

namespace Homeverse.Infrastructure.Data;

public class UnitOfWork : IUnitOfWork
{
    private IDbContextTransaction? _transaction;
    private bool _disposed;
    private readonly HomeverseDbContext _context;

    public UnitOfWork(HomeverseDbContext context)
    {
        _context = context;
    }

    public int SaveChanges() => _context.SaveChanges();

    public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();

    public void BeginTransaction()
    {
        _transaction = _context.Database.BeginTransaction();
    }

    public void Commit()
    {
        if (_transaction == null)
        {
            throw new TransactionException("No transaction to commit");
        }
        try
        {
            _context.SaveChanges();
            _transaction.Commit();
        }
        finally
        {
            _transaction.Dispose();
            _transaction = null;
        }
    }

    public async Task CommitAsync()
    {
        if (_transaction == null)
        {
            throw new TransactionException("No transaction to commit");
        }

        try
        {
            await _context.SaveChangesAsync();
            _transaction.Commit();
        }
        finally
        {
            _transaction.Dispose();
            _transaction = null;
        }
    }

    public void Rollback()
    {
        if (_transaction == null)
        {
            throw new TransactionException("No transaction to commit");
        }

        _transaction.Rollback();
        _transaction.Dispose();
        _transaction = null;
    }

    public async Task RollbackAsync()
    {
        if (_transaction == null)
        {
            throw new TransactionException("No transaction to commit");
        }

        await _transaction.RollbackAsync();
        _transaction.Dispose();
        _transaction = null;
    }


    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _transaction?.Dispose();
                _context.Dispose();
            }

            _disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public async Task ExecuteTransactionAsync(Action action)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            action();
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            throw new TransactionException("Can't execute transaction: " + ex);
        }
    }

    public async Task ExecuteTransactionAsync(Func<Task> action)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            await action();
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            throw new TransactionException("Can't execute transaction: " + ex);
        }
    }
}
