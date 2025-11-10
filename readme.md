# optimized-restore

This project provides a configuration template for restoring SQL Server databases. It includes connection details and a list of queries that can be run after the restore process is completed.

## Configuration Overview

### SqlServerHost

The SQL Server host where the database is hosted. This should be the server address or name of the SQL Server instance.

- **Example**: `"SqlServerHost": "localhost"`

### RestoreConfigurations

A list of configurations specifying the databases to be restored.

#### Properties:

- **Name**: The name of the restore configuration. This is an identifier for each restore task.
  
- **DatabaseNamePrefix**: An optional prefix that will be applied to the restored database names.

- **QueriesAfterRestore**: A list of SQL queries that will be executed after the restore is complete. This can include any necessary post-restore actions such as setting permissions, updating data, etc.

#### Example:

```json
"RestoreConfigurations": [
  {
    "Name": "MyDatabaseRestore",
    "DatabaseNamePrefix": "prod_",
    "QueriesAfterRestore": [
      "UPDATE users SET active = 1 WHERE last_login > '2025-01-01';",
      "EXEC sp_configure 'show advanced options', 1; RECONFIGURE;"
    ]
  }
]