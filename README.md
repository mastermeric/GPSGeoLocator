# DominosGeoLocator
Inserts GPS coordinates to DB and logs records through RabbitMQ queu

Deployment notes :

- Download the complete repository
- Open "DominosGeoLocator.sln" file through VS2019.
- Rebuild the complete solution inorder to generate binary files/folders and executables.
- Change xxxxxx fields inside  appsettings.json  file for custon DB conenction.
- Run DB script under docs/script.sql file in SQL Server Management Studio (SSMS). (Database design and Indexing is skipped but must be considered in forther optimisations)
- Start Visual studio project by "F5"
- After the execution, There will be RabbitMQ Consumer started to waiting que messages. Then WebAPI will start to inserting 3M records to DB automatically. Afterwards QueConsumer.exe will create "LogFile.txt" log into its bin folder to log all 3M records.
NOTES - known issues:
- For scaling: Project is deployed in Docker container. But it is skipped in this project having kubernetes management in terms of horizontal scaling.
- For documentation : "http://localhost:58438/swagger/index.html" can be used but it sohuld be modified for further/cleaner documentation.
