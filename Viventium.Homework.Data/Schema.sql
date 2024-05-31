
USE master;
SET NOCOUNT ON;

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'Viventium') BEGIN
	declare @tail int
	declare @basefolder nvarchar(max)
	declare @datafile nvarchar(max)
	declare @logfile nvarchar(max)

	set @tail = (
	  select charindex('\',reverse(physical_name))
	  from master.sys.master_files
	  where name = 'master'
	)

	set @basefolder = (select substring(physical_name,1,len(physical_name)-@tail)
	from master.sys.master_files
	where name = 'master')

	set @datafile = @basefolder + '\Viventium.mdf'
	set @logfile = @basefolder + '\Viventium_log.ldf'

	declare @sql nvarchar(MAX)
	set @sql = 'CREATE DATABASE [Viventium] ON PRIMARY ' +
	'( NAME = N''Viventium'', FILENAME = ''' + @datafile + ''', SIZE = 4096KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB ) ' +
	 ' LOG ON ' + 
	'( NAME = N''Viventium_log'', FILENAME = ''' + @logfile + ''', SIZE = 1024KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)'
	EXEC(@sql)
	
	ALTER DATABASE Viventium SET COMPATIBILITY_LEVEL = 100;
	ALTER DATABASE Viventium SET ANSI_NULL_DEFAULT OFF;
	ALTER DATABASE Viventium SET ANSI_NULLS OFF;
	ALTER DATABASE Viventium SET ANSI_PADDING OFF; 
	ALTER DATABASE Viventium SET ANSI_WARNINGS OFF; 
	ALTER DATABASE Viventium SET ARITHABORT OFF; 
	ALTER DATABASE Viventium SET AUTO_CLOSE OFF; 
	ALTER DATABASE Viventium SET AUTO_CREATE_STATISTICS ON; 
	ALTER DATABASE Viventium SET AUTO_SHRINK OFF; 
	ALTER DATABASE Viventium SET AUTO_UPDATE_STATISTICS ON; 
	ALTER DATABASE Viventium SET CURSOR_CLOSE_ON_COMMIT OFF; 
	ALTER DATABASE Viventium SET CURSOR_DEFAULT  GLOBAL; 
	ALTER DATABASE Viventium SET CONCAT_NULL_YIELDS_NULL OFF; 
	ALTER DATABASE Viventium SET NUMERIC_ROUNDABORT OFF; 
	ALTER DATABASE Viventium SET QUOTED_IDENTIFIER OFF; 
	ALTER DATABASE Viventium SET RECURSIVE_TRIGGERS OFF; 
	ALTER DATABASE Viventium SET AUTO_UPDATE_STATISTICS_ASYNC OFF; 
	ALTER DATABASE Viventium SET DATE_CORRELATION_OPTIMIZATION OFF; 
	ALTER DATABASE Viventium SET TRUSTWORTHY OFF; 
	ALTER DATABASE Viventium SET ALLOW_SNAPSHOT_ISOLATION OFF; 
	ALTER DATABASE Viventium SET PARAMETERIZATION SIMPLE; 
	ALTER DATABASE Viventium SET READ_WRITE; 
	ALTER DATABASE Viventium SET RECOVERY SIMPLE; 
	ALTER DATABASE Viventium SET MULTI_USER; 
	ALTER DATABASE Viventium SET PAGE_VERIFY CHECKSUM;  
	ALTER DATABASE Viventium SET DB_CHAINING OFF; 
		
     if(@@error <> 0) begin
       RETURN
     end
END
GO

-- Start the main transaction
BEGIN TRANSACTION initialCreate;

	USE Viventium;

	IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[SchemaVersion]')) 
	BEGIN
		CREATE TABLE [dbo].[SchemaVersion](
			[Id] [uniqueidentifier] NOT NULL default newid(),
			[MajorVersion] [int] NOT NULL,
			[MinorVersion] [int] NOT NULL,
			[InstallDate] [datetimeoffset](7) NOT NULL
		 CONSTRAINT [PK_SchemaVersion] PRIMARY KEY CLUSTERED 
		(
			[Id] ASC
		)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
		) ON [PRIMARY]
	END
	
	if(@@error <> 0)
	begin
		ROLLBACK TRANSACTION;
		RETURN;
	end

	declare @majorVersion int;
	declare @minorVersion int;

	set @majorVersion = 1;
	set @minorVersion = 0;
	if(not(exists(select (1) from SchemaVersion where (MajorVersion = @majorVersion) and (MinorVersion = @minorVersion))))
	BEGIN
	
		CREATE TABLE [dbo].[Employees](
			[CompanyId] [int] IDENTITY(1,1) NOT NULL,
			[CompanyCode] [nvarchar](50) NULL,
			[CompanyDescription] [nvarchar](250) NULL,
			[EmployeeNumber] [nvarchar](250) NULL,
			
			[EmployeeFirstName] [nvarchar](50) NULL,
			[EmployeeLastName] [nvarchar](50) NULL,
			[EmployeeEmail] [nvarchar](50) NULL,
			[EmployeeDepartment] [nvarchar](50) NULL,
			[HireDate] [datetime] NULL,
		    [ManagerEmployeeNumber] [nvarchar](50) NULL,		
		CONSTRAINT [PK_Employees] PRIMARY KEY CLUSTERED 
		(
			[CompanyId] ASC
		)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
		) ON [PRIMARY]
		
		INSERT INTO SchemaVersion values (newid(), @majorVersion, @minorVersion, getutcdate());
	END
	
	if(@@error <> 0)
	begin
		ROLLBACK TRANSACTION;
		RETURN;
	end

COMMIT TRANSACTION initialCreate;


--if(not(exists(select (1) from SchemaVersion where (MajorVersion = 1) and (MinorVersion = 1))))
--BEGIN
--	BEGIN TRANSACTION Version1_1

--		-- Do DDL Work...

--		INSERT INTO SchemaVersion values (newid(), 1, 1, getutcdate());
--	COMMIT TRANSACTION Version1_1
--END

