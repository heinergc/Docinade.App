# SQL Server Execution Strategy Fix

## Problem

When using SQL Server with Entity Framework Core's retry execution strategy (`SqlServerRetryingExecutionStrategy`), you cannot use explicit transactions with `BeginTransactionAsync()` directly. This causes the error:

```
The configured execution strategy 'SqlServerRetryingExecutionStrategy' does not support user-initiated transactions. Use the execution strategy returned by 'DbContext.Database.CreateExecutionStrategy()' to execute all the operations in the transaction as a retriable unit.
```

## Solution

### 1. Created DbContext Extensions

**File: `src\RubricasApp.Web\Extensions\DbContextExtensions.cs`**

This provides helper methods to properly handle transactions with SQL Server's retry strategy:

- `ExecuteWithStrategyAsync<T>()` - For operations with return values
- `ExecuteWithStrategyAsync()` - For operations without return values
- `ExecuteInTransactionAsync<T>()` - For transactional operations with return values
- `ExecuteInTransactionAsync()` - For transactional operations without return values

### 2. Updated SqlServerDatabaseInitializer

**File: `src\RubricasApp.Web\Utils\SqlServerDatabaseInitializer.cs`**

The database initializer now properly uses execution strategy for initialization operations:

```csharp
// Use the execution strategy to handle retries and transactions properly
var strategy = _context.Database.CreateExecutionStrategy();

await strategy.ExecuteAsync(async () =>
{
    using var transaction = await _context.Database.BeginTransactionAsync();
    try
    {
        // Database operations...
        await transaction.CommitAsync();
    }
    catch
    {
        await transaction.RollbackAsync();
        throw;
    }
});
```

### 3. Updated EvaluacionesController

**File: `src\RubricasApp.Web\Controllers\EvaluacionesController.cs`**

The controller now uses the extension methods instead of direct transaction handling:

**Before (Problematic):**
```csharp
using var transaction = await _context.Database.BeginTransactionAsync();
try
{
    // Operations...
    await transaction.CommitAsync();
}
catch (Exception ex)
{
    await transaction.RollbackAsync();
    // Error handling...
}
```

**After (Fixed):**
```csharp
var result = await _context.ExecuteInTransactionAsync(async () =>
{
    // Operations...
    return result;
});
```

## Usage Guidelines

### For New Controllers

When you need to use transactions in controllers with SQL Server:

```csharp
using RubricasApp.Web.Extensions;

// In your controller:
public async Task<IActionResult> SomeAction()
{
    try
    {
        var result = await _context.ExecuteInTransactionAsync(async () =>
        {
            // Your database operations here
            // Return any object you need
            return someResult;
        });
        
        // Success handling
        return View(result);
    }
    catch (Exception ex)
    {
        // Error handling
        ModelState.AddModelError("", ex.Message);
        return View();
    }
}
```

### For Non-Transactional Operations

For operations that need retry logic but don't need transactions:

```csharp
var result = await _context.ExecuteWithStrategyAsync(async () =>
{
    // Your database operations here
    return await _context.SomeEntity.ToListAsync();
});
```

## Benefits

1. **Compatibility**: Works correctly with SQL Server's retry execution strategy
2. **Reliability**: Automatic retry logic for transient failures
3. **Consistency**: Centralized transaction handling logic
4. **Simplicity**: Easy to use extension methods
5. **Error Handling**: Proper rollback and error propagation

## Files Modified

- ? `src\RubricasApp.Web\Extensions\DbContextExtensions.cs` (NEW)
- ? `src\RubricasApp.Web\Utils\SqlServerDatabaseInitializer.cs` (UPDATED)
- ? `src\RubricasApp.Web\Controllers\EvaluacionesController.cs` (UPDATED)

## Testing

After applying these changes:

1. ? Application compiles successfully
2. ? Database initialization works properly
3. ? Creating evaluations should work without the execution strategy error
4. ? Editing evaluations should work without the execution strategy error

The error `"The configured execution strategy 'SqlServerRetryingExecutionStrategy' does not support user-initiated transactions"` should no longer occur.