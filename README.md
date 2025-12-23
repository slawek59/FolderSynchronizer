# Folder Synchronizer (C#)

Simple one-way folder synchronization tool written in C#.

The application keeps the **replica folder identical to the source folder**.
Synchronization is performed **periodically** and includes file creation, update, deletion and directory structure synchronization.

## Features

- One-way synchronization (source → replica)
- Periodic sync based on configurable interval
- Copying new files
- Updating modified files (size + MD5 checksum comparison)
- Deleting files removed from source
- Creating empty directories
- Removing directories not present in source
- Logging to console and file
- Graceful error handling (locked/missing files)

## Usage

dotnet run -- sourcePath replicaPath logDirectory syncIntervalMs

dotnet run -- "C:\Data\Source" "C:\Data\Replica" "C:\Logs" "1000"
