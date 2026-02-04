## Install MS SQL Server on Ubuntu


### Run Docker container

```bash
docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=YourStrong!Passw0rd" \
   -p 1433:1433 --name sqlserver \
   -v sqlserverdata:/var/opt/mssql \
   -d mcr.microsoft.com/mssql/server:2022-latest
```

### Download packages

```bash
wget https://packages.microsoft.com/ubuntu/22.04/prod/pool/main/m/msodbcsql18/msodbcsql18_18.5.1.1-1_amd64.deb
wget https://packages.microsoft.com/ubuntu/22.04/prod/pool/main/m/mssql-tools18/mssql-tools18_18.4.1.1-1_amd64.deb
```
### Install ODBC driver

```bash
sudo dpkg -i msodbcsql18_18.5.1.1-1_amd64.deb
sudo apt-get install -f -y
```

### Instal SQL Tools

```bash
sudo dpkg -i mssql-tools18_18.4.1.1-1_amd64.deb
sudo apt-get install -f -y
```

### Add Tools to Path


```bash
echo 'export PATH="$PATH:/opt/mssql-tools18/bin"' >> ~/.bashrc
source ~/.bashrc
```


### Test the connection

-C = TrustServerCertificate

```bash
sqlcmd -S 127.0.0.1 -U SA -P "StrongPwd!" -C
```

## Creating DB and tables

### See databases

```sql
SELECT Name FROM sys.Databases;
GO
```

### Create DB, use DB, create table, populate table


```sql
CREATE DATABASE Memory;
USE Memory;
CREATE TABLE Health (Id INT IDENTITY(1,1) PRIMARY KEY, Msg NVARCHAR(50));
INSERT INTO Health (Msg) VALUES ('Hello world from SQL Server DB!');

```

## Connect internally and externally

### Internal connection

```bash
sqlcmd -S 127.0.0.1 -U SA -P "StrongPwd!" -C
```

### External connection

```bash
sqlcmd -S barryonweb.com,1433 -U SA -P "StrongPwd!" -C
```

