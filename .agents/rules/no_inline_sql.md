# Stored Procedures over Inline SQL

## Rule Description
When writing backend database queries or interactions, ALWAYS use Stored Procedures and the `ExecutionUnit` class. 

## Rationale
- Standardizes data access across the application.
- Improves security (prevents SQL injection).
- Enhances maintainability by keeping SQL logic inside the database.

## Instructions
- **DO NOT** use inline SQL (e.g., `string query = "SELECT ...";`).
- **DO NOT** use `SqlConnection` or `SqlCommand` directly to write inline SQL.
- **DO** use `ExecutionUnit unit = new ExecutionUnit("SP_Name");` to call a stored procedure.
- **DO** execute the unit via `DatabaseAccessEngine.ExecuteDataReader(unit)` or `DatabaseAccessEngine.ExecuteNonQuery(unit)`.
- If a stored procedure doesn't exist for the task you are doing, write the SQL for the stored procedure and provide it to the user so they can execute it in SQL Server Management Studio (SSMS).
